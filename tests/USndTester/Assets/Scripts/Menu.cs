using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using USnd;
using System.IO;
using System;

using UnityEngine.Audio;

public class Menu : MonoBehaviour {

    public bool loadBinary = false;

    public List<string> MASTER_XML_NAME = new List<string>();//= "MasterSettings";
    public List<string> CATEGORY_XML_NAME = new List<string>();//= "CategorySettings";
    public List<string> LABEL_XML_NAME = new List<string>();//= "LabelSettings";

    public List<string> MASTER_BINARY_NAME = new List<string>();//= "MasterSettings";
    public List<string> CATEGORY_BINARY_NAME = new List<string>();//= "CategorySettings";
    public List<string> LABEL_BINARY_NAME = new List<string>();//= "LabelSettings";

    
    public List<string> AUDIO_CLIP_PATH = new List<string>();



    int subStep = 0;

#if UNITY_EDITOR
    public int SCREEN_WIDTH = 1136;
    public int SCREEN_HEIGHT = 800;//640;
#else
    public int SCREEN_WIDTH = 1136;
    public int SCREEN_HEIGHT = 640;
#endif


    const int SELECT_XML = 0;
    const int LOADING_XML = 1;
    const int FINISH_LOADING = 2;
    const int LOADING_XML2 = 3;

    int status = SELECT_XML;

    private bool[] LoadXmlFlag;


    bool loadAudioClip = false;

    public int AGING_INTERVAL = 20;      // エージングのインターバル

    public List<AudioMixerSnapshot> snapshotList = new List<AudioMixerSnapshot>();

    public GameObject audioTarget = null;

    IEnumerator loadAssetBundle()
    {
        using (WWW loader = WWW.LoadFromCacheOrDownload("file://" + Application.streamingAssetsPath + "/mainmixer.mixer", 0))
        {
            yield return loader;
            if (loader.error != null)
                throw new Exception("WWW download had an error:" + loader.error);
            AssetBundle bundle = loader.assetBundle;
            //AudioMixer[] mixer = bundle.LoadAllAssets<AudioMixer>();

            AudioMixer main = bundle.LoadAsset<AudioMixer>("Main");

            AudioManager.SetAudioMixer(main);

            bundle.Unload(false);

        }
    }

    IEnumerator LoadAndroidData(string path, string saveName)
    {
        WWW www = new WWW(path);
        if (www.error != null)
        {
            Debug.Log("!!! " + www.error);
        }
        else
        {
            yield return www;
        }

        Debug.Log("!!! www no error:" + www.isDone + " : " + www.bytes.Length);
#if UNITY_ANDROID || !UNITY_EDITOR
        string toPath = Application.persistentDataPath + "/" + saveName;
        Debug.Log("!!! load: " + toPath);
        File.WriteAllBytes(toPath, www.bytes);
#endif
    }

	// Use this for initialization
	void Start () {

        status = SELECT_XML;
        if (loadBinary)
        {
            LoadXmlFlag = new bool[LABEL_BINARY_NAME.Count];
            for (int i = 0; i < LABEL_BINARY_NAME.Count; ++i)
            {
                LoadXmlFlag[i] = true;
            }
        }
        else
        {
            LoadXmlFlag = new bool[LABEL_XML_NAME.Count];
            for (int i = 0; i < LABEL_XML_NAME.Count; ++i)
            {
                LoadXmlFlag[i] = true;
            }
        }
        AudioManager.Initialize();

        Audio3DSettings d3setting = Resources.Load<Audio3DSettings>("bgm");
        AudioManager.SetAudio3DSettings(d3setting);


		TextAsset ta;
		Stream xml;


		ta = Resources.Load("sample3D") as TextAsset;
		AudioManager.SetAudio3DSettingsFromJson(ta.ToString());


		ta = Resources.Load("Test3D") as TextAsset;
		AudioManager.SetAudio3DSettingsFromJson(ta.ToString());



		AudioMixer umix = Resources.Load<AudioMixer>("Main");
        AudioManager.SetAudioMixer(umix);
        //StartCoroutine(loadAssetBundle());

#if UNITY_ANDROID || !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.Android)
        {
            var cls = new AndroidJavaClass("android.os.Build$VERSION");
            int apiLevel = cls.GetStatic<int>("SDK_INT");

            string path;
            if (apiLevel >= 23)
            {
                path = "jar:file://" + Application.dataPath + "!/assets/shot_1p.ogg";
            }
            else
            {
                path = Application.streamingAssetsPath + "/shot_1p.ogg";
            }

            StartCoroutine(LoadAndroidData(path, "shot_1p.ogg"));
        }
#endif



        //mainCamera = FindObjectOfType<Camera>();
        
        float start = Time.realtimeSinceStartup;

        // Mixer情報を取得、シーンに登録済み
        /*
        mixer = gameObject.GetComponent<AudioMixerSettings>();
        if ( mixer.mixer != null )
        {
	        AudioManager.SetUnityMixerInfo(mixer);
		}
        Debug.Log("!!!! start component time: " + (float)(Time.realtimeSinceStartup - start));
         */
        /*
        float startClip = Time.realtimeSinceStartup;
        // AudioClipを読み込み(XMLから追加でAudioSourceを生成するとき用なので、生成しない場合は不要)
        AudioClip[] clip = Resources.LoadAll<AudioClip>("AudioClip");
        Debug.Log("!!!! start clip time: " + (float)(Time.realtimeSinceStartup - startClip));
        if (clip != null)
        {
            AudioManager.AddAudioClip(clip);
        }
        */
    }


    bool init = false;
    int loadStep = 0;
    float stepStartTime;
    int agingCount = 0;
    // Update is called once per frame
    void Update()
    {
        /*
         // 解像度に対してアスペクト比を固定する、がGUIには関係ない？
        float targetAspect;
        float currentAspect;
        float ratio;

        targetAspect = SCREEN_WIDTH / SCREEN_HEIGHT;

        currentAspect = Screen.width * 1.0f / Screen.height;
        ratio = currentAspect / targetAspect;

        if (1.0f > ratio)
        {
            Rect rec = mainCamera.rect;
            rec.x = 0.0f;
            rec.width = 1.0f;
            rec.y = (1.0f - ratio) / 2.0f;
            rec.height = ratio;
            mainCamera.orthographicSize = Screen.width / 2.0f;
        }
        else
        {
            Rect rec = mainCamera.rect;
            ratio = 1.0f / ratio;
            rec.x = (1.0f - ratio) / 2.0f;
            rec.width = ratio;
            rec.y = 0.0f;
            rec.height = 1.0f;
            mainCamera.orthographicSize = Screen.height / 2.0f;
        }
        */

        if ( aging == true )
        {
            if ( ++agingCount > AGING_INTERVAL)
            {
                agingCount = 0;
            }
            if (agingCount == 0)
            {
                int playLabel = UnityEngine.Random.Range(0, labelNum);
                AudioManager.Play(labelNames[playLabel]);
            }
        }
 

        if (status == LOADING_XML)
        {
            if (init == false)
            {
                if (loadBinary)
                {
                    loadBinaryTable();
                }
                else
                {
                    loadXmlTable();
                }
            }
        }
	}

    void loadBinaryTable()
    {
        // XMLで更新・存在しないものは追加
        if (loadStep == 0)
        {
            stepStartTime = Time.realtimeSinceStartup;

            TextAsset text = Resources.Load("Binary/" + MASTER_BINARY_NAME[subStep]) as TextAsset;
            byte[] row_data = text.bytes;

            AudioManager.LoadBinaryTable(row_data);

            loadStep = 1;
        }
        else if (loadStep == 1)
        {
            if (++subStep < MASTER_BINARY_NAME.Count)
            {
                TextAsset text = Resources.Load("Binary/" + MASTER_BINARY_NAME[subStep]) as TextAsset;
                byte[] row_data = text.bytes;

                AudioManager.LoadBinaryTable(row_data);
            }
            else
            {
                subStep = 0;

                TextAsset text = Resources.Load("Binary/" + CATEGORY_BINARY_NAME[subStep]) as TextAsset;
                byte[] row_data = text.bytes;

                AudioManager.LoadBinaryTable(row_data);

                loadStep = 2;
            }                    
        }
        else if (loadStep == 2)
        {
            if (++subStep < CATEGORY_BINARY_NAME.Count)
            {
                TextAsset text = Resources.Load("Binary/" + CATEGORY_BINARY_NAME[subStep]) as TextAsset;
                byte[] row_data = text.bytes;

                AudioManager.LoadBinaryTable(row_data);
            }
            else
            {
                subStep = 0;
                setSubStep();
                if (subStep == -1)
                {
                    endInit();
                }
                else
                {

                    TextAsset text = Resources.Load("Binary/" + LABEL_BINARY_NAME[subStep]) as TextAsset;
                    byte[] row_data = text.bytes;

                    AudioManager.LoadBinaryTable(row_data);

                    loadStep = 3;
                }
            }
        }
        else if (loadStep == 3)
        {
            if (++subStep < LABEL_BINARY_NAME.Count)
            {
                setSubStep();
                if (subStep == -1)
                {
                    endInit();
                }
                else
                {
                    TextAsset text = Resources.Load("Binary/" + LABEL_BINARY_NAME[subStep]) as TextAsset;
                    byte[] row_data = text.bytes;

                    AudioManager.LoadBinaryTable(row_data);
                }
            }
            else
            {
                endInit();
            }
        }
    }


    void loadXmlTable()
    {
        TextAsset ta;
        Stream xml;


        // XMLで更新・存在しないものは追加
        if (loadStep == 0)
        {
            stepStartTime = Time.realtimeSinceStartup;

            ta = Resources.Load("XML/" + MASTER_XML_NAME[subStep]) as TextAsset;
            if (ta != null)
            {
                xml = new MemoryStream(ta.bytes);
                AudioManager.LoadMasterXml(xml);
            }

            loadStep = 1;
        }
        else if (loadStep == 1)
        {
            if (AudioManager.GetLoadXmlStatus() != AudioDefine.LOAD_XML_STATUS.LOADING)
            {
                Debug.Log("!!!! loadStep0 xml master time: " + (float)(Time.realtimeSinceStartup - stepStartTime));

                if (++subStep < MASTER_XML_NAME.Count)
                {
                    ta = Resources.Load("XML/" + MASTER_XML_NAME[subStep]) as TextAsset;
                    if (ta != null)
                    {
                        xml = new MemoryStream(ta.bytes);
                        AudioManager.LoadMasterXml(xml);
                    }
                }
                else
                {
                    subStep = 0;

                    stepStartTime = Time.realtimeSinceStartup;

                    ta = Resources.Load("XML/" + CATEGORY_XML_NAME[subStep]) as TextAsset;
                    if (ta != null)
                    {
                        xml = new MemoryStream(ta.bytes);
                        AudioManager.LoadCategoryXml(xml);
                    }
                    loadStep = 2;
                }
            }
        }
        else if (loadStep == 2)
        {
            if (AudioManager.GetLoadXmlStatus() != AudioDefine.LOAD_XML_STATUS.LOADING)
            {
                Debug.Log("!!!! loadStep1 xml category time: " + (float)(Time.realtimeSinceStartup - stepStartTime));

                if (++subStep < CATEGORY_XML_NAME.Count)
                {
                    ta = Resources.Load("XML/" + CATEGORY_XML_NAME[subStep]) as TextAsset;
                    if (ta != null)
                    {
                        xml = new MemoryStream(ta.bytes);
                        AudioManager.LoadCategoryXml(xml);
                    }
                }
                else
                {
                    subStep = 0;
                    setSubStep();
                    if (subStep == -1)
                    {
                        endInit();
                    }
                    else
                    {
                        stepStartTime = Time.realtimeSinceStartup;

                        ta = Resources.Load("XML/" + LABEL_XML_NAME[subStep]) as TextAsset;
                        Debug.Log("+++++++++++load xml:" + LABEL_XML_NAME[subStep] + " " + subStep);
                        if (ta != null)
                        {
                            xml = new MemoryStream(ta.bytes);
                            AudioManager.LoadLabelXml(0, xml);
                        }
                        loadStep = 3;
                    }
                }
            }
        }
        else if (loadStep == 3)
        {
            if (AudioManager.GetLoadXmlStatus() != AudioDefine.LOAD_XML_STATUS.LOADING)
            {
                Debug.Log("!!!! loadStep2 xml label time: " + (float)(Time.realtimeSinceStartup - stepStartTime));

                if (++subStep < LABEL_XML_NAME.Count)
                {
                    setSubStep();
                    if (subStep == -1)
                    {
                        endInit();
                    }
                    else
                    {
                        ta = Resources.Load("XML/" + LABEL_XML_NAME[subStep]) as TextAsset;
                        Debug.Log("+++++++++++load xml:" + LABEL_XML_NAME[subStep] + " " + subStep);
                        if (ta != null)
                        {
                            xml = new MemoryStream(ta.bytes);
                            AudioManager.LoadLabelXml(0, xml);
                        }
                    }
                }
                else
                {

                    stepStartTime = Time.realtimeSinceStartup;

                    endInit();

                    Debug.Log("!!!! loadStep end time: " + (float)(Time.realtimeSinceStartup - stepStartTime));
                }
            }
        }
    }

    void endInit()
    {
        init = true;
        snapNum = snapshotList.Count;
        List<string> nameList = new List<string>();
        for (int i = 0; i < snapNum; ++i )
        {
            nameList.Add(snapshotList[i].name);
        }
        snapNames = nameList.ToArray();
        labelNum = AudioManager.GetLabelNum();
        labelNames = AudioManager.GetLabelNameList();
        masterNum = AudioManager.GetMasterNum();
        masterNames = AudioManager.GetMasterNameList();
        categoryNum = AudioManager.GetCategoryNum();
        categoryNames = AudioManager.GetCategoryNameList();
        masterValue = new float[masterNum];
        categoryValue = new float[categoryNum];
        for (int i = 0; i < masterNum; ++i)
        {
            masterValue[i] = AudioManager.GetMasterVolume(masterNames[i]);
        }
        for (int i = 0; i < categoryNum; ++i)
        {
            categoryValue[i] = AudioManager.GetCategoryVolume(categoryNames[i]);
        }

        loadStep = 4;

    }

    void deleteInfo()
    {
        snapNum = 0;
        snapNames = null;
        labelNum = 0;
        labelNames = null;
        masterNum = 0;
        masterNames = null;
        categoryNum = 0;
        categoryNames = null;
        masterValue = null;
        categoryValue = null;
    }

    void setSubStep()
    {
        if (LoadXmlFlag[subStep] == false)
        {
            int tmpStep = subStep;
            for (int i = subStep + 1; i < LoadXmlFlag.Length; ++i)
            {
                if (LoadXmlFlag[i] == true)
                {
                    subStep = i;
                    break;
                }
            }
            if (tmpStep == subStep)
            {
                subStep = -1;
            }
        }
    }

    int snapNum = 0;
    string[] snapNames = null;
    int labelNum = 0;
    string[] labelNames = null;
    int masterNum = 0;
    string[] masterNames = null;
    int categoryNum = 0;
    string[] categoryNames = null;
    int pageIndex = 0;
    int masterIndex = 0;
    int categoryIndex = 0;
    int snapIndex = 0;
    float[] masterValue = null;
    float[] categoryValue = null;
    float snapChangeTime = 1;
    public int LABEL_VIEW_NUM = 5;
    public int LABEL_VIEW_ROW = 5;
    bool aging = false;


    string savePath = @"D:\USndDefine.cs";


    void OnGUI()
    {
        GUILayout.Label("", GUILayout.Height(5));
        switch(status)
        {
            case SELECT_XML:
                SelectXmlGUI();
                break;
            case LOADING_XML:
                LoadingXmlGUI();
                break;
            case FINISH_LOADING:
                FinishLoadingGUI();
                break;
            case LOADING_XML2:
                LoadingXmlGUI2();
                break;
        }
    }

    Vector2 vScroll;
    bool selectAll = true;


    void SelectXmlGUI()
    {
        string labelText = null;
        string buttonText = null;
        string buttonText2 = null;

        if (loadBinary)
        {
            labelText = "Select Label Binary";
            buttonText = "Load Binary";
            buttonText2 = "Load Binary(先にAudioClipを読み込む)";
        }
        else
        {
            labelText = "Select Label XML";
            buttonText = "Load XML";
            buttonText2 = "Load XML(先にAudioClipを読み込む)";
        }

        GUILayout.Label(labelText);

        if (GUILayout.Button(buttonText/*, GUILayout.Width(300), GUILayout.Height(100)*/))
        {
            status = LOADING_XML;
            subStep = 0;
        }
        /*
        if (GUILayout.Button(buttonText2))
        {
            status = LOADING_XML2;
            subStep = 0;
        }
        */
        List<string> loadList = null;
        if ( loadBinary )
        {
            loadList = LABEL_BINARY_NAME;
        }
        else
        {
            loadList = LABEL_XML_NAME;
        }

        vScroll = GUILayout.BeginScrollView(vScroll, GUILayout.Height(SCREEN_HEIGHT-400));
        for (int i = 0; i < loadList.Count; ++i)
        {
            LoadXmlFlag[i] = GUILayout.Toggle(LoadXmlFlag[i], loadList[i], GUILayout.Width(300), GUILayout.Height(100));
        }
        GUILayout.EndScrollView();

        if ( GUILayout.Button("Select All ON/OFF") )
        {
            selectAll = selectAll ? false : true;
            for (int i = 0; i < loadList.Count; ++i)
            {
                LoadXmlFlag[i] = selectAll;
            }
        }

    }

    int currentLoad = 0;
    int maxClipNum = 0;

    void LoadingXmlGUI2()
    {
        Debug.Log("!!! Load All Resources AudioClip");
        AudioClip[] clip = Resources.LoadAll<AudioClip>("AudioClip");
        if (clip != null)
        {
            AudioManager.AddAudioClip(clip);
            loadAudioClip = true;
            status = LOADING_XML;
        }
        GUILayout.Label("Loading: " + currentLoad + "/" + maxClipNum);
    }

    void LoadingXmlGUI()
    {
        if ( loadStep == 4 )
        {            
            // AudioClipを読み込み(XMLから追加でAudioSourceを生成するとき用なので、生成しない場合は不要)
            //AudioClip[] clip = Resources.LoadAll<AudioClip>("AudioClip");

            Debug.Log("!!! Resource Load Start.");
            if (loadAudioClip == false)
            {
                StartCoroutine(loadClip());
                loadStep = 5;
            }
            else
            {
                status = FINISH_LOADING;
            }
        }
        GUILayout.Label("Loading: " + currentLoad + "/" + maxClipNum);
    }

    void onLoadFinishCallback(string status)
    {
        Debug.Log("!!! onLoadFinishCallback:" + status);
    }


    IEnumerator loadClip()
    {
        
        ResourceRequest req = null;
        string[] clipList = AudioManager.GetAudioClipNameAll();
        maxClipNum = clipList.Length;
        int pathIndex = 0;

        currentLoad = 0;
        while (currentLoad < clipList.Length)
        {
            if (req == null)
            {
                req = Resources.LoadAsync<AudioClip>(clipList[currentLoad]);
            }

            while(req.isDone==false)
            {
                yield return null;
            }

            if (req.isDone == true)
            {
                AudioClip clip = req.asset as AudioClip;
                
                if (clip != null)
                {
                    AudioManager.AddAudioClip(clip);
                    ++currentLoad;
                    pathIndex = 0;
                    req = null;
                }
                else
                {
                    if ( pathIndex >= AUDIO_CLIP_PATH.Count )
                    {
                    	++currentLoad;
	                    pathIndex = 0;
	                    req = null;
                    }
                    else
                    {
	                    req = Resources.LoadAsync<AudioClip>(AUDIO_CLIP_PATH[pathIndex] + "/" + clipList[currentLoad]);
	                    ++pathIndex;
	                }
                }
            }
        }
        AudioManager.SetAudioClipToLabelAll();
        status = FINISH_LOADING;

        string toPath = Application.persistentDataPath + "/" + "shot_1p.ogg";
        Debug.Log("!!! load android native:" + toPath);
        AudioManager.SetAndroidNativeToLabel("se_comp_finish", toPath, this.name, "onLoadFinishCallback");
 


        Debug.Log("!!! Resource Load Finish.");
         /*
#if UNITY_ANDROID
        using (WWW loader = WWW.LoadFromCacheOrDownload(Application.streamingAssetsPath + "/sample", 0))
#else
        using (WWW loader = WWW.LoadFromCacheOrDownload("file://" + Application.streamingAssetsPath + "/sample", 0))
#endif
        {
            yield return loader;
            if (loader.error != null)
            {
                Debug.Log("Error: " + loader.error);
                throw new Exception("WWW download had an error:" + loader.error);
            }
            AssetBundle bundle = loader.assetBundle;
            AudioClip[] clips = bundle.LoadAllAssets<AudioClip>();

            AudioManager.AddAudioClip(clips);
            Debug.Log("ClipNum:" + clips.Length);

            // Unloadしなければ再生成功する
            //clip.LoadAudioData();
            //bundle.Unload(false);
            AudioManager.SetAudioClipToLabelAll();
            Resources.UnloadUnusedAssets();
            Debug.Log("FINISH_LOADING");
            status = FINISH_LOADING;
        }*/
    }

    int instanceId;
    bool zeroVolume = false;


    void FinishLoadingGUI()
    {
        if (init == false) return;

        int labelIndex = 0;
        int PAGE_MAX = labelNum / LABEL_VIEW_NUM;
        if ((labelNum % LABEL_VIEW_NUM) != 0)
        {
            ++PAGE_MAX;
        }

        List<GUILayoutOption> playButton = new List<GUILayoutOption>();
        playButton.Add(GUILayout.Width(250));
        playButton.Add(GUILayout.Height(40));

        List<GUILayoutOption> stopButton = new List<GUILayoutOption>();
        stopButton.Add(GUILayout.Width(85));
        stopButton.Add(GUILayout.Height(40));


        long monoUsed = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
        long monoSize = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
        long totalUsed = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong(); // == Profiler.usedHeapSize
        long totalSize = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
        string text = string.Format(
            "mono:{0:f2}/{1:f2} MB({2:f1}%)\n" +
            "total:{3:f2}/{4:f2} MB({5:f1}%)\n" + 
			"pay time:{6:f3}\n",
            monoUsed / 1024.0f / 1024.0f, monoSize / 1024.0f / 1024.0f, 100.0 * monoUsed / monoSize,
            totalUsed / 1024.0f / 1024.0f, totalSize / 1024.0f / 1024.0f, 100.0 * totalUsed / totalSize,
			AudioManager.GetPlayTime(instanceId));
        GUILayout.Label(text);


        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();


        // ------------------------------------------------------
        for (int i = 0; i < LABEL_VIEW_NUM; ++i)
        {
            labelIndex = pageIndex * LABEL_VIEW_NUM + i;
            GUILayout.BeginHorizontal();
            if (labelIndex < labelNum)
            {
                if (GUILayout.Button(labelNames[labelIndex], playButton.ToArray()))
                {
                    float playTime = Time.realtimeSinceStartup;
                    //Profiler.BeginSample("play");
                    Debug.Log("time skip");
                    //instanceId = AudioManager.Play(labelNames[labelIndex]);
                    //Profiler.EndSample();
                    instanceId = AudioManager.Play3D(labelNames[labelIndex], audioTarget.transform);
					//instanceId = AudioManager.Play2D(labelNames[labelIndex], 1, 0, 0.5f, 0);
					zeroVolume = false;
                    Debug.Log("!!!! play time: " + (float)(Time.realtimeSinceStartup - playTime));
                    Debug.Log("!!!! play length:" + AudioManager.GetLabelLength(labelNames[labelIndex]) +
                                " samples:" + AudioManager.GetLabelSamples(labelNames[labelIndex]));

                    string[] clips = AudioManager.GetRandomSourceNames(labelNames[labelIndex]);
                    for(int j=0; j<clips.Length; ++j)
                    {
                        Debug.Log("!!! j=" + j + " name=" + clips[j]);
                    }
                }
                if (GUILayout.Button("STOP Label", stopButton.ToArray()))
                {
                    AudioManager.StopLabel(labelNames[labelIndex], 0.5f);
                }
                if (GUILayout.Button("Prepare Play", stopButton.ToArray()))
                {
                    instanceId = AudioManager.Prepare(labelNames[labelIndex]);
                    AudioManager.SetPitch(instanceId, 200, 0);
                    AudioManager.PlayInstance(instanceId);
                    zeroVolume = false;
                }
                if (GUILayout.Button("Delay Play", stopButton.ToArray()))
                {
                    instanceId = AudioManager.Play(labelNames[labelIndex], 1);
                    zeroVolume = false;
                }
            }
            else
            {
                if (GUILayout.Button("empty", playButton.ToArray()))
                {
                    // AudioManager.Play(labelNames[labelIndex]);
                }
                if (GUILayout.Button("STOP", stopButton.ToArray()))
                {
                    //AudioManager.StopLabel(labelNames[labelIndex]);
                }
            }
            GUILayout.EndHorizontal();

        }
        // ------------------------------------------------------
        string msg = (pageIndex + 1) + "/" + PAGE_MAX;
        GUILayout.Label(msg);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<<"))
        {
            --pageIndex;
            if (pageIndex < 0)
            {
                pageIndex = (labelNum / LABEL_VIEW_NUM);
            }
            Debug.Log("page:" + pageIndex);
        }
        if (GUILayout.Button(">>"))
        {
            ++pageIndex;
            if (pageIndex >= PAGE_MAX)
            {
                pageIndex = 0;
            }
            Debug.Log("page:" + pageIndex);
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<< -10"))
        {
            pageIndex -= 10;
            if (pageIndex < 0)
            {
                pageIndex = (labelNum / LABEL_VIEW_NUM) - 10;
                if (pageIndex < 0) pageIndex = 0;
            }
            Debug.Log("page:" + pageIndex);
        }
        if (GUILayout.Button("+10 >>"))
        {
            pageIndex += 10;
            if (pageIndex >= PAGE_MAX)
            {
                pageIndex = 0;
            }
            Debug.Log("page:" + pageIndex);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<< -100"))
        {
            pageIndex -= 100;
            if (pageIndex < 0)
            {
                pageIndex = (labelNum / LABEL_VIEW_NUM) - 100;
                if (pageIndex < 0) pageIndex = 0;
            }
            Debug.Log("page:" + pageIndex);
        }
        if (GUILayout.Button("+100 >>"))
        {
            pageIndex += 100;
            if (pageIndex >= PAGE_MAX)
            {
                pageIndex = 0;
            }
            Debug.Log("page:" + pageIndex);
        }
        GUILayout.EndHorizontal();

        // ------------------------------------------------------
        if (GUILayout.Button("STOP ALL"))
        {
            AudioManager.StopAll();
        }

        // ------------------------------------------------------
        if (GUILayout.Button("STOP Last Instance"))
        {
            AudioManager.Stop(instanceId);
        }

       
        float value;

        // ------------------------------------------------------
        if (masterNum > 0)
        {
            GUILayout.BeginHorizontal();
            msg = "Master:" + masterNames[masterIndex] + "  volume:" + masterValue[masterIndex];
            GUILayout.Label(msg);

            if (GUILayout.Button(">>"))
            {
                ++masterIndex;
                if (masterIndex >= masterNum)
                {
                    masterIndex = 0;
                }
            }
            GUILayout.EndHorizontal();

            value = GUILayout.HorizontalSlider(masterValue[masterIndex], 0.0f, 1.0f);
            if (value != masterValue[masterIndex])
            {
                AudioManager.SetMasterVolume(masterNames[masterIndex], value);
                masterValue[masterIndex] = value;
            }
        }
        // ------------------------------------------------------
        if (categoryNum > 0)
        {
            GUILayout.BeginHorizontal();
            msg = "Category:" + categoryNames[categoryIndex] + "  volume:" + categoryValue[categoryIndex];
            GUILayout.Label(msg);

            if (GUILayout.Button(">>"))
            {
                ++categoryIndex;
                if (categoryIndex >= categoryNum)
                {
                    categoryIndex = 0;
                }
            }
            GUILayout.EndHorizontal();

            value = GUILayout.HorizontalSlider(categoryValue[categoryIndex], 0.0f, 1.0f);
            if (value != categoryValue[categoryIndex])
            {
                AudioManager.SetCategoryVolume(categoryNames[categoryIndex], value);
                categoryValue[categoryIndex] = value;
            }
        }
        if (snapNum > 0)
        {

            GUILayout.BeginHorizontal();
            // ------------------------------------------------------
            msg = "Snapshot:" + snapNames[snapIndex] + "  change time:" + snapChangeTime + "s";
            GUILayout.Label(msg);

            if (GUILayout.Button(">>"))
            {
                ++snapIndex;
                if (snapIndex >= snapNum)
                {
                    snapIndex = 0;
                }
            }
            GUILayout.EndHorizontal();

            value = GUILayout.HorizontalSlider(snapChangeTime, 0.0f, 10.0f);
            if (value != snapChangeTime)
            {
                snapChangeTime = value;
            }

            if (GUILayout.Button("Snapshot Change"))
            {

				AudioManager.SetSnapshot(snapNames[snapIndex], snapChangeTime);
            }

            if (GUILayout.Button("IsMannerMode"))
            {
                //USndPlugin.IsMannerMode();
                //USndPlugin.UpdateAndroidMusicStatus();
                Debug.Log("IsMannerMode:" + USndPlugin.IsMannerMode());
            }
        }

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            msg = "Header Save Path:";
            GUILayout.Label(msg);


            savePath = GUILayout.TextField(@savePath, 180);


            if (GUILayout.Button("Create Header"))
            {
                OutputDefine();
            }
        }

        if (GUILayout.Button("ClearAudioPool"))
        {
            AudioManager.ClearObjectPool();
        }

        GUILayout.EndVertical();

        /////////////////////////////////////////////
        // チェック項目
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Preload Load"))
        {
            AudioManager.LoadAudioDataLoadId(0);
        }
        if (GUILayout.Button("Preload Reset"))
        {
            AudioManager.UnloadAudioDataLoadId(0);
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Move Volume Test"))
        {
            if ( zeroVolume == false )
            {
                AudioManager.SetVolume(instanceId, 0, 1);
                zeroVolume = true;
            }
            else
            {
                AudioManager.SetVolume(instanceId, 1, 1);
                zeroVolume = false;
            }
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Ducking ON"))
        {
            AudioManager.SetDucking("BGM", 0.3f, 1f);
        }
        if (GUILayout.Button("Ducking OFF"))
        {
            AudioManager.ResetDuckingAll(1);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Mute ON"))
        {
            AudioManager.SetMute(true);
        }
        if (GUILayout.Button("Mute OFF"))
        {
            AudioManager.SetMute(false);
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Position"))
        {
            AudioManager.ResetPlayPositionAll();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Snapshot ON"))
        {
            AudioManager.SetSnapshot(snapNames[1], 1);
        }
        if (GUILayout.Button("Snapshot OFF"))
        {
            AudioManager.SetSnapshot(snapNames[0], 1);
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Update XML"))
        {
            //LabelSettings2.xml
            TextAsset ta;
            Stream xml;
            ta = Resources.Load("XML/LabelSettings2") as TextAsset;
            if (ta != null)
            {
                xml = new MemoryStream(ta.bytes);
                AudioManager.LoadLabelXml(0, xml);
            }
        }

        if (GUILayout.Button("Restart"))
        {
            //AudioManager.RemoveAll();
            AudioManager.Terminate();
            AudioManager.Initialize();
            status = SELECT_XML;
            loadStep = 0;
            subStep = 0;
            deleteInfo();
            init = false;
        }

        if (GUILayout.Button("UnloadUnusedAssets", GUILayout.Height(200)))
        {
            Resources.UnloadUnusedAssets();
            Debug.Log("UnloadUnusedAssets exec.");
        }
        
        if (GUILayout.Button("Aging ON/OFF"))
        {
            aging = !aging;
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    void OnApplicationPause(bool pause)
    {
        Debug.Log("!!!!!!!!! onApplication pause: " + pause);
    }

    void OnApplicationFocus(bool focus)
    {
        Debug.Log("!!!!!!!!! OnApplicationFocus focus: " + focus);

        Debug.Log("IsMusicPlaying:" + USndPlugin.IsMusicPlaying());
    }

    void OutputDefine()
    {
        try
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(savePath, false, System.Text.Encoding.UTF8);
            // ヘッダー保存
            sw.WriteLine("// USndDefine generate:" + DateTime.Now.ToString());
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("namespace USnd {");
            sw.WriteLine("  public class USndDefine {");
            // Master
            sw.WriteLine("");
            // ---------------------------------------
            if (masterNames != null)
            {
                sw.WriteLine("      public enum Master {");
                for (int i = 0; i < masterNames.Length; ++i)
                {
                    sw.WriteLine("          " + masterNames[i] + " = " + i + ",");
                }
                sw.WriteLine("");
                sw.WriteLine("          Master_Max = " + masterNames.Length + ",");
                sw.WriteLine("      }");  // enum Master
            }
            // ---------------------------------------
            sw.WriteLine("");
            if (masterNames != null)
            {
                sw.WriteLine("      static public readonly string[] MasterName = new string[] {");
                for (int i = 0; i < masterNames.Length; ++i)
                {
                    sw.WriteLine("          \"" + masterNames[i] + "\",");
                }
                sw.WriteLine("      };");  // string Master
            }
            // ---------------------------------------

            // Category
            sw.WriteLine("");
            // ---------------------------------------
            sw.WriteLine("      public enum Category {");
            if (categoryNames != null)
            {
                for (int i = 0; i < categoryNames.Length; ++i)
                {
                    sw.WriteLine("          " + categoryNames[i] + " = " + i + ",");
                }
                sw.WriteLine("");
                sw.WriteLine("          Category_Max = " + categoryNames.Length + ",");
                sw.WriteLine("      }");  // enum Category
            }
            // ---------------------------------------
            sw.WriteLine("");
            if (categoryNames != null)
            {
                sw.WriteLine("      static public readonly string[] CategoryName = new string[] {");
                for (int i = 0; i < categoryNames.Length; ++i)
                {
                    sw.WriteLine("          \"" + categoryNames[i] + "\",");
                }
                sw.WriteLine("      };");  // string Category
            }
            // ---------------------------------------


            // Label
            sw.WriteLine("");
            // ---------------------------------------
            if (labelNames != null)
            {
                sw.WriteLine("      public enum Label {");
                for (int i = 0; i < labelNames.Length; ++i)
                {
                    sw.WriteLine("          " + labelNames[i] + " = " + i + ",");
                }
                sw.WriteLine("");
                sw.WriteLine("          Label_Max = " + labelNames.Length + ",");
                sw.WriteLine("      }");  // enum Label
            }
            // ---------------------------------------
            sw.WriteLine("");
            if (labelNames != null)
            {
                sw.WriteLine("      static public readonly string[] LabelName = new string[] {");
                for (int i = 0; i < labelNames.Length; ++i)
                {
                    sw.WriteLine("          \"" + labelNames[i] + "\",");
                }
                sw.WriteLine("      };");  // string Label
            }
            // ---------------------------------------


            //Snapsot
            sw.WriteLine("");
            // ---------------------------------------
            if (snapNames != null)
            {
                sw.WriteLine("      public enum Snapshot {");
                for (int i = 0; i < snapNames.Length; ++i)
                {
                    sw.WriteLine("          " + snapNames[i] + " = " + i + ",");
                }
                sw.WriteLine("");
                sw.WriteLine("          Snapshot_Max = " + snapNames.Length + ",");
                sw.WriteLine("      }");  // enum Snapshot
            }
            // ---------------------------------------
            sw.WriteLine("");
            if (snapNames != null)
            {
                sw.WriteLine("      static public readonly string[] SnapshotName = new string[] {");
                for (int i = 0; i < snapNames.Length; ++i)
                {
                    sw.WriteLine("          \"" + snapNames[i] + "\",");
                }
                sw.WriteLine("      };");  // string Snapshot
            }
            // ---------------------------------------


            // AudioClip
            AudioClip[] clip = Resources.LoadAll<AudioClip>("AudioClip");
            sw.WriteLine("");
            // ---------------------------------------
            if (clip != null)
            {
                sw.WriteLine("      public enum AudioClip {");
                for (int i = 0; i < clip.Length; ++i)
                {
                    sw.WriteLine("          " + clip[i].name + " = " + i + ",");
                }
                sw.WriteLine("      }");  // enum AudioClip
            }
            // ---------------------------------------
            sw.WriteLine("");
            if (clip != null)
            {
                sw.WriteLine("      static public readonly string[] AudioClipName = new string[] {");
                for (int i = 0; i < clip.Length; ++i)
                {
                    sw.WriteLine("          \"" + clip[i].name + "\",");
                }
                sw.WriteLine("      };");  // string AudioClip
            }
            // ---------------------------------------

            sw.WriteLine("  }");    // class USndDefine
            sw.WriteLine("}");  // namespace USnd
            sw.Close();
            Debug.Log("ヘッダー生成完了！");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log("ヘッダー生成に失敗しました。");
        }
    }


}
