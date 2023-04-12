/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
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
        /// <param name="device">Audio device that generated this audio buffer.</param>
        /// <param name="sampleBuffer">Linear PCM audio sample buffer interleaved by channel.</param>
        /// <param name="sampleRate">Audio buffer sample rate.</param>
        /// <param name="channelCount">Audio buffer channel count.</param>
        /// <param name="timestamp">Audio buffer timestamp in nanoseconds.</param>
        public AudioBuffer (
            IMediaDevice<AudioBuffer> device,
            float[] sampleBuffer,
            int sampleRate,
            int channelCount,
            long timestamp
        ) : this(device, Wrap(sampleBuffer), sampleRate, channelCount, timestamp) { }

        /// <summary>
        /// Create an audio buffer.
        /// </summary>
        /// <param name="device">Audio device that generated this audio buffer.</param>
        /// <param name="sampleBuffer">Linear PCM audio sample buffer interleaved by channel.</param>
        /// <param name="sampleRate">Audio buffer sample rate.</param>
        /// <param name="channelCount">Audio buffer channel count.</param>
        /// <param name="timestamp">Audio buffer timestamp in nanoseconds.</param>
        public AudioBuffer (
            IMediaDevice<AudioBuffer> device,
            NativeArray<float> sampleBuffer,
            int sampleRate,
            int channelCount,
            long timestamp
        ) {
            this.device = device;
            this.sampleBuffer = sampleBuffer;
            this.sampleRate = sampleRate;
            this.channelCount = channelCount;
            this.timestamp = timestamp;
        }

        /// <summary>
        /// Create an audio buffer.
        /// </summary>
        /// <param name="device">Audio device that generated this audio buffer.</param>
        /// <param name="sampleBuffer">Linear PCM audio sample buffer interleaved by channel.</param>
        /// <param name="sampleRate">Audio buffer sample rate.</param>
        /// <param name="channelCount">Audio buffer channel count.</param>
        /// <param name="sampleCount">Audio buffer total sample count.</param>
        /// <param name="timestamp">Audio buffer timestamp in nanoseconds.</param>
        public unsafe AudioBuffer (
            IMediaDevice<AudioBuffer> device,
            float* sampleBuffer,
            int sampleRate,
            int channelCount,
            int sampleCount,
            long timestamp
        ) : this(device, Wrap(sampleBuffer, sampleCount), sampleRate, channelCount, timestamp) { }

        /// <summary>
        /// Safely clone the audio buffer.
        /// The clone will never contain a valid sample buffer
        /// </summary>
        public AudioBuffer Clone () => new AudioBuffer(
            device,
            null,
            sampleRate,
            channelCount,
            timestamp
        );
        #endregion


        #region --Operations--

        private static unsafe NativeArray<float> Wrap (float* buffer, int size) {
            var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(buffer, size, Allocator.None);
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArray, AtomicSafetyHandle.Create());
            #endif
            return nativeArray;
        }

        private static NativeArray<float> Wrap (float[] buffer) => new NativeArray<float>(buffer, Allocator.Temp);
        #endregion
    }
}