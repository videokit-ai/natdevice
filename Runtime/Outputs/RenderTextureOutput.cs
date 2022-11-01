/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Internal;

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
        public sealed class ConversionOptions : PixelBufferOutput.ConversionOptions { }

        /// <summary>
        /// Texture containing the latest camera image.
        /// </summary>
        public RenderTexture texture { get; private set; }

        /// <summary>
        /// Texture timestamp.
        /// This is the timestamp of the image in the render texture.
        /// </summary>
        public long timestamp { get; private set; }

        /// <summary>
        /// Get or set the pixel buffer orientation.
        /// </summary>
        public ScreenOrientation orientation;

        /// <summary>
        /// Event raised when a new camera image is available in the texture output.
        /// </summary>
        public event Action onFrame;

        /// <summary>
        /// Create a RenderTexture output.
        /// </summary>
        public RenderTextureOutput () {
            // Check
            if (!SystemInfo.supportsComputeShaders)
                throw new InvalidOperationException(@"RenderTextureOutput can only be used on platforms that support compute shaders");
            // Retrieve kernels
            this.shader = Resources.Load(@"RenderTextureOutput") as ComputeShader;
            this.lifecycleHelper = LifecycleHelper.Create();
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
            this.fence = new object();
            // Upload
            lifecycleHelper.onUpdate += OnFrame;
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
            lock (fence) {
                var bufferSize = image.width * image.height * 4 + 16 * image.height; // Overallocate in case of padding
                pixelBuffer = pixelBuffer?.Length == bufferSize ? pixelBuffer : null;
                pixelBuffer ??= new byte[bufferSize];
                CopyImage(image, pixelBuffer, conversionOffset, conversionStride);
                imageBuffer = new ImageBuffer {
                    pixelBuffer = pixelBuffer,
                    width = image.width,
                    height = image.height,
                    format = image.format,
                    timestamp = image.timestamp,
                    mirror = options?.mirror ?? image.verticallyMirrored,
                    orientation = options?.orientation ?? this.orientation,
                };
            }
        }

        /// <summary>
        /// Dispose the RenderTexture output and release resources.
        /// </summary>
        public override void Dispose () {
            lock (fence) {
                if (texture)
                    texture.Release();
                if (lifecycleHelper)
                    lifecycleHelper.Dispose();
                RenderTexture.Destroy(texture);
                conversionBuffer?.Dispose();
                texture = null;
                pixelBuffer = null;
                conversionBuffer = null;
            }
        }
        #endregion


        #region --Operations--
        private readonly ComputeShader shader;
        private readonly LifecycleHelper lifecycleHelper;
        private readonly IReadOnlyDictionary<CameraImage.Format, int> conversionKernelMap;
        private readonly IReadOnlyDictionary<ScreenOrientation, int> rotationKernelMap;
        private readonly int[] conversionOffset;
        private readonly int[] conversionStride;
        private readonly object fence;
        private byte[] pixelBuffer;
        private ImageBuffer imageBuffer;
        private ComputeBuffer conversionBuffer;
        private static readonly RuntimePlatform[] OrientationSupport = new [] {
            RuntimePlatform.Android,
            RuntimePlatform.IPhonePlayer
        };

        private struct ImageBuffer {
            public byte[] pixelBuffer;
            public int width;
            public int height;
            public CameraImage.Format format;
            public long timestamp;
            public bool mirror;
            public ScreenOrientation orientation;
        }

        private void OnFrame () {
            ImageBuffer imageBuffer;
            lock (fence) {
                // Check dirty
                imageBuffer = this.imageBuffer;
                if (timestamp == imageBuffer.timestamp)
                    return;
                // Check buffer
                if (conversionBuffer != null && conversionBuffer.count * conversionBuffer.stride != imageBuffer.pixelBuffer.Length) {
                    conversionBuffer.Dispose();
                    conversionBuffer = null;
                }
                // Upload
                conversionBuffer ??= new ComputeBuffer(imageBuffer.pixelBuffer.Length / sizeof(int), sizeof(int), ComputeBufferType.Raw, ComputeBufferMode.Immutable);
                conversionBuffer.SetData(imageBuffer.pixelBuffer);
            }
            // Convert
            var imageDescriptor = new RenderTextureDescriptor(imageBuffer.width, imageBuffer.height, RenderTextureFormat.ARGB32, 0);
            imageDescriptor.enableRandomWrite = true;
            var convertedTexture = RenderTexture.GetTemporary(imageDescriptor);
            var conversionKernel = conversionKernelMap[imageBuffer.format];
            shader.SetBuffer(conversionKernel, @"Input", conversionBuffer);
            shader.SetTexture(conversionKernel, @"Result", convertedTexture);
            shader.SetInts(@"Offset", conversionOffset);
            shader.SetInts(@"Stride", conversionStride);
            shader.SetBool(@"Mirror", imageBuffer.mirror);
            shader.GetKernelThreadGroupSizes(conversionKernel, out var gx, out var gy, out var _);
            shader.Dispatch(conversionKernel, Mathf.CeilToInt((float)imageBuffer.width / gx), Mathf.CeilToInt((float)imageBuffer.height / gy), 1);
            // Create
            var portrait = imageBuffer.orientation == ScreenOrientation.Portrait || imageBuffer.orientation == ScreenOrientation.PortraitUpsideDown;
            var width = portrait ? imageBuffer.height : imageBuffer.width;
            var height = portrait ? imageBuffer.width : imageBuffer.height;
            var descriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
            descriptor.enableRandomWrite = true;
            texture = texture ? texture : new RenderTexture(descriptor);
            if (texture.width != width || texture.height != height) {
                texture.Release();
                texture.width = width;
                texture.height = height;
            }
            // Rotate
            if (rotationKernelMap.TryGetValue(imageBuffer.orientation, out var rotationKernel)) {
                shader.SetTexture(rotationKernel, @"Image", convertedTexture);
                shader.SetTexture(rotationKernel, @"Result", texture);
                shader.GetKernelThreadGroupSizes(rotationKernel, out var rx, out var ry, out var _);
                shader.Dispatch(rotationKernel, Mathf.CeilToInt((float)width / rx), Mathf.CeilToInt((float)height / ry), 1);
            }
            else
                Graphics.Blit(convertedTexture, texture);
            // Release
            RenderTexture.ReleaseTemporary(convertedTexture);
            timestamp = imageBuffer.timestamp;
            onFrame?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyImage (in CameraImage image, byte[] buffer, int[] offset, int[] stride) {
            var i420 = image.planes != null && image.planes[1].pixelStride == 1;
            switch (image.format) {
                case CameraImage.Format.YCbCr420 when i420:
                    CopyImageYUV420p(image, buffer, offset, stride);
                    break;
                case CameraImage.Format.YCbCr420 when !i420:
                    CopyImageYUV420sp(image, buffer, offset, stride);
                    break;
                case CameraImage.Format.BGRA8888:
                case CameraImage.Format.RGBA8888:
                    CopyImageRGBA8888(image, buffer, offset, stride);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyImageRGBA8888 (in CameraImage image, byte[] buffer, int[] offset, int[] stride) {
            NativeArray<byte>.Copy(image.pixelBuffer, buffer, image.pixelBuffer.Length);
            Array.Clear(offset, 0, offset.Length);
            stride[0] = image.pixelBuffer.Length / image.height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyImageYUV420p (in CameraImage image, byte[] buffer, int[] offset, int[] stride) {
            var yBuffer = image.planes[0].buffer;
            var cbBuffer = image.planes[1].buffer;
            var crBuffer = image.planes[2].buffer;
            NativeArray<byte>.Copy(yBuffer, buffer, yBuffer.Length);
            NativeArray<byte>.Copy(cbBuffer, 0, buffer, yBuffer.Length, cbBuffer.Length);
            NativeArray<byte>.Copy(crBuffer, 0, buffer, yBuffer.Length + cbBuffer.Length, crBuffer.Length);
            offset[0] = 0;
            offset[1] = yBuffer.Length;
            offset[2] = yBuffer.Length + cbBuffer.Length;
            offset[3] = 0;
            stride[0] = image.planes[0].rowStride;
            stride[1] = image.planes[1].rowStride;
            stride[2] = image.planes[0].pixelStride;
            stride[3] = image.planes[1].pixelStride;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void CopyImageYUV420sp (in CameraImage image, byte[] buffer, int[] offset, int[] stride) {
            var nv21 = image.planes.Length > 2 && image.planes[1].buffer.GetUnsafePtr() > image.planes[2].buffer.GetUnsafePtr();
            var yBuffer = image.planes[0].buffer;
            var cbcrBuffer = nv21 ? image.planes[2].buffer : image.planes[1].buffer;
            var cbcrLength = cbcrBuffer.Length + (cbcrBuffer.Length % 2 != 0 ? 1 : 0);
            NativeArray<byte>.Copy(yBuffer, buffer, yBuffer.Length);
            fixed (byte* dstBuffer = buffer)
                UnsafeUtility.MemCpy(dstBuffer + yBuffer.Length, cbcrBuffer.GetUnsafePtr(), cbcrLength);
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