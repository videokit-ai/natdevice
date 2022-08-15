/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;

    /// <summary>
    /// </summary>
    public abstract class AudioOutput : IMediaOutput<AudioBuffer> {

        #region --Client API--
        /// <summary>
        /// Update the output with a new audio buffer.
        /// </summary>
        /// <param name="audioBuffer">Audio buffer.</param>
        public abstract void Update (AudioBuffer audioBuffer);

        /// <summary>
        /// Dispose the audio output and release resources.
        /// </summary>
        public virtual void Dispose () {}
        #endregion


        #region --Operations--
        /// <summary>
        /// Implicitly convert the output to an audio buffer delegate.
        /// </summary>
        public static implicit operator Action<AudioBuffer> (AudioOutput output) => output.Update;
        #endregion
    }
}