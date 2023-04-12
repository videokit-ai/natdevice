/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {
    
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using AOT;

    internal sealed class NativePermissions : Permissions {

        #region --Client API--

        public override PermissionStatus CheckPermissions<T> () {
            var type = GetPermissionType<T>();
            return NatDeviceExt.CheckPermissions(type);
        }

        public override Task<PermissionStatus> RequestPermissions<T> () {
            var type = GetPermissionType<T>();
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var handle = GCHandle.Alloc(tcs, GCHandleType.Normal);
            NatDeviceExt.RequestPermissions(type, OnPermissionResult, (IntPtr)handle);
            return tcs.Task;
        }
        #endregion


        #region --Operations--

        internal static NatDeviceExt.PermissionType GetPermissionType<T> () => typeof(T) switch {
            var type when typeof(AudioDevice).IsAssignableFrom(type)    => NatDeviceExt.PermissionType.AudioDevice,
            var type when typeof(CameraDevice).IsAssignableFrom(type)   => NatDeviceExt.PermissionType.CameraDevice,
            var type => throw new ArgumentException($"Cannot infer permission type for unknown device type: {type}"),
        };

        [MonoPInvokeCallback(typeof(NatDeviceExt.PermissionResultHandler))]
        private static void OnPermissionResult (IntPtr context, PermissionStatus status) {
            var handle = (GCHandle)context;
            var tcs = handle.Target as TaskCompletionSource<PermissionStatus>;
            handle.Free();
            tcs?.SetResult(status);
        }
        #endregion
    }
}