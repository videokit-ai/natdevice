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
 Input array to be populated with audio devices.
 
 @param size
 Input array size.

 @param count
 Output device count.

 @returns Status code.
 */
NML_BRIDGE NML_EXPORT NDMediaStatus NML_API NDGetAudioDevices (
    NDMediaDevice** devices,
    int32_t size,
    int32_t* count
);

/*!
 @function NDEchoCancellation
 
 @abstract Get the device echo cancellation mode.
 
 @discussion Get the device echo cancellation mode.
 
 @param audioDevice
 Audio device.
 
 @returns True if the device performs adaptive echo cancellation.
 */
NML_BRIDGE NML_EXPORT bool NML_API NDAudioDeviceEchoCancellation (NDMediaDevice* audioDevice);

/*!
 @function NDAudioDeviceSetEchoCancellation
 
 @abstract Enable or disable echo cancellation on the device.
 
 @discussion If the device does not support echo cancellation, this will be a nop.
 
 @param audioDevice
 Audio device.
 
 @param echoCancellation
 Echo cancellation.
 */
NML_BRIDGE NML_EXPORT void NML_API NDAudioDeviceSetEchoCancellation (
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
NML_BRIDGE NML_EXPORT int32_t NML_API NDAudioDeviceSampleRate (NDMediaDevice* audioDevice);

/*!
 @function NDAudioDeviceSetSampleRate
 
 @abstract Set the audio device sample rate.
 
 @discussion Set the audio device sample rate.
 
 @param audioDevice
 Audio device.
 
 @param sampleRate
 Sample rate to set.
 */
NML_BRIDGE NML_EXPORT void NML_API NDAudioDeviceSetSampleRate (
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
NML_BRIDGE NML_EXPORT int32_t NML_API NDAudioDeviceChannelCount (NDMediaDevice* audioDevice);

/*!
 @function NDAudioDeviceSetChannelCount
 
 @abstract Set the audio device channel count.
 
 @discussion Set the audio device channel count.
 
 @param audioDevice
 Audio device.
 
 @param channelCount
 Channel count to set.
 */
NML_BRIDGE NML_EXPORT void NML_API NDAudioDeviceSetChannelCount (
    NDMediaDevice* audioDevice,
    int32_t channelCount
);
#pragma endregion