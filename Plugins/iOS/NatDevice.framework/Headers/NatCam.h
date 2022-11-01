//
//  NatCam.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 1/14/2021.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

#pragma once

#include "NDMediaDevice.h"

#pragma region --Enumerations--
/*!
 @enum NDExposureMode

 @abstract Camera device exposure mode.

 @constant ND_EXPOSURE_MODE_CONTINUOUS
 Continuous auto exposure.

 @constant ND_EXPOSURE_MODE_LOCKED
 Locked exposure. Exposure settings will be fixed to their current values.
 Requires `ND_CAMERA_FLAG_LOCKED_EXPOSURE` device flag.

 @constant ND_EXPOSURE_MODE_MANUAL
 Manual exposure. User will set exposure duration and sensitivity.
 Requires `ND_CAMERA_FLAG_MANUAL_EXPOSURE` device flag.
*/
enum NDExposureMode {
    ND_EXPOSURE_MODE_CONTINUOUS         = 0,
    ND_EXPOSURE_MODE_LOCKED             = 1,
    ND_EXPOSURE_MODE_MANUAL             = 2
};
typedef enum NDExposureMode NDExposureMode;

/*!
 @enum NDFlashMode

 @abstract Camera device photo flash modes.

 @constant ND_FLASH_MODE_OFF
 The flash will never be fired.

 @constant ND_FLASH_MODE_ON
 The flash will always be fired.

 @constant ND_FLASH_MODE_AUTO
 The sensor will determine whether to fire the flash.
*/
enum NDFlashMode {
    ND_FLASH_MODE_OFF       = 0,
    ND_FLASH_MODE_ON        = 1,
    ND_FLASH_MODE_AUTO      = 2
};
typedef enum NDFlashMode NDFlashMode;

/*!
 @enum NDFocusMode

 @abstract Camera device focus mode.

 @constant ND_FOCUS_MODE_CONTINUOUS
 Continuous auto focus.

 @constant ND_FOCUS_MODE_LOCKED
 Locked auto focus. Focus settings will be fixed to their current values.
 Requires `ND_CAMERA_FLAG_FOCUS_LOCK` device flag.
*/
enum NDFocusMode {
    ND_FOCUS_MODE_CONTINUOUS    = 0,
    ND_FOCUS_MODE_LOCKED        = 1,
};
typedef enum NDFocusMode NDFocusMode;

/*!
 @enum NDTorchMode

 @abstract Camera device torch mode.

 @constant ND_TORCH_MODE_OFF
 Disabled torch mode.

 @constant ND_TORCH_MODE_MAXIMUM
 Maximum torch mode.
 Requires `ND_CAMERA_FLAG_TORCH` device flag.
*/
enum NDTorchMode {
    ND_TORCH_MODE_OFF       = 0,
    ND_TORCH_MODE_MAXIMUM   = 100,
};
typedef enum NDTorchMode NDTorchMode;

/*!
 @enum NDVideoStabilizationMode

 @abstract Camera device video stabilization mode.

 @constant ND_VIDEO_STABILIZATION_OFF
 Disabled video stabilization.

 @constant ND_VIDEO_STABILIZATION_STANDARD
 Standard video stabilization
 Requires `ND_CAMERA_FLAG_VIDEO_STABILIZATION` device flag.
*/
enum NDVideoStabilizationMode {
    ND_VIDEO_STABILIZATION_OFF      = 0,
    ND_VIDEO_STABILIZATION_STANDARD = 1,
};
typedef enum NDVideoStabilizationMode NDVideoStabilizationMode;

/*!
 @enum NDWhiteBalanceMode

 @abstract Camera device white balance mode.

 @constant ND_WHITE_BALANCE_MODE_CONTINUOUS
 Continuous auto white balance.

 @constant ND_WHITE_BALANCE_MODE_LOCKED
 Locked auto white balance. White balance settings will be fixed to their current values.
 Requires `ND_CAMERA_FLAG_WHITE_BALANCE_LOCK` device flag.
*/
enum NDWhiteBalanceMode {
    ND_WHITE_BALANCE_MODE_CONTINUOUS    = 0,
    ND_WHITE_BALANCE_MODE_LOCKED        = 1,
};
typedef enum NDWhiteBalanceMode NDWhiteBalanceMode;
#pragma endregion


#pragma region --Client API--
/*!
 @function NDGetCameraDevices

 @abstract Get all available camera devices.

 @discussion Get all available camera devices.

 @param devices
 Input array to be populated with camera devices.

 @param size
 Input array size.

 @param count
 Output device count.

 @returns Status code.
*/
NML_BRIDGE NML_EXPORT NDMediaStatus NML_API NDGetCameraDevices (
    NDMediaDevice** devices,
    int32_t size,
    int32_t* count
);

/*!
 @function NDCameraDeviceFieldOfView

 @abstract Camera field of view in degrees.

 @discussion Camera field of view in degrees.

 @param cameraDevice
 Camera device.

 @param outWidth
 Output FOV width in degrees.

 @param outHeight
 Output FOV height in degrees.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceFieldOfView (
    NDMediaDevice* cameraDevice,
    float* outWidth,
    float* outHeight
);

/*!
 @function NDCameraDeviceExposureBiasRange

 @abstract Camera exposure bias range in EV.

 @discussion Camera exposure bias range in EV.

 @param cameraDevice
 Camera device.

 @param outMin
 Output minimum exposure bias.

 @param outMax
 Output maximum exposure bias.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceExposureBiasRange (
    NDMediaDevice* cameraDevice,
    float* outMin,
    float* outMax
);

/*!
 @function NDCameraDeviceExposureDurationRange

 @abstract Camera exposure duration range in seconds.

 @discussion Camera exposure duration range in seconds.

 @param cameraDevice
 Camera device.

 @param outMin
 Output minimum exposure duration in seconds.

 @param outMax
 Output maximum exposure duration in seconds.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceExposureDurationRange (
    NDMediaDevice* cameraDevice,
    float* outMin,
    float* outMax
);

/*!
 @function NDCameraDeviceISORange

 @abstract Camera sensor sensitivity range.

 @discussion Camera sensor sensitivity range.

 @param cameraDevice
 Camera device.

 @param outMin
 Output minimum ISO sensitivity value.

 @param outMax
 Output maximum ISO sensitivity value.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceISORange (
    NDMediaDevice* cameraDevice,
    float* outMin,
    float* outMax
);

/*!
 @function NDCameraDeviceZoomRange

 @abstract Camera optical zoom range.

 @discussion Camera optical zoom range.

 @param cameraDevice
 Camera device.

 @param outMin
 Output minimum zoom ratio.

 @param outMax
 Output maximum zoom ratio.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceZoomRange (
    NDMediaDevice* cameraDevice, 
    float* outMin, 
    float* outMax
);

/*!
 @function NDCameraDevicePreviewResolution

 @abstract Get the camera preview resolution.

 @discussion Get the camera preview resolution.

 @param cameraDevice
 Camera device.

 @param outWidth
 Output width in pixels.

 @param outHeight
 Output height in pixels.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDevicePreviewResolution (
    NDMediaDevice* cameraDevice,
    int32_t* outWidth,
    int32_t* outHeight
);

/*!
 @function NDCameraDeviceSetPreviewResolution

 @abstract Set the camera preview resolution.

 @discussion Set the camera preview resolution.

 Most camera devices do not support arbitrary preview resolutions, so the camera will
 set a supported resolution which is closest to the requested resolution that is specified.

 Note that this method should only be called before the camera preview is started.

 @param cameraDevice
 Camera device.

 @param width
 Width in pixels.

 @param height
 Height in pixels.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetPreviewResolution (
    NDMediaDevice* cameraDevice,
    int32_t width,
    int32_t height
);

/*!
 @function NDCameraDevicePhotoResolution

 @abstract Get the camera photo resolution.

 @discussion Get the camera photo resolution.

 @param cameraDevice
 Camera device.

 @param outWidth
 Output width in pixels.

 @param outHeight
 Output height in pixels.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDevicePhotoResolution (
    NDMediaDevice* cameraDevice,
    int32_t* outWidth,
    int32_t* outHeight
);

/*!
 @function NDCameraDeviceSetPhotoResolution

 @abstract Set the camera photo resolution.

 @discussion Set the camera photo resolution.

 Most camera devices do not support arbitrary photo resolutions, so the camera will
 set a supported resolution which is closest to the requested resolution that is specified.

 Note that this method should only be called before the camera preview is started.

 @param cameraDevice
 Camera device.

 @param width
 Width in pixels.

 @param height
 Height in pixels.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetPhotoResolution (
    NDMediaDevice* cameraDevice,
    int32_t width,
    int32_t height
);

/*!
 @function NDCameraDeviceFrameRate

 @abstract Get the camera preview frame rate.

 @discussion Get the camera preview frame rate.

 @param cameraDevice
 Camera device.

 @returns Camera preview frame rate.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraDeviceFrameRate (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetFrameRate

 @abstract Set the camera preview frame rate.

 @discussion Set the camera preview frame rate.

 Note that this method should only be called before the camera preview is started.

 @param cameraDevice
 Camera device.

 @param frameRate
 Frame rate to set.
*/
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetFrameRate (
    NDMediaDevice* cameraDevice,
    int32_t frameRate
);

/*!
 @function NDCameraDeviceExposureMode

 @abstract Get the camera exposure mode.

 @discussion Get the camera exposure mode.

 @param cameraDevice
 Camera device.

 @returns Exposure mode.
*/
NML_BRIDGE NML_EXPORT NDExposureMode NML_API NDCameraDeviceExposureMode (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetExposureMode

 @abstract Set the camera exposure mode.

 @discussion Set the camera exposure mode.

 @param cameraDevice
 Camera device.

 @param mode
 Exposure mode.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetExposureMode (
    NDMediaDevice* cameraDevice,
    NDExposureMode mode
);

/*!
 @function NDCameraDeviceExposureBias

 @abstract Get the camera exposure bias.

 @discussion Get the camera exposure bias.

 @param cameraDevice
 Camera device.

 @returns Camera exposure bias.
 */
NML_BRIDGE NML_EXPORT float NML_API NDCameraDeviceExposureBias (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetExposureBias

 @abstract Set the camera exposure bias.

 @discussion Set the camera exposure bias.

 Note that the value MUST be in the camera exposure range.

 @param cameraDevice
 Camera device.

 @param bias
 Exposure bias value to set.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetExposureBias (
    NDMediaDevice* cameraDevice,
    float bias
);

/*!
 @function NDCameraDeviceSetExposurePoint

 @abstract Set the camera exposure point of interest.

 @discussion Set the camera exposure point of interest.
 The coordinates are specified in viewport space, with each value in range [0., 1.].

 @param cameraDevice
 Camera device.

 @param x
 Exposure point x-coordinate in viewport space.

 @param y
 Exposure point y-coordinate in viewport space.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetExposurePoint (
    NDMediaDevice* cameraDevice,
    float x,
    float y
);

/*!
 @function NDCameraDeviceSetExposureDuration

 @abstract Set the camera exposure duration.

 @discussion Set the camera exposure duration.
 This method will automatically change the camera's exposure mode to `MANUAL`.

 @param cameraDevice
 Camera device.

 @param duration
 Exposure duration in seconds. MUST be in `ExposureDurationRange`.

 @param ISO
 Shutter sensitivity. MUST be in `ISORange`.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetExposureDuration (
    NDMediaDevice* cameraDevice,
    float duration,
    float ISO
);

/*!
 @function NDCameraDeviceFlashMode

 @abstract Get the camera photo flash mode.

 @discussion Get the camera photo flash mode.

 @param cameraDevice
 Camera device.

 @returns Camera photo flash mode.
 */
NML_BRIDGE NML_EXPORT NDFlashMode NML_API NDCameraDeviceFlashMode (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetFlashMode

 @abstract Set the camera photo flash mode.

 @discussion Set the camera photo flash mode.

 @param cameraDevice
 Camera device.

 @param mode
 Flash mode to set.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetFlashMode (
    NDMediaDevice* cameraDevice,
    NDFlashMode mode
);

/*!
 @function NDCameraDeviceFocusMode

 @abstract Get the camera focus mode.

 @discussion Get the camera focus mode.

 @param cameraDevice
 Camera device.

 @returns Camera focus mode.
 */
NML_BRIDGE NML_EXPORT NDFocusMode NML_API NDCameraDeviceFocusMode (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetFocusMode

 @abstract Set the camera focus mode.

 @discussion Set the camera focus mode.

 @param cameraDevice
 Camera device.

 @param mode
 Focus mode.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetFocusMode (
    NDMediaDevice* cameraDevice,
    NDFocusMode mode
);

/*!
 @function NDCameraDeviceSetFocusPoint

 @abstract Set the camera focus point.

 @discussion Set the camera focus point of interest.
 The coordinates are specified in viewport space, with each value in range [0., 1.].
 This function should only be used if the camera supports setting the focus point.
 See `NDCameraDeviceFocusPointSupported`.

 @param cameraDevice
 Camera device.

 @param x
 Focus point x-coordinate in viewport space.

 @param y
 Focus point y-coordinate in viewport space.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetFocusPoint (
    NDMediaDevice* cameraDevice,
    float x,
    float y
);

/*!
 @function NDCameraDeviceTorchMode

 @abstract Get the current camera torch mode.

 @discussion Get the current camera torch mode.

 @param cameraDevice
 Camera device.

 @returns Current camera torch mode.
 */
NML_BRIDGE NML_EXPORT NDTorchMode NML_API NDCameraDeviceTorchMode (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetTorchMode

 @abstract Set the camera torch mode.

 @discussion Set the camera torch mode.

 @param cameraDevice
 Camera device.

 @param mode
 Torch mode.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetTorchMode (
    NDMediaDevice* cameraDevice,
    NDTorchMode mode
);

/*!
 @function NDCameraDeviceWhiteBalanceMode

 @abstract Get the camera white balance mode.

 @discussion Get the camera white balance mode.

 @param cameraDevice
 Camera device.

 @returns White balance mode.
 */
NML_BRIDGE NML_EXPORT NDWhiteBalanceMode NML_API NDCameraDeviceWhiteBalanceMode (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetWhiteBalanceMode

 @abstract Set the camera white balance mode.

 @discussion Set the camera white balance mode.

 @param cameraDevice
 Camera device.

 @param mode
 White balance mode.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetWhiteBalanceMode (
    NDMediaDevice* cameraDevice,
    NDWhiteBalanceMode mode
);

/*!
 @function NDCameraDeviceVideoStabilizationMode

 @abstract Get the camera video stabilization mode.

 @discussion Get the camera video stabilization mode.

 @param cameraDevice
 Camera device.

 @returns Video stabilization mode.
 */
NML_BRIDGE NML_EXPORT NDVideoStabilizationMode NML_API NDCameraDeviceVideoStabilizationMode (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetVideoStabilizationMode

 @abstract Set the camera video stabilization mode.

 @discussion Set the camera video stabilization mode.

 @param cameraDevice
 Camera device.

 @param mode
 Video stabilization mode.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetVideoStabilizationMode (
    NDMediaDevice* cameraDevice,
    NDVideoStabilizationMode mode
);

/*!
 @function NDCameraDeviceZoomRatio

 @abstract Get the camera zoom ratio.

 @discussion Get the camera zoom ratio.
 This value will always be within the minimum and maximum zoom values reported by the camera device.

 @param cameraDevice
 Camera device.

 @returns Zoom ratio.
 */
NML_BRIDGE NML_EXPORT float NML_API NDCameraDeviceZoomRatio (NDMediaDevice* cameraDevice);

/*!
 @function NDCameraDeviceSetZoomRatio

 @abstract Set the camera zoom ratio.

 @discussion Set the camera zoom ratio.
 This value must always be within the minimum and maximum zoom values reported by the camera device.

 @param cameraDevice
 Camera device.

 @param ratio
 Zoom ratio.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceSetZoomRatio (
    NDMediaDevice* cameraDevice,
    float ratio
);

/*!
 @function NDCameraDeviceCapturePhoto

 @abstract Capture a still photo.

 @discussion Capture a still photo.

 @param cameraDevice
 Camera device.

 @param handler
 Photo handler.

 @param context
 User-provided context.
 */
NML_BRIDGE NML_EXPORT void NML_API NDCameraDeviceCapturePhoto (
    NDMediaDevice* cameraDevice,
    NDSampleBufferHandler handler,
    void* context
);
#pragma endregion