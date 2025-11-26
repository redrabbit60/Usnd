//
//  USndExtension.cpp
//  USndPlugin
//
//  Created by suwa yukiko on 2015/03/31.
//  Copyright (c) 2015年 suwa yukiko. All rights reserved.
//

#include "USndExtension.h"
#import <UIKit/UIKit.h>
#import <AVFoundation/AVFoundation.h>
#import <MediaPlayer/MediaPlayer.h>
#import <AVFoundation/AVAudioSession.h>


bool isSpeaker = false;

@interface AudioRouteChange:NSObject

- (void)setRouteChange;
-(void)removeRouteChange;

@end

@implementation AudioRouteChange

-(void)removeRouteChange
{
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

-(void)setRouteChange
{
    [[AVAudioSession sharedInstance] setActive:YES error:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(routeChangeNotification:) name:AVAudioSessionRouteChangeNotification object:nil];
}

-(void)routeChangeNotification:(NSNotification*)notification
{
    // ヘッドホンが刺さっていたか
    BOOL (^isHeadphone)(NSArray*) = ^(NSArray *output){
        for(AVAudioSessionPortDescription *desc in output)
        {
            if ([desc.portType isEqual:AVAudioSessionPortHeadphones])
            {
                return YES;
            }
        }
        return NO;
    };
    
    // 直前の状態
    AVAudioSessionRouteDescription* prevDesc = notification.userInfo[AVAudioSessionRouteChangePreviousRouteKey];
    if (isHeadphone([[[AVAudioSession sharedInstance] currentRoute] outputs]))
    {
        if (!isHeadphone(prevDesc.outputs))
        {
            // ヘッドフォンがささった
            isSpeaker = false;
        }
    }
    else
    {
        if (isHeadphone(prevDesc.outputs))
        {
            // ヘッドフォンが抜かれた
            isSpeaker = true;
        }
    }
}

@end


float version;
AudioRouteChange* routeChange = NULL;

void USndPlugin_Init()
{
    @autoreleasepool {
        version = [[[UIDevice currentDevice] systemVersion] floatValue];
        routeChange = [[AudioRouteChange alloc] init];
        if ( version >= 6.0 )
        {
            [routeChange setRouteChange];
        }
    }
    
}

void USndPlugin_Delete()
{
    [routeChange removeRouteChange];
    routeChange = NULL;
}

bool USndPlugin_IsMusicPlaying()
{
    if ( version >= 8.0 )
    {
        if (([MPMusicPlayerController systemMusicPlayer].playbackState == MPMusicPlaybackStateStopped) ||
            ([MPMusicPlayerController systemMusicPlayer].playbackState == MPMusicPlaybackStatePaused)) {
            return NO;
        }
    }
    else
    {
        // version 8以前
        if (([MPMusicPlayerController iPodMusicPlayer].playbackState == MPMusicPlaybackStateStopped) ||
            ([MPMusicPlayerController iPodMusicPlayer].playbackState == MPMusicPlaybackStatePaused)) {
            return NO;
        }
    }
    // その他のステータスのときはIsOtherAudioPlayingをチェックして返す
    return USndPlugin_IsOtherAudioPlaying();
}

bool USndPlugin_IsOtherAudioPlaying()
{
    if ( version >= 6.0 )
    {
        // 6以上ならAVAudioSessionで調べる
        Boolean isPlaying = [[AVAudioSession sharedInstance] isOtherAudioPlaying];
        return (isPlaying) ? true : false;//isPlaying;
    }
    else
    {
        // 6未満ならAudioSessionGetPropertyで調べる
        // Playingの誤判定があるので、AudioSessionでも判定する
        UInt32 otherAudioIsPlaying;
        UInt32 propertySize = sizeof(otherAudioIsPlaying);
        
        AudioSessionGetProperty(kAudioSessionProperty_OtherAudioIsPlaying,
                                &propertySize, &otherAudioIsPlaying);
        return (otherAudioIsPlaying) ? true : false;
    }
    return false;
}

bool USndPlugin_IsSpeaker()
{
    return isSpeaker;
}
