/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;
    using System.Runtime.CompilerServices;
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
        public float[] spectrum {
            [MethodImpl(MethodImplOptions.Synchronized)] get;
            private set;
        }

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
        }

        /// <summary>
        /// Update the output with a new audio buffer.
        /// </summary>
        /// <param name="audioBuffer">Audio buffer.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public unsafe override void Update (AudioBuffer audioBuffer) {
            fixed (float* dstSpectrum = spectrum)
                output.Forward(
                    (float*)audioBuffer.sampleBuffer.GetUnsafeReadOnlyPtr() + channel,
                    audioBuffer.channelCount,
                    audioBuffer.sampleBuffer.Length / audioBuffer.channelCount,
                    dstSpectrum
                );
        }

        /// <summary>
        /// Dispose the output and release resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Dispose () => output.ReleaseAudioSpectrumOutput();
        #endregion


        #region --Operations--
        private readonly IntPtr output;
        private readonly int channel;
        #endregion
    }
}