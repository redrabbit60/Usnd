//
//  USndExtension.h
//  USndPlugin
//
//  Created by suwa yukiko on 2015/03/31.
//  Copyright (c) 2015年 suwa yukiko. All rights reserved.
//

#ifndef __USndPlugin__USndExtension__
#define __USndPlugin__USndExtension__

#include <stdio.h>

#ifdef __cplusplus
extern "C" {
#endif
    
    void USndPlugin_Init();
    
    void USndPlugin_Delete();
    
    bool USndPlugin_IsMusicPlaying();
    
    bool USndPlugin_IsOtherAudioPlaying();
    
    bool USndPlugin_IsSpeaker();
    
    
    
#ifdef __cplusplus
}
#endif



#endif /* defined(__USndPlugin__USndExtension__) */
