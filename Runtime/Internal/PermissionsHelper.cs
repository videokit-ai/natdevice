/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using AOT;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Android;
    using PermissionType = Internal.NatDeviceExt.PermissionType;
    using PermissionResultHandler = Internal.NatDeviceExt.PermissionResultHandler;

    internal static class PermissionsHelper {

        #region --Client API--

        public static PermissionStatus CheckPermissions<T> () where T : IMediaDevice {
            var type = GetPermissionType<T>();
            switch (Application.platform) {
                case RuntimePlatform.Android:       return CheckPermissionsAndroid(type);
                case RuntimePlatform.IPhonePlayer:  return NatDeviceExt.CheckPermissions(type);
                case RuntimePlatform.OSXEditor:     return NatDeviceExt.CheckPermissions(type);
                case RuntimePlatform.OSXPlayer:     return NatDeviceExt.CheckPermissions(type);
                case RuntimePlatform.WebGLPlayer:   return NatDeviceExt.CheckPermissions(type);
                case RuntimePlatform.WindowsEditor: return PermissionStatus.Authorized;
                case RuntimePlatform.WindowsPlayer: return PermissionStatus.Authorized; // Windows doesn't really care
                default:                            return CheckPermissionsUnity(type);
            }
        }

        public static void RequestPermissions<T> (Action<PermissionStatus> completionHandler) where T : IMediaDevice {
            var type = GetPermissionType<T>();
            switch (Application.platform) {
                case RuntimePlatform.Android:
                    RequestAndroid(type, completionHandler);
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.WebGLPlayer:
                    RequestNative(type, completionHandler);
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    completionHandler?.Invoke(PermissionStatus.Authorized); // Windows does have proper permissions infra
                    break;
                default:
                    RequestUnity(type, completionHandler);
                    break;
            }
        }
        #endregion


        #region --Operations--

        private static PermissionStatus CheckPermissionsAndroid (PermissionType type) {
            var permission = type == PermissionType.CameraDevice ? Permission.Camera : Permission.Microphone;
            var authorized = Permission.HasUserAuthorizedPermission(permission);
            return authorized ? PermissionStatus.Authorized : PermissionStatus.Denied;
        }

        private static PermissionStatus CheckPermissionsUnity (PermissionType type) {
            var permission = type == PermissionType.CameraDevice ? UserAuthorization.WebCam : UserAuthorization.Microphone;
            var granted = Application.HasUserAuthorization(permission);
            return granted ? PermissionStatus.Authorized : PermissionStatus.Denied;
        }

        private static void RequestAndroid (PermissionType type, Action<PermissionStatus> completionHandler) {
            var helperGO = new GameObject("MediaDeviceQuery Android Permissions Helper");
            helperGO.AddComponent<MediaDeviceQueryPermissionsHelper>().StartCoroutine(Request());
            IEnumerator Request () {
                var permission = type == PermissionType.CameraDevice ? Permission.Camera : Permission.Microphone;
                if (Permission.HasUserAuthorizedPermission(permission))
                    completionHandler?.Invoke(PermissionStatus.Authorized);
                else {
                    Permission.RequestUserPermission(permission);
                    /**
                    * Unity dooesn't provide a callback for completion, and doesn't provide an indeterminate state.
                    * so instead we're gonna have to wait for an arbitrary amount of time then check again.
                    */
                    yield return new WaitForSeconds(3f); // This should be enough for user to decide
                    var authorized = Permission.HasUserAuthorizedPermission(permission);
                    completionHandler?.Invoke(authorized ? PermissionStatus.Authorized : PermissionStatus.Denied);
                }
                MonoBehaviour.Destroy(helperGO);
            }
        }

        private static void RequestUnity (PermissionType type, Action<PermissionStatus> completionHandler) {
            var helperGO = new GameObject("MediaDeviceQuery Permissions Helper");
            helperGO.AddComponent<MediaDeviceQueryPermissionsHelper>().StartCoroutine(Request());
            IEnumerator Request () {
                var permission = type == PermissionType.CameraDevice ? UserAuthorization.WebCam : UserAuthorization.Microphone;
                if (Application.HasUserAuthorization(permission))
                    completionHandler?.Invoke(PermissionStatus.Authorized);
                else {
                    yield return Application.RequestUserAuthorization(permission);
                    var granted = Application.HasUserAuthorization(permission);
                    completionHandler?.Invoke(granted ? PermissionStatus.Authorized : PermissionStatus.Denied);
                }
                MonoBehaviour.Destroy(helperGO);
            }
        }

        private static void RequestNative (PermissionType type, Action<PermissionStatus> completionHandler) {
            var handle = GCHandle.Alloc(completionHandler, GCHandleType.Normal);
            NatDeviceExt.RequestPermissions(type, OnPermissionResult, (IntPtr)handle);
        }

        private static PermissionType GetPermissionType<T> () where T : IMediaDevice {
            // Camera device
            if (typeof(CameraDevice).IsAssignableFrom(typeof(T)))
                return PermissionType.CameraDevice;
            // Audio device
            if (typeof(AudioDevice).IsAssignableFrom(typeof(T)))
                return  PermissionType.AudioDevice;
            // Thow
            throw new ArgumentException($"Cannot infer permission type for unknown device type: {typeof(T)}");
        }

        private sealed class MediaDeviceQueryPermissionsHelper : MonoBehaviour {
            void Awake () => DontDestroyOnLoad(this.gameObject);
        }

        [MonoPInvokeCallback(typeof(PermissionResultHandler))]
        private static void OnPermissionResult (IntPtr context, PermissionStatus status) {
            var handle = (GCHandle)context;
            var handler = handle.Target as Action<PermissionStatus>;
            handle.Free();
            handler?.Invoke(status);
        }
        #endregion
    }
}