using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using USnd;



public class USndParamEditor : EditorWindow
{

    [MenuItem("USnd/パラメータエディタ")]
    public static void Create()
    {
        USndParamEditor window = (USndParamEditor)ScriptableObject.CreateInstance<USndParamEditor>();
        window.Show();

    }

    void Update()
    {
        if (EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
            Repaint();
        }
    }

    void InitParams()
    {
        labelSearchWord = "";
        labelSelectIndex = -1;
        categorySelectIndex = -1;
        masterSelectIndex = -1;
        selectType = (int)SELECT_TYPE.LABEL;
    }

    bool viewModeSeparate = true;
    bool labelFold = false;

    enum SELECT_TYPE
    {
        MASTER = 0,
        CATEGORY = 1,
        LABEL = 2,
    }

    int selectType = (int)SELECT_TYPE.LABEL;

    void OnGUI()
    {
        if (GUILayout.Button("表示方法を変更する"))
        {
            viewModeSeparate = !viewModeSeparate;
        }

        if (viewModeSeparate)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(240));
            ViewToolBar();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            ViewInfo();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
        else
        {

            labelFold = EditorGUILayout.Foldout(labelFold, "パラメータ変更項目選択");
            if (labelFold)
            {
                ViewToolBar();
            }
            ViewInfo();
        }

    }

    string[] labelName;
    string[] categoryName;
    string[] masterName;

    Vector2 selectLabelPos;
    Vector2 selectCategoryPos;
    Vector2 selectMasterPos;
    int labelSelectIndex = -1;
    int categorySelectIndex = -1;
    int masterSelectIndex = -1;

    string labelSearchWord;

    void ViewToolBar()
    {
        if (!AudioManager.IsInitialized())
        {
            return;
        }

        selectType = GUILayout.Toolbar(selectType, new string[] { "Master", "Category", "Label" });
        
        switch(selectType)
        {
            case (int)SELECT_TYPE.MASTER:
                ViewSelectMaster();
                break;
            case (int)SELECT_TYPE.CATEGORY:
                ViewSelectCategory();
                break;
            case (int)SELECT_TYPE.LABEL:
                ViewSelectLabel();
                break;
        }

    }

    void ViewSelectMaster()
    {
        if (!AudioManager.IsInitialized())
        {
            return;
        }
        GUILayout.Label("Loading Master List");

        masterName = AudioManager.GetMasterNameList();

        selectMasterPos = EditorGUILayout.BeginScrollView(selectMasterPos);

        SetSelectButton(masterName, ref masterSelectIndex);

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("選択中の項目をクリップボードにコピー"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            if (0 <= masterSelectIndex && masterSelectIndex < AudioManager.GetMasterNum())
            {
                AudioManager.AudioMasterSettings master = AudioManager.GetMasterInfo(masterName[masterSelectIndex]);
                string info = "";
                CopyMasterInfo(ref info, master);
                GUIUtility.systemCopyBuffer = info;
            }
#endif
        }

        if ( GUILayout.Button("すべての項目をクリップボードにコピー"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            string info = "";
            for (int i = 0; i < AudioManager.GetMasterNum(); ++i)
            {
                AudioManager.AudioMasterSettings master = AudioManager.GetMasterInfo(masterName[i]);
                CopyMasterInfo(ref info, master);
                info += "\n";
            }
            GUIUtility.systemCopyBuffer = info;
#endif
        }
    }

    void CopyMasterInfo(ref string info, AudioManager.AudioMasterSettings master)
    {
        info += master.masterName + "\t" + master.volume;
    }


    void ViewSelectCategory()
    {
        if (!AudioManager.IsInitialized())
        {
            return;
        }
        GUILayout.Label("Loading Category List");

        categoryName = AudioManager.GetCategoryNameList();

        selectCategoryPos = EditorGUILayout.BeginScrollView(selectCategoryPos);

        SetSelectButton(categoryName, ref categorySelectIndex);

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("選択中の項目をクリップボードにコピー"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            if (0 <= categorySelectIndex && categorySelectIndex < AudioManager.GetCategoryNum())
            {
                AudioManager.AudioCategorySettings category = AudioManager.GetCategoryInfo(categoryName[categorySelectIndex]);
                string info = "";
                CopyCategoryInfo(ref info, category);
                GUIUtility.systemCopyBuffer = info;
            }
#endif
        }

        if (GUILayout.Button("すべての項目をクリップボードにコピー"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            string info = "";
            for (int i = 0; i < AudioManager.GetCategoryNum(); ++i)
            {
                AudioManager.AudioCategorySettings category = AudioManager.GetCategoryInfo(categoryName[i]);
                CopyCategoryInfo(ref info, category);
                info += "\n";
            }
            GUIUtility.systemCopyBuffer = info;
#endif
        }
    }

    void CopyCategoryInfo(ref string info, AudioManager.AudioCategorySettings category)
    {
        info += category.categoryName + "\t" + category.volume + "\t" + category.maxPlaybacksNum + "\t" + category.masterName;
    }

    void ViewSelectLabel()
    {
        if ( !AudioManager.IsInitialized() )
        {
            return;
        }
        GUILayout.Label("Loading Label List");

        labelName = AudioManager.GetLabelNameList();

        selectLabelPos = EditorGUILayout.BeginScrollView(selectLabelPos);

        EditorGUILayout.LabelField("検索:");
        labelSearchWord = EditorGUILayout.TextField(labelSearchWord);
        
        SetSelectButton(labelName, ref labelSelectIndex, labelSearchWord);

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("選択中の項目をクリップボードにコピー"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            if (0 <= labelSelectIndex && labelSelectIndex < AudioManager.GetLabelNum())
            {
                AudioManager.AudioLabelSettings label = AudioManager.GetLabelInfo(labelName[labelSelectIndex]);
                string info = "";
                CopyLabelInfo(ref info, label);
                GUIUtility.systemCopyBuffer = info;
            }
#endif
        }

        if (GUILayout.Button("すべての項目をクリップボードにコピー"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            string info = "";
            for (int i = 0; i < AudioManager.GetLabelNum(); ++i)
            {
                AudioManager.AudioLabelSettings label = AudioManager.GetLabelInfo(labelName[i]);
                CopyLabelInfo(ref info, label);
                info += "\n";
            }
            GUIUtility.systemCopyBuffer = info;
#endif
        }
    }

    void CopyLabelInfo(ref string info, AudioManager.AudioLabelSettings label)
    {
        info += label.name + "\t" + label.clipName + "\t" + label.isLoop + "\t" + label.volume + "\t";
        info += label.maxPlaybacksBehavior.ToString() + "\t" + label.priority + "\t" + label.categoryName + "\t" + label.singleGroup + "\t";
        info += label.maxPlaybacksNum + "\t" + label.isStealOldest + "\t" + label.unityMixerName + "\t" + label.spatialGroup + "\t";
        info += label.playStartDelay + "\t" + label.pan + "\t" + label.pitchShiftCent + "\t" + label.isPlayLastSamples + "\t";
        info += label.fadeInTime + "\t" + label.fadeOutTime + "\t" + label.fadeInTimeOldSamples + "\t";
        info += label.fadeOutTimeOnPause + "\t" + label.fadeInTimeOffPause + "\t";
        info += label.isVolumeRandom + "\t" + label.inconsecutiveVolume + "\t" + label.volumeRandomMin + "\t" + label.volumeRandomMax + "\t" + label.volumeRandomUnit + "\t";
        info += label.isPitchRandom + "\t" + label.inconsecutivePitch + "\t" + label.pitchRandomMin + "\t" + label.pitchRandomMax + "\t" + label.pitchRandomUnit + "\t";
        info += label.isPanRandom + "\t" + label.inconsecutivePan + "\t" + label.panRandomMin + "\t" + label.panRandomMax + "\t" + label.panRandomUnit + "\t";
        info += label.isRandomPlay + "\t" + label.inconsecutiveSource + "\t";

        SetListString(ref info, label.randomSource);

        info += label.isMovePitch + "\t" + label.pitchStart + "\t" + label.pitchEnd + "\t" + label.pitchMoveTime + "\t";
        info += label.isMovePan + "\t" + label.panStart + "\t" + label.panEnd + "\t" + label.panMoveTime + "\t";

        SetListString(ref info, label.duckingCategories);

        info += label.duckingStartTime + "\t" + label.duckingEndTime + "\t" + label.duckingVolumeFactor + "\t";
        info += label.autoRestoreDucking + "\t" + label.restoreTime + "\t" + label.isAndroidNative;
    }

    void SetListString(ref string src, List<string> text)
    {
        src += "\"";
        for (int i = 0; i < text.Count; ++i)
        {
            src += text[i];
            if (i + 1 != text.Count)
            {
                src += "\n";
            }
        }
        src += "\"\t";
    }

    void SetSelectButton(string[] nameList, ref int selectIndex, string searchWord = null)
    {
        if (nameList != null)
        {

            for (int i = 0; i < nameList.Length; ++i)
            {
                if ( searchWord != null  )
                {
                    if ( !nameList[i].Contains(searchWord) )
                    {
                        continue;
                    }
                }
                if (selectIndex == i)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button(nameList[i], GUILayout.Width(200)))
                {
                    selectIndex = i;
                }
            }
            GUI.color = Color.white;
        }
    }


    Vector2 labelInfoPos;
    Vector2 categoryInfoPos;
    Vector2 masterInfoPos;


    void ViewInfo()
    {
        switch (selectType)
        {
            case (int)SELECT_TYPE.MASTER:
                ViewMasterInfo();
                break;
            case (int)SELECT_TYPE.CATEGORY:
                ViewCategoryInfo();
                break;
            case (int)SELECT_TYPE.LABEL:
                ViewLabelInfo();
                break;
        }
    }

    void ViewMasterInfo()
    {
        if (masterSelectIndex < 0 || AudioManager.GetMasterNum() <= masterSelectIndex)
        {
            masterSelectIndex = -1;
            return;
        }

        if (!AudioManager.IsInitialized())
        {
            return;
        }

        if (AudioManager.GetMasterNum() == 0  || masterName == null)
        {
            return;
        }

        AudioManager.AudioMasterSettings master = AudioManager.GetMasterInfo(masterName[masterSelectIndex]);

        if (master == null)
        {
            return;
        }

        masterInfoPos = EditorGUILayout.BeginScrollView(masterInfoPos);

        SetHeader("Master: " + masterName[masterSelectIndex]);

        master.volume = EditorGUILayout.Slider("Volume", master.volume, 0, 1);

        EditorGUILayout.LabelField("Program Volume: " + master.programVolume);

        EditorGUILayout.LabelField("Mute Volume: " + master.mute);

        EditorGUILayout.EndScrollView();
    }

    void ViewCategoryInfo()
    {
        if (categorySelectIndex < 0 || AudioManager.GetCategoryNum() <= categorySelectIndex)
        {
            categorySelectIndex = -1;
            return;
        }

        if (!AudioManager.IsInitialized())
        {
            return;
        }

        if (AudioManager.GetCategoryNum() == 0 || categoryName == null)
        {
            return;
        }

        AudioManager.AudioCategorySettings category = AudioManager.GetCategoryInfo(categoryName[categorySelectIndex]);

        if (category == null)
        {
            return;
        }

        categoryInfoPos = EditorGUILayout.BeginScrollView(categoryInfoPos);

        SetHeader("Category: " + categoryName[categorySelectIndex]);

        category.volume = EditorGUILayout.Slider("Volume", category.volume, 0, 1);

        category.maxPlaybacksNum = EditorGUILayout.IntSlider("MaxPlaybacksNum", category.maxPlaybacksNum, 0, 30);

        category.masterName = EditorGUILayout.TextField("MasterName", category.masterName);

        EditorGUILayout.LabelField("Program Volume: " + category.programVolume);

        EditorGUILayout.LabelField("Ducking Volume: " + category.duckingVolume);


        EditorGUILayout.EndScrollView();
    }


    void ViewLabelInfo()
    {
        if (labelSelectIndex < 0 || AudioManager.GetLabelNum() <= labelSelectIndex)
        {
            labelSelectIndex = -1;
            return;
        }

        if ( !AudioManager.IsInitialized() )
        {
            return;
        }

        if (AudioManager.GetLabelNum() == 0 || labelName == null)
        {
            return;
        }

        AudioManager.AudioLabelSettings label = AudioManager.GetLabelInfo(labelName[labelSelectIndex]);

        if ( label == null )
        {
            return;
        }

        labelInfoPos = EditorGUILayout.BeginScrollView(labelInfoPos);

        SetHeader("Label: " + labelName[labelSelectIndex] + "  LoadId:" + label.loadId);

        EditorGUILayout.LabelField("AudioClip: " + label.clipName);

        label.isLoop = EditorGUILayout.Toggle("IsLoop", label.isLoop);

        label.volume = EditorGUILayout.Slider("Volume", label.volume, 0, 1);

        SetHeader("Category Playbacks Behaviour");

        label.categoryName = EditorGUILayout.TextField("CategoryName", label.categoryName);

        label.maxPlaybacksBehavior = (AudioManager.AudioLabelSettings.BEHAVIOR)EditorGUILayout.EnumPopup("MaxPlaybacksBehaviour", label.maxPlaybacksBehavior);
        
        label.priority = EditorGUILayout.IntSlider("Priority", label.priority, 0, 127);

        label.singleGroup = EditorGUILayout.TextField("SingleGroup", label.singleGroup);

        SetHeader("Label Playbacks Behaviour");

        label.maxPlaybacksNum = EditorGUILayout.IntSlider("MaxPlaybacksNum", label.maxPlaybacksNum, 0, 30);
        
        label.isStealOldest = EditorGUILayout.Toggle("IsStealOldest", label.isStealOldest);

        SetHeader("Unity Mixer");

        label.unityMixerName = EditorGUILayout.TextField("UnityMixerName", label.unityMixerName);

        SetHeader("3D Group");

        label.spatialGroup = EditorGUILayout.TextField("SpatialGroup", label.spatialGroup);

        SetHeader("Start Delay");

        label.playStartDelay = EditorGUILayout.Slider("PlayStartDelay", label.playStartDelay, 0, 30);

        SetHeader("Play Interval");

        label.playInterval = EditorGUILayout.Slider("PlayInterval", label.playInterval, 0, 30);

        SetHeader("Pan");

        label.pan = EditorGUILayout.Slider("Pan", label.pan, -1, 1);
        
        SetHeader("Pitch");
        
        label.pitchShiftCent = EditorGUILayout.IntSlider("PitchShift", label.pitchShiftCent, -1200, 1200);

        SetHeader("Start Position");
        
        label.isPlayLastSamples = EditorGUILayout.Toggle("IsPlayLastSamples", label.isPlayLastSamples);

        SetHeader("Fade");

        label.fadeInTime = EditorGUILayout.Slider("FadeInTime", label.fadeInTime, 0, 30);

        label.fadeOutTime = EditorGUILayout.Slider("FadeOutTime", label.fadeOutTime, 0, 30);

        label.fadeInTimeOldSamples = EditorGUILayout.Slider("FadeInTimeOldSamples", label.fadeInTimeOldSamples, 0, 30);

        label.fadeOutTimeOnPause = EditorGUILayout.Slider("FadeOutTimeOnPause", label.fadeOutTimeOnPause, 0, 30);

        label.fadeInTimeOffPause = EditorGUILayout.Slider("FadeInTimeOffPause", label.fadeInTimeOffPause, 0, 30);

        SetHeader("Random Volume");

        label.isVolumeRandom = EditorGUILayout.Toggle("IsVolumeRandom", label.isVolumeRandom);

        label.inconsecutiveVolume = EditorGUILayout.Toggle("InconsecutiveVolume", label.inconsecutiveVolume);

        label.volumeRandomMin = EditorGUILayout.Slider("VolumeRandomMin", label.volumeRandomMin, 0, 1);

        label.volumeRandomMax = EditorGUILayout.Slider("VolumeRandomMax", label.volumeRandomMax, 0, 1);

        label.volumeRandomUnit = EditorGUILayout.Slider("VolumeRandomUnit", label.volumeRandomUnit, 0, 1);

        SetHeader("Random Pitch");

        label.isPitchRandom = EditorGUILayout.Toggle("IsPitchRandom", label.isPitchRandom);

        label.inconsecutivePitch = EditorGUILayout.Toggle("InconsecutivePitch", label.inconsecutivePitch);

        label.pitchRandomMin = EditorGUILayout.IntSlider("PitchRandomMin", label.pitchRandomMin, -1200, 1200);

        label.pitchRandomMax = EditorGUILayout.IntSlider("PitchRandomMax", label.pitchRandomMax, -1200, 1200);

        label.pitchRandomUnit = EditorGUILayout.IntSlider("PitchRandomUnit", label.pitchRandomUnit, -1200, 1200);

        SetHeader("Random Stereo Pan");

        label.isPanRandom = EditorGUILayout.Toggle("IsPanRandom", label.isPanRandom);

        label.inconsecutivePan = EditorGUILayout.Toggle("InconsecutivePan", label.inconsecutivePan);

        label.panRandomMin = EditorGUILayout.Slider("PanRandomMin", label.panRandomMin, -1, 1);

        label.panRandomMax = EditorGUILayout.Slider("PanRandomMax", label.panRandomMax, -1, 1);

        label.panRandomUnit = EditorGUILayout.Slider("PanRandomUnit", label.panRandomUnit, -1, 1);

        SetHeader("Random Source");

        label.isRandomPlay = EditorGUILayout.Toggle("IsRandomPlay", label.isRandomPlay);

        label.inconsecutiveSource = EditorGUILayout.Toggle("InconsecutiveSource", label.inconsecutiveSource);

        EditorGUI.BeginChangeCheck();

        int rndSrcLen = label.randomSource.Count;
        rndSrcLen = EditorGUILayout.IntField("RandomSourceNum", rndSrcLen);

        if (EditorGUI.EndChangeCheck())
        {
            UpdateArrayNum(rndSrcLen, label.randomSource.Count, label.randomSource);
        }

        for (int i = 0; i < label.randomSource.Count; ++i)
        {
            label.randomSource[i] = EditorGUILayout.TextField("RandomSource(" + (i+1) + "):", label.randomSource[i]);
        }

        SetHeader("Start Move Pitch");

        label.isMovePitch = EditorGUILayout.Toggle("IsMovePitch", label.isMovePitch);

        label.pitchStart = EditorGUILayout.IntSlider("PitchStart", label.pitchStart, -1200, 1200);

        label.pitchEnd = EditorGUILayout.IntSlider("PitchEnd", label.pitchEnd, -1200, 1200);

        label.pitchMoveTime = EditorGUILayout.Slider("PitchMoveTime", label.pitchMoveTime, 0, 30);

        SetHeader("Start Move Stereo Pan");

        label.isMovePan = EditorGUILayout.Toggle("IsMovePan", label.isMovePan);

        label.panStart = EditorGUILayout.Slider("PanStart", label.panStart, -1, 1);

        label.panEnd = EditorGUILayout.Slider("PanEnd", label.panEnd, -1, 1);

        label.panMoveTime = EditorGUILayout.Slider("PanMoveTime", label.panMoveTime, 0, 30);

        SetHeader("Category Ducking");

        EditorGUI.BeginChangeCheck();

        rndSrcLen = label.duckingCategories.Count;
        rndSrcLen = EditorGUILayout.IntField("DuckingCategoriesNum", rndSrcLen);

        if (EditorGUI.EndChangeCheck())
        {
            UpdateArrayNum(rndSrcLen, label.duckingCategories.Count, label.duckingCategories);
        }

        for (int i = 0; i < label.duckingCategories.Count; ++i)
        {
            label.duckingCategories[i] = EditorGUILayout.TextField("DuckingCategory(" + (i + 1) + "):", label.duckingCategories[i]);
        }

        label.duckingStartTime = EditorGUILayout.Slider("DuckingStartTime", label.duckingStartTime, 0, 30);

        label.duckingEndTime = EditorGUILayout.Slider("DuckingEndTime", label.duckingEndTime, 0, 30);

        label.duckingVolumeFactor = EditorGUILayout.Slider("DuckingVolumeFactor", label.duckingVolumeFactor, 0, 1);

        label.autoRestoreDucking = EditorGUILayout.Toggle("AutoRestoreDucking", label.autoRestoreDucking);

        label.restoreTime = EditorGUILayout.FloatField("RestoreTime", label.restoreTime);

        label.isAndroidNative = EditorGUILayout.Toggle("IsAndroidNative", label.isAndroidNative);

        EditorGUILayout.EndScrollView();
    }

    void SetHeader(string title)
    {
        GUILayout.Space(10);
        GUILayout.Label(title, EditorStyles.boldLabel);
    }

    void UpdateArrayNum(int newNum, int baseNum, List<string> array)
    {
        while (baseNum != newNum)
        {
            if (baseNum > newNum)
            {
                array.RemoveAt(baseNum - 1);
                --baseNum;
            }
            else if (baseNum < newNum)
            {
                array.Add("");
                ++baseNum;
            }
        }
    }

}
