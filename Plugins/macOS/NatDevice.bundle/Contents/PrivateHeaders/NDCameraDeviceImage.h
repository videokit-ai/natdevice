//
//  NDCameraDeviceImage.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 12/5/2021.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

@import AVFoundation;

@interface NDCameraDeviceImage : NSObject
@property (readonly, nonnull) CVPixelBufferRef pixelBuffer;
@property (readonly) UInt64 timestamp;
@property (readonly) bool verticallyMirrored;
@property (readonly) bool hasIntrinsicMatrix;
@property (readonly) matrix_float3x3 intrinsicMatrix;
@property (readonly, nonnull) NSDictionary* metadata;
- (nonnull instancetype) initWithSampleBuffer:(nonnull CMSampleBufferRef) sampleBuffer andMirror:(bool) mirror;
- (nonnull instancetype) initWithPhoto:(nonnull AVCapturePhoto*) photo andMirror:(bool) mirror;
@end
