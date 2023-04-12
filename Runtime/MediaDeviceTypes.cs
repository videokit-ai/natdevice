/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    /// <summary>
    /// Device location.
    /// </summary>
    public enum DeviceLocation : int { // CHECK // Must match `NatDevice.h`
        /// <summary>
        /// Device type is unknown.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Device is internal.
        /// </summary>
        Internal = 1 << 0,
        /// <summary>
        /// Device is external.
        /// </summary>
        External = 1 << 1
    }

    /// <summary>
    /// Permissions status.
    /// </summary>
    public enum PermissionStatus : int {
        /// <summary>
        /// User has not authorized or denied access to media device.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// User has denied access to media device.
        /// </summary>
        Denied = 2,
        /// <summary>
        /// User has authorized access to media device.
        /// </summary>
        Authorized = 3
    }
}