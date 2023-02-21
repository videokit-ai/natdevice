/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All rights reserved.
*/

namespace NatML.Devices.Editor {

    using UnityEditor;
    using Internal;

    internal static class NatDeviceMenu {

        private const int BasePriority = 50;
        
        [MenuItem(@"NatML/NatDevice " + NatDeviceSettings.Version, false, BasePriority)]
        private static void Version () { }

        [MenuItem(@"NatML/NatDevice " + NatDeviceSettings.Version, true, BasePriority)]
        private static bool DisableVersion () => false;

        [MenuItem(@"NatML/View NatDevice Docs", false, BasePriority + 1)]
        private static void OpenDocs () => Help.BrowseURL(@"https://docs.natml.ai/natdevice");

        [MenuItem(@"NatML/Open a NatDevice Issue", false, BasePriority + 2)]
        private static void OpenIssue () => Help.BrowseURL(@"https://github.com/natmlx/NatDevice");
    }
}