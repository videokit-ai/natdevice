/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public static class NatDevice { // NatDevice.h

        public const string Assembly =
        #if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
        @"__Internal";
        #else
        @"NatDevice";
        #endif


        #region --Enumerations--
        public enum Status : int {
            Ok                  = 0,
            InvalidArgument     = 1,
            InvalidOperation    = 2,
            NotImplemented      = 3,
            InvalidSession      = 101,
            MissingNatMLHub     = 102,
            InvalidNatMLHub     = 103,
            InvalidPlan         = 104,
            LimitedPlan         = 105,
        }

        [Flags]
        public enum DeviceFlags : int { 
            // MediaDevice
            Internal                = 1 << 0,
            External                = 1 << 1,
            Default                 = 1 << 3,
            // AudioDevice
            EchoCancellation        = 1 << 2,
            // CameraDevice
            FrontFacing             = 1 << 6,
            Flash                   = 1 << 7,
            Torch                   = 1 << 8,
            Depth                   = 1 << 15,
            // CameraDevice.Exposure
            ExposureContinuous      = 1 << 16,
            ExposureLock            = 1 << 11,
            ExposureManual          = 1 << 14,
            ExposurePoint           = 1 << 9,
            // CameraDevice.Focus
            FocusContinuous         = 1 << 17,
            FocusLock               = 1 << 12,
            FocusPoint              = 1 << 10,
            // CameraDevice.WhiteBalance
            WhiteBalanceContinuous  = 1 << 18,
            WhiteBalanceLock        = 1 << 13,
            // CameraDevice.VideoStabilization
            VideoStabilization      = 1 << 19,
        }

        public enum MetadataKey : int {
            IntrinsicMatrix     = 1,
            ExposureBias        = 2,
            ExposureDuration    = 3,
            FocalLength         = 4,
            FNumber             = 5,
            Brightness          = 6,
            ISO                 = 7,
        }
        #endregion


        #region --Delegates--
        public delegate void SampleBufferHandler (IntPtr context, IntPtr sampleBuffer);
        public delegate void DeviceDisconnectHandler (IntPtr context);
        #endregion


        #region --NatML--
        [DllImport(Assembly, EntryPoint = @"NDSetSessionToken")]
        public static extern Status SetSessionToken (
            [MarshalAs(UnmanagedType.LPStr)] string token
        );
        #endregion


        #region --IMediaDevice--
        [DllImport(Assembly, EntryPoint = @"NDReleaseMediaDevice")]
        public static extern Status ReleaseDevice (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDMediaDeviceGetUniqueID")]
        public static extern Status UniqueID (
            this IntPtr device,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder dest
        );

        [DllImport(Assembly, EntryPoint = @"NDMediaDeviceGetName")]
        public static extern Status Name (
            this IntPtr device,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder dest
        );

        [DllImport(Assembly, EntryPoint = @"NDMediaDeviceGetFlags")]
        public static extern DeviceFlags Flags (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDMediaDeviceIsRunning")]
        public static extern bool Running (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDMediaDeviceStartRunning")]
        public static extern Status StartRunning (
            this IntPtr device,
            SampleBufferHandler callback,
            IntPtr context
        );

        [DllImport(Assembly, EntryPoint = @"NDMediaDeviceStopRunning")]
        public static extern Status StopRunning (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDMediaDeviceSetDisconnectHandler")]
        public static extern Status SetDisconnectHandler (
            this IntPtr device,
            DeviceDisconnectHandler handler,
            IntPtr context
        );
        #endregion


        #region --AudioDevice--
        [DllImport(Assembly, EntryPoint = @"NDGetAudioDevices")]
        public static extern Status GetAudioDevices (
            [Out] IntPtr[] devices,
            int size,
            out int count
        );

        [DllImport(Assembly, EntryPoint = @"NDAudioDeviceEchoCancellation")]
        public static extern bool EchoCancellation (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDAudioDeviceSetEchoCancellation")]
        public static extern void SetEchoCancellation (this IntPtr device, bool echoCancellation);

        [DllImport(Assembly, EntryPoint = @"NDAudioDeviceSampleRate")]
        public static extern int SampleRate (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDAudioDeviceSetSampleRate")]
        public static extern void SetSampleRate (this IntPtr device, int sampleRate);

        [DllImport(Assembly, EntryPoint = @"NDAudioDeviceChannelCount")]
        public static extern int ChannelCount (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDAudioDeviceSetChannelCount")]
        public static extern void SetChannelCount (this IntPtr device, int sampleRate);
        #endregion


        #region --CameraDevice--
        [DllImport(Assembly, EntryPoint = @"NDGetCameraDevices")]
        public static extern Status GetCameraDevices (
            [Out] IntPtr[] devices,
            int size,
            out int count
        );

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceFieldOfView")]
        public static extern void FieldOfView (this IntPtr device, out float x, out float y);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceExposureBiasRange")]
        public static extern void ExposureBiasRange (this IntPtr device, out float min, out float max);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceExposureDurationRange")]
        public static extern void ExposureDurationRange (this IntPtr device, out float min, out float max);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceISORange")]
        public static extern void ISORange (this IntPtr device, out float min, out float max);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceZoomRange")]
        public static extern void ZoomRange (this IntPtr device, out float min, out float max);

        [DllImport(Assembly, EntryPoint = @"NDCameraDevicePreviewResolution")]
        public static extern void PreviewResolution (this IntPtr device, out int width, out int height);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetPreviewResolution")]
        public static extern void SetPreviewResolution (this IntPtr device, int width, int height);

        [DllImport(Assembly, EntryPoint = @"NDCameraDevicePhotoResolution")]
        public static extern void PhotoResolution (this IntPtr device, out int width, out int height);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetPhotoResolution")]
        public static extern void SetPhotoResolution (this IntPtr device, int width, int height);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceFrameRate")]
        public static extern int FrameRate (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetFrameRate")]
        public static extern void SetFrameRate (this IntPtr device, int framerate);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceExposureBias")]
        public static extern float ExposureBias (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetExposureBias")]
        public static extern void SetExposureBias (this IntPtr device, float bias);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetExposurePoint")]
        public static extern void SetExposurePoint (this IntPtr device, float x, float y);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceExposureMode")]
        public static extern CameraDevice.ExposureMode ExposureMode (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetExposureMode")]
        public static extern void SetExposureMode (this IntPtr device, CameraDevice.ExposureMode mode);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetExposureDuration")]
        public static extern void SetExposureDuration (this IntPtr device, float duration, float ISO);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceFlashMode")]
        public static extern CameraDevice.FlashMode FlashMode (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetFlashMode")]
        public static extern void SetFlashMode (this IntPtr device, CameraDevice.FlashMode mode);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceFocusMode")]
        public static extern CameraDevice.FocusMode FocusMode (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetFocusMode")]
        public static extern void SetFocusMode (this IntPtr device, CameraDevice.FocusMode mode);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetFocusPoint")]
        public static extern void SetFocusPoint (this IntPtr device, float x, float y);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceTorchMode")]
        public static extern CameraDevice.TorchMode TorchMode (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetTorchMode")]
        public static extern void SetTorchMode (this IntPtr device, CameraDevice.TorchMode mode);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceWhiteBalanceMode")]
        public static extern CameraDevice.WhiteBalanceMode WhiteBalanceMode (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetWhiteBalanceMode")]
        public static extern void SetWhiteBalanceMode (this IntPtr device, CameraDevice.WhiteBalanceMode mode);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceVideoStabilizationMode")]
        public static extern CameraDevice.VideoStabilizationMode VideoStabilizationMode (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetVideoStabilizationMode")]
        public static extern void SetVideoStabilizationMode (
            this IntPtr device,
            CameraDevice.VideoStabilizationMode mode
        );

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceZoomRatio")]
        public static extern float ZoomRatio (this IntPtr device);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceSetZoomRatio")]
        public static extern void SetZoomRatio (this IntPtr device, float ratio);

        [DllImport(Assembly, EntryPoint = @"NDCameraDeviceCapturePhoto")]
        public static extern void CapturePhoto (
            this IntPtr device,
            SampleBufferHandler handler,
            IntPtr context
        );
        #endregion


        #region --AudioBuffer--
        [DllImport(Assembly, EntryPoint = @"NDAudioBufferData")]
        public static unsafe extern float* AudioBufferData (this IntPtr audioBuffer);

        [DllImport(Assembly, EntryPoint = @"NDAudioBufferSampleCount")]
        public static extern int AudioBufferSampleCount (this IntPtr audioBuffer);

        [DllImport(Assembly, EntryPoint = @"NDAudioBufferSampleRate")]
        public static extern int AudioBufferSampleRate (this IntPtr audioBuffer);

        [DllImport(Assembly, EntryPoint = @"NDAudioBufferChannelCount")]
        public static extern int AudioBufferChannelCount (this IntPtr audioBuffer);

        [DllImport(Assembly, EntryPoint = @"NDAudioBufferTimestamp")]
        public static extern long AudioBufferTimestamp (this IntPtr audioBuffer);
        #endregion


        #region --CameraImage--
        [DllImport(Assembly, EntryPoint = @"NDCameraImageData")]
        public static unsafe extern void* CameraImageData (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImageDataSize")]
        public static extern int CameraImageDataSize (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImageFormat")]
        public static extern CameraImage.Format CameraImageFormat (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImageWidth")]
        public static extern int CameraImageWidth (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImageHeight")]
        public static extern int CameraImageHeight (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImageRowStride")]
        public static extern int CameraImageRowStride (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImageTimestamp")]
        public static extern long CameraImageTimestamp (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImageVerticallyMirrored")]
        public static extern bool CameraImageVerticallyMirrored (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImagePlaneCount")]
        public static extern int CameraImagePlaneCount (this IntPtr cameraImage);

        [DllImport(Assembly, EntryPoint = @"NDCameraImagePlaneData")]
        public static unsafe extern void* CameraImagePlaneData (this IntPtr cameraImage, int planeIdx);

        [DllImport(Assembly, EntryPoint = @"NDCameraImagePlaneDataSize")]
        public static extern int CameraImagePlaneDataSize (this IntPtr cameraImage, int planeIdx);

        [DllImport(Assembly, EntryPoint = @"NDCameraImagePlaneWidth")]
        public static extern int CameraImagePlaneWidth (this IntPtr cameraImage, int planeIdx);

        [DllImport(Assembly, EntryPoint = @"NDCameraImagePlaneHeight")]
        public static extern int CameraImagePlaneHeight (this IntPtr cameraImage, int planeIdx);

        [DllImport(Assembly, EntryPoint = @"NDCameraImagePlanePixelStride")]
        public static extern int CameraImagePlanePixelStride (this IntPtr cameraImage, int planeIdx);

        [DllImport(Assembly, EntryPoint = @"NDCameraImagePlaneRowStride")]
        public static extern int CameraImagePlaneRowStride (this IntPtr cameraImage, int planeIdx);

        [DllImport(Assembly, EntryPoint = @"NDCameraImageMetadata")]
        public static unsafe extern bool CameraImageMetadata (
            this IntPtr cameraImage,
            MetadataKey key,
            float* value,
            int count = 1
        );
        #endregion


        #region --Utility--

        public static void CheckStatus (this NatDevice.Status status) {
            switch (status) {
                case Status.Ok:                 break;
                case Status.InvalidArgument:    throw new ArgumentException();
                case Status.InvalidOperation:   throw new InvalidOperationException();
                case Status.NotImplemented:     throw new NotImplementedException();
                case Status.InvalidSession:     throw new InvalidOperationException(@"NatDevice session token is invalid. Check your NatML access key and plan.");
                case Status.MissingNatMLHub:    throw new InvalidOperationException(@"NatMLHub native library could not be found.");
                case Status.InvalidNatMLHub:    throw new InvalidOperationException(@"NatMLHub native library is invalid.");
                case Status.InvalidPlan:        throw new InvalidOperationException(@"NatML billing plan does not support this operation. Check your plan and upgrade at https://hub.natml.ai");
                case Status.LimitedPlan:        Debug.LogWarning(@"NatML billing plan only allows for limited functionality. Check your plan and upgrade at https://hub.natml.ai"); break;
                default:                        throw new InvalidOperationException();
            }
        }
        #endregion
    }
}