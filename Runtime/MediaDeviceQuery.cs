/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

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
        /// <param name="capacity">Maximum number of devices to discover.</param>
        public MediaDeviceQuery (Predicate<IMediaDevice> filter = null, int capacity = int.MaxValue) : this(DiscoverDevices(filter, capacity)) { }

        /// <summary>
        /// Create a media device query from one or more media devices.
        /// </summary>
        public MediaDeviceQuery (IEnumerable<IMediaDevice> devices) {
            this.devices = devices.ToArray();
            this.index = 0;
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
        public static PermissionStatus CheckPermissions<T> () where T : IMediaDevice => Permissions.Instance.CheckPermissions<T>();

        /// <summary>
        /// Request permissions to use media devices from the user.
        /// </summary>
        /// <returns>Permission status.</returns>
        public static Task<PermissionStatus> RequestPermissions<T> () where T : IMediaDevice => Permissions.Instance.RequestPermissions<T>();
        #endregion


        #region --Operations--
        private readonly IMediaDevice[] devices;
        private int index;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnInitialize () => NatDevice.SetSessionToken(NatDeviceSettings.Instance.Token);

        int IReadOnlyCollection<IMediaDevice>.Count => count;

        IEnumerator IEnumerable.GetEnumerator () => (this as IEnumerable<IMediaDevice>).GetEnumerator();

        IEnumerator<IMediaDevice> IEnumerable<IMediaDevice>.GetEnumerator () => (devices as IEnumerable<IMediaDevice>).GetEnumerator();
        #endregion


        #region --Discovery--

        private static IEnumerable<IMediaDevice> DiscoverDevices (Predicate<IMediaDevice> filter, int capacity) {
            var filterFn = filter != null ? new Func<IMediaDevice, bool>(filter) : (_ => true);
            var devices = new List<IMediaDevice>();
            devices.AddRange(DiscoverAudioDevices());
            devices.AddRange(DiscoverCameraDevices());
            return devices.Where(filterFn).OrderBy(ComputeDeviceOrder).Take(capacity);
        }

        private static IEnumerable<AudioDevice> DiscoverAudioDevices () {
            if (ConfigureAudioSession)
                NatDeviceExt.ConfigureAudioSession();
            var devices = new IntPtr[1 << 6];
            NatDevice.GetAudioDevices(devices, devices.Length, out var count).CheckStatus();
            for (var i = 0; i < count; ++i)
                yield return new AudioDevice(devices[i]);
        }

        private static IEnumerable<CameraDevice> DiscoverCameraDevices () {
            var devices = new IntPtr[1 << 6];
            NatDevice.GetCameraDevices(devices, devices.Length, out var count).CheckStatus();
            for (var i = 0; i < count; ++i)
                yield return new CameraDevice(devices[i]);
        }

        private static int ComputeDeviceOrder (IMediaDevice device) { // #24
            var order = 0;
            if (!device.defaultForMediaType)
                order += 1;
            if (device.location == DeviceLocation.External)
                order += 10;
            if (device.location == DeviceLocation.Unknown)
                order += 100;
            if (device is CameraDevice)
                order += 1000;
            return order;
        }
        #endregion
    }
}