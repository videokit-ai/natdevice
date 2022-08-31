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
    using Internal;

    /// <summary>
    /// Camera device output that converts camera images into RGBA8888 pixel buffers.
    /// </summary>
    public sealed class PixelBufferOutput : CameraOutput {

        #region --Client API--
        /// <summary>
        /// Pixel buffer conversion options.
        /// </summary>
        public class ConversionOptions {
            /// <summary>
            /// Desired pixel buffer orientation.
            /// </summary>
            public ScreenOrientation orientation;
            /// <summary>
            /// Whether to vertically mirror the pixel buffer.
            /// </summary>
            public bool mirror;
        }

        /// <summary>
        /// Pixel buffer with latest camera image.
        /// The pixel buffer is always laid out in RGBA8888 format.
        /// </summary>
        public NativeArray<byte> pixelBuffer { get; private set; }

        /// <summary>
        /// Pixel buffer width.
        /// </summary>
        public int width { get; private set; }

        /// <summary>
        /// Pixel buffer height.
        /// </summary>
        public int height { get; private set; }

        /// <summary>
        /// Get or set the pixel buffer orientation.
        /// </summary>
        public ScreenOrientation orientation;

        /// <summary>
        /// Create a pixel buffer output.
        /// </summary>
        public PixelBufferOutput () {
            this.orientation = OrientationSupport.Contains(Application.platform) ? Screen.orientation : 0;
            this.lifecycleHelper = LifecycleHelper.Create();
            lifecycleHelper.onQuit += Dispose;
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
        public unsafe void Update (CameraImage image, ConversionOptions options) {
            // Create
            var bufferSize = image.width * image.height * 4;
            if (!pixelBuffer.IsCreated)
                pixelBuffer = new NativeArray<byte>(bufferSize, Allocator.Persistent);
            // Check
            if (pixelBuffer.Length != bufferSize)
                throw new ArgumentException($"PixelBufferOutput received image with size {bufferSize} but expected {pixelBuffer.Length}");
            // Create temp buffer
            if (!tempBuffer.IsCreated)
                tempBuffer = new NativeArray<byte>(image.width * image.height * 4, Allocator.Persistent);
            // Check options
            options ??= new ConversionOptions {
                orientation = orientation,
                mirror = image.verticallyMirrored
            };
            // Convert
            NatDeviceExt.Convert(
                image,
                (int)options.orientation,
                options.mirror,
                tempBuffer.GetUnsafePtr(),
                pixelBuffer.GetUnsafePtr(),
                out var width,
                out var height
            );
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Dispose the pixel buffer output and release resources.
        /// </summary>
        public override void Dispose () {
            lifecycleHelper?.Dispose();
            try { pixelBuffer.Dispose(); } catch (ObjectDisposedException) { } // Checking `IsCreated` doesn't prevent this
            try { tempBuffer.Dispose(); } catch (ObjectDisposedException) { }
            lifecycleHelper = null;
        }
        #endregion


        #region --Operations--
        private NativeArray<byte> tempBuffer;
        private LifecycleHelper lifecycleHelper;
        private static readonly List<RuntimePlatform> OrientationSupport = new List<RuntimePlatform> {
            RuntimePlatform.Android,
            RuntimePlatform.IPhonePlayer
        };
        #endregion
    }
}