//
//  NDMediaTypes.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 1/14/2021.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

#pragma once

#include <stdint.h>

// Platform defines
#ifdef __cplusplus
    #define BRIDGE extern "C"
#else
    #define BRIDGE extern
#endif

#ifdef _WIN64
    #define EXPORT __declspec(dllexport)
#else
    #define EXPORT
    #define APIENTRY
#endif