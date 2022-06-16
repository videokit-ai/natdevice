/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using System;
    using System.IO;
    using System.Threading.Tasks;
    using UnityEngine;
    using Hub;
    using Hub.Internal;
    using Hub.Requests;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    internal sealed class NatDeviceSettings : ScriptableObject {

        #region --Data--
        [SerializeField, HideInInspector]
        private string token = string.Empty;
        #endregion


        #region --Client API--
        /// <summary>
        /// Get or set the NatDevice app token.
        /// </summary>
        internal string Token {
            get => !string.IsNullOrEmpty(token) ? token : null;
            set => token = value;
        }

        /// <summary>
        /// NatDevice settings for this project.
        /// </summary>
        public static NatDeviceSettings Instance {
            get {
                #if UNITY_EDITOR
                // Check
                if (settings)
                    return settings;
                // Check
                if (EditorBuildSettings.TryGetConfigObject<NatDeviceSettings>(SettingsIdentifier, out settings))
                    return settings;
                // Create
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                settings = ScriptableObject.CreateInstance<NatDeviceSettings>();
                AssetDatabase.CreateAsset(settings, SettingsPath);
                EditorBuildSettings.AddConfigObject(SettingsIdentifier, settings, true);
                return settings;
                #else
                return settings;
                #endif
            }
        }
        #endregion


        #region --Operations--
        public const string API = @"ai.natml.natdevice";
        public const string Version = @"1.2.1";
        private static NatDeviceSettings settings;
        internal static string SettingsIdentifier => $"{API}.settings";
        private const string SettingsPath = @"Assets/NatML/Settings/NatDevice.asset";

        static NatDeviceSettings () => HubSettings.OnUpdateSettings += UpdateToken;

        void OnEnable () => settings = this;

        internal static void UpdateToken (HubSettings hubSettings) {
            var input = new CreateAppTokenRequest.Input {
                api = API,
                version = Version,
                platform = NatMLHub.CurrentPlatform,
                bundle = HubSettings.EditorBundle
            };
            try {
                Instance.token = Task.Run(() => NatMLHub.CreateAppToken(input, hubSettings.AccessKey)).Result;
            } catch (Exception ex) {
                Debug.LogWarning($"NatDevice Error: {ex.InnerException.Message}");
            }
        }
        #endregion
    }
}