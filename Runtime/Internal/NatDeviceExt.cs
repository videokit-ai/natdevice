/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using System;
    using System.Runtime.InteropServices;

    public static class NatDeviceExt { // NatDeviceExt.h

        #region --Enumerations--
        public enum PermissionType : int {
            AudioDevice     = 1,
            CameraDevice    = 2
        }
        #endregion


        #region --Delegates--
        public delegate void PermissionResultHandler (IntPtr context, PermissionStatus result);
        #endregion


        #region --Permissions--
        [DllImport(NatDevice.Assembly, EntryPoint = @"NDCheckPermissions")]
        public static extern PermissionStatus CheckPermissions (PermissionType type);

        [DllImport(NatDevice.Assembly, EntryPoint = @"NDRequestPermissions")]
        public static extern void RequestPermissions (
            PermissionType type,
            PermissionResultHandler handler,
            IntPtr context
        );
        #endregion


        #region --PixelBufferOutput--
        [DllImport(NatDevice.Assembly, EntryPoint = @"NDPixelBufferOutputConvert")]
        public static unsafe extern void Convert (
            void** srcBuffers,
            int bufferCount,
            CameraImage.Format format,
            int width,
            int height,
            int* rowStrides,
            int* pixelStrides,
            int orientation,
            bool mirror,
            void* tempBuffer,
            void* dstBuffer,
            out int dstWidth,
            out int dstHeight
        );
        #endregion


        #region --AudioSpectrumOutput--
        [DllImport(NatDevice.Assembly, EntryPoint = @"NDAudioSpectrumOutputCreate")]
        public static unsafe extern void CreateAudioSpectrumOutput (int sampleCount, out IntPtr output);

        [DllImport(NatDevice.Assembly, EntryPoint = @"NDAudioSpectrumOutputForward")]
        public static unsafe extern void Forward (
            this IntPtr output,
            float* srcBuffer,
            int stride,
            int count,
            float* dstSpectrum
        );

        [DllImport(NatDevice.Assembly, EntryPoint = @"NDAudioSpectrumOutputRelease")]
        public static extern void ReleaseAudioSpectrumOutput (this IntPtr output);
        #endregion


        #region --AudioSession--
        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport(NatDevice.Assembly, EntryPoint = @"NDConfigureAudioSession")]
        public static extern void ConfigureAudioSession ();
        #else
        public static void ConfigureAudioSession () { }
        #endif
        #endregion
    }
}