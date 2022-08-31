//
//  NDMediaTypes.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 1/14/2021.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

#pragma once

#pragma region --Platform Defines--
#ifdef __cplusplus
    #define BRIDGE extern "C"
#else
    #define BRIDGE extern
#endif

#ifdef _WIN64
    #define EXPORT __declspec(dllexport)
#else
    #define EXPORT
#endif

#ifdef __EMSCRIPTEN__
    #define APIENTRY EMSCRIPTEN_KEEPALIVE
#elif !defined(_WIN64)
    #define APIENTRY
#endif
#pragma endregion