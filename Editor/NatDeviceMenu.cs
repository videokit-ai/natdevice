/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Devices.Editor {

    using UnityEditor;

    internal static class NatDeviceMenu {

        private const int BasePriority = 50;
        
        [MenuItem(@"NatML/NatDevice 1.2.2", false, BasePriority)]
        private static void Version () { }

        [MenuItem(@"NatML/NatDevice 1.2.2", true, BasePriority)]
        private static bool DisableVersion () => false;

        [MenuItem(@"NatML/View NatDevice Docs", false, BasePriority + 1)]
        private static void OpenDocs () => Help.BrowseURL(@"https://docs.natml.ai/natdevice");

        [MenuItem(@"NatML/Open a NatDevice Issue", false, BasePriority + 2)]
        private static void OpenIssue () => Help.BrowseURL(@"https://github.com/natmlx/NatDevice");
    }
}