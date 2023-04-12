/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All rights reserved.
*/

namespace NatML.Devices.Editor {

    using System;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEditor;
    using Hub;
    using Hub.Internal;
    using Hub.Requests;
    using Internal;

    [FilePath(@"ProjectSettings/NatDevice.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class NatDeviceProjectSettings : ScriptableSingleton<NatDeviceProjectSettings> {

        #region --Client API--
        /// <summary>
        /// NatDevice settings from the current project settings.
        /// </summary>
        internal static NatDeviceSettings CurrentSettings {
            get {
                var settings = ScriptableObject.CreateInstance<NatDeviceSettings>();
                settings.token = SessionState.GetString(tokenKey, string.Empty);
                return settings;
            }
        }

        /// <summary>
        /// Create NatDevice settings.
        /// </summary>
        /// <param name="platform">NatML platform identifier.</param>
        /// <param name="bundle">NatML app bundle.</param>
        /// <param name="accessKey">NatML access key.</param>
        internal static NatDeviceSettings CreateSettings (string platform, string bundle, string accessKey) {
            var input = new CreateMediaSessionRequest.Input {
                api = NatDeviceSettings.API,
                version = NatDeviceSettings.Version,
                platform = platform,
                bundle = bundle
            };
            var session = Task.Run(() => NatMLHub.CreateMediaSession(input, accessKey)).Result;
            var settings = ScriptableObject.CreateInstance<NatDeviceSettings>();
            settings.token = session.token;
            return settings;
        }
        #endregion


        #region --Operations--
        private static string tokenKey => $"{NatDeviceSettings.API}.token";

        [InitializeOnLoadMethod]
        private static void OnLoad () {
            NatDeviceSettings.Instance = CurrentSettings;
            HubSettings.OnUpdateSettings += OnUpdateHubSettings;
        }

        private static void OnUpdateHubSettings (HubSettings hubSettings) {
            try {
                var settings = CreateSettings(NatMLHub.CurrentPlatform, NatMLHub.GetEditorBundle(), hubSettings.AccessKey);
                SessionState.SetString(tokenKey, settings.token);
            } catch (Exception ex) {
                SessionState.EraseString(tokenKey);
                Debug.LogWarning($"NatDevice Error: {ex.InnerException.Message}");
            }
            NatDeviceSettings.Instance = CurrentSettings;
        }
        #endregion
    }
}