/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
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
        public unsafe readonly NativeArray<byte> pixelBuffer {
            get {
                var pixelBuffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(
                    image.CameraImageData(),
                    image.CameraImageDataSize(),
                    Allocator.None
                );
                #if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref pixelBuffer, AtomicSafetyHandle.Create());
                #endif
                return pixelBuffer;
            }
        }

        /// <summary>
        /// Image width.
        /// </summary>
        public readonly int width => image.CameraImageWidth();

        /// <summary>
        /// Image height.
        /// </summary>
        public readonly int height => image.CameraImageHeight();

        /// <summary>
        /// Image row stride in bytes.
        /// This is zero if the image is planar.
        /// </summary>
        public readonly int rowStride => image.CameraImageRowStride();

        /// <summary>
        /// Image timestamp in nanoseconds.
        /// The timestamp is based on the system media clock.
        /// </summary>
        public readonly long timestamp => image.CameraImageTimestamp();

        /// <summary>
        /// Image format.
        /// </summary>
        public readonly Format format => image.CameraImageFormat();

        /// <summary>
        /// Whether the image is vertically mirrored.
        /// </summary>
        public readonly bool verticallyMirrored => image.CameraImageVerticallyMirrored();

        /// <summary>
        /// Image plane for planar formats.
        /// This is `null` for interleaved formats.
        /// </summary>
        public readonly Plane[] planes;

        /// <summary>
        /// Camera intrinsics as a flattened row-major 3x3 matrix.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public unsafe readonly float[] intrinsics {
            get {
                var result = new float[9];
                fixed (float* dst = result)
                    return image.CameraImageMetadata(MetadataKey.IntrinsicMatrix, dst, result.Length) ? result : null;
            }
        }

        /// <summary>
        /// Exposure bias value in EV.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public unsafe readonly float? exposureBias {
            get {
                var result = 0f;
                return image.CameraImageMetadata(MetadataKey.ExposureBias, &result) ? (float?)result : null;
            }
        }

        /// <summary>
        /// Image exposure duration in seconds.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public unsafe readonly float? exposureDuration {
            get {
                var result = 0f;
                return image.CameraImageMetadata(MetadataKey.ExposureDuration, &result) ? (float?)result : null;
            }
        }

        /// <summary>
        /// Sensor sensitivity ISO value.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public unsafe readonly float? ISO {
            get {
                var result = 0f;
                return image.CameraImageMetadata(MetadataKey.ISO, &result) ? (float?)result : null;
            }
        }

        /// <summary>
        /// Camera focal length in millimeters.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public unsafe readonly float? focalLength {
            get {
                var result = 0f;
                return image.CameraImageMetadata(MetadataKey.FocalLength, &result) ? (float?)result : null;
            }
        }

        /// <summary>
        /// Image aperture, in f-number.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public unsafe readonly float? fNumber {
            get {
                var result = 0f;
                return image.CameraImageMetadata(MetadataKey.FNumber, &result) ? (float?)result : null;
            }
        }

        /// <summary>
        /// Ambient brightness.
        /// This is `null` if the camera does not report it.
        /// </summary>
        public unsafe readonly float? brightness {
            get {
                var result = 0f;
                return image.CameraImageMetadata(MetadataKey.Brightness, &result) ? (float?)result : null;;
            }
        }
        #endregion


        #region --Operations--
        private readonly IntPtr image;

        internal CameraImage (IMediaDevice<CameraImage> device, IntPtr image) {
            this.device = device;
            this.image = image;
            // Get planes up front to prevent GC on access
            var planeCount = image.CameraImagePlaneCount();
            this.planes = planeCount > 0 ? new Plane[planeCount] : null;
            if (planeCount > 0)
                for (var i = 0; i < planeCount; ++i)
                    planes[i] = new Plane(image, i);
        }

        public static implicit operator IntPtr (CameraImage image) => image.image;
        #endregion
    }
}