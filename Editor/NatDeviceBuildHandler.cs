/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Devices.Editor {

    using System;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using Hub;
    using Hub.Editor;
    using Hub.Internal;
    using Hub.Requests;
    using Internal;

    internal sealed class NatDeviceBuildHandler : BuildEmbedHelper<NatDeviceSettings> {

        protected override BuildTarget[] SupportedTargets => new [] {
            BuildTarget.Android,
            BuildTarget.iOS,
            BuildTarget.StandaloneOSX,
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneWindows64,
        };

        protected override NatDeviceSettings[] CreateEmbeds (BuildReport report) {
            // Create app token
            var input = new CreateAppTokenRequest.Input {
                api = NatDeviceSettings.API,
                version = NatDeviceSettings.Version,
                platform = ToPlatform(report.summary.platform),
                bundle = BundleIdentifierOverride?.identifier ?? Application.identifier
            };
            var accessKey = HubSettings.Instance.AccessKey;
            var settings = NatDeviceSettings.Instance;
            settings.Token = null;
            try {
                settings.Token = Task.Run(() => NatMLHub.CreateAppToken(input, accessKey)).Result;
            } catch (Exception ex) {
                Debug.LogWarning($"NatDevice Error: {ex.InnerException.Message}");
            }
            return new [] { settings };
        }

        protected override void ClearEmbeds (BuildReport report) {
            base.ClearEmbeds(report);
            NatDeviceSettings.UpdateToken(HubSettings.Instance);
        }
    }
}