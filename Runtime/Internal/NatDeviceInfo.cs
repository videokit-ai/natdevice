/* 
*   NatDevice
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

using System.Reflection;
using System.Runtime.CompilerServices;
using NatML.Devices.Internal;

// Metadata
[assembly: AssemblyCompany(@"NatML Inc")]
[assembly: AssemblyTitle(@"NatDevice")]
[assembly: AssemblyVersionAttribute(NatDeviceSettings.Version)]
[assembly: AssemblyCopyright(@"Copyright © 2023 NatML Inc. All Rights Reserved.")]

// Friends
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Devices.Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Devices.MultiCam")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Devices.OpenCV")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Devices.Tests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.ML.ARFoundation")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatSuite.Devices.MultiCam")]