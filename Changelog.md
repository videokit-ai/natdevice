## 1.2.5
+ Fixed sporadic crash when `CameraDevice.flashMode` is set on Android.
+ Fixed `CameraDevice.StartRunning` method blocking for a long time on Windows (#3).
+ Fixed `CameraDevice.CapturePhoto` raising an exception on Windows (#7).

## 1.2.4
+ Fixed compiler errors in `CameraImage` struct on Unity 2021.1.
+ Fixed `Failed to validate app token` error after setting valid NatML access key in Project Settings.

## 1.2.3
+ Added support for WebGL. NatDevice now brings all of its features to the web, with no changes in code.
+ Added `IMediaOutput<T>` interface for working with media device outputs.
+ Added `AudioSpectrumOutput` output for computing the audio spectrum of audio buffers using the Fast Fourier Transform.
+ Added `AudioBuffer` constructor that accepts a native `float*` buffer.
+ Added `CameraImage.rowStride` property for retrieving the row stride of interleaved camera images.
+ Added `TextureOutput.orientation` convenience property for getting and setting the texture orientation.
+ Fixed some `CameraDevice` instances not being discovered on low-end Android models.
+ Fixed bug where continuous autoexposure routine stopped after setting exposure point on iOS (#1).
+ Removed `TextureOutput.pixelBufferOutput` property. This is now an implementation detail that is subject to change.
+ Removed `TextureOutput` constructor that accepts `PixelBufferOutput`. The constructor is now parameterless.

## 1.2.2
+ Fixed crash after `CameraDevice.CapturePhoto` callback is invoked on iOS and macOS.
+ Fixed `CameraDevice.onDisconnected` event not being raised on macOS.
+ Fixed `AudioDevice.onDisconnected` event not being raised on macOS.
+ Upgraded to Hub 1.0.8.

## 1.2.1
+ Rewrote Android implementation from scratch in C++ to improve performance and significantly reduce fatal crashes from errors.
+ Added `RenderTextureOutput` for streaming camera images into `RenderTexture` for display.
+ Added generic `IMediaDevice<T>` interface for working with specific types of media devices.
+ Added `IMediaDevice.onDisconnected` event which is raised when the media device is disconnected.
+ Added `AudioBuffer` constructor for creating audio buffers for use with device outputs.
+ Added `CameraDevice.TorchMode` enumeration  for finer-grained control of camera torch mode.
+ Added `CameraDevice.torchMode` property for getting and setting the camera torch mode.
+ Added `CameraDevice.WhiteBalanceMode` enumeration for finer-grained control of camera white balance mode.
+ Added `CameraDevice.WhiteBalanceModeSupported` method for checking whether a given white balance mode is supported.
+ Added `CameraDevice.whiteBalanceMode` property for getting and setting the camera white balance mode.
+ Added `CameraDevice.VideoStabilizationMode` enumeration for control of camera video stabilization mode.
+ Added `CameraDevice.VideoStabilizationModeSupported` method for checking whether a given video stabilization mode is supported.
+ Added `CameraDevice.videoStabilizationMode` property for getting and setting the camera video stabilization mode.
+ Added `PixelBufferOutput.ConversionOptions` class for specifying options like rotation and mirroring when converting camera images.
+ Added `TextureOutput.ConversionOptions` class for specifying options like rotation and mirroring when converting camera images.
+ Updated `AudioBuffer` from a `class` to a `readonly struct` for better memory behaviour.
+ Updated `CameraImage` from a `class` to a `readonly struct` for better memory behaviour.
+ Improved performance and memory behaviour of `IMediaDevice.uniqueID` and `IMediaDevice.name` properties.
+ Fixed camera image plane buffer size being incorrect on iOS and macOS.
+ Fixed high CPU usage when streaming from an `AudioDevice` on Android.
+ Fixed `AudioDevice` failing to start running on some Android devices.
+ Fixed `AudioDevice.echoCancellationSupported` incorrectly reporting `false` on a large number of microphones.
+ Fixed `AudioDevice.defaultForMediaType` not updating when the default device is changed on Windows.
+ Fixed crash when creating a `MediaDeviceQuery` when system does not have any microphones on Windows.
+ Fixed crash after stopping a `CameraDevice` on Windows.
+ Fixed rare `SIGFPE` crash when accessing `CameraDevice.frameRate` on macOS.
+ Fixed rare `IllegalStateException` when running a `CameraDevice` on Android.
+ Fixed rare bug where some `CameraDevice` preview and photo resolutions were unavailable on Android.
+ Fixed bug where `AudioDevice` fails to stream after creating a second `MediaDeviceQuery` on iOS.
+ Deprecated `CameraDevice.torchEnabled` property. Use `CameraDevice.torchMode` property instead.
+ Deprecated `CameraDevice.whiteBalanceLockSupported` property. Use `CameraDevice.WhiteBalanceModeSupported` method instead.
+ Deprecated `CameraDevice.whiteBalanceLock` property. Use `CameraDevice.whiteBalanceMode` property instead.
+ Refactored top-level namespace from `NatSuite.Devices` to `NatML.Devices`.

## 1.2.0
+ Greatly reduced processing and memory cost when streaming camera preview on iOS.
+ Improved audio timestamp accuracy for audio buffers on Android.
+ Reduced memory consumption when streaming microphone audio on Android.
+ Added `CameraImage` class which provides raw pixel buffers from camera devices with zero memory copies and EXIF metadata.
+ Added `AudioBuffer` class which provides raw audio sample buffers from media devices with zero memory copies.
+ Added native `CameraDevice` support on macOS, bringing features like focus control, exposure control, torch, and so on.
+ Added native `CameraDevice` support on Windows.
+ Added support for manual exposure with `CameraDevice.SetExposureDuration` method.
+ Added `IMediaDevice.defaultForMediaType` property for checking if a device is the system default for its media type.
+ Added `CameraDevice.exposureMode` property for setting the exposure mode.
+ Added `CameraDevice.ExposureModeSupported` method to check if camera device supports a given exposure mode.
+ Added `CameraDevice.focusMode` property for setting the focus mode.
+ Added `CameraDevice.FocusModeSupported` method to check if camera device supports a given focus mode.
+ Added `CameraDevice.exposureDurationRange` property for setting camera device manual exposure.
+ Added `CameraDevice.exposureBiasRange` property for setting camera device auto exposure.
+ Added `CameraDevice.onDisconnected` event which is raised when the camera device is disconnected.
+ Added `MediaDeviceQuery.CheckPermissions` method for checking current permission status for a given media device type.
+ Added `MediaDeviceQuery.ConfigureAudioSession` static property for configuring app audio session on iOS.
+ Fixed crash when `AudioDevice.StartRunning` is called while the microphone is in use by another app on iOS.
+ Fixed `CameraDevice` incorrectly reporting that locked focus mode is not supported on some Android devices.
+ Fixed `CameraDevice` incorrectly reporting that focus is not locked when it is on some Android devices.
+ Fixed `MediaDeviceQuery.RequestPermissions` taking several seconds to complete even when permissions have been granted by the user.
+ Refactored `DeviceType` enumeration to `DeviceLocation`.
+ Refactored `IMediaDevice.type` property to `IMediaDevice.location`.
+ Refactored `FlashMode` enumeration to `CameraDevice.FlashMode`.
+ Deprecated `CameraDevice.exposureLock` property. Use `CameraDevice.exposureMode` property instead.
+ Deprecated `CameraDevice.exposureLockSupported` property. Use `CameraDevice.ExposureModeSupported` method instead.
+ Deprecated `CameraDevice.exposureRange` property. Use `CameraDevice.exposureBiasRange` property instead.
+ Deprecated `CameraDevice.exposurePoint` property. Use `CameraDevice.SetExposurePoint` method instead.
+ Deprecated `CameraDevice.focusLock` property. Use `CameraDevice.focusMode` property instead.
+ Deprecated `CameraDevice.focusLockSupported` property. Use `CameraDevice.FocusModeSupported` method instead.
+ Deprecated `CameraDevice.focusPoint` property. Use `CameraDevice.SetFocusPoint` method instead.

## 1.1.0
+ Greatly reduced camera preview memory consumption on Android.
+ Greatly reduced camera preview latency on Android.
+ Greatly reduced microphone streaming latency on iOS.
+ Added `IMediaDevice.type` enumeration for checking if device is internal or external.
+ Added `AudioDevice.StartRunning` overload with callback that is given the native sample buffer handle with no memory copy.
+ Added `AudioDevice.echoCancellationSupported` boolean for checking if audio device supports echo cancellation.
+ Added `CameraDevice.StartRunning` overload with callback that is given the raw pixel buffer in managed memory.
+ Added `CameraDevice.StartRunning` overload with callback that is given the native pixel buffer handle with no memory copy.
+ Added `CameraDevice.exposurePointSupported` for checking if camera supports setting exposure point.
+ Added `CameraDevice.focusPointSupported` for checking if camera supports setting focus point.
+ Added `MediaDeviceQuery.count` property.
+ Added `MediaDeviceCriteria.Internal` criterion for finding internal media devices.
+ Added `MediaDeviceCriteria.External` criterion for finding external media devices.
+ Added `MediaDeviceCriteria.EchoCancellation` criterion for finding microphones that perform echo cancellation.
+ Added `MediaDeviceCriteria.Any` function for creating a criterion that requires any of the provided sub-criteria.
+ Added `MediaDeviceCriteria.All` function for creating a criterion that requires all of the provided sub-criteria.
+ Added support for Apple Silicon on macOS.
+ Changed `CameraDevice` behaviour to no longer pause and resume when app is suspended. **You must handle this yourself**.
+ Fixed flash not firing on some Android devices.
+ Fixed preview resolution setting being ignored on iOS.
+ Fixed hardware microphone format being ignored on macOS.
+ Fixed memory leak in photo capture on iOS.
+ Fixed memory leak when `CameraDevice` is stopped on macOS and Windows.
+ Fixed crash when focus point is set on Sony Xperia devices.
+ Fixed non-ASCII characters in `AudioDevice` name not being captured on Windows.
+ Fixed media device query permission task never completing on Android.
+ Fixed media device query permission task never completing on Windows.
+ Fixed permissions dialog not being presented on UWP.
+ Refactored `MediaDeviceQuery.Criteria` to `MediaDeviceCriteria`.
+ Refactored `MediaDeviceCriteria.RearFacing` to `MediaDeviceCriteria.RearCamera`.
+ Refactored `MediaDeviceCriteria.FrontFacing` to `MediaDeviceCriteria.FrontCamera`.
+ Refactored `MediaDeviceQuery.currentDevice` to `MediaDeviceQuery.current`.
+ Deprecated `MediaDeviceQuery.devices` property. The query now acts as a collection itself.
+ Removed `IAudioDevice` interface. All microphones are now exposed as `AudioDevice` instances.
+ Removed `ICameraDevice` interface. All cameras are now exposed as `CameraDevice` instances.
+ Removed `WebCameraDevice` class.
+ Removed `MixerDevice` class.
+ Removed `MediaDeviceCriteria.GenericCameraDevice`.
+ Removed `FrameOrientation` enumeration. Use Unity's `ScreenOrientation` enumeration instead.
+ NatDevice now requires Android API level 24+.
+ NatDevice now requires iOS 13+.
+ NatDevice now requires macOS 10.15+.

## 1.0.2
+ Moved documentation [online](https://docs.natml.ai/natdevice/).
+ Added native permissions requests on iOS and macOS.
+ Echo cancellation can now be enabled and disabled on `AudioDevice` instances that support it.
+ Changed `MediaDeviceQuery` to only accept a single criterion, instead of multiple.
+ Fixed hard crash on iPhone 6 when `MediaDeviceQuery` is created.
+ Fixed `AudioDevice` causing NatCorder to crash when recording is stopped on iOS.
+ Fixed `AudioDevice` reporting incorrect format before the device starts running on iOS.
+ Deprecated `MediaDeviceQuery.count` property. Use `MediaDeviceQuery.devices.Length` instead.
+ Deprecated `MediaDeviceQuery.Criterion` delegate type. Use `System.Predicate` delegate from .NET BCL instead.
+ Deprecated `MediaDeviceQuery.Criteria.EchoCancellation` criterion as it is no longer useful.

## 1.0.1
+ Updated top-level namespace to `NatSuite.Devices` for parity with other NatSuite API's.
+ Fixed camera device query crash on Galaxy S10 and S10+.
+ Fixed sporadic crashes on some Android devices when the camera preview is started.
+ Fixed crash due to JNI local reference table overflow on Android.
+ Fixed `MediaDeviceQuery.Criteria.FrontFacing` not finding any cameras on iOS.
+ Fixed iOS archive generating error due to NatDevice not being built with full bitcode generation.

## 1.0.0
+ First release.