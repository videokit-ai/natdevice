//
//  NDAuth.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 6/09/2022.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

#include "NDMediaTypes.h"

#pragma region --Enumerations--
/*!
 @enum NDAppTokenStatus

 @abstract NatDevice application token status.

 @constant ND_APP_TOKEN_INVALID
 NatDevice app token is invalid.

 @constant ND_APP_TOKEN_VALID
 NatDevice app token is valid

 @constant ND_APP_TOKEN_HUB_MISSING
 The NatMLHub dependency library is missing.

 @constant ND_APP_TOKEN_HUB_INVALID
 The NatMLHub dependency library is invalid.
 */
enum NDAppTokenStatus {
    ND_APP_TOKEN_INVALID        = 0,
    ND_APP_TOKEN_VALID          = 1,
    ND_APP_TOKEN_HUB_MISSING    = 2,
    ND_APP_TOKEN_HUB_INVALID    = 3,
};
typedef enum NDAppTokenStatus NDAppTokenStatus;
#pragma endregion


#pragma region --Client API--
/*!
 @function NDSetAppToken
 
 @abstract Set the NatDevice app token.
 
 @discussion Set the NatDevice app token.
 
 @param token
 NatDevice app token.

 @returns App token status.
*/
BRIDGE EXPORT NDAppTokenStatus APIENTRY NDSetAppToken (const char* token);
#pragma endregion
