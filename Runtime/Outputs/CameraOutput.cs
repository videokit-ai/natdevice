/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;

    /// <summary>
    /// </summary>
    public abstract class CameraOutput : IMediaOutput<CameraImage> {

        #region --Client API--
        /// <summary>
        /// Update the output with a new camera image.
        /// </summary>
        /// <param name="cameraImage">Camera image.</param>
        public abstract void Update (CameraImage cameraImage);

        /// <summary>
        /// Dispose the camera output and release resources.
        /// </summary>
        public virtual void Dispose () {}
        #endregion


        #region --Operations--
        public static implicit operator Action<CameraImage> (CameraOutput output) => output.Update;
        #endregion
    }
}