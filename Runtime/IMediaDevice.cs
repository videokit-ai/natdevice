/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using System;

    /// <summary>
    /// Media device which provides media sample buffers.
    /// </summary>
    public interface IMediaDevice : IEquatable<IMediaDevice> {

        #region --Properties--
        /// <summary>
        /// Device unique ID.
        /// </summary>
        string uniqueID { get; }

        /// <summary>
        /// Display friendly device name.
        /// </summary>
        string name { get; }

        /// <summary>
        /// Device location.
        /// </summary>
        DeviceLocation location { get; }

        /// <summary>
        /// Device is the default device for its media type.
        /// </summary>
        bool defaultForMediaType { get; }
        #endregion


        #region --Events--
        /// <summary>
        /// Event raised when the device is disconnected.
        /// </summary>
        event Action onDisconnected;
        #endregion


        #region --Streaming--
        /// <summary>
        /// Is the device running?
        /// </summary>
        bool running { get; }

        /// <summary>
        /// Stop running.
        /// </summary>
        void StopRunning ();
        #endregion
    }

    /// <summary>
    /// Media device which provides media sample buffers.
    /// </summary>
    public interface IMediaDevice<TSampleBuffer> : IMediaDevice {

        #region --Streaming--
        /// <summary>
        /// Start running.
        /// </summary>
        /// <param name="handler">Delegate to receive sample buffers.</param>
        void StartRunning (Action<TSampleBuffer> handler);
        #endregion
    }
}