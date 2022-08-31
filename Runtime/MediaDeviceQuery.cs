/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Devices.Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Devices.MultiCam")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Devices.Tests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatSuite.Devices.MultiCam")]
namespace NatML.Devices {

    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Internal;

    /// <summary>
    /// Query that can be used to access available media devices.
    /// </summary>
    public sealed class MediaDeviceQuery : IReadOnlyList<IMediaDevice> {

        #region --Discovery--
        /// <summary>
        /// Number of devices discovered by the query.
        /// </summary>
        public int count => devices.Length;

        /// <summary>
        /// Current device that meets the provided criteria.
        /// </summary>
        public IMediaDevice current => index < devices.Length ? devices[index] : null;

        /// <summary>
        /// Get the device at a given index.
        /// </summary>
        public IMediaDevice this [int index] => devices[index];

        /// <summary>
        /// Configure the app's global audio session for audio device discovery.
        /// The desired value MUST be set before a query is created. It defaults to `true`.
        /// Currently this only has an effect on iOS.
        /// </summary>
        public static bool ConfigureAudioSession = true;

        /// <summary>
        /// Create a media device query.
        /// </summary>
        /// <param name="filter">Filter for specific devices.</param>
        public MediaDeviceQuery (Predicate<IMediaDevice> filter = null) {
            // Check filter
            var filterFn = filter != null ? new Func<IMediaDevice, bool>(filter) : (_ => true);
            // Get devices
            var devices = new List<IMediaDevice>();
            devices.AddRange(AudioDevices());
            devices.AddRange(CameraDevices());
            // Filter
            this.devices = devices.Where(filterFn).ToArray();
        }

        /// <summary>
        /// Advance the next available device that meets the provided criteria.
        /// </summary>
        public void Advance () => index = (index + 1) % devices.Length;
        #endregion


        #region --Permissions--
        /// <summary>
        /// Check the current permission status for a media device type.
        /// </summary>
        /// <returns>Current permissions status.</returns>
        public static PermissionStatus CheckPermissions<T> () where T : IMediaDevice => PermissionsHelper.CheckPermissions<T>();

        /// <summary>
        /// Request permissions to use media devices from the user.
        /// </summary>
        /// <returns>Permission status.</returns>
        public static Task<PermissionStatus> RequestPermissions<T> () where T : IMediaDevice {        
            var tcs = new TaskCompletionSource<PermissionStatus>();
            PermissionsHelper.RequestPermissions<T>(tcs.SetResult);
            return tcs.Task;
        }
        #endregion


        #region --Operations--
        private readonly IMediaDevice[] devices;
        private int index;

        [RuntimeInitializeOnLoadMethod]
        private static void OnInitialize () => NatDevice.SetAppToken(NatDeviceSettings.Instance.Token);

        private static IEnumerable<AudioDevice> AudioDevices () {
            if (ConfigureAudioSession)
                NatDeviceExt.ConfigureAudioSession();
            var devices = new IntPtr[1 << 6];
            var count = devices.Length;
            NatDevice.AudioDevices(devices, ref count).CheckStatus();
            for (var i = 0; i < count; ++i)
                yield return new AudioDevice(devices[i]);
        }

        private static IEnumerable<CameraDevice> CameraDevices () {
            var devices = new IntPtr[1 << 6];
            var count = devices.Length;
            NatDevice.CameraDevices(devices, ref count).CheckStatus();
            for (var i = 0; i < count; ++i)
                yield return new CameraDevice(devices[i]);
        }

        int IReadOnlyCollection<IMediaDevice>.Count => count;

        IEnumerator IEnumerable.GetEnumerator () => (this as IEnumerable<IMediaDevice>).GetEnumerator();

        IEnumerator<IMediaDevice> IEnumerable<IMediaDevice>.GetEnumerator () => (devices as IEnumerable<IMediaDevice>).GetEnumerator();
        #endregion
    }
}