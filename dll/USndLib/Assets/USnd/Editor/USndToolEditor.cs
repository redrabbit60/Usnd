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
	int sort_tab = 0;
    string logs = "";

	List<int> holdLabelList = new List<int>();
	

	void Update()
    {
		if ( EditorApplication.isPlaying == false && EditorApplication.isPlayingOrWillChangePlaymode == true )
		{
			holdLabelList.Clear();
		}
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

		GUIStyle header = new GUIStyle();
		header.border = EditorStyles.label.border;
		header.contentOffset = EditorStyles.label.contentOffset;
		header.normal.background = EditorStyles.label.normal.background;
		header.padding = EditorStyles.label.padding;
		header.fontStyle = FontStyle.Bold;
		header.normal.textColor = Color.yellow;


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
                            EditorGUILayout.LabelField("ID", header, GUILayout.Width(100));
                            EditorGUILayout.LabelField("Name", header, GUILayout.Width(250));
                            EditorGUILayout.LabelField("Volume", header, GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();
						GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

						for (int i = 0; i < masterList.Length; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(i + "", GUILayout.Width(100));
                                EditorGUILayout.LabelField(masterList[i], GUILayout.Width(250));
                                EditorGUILayout.LabelField("" + AudioManager.GetMasterVolume(masterList[i]), GUILayout.Width(100));
                            }
                            EditorGUILayout.EndHorizontal();
							GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
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
                            EditorGUILayout.LabelField("ID", header, GUILayout.Width(100));
                            EditorGUILayout.LabelField("Name", header, GUILayout.Width(250));
                            EditorGUILayout.LabelField("Volume", header, GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();
						GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

						for (int i = 0; i < categoryList.Length; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(i + "", GUILayout.Width(100));
                                EditorGUILayout.LabelField(categoryList[i], GUILayout.Width(250));
                                EditorGUILayout.LabelField("" + AudioManager.GetCategoryVolume(categoryList[i]), GUILayout.Width(100));
                            }
                            EditorGUILayout.EndHorizontal();
							GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
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
                            EditorGUILayout.LabelField("ID", header, GUILayout.Width(100), GUILayout.ExpandWidth(false));
							EditorGUILayout.LabelField("Name", header, GUILayout.Width(200), GUILayout.ExpandWidth(true));
                            EditorGUILayout.LabelField("Volume", header, GUILayout.Width(300), GUILayout.ExpandWidth(false));
						}
						EditorGUILayout.EndHorizontal();
						GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

						for (int i = 0; i < labelList.Length; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(i + "", GUILayout.Width(100), GUILayout.ExpandWidth(false));
								EditorGUILayout.LabelField(labelList[i], GUILayout.Width(200), GUILayout.ExpandWidth(true));
                                EditorGUILayout.LabelField("" + AudioManager.GetLabelVolume(labelList[i]), GUILayout.Width(100), GUILayout.ExpandWidth(false));
                                if (GUILayout.Button("Play", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
                                {
                                    AudioManager.Play(labelList[i]);
                                }
                                if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.ExpandWidth(false)))
                                {
                                    AudioManager.StopLabel(labelList[i]);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
							GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
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
				holdLabelList.Clear();
            }

			EditorGUILayout.BeginHorizontal();
            {
				if (GUILayout.Button(((sort_tab == 0) ? "▼" : "▲"), GUILayout.Width(30)))
				{
					sort_tab ^= 1;
				}

				EditorGUILayout.LabelField("Instance ID", header, GUILayout.Width(148), GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField("Label Name or ID", header, GUILayout.Width(170), GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField("Status", header, GUILayout.Width(90), GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField("Volume", header, GUILayout.Width(70), GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField("Detail", header, GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();
            scrollCtrlPos = EditorGUILayout.BeginScrollView(scrollCtrlPos);

            List<AudioManager.SoundLabelInfo> labelInfoList = AudioManager.GetLabelInfoList();

			// 固定ラベルリスト
			if (holdLabelList.Count > 0)
			{
				foreach (AudioManager.SoundLabelInfo info in labelInfoList)
				{
					int index = holdLabelList.IndexOf(info.instance);
					if (index >= 0)
					{
						AudioDefine.INSTANCE_STATUS status = AudioManager.GetInstanceStatus(info.instance);
						// ステータスによらず表示
						AddLabelInfo(info, status);
					}
				}
			}

			if (sort_tab == 0)
			{
				// 全ラベルリスト
				foreach (AudioManager.SoundLabelInfo info in labelInfoList)
				{
					int index = holdLabelList.IndexOf(info.instance);
					if (index < 0)
					{
						AudioDefine.INSTANCE_STATUS status = AudioManager.GetInstanceStatus(info.instance);
						if (narrow_tab == 0 ||
							(narrow_tab == 1 && status != AudioDefine.INSTANCE_STATUS.STOP) ||
							(narrow_tab == 2 && status == AudioDefine.INSTANCE_STATUS.STOP))
						{
							AddLabelInfo(info, status);
						}
					}
				}
			}
			else
			{
				// 古い順に表示したいので後ろから表示
				for(int i=labelInfoList.Count - 1; i>=0; --i)
				{
					int index = holdLabelList.IndexOf(labelInfoList[i].instance);
					if (index < 0)
					{
						AudioDefine.INSTANCE_STATUS status = AudioManager.GetInstanceStatus(labelInfoList[i].instance);
						if (narrow_tab == 0 ||
							(narrow_tab == 1 && status != AudioDefine.INSTANCE_STATUS.STOP) ||
							(narrow_tab == 2 && status == AudioDefine.INSTANCE_STATUS.STOP))
						{
							AddLabelInfo(labelInfoList[i], status);
						}
					}
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

	void AddLabelInfo(AudioManager.SoundLabelInfo info, AudioDefine.INSTANCE_STATUS status)
	{
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

		EditorGUILayout.BeginHorizontal(GUI.skin.box);
		{
			// 固定チェックボックス
			EditorGUILayout.BeginHorizontal(GUILayout.Width(20), GUILayout.ExpandWidth(false));
			{
				int index = holdLabelList.IndexOf(info.instance);

				bool check = (index >= 0);
				bool newcheck = GUILayout.Toggle(check, "");
				
				if (newcheck == true && check == false)
				{
					// 固定するのでholdListに入れる
					holdLabelList.Add(info.instance);
				}
				else if ( newcheck == false && check == true)
				{
					// 固定を外すのでholdListから削除
					holdLabelList.RemoveAt(index);
				}
			}
			EditorGUILayout.EndHorizontal();

			// インスタンスID
			EditorGUILayout.LabelField(info.instance.ToString(), GUILayout.Width(80), GUILayout.ExpandWidth(false));
			// ボタン
			EditorGUILayout.BeginHorizontal(GUILayout.Width(60), GUILayout.ExpandWidth(false));
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
			// ラベル名
			EditorGUILayout.LabelField(info.labelName, GUILayout.Width(180), GUILayout.ExpandWidth(true));
			// ステータス
			EditorGUILayout.LabelField("" + status.ToString(), GUILayout.Width(100), GUILayout.ExpandWidth(false));
			// ボリューム
			EditorGUILayout.LabelField("" + AudioManager.GetInstanceCalcVolume(info.instance), GUILayout.Width(50), GUILayout.ExpandWidth(false));
			// 詳しい状況
			EditorGUILayout.LabelField(getInstanceStatus(status), GUILayout.ExpandWidth(false));
		}
		EditorGUILayout.EndHorizontal();
		GUI.color = Color.white;
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
        // 15000文字以上表示しようとするとエラーが出るので越えてたらカット
        if (logs.Length >= 15000)
        {
            try
            {
                EditorGUILayout.TextArea(logs.Substring(0, logs.IndexOf("\n", 15000)), style);
            } catch (System.Exception e) {
                Debug.Log(e.ToString());
            }
        }
        else
        {
            EditorGUILayout.TextArea(logs, style);
        }
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
