/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using System.Collections;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Android;

    internal sealed class AndroidPermissions : Permissions {

        #region --Client API--

        public override PermissionStatus CheckPermissions<T> () {
            var permission = GetPermission<T>();
            var authorized = Permission.HasUserAuthorizedPermission(permission);
            return authorized ? PermissionStatus.Authorized : PermissionStatus.Denied;
        }

        public override Task<PermissionStatus> RequestPermissions<T> () {
            // Create task
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var helper = LifecycleHelper.Create();
            helper.StartCoroutine(Request());
            return tcs.Task;
            // Request
            IEnumerator Request () {
                // Check
                var status = CheckPermissions<T>();
                if (status == PermissionStatus.Authorized) {
                    tcs.SetResult(status);
                    yield break;
                }
                // Request
                var permission = GetPermission<T>();
                Permission.RequestUserPermission(permission);
                yield return null;
                var authorized = Permission.HasUserAuthorizedPermission(permission);
                status = authorized ? PermissionStatus.Authorized : PermissionStatus.Denied;
                tcs.SetResult(status);
                GameObject.Destroy(helper.gameObject);
            }
        }
        #endregion


        #region --Operations--

        private static string GetPermission<T> () => typeof(T) switch {
            var type when typeof(AudioDevice).IsAssignableFrom(type)    => Permission.Microphone,
            var type when typeof(CameraDevice).IsAssignableFrom(type)   => Permission.Camera,
            _                                                           => null
        };
        #endregion
    }
}