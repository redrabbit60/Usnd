using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using USnd;

public class USndToolEditor : EditorWindow
{
    Vector2 scrollStatusPos;
    Vector2 scrollCtrlPos;

    bool showStatus = false;
    bool showMasterList = false;
    bool showCategoryList = false;
    bool showLabelList = false;

    int narrow_tab;
    int mode_tab;
    string logs = "";

    void Update()
    {
        if (EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
            Repaint();
        }
    }

    [MenuItem("USnd/USndTool")]
    public static void Create()
    {
        USndToolEditor window = (USndToolEditor)ScriptableObject.CreateInstance<USndToolEditor>();
        window.Show();
    }


    void OnGUI()
    {

        if (AudioManager.IsInitialized() == false)
        {
            ViewLog();
            return;
        }
        
        // 実行中ならログを更新
        if (AudioManager.IsInitialized())
        {
            logs = string.Join("\n", AudioManager.GetLog().ToArray());//manager.logs;
        }

        scrollStatusPos = EditorGUILayout.BeginScrollView(scrollStatusPos);
        showStatus = EditorGUILayout.Foldout(showStatus, "Status");
        if (showStatus)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            ++EditorGUI.indentLevel;
            {
                // -----------------------------------------------------------
                // マスターリスト
                string[] masterList = AudioManager.GetMasterNameList();
                EditorGUILayout.LabelField("GetMasterNum", "" + masterList.Length);

                showMasterList = EditorGUILayout.Foldout(showMasterList, "MasterList");
                if (showMasterList)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    ++EditorGUI.indentLevel;
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("ID", GUILayout.Width(100));
                            EditorGUILayout.LabelField("Name", GUILayout.Width(200));
                            EditorGUILayout.LabelField("Volume", GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();
                        for (int i = 0; i < masterList.Length; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(i + "", GUILayout.Width(100));
                                EditorGUILayout.LabelField(masterList[i], GUILayout.Width(200));
                                EditorGUILayout.LabelField("" + AudioManager.GetMasterVolume(masterList[i]), GUILayout.Width(100));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    --EditorGUI.indentLevel;
                    EditorGUILayout.EndVertical();
                }

                // -----------------------------------------------------------
                // カテゴリリスト
                string[] categoryList = AudioManager.GetCategoryNameList();
                EditorGUILayout.LabelField("GetCategoryNum", "" + categoryList.Length);

                showCategoryList = EditorGUILayout.Foldout(showCategoryList, "CategoryList");
                if (showCategoryList)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    ++EditorGUI.indentLevel;
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("ID", GUILayout.Width(100));
                            EditorGUILayout.LabelField("Name", GUILayout.Width(200));
                            EditorGUILayout.LabelField("Volume", GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();
                        for (int i = 0; i < categoryList.Length; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(i + "", GUILayout.Width(100));
                                EditorGUILayout.LabelField(categoryList[i], GUILayout.Width(200));
                                EditorGUILayout.LabelField("" + AudioManager.GetCategoryVolume(categoryList[i]), GUILayout.Width(100));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    --EditorGUI.indentLevel;
                    EditorGUILayout.EndVertical();
                }

                // -----------------------------------------------------------
                // ラベルリスト
                string[] labelList = AudioManager.GetLabelNameList();
                EditorGUILayout.LabelField("GetLabelNum", "" + labelList.Length);

                showLabelList = EditorGUILayout.Foldout(showLabelList, "LabelList");
                if (showLabelList)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    ++EditorGUI.indentLevel;
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("ID", GUILayout.Width(100));
                            EditorGUILayout.LabelField("Name", GUILayout.Width(200));
                            EditorGUILayout.LabelField("Volume", GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();
                        for (int i = 0; i < labelList.Length; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(i + "", GUILayout.Width(100));
                                EditorGUILayout.LabelField(labelList[i], GUILayout.Width(200));
                                EditorGUILayout.LabelField("" + AudioManager.GetLabelVolume(labelList[i]), GUILayout.Width(100));
                                if (GUILayout.Button("Play"))
                                {
                                    AudioManager.Play(labelList[i]);
                                }
                                if (GUILayout.Button("Stop"))
                                {
                                    AudioManager.StopLabel(labelList[i]);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    --EditorGUI.indentLevel;
                    EditorGUILayout.EndVertical();
                }
            }
            --EditorGUI.indentLevel;
            EditorGUILayout.EndVertical();
        }

        mode_tab = GUILayout.Toolbar(mode_tab, new string[] { "Status", "Logs" });

        if (mode_tab == 0)
        {
            narrow_tab = GUILayout.Toolbar(narrow_tab, new string[] { "All", "Play", "Stop" });
            if (GUILayout.Button("Clear", GUILayout.Width(100)))
            {
                AudioManager.SoundToolPlayListClear();
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Instance ID", GUILayout.Width(160));
                EditorGUILayout.LabelField("Label Name or ID", GUILayout.Width(200));
                EditorGUILayout.LabelField("Status", GUILayout.Width(100));
                EditorGUILayout.LabelField("Volume", GUILayout.Width(50));
                EditorGUILayout.LabelField("Detail");
            }
            EditorGUILayout.EndHorizontal();
            scrollCtrlPos = EditorGUILayout.BeginScrollView(scrollCtrlPos);

            List<AudioManager.SoundLabelInfo> labelInfoList = AudioManager.GetLabelInfoList();
            foreach (AudioManager.SoundLabelInfo info in labelInfoList)
            {
                AudioDefine.INSTANCE_STATUS status = AudioManager.GetInstanceStatus(info.instance);
                if (status == AudioDefine.INSTANCE_STATUS.STOP)
                {
                    GUI.color = Color.red;
                }
                else if (status == AudioDefine.INSTANCE_STATUS.PAUSE || status == AudioDefine.INSTANCE_STATUS.PAUSE_SOON)
                {
                    GUI.color = Color.cyan;
                }
                else
                {
                    GUI.color = Color.green;
                }

                if (narrow_tab == 0 ||
                    (narrow_tab == 1 && status != AudioDefine.INSTANCE_STATUS.STOP) ||
                    (narrow_tab == 2 && status == AudioDefine.INSTANCE_STATUS.STOP))
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUILayout.LabelField(info.instance.ToString(), GUILayout.Width(100));
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(60));
                        {
                            if (status == AudioDefine.INSTANCE_STATUS.PLAY)
                            {
                                if (GUILayout.Button("▮▮", GUILayout.Width(30), GUILayout.Height(15)))
                                {
                                    AudioManager.OnPause(info.instance);
                                }
                                if (GUILayout.Button("■", GUILayout.Width(30), GUILayout.Height(15)))
                                {
                                    AudioManager.Stop(info.instance);
                                }
                            }
                            else if (status == AudioDefine.INSTANCE_STATUS.PAUSE || status == AudioDefine.INSTANCE_STATUS.PAUSE_SOON)
                            {
                                if (GUILayout.Button("▶", GUILayout.Width(30), GUILayout.Height(15)))
                                {
                                    AudioManager.OffPause(info.instance);
                                }
                                if (GUILayout.Button("■", GUILayout.Width(30), GUILayout.Height(15)))
                                {
                                    AudioManager.Stop(info.instance);
                                }
                            }
                            else
                            {
                                GUILayout.Label("", GUILayout.Width(60), GUILayout.Height(15));
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField(info.labelName, GUILayout.Width(200));
                        EditorGUILayout.LabelField("" + status.ToString(), GUILayout.Width(100));
                        EditorGUILayout.LabelField("" + AudioManager.GetInstanceCalcVolume(info.instance), GUILayout.Width(50));
                        EditorGUILayout.LabelField(getInstanceStatus(status));
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.color = Color.white;
                }
            }



            EditorGUILayout.EndScrollView();
        }
        else if (mode_tab == 1)
        {
            ViewLog();
        }

        EditorGUILayout.EndScrollView();
    }

    void ViewLog()
    {
        // テキストエリアのフォーカスを外す、これをすると選択ができなくなるが古い内容が残らなくなる
        GUIUtility.keyboardControl = 0;
        GUIStyle style = GUI.skin.GetStyle("TextArea");

        style.richText = true;
        
        scrollCtrlPos = EditorGUILayout.BeginScrollView(scrollCtrlPos);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("LogClear"))
            {
                if (AudioManager.IsInitialized())
                {
                    AudioManager.SoundToolLogsClear();
                }
            }
            if (GUILayout.Button("Copy"))
            {
#if UNITY_5_0 || UNITY_5_1
#else
                GUIUtility.systemCopyBuffer = logs;
#endif
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.TextArea(logs, style);
        EditorGUILayout.EndScrollView();
    }

    string getInstanceStatus(AudioDefine.INSTANCE_STATUS status)
    {
        string message = "";
        switch (status)
        {
            case AudioDefine.INSTANCE_STATUS.PLAY:
                message = "インスタンス再生中";
                break;
            case AudioDefine.INSTANCE_STATUS.PREPARE:
                message = "インスタンス再生準備完了";
                break;
            case AudioDefine.INSTANCE_STATUS.PAUSE:
                message = "インスタンス一時停止中";
                break;
            case AudioDefine.INSTANCE_STATUS.PAUSE_SOON:
                message = "インスタンス一時停止予定";
                break;
            case AudioDefine.INSTANCE_STATUS.STOP:
                message = "インスタンス停止済み";
                break;
            case AudioDefine.INSTANCE_STATUS.STOP_SOON:
                message = "インスタンス停止予定";
                break;
        }

        return message;
    }

}
