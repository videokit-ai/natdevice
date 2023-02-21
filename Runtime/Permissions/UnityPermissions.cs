/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using System.Collections;
    using System.Threading.Tasks;
    using UnityEngine;

    internal sealed class UnityPermissions : Permissions {

        #region --Client API--

        public override PermissionStatus CheckPermissions<T> () {
            var authorization = GetAuthorization<T>();
            var granted = Application.HasUserAuthorization(authorization);
            return granted ? PermissionStatus.Authorized : PermissionStatus.Denied;
        }

        public override Task<PermissionStatus> RequestPermissions<T> () {
            // Create task
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var helper = LifecycleHelper.Create();
            helper.StartCoroutine(Request());
            return tcs.Task;
            // Request
            IEnumerator Request () {
                var authorization = GetAuthorization<T>();
                yield return Application.RequestUserAuthorization(authorization);
                var status = CheckPermissions<T>();
                tcs.SetResult(status);
                GameObject.Destroy(helper.gameObject);
            }
        }
        #endregion


        #region --Operations--

        private static UserAuthorization GetAuthorization<T> () => typeof(T) switch {
            var type when typeof(AudioDevice).IsAssignableFrom(type)    => UserAuthorization.Microphone,
            var type when typeof(CameraDevice).IsAssignableFrom(type)   => UserAuthorization.WebCam,
            _                                                           => 0
        };
        #endregion
    }
}