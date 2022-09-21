/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using Unity.Collections;
    using Devices;

    /// <summary>
    /// Audio device output that accumulates audio buffers into an `AudioClip`.
    /// </summary>
    public sealed class AudioClipOutput : AudioOutput {

        #region --Client API--
        /// <summary>
        /// Create an audio clip output.
        /// </summary>
        public AudioClipOutput () => this.audioBuffer = new MemoryStream();

        /// <summary>
        /// Update the output with a new audio buffer.
        /// </summary>
        /// <param name="buffer">Audio buffer.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Update (AudioBuffer buffer) {
            sampleRate = sampleRate == 0 ? buffer.sampleRate : sampleRate;
            channelCount = channelCount == 0 ? buffer.channelCount : channelCount;
            var audioData = new NativeSlice<float>(buffer.sampleBuffer).SliceConvert<byte>().ToArray();
            audioBuffer.Write(audioData, 0, audioData.Length);
        }

        /// <summary>
        /// Dispose the output and release resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Dispose () => audioBuffer.Dispose();

        /// <summary>
        /// Get the current clip containing all audio recorded up till this point.
        /// Note that this clip DOES NOT stream new audio that is provided to the output.
        /// </summary>
        public AudioClip ToClip () {
            // Check
            if (sampleRate == 0 || channelCount == 0)
                return null;
            // Get the full sample buffer
            var byteSamples = audioBuffer.ToArray();
            var totalSampleCount = byteSamples.Length / sizeof(float); 
            var sampleBuffer = new float[totalSampleCount];  
            var recordingName = string.Format("recording_{0}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff"));
            Buffer.BlockCopy(byteSamples, 0, sampleBuffer, 0, byteSamples.Length);
            // Create audio clip
            var audioClip = AudioClip.Create(recordingName, totalSampleCount / channelCount, channelCount, sampleRate, false);
            audioClip.SetData(sampleBuffer, 0);
            return audioClip;
        }
        #endregion


        #region --Operations--
        private readonly MemoryStream audioBuffer;
        private int sampleRate, channelCount;
        #endregion
    }
}