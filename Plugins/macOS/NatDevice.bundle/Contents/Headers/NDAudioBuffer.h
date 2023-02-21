//
//  NDAudioBuffer.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 10/28/2021.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

#pragma once

#include "NDMediaDevice.h"

#pragma region --Client API--
/*!
 @function NDAudioBufferData

 @abstract Get the audio data of an audio buffer.

 @discussion Get the audio data of an audio buffer.

 @param audioBuffer
 Audio buffer.
*/
NML_BRIDGE NML_EXPORT float* NML_API NDAudioBufferData (NDSampleBuffer* audioBuffer);

/*!
 @function NDAudioBufferSampleCount

 @abstract Get the total sample count of an audio buffer.

 @discussion Get the total sample count of an audio buffer.

 @param audioBuffer
 Audio buffer.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDAudioBufferSampleCount (NDSampleBuffer* audioBuffer);

/*!
 @function NDAudioBufferSampleRate

 @abstract Get the sample rate of an audio buffer.

 @discussion Get the sample rate of an audio buffer.

 @param audioBuffer
 Audio buffer.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDAudioBufferSampleRate (NDSampleBuffer* audioBuffer);

/*!
 @function NDAudioBufferChannelCount

 @abstract Get the channel count of an audio buffer.

 @discussion Get the channel count of an audio buffer.

 @param audioBuffer
 Audio buffer.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDAudioBufferChannelCount (NDSampleBuffer* audioBuffer);

/*!
 @function NDAudioBufferTimestamp

 @abstract Get the timestamp of an audio buffer.

 @discussion Get the timestamp of an audio buffer.

 @param audioBuffer
 Audio buffer.
*/
NML_BRIDGE NML_EXPORT int64_t NML_API NDAudioBufferTimestamp (NDSampleBuffer* audioBuffer);
#pragma endregion