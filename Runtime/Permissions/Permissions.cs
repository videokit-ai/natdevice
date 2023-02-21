/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using System.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// Platform-specific permissions provider.
    /// </summary>
    internal abstract class Permissions {

        #region --Client API--
        /// <summary>
        /// Permissions provider instance for this platform.
        /// </summary>
        public static Permissions Instance => instance ??= Create();

        /// <summary>
        /// Check the current permission status for a given media device type.
        /// </summary>
        /// <returns>Permission status.</returns>
        public abstract PermissionStatus CheckPermissions<T> () where T : IMediaDevice;

        /// <summary>
        /// Request permissions for the given media device type.
        /// </summary>
        /// <returns>Permission status.</returns>
        public abstract Task<PermissionStatus> RequestPermissions<T> () where T : IMediaDevice;
        #endregion


        #region --Operations--

        private static Permissions instance;

        private static Permissions Create () => Application.platform switch {
            RuntimePlatform.Android         => new AndroidPermissions(),
            RuntimePlatform.IPhonePlayer    => new NativePermissions(),
            RuntimePlatform.OSXEditor       => new NativePermissions(),
            RuntimePlatform.OSXPlayer       => new NativePermissions(),
            RuntimePlatform.WebGLPlayer     => new NativePermissions(),
            _                               => new UnityPermissions(),
        };
        #endregion
    }
}