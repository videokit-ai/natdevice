//
//  NDCameraImage.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 10/28/2021.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

#pragma once

#include "NDMediaDevice.h"

#pragma region --Enumerations--
/*!
 @enum NDImageFormat

 @abstract Camera image format.

 @constant ND_IMAGE_FORMAT_UNKNOWN
 Unknown or invalid format.

 @constant ND_IMAGE_FORMAT_YCbCr420
 YUV semi-planar format.

 @constant ND_IMAGE_FORMAT_RGBA8888
 RGBA8888 interleaved format.

 @constant ND_IMAGE_FORMAT_BGRA8888
 BGRA8888 interleaved format.
 */
enum NDImageFormat {
    ND_IMAGE_FORMAT_UNKNOWN     = 0,
    ND_IMAGE_FORMAT_YCbCr420    = 1,
    ND_IMAGE_FORMAT_RGBA8888    = 2,
    ND_IMAGE_FORMAT_BGRA8888    = 3,
};
typedef enum NDImageFormat NDImageFormat;

/*!
 @enum NDMetadataKey

 @abstract Sample buffer metadata key.

 @constant ND_IMAGE_INTRINSIC_MATRIX
 Camera intrinsic matrix. Value array must have enough capacity for 9 float values.

 @constant ND_IMAGE_EXPOSURE_BIAS
 Camera image exposure bias value in EV.

 @constant ND_IMAGE_EXPOSURE_DURATION
 Camera image exposure duration in seconds.

 @constant ND_IMAGE_FOCAL_LENGTH
 Camera image focal length.

 @constant ND_IMAGE_F_NUMBER
 Camera image aperture F-number.

 @constant ND_IMAGE_BRIGHTNESS
 Camera image ambient brightness.
*/
enum NDMetadataKey {
    ND_IMAGE_INTRINSIC_MATRIX   = 1,
    ND_IMAGE_EXPOSURE_BIAS      = 2,
    ND_IMAGE_EXPOSURE_DURATION  = 3,
    ND_IMAGE_FOCAL_LENGTH       = 4,
    ND_IMAGE_F_NUMBER           = 5,
    ND_IMAGE_BRIGHTNESS         = 6,
    ND_IMAGE_ISO                = 7,
};
typedef enum NDMetadataKey NDMetadataKey;
#pragma endregion


#pragma region --Client API--
/*!
 @function NDCameraImageData

 @abstract Get the image data of a camera image.

 @discussion Get the image data of a camera image.
 If the camera image uses a planar format, this will return `NULL`.

 @param cameraImage
 Camera image.
*/
NML_BRIDGE NML_EXPORT void* NML_API NDCameraImageData (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImageDataSize

 @abstract Get the image data size of a camera image in bytes.

 @discussion Get the image data size of a camera image in bytes.

 @param cameraImage
 Camera image.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImageDataSize (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImageFormat

 @abstract Get the format of a camera image.

 @discussion Get the format of a camera image.

 @param cameraImage
 Camera image.
*/
NML_BRIDGE NML_EXPORT NDImageFormat NML_API NDCameraImageFormat (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImageWidth

 @abstract Get the width of a camera image.

 @discussion Get the width of a camera image.

 @param cameraImage
 Camera image.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImageWidth (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImageHeight

 @abstract Get the height of a camera image.

 @discussion Get the height of a camera image.

 @param cameraImage
 Camera image.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImageHeight (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImageRowStride

 @abstract Get the row stride of a camera image in bytes.

 @discussion Get the row stride of a camera image in bytes.

 @param cameraImage
 Camera image.

 @returns Row stride in bytes.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImageRowStride (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImageTimestamp

 @abstract Get the timestamp of a camera image.

 @discussion Get the timestamp of a camera image.

 @param cameraImage
 Camera image.

 @returns Image timestamp in nanoseconds.
*/
NML_BRIDGE NML_EXPORT int64_t NML_API NDCameraImageTimestamp (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImageVerticallyMirrored

 @abstract Whether the camera image is vertically mirrored.

 @discussion Whether the camera image is vertically mirrored.

 @param cameraImage
 Camera image.
*/
NML_BRIDGE NML_EXPORT bool NML_API NDCameraImageVerticallyMirrored (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImagePlaneCount

 @abstract Get the plane count of a camera image.

 @discussion Get the plane count of a camera image.
 If the image uses an interleaved format or only has a single plane, this function returns zero.

 @param cameraImage
 Camera image.

 @returns Number of planes in image.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImagePlaneCount (NDSampleBuffer* cameraImage);

/*!
 @function NDCameraImagePlaneData

 @abstract Get the plane data for a given plane of a camera image.

 @discussion Get the plane data for a given plane of a camera image.

 @param cameraImage
 Camera image.
 
 @param planeIdx
 Plane index.
*/
NML_BRIDGE NML_EXPORT void* NML_API NDCameraImagePlaneData (
    NDSampleBuffer* cameraImage,
    int32_t planeIdx
);

/*!
 @function NDCameraImagePlaneDataSize

 @abstract Get the plane data size of a given plane of a camera image.

 @discussion Get the plane data size of a given plane of a camera image.

 @param cameraImage
 Camera image.
 
 @param planeIdx
 Plane index.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImagePlaneDataSize (
    NDSampleBuffer* cameraImage,
    int32_t planeIdx
);

/*!
 @function NDCameraImagePlaneWidth

 @abstract Get the width of a given plane of a camera image.

 @discussion Get the width of a given plane of a camera image.

 @param cameraImage
 Camera image.
 
 @param planeIdx
 Plane index.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImagePlaneWidth (
    NDSampleBuffer* cameraImage,
    int32_t planeIdx
);

/*!
 @function NDCameraImagePlaneHeight

 @abstract Get the height of a given plane of a camera image.

 @discussion Get the height of a given plane of a camera image.

 @param cameraImage
 Camera image.
 
 @param planeIdx
 Plane index.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImagePlaneHeight (
    NDSampleBuffer* cameraImage,
    int32_t planeIdx
);

/*!
 @function NDCameraImagePlanePixelStride

 @abstract Get the plane pixel stride for a given plane of a camera image.

 @discussion Get the plane pixel stride for a given plane of a camera image.

 @param cameraImage
 Camera image.
 
 @param planeIdx
 Plane index.

 @returns Plane pixel stride in bytes.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImagePlanePixelStride (
    NDSampleBuffer* cameraImage,
    int32_t planeIdx
);

/*!
 @function NDCameraImagePlaneRowStride

 @abstract Get the plane row stride for a given plane of a camera image.

 @discussion Get the plane row stride for a given plane of a camera image.

 @param cameraImage
 Camera image.
 
 @param planeIdx
 Plane index.

 @returns Plane row stride in bytes.
*/
NML_BRIDGE NML_EXPORT int32_t NML_API NDCameraImagePlaneRowStride (
    NDSampleBuffer* cameraImage,
    int32_t planeIdx
);

/*!
 @function NDCameraImageMetadata

 @abstract Get the metadata value for a given key in a camera image.

 @discussion Get the metadata value for a given key in a camera image.

 @param cameraImage
 Camera image.
 
 @param key
 Metadata key.

 @param value
 Destination value array.

 @param count
 Destination value array size.

 @returns Whether the metadata key was successfully looked up.
*/
NML_BRIDGE NML_EXPORT bool NML_API NDCameraImageMetadata (
    NDSampleBuffer* cameraImage,
    NDMetadataKey key,
    float* value,
    int32_t count
);
#pragma endregion