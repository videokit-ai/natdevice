/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using System;
    using Unity.Collections;
    using Internal;
    using MetadataKey = Internal.NatDevice.MetadataKey;

    /// <summary>
    /// Camera image provided by a camera device.
    /// The camera image always contains a pixel buffer along with image metadata.
    /// The format of the pixel buffer varies by platform and must be taken into consideration when using the pixel data.
    /// </summary>
    public readonly partial struct CameraImage {

        #region --Types--
        /// <summary>
        /// Image buffer format.
        /// </summary>
        public enum Format { // CHECK // NatDevice.h
            /// <summary>
            /// Unknown image format.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Generic YUV 420 planar format.
            /// </summary>
            YCbCr420 = 1,
            /// <summary>
            /// RGBA8888.
            /// </summary>
            RGBA8888 = 2,
            /// <summary>
            /// BGRA8888.
            /// </summary>
            BGRA8888 = 3,    
        }
        #endregion


        #region --Properties--
        /// <summary>
        /// Camera device that this image was generated from.
        /// </summary>
        public readonly IMediaDevice<CameraImage> device;

        /// <summary>
        /// Image pixel buffer.
        /// Some planar images might not have a contiguous pixel buffer.
        /// In this case, the buffer is uninitialized.
        /// </summary>
        public readonly NativeArray<byte> pixelBuffer;

        /// <summary>
        /// Image format.
        /// </summary>
        public readonly Format format;

        /// <summary>
        /// Image width.
        /// </summary>
        public readonly int width;

        /// <summary>
        /// Image height.
        /// </summary>
        public readonly int height;

        /// <summary>
        /// Image row stride in bytes.
        /// This is zero if the image is planar.
        /// </summary>
        public readonly int rowStride;

        /// <summary>
        /// Image timestamp in nanoseconds.
        /// The timestamp is based on the system media clock.
        /// </summary>
        public readonly long timestamp;

        /// <summary>
        /// Whether the image is vertically mirrored.
        /// </summary>
        public readonly bool verticallyMirrored;

        /// <summary>
        /// Image plane for planar formats.
        /// This is `null` for interleaved formats.
        /// </summary>
        public readonly Plane[] planes;

        /// <summary>
        /// Camera intrinsics as a flattened row-major 3x3 matrix.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public readonly float[] intrinsics;

        /// <summary>
        /// Exposure bias value in EV.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public readonly float? exposureBias;

        /// <summary>
        /// Image exposure duration in seconds.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public readonly float? exposureDuration;

        /// <summary>
        /// Sensor sensitivity ISO value.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public readonly float? ISO;

        /// <summary>
        /// Camera focal length in millimeters.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public readonly float? focalLength;

        /// <summary>
        /// Image aperture, in f-number.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public readonly float? fNumber;

        /// <summary>
        /// Ambient brightness.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public readonly float? brightness;

        /// <summary>
        /// Create a camera image.
        /// </summary>
        public CameraImage (
            IMediaDevice<CameraImage> device,
            NativeArray<byte> pixelBuffer,
            Format format,
            int width,
            int height,
            int rowStride,
            long timestamp,
            bool mirrored,
            Plane[] planes = null,
            float[] intrinsics = null,
            float? exposureBias = null,
            float? exposureDuration = null,
            float? ISO = null,
            float? focalLength = null,
            float? fNumber = null,
            float? brightness = null
        ) {
            this.device = device;
            this.pixelBuffer = pixelBuffer;
            this.format = format;
            this.width = width;
            this.height = height;
            this.rowStride = rowStride;
            this.timestamp = timestamp;
            this.verticallyMirrored = mirrored;
            this.planes = planes;
            this.intrinsics = intrinsics;
            this.exposureBias = exposureBias;
            this.exposureDuration = exposureDuration;
            this.ISO = ISO;
            this.focalLength = focalLength;
            this.fNumber = fNumber;
            this.brightness = brightness;
        }
        #endregion
    }
}