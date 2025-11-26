using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class USndOutputCallLog : EditorWindow
{


    [MenuItem("USnd/コール履歴を出力")]
    public static void Create()
    {
        USndOutputCallLog window = (USndOutputCallLog)ScriptableObject.CreateInstance<USndOutputCallLog>();
        window.Show();
    }

    void OnGUI()
    {
#if !USND_OUTPUT_CALL_LOG
        GUILayout.Label("コンパイル設定でUSND_OUTPUT_CALL_LOGを有効にしてください。");
#endif 

        if (GUILayout.Button("履歴を出力", GUILayout.Height( 60 )))
        {
#if UNITY_EDITOR
            HashSet<string> callLog = USnd.AudioManager.GetCallLog();
            string savePath = EditorUtility.SaveFolderPanel("保存先を選択", "", "");
            StreamWriter sw = new StreamWriter(savePath + "\\" + "CallLog.txt", false);

            ArrayList tmp = new ArrayList();

            foreach(var value in callLog)
            {
                tmp.Add(value);
            }
            tmp.Sort();

            foreach (var value in tmp)
            {
                sw.WriteLine(value);
            }

            sw.Flush();
            sw.Close();
#endif
        }
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("履歴をクリア"))
        {
            USnd.AudioManager.ClearCallLog();
        }
    }

    public static void AddLog(string callName)
    {
        
    }


}
