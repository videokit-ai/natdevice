//
//  NDNativeDevice.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 1/3/2020.
//  Copyright © 2023 NatML Inc. All rights reserved.
//

@import AVFoundation;
#include "NatDevice.h"

@protocol NDNativeDevice;

typedef void (^NDSampleBufferBlock) (id _Nonnull sampleBuffer);
typedef void (^NDDisconnectBlock) (id<NDNativeDevice> _Nonnull device);

@protocol NDNativeDevice <NSObject>
@required
@property (readonly, nonnull) NSString* uniqueID;
@property (readonly, nonnull) NSString* name;
@property (readonly) NDMediaDeviceFlags flags;
@property (readonly) bool running;
@property (nullable) NDDisconnectBlock disconnectHandler;
- (void) startRunning:(nonnull NDSampleBufferBlock) sampleBufferHandler;
- (void) stopRunning;
@end

@interface NDAudioDevice : NSObject <NDNativeDevice>
// Introspection
- (nonnull instancetype) initWithPort:(nonnull AVAudioSessionPortDescription*) port;
@property (readonly, nonnull) AVAudioSessionPortDescription* port;
// Settings
@property bool echoCancellation;
@property int sampleRate;
@property int channelCount;
@end

@interface NDCameraDevice : NSObject <NDNativeDevice>
// Introspection
- (nonnull instancetype) initWithDevice:(nonnull AVCaptureDevice*) device;
@property (readonly, nonnull) AVCaptureDevice* device;
@property (readonly) CGSize fieldOfView;
// Settings
@property CGSize previewResolution;
@property CGSize photoResolution;
@property int frameRate;
@property NDFlashMode flashMode;
@property NDVideoStabilizationMode videoStabilizationMode;
- (void) capturePhoto:(nonnull NDSampleBufferBlock) photoBufferBlock;
@end
