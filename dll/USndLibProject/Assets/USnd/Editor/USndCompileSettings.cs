using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using USnd;

public class USndCompileSettings : ScriptableWizard
{

    // デザイナ向けエディット機能をONにするか
    public bool USND_EDIT_MODE;

    // Androidのマナーモード対応をONにするか
    public bool USND_ANDROID_MANNER_MODE;

    // AndroidのオーディオフォーカスをONにするか
    public bool USND_ANDROID_AUDIO_FOCUS;

    // USndのデバッグログを表示するか
    public bool USND_DEBUG_LOG;

    [MenuItem("USnd/コンパイル設定")]
    public static USndCompileSettings Create()
    {
        USndCompileSettings window = (USndCompileSettings)ScriptableWizard.DisplayWizard("USnd Define", typeof(USndCompileSettings), "Save");
        window.LoadSettings();
        return window;
    }

    public void LoadSettings()
    {
        BuildTargetGroup CurrentPlatform = EditorUserBuildSettings.selectedBuildTargetGroup;
        string def = PlayerSettings.GetScriptingDefineSymbolsForGroup(CurrentPlatform);
        string[] defArray = def.Split(';');

        for(int i=0; i<defArray.Length; ++i)
        {
            switch(defArray[i])
            {
                case "USND_EDIT_MODE":
                    USND_EDIT_MODE = true;
                    break;
                case "USND_ANDROID_MANNER_MODE":
                    USND_ANDROID_MANNER_MODE = true;
                    break;
                case "USND_ANDROID_AUDIO_FOCUS":
                    USND_ANDROID_AUDIO_FOCUS = true;
                    break;
                case "USND_DEBUG_LOG":
                    USND_DEBUG_LOG = true;
                    break;
            }
        }
    }

    public void SaveSettings()
    {
        BuildTargetGroup CurrentPlatform = EditorUserBuildSettings.selectedBuildTargetGroup;
        string def = PlayerSettings.GetScriptingDefineSymbolsForGroup(CurrentPlatform);
        string[] defArray = def.Split(';');

        string write = "";

        for (int i = 0; i < defArray.Length; ++i)
        {
            switch (defArray[i])
            {
                case "USND_EDIT_MODE":
                    break;
                case "USND_ANDROID_MANNER_MODE":
                    break;
                case "USND_ANDROID_AUDIO_FOCUS":
                    break;
                case "USND_DEBUG_LOG":
                    break;
                default:
                    write += defArray[i] + ";";
                    break;
            }
        }

        if (USND_EDIT_MODE)
        {
            write += "USND_EDIT_MODE;";
        }
        if (USND_ANDROID_MANNER_MODE)
        {
            write += "USND_ANDROID_MANNER_MODE;";
        }
        if (USND_ANDROID_AUDIO_FOCUS)
        {
            write += "USND_ANDROID_AUDIO_FOCUS;";
        }
        if (USND_DEBUG_LOG)
        {
            write += "USND_DEBUG_LOG;";
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentPlatform, write);
    }


    public void OnWizardCreate()
    {
        SaveSettings();
    }
	
}
