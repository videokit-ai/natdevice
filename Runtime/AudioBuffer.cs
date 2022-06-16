/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Internal;

    /// <summary>
    /// Audio buffer provided by an audio device.
    /// The audio buffer always contains a linear PCM sample buffer interleaved by channel.
    /// </summary>
    public readonly struct AudioBuffer {

        #region --Client API--
        /// <summary>
        /// Audio device that this buffer was generated from.
        /// </summary>
        public readonly IMediaDevice<AudioBuffer> device;

        /// <summary>
        /// Audio sample buffer.
        /// </summary>
        public unsafe readonly NativeArray<float> sampleBuffer;

        /// <summary>
        /// Audio buffer sample rate.
        /// </summary>
        public readonly int sampleRate;

        /// <summary>
        /// Audio buffer channel count.
        /// </summary>
        public readonly int channelCount;

        /// <summary>
        /// Audio buffer timestamp in nanoseconds.
        /// The timestamp is based on the system media clock.
        /// </summary>
        public readonly long timestamp;

        /// <summary>
        /// Create an audio buffer.
        /// </summary>
        /// <param name="sampleBuffer">Linear PCM audio sample buffer interleaved by channel.</param>
        /// <param name="sampleRate">Audio buffer sample rate.</param>
        /// <param name="channelCount">Audio buffer channel count.</param>
        /// <param name="timestamp">Audio buffer timestamp in nanoseconds.</param>
        public AudioBuffer (float[] sampleBuffer, int sampleRate, int channelCount, long timestamp) : this(
            new NativeArray<float>(sampleBuffer, Allocator.Temp),
            sampleRate,
            channelCount,
            timestamp
        ) { }

        /// <summary>
        /// Create an audio buffer.
        /// </summary>
        /// <param name="sampleBuffer">Linear PCM audio sample buffer interleaved by channel.</param>
        /// <param name="sampleRate">Audio buffer sample rate.</param>
        /// <param name="channelCount">Audio buffer channel count.</param>
        /// <param name="timestamp">Audio buffer timestamp in nanoseconds.</param>
        public AudioBuffer (NativeArray<float> sampleBuffer, int sampleRate, int channelCount, long timestamp) {
            this.audioBuffer = IntPtr.Zero;
            this.device = null;
            this.sampleBuffer = sampleBuffer;
            this.sampleRate = sampleRate;
            this.channelCount = channelCount;
            this.timestamp = timestamp;
        }
        #endregion


        #region --Operations--
        private readonly IntPtr audioBuffer;

        internal unsafe AudioBuffer (IntPtr audioBuffer, IMediaDevice<AudioBuffer> device) {
            this.audioBuffer = audioBuffer;
            this.device = device;
            this.sampleBuffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(
                audioBuffer.AudioBufferData(),
                audioBuffer.AudioBufferSampleCount(),
                Allocator.None
            );
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref sampleBuffer, AtomicSafetyHandle.Create());
            #endif
            this.sampleRate = audioBuffer.AudioBufferSampleRate();
            this.channelCount = audioBuffer.AudioBufferChannelCount();
            this.timestamp = audioBuffer.AudioBufferTimestamp();
        }

        public static implicit operator IntPtr (AudioBuffer audioBuffer) => audioBuffer.audioBuffer;
        #endregion
    }
}