using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using USnd;

public class USndTableLog : EditorWindow
{
    string logs = "";

    [MenuItem("USnd/テーブルログ")]
    public static void Create()
    {
        USndTableLog window = (USndTableLog)ScriptableObject.CreateInstance<USndTableLog>();
        window.Show();
    }
	
	// Update is called once per frame
	void Update () {
        if (EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
            Repaint();
        }
	}

    Vector2 pos;

    void OnGUI()
    {
        if (AudioManager.IsInitialized())
        {
            logs = string.Join("\n", AudioManager.GetTableLog().ToArray());
        }
        else
        {
            return;
        }
        
        GUIUtility.keyboardControl = 0;
        GUIStyle style = GUI.skin.GetStyle("TextArea");

        style.richText = true;

        pos = EditorGUILayout.BeginScrollView(pos);
       
        EditorGUILayout.TextArea(logs, style);

        EditorGUILayout.EndScrollView();
    }
}
