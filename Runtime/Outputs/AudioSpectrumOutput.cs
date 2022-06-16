/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;
    using System.Runtime.CompilerServices;
    using Unity.Collections.LowLevel.Unsafe;
    using Internal;

    /// <summary>
    /// Audio device output that computes the spectral distribution of audio buffers using the Fast Fourier Transform.
    /// </summary>
    internal sealed class AudioSpectrumOutput : IDisposable { // INCOMPLETE

        #region --Client API--
        /// <summary>
        /// DFT real value coefficients.
        /// </summary>
        public readonly float[] reals;

        /// <summary>
        /// DFT imaginary value coefficients.
        /// </summary>
        public readonly float[] imaginaries;
        
        /// <summary>
        /// Spectrum magnitude.
        /// </summary>
        public readonly float[] spectrum;

        /// <summary>
        /// Create an audio spectrum output.
        /// </summary>
        /// <param name="sampleCount">Number of samples. This is rounded to the next power of two.</param>
        /// <param name="channel">Audio channel on which to perform spectrum transformation.</param>
        public AudioSpectrumOutput (int sampleCount = 1024, int channel = 0) {
            NatDeviceExt.CreateAudioSpectrumOutput(sampleCount, out output);
            this.channel = channel;
            this.reals = new float[sampleCount / 2];
            this.imaginaries = new float[sampleCount / 2];
            this.spectrum = new float[sampleCount / 2];
        }

        /// <summary>
        /// Update the output with a new audio buffer.
        /// </summary>
        /// <param name="audioBuffer">Audio buffer.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public unsafe void Update (AudioBuffer audioBuffer) {
            fixed (float* dstReals = reals, dstImaginaries = imaginaries, dstSpectrum = spectrum)
                output.Forward(
                    (float*)audioBuffer.sampleBuffer.GetUnsafeReadOnlyPtr(),
                    channel,
                    audioBuffer.channelCount,
                    audioBuffer.sampleBuffer.Length / audioBuffer.channelCount,
                    dstReals,
                    dstImaginaries,
                    dstSpectrum
                );
        }

        /// <summary>
        /// Dispose the output and release resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose () => output.ReleaseAudioSpectrumOutput();
        #endregion


        #region --Operations--
        private readonly IntPtr output;
        private readonly int channel;

        public static implicit operator Action<AudioBuffer> (AudioSpectrumOutput output) => output.Update;
        #endregion
    }
}