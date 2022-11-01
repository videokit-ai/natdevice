/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Internal;

    public partial struct CameraImage {

        /// <summary>
        /// Image plane for planar formats.
        /// </summary>
        public readonly struct Plane {

            /// <summary>
            /// Pixel buffer.
            /// </summary>
            public readonly NativeArray<byte> buffer;

            /// <summary>
            /// Plane width.
            /// </summary>
            public readonly int width;

            /// <summary>
            /// Plane height.
            /// </summary>
            public readonly int height;

            /// <summary>
            /// Row stride in bytes.
            /// </summary>
            public readonly int rowStride;
            
            /// <summary>
            /// Pixel stride in bytes.
            /// </summary>
            public readonly int pixelStride;

            /// <summary>
            /// Create a camera image plane.
            /// </summary>
            public Plane (NativeArray<byte> buffer, int width, int height, int rowStride, int pixelStride) {
                this.buffer = buffer;
                this.width = width;
                this.height = height;
                this.rowStride = rowStride;
                this.pixelStride = pixelStride;
            }
        }
    }
}