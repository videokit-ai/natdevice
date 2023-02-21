# NatDevice
NatDevice is a high performance, cross-platform camera and microphone streaming API for Unity Engine. To start, discover available media devices:
```csharp
// Create a media device query for a camera device
var filter = MediaDeviceCriteria.CameraDevice;
var query = new MediaDeviceQuery(filter);
// Get the first camera device
var device = query.current as CameraDevice;
```

Then stream camera images or audio buffers from the device:
```csharp
// Start streaming camera images
device.StartRunning(OnCameraImage);

void OnCameraImage (CameraImage image) {
    // Do stuff
    ...
}
```

From here, you can utilize extensive functionality provided by the API to build your interactive applications. Features include:

+ **Ultra-low Latency**. Stream the camera preview and microphone audio into Unity with extremely small overhead.

+ **Ultra-high Resolution**. NatDevice supports high resolution camera previews, at full HD and higher on devices that support it.

+ **Extensive Camera Control**. Set the camera exposure mode, exposure point, exposure duration, focus mode, focus point, flash mode, white balance mode, torch mode, zoom, and more.

+ **Granular Microphone Control**. Configure the microphone sample rate, channel count, and echo cancellation mode on devices that support it.

+ **Machine Learning Integration**. NatDevice tightly integrates with [NatML](https://github.com/natmlx/NatML) for building machine learning and computer vision apps.

+ **Video Recording Integration**. NatDevice tightly integrates with [NatCorder](https://github.com/natmlx/NatCorder) for enabling user-generated content.

[See the docs](https://docs.natml.ai/natdevice) to learn more about NatDevice.

## Installing NatDevice
First, add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.natdevice": "1.3.3"
  }
}
```
Then retrieve your access key from [NatML Hub](https://hub.natml.ai/profile) and add it to your Project Settings:

![Specifying your access key](.media/key.png)

> Using NatDevice requires an active [NatML MediaKit](https://natml.ai/mediakit) subscription. You can try it out for free, but functionality is limited. [See the docs](https://docs.natml.ai/natdevice/prelims/faq) for more info.

___

## Requirements
- Unity 2021.2+

## Supported Platforms
- Android API Level 24+
- iOS 14+
- macOS 10.15+ (Apple Silicon and Intel)
- Windows 10+ (64-bit only)
- WebGL:
  - Chrome 91+
  - Firefox 90+

## Resources
- Join the [NatML community on Discord](https://natml.ai/community).
- See the [NatDevice documentation](https://docs.natml.ai/natdevice).
- Check out [NatML on GitHub](https://github.com/natmlx).
- Read the [NatML blog](https://blog.natml.ai/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).