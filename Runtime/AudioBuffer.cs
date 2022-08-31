/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// Audio buffer provided by an audio device.
    /// The audio buffer always contains a linear PCM sample buffer interleaved by channel.
    /// </summary>
    public readonly struct AudioBuffer {

        #region --Client API--
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
        /// Audio device that this buffer was generated from.
        /// </summary>
        public readonly IMediaDevice<AudioBuffer> device;

        /// <summary>
        /// Create an audio buffer.
        /// </summary>
        /// <param name="sampleBuffer">Linear PCM audio sample buffer interleaved by channel.</param>
        /// <param name="sampleRate">Audio buffer sample rate.</param>
        /// <param name="channelCount">Audio buffer channel count.</param>
        /// <param name="timestamp">Audio buffer timestamp in nanoseconds.</param>
        /// <param name="device">Audio device that generated this audio buffer.</param>
        public AudioBuffer (
            float[] sampleBuffer,
            int sampleRate,
            int channelCount,
            long timestamp,
            IMediaDevice<AudioBuffer> device = null
        ) : this(
            new NativeArray<float>(sampleBuffer, Allocator.Temp),
            sampleRate,
            channelCount,
            timestamp,
            device
        ) { }

        /// <summary>
        /// Create an audio buffer.
        /// </summary>
        /// <param name="sampleBuffer">Linear PCM audio sample buffer interleaved by channel.</param>
        /// <param name="sampleRate">Audio buffer sample rate.</param>
        /// <param name="channelCount">Audio buffer channel count.</param>
        /// <param name="timestamp">Audio buffer timestamp in nanoseconds.</param>
        /// <param name="device">Audio device that generated this audio buffer.</param>
        public AudioBuffer (
            NativeArray<float> sampleBuffer,
            int sampleRate,
            int channelCount,
            long timestamp,
            IMediaDevice<AudioBuffer> device = null
        ) {
            this.sampleBuffer = sampleBuffer;
            this.sampleRate = sampleRate;
            this.channelCount = channelCount;
            this.timestamp = timestamp;
            this.device = device;
        }

        /// <summary>
        /// Create an audio buffer.
        /// </summary>
        /// <param name="sampleBuffer">Linear PCM audio sample buffer interleaved by channel.</param>
        /// <param name="sampleRate">Audio buffer sample rate.</param>
        /// <param name="channelCount">Audio buffer channel count.</param>
        /// <param name="sampleCount">Audio buffer total sample count.</param>
        /// <param name="timestamp">Audio buffer timestamp in nanoseconds.</param>
        /// <param name="device">Audio device that generated this audio buffer.</param>
        public unsafe AudioBuffer (
            float* sampleBuffer,
            int sampleRate,
            int channelCount,
            int sampleCount,
            long timestamp,
            IMediaDevice<AudioBuffer> device = null
        ) {
            this.sampleBuffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(
                sampleBuffer,
                sampleCount,
                Allocator.None
            );
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref this.sampleBuffer, AtomicSafetyHandle.Create());
            #endif
            this.sampleRate = sampleRate;
            this.channelCount = channelCount;
            this.timestamp = timestamp;
            this.device = device;
        }
        #endregion
    }
}