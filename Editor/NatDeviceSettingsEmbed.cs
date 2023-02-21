/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All rights reserved.
*/

namespace NatML.Devices.Editor {

    using System;
    using System.IO;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using Hub;
    using Hub.Editor;
    using Hub.Internal;
    using Hub.Requests;
    using Internal;

    internal sealed class NatDeviceSettingsEmbed : BuildEmbedHelper<NatDeviceSettings> {

        protected override BuildTarget[] SupportedTargets => new [] {
            BuildTarget.Android,
            BuildTarget.iOS,
            BuildTarget.StandaloneOSX,
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneWindows64,
            BuildTarget.WebGL,
        };
        private const string CachePath = @"Assets/NMLBuildCache";

        protected override NatDeviceSettings[] CreateEmbeds (BuildReport report) {
            var platform = ToPlatform(report.summary.platform);
            var bundle = BundleOverride?.identifier ?? NatMLHub.GetAppBundle(platform);
            var accessKey = HubSettings.Instance.AccessKey;
            var settings = NatDeviceProjectSettings.CurrentSettings;
            try {
                settings = NatDeviceProjectSettings.CreateSettings(platform, bundle, accessKey);
            } catch(Exception ex) {
                Debug.LogWarning($"NatDevice Error: {ex.InnerException.Message}");
            }
            Directory.CreateDirectory(CachePath);
            AssetDatabase.CreateAsset(settings, $"{CachePath}/NatDevice.asset");
            return new [] { settings };
        }

        protected override void ClearEmbeds (BuildReport report) {
            base.ClearEmbeds(report);
            AssetDatabase.DeleteAsset(CachePath);
        }
    }
}