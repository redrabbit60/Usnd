package com.konami.usndplugin;

import com.unity3d.player.UnityPlayer;

import android.annotation.TargetApi;
import android.media.AudioAttributes;
import android.media.AudioManager;
import android.media.SoundPool;
import android.media.SoundPool.OnLoadCompleteListener;
import android.os.Build;

public class USndSoundPoolPlayer {
    @SuppressWarnings("deprecation")
    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    private SoundPool buildSoundPool(int poolMax)
    {
        SoundPool pool = null;

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.LOLLIPOP) {
            pool = new SoundPool(poolMax, AudioManager.STREAM_MUSIC, 0);
        }
        else {
            AudioAttributes attr = new AudioAttributes.Builder()
                    .setUsage(AudioAttributes.USAGE_MEDIA)
                    .setContentType(AudioAttributes.CONTENT_TYPE_MUSIC)
                    .build();

            pool = new SoundPool.Builder()
                    .setAudioAttributes(attr)
                    .setMaxStreams(poolMax)
                    .build();
        }

        return pool;
    }

    SoundPool pool = null;

    public void Init(int poolMaxNum)
    {
        if ( pool != null )
        {
            pool.release();
        }
        pool = buildSoundPool(poolMaxNum);
    }

    public int LoadSound(final String gameObjName, final String funcName, final String soundName)
    {
        int soundId = pool.load(soundName, 1);

        pool.setOnLoadCompleteListener(new OnLoadCompleteListener() {
            @Override
            public void onLoadComplete(SoundPool soundPool, int sampleId, int status) {
                if (0 == status) {
                    UnityPlayer.UnitySendMessage(gameObjName, funcName, "load complete: " + soundName);
                }
            }
        });

        return soundId;
    }

    // int non-zero streamID if successful, zero if failed
    public int Play(int soundId, float volume, float rate)
    {
        return pool.play(soundId, volume, volume, 1, 0, rate);
    }

    public void Stop(int streamId)
    {
        pool.stop(streamId);
    }

    public void Unload(int soundId)
    {
        pool.unload(soundId);
    }

    public void Release()
    {
        pool.release();
        pool = null;
    }
}
