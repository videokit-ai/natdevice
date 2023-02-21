/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using AOT;
    using UnityEngine;
    using Internal;
    using DeviceFlags = Internal.NatDevice.DeviceFlags;

    /// <summary>
    /// Hardware audio input device.
    /// </summary>
    public sealed class AudioDevice : IMediaDevice<AudioBuffer> {

        #region --Properties--
        /// <summary>
        /// Device unique ID.
        /// </summary>
        public string uniqueID { get; private set; }

        /// <summary>
        /// Display friendly device name.
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// Device location.
        /// </summary>
        public DeviceLocation location => (DeviceLocation)((int)device.Flags() & 0x3);

        /// <summary>
        /// Device is the default device for its media type.
        /// </summary>
        public bool defaultForMediaType => device.Flags().HasFlag(DeviceFlags.Default);

        /// <summary>
        /// Is echo cancellation supported?
        /// </summary>
        public bool echoCancellationSupported => device.Flags().HasFlag(DeviceFlags.EchoCancellation);

        /// <summary>
        /// Enable or disable Adaptive Echo Cancellation (AEC).
        /// </summary>
        public bool echoCancellation {
            get => device.EchoCancellation();
            set => device.SetEchoCancellation(value);
        }

        /// <summary>
        /// Audio sample rate.
        /// </summary>
        public int sampleRate {
            get => device.SampleRate();
            set => device.SetSampleRate(value);
        }

        /// <summary>
        /// Audio channel count.
        /// </summary>
        public int channelCount {
            get => device.ChannelCount();
            set => device.SetChannelCount(value);
        }

        /// <summary>
        /// Event raised when the audio device is disconnected.
        /// </summary>
        public event Action onDisconnected;
        #endregion


        #region --Streaming--
        /// <summary>
        /// Is the device running?
        /// </summary>
        public bool running => device.Running();

        /// <summary>
        /// Start running.
        /// </summary>
        /// <param name="handler">Delegate to receive audio buffers.</param>
        public unsafe void StartRunning (Action<AudioBuffer> handler) {
            Action<IntPtr> wrapper = sampleBuffer => {
                var audioBuffer = new AudioBuffer(
                    this,
                    sampleBuffer.AudioBufferData(),
                    sampleBuffer.AudioBufferSampleRate(),
                    sampleBuffer.AudioBufferChannelCount(),
                    sampleBuffer.AudioBufferSampleCount(),
                    sampleBuffer.AudioBufferTimestamp()
                );
                handler?.Invoke(audioBuffer);
            };
            streamHandle = GCHandle.Alloc(wrapper, GCHandleType.Normal);
            lifecycleHelper = LifecycleHelper.Create();
            lifecycleHelper.onQuit += StopRunning;
            device.StartRunning(OnAudioBuffer, (IntPtr)streamHandle).CheckStatus();
        }

        /// <summary>
        /// Stop running.
        /// </summary>
        public void StopRunning () {
            if (lifecycleHelper)
                lifecycleHelper.Dispose();
            device.StopRunning().CheckStatus();
            if (streamHandle != default)
                streamHandle.Free();
            streamHandle = default;
            lifecycleHelper = default;
        }
        #endregion


        #region --Operations--
        private readonly IntPtr device;
        private readonly GCHandle weakSelf;
        private GCHandle streamHandle;
        private LifecycleHelper lifecycleHelper;

        internal AudioDevice (IntPtr device) {
            this.device = device;
            this.weakSelf = GCHandle.Alloc(this, GCHandleType.Weak);
            // Cache UID
            var stringBuilder = new StringBuilder(2048);
            device.UniqueID(stringBuilder);
            this.uniqueID = stringBuilder.ToString();
            // Cache name
            stringBuilder.Clear();
            device.Name(stringBuilder);
            this.name = stringBuilder.ToString();
            // Register handlers
            device.SetDisconnectHandler(OnDeviceDisconnect, (IntPtr)weakSelf);
        }

        ~AudioDevice () {
            device.ReleaseDevice();
            weakSelf.Free();
        }

        [MonoPInvokeCallback(typeof(NatDevice.SampleBufferHandler))]
        private static void OnAudioBuffer (IntPtr context, IntPtr sampleBuffer) {
            try {
                var handle = (GCHandle)context;
                var handler = handle.Target as Action<IntPtr>;
                handler?.Invoke(sampleBuffer);
            }
            catch (Exception ex) {
                Debug.LogException(ex);
            }
        }

        [MonoPInvokeCallback(typeof(NatDevice.DeviceDisconnectHandler))]
        private static void OnDeviceDisconnect (IntPtr context) {
            try {
                var handle = (GCHandle)context;
                var device = handle.Target as AudioDevice;
                device?.onDisconnected?.Invoke();
            } catch (Exception ex) {
                Debug.LogException(ex);
            }
        }
        #endregion


        #region --Utility--

        public bool Equals (IMediaDevice other) => other != null && other is AudioDevice && other.uniqueID == uniqueID;

        public override string ToString () => $"microphone:{uniqueID}";

        public static implicit operator IntPtr (AudioDevice device) => device.device;
        #endregion
    }
}