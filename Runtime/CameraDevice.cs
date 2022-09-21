/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using AOT;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using UnityEngine;
    using Internal;
    using DeviceFlags = Internal.NatDevice.DeviceFlags;
    
    /// <summary>
    /// Hardware camera device.
    /// </summary>
    public sealed class CameraDevice : IMediaDevice<CameraImage> {

        #region --Types--
        /// <summary>
        /// Exposure mode.
        /// </summary>
        public enum ExposureMode : int {
            /// <summary>
            /// Continuous auto exposure.
            /// </summary>
            Continuous  = 0,
            /// <summary>
            /// Locked auto exposure.
            /// </summary>
            Locked      = 1,
            /// <summary>
            /// Manual exposure.
            /// </summary>
            Manual      = 2
        }

        /// <summary>
        /// Photo flash mode.
        /// </summary>
        public enum FlashMode : int {
            /// <summary>
            /// Never use flash.
            /// </summary>
            Off     = 0,
            /// <summary>
            /// Always use flash.
            /// </summary>
            On      = 1,
            /// <summary>
            /// Let the sensor detect if it needs flash.
            /// </summary>
            Auto    = 2
        }

        /// <summary>
        /// Focus mode.
        /// </summary>
        public enum FocusMode : int {
            /// <summary>
            /// Continuous autofocus.
            /// </summary>
            Continuous  = 0,
            /// <summary>
            /// Locked focus.
            /// </summary>
            Locked      = 1,
        }

        /// <summary>
        /// Torch mode.
        /// </summary>
        public enum TorchMode : int {
            /// <summary>
            /// Disabled torch.
            /// </summary>
            Off     = 0,
            /// <summary>
            /// Maximum supported torch level.
            /// </summary>
            Maximum = 100
        }

        /// <summary>
        /// Video stabilization mode.
        /// </summary>
        public enum VideoStabilizationMode : int {
            /// <summary>
            /// Disabled video stabilization.
            /// </summary>
            Off         = 0,
            /// <summary>
            /// Standard video stabilization.
            /// </summary>
            Standard    = 1
        }

        /// <summary>
        /// White balance mode.
        /// </summary>
        public enum WhiteBalanceMode : int {
            /// <summary>
            /// Continuous auto white balance.
            /// </summary>
            Continuous  = 0,
            /// <summary>
            /// Locked auto white balance.
            /// </summary>
            Locked      = 1,
        }
        #endregion


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
        /// Is this camera front facing?
        /// </summary>
        public bool frontFacing => device.Flags().HasFlag(DeviceFlags.FrontFacing);

        /// <summary>
        /// Is flash supported for photo capture?
        /// </summary>
        public bool flashSupported => device.Flags().HasFlag(DeviceFlags.Flash);

        /// <summary>
        /// Is torch supported?
        /// </summary>
        public bool torchSupported => device.Flags().HasFlag(DeviceFlags.Torch);
        
        /// <summary>
        /// Is setting the exposure point supported?
        /// </summary>
        public bool exposurePointSupported => device.Flags().HasFlag(DeviceFlags.ExposurePoint);

        /// <summary>
        /// Is setting the focus point supported?
        /// </summary>
        public bool focusPointSupported => device.Flags().HasFlag(DeviceFlags.FocusPoint);

        /// <summary>
        /// Is depth streaming supported?
        /// </summary>
        internal bool depthStreamingSupported => device.Flags().HasFlag(DeviceFlags.Depth);

        /// <summary>
        /// Field of view in degrees.
        /// </summary>
        public (float width, float height) fieldOfView {
            get {
                device.FieldOfView(out var width, out var height);
                return (width, height);
            }
        }

        /// <summary>
        /// Exposure bias range in EV.
        /// </summary>
        public (float min, float max) exposureBiasRange {
            get {
                device.ExposureBiasRange(out var min, out var max);
                return (min, max);
            }
        }

        /// <summary>
        /// Exposure duration range in seconds.
        /// </summary>
        public (float min, float max) exposureDurationRange {
            get {
                device.ExposureDurationRange(out var min, out var max);
                return (min, max);
            }
        }

        /// <summary>
        /// Sensor sensitivity range.
        /// </summary>
        public (float min, float max) ISORange {
            get {
                device.ISORange(out var min, out var max);
                return (min, max);
            }
        }

        /// <summary>
        /// Zoom ratio range.
        /// </summary>
        public (float min, float max) zoomRange {
            get {
                device.ZoomRange(out var min, out var max);
                return (min, max);
            }
        }

        /// <summary>
        /// Get or set the preview resolution.
        /// </summary>
        public (int width, int height) previewResolution {
            get { device.PreviewResolution(out var width, out var height); return (width, height); }
            set => device.SetPreviewResolution(value.width, value.height);
        }

        /// <summary>
        /// Get or set the photo resolution.
        /// </summary>
        public (int width, int height) photoResolution {
            get { device.PhotoResolution(out var width, out var height); return (width, height); }
            set => device.SetPhotoResolution(value.width, value.height);
        }

        /// <summary>
        /// Get or set the preview framerate.
        /// </summary>
        public int frameRate {
            get => device.FrameRate();
            set => device.SetFrameRate(value);
        }

        /// <summary>
        /// Get or set the exposure mode.
        /// If the requested exposure mode is not supported, the camera device will ignore.
        /// </summary>
        public ExposureMode exposureMode {
            get => device.ExposureMode();
            set => device.SetExposureMode(value);
        }

        /// <summary>
        /// Get or set the exposure bias.
        /// This value must be in the range returned by `exposureRange`.
        /// </summary>
        public float exposureBias {
            get => device.ExposureBias();
            set => device.SetExposureBias(value);
        }

        /// <summary>
        /// Get or set the photo flash mode.
        /// </summary>
        public FlashMode flashMode {
            get => device.FlashMode();
            set => device.SetFlashMode(value);
        }

        /// <summary>
        /// Get or set the focus mode.
        /// </summary>
        public FocusMode focusMode {
            get => device.FocusMode();
            set => device.SetFocusMode(value);
        }

        /// <summary>
        /// Get or set the torch mode.
        /// </summary>
        public TorchMode torchMode {
            get => device.TorchMode();
            set => device.SetTorchMode(value);
        }

        /// <summary>
        /// Get or set the white balance mode.
        /// </summary>
        public WhiteBalanceMode whiteBalanceMode {
            get => device.WhiteBalanceMode();
            set => device.SetWhiteBalanceMode(value);
        }

        /// <summary>
        /// Get or set the video stabilization mode.
        /// </summary>
        public VideoStabilizationMode videoStabilizationMode {
            get => device.VideoStabilizationMode();
            set => device.SetVideoStabilizationMode(value);
        }

        /// <summary>
        /// Get or set the zoom ratio.
        /// This value must be in the range returned by `zoomRange`.
        /// </summary>
        public float zoomRatio {
            get => device.ZoomRatio();
            set => device.SetZoomRatio(value);
        }
        #endregion


        #region --Events--
        /// <summary>
        /// Event raised when the camera device is disconnected.
        /// </summary>
        public event Action onDisconnected;
        #endregion


        #region --Controls--
        /// <summary>
        /// Check if a given exposure mode is supported by the camera device.
        /// </summary>
        /// <param name="mode">Exposure mode.</param>
        public bool ExposureModeSupported (ExposureMode mode) => mode switch {
            ExposureMode.Continuous => device.Flags().HasFlag(DeviceFlags.ExposureContinuous),
            ExposureMode.Locked     => device.Flags().HasFlag(DeviceFlags.ExposureLock),
            ExposureMode.Manual     => device.Flags().HasFlag(DeviceFlags.ExposureManual),
            _                       => false
        };

        /// <summary>
        /// Check if a given focus mode is supported by the camera device.
        /// </summary>
        /// <param name="mode">Focus mode.</param>
        public bool FocusModeSupported (FocusMode mode) => mode switch {
            FocusMode.Continuous    => device.Flags().HasFlag(DeviceFlags.FocusContinuous),
            FocusMode.Locked        => device.Flags().HasFlag(DeviceFlags.FocusLock),
            _                       => false,
        };

        /// <summary>
        /// Check if a given white balance mode is supported by the camera device.
        /// </summary>
        /// <param name="mode">White balance mode.</param>
        public bool WhiteBalanceModeSupported (WhiteBalanceMode mode) => mode switch {
            WhiteBalanceMode.Continuous => device.Flags().HasFlag(DeviceFlags.WhiteBalanceContinuous),
            WhiteBalanceMode.Locked     => device.Flags().HasFlag(DeviceFlags.WhiteBalanceLock),
            _                           => false
        };

        /// <summary>
        /// Check if a given video stabilization mode is supported by the camera device.
        /// </summary>
        /// <param name="mode">Video stabilization mode.</param>
        public bool VideoStabilizationModeSupported (VideoStabilizationMode mode) => mode switch {
            VideoStabilizationMode.Off      => true,
            VideoStabilizationMode.Standard => device.Flags().HasFlag(DeviceFlags.VideoStabilization),
            _                               => false
        };

        /// <summary>
        /// Set manual exposure.
        /// </summary>
        /// <param name="duration">Exposure duration in seconds. MUST be in `exposureDurationRange`.</param>
        /// <param name="ISO">Sensor sensitivity ISO value. MUST be in `ISORange`.</param>
        public void SetExposureDuration (float duration, float ISO) => device.SetExposureDuration(duration, ISO);

        /// <summary>
        /// Set the exposure point of interest.
        /// The point is specified in normalized coordinates in range [0.0, 1.0].
        /// </summary>
        /// <param name="x">Normalized x coordinate.</param>
        /// <param name="y">Normalized y coordinate.</param>
        public void SetExposurePoint (float x, float y) => device.SetExposurePoint(x, y);

        /// <summary>
        /// Set the focus point of interest.
        /// The point is specified in normalized coordinates in range [0.0, 1.0].
        /// </summary>
        /// <param name="x">Normalized x coordinate.</param>
        /// <param name="y">Normalized y coordinate.</param>
        public void SetFocusPoint (float x, float y) => device.SetFocusPoint(x, y);
        #endregion


        #region --Streaming--
        /// <summary>
        /// Is the device running?
        /// </summary>
        public bool running => device.Running();

        /// <summary>
        /// Start the camera preview.
        /// </summary>
        /// <param name="handler">Delegate to receive preview image frames.</param>
        public void StartRunning (Action<CameraImage> handler) {
            var syncContext = SynchronizationContext.Current; // send to main thread if available
            var unityThreadID = Thread.CurrentThread.ManagedThreadId;
            var paused = false;
            SendOrPostCallback callback = arg => {
                var sampleBuffer = (IntPtr)arg;
                try {
                    if (running) // due to deferred callback, this is necessary
                        handler?.Invoke(new CameraImage(this, sampleBuffer));
                } finally {
                    sampleBuffer.ReleaseSampleBuffer();
                }
            };
            Action<IntPtr> wrapper = sampleBuffer => {
                if (paused)
                    sampleBuffer.ReleaseSampleBuffer();
                else if (syncContext != null && Thread.CurrentThread.ManagedThreadId != unityThreadID)
                    syncContext.Post(callback, sampleBuffer);
                else
                    callback.Invoke(sampleBuffer);
            };
            previewHandle = GCHandle.Alloc(wrapper, GCHandleType.Normal);
            lifecycleHelper = LifecycleHelper.Create();
            lifecycleHelper.onPause += p => paused = p;
            lifecycleHelper.onQuit += StopRunning;
            device.StartRunning(OnCameraImage, (IntPtr)previewHandle);
        }

        /// <summary>
        /// Start the camera preview and stream camera depth.
        /// </summary>
        /// <param name="handler">Delegate to receive preview image and depth frames.</param>
        internal void StartRunning (Action<CameraImage, CameraImage> handler) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stop the camera preview.
        /// </summary>
        public void StopRunning () {
            if (lifecycleHelper)
                lifecycleHelper.Dispose();
            device.StopRunning();
            if (previewHandle != default)
                previewHandle.Free();
            previewHandle = default;
            lifecycleHelper = default;
        }

        /// <summary>
        /// Capture a photo.
        /// </summary>
        /// <param name="handler">Delegate to receive high-resolution photo.</param>
        public void CapturePhoto (Action<CameraImage> handler) {
            var syncContext = SynchronizationContext.Current; // send to main thread if available
            var unityThreadID = Thread.CurrentThread.ManagedThreadId;
            SendOrPostCallback callback = arg => {
                var sampleBuffer = (IntPtr)arg;
                try {
                    handler?.Invoke(new CameraImage(this, sampleBuffer));
                } finally {
                    sampleBuffer.ReleaseSampleBuffer();
                }
            };
            Action<IntPtr> wrapper = sampleBuffer => {
                if (syncContext != null && Thread.CurrentThread.ManagedThreadId != unityThreadID)
                    syncContext.Post(callback, sampleBuffer);
                else
                    callback.Invoke(sampleBuffer);
            };
            var handle = GCHandle.Alloc(wrapper, GCHandleType.Normal);
            device.CapturePhoto(OnCameraPhoto, (IntPtr)handle);
        }
        #endregion


        #region --Operations--
        private readonly IntPtr device;
        private readonly GCHandle weakSelf;
        private GCHandle previewHandle;
        private LifecycleHelper lifecycleHelper;

        internal CameraDevice (IntPtr device) {
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

        ~CameraDevice () {
            device.ReleaseDevice();
            weakSelf.Free();
        }

        [MonoPInvokeCallback(typeof(NatDevice.SampleBufferHandler))]
        private static unsafe void OnCameraImage (IntPtr context, IntPtr sampleBuffer) {
            var handle = (GCHandle)context;
            var handler = handle.Target as Action<IntPtr>;
            handler?.Invoke(sampleBuffer);
        }

        [MonoPInvokeCallback(typeof(NatDevice.SampleBufferHandler))]
        private static unsafe void OnCameraPhoto (IntPtr context, IntPtr sampleBuffer) {
            var handle = (GCHandle)context;
            var handler = handle.Target as Action<IntPtr>;
            handle.Free();
            handler?.Invoke(sampleBuffer);
        }

        [MonoPInvokeCallback(typeof(NatDevice.DeviceDisconnectHandler))]
        private static void OnDeviceDisconnect (IntPtr context) {
            var handle = (GCHandle)context;
            var device = handle.Target as CameraDevice;
            device?.onDisconnected?.Invoke();
        }
        #endregion


        #region --Utility--

        public bool Equals (IMediaDevice other) => other != null && other is CameraDevice && other.uniqueID == uniqueID;

        public override string ToString () => $"camera:{uniqueID}";

        public static implicit operator IntPtr (CameraDevice device) => device.device;
        #endregion


        #region --DEPRECATED--
        [Obsolete(@"Deprecated in NatDevice 1.2.0. Use `ExposureModeSupported(ExposureMode.Locked)` instead.", false)]
        public bool exposureLockSupported => ExposureModeSupported(ExposureMode.Locked);

        [Obsolete(@"Deprecated in NatDevice 1.2.0. Use `FocusModeSupported(FocusMode.Locked)` instead.", false)]
        public bool focusLockSupported => FocusModeSupported(FocusMode.Locked);

        [Obsolete(@"Deprecated in NatDevice 1.2.1. Use `WhiteBalanceModeSupported(WhiteBalanceMode.Locked)` instead.", false)]
        public bool whiteBalanceLockSupported => WhiteBalanceModeSupported(WhiteBalanceMode.Locked);

        [Obsolete(@"Deprecated in NatDevice 1.2.0. Use `exposureBiasRange` instead.")]
        public (float min, float max) exposureRange => exposureBiasRange;

        [Obsolete(@"Deprecated in NatDevice 1.2.0. Use `exposureMode` instead.", false)]
        public bool exposureLock {
            get => exposureMode == ExposureMode.Locked;
            set => exposureMode = ExposureMode.Locked;
        }

        [Obsolete(@"Deprecated in NatDevice 1.2.0. Use `focusMode` instead.", false)]
        public bool focusLock {
            get => focusMode == FocusMode.Locked;
            set => focusMode = FocusMode.Locked;
        }

        [Obsolete(@"Deprecated in NatDevice 1.2.1. Use `whiteBalanceMode` instead.", false)]
        public bool whiteBalanceLock {
            get => whiteBalanceMode == WhiteBalanceMode.Locked;
            set => whiteBalanceMode = WhiteBalanceMode.Locked;
        }

        [Obsolete(@"Deprecated in NatDevice 1.2.0. Use `SetExposurePoint` instead.", false)]
        public (float x, float y) exposurePoint { set => SetExposurePoint(value.x, value.y); }

        [Obsolete(@"Deprecated in NatDevice 1.2.0. Use `SetFocusPoint` instead.", false)]
        public (float x, float y) focusPoint { set => SetFocusPoint(value.x, value.y); }

        [Obsolete(@"Deprecated in NatDevice 1.2.1. Use `torchMode` instead.", false)]
        public bool torchEnabled {
            get => torchMode == TorchMode.Maximum;
            set => torchMode = value ? TorchMode.Maximum : TorchMode.Off;
        }
        #endregion
    }
}