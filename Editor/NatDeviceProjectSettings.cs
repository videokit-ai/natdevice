/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All rights reserved.
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
        /// Create NatDevice settings from the current project settings.
        /// </summary>
        internal static NatDeviceSettings CreateSettings () {
            var settings = ScriptableObject.CreateInstance<NatDeviceSettings>();
            settings.token = SessionState.GetString(TokenKey, string.Empty);
            return settings;
        }
        #endregion


        #region --Operations--
        private static string TokenKey => $"{NatDeviceSettings.API}.token";

        [InitializeOnLoadMethod]
        private static void OnLoad () {
            NatDeviceSettings.Instance = CreateSettings();
            HubSettings.OnUpdateSettings += OnUpdateHubSettings;
        }

        private static void OnUpdateHubSettings (HubSettings settings) {
            var input = new CreateAppTokenRequest.Input {
                api         = NatDeviceSettings.API,
                version     = NatDeviceSettings.Version,
                platform    = NatMLHub.CurrentPlatform,
                bundle      = HubSettings.EditorBundle
            };
            try {
                var token = Task.Run(() => NatMLHub.CreateAppToken(input, settings.AccessKey)).Result;
                SessionState.SetString(TokenKey, token);
            } catch (Exception ex) {
                SessionState.EraseString(TokenKey);
                Debug.LogWarning($"NatDevice Error: {ex.InnerException.Message}");
            }
            NatDeviceSettings.Instance = CreateSettings();
        }
        #endregion
    }
}