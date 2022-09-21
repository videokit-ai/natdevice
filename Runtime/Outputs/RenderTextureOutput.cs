/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// Camera device output that streams camera images into a `RenderTexture` for display.
    /// The render texture output performs necessary conversions entirely on the GPU.
    /// This output can provide better performance than the `TextureOutput` when pixel data is not accessed on the CPU.
    /// </summary>
    public sealed class RenderTextureOutput : CameraOutput {

        #region --Client API--
        /// <summary>
        /// RenderTexture conversion options.
        /// </summary>
        public class ConversionOptions : PixelBufferOutput.ConversionOptions { }

        /// <summary>
        /// Texture containing the latest camera image.
        /// </summary>
        public RenderTexture texture { get; private set; }

        /// <summary>
        /// Get or set the pixel buffer orientation.
        /// </summary>
        public ScreenOrientation orientation;

        /// <summary>
        /// Create a RenderTexture output.
        /// </summary>
        public RenderTextureOutput () {
            // Check
            if (!SystemInfo.supportsComputeShaders)
                throw new InvalidOperationException(@"RenderTextureOutput can only be used on platforms that support compute shaders");
            // Retrieve kernels
            this.shader = Resources.Load(@"RenderTextureOutput") as ComputeShader;
            this.orientation = Array.IndexOf(OrientationSupport, Application.platform) >= 0 ? Screen.orientation : 0;
            this.conversionKernelMap = new Dictionary<CameraImage.Format, int> {
                [CameraImage.Format.YCbCr420] = shader.FindKernel(@"ConvertYUV420"),
                [CameraImage.Format.RGBA8888] = shader.FindKernel(@"ConvertRGBA8888"),
                [CameraImage.Format.BGRA8888] = shader.FindKernel(@"ConvertBGRA8888"),
            };
            this.rotationKernelMap = new Dictionary<ScreenOrientation, int> {
                [ScreenOrientation.Portrait]            = shader.FindKernel(@"Rotate90"),
                [ScreenOrientation.LandscapeRight]      = shader.FindKernel(@"Rotate180"),
                [ScreenOrientation.PortraitUpsideDown]  = shader.FindKernel(@"Rotate270"),
            };
            this.conversionOffset = new int[4];
            this.conversionStride = new int[4];
        }

        /// <summary>
        /// Update the output with a new camera image.
        /// </summary>
        /// <param name="image">Camera image.</param>
        public override void Update (CameraImage image) => Update(image, null);

        /// <summary>
        /// Update the output with a new camera image.
        /// </summary>
        /// <param name="image">Camera image.</param>
        /// <param name="options">Conversion options.</param>
        public void Update (CameraImage image, ConversionOptions options) {
            // Create
            var imageBufferSize = image.width * image.height * 4 + 16 * image.height; // Overallocate in case of padding
            conversionBuffer ??= new ComputeBuffer(imageBufferSize / sizeof(int), sizeof(int), ComputeBufferType.Raw, ComputeBufferMode.Immutable);
            var conversionBufferSize = conversionBuffer.count * conversionBuffer.stride;
            // Check
            if (conversionBufferSize != imageBufferSize)
                throw new ArgumentException($"RenderTextureOutput received image with size {imageBufferSize} but expected {conversionBufferSize}");         
            // Check options
            options ??= new ConversionOptions {
                orientation = orientation,
                mirror = image.verticallyMirrored
            };
            // Convert
            var convertedTextureDescriptor = new RenderTextureDescriptor(image.width, image.height, RenderTextureFormat.ARGB32, 0);
            convertedTextureDescriptor.enableRandomWrite = true;
            var convertedTexture = RenderTexture.GetTemporary(convertedTextureDescriptor);
            var conversionKernel = conversionKernelMap[image.format];
            UploadImage(image, conversionBuffer, conversionOffset, conversionStride);
            shader.SetBuffer(conversionKernel, @"Input", conversionBuffer);
            shader.SetInts(@"Offset", conversionOffset);
            shader.SetInts(@"Stride", conversionStride);
            shader.SetBool(@"Mirror", options.mirror);
            shader.SetTexture(conversionKernel, @"Result", convertedTexture);
            shader.GetKernelThreadGroupSizes(conversionKernel, out var gx, out var gy, out var _);
            shader.Dispatch(conversionKernel, Mathf.CeilToInt((float)image.width / gx), Mathf.CeilToInt((float)image.height / gy), 1);
            // Calculate destination size
            var portrait = options.orientation == ScreenOrientation.Portrait || options.orientation == ScreenOrientation.PortraitUpsideDown;
            var width = portrait ? image.height : image.width;
            var height = portrait ? image.width : image.height;
            // Create
            var descriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
            descriptor.enableRandomWrite = true;
            texture = texture ? texture : new RenderTexture(descriptor);
            // Resize
            if (texture.width != width || texture.height != height) {
                texture.Release();
                texture.width = width;
                texture.height = height;
            }
            // Rotate
            if (rotationKernelMap.TryGetValue(options.orientation, out var rotationKernel)) {
                shader.SetTexture(rotationKernel, @"Image", convertedTexture);
                shader.SetTexture(rotationKernel, @"Result", texture);
                shader.GetKernelThreadGroupSizes(rotationKernel, out var rx, out var ry, out var _);
                shader.Dispatch(rotationKernel, Mathf.CeilToInt((float)width / rx), Mathf.CeilToInt((float)height / ry), 1);
            }
            else
                Graphics.Blit(convertedTexture, texture);
            RenderTexture.ReleaseTemporary(convertedTexture);
        }

        /// <summary>
        /// Dispose the RenderTexture output and release resources.
        /// </summary>
        public override void Dispose () {
            if (texture)
                texture.Release();
            RenderTexture.Destroy(texture);
            conversionBuffer?.Dispose();
            texture = null;
        }
        #endregion


        #region --Operations--
        private readonly ComputeShader shader;
        private readonly IReadOnlyDictionary<CameraImage.Format, int> conversionKernelMap;
        private readonly IReadOnlyDictionary<ScreenOrientation, int> rotationKernelMap;
        private readonly int[] conversionOffset;
        private readonly int[] conversionStride;
        private ComputeBuffer conversionBuffer;
        private static readonly RuntimePlatform[] OrientationSupport = new [] {
            RuntimePlatform.Android,
            RuntimePlatform.IPhonePlayer
        };

        private static void UploadImage (CameraImage image, ComputeBuffer buffer, int[] offset, int[] stride) {
            var i420 = image.planes != null && image.planes[1].pixelStride == 1;
            switch (image.format) {
                case CameraImage.Format.YCbCr420 when i420:
                    UploadImageYUV420p(image, buffer, offset, stride);
                    break;
                case CameraImage.Format.YCbCr420 when !i420:
                    UploadImageYUV420sp(image, buffer, offset, stride);
                    break;
                case CameraImage.Format.BGRA8888:
                case CameraImage.Format.RGBA8888:
                    UploadImageRGBA8888(image, buffer, offset, stride);
                    break;
            }
        }

        private static void UploadImageRGBA8888 (CameraImage image, ComputeBuffer buffer, int[] offset, int[] stride) {
            buffer.SetData(image.pixelBuffer);
            Array.Clear(offset, 0, offset.Length);
            stride[0] = image.pixelBuffer.Length / image.height;
        }

        private static void UploadImageYUV420p (CameraImage image, ComputeBuffer buffer, int[] offset, int[] stride) {
            var yBuffer = image.planes[0].buffer;
            var cbBuffer = image.planes[1].buffer;
            var crBuffer = image.planes[2].buffer;
            buffer.SetData(yBuffer, 0, 0, yBuffer.Length);
            buffer.SetData(cbBuffer, 0, yBuffer.Length, cbBuffer.Length);
            buffer.SetData(crBuffer, 0, yBuffer.Length + cbBuffer.Length, crBuffer.Length);
            offset[0] = 0;
            offset[1] = yBuffer.Length;
            offset[2] = yBuffer.Length + cbBuffer.Length;
            offset[3] = 0;
            stride[0] = image.planes[0].rowStride;
            stride[1] = image.planes[1].rowStride;
            stride[2] = image.planes[0].pixelStride;
            stride[3] = image.planes[1].pixelStride;
        }

        private static unsafe void UploadImageYUV420sp (CameraImage image, ComputeBuffer buffer, int[] offset, int[] stride) {
            var nv21 = image.planes.Length > 2 && image.planes[1].buffer.GetUnsafePtr() > image.planes[2].buffer.GetUnsafePtr();
            var yBuffer = image.planes[0].buffer;
            var cbcrBuffer = nv21 ? image.planes[2].buffer : image.planes[1].buffer;
            if (cbcrBuffer.Length % 2 != 0) {
                cbcrBuffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(
                    cbcrBuffer.GetUnsafePtr(),
                    cbcrBuffer.Length + 1,
                    Allocator.None
                );
                #if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref cbcrBuffer, AtomicSafetyHandle.Create());
                #endif
            }
            buffer.SetData(yBuffer, 0, 0, yBuffer.Length);
            buffer.SetData(cbcrBuffer, 0, yBuffer.Length, cbcrBuffer.Length);
            offset[0] = 0;
            offset[1] = nv21 ? yBuffer.Length - 1 : yBuffer.Length;
            offset[2] = nv21 ? yBuffer.Length : yBuffer.Length + 1;
            offset[3] = 0;
            stride[0] = image.planes[0].rowStride;
            stride[1] = image.planes[1].rowStride;
            stride[2] = image.planes[0].pixelStride;
            stride[3] = image.planes[1].pixelStride;
        }
        #endregion
    }
}