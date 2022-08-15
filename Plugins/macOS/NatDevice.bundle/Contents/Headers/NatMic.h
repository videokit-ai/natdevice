//
//  NatMic.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 1/14/2021.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

#pragma once

#include "NDMediaDevice.h"

#pragma region --Client API--
/*!
 @function NDGetAudioDevices
 
 @abstract Get all available audio devices.
 
 @discussion Get all available audio devices.
 
 @param devices
 Array populated with audio devices.
 
 @param size
 Array size. On return, this will contain the current count of devices in the array.

 @returns Status code.
 */
BRIDGE EXPORT NDStatus APIENTRY NDGetAudioDevices (
    NDMediaDevice** devices,
    int32_t* size
);

/*!
 @function NDEchoCancellation
 
 @abstract Get the device echo cancellation mode.
 
 @discussion Get the device echo cancellation mode.
 
 @param audioDevice
 Audio device.
 
 @returns True if the device performs adaptive echo cancellation.
 */
BRIDGE EXPORT bool APIENTRY NDAudioDeviceEchoCancellation (NDMediaDevice* audioDevice);

/*!
 @function NDAudioDeviceSetEchoCancellation
 
 @abstract Enable or disable echo cancellation on the device.
 
 @discussion If the device does not support echo cancellation, this will be a nop.
 
 @param audioDevice
 Audio device.
 
 @param echoCancellation
 Echo cancellation.
 */
BRIDGE EXPORT void APIENTRY NDAudioDeviceSetEchoCancellation (
    NDMediaDevice* audioDevice,
    bool echoCancellation
);

/*!
 @function NDAudioDeviceSampleRate
 
 @abstract Audio device sample rate.
 
 @discussion Audio device sample rate.
 
 @param audioDevice
 Audio device.
 
 @returns Current sample rate.
 */
BRIDGE EXPORT int32_t APIENTRY NDAudioDeviceSampleRate (NDMediaDevice* audioDevice);

/*!
 @function NDAudioDeviceSetSampleRate
 
 @abstract Set the audio device sample rate.
 
 @discussion Set the audio device sample rate.
 
 @param audioDevice
 Audio device.
 
 @param sampleRate
 Sample rate to set.
 */
BRIDGE EXPORT void APIENTRY NDAudioDeviceSetSampleRate (
    NDMediaDevice* audioDevice,
    int32_t sampleRate
);

/*!
 @function NDAudioDeviceChannelCount
 
 @abstract Audio device channel count.
 
 @discussion Audio device channel count.
 
 @param audioDevice
 Audio device.
 
 @returns Current channel count.
 */
BRIDGE EXPORT int32_t APIENTRY NDAudioDeviceChannelCount (NDMediaDevice* audioDevice);

/*!
 @function NDAudioDeviceSetChannelCount
 
 @abstract Set the audio device channel count.
 
 @discussion Set the audio device channel count.
 
 @param audioDevice
 Audio device.
 
 @param channelCount
 Channel count to set.
 */
BRIDGE EXPORT void APIENTRY NDAudioDeviceSetChannelCount (
    NDMediaDevice* audioDevice,
    int32_t channelCount
);
#pragma endregion