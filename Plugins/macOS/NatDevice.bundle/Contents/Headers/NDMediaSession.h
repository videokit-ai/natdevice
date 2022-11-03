//
//  NDMediaSession.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 10/29/2022.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

#pragma once

#include "NDMediaTypes.h"

#pragma region --Enumerations--
/*!
 @enum NDMediaStatus
 
 @abstract NatDevice status codes.
 
 @constant ND_OK
 Successful operation.

 @constant ND_ERROR_INVALID_ARGUMENT
 Provided argument is invalid.

 @constant ND_ERROR_INVALID_OPERATION
 Operation is invalid in current state.

 @constant ND_ERROR_NOT_IMPLEMENTED
 Operation has not been implemented.

 @constant ND_ERROR_INVALID_SESSION
 NatDevice session token has not been set or is invalid.

 @constant ND_ERROR_MISSING_NATML_HUB
 NatMLHub dependency library is missing.

 @constant ND_ERROR_INVALID_NATML_HUB
 NatMLHub dependency library is invalid.

 @constant ND_ERROR_INVALID_PLAN
 Current NatML plan does not allow the operation.

 @constant ND_WARNING_LIMITED_PLAN
 Current NatML plan only allows for limited functionality.
*/
enum NDMediaStatus {
    ND_OK                       = 0,
    ND_ERROR_INVALID_ARGUMENT   = 1,
    ND_ERROR_INVALID_OPERATION  = 2,
    ND_ERROR_NOT_IMPLEMENTED    = 3,
    ND_ERROR_INVALID_SESSION    = 101,
    ND_ERROR_MISSING_NATML_HUB  = 102,
    ND_ERROR_INVALID_NATML_HUB  = 103,
    ND_ERROR_INVALID_PLAN       = 104,
    ND_WARNING_LIMITED_PLAN     = 105,
};
typedef enum NDMediaStatus NDMediaStatus;
#pragma endregion


#pragma region --Client API--
/*!
 @function NDSetSessionToken
 
 @abstract Set the NatDevice session token.
 
 @discussion Set the NatDevice session token.
 
 @param token
 NatDevice session token.

 @returns Session status.
*/
NML_BRIDGE NML_EXPORT NDMediaStatus NML_API NDSetSessionToken (const char* token);
#pragma endregion