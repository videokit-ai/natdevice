//
//  NDAudioDeviceBuffer.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 5/7/2022.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

@import AVFoundation;

@interface NDAudioDeviceBuffer : NSObject
@property (readonly, nonnull) CMSampleBufferRef sampleBuffer;
- (nonnull instancetype) initWithSampleBuffer:(nonnull CMSampleBufferRef) sampleBuffer;
@end
