package com.konami.usndplugin;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.media.AudioManager;
//import android.util.Log;
import android.media.AudioManager.OnAudioFocusChangeListener;
import android.os.Build;
import android.media.AudioDeviceInfo;

import com.unity3d.player.UnityPlayer;

public class USndHeadsetReceiver {

    private static String objName;
    private static String funcName;

    public void SetAudioFocus()
    {
        Activity act = UnityPlayer.currentActivity;
        Context context = act.getApplicationContext();
        AudioManager manager = (AudioManager)context.getSystemService(Context.AUDIO_SERVICE);

        manager.requestAudioFocus(afChangeListener, AudioManager.STREAM_MUSIC, AudioManager.AUDIOFOCUS_GAIN);
    }

    OnAudioFocusChangeListener afChangeListener = new OnAudioFocusChangeListener() {
        public void onAudioFocusChange(int focusChange) {
            if (focusChange == AudioManager.AUDIOFOCUS_LOSS_TRANSIENT) {
                // Pause playback
            } else if (focusChange == AudioManager.AUDIOFOCUS_GAIN) {
                // Resume playback
            } else if (focusChange == AudioManager.AUDIOFOCUS_LOSS) {
                Activity act = UnityPlayer.currentActivity;
                Context context = act.getApplicationContext();
                AudioManager manager = (AudioManager)context.getSystemService(Context.AUDIO_SERVICE);

                manager.abandonAudioFocus(afChangeListener);
                // Stop playback
            }
        }
    };

    public int GetRingerMode()
    {
        Activity act = UnityPlayer.currentActivity;
        Context context = act.getApplicationContext();

        AudioManager manager = (AudioManager)context.getSystemService(Context.AUDIO_SERVICE);

        return manager.getRingerMode();
    }

    public boolean IsSpeakerOut()
    {
        Activity act = UnityPlayer.currentActivity;
        Context context = act.getApplicationContext();

        AudioManager manager = (AudioManager)context.getSystemService(Context.AUDIO_SERVICE);

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {

            boolean status = manager.isWiredHeadsetOn();
            if (status == true) return false;        // wireheadsetがtrueならスピーカーアウトじゃない

            status = manager.isBluetoothA2dpOn();

            return status ? false : true;        // bluetoothがtrueならスピーカーアウトじゃない
        }
        else {
            AudioDeviceInfo[] devices = manager.getDevices(AudioManager.GET_DEVICES_OUTPUTS);

            for (int i = 0; i < devices.length; i++) {
                AudioDeviceInfo device = devices[i];
                if (device.getType() == AudioDeviceInfo.TYPE_WIRED_HEADPHONES
                        || device.getType() == AudioDeviceInfo.TYPE_BLUETOOTH_A2DP
                        || device.getType() == AudioDeviceInfo.TYPE_BLUETOOTH_SCO) {
                    return false;
                }

            }
        }
        return true;
    }

    public void SetHeadsetPlugCallback(final String gameObjName, final String func)
    {
        Activity act = UnityPlayer.currentActivity;
        objName = gameObjName;
        funcName = func;
        try {
            act.registerReceiver(onHeadsetPlug, new IntentFilter(Intent.ACTION_HEADSET_PLUG));
        }catch(Exception e)
        {
            //Log.v("Usnd", e.toString());
        }

        try {
            act.registerReceiver(onBecomingNoisy, new IntentFilter(AudioManager.ACTION_AUDIO_BECOMING_NOISY));
        }catch(Exception e)
        {
            //Log.v("Usnd", e.toString());
        }

        try {
            act.registerReceiver(onRingerModeChange, new IntentFilter(AudioManager.RINGER_MODE_CHANGED_ACTION));
        }catch(Exception e)
        {
            //Log.v("Usnd", e.toString());
        }

    }

    private static BroadcastReceiver onHeadsetPlug = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            // TODO 自動生成されたメソッド・スタブ

            if ( intent.getAction().equalsIgnoreCase(Intent.ACTION_HEADSET_PLUG))
            {
                int plugged = intent.getIntExtra("state", 0);
                try{
                    if ( plugged == 0)
                        UnityPlayer.UnitySendMessage(objName, funcName, "false");
                    else
                        UnityPlayer.UnitySendMessage(objName, funcName, "true");
                } catch(Exception e){
                   // Log.v("USnd", "send message:" + e.toString());
                }
            }

        }
    };

    private static BroadcastReceiver onBecomingNoisy = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            // TODO 自動生成されたメソッド・スタブ

            if ( intent.getAction().equalsIgnoreCase(AudioManager.ACTION_AUDIO_BECOMING_NOISY))
            {
                UnityPlayer.UnitySendMessage(objName, funcName, "noisy");
            }

        }
    };

    private static BroadcastReceiver onRingerModeChange = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            // TODO 自動生成されたメソッド・スタブ

            if ( intent.getAction().equalsIgnoreCase(AudioManager.RINGER_MODE_CHANGED_ACTION))
            {
                int mode = intent.getIntExtra(AudioManager.EXTRA_RINGER_MODE,
                        AudioManager.RINGER_MODE_NORMAL);
                switch(mode){
                    case AudioManager.RINGER_MODE_SILENT:
                    case AudioManager.RINGER_MODE_VIBRATE:
                        UnityPlayer.UnitySendMessage(objName, funcName, "mute_on");
                        break;
                    case AudioManager.RINGER_MODE_NORMAL:
                        UnityPlayer.UnitySendMessage(objName, funcName, "mute_off");
                        break;
                    default:
                        UnityPlayer.UnitySendMessage(objName, funcName, "mute_off");
                        break;
                }
            }

        }
    };
}

