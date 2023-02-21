/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;
    using UnityEngine;
    using Unity.Collections.LowLevel.Unsafe;
    using Internal;

    /// <summary>
    /// Audio device output that computes the spectral distribution of audio buffers using the Fast Fourier Transform.
    /// </summary>
    public sealed class AudioSpectrumOutput : AudioOutput {

        #region --Client API--        
        /// <summary>
        /// Audio data autospectrum.
        /// This is the unnormalized magnitude of the FFT complex coefficients.
        /// </summary>
        public float[] spectrum { get; private set; }

        /// <summary>
        /// Create an audio spectrum output.
        /// </summary>
        /// <param name="sampleCount">Audio sample count. This is rounded to the closest power of two.</param>
        /// <param name="channel">Audio channel index on which to perform spectrum transformation.</param>
        public AudioSpectrumOutput (int sampleCount = 1024, int channel = 0) {
            sampleCount = Mathf.ClosestPowerOfTwo(sampleCount);
            NatDeviceExt.CreateAudioSpectrumOutput(sampleCount, out output);
            this.channel = channel;
            this.spectrum = new float[sampleCount / 2];
            this.fence = new object();
        }

        /// <summary>
        /// Update the output with a new audio buffer.
        /// </summary>
        /// <param name="audioBuffer">Audio buffer.</param>
        public unsafe override void Update (AudioBuffer audioBuffer) {
            lock (fence) {
                fixed (float* dstSpectrum = spectrum)
                    output.Forward(
                        (float*)audioBuffer.sampleBuffer.GetUnsafeReadOnlyPtr() + channel,
                        audioBuffer.channelCount,
                        audioBuffer.sampleBuffer.Length / audioBuffer.channelCount,
                        dstSpectrum
                    );
                buffer = audioBuffer.Clone();
            }
        }

        /// <summary>
        /// Dispose the output and release resources.
        /// </summary>
        public override void Dispose () {
            lock (fence)
                output.ReleaseAudioSpectrumOutput();
        }
        #endregion


        #region --Operations--
        private readonly IntPtr output;
        private readonly int channel;
        private readonly object fence;
        #endregion
    }
}