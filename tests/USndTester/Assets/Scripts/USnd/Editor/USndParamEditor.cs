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
        D3SETTINGS = 3,
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
    string[] d3Name;

    Vector2 selectLabelPos;
    Vector2 selectCategoryPos;
    Vector2 selectMasterPos;
    Vector2 select3DPos;
    int labelSelectIndex = -1;
    int categorySelectIndex = -1;
    int masterSelectIndex = -1;
    int d3SelectIndex = -1;

    string labelSearchWord;

    void ViewToolBar()
    {
        if (!AudioManager.IsInitialized())
        {
            return;
        }

        selectType = GUILayout.Toolbar(selectType, new string[] { "Master", "Category", "Label", "3D" });
        
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
            case (int)SELECT_TYPE.D3SETTINGS:
                ViewSelect3DSettings();
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

    void ViewSelect3DSettings()
    {
        if (!AudioManager.IsInitialized())
        {
            return;
        }
        GUILayout.Label("Loading 3DSettings List");

        d3Name = AudioManager.GetAudio3DSettingsNameList();

        select3DPos = EditorGUILayout.BeginScrollView(select3DPos);

        SetSelectButton(d3Name, ref d3SelectIndex);

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        SetHeader("Save");
        if (GUILayout.Button("選択中の項目をクリップボードにコピー"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            if (0 <= d3SelectIndex && d3SelectIndex < AudioManager.GetAudio3DSettingsNum())
            {
                Audio3DSettings settings = AudioManager.GetAudio3DSettingsInfo(d3Name[d3SelectIndex]);

                if (settings != null)
                {
                    AudioSourceWrapper wrapper = new AudioSourceWrapper();

                    wrapper.spatialName = settings.spatialName;
                    wrapper.spatialBlend = settings.spatialBlend;
                    wrapper.reverbZoneMix = settings.reverbZoneMix;
                    wrapper.dopplerLevel = settings.dopplerLevel;
                    wrapper.spread = settings.spread;
                    wrapper.rolloffMode = settings.rolloffMode;
                    wrapper.minDistance = settings.minDistance;
                    wrapper.maxDistance = settings.maxDistance;
                    wrapper.customRolloffCurve = settings.customRolloffCurve;
                    wrapper.spatialBlendCurve = settings.spatialBlendCurve;
                    wrapper.reverbZoneMixCurve = settings.reverbZoneMixCurve;
                    wrapper.spreadCurve = settings.spreadCurve;

                    string json = JsonUtility.ToJson(wrapper);
                    GUIUtility.systemCopyBuffer = json;
                }
            }
#endif
        }
        if (GUILayout.Button("選択中の項目をScriptableObjectにコピー"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            if (0 <= d3SelectIndex && d3SelectIndex < AudioManager.GetAudio3DSettingsNum())
            {
                Audio3DSettings settings = AudioManager.GetAudio3DSettingsInfo(d3Name[d3SelectIndex]);

                if (settings != null)
                {
                    AudioManager.SaveAudio3DSettingsParam(settings);
                }
            }
#endif
        }

        SetHeader("Undo");
        if (GUILayout.Button("選択中の項目の値を元に戻す"))
        {
#if UNITY_5_0 || UNITY_5_1
#else
            if (0 <= d3SelectIndex && d3SelectIndex < AudioManager.GetAudio3DSettingsNum())
            {
                Audio3DSettings settings = AudioManager.GetAudio3DSettingsInfo(d3Name[d3SelectIndex]);

                if (settings != null)
                {
                    AudioManager.UndoAudio3DSettingsParam(settings);
                }
            }
#endif
        }
    }

    [System.Serializable]
    private class AudioSourceWrapper
    {
        public string spatialName = "";

        [Range(0, 1)]
        public float spatialBlend = 1;

        [Range(0, 1.1f)]
        public float reverbZoneMix = 1;

        [Range(0, 5)]
        public float dopplerLevel = 1;

        [Range(0, 360)]
        public int spread = 0;

        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        public float minDistance = 1;

        public float maxDistance = 500;

        // キーフレームが2つ以上あったらAnimationCurveを使用する
        //AudioSourceCurveType
        public AnimationCurve customRolloffCurve;

        public AnimationCurve spatialBlendCurve;

        public AnimationCurve reverbZoneMixCurve;

        public AnimationCurve spreadCurve;
    }

    void CopyLabelInfo(ref string info, AudioManager.AudioLabelSettings label)
    {
        info += label.name + "\t" + label.clipName + "\t" + label.isLoop + "\t" + label.volume + "\t";
        info += label.maxPlaybacksBehavior.ToString() + "\t" + label.priority + "\t" + label.categoryName + "\t" + label.singleGroup + "\t";
        info += label.maxPlaybacksNum + "\t" + label.isStealOldest + "\t" + label.unityMixerName + "\t" + label.spatialGroup + "\t";
        info += label.playStartDelay + "\t" + label.playInterval + "\t" + label.pan + "\t" + label.pitchShiftCent + "\t" + label.isPlayLastSamples + "\t";
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
		if (text != null)
		{
			for (int i = 0; i < text.Count; ++i)
			{
				src += text[i];
				if (i + 1 != text.Count)
				{
					src += "\n";
				}
			}
		}
        src += "\"\t";
    }

    void SetListString(ref string src, string[] text)
    {
        src += "\"";
		if (text != null)
		{
			for (int i = 0; i < text.Length; ++i)
			{
				src += text[i];
				if (i + 1 != text.Length)
				{
					src += "\n";
				}
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

                if (GUILayout.Button(nameList[i], GUILayout.Width(250)))
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
    Vector2 d3InfoPos;


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
            case (int)SELECT_TYPE.D3SETTINGS:
                View3DSettingsInfo();
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

		if (GUILayout.Button("Apply"))
		{
			AudioManager.UpdateUnityMixerName(label.name, label.unityMixerName);
		}

		SetHeader("3D Group");

        label.spatialGroup = EditorGUILayout.TextField("SpatialGroup", label.spatialGroup);

		if (GUILayout.Button("Apply"))
		{
			AudioManager.UpdateSpatialGroupName(label.name, label.spatialGroup);
		}

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

        int rndSrcLen = (label.randomSource != null ) ? label.randomSource.Length : 0;
        rndSrcLen = EditorGUILayout.IntField("RandomSourceNum", rndSrcLen);

        if (EditorGUI.EndChangeCheck())
        {
            if (label.randomSource == null)
            {
                UpdateArrayNum(rndSrcLen, 0, ref label.randomSource);
            }
            else
            {
                UpdateArrayNum(rndSrcLen, label.randomSource.Length, ref label.randomSource);
            }
            //UpdateArrayNum(rndSrcLen, label.randomSource.Count, label.randomSource
        }

        if (label.randomSource != null)
        {
            //for (int i = 0; i < label.randomSource.Count; ++i)
            for (int i = 0; i < label.randomSource.Length; ++i)
            {
                label.randomSource[i] = EditorGUILayout.TextField("RandomSource(" + (i + 1) + "):", label.randomSource[i]);
            }
        }

        if (GUILayout.Button("Apply"))
        {
            AudioManager.UpdateRandomSourceInfo(label.name);
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

        //rndSrcLen = label.duckingCategories.Count;
        rndSrcLen = (label.duckingCategories == null) ? 0 : label.duckingCategories.Length;
        rndSrcLen = EditorGUILayout.IntField("DuckingCategoriesNum", rndSrcLen);

        if (EditorGUI.EndChangeCheck())
        {
            if (label.duckingCategories == null)
            {
                UpdateArrayNum(rndSrcLen, 0, ref label.duckingCategories);
            }
            else
            {
                //UpdateArrayNum(rndSrcLen, label.duckingCategories.Count, label.duckingCategories);
                UpdateArrayNum(rndSrcLen, label.duckingCategories.Length, ref label.duckingCategories);
            }
        }

        if (label.duckingCategories != null)
        {
            //for (int i = 0; i < label.duckingCategories.Count; ++i)
            for (int i = 0; i < label.duckingCategories.Length; ++i)
            {
                label.duckingCategories[i] = EditorGUILayout.TextField("DuckingCategory(" + (i + 1) + "):", label.duckingCategories[i]);
            }
        }

        label.duckingStartTime = EditorGUILayout.Slider("DuckingStartTime", label.duckingStartTime, 0, 30);

        label.duckingEndTime = EditorGUILayout.Slider("DuckingEndTime", label.duckingEndTime, 0, 30);

        label.duckingVolumeFactor = EditorGUILayout.Slider("DuckingVolumeFactor", label.duckingVolumeFactor, 0, 1);

        label.autoRestoreDucking = EditorGUILayout.Toggle("AutoRestoreDucking", label.autoRestoreDucking);

        label.restoreTime = EditorGUILayout.FloatField("RestoreTime", label.restoreTime);

        label.isAndroidNative = EditorGUILayout.Toggle("IsAndroidNative", label.isAndroidNative);

        EditorGUILayout.EndScrollView();
    }

    void View3DSettingsInfo()
    {
        if (d3SelectIndex < 0 || AudioManager.GetAudio3DSettingsNum() <= d3SelectIndex)
        {
            d3SelectIndex = -1;
            return;
        }

        if (!AudioManager.IsInitialized())
        {
            return;
        }

        if (AudioManager.GetAudio3DSettingsNum() == 0 || d3Name == null)
        {
            return;
        }

        Audio3DSettings settings = AudioManager.GetAudio3DSettingsInfo(d3Name[d3SelectIndex]);

        if (settings == null)
        {
            return;
        }

        d3InfoPos = EditorGUILayout.BeginScrollView(d3InfoPos);

        SetHeader("3DSettings: " + d3Name[d3SelectIndex]);

        settings.spatialBlend = EditorGUILayout.Slider("SpatialBlend", settings.spatialBlend, 0, 1);
        settings.reverbZoneMix = EditorGUILayout.Slider("ReverbZoneMix", settings.reverbZoneMix, 0, 1.1f);
        settings.dopplerLevel = EditorGUILayout.Slider("DopplerLevel", settings.dopplerLevel, 0, 5);
        settings.spread = EditorGUILayout.IntSlider("Spread", settings.spread, 0, 360);
        settings.rolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("RolloffMode", settings.rolloffMode);
        settings.minDistance = EditorGUILayout.FloatField("MinDistance", settings.minDistance);
        settings.maxDistance = EditorGUILayout.FloatField("MaxDistance", settings.maxDistance);
        settings.customRolloffCurve = EditorGUILayout.CurveField("CustomRolloffCurve", settings.customRolloffCurve);
        settings.spatialBlendCurve = EditorGUILayout.CurveField("SpatialBrendCurve", settings.spatialBlendCurve);
        settings.reverbZoneMixCurve = EditorGUILayout.CurveField("ReverbZoneMixCurve", settings.reverbZoneMixCurve);
        settings.spreadCurve = EditorGUILayout.CurveField("SpreadCurve", settings.spreadCurve);

        AudioManager.UpdateAudio3DSettings(settings);

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

    void UpdateArrayNum(int newNum, int baseNum, ref string[] array)
    {
        if (newNum == 0)
        {
            array = null;
        }
        else
        {
            string[] tmp = new string[newNum];
            for(int i=0; i<newNum; ++i)
            {
                if ( i < baseNum )
                {
                    tmp[i] = array[i];
                }
                else
                {
                    tmp[i] = "";
                }
            }
            array = tmp;
        }
    }

}
