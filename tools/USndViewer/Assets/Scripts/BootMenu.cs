using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.Audio;

using USnd;

public class BootMenu : MonoBehaviour {

    public string AddPath = "";

    private Toggle baseUI;

    private Toggle loadTableType;
    private Toggle loadTableTypeJson;

    private List<Toggle> toggleList = new List<Toggle>();
    private string[] labelListName = null;
    private string[] labelListNameXML = null;
    private string[] labelListNameJson = null;
    private string[] labelListNameBinary = null;

    private GameObject maskPanel;
    private Text loadingInfo;

	private GameObject errorMsg;
	private Text messageText;
	private Button closeMessage;

    private Dropdown selectFolder;
    private List<string> folderList = new List<string>();
    private int selectFolderIndex = 0;

    private Dropdown defaultRateSelect;
    private int defaultRateIndex = 0;
    List<string> rate = new List<string> { "44100", "48000", "32000", "24000" };


    string NAMELIST_PATH = "NameList/";
    string DIRECTORY_TABLE_LIST = "NameList/Directory";

    string MASTER_TABLE_LIST_NAME = "MasterName";
    string CATEGORY_TABLE_LIST_NAME = "CategoryName";
    string LABEL_TABLE_LIST_NAME = "LabelName";

    string MASTER_TABLE_LIST_BINARY_NAME = "MasterNameBinary";
    string CATEGORY_TABLE_LIST_BINARY_NAME = "CategoryNameBinary";
    string LABEL_TABLE_LIST_BINARY_NAME = "LabelNameBinary";

    string MASTER_TABLE_LIST_JSON_NAME = "MasterNameJson";
    string CATEGORY_TABLE_LIST_JSON_NAME = "CategoryNameJson";
    string LABEL_TABLE_LIST_JSON_NAME = "LabelNameJson";

    public AudioMixer mixer = null;

    public List<AudioMixerSnapshot> snapshot = null;


    // Editorのロード先をテストでアセットバンドルにするときはtrue
    private bool TEST_LOAD_ASSETBUNDLE = false;
    private bool isLoadAssetBundle = false;


    private TextAsset masterList;
    private TextAsset categoryList;
    private TextAsset labelList;

    private TextAsset masterListBinary;
    private TextAsset categoryListBinary;
    private TextAsset labelListBinary;

    private TextAsset masterListJson;
    private TextAsset categoryListJson;
    private TextAsset labelListJson;


    private string assetDir;
    private string assetPath;

    private string orgAssetDir;
    private string orgAssetPath;

    private string AB_NAME_LIST = "/namelist.usnd";
    private string AB_BINARY = "/binary.usnd";
    private string AB_JSON = "/json.usnd";
    private string AB_XML = "/xml.usnd";
    private string AB_MIXER = "/mixer.usnd";
    private string AB_SOUND_EXT = ".audio";

    private List<AssetBundle> soundABList = new List<AssetBundle>();

	private AudioMixer loadedMixer = null;

	// Use this for initialization
	void Start () {

        Caching.maximumAvailableDiskSpace = 1024 * 1024 * 1024;

#if UNITY_EDITOR
        assetDir = Application.streamingAssetsPath;
        assetPath = "file://" + Application.streamingAssetsPath;
#elif UNITY_ANDROID
        assetDir = "/sdcard/usndviewer";
        assetPath =  "file:///sdcard/usndviewer";
#elif UNITY_IOS
        assetDir = Application.persistentDataPath;
        assetPath = "file://" + Application.persistentDataPath;
#endif
        orgAssetDir = assetDir;
        orgAssetPath = assetPath;


#if UNITY_EDITOR
        isLoadAssetBundle = TEST_LOAD_ASSETBUNDLE;
#else
        isLoadAssetBundle = true;
#endif

        /*
        AudioManager.Initialize();
#if UNITY_EDITOR
        if (mixer != null)
        {
            if (!isLoadAssetBundle)
            {
                AudioManager.SetAudioMixer(mixer);
            }
        }
#endif
        */
        GameObject tmp = GameObject.Find("StartButton");
        Button startButton = tmp.GetComponent<Button>();
        startButton.onClick.AddListener(OnClickStart);

        tmp = GameObject.Find("SelectSwitch");
        Button selectSwitch = tmp.GetComponent<Button>();
        selectSwitch.onClick.AddListener(OnClickSelectOnOff);

        tmp = GameObject.Find("BaseToggle");
        baseUI = tmp.GetComponent<Toggle>();

        tmp = GameObject.Find("LoadTableTypeToggle");
        loadTableType = tmp.GetComponent<Toggle>();
        loadTableType.onValueChanged.AddListener(OnCheckValueChange);

        tmp = GameObject.Find("LoadTableTypeToggle02");
        loadTableTypeJson = tmp.GetComponent<Toggle>();
        loadTableTypeJson.onValueChanged.AddListener(OnCheckValueChangeJson);

        maskPanel = GameObject.Find("MaskPanel");

        tmp = GameObject.Find("LoadingProgressText");
        loadingInfo = tmp.GetComponent<Text>();

        tmp = GameObject.Find("Dropdown");
        selectFolder = tmp.GetComponent<Dropdown>();

        tmp = GameObject.Find("DefaultRateSelect");
        defaultRateSelect = tmp.GetComponent<Dropdown>();

        tmp = GameObject.Find("CleanCache");
        Button cleanButton = tmp.GetComponent<Button>();
        cleanButton.onClick.AddListener(OnClickCleanCache);

		//	private GameObject errorMsg;
		//	private Text messageText;
		errorMsg = GameObject.Find("ErrorMessage");


		tmp = GameObject.Find("MessageText");
		messageText = tmp.GetComponent<Text>();

		tmp = GameObject.Find("CloseButton");
		closeMessage = tmp.GetComponent<Button>();
		closeMessage.onClick.AddListener(OnClickMessgeClose);

		errorMsg.SetActive(false);


		tmp = GameObject.Find("UnityVersionText");
		Text tmpText = tmp.GetComponent<Text>();
		tmpText.text = "Build Unity " + Application.unityVersion;

#if UNITY_EDITOR
		if (!isLoadAssetBundle)
        {
            selectFolder.gameObject.SetActive(false);
        }
#endif
        if ( Application.platform != RuntimePlatform.WindowsEditor ||
            (Application.platform == RuntimePlatform.WindowsEditor && isLoadAssetBundle ))
        {
            selectFolder.ClearOptions();
            // データフォルダの中のフォルダ一覧を取得
            string[] dir = Directory.GetDirectories(assetDir);
            for(int i=0; i<dir.Length; ++i)
            {
                folderList.Add(Path.GetFileName(dir[i]));
            }
            selectFolder.AddOptions(folderList);
            selectFolder.value = selectFolderIndex;
            assetPath = orgAssetPath + "/" + folderList[selectFolderIndex];
            assetDir = orgAssetDir + "/" + folderList[selectFolderIndex];
        }

        defaultRateSelect.ClearOptions();
        defaultRateSelect.AddOptions(rate);
        defaultRateIndex = 0;
        defaultRateSelect.value = defaultRateIndex;


        maskPanel.SetActive(false);


        loadTableInfo();

        USndViewer.backButtonMethod = () =>
        {
            AudioManager.RemoveAll();
            SceneManager.LoadScene("Boot");
            Debug.Log("delegate BootMenu");

            for(int i=0; i<soundABList.Count; ++i)
            {
                soundABList[i].Unload(false);
            }
            soundABList.Clear();
        };
	}
	
	void OnClickMessgeClose()
	{
		errorMsg.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
	
	}

    void loadTableInfo()
    {
        if (isLoadAssetBundle)
        {
            // NameListを読み込む
            StartCoroutine(loadNameList());
        }
        else
        {
            labelListNameXML = loadText(AddPath + NAMELIST_PATH + LABEL_TABLE_LIST_NAME);
            labelListNameJson = loadText(AddPath + NAMELIST_PATH + LABEL_TABLE_LIST_JSON_NAME);
            labelListNameBinary = loadText(AddPath + NAMELIST_PATH + LABEL_TABLE_LIST_BINARY_NAME);
            labelListName = labelListNameXML;
            setLoadLabelList();
        }
    }

    IEnumerator loadNameList()
    {
		Debug.Log("Load Name List Start.");
        long version = File.GetLastWriteTime(assetDir + AB_NAME_LIST).ToFileTime();
		Debug.Log(" --- GetLastWriteTime:" + version);
		using (WWW loader = WWW.LoadFromCacheOrDownload(assetPath + AB_NAME_LIST, (int)version))
        {
			Debug.Log(" --- load start:" + assetPath + AB_NAME_LIST);
			yield return loader;
            if (!string.IsNullOrEmpty(loader.error))
			{
				errorMsg.SetActive(true);
				messageText.text = "WWW download had an error: " + loader.error + " " + AB_NAME_LIST;
				yield break;
				//throw new Exception("WWW download had an error:" + loader.error);
            }
            AssetBundle bundle = loader.assetBundle;
			Debug.Log(" --- assetBundle Load");

			if ( bundle == null )
			{
				errorMsg.SetActive(true);
				messageText.text = AB_NAME_LIST + "の読み込みに失敗しました。\nアセットバンドルの内容が正しいか確認してください。\nもしくは、USndViewerのUnityバージョンとアセットバンドルを作成したUnityバージョンを確認してください。\n\nBuild Unity " + Application.unityVersion;
				yield break;
			}

			masterList = bundle.LoadAsset<TextAsset>(MASTER_TABLE_LIST_NAME);
            categoryList = bundle.LoadAsset<TextAsset>(CATEGORY_TABLE_LIST_NAME);
            labelList = bundle.LoadAsset<TextAsset>(LABEL_TABLE_LIST_NAME);

            masterListBinary = bundle.LoadAsset<TextAsset>(MASTER_TABLE_LIST_BINARY_NAME);
            categoryListBinary = bundle.LoadAsset<TextAsset>(CATEGORY_TABLE_LIST_BINARY_NAME);
            labelListBinary = bundle.LoadAsset<TextAsset>(LABEL_TABLE_LIST_BINARY_NAME);

            masterListJson = bundle.LoadAsset<TextAsset>(MASTER_TABLE_LIST_JSON_NAME);
            categoryListJson = bundle.LoadAsset<TextAsset>(CATEGORY_TABLE_LIST_JSON_NAME);
            labelListJson = bundle.LoadAsset<TextAsset>(LABEL_TABLE_LIST_JSON_NAME);


			Debug.Log(" --- assetBundle Load Finish");
			bundle.Unload(false);

			Debug.Log(" --- load Text");
			labelListName = loadText(labelList);
            labelListNameXML = labelListName;
            labelListNameBinary = loadText(labelListBinary);
            labelListNameJson = loadText(labelListJson);
			Debug.Log(" --- setLoadLabelList");
			setLoadLabelList();
        }


        if (File.Exists(assetDir + AB_MIXER))
        {
            version = File.GetLastWriteTime(assetDir + AB_MIXER).ToFileTime();
            using (WWW loader = WWW.LoadFromCacheOrDownload(assetPath + AB_MIXER, (int)version))
            {
                yield return loader;
                if (loader.error != null)
                {
                    throw new Exception("WWW download had an error:" + loader.error);
                }
                AssetBundle bundle = loader.assetBundle;

                AudioMixer[] tmpMix = bundle.LoadAllAssets<AudioMixer>();
                if (tmpMix != null && tmpMix.Length > 0)
                {
					// Initialize後じゃないと設定できないので保存だけする
					//AudioManager.SetAudioMixer(tmpMix[0]);
					loadedMixer = tmpMix[0];
				}
                AudioMixerSnapshot[] tmpSnap = bundle.LoadAllAssets<AudioMixerSnapshot>();
                if (tmpSnap != null)
                {
                    snapshot.Clear();
                    List<string> snapList = new List<string>();
                    for (int i = 0; i < tmpSnap.Length; ++i)
                    {
                        snapshot.Add(tmpSnap[i]);
                        snapList.Add(tmpSnap[i].name);
                    }
                    USndViewer.SetSnapInfo(snapList.ToArray());
                }
                bundle.Unload(false);
            }
        }
    }

    void setLoadLabelList()
    {
        // Intantiateしているのを取らないと増えていくのでリストで管理
        for (int i = 0; i < toggleList.Count; ++i )
        {
            if ( i==0 )
            {
                // baseUIは外さない
            }
            else
            {
                toggleList[i].gameObject.transform.SetParent(null);
                Destroy(toggleList[i]);
            }
        }

        toggleList.Clear();

        if (labelListName != null)
        {
            for (int i = 0; i < labelListName.Length; ++i)
            {
                if (labelListName[i] != "")
                {
                    Toggle tmp;
                    if (i == 0)
                    {
                        tmp = baseUI;
                    }
                    else
                    {
                        tmp = Instantiate(baseUI);
                        tmp.gameObject.transform.SetParent(baseUI.gameObject.transform.parent);
                    }

                    Transform transform = tmp.gameObject.transform.Find("Label");
                    Text label = transform.GetComponent<Text>();
                    label.text = labelListName[i];

                    RectTransform rect = tmp.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1, 1, 1);
                    toggleList.Add(tmp);
                    tmp.isOn = true;
                }
            }
        }
    }

    string[] loadText(TextAsset text)
    {
        if (text != null)
        {
            string tmp = text.text.Replace("\r\n", "\r");
            return tmp.Split('\r');
        }
        return null;
    }

    string[] loadText(string name)
    {
        TextAsset text = Resources.Load(name) as TextAsset;
        if ( text != null )
        {
            string tmp = text.text.Replace("\r\n", "\r");
            return tmp.Split('\r');
        }
        return null;
    }

    public void OnCheckValueChange(bool flag)
    {
        if (flag == true)
        {
            loadTableTypeJson.isOn = false;
            labelListName = labelListNameBinary;
            setLoadLabelList();
        }
        else if (flag == false)
        {
            if (loadTableTypeJson.isOn == false)
            {
                labelListName = labelListNameXML;
                setLoadLabelList();
            }
        }
    }

    public void OnCheckValueChangeJson(bool flag)
    {
        if (flag == true)
        {
            loadTableType.isOn = false;
            labelListName = labelListNameJson;
            setLoadLabelList();
        }
        else if (flag == false)
        {
            if (loadTableType.isOn == false )
            {
                labelListName = labelListNameXML;
                setLoadLabelList();
            }
        }
    }

    public void OnClickSelectOnOff()
    {
        bool flag = baseUI.isOn ? false : true;

        for(int i=0; i<toggleList.Count; ++i)
        {
            toggleList[i].isOn = flag;
        }

    }

    public void OnClickStart()
    {
        Debug.Log("OnClickStart");
        maskPanel.SetActive(true);

        int setRate = 0;
        int.TryParse(rate[defaultRateSelect.value], out setRate);
        AudioManager.Initialize(setRate);
#if UNITY_EDITOR
        if (mixer != null)
        {
            if (!isLoadAssetBundle)
            {
                AudioManager.SetAudioMixer(mixer);
            }
        }
#endif
		if (loadedMixer != null)
		{
			AudioManager.SetAudioMixer(loadedMixer);
		}

		if ( loadTableType.isOn )
        {
            if ( isLoadAssetBundle )
            {
                // AssetBundleから読み込みの場合
                StartCoroutine(loadBinaryFromAB());
            }
            else
            {
                // trueならbinary
                LoadBinary();
            }
        }
        else if ( loadTableTypeJson.isOn )
        {
            if (isLoadAssetBundle)
            {
                // AssetBundleから読み込みの場合
                StartCoroutine(loadJsonFromAB());
            }
            else
            {
                // trueならbinary
                StartCoroutine(LoadJson());
            }
        }
        else
        {
            if (isLoadAssetBundle)
            {
                // AssetBundleから読み込みの場合
                StartCoroutine(loadXMLFromAB());
            }
            else
            {
                // falseならxml
                StartCoroutine(LoadXML());
            }
        }

        //SceneManager.LoadScene("USndViewer");
    }

    public void OnClickCleanCache()
    {
#if UNITY_2017
        Caching.ClearCache();
#else
        Caching.ClearCache();
#endif
    }

    public void OnValueChanged(int value)
    {
        selectFolderIndex = value;

        assetPath = orgAssetPath + "/" + folderList[selectFolderIndex];
        assetDir = orgAssetDir + "/" + folderList[selectFolderIndex];

        loadTableInfo();

        Debug.Log("select dropdown: " + value);
    }

    IEnumerator loadBinaryFromAB()
    {
        // binary.usndから各バイナリテーブルを読み込む
        // 読み込み済みのnamelist.usndに記載されているファイルを読み込む
        long version = File.GetLastWriteTime(assetDir + AB_BINARY).ToFileTime();
        using (WWW loader = WWW.LoadFromCacheOrDownload(assetPath + AB_BINARY, (int)version))
        {
            yield return loader;
            if (loader.error != null)
            {
				errorMsg.SetActive(true);
				messageText.text = "WWW download had an error: " + loader.error + " " + AB_BINARY;
				yield break;
				//throw new Exception("WWW download had an error:" + loader.error);
			}
            AssetBundle bundle = loader.assetBundle;

			if (bundle == null)
			{
				errorMsg.SetActive(true);
				messageText.text = AB_BINARY + "の読み込みに失敗しました。\nアセットバンドルの内容が正しいか確認してください。\nもしくは、USndViewerのUnityバージョンとアセットバンドルを作成したUnityバージョンを確認してください。\n\nBuild Unity " + Application.unityVersion;
				yield break;
			}

			TextAsset ta;
            // master
            string[] list = loadText(masterListBinary);
            for (int i = 0; i < list.Length; ++i)
            {
                loadingInfo.text = "Loading Master(" + i + "/" + list.Length + ")";

                if (list[i] != "")
                {
                    ta = bundle.LoadAsset<TextAsset>(list[i]);
                    if (ta != null)
                    {
                        AudioManager.LoadBinaryTable(ta.bytes);
                    }
                }
            }

            // category
            list = loadText(categoryListBinary);
            for (int i = 0; i < list.Length; ++i)
            {
                loadingInfo.text = "Loading Category(" + i + "/" + list.Length + ")";

                if (list[i] != "")
                {
                    ta = bundle.LoadAsset<TextAsset>(list[i]);
                    if (ta != null)
                    {
                        AudioManager.LoadBinaryTable(ta.bytes);
                    }
                }
            }

            // label
            for (int i = 0; i < labelListName.Length; ++i)
            {
                loadingInfo.text = "Loading Label(" + i + "/" + labelListName.Length + ")";

                if (labelListName[i] != "" && toggleList[i].isOn == true)
                {
                    ta = bundle.LoadAsset<TextAsset>(labelListName[i]);
                    if (ta != null)
                    {
                        AudioManager.LoadBinaryTable(ta.bytes);
                    }
                }
            }

            bundle.Unload(false);
            
            // loadClip
            StartCoroutine(loadClipFromAB());
        }
    }

    IEnumerator loadJsonFromAB()
    {
        // json.usndから各バイナリテーブルを読み込む
        // 読み込み済みのnamelist.usndに記載されているファイルを読み込む
        long version = File.GetLastWriteTime(assetDir + AB_JSON).ToFileTime();
        using (WWW loader = WWW.LoadFromCacheOrDownload(assetPath + AB_JSON, (int)version))
        {
            yield return loader;
            if (loader.error != null)
            {
				errorMsg.SetActive(true);
				messageText.text = "WWW download had an error: " + loader.error  + " " + AB_JSON;
				yield break;
				//throw new Exception("WWW download had an error:" + loader.error);
			}
            AssetBundle bundle = loader.assetBundle;
			if (bundle == null)
			{
				errorMsg.SetActive(true);
				messageText.text = AB_JSON + "の読み込みに失敗しました。\nアセットバンドルの内容が正しいか確認してください。\nもしくは、USndViewerのUnityバージョンとアセットバンドルを作成したUnityバージョンを確認してください。\n\nBuild Unity " + Application.unityVersion;
				yield break;
			}

			TextAsset ta;
            // master
            string[] list = loadText(masterListJson);
            for (int i = 0; i < list.Length; ++i)
            {
                loadingInfo.text = "Loading Master(" + i + "/" + list.Length + ")";

                if (list[i] != "")
                {
                    ta = bundle.LoadAsset<TextAsset>(list[i]);
                    if (ta != null)
                    {
                        AudioManager.LoadJson(ta.text);

                        while (AudioManager.GetLoadJsonStatus() == AudioDefine.LOAD_JSON_STATUS.LOADING)
                        {
                            yield return null;
                        }
                    }
                }
            }

            // category
            list = loadText(categoryListJson);
            for (int i = 0; i < list.Length; ++i)
            {
                loadingInfo.text = "Loading Category(" + i + "/" + list.Length + ")";

                if (list[i] != "")
                {
                    ta = bundle.LoadAsset<TextAsset>(list[i]);
                    if (ta != null)
                    {
                        AudioManager.LoadJson(ta.text);

                        while (AudioManager.GetLoadJsonStatus() == AudioDefine.LOAD_JSON_STATUS.LOADING)
                        {
                            yield return null;
                        }
                    }
                }
            }

            // label
            for (int i = 0; i < labelListName.Length; ++i)
            {
                loadingInfo.text = "Loading Label(" + i + "/" + labelListName.Length + ")";

                if (labelListName[i] != "" && toggleList[i].isOn == true)
                {
                    ta = bundle.LoadAsset<TextAsset>(labelListName[i]);
                    if (ta != null)
                    {
                        AudioManager.LoadJson(ta.text);

                        while (AudioManager.GetLoadJsonStatus() == AudioDefine.LOAD_JSON_STATUS.LOADING)
                        {
                            yield return null;
                        }
                    }
                }
            }

            bundle.Unload(false);

            // loadClip
            StartCoroutine(loadClipFromAB());
        }
    }

    IEnumerator loadXMLFromAB()
    {
        // xml.usndから各XMLテーブルを読み込む
        // 読み込み済みのnamelist.usndに記載されているファイルを読み込む
        long version = File.GetLastWriteTime(assetDir + AB_XML).ToFileTime();
        using (WWW loader = WWW.LoadFromCacheOrDownload(assetPath + AB_XML, (int)version))
        {
            yield return loader;
            if (loader.error != null)
            {
				errorMsg.SetActive(true);
				messageText.text = "WWW download had an error: " + loader.error + " " + AB_XML;
				yield break;
				//throw new Exception("WWW download had an error:" + loader.error);
			}
            AssetBundle bundle = loader.assetBundle;

			if (bundle == null)
			{
				errorMsg.SetActive(true);
				messageText.text = AB_XML + "の読み込みに失敗しました。\nアセットバンドルの内容が正しいか確認してください。\nもしくは、USndViewerのUnityバージョンとアセットバンドルを作成したUnityバージョンを確認してください。\n\nBuild Unity " + Application.unityVersion;
				yield break;
			}


			loadingInfo.text = "Loading Master";

            TextAsset ta;
            Stream xml;
            // master
            string[] list = loadText(masterList);
            for (int i = 0; i < list.Length; ++i)
            {
                loadingInfo.text = "Loading Master(" + i + "/" + list.Length + ")";

                if (list[i] != "")
                {
                    ta = bundle.LoadAsset<TextAsset>(list[i]);
                    if (ta != null)
                    {
                        xml = new MemoryStream(ta.bytes);
                        AudioManager.LoadMasterXml(xml);

                        while (AudioManager.GetLoadXmlStatus() == AudioDefine.LOAD_XML_STATUS.LOADING)
                        {
                            yield return null;
                        }
                    }
                }
            }

            // category
            list = loadText(categoryList);
            for (int i = 0; i < list.Length; ++i)
            {
                loadingInfo.text = "Loading Category(" + i + "/" + list.Length + ")";

                if (list[i] != "")
                {
                    ta = bundle.LoadAsset<TextAsset>(list[i]);
                    if (ta != null)
                    {
                        xml = new MemoryStream(ta.bytes);
                        AudioManager.LoadCategoryXml(xml);

                        while (AudioManager.GetLoadXmlStatus() == AudioDefine.LOAD_XML_STATUS.LOADING)
                        {
                            yield return null;
                        }
                    }
                }
            }

            // label
            for (int i = 0; i < labelListName.Length; ++i)
            {
                loadingInfo.text = "Loading Label(" + i + "/" + labelListName.Length + ")";

                if (labelListName[i] != "" && toggleList[i].isOn == true)
                {
                    ta = bundle.LoadAsset<TextAsset>(labelListName[i]);
                    if (ta != null)
                    {
                        xml = new MemoryStream(ta.bytes);
                        AudioManager.LoadLabelXml(0, xml);

                        while (AudioManager.GetLoadXmlStatus() == AudioDefine.LOAD_XML_STATUS.LOADING)
                        {
                            yield return null;
                        }
                    }
                }
            }
            bundle.Unload(false);

            // loadClip
            StartCoroutine(loadClipFromAB());
        }
    }

    void LoadBinary()
    {
        TextAsset ta;
        // master
        string[] list = loadText(AddPath + NAMELIST_PATH + MASTER_TABLE_LIST_BINARY_NAME);
        for (int i = 0; i < list.Length; ++i)
        {
            loadingInfo.text = "Loading Master(" + i + "/" + list.Length + ")";

            if (list[i] != "")
            {
                ta = Resources.Load(AddPath + "MasterBinary/" + list[i]) as TextAsset;
                if (ta != null)
                {
                    AudioManager.LoadBinaryTable(ta.bytes);
                }
            }
        }

        // category
        list = loadText(AddPath + NAMELIST_PATH + CATEGORY_TABLE_LIST_BINARY_NAME);
        for (int i = 0; i < list.Length; ++i)
        {
            loadingInfo.text = "Loading Category(" + i + "/" + list.Length + ")";

            if (list[i] != "")
            {
                ta = Resources.Load(AddPath + "CategoryBinary/" + list[i]) as TextAsset;
                if (ta != null)
                {
                    AudioManager.LoadBinaryTable(ta.bytes);
                }
            }
        }

        // label
        for (int i = 0; i < labelListName.Length; ++i)
        {
            loadingInfo.text = "Loading Label(" + i + "/" + labelListName.Length + ")";

            if (labelListName[i] != "" && toggleList[i].isOn == true)
            {
                ta = Resources.Load(AddPath + "LabelBinary/" + labelListName[i]) as TextAsset;
                if (ta != null)
                {
                    AudioManager.LoadBinaryTable(ta.bytes);
                }
            }
        }

        // loadClip
        StartCoroutine(loadClip());
    }

    IEnumerator LoadJson()
    {
        // 遅いといやなので非同期にしたい
        // JsonUtilityはスレッドから呼び出し可能
        // Jsonのパース処理以外はメインでやる
        // XMLと同じようにステータスでOKか返す


        TextAsset ta;
        // master
        string[] list = loadText(AddPath + NAMELIST_PATH + MASTER_TABLE_LIST_JSON_NAME);
        for (int i = 0; i < list.Length; ++i)
        {
            loadingInfo.text = "Loading Master(" + i + "/" + list.Length + ")";

            if (list[i] != "")
            {
                ta = Resources.Load(AddPath + "MasterJson/" + list[i]) as TextAsset;
                if (ta != null)
                {
                    AudioManager.LoadJson(ta.text);

                    while (AudioManager.GetLoadJsonStatus() == AudioDefine.LOAD_JSON_STATUS.LOADING)
                    {
                        yield return null;
                    }
                }
            }
        }

        // category
        list = loadText(AddPath + NAMELIST_PATH + CATEGORY_TABLE_LIST_JSON_NAME);
        for (int i = 0; i < list.Length; ++i)
        {
            loadingInfo.text = "Loading Category(" + i + "/" + list.Length + ")";

            if (list[i] != "")
            {
                ta = Resources.Load(AddPath + "CategoryJson/" + list[i]) as TextAsset;
                if (ta != null)
                {
                    AudioManager.LoadJson(ta.text);

                    while (AudioManager.GetLoadJsonStatus() == AudioDefine.LOAD_JSON_STATUS.LOADING)
                    {
                        yield return null;
                    }
                }
            }
        }

        // label
        for (int i = 0; i < labelListName.Length; ++i)
        {
            loadingInfo.text = "Loading Label(" + i + "/" + labelListName.Length + ")";

            if (labelListName[i] != "" && toggleList[i].isOn == true)
            {
                ta = Resources.Load(AddPath + "LabelJson/" + labelListName[i]) as TextAsset;
                if (ta != null)
                {
                    AudioManager.LoadJson(ta.text);

                    while (AudioManager.GetLoadJsonStatus() == AudioDefine.LOAD_JSON_STATUS.LOADING)
                    {
                        yield return null;
                    }
                }
            }
        }
        
        // loadClip
        StartCoroutine(loadClip());
    }

    IEnumerator LoadXML()
    {
        loadingInfo.text = "Loading Master";

        TextAsset ta;
        Stream xml;
        // master
        string[] list = loadText(AddPath + NAMELIST_PATH + MASTER_TABLE_LIST_NAME);
        for (int i = 0; i < list.Length; ++i)
        {
            loadingInfo.text = "Loading Master(" + i + "/" + list.Length + ")";

            if (list[i] != "")
            {
                ta = Resources.Load(AddPath + "MasterXML/" + list[i]) as TextAsset;
                if (ta != null)
                {
                    xml = new MemoryStream(ta.bytes);
                    AudioManager.LoadMasterXml(xml);

                    while(AudioManager.GetLoadXmlStatus() == AudioDefine.LOAD_XML_STATUS.LOADING)
                    {
                        yield return null;
                    }
                }
            }
        }

        // category
        list = loadText(AddPath + NAMELIST_PATH + CATEGORY_TABLE_LIST_NAME);
        for (int i = 0; i < list.Length; ++i)
        {
            loadingInfo.text = "Loading Category(" + i + "/" + list.Length + ")";

            if (list[i] != "")
            {
                ta = Resources.Load(AddPath + "CategoryXML/" + list[i]) as TextAsset;
                if (ta != null)
                {
                    xml = new MemoryStream(ta.bytes);
                    AudioManager.LoadCategoryXml(xml);

                    while (AudioManager.GetLoadXmlStatus() == AudioDefine.LOAD_XML_STATUS.LOADING)
                    {
                        yield return null;
                    }
                }
            }
        }

        // label
        for (int i = 0; i < labelListName.Length; ++i)
        {
            loadingInfo.text = "Loading Label(" + i + "/" + labelListName.Length + ")";
            
            if (labelListName[i] != "" && toggleList[i].isOn == true)
            {
                ta = Resources.Load(AddPath + "LabelXML/" + labelListName[i]) as TextAsset;
                if (ta != null)
                {
                    xml = new MemoryStream(ta.bytes);
                    AudioManager.LoadLabelXml(0, xml);

                    while (AudioManager.GetLoadXmlStatus() == AudioDefine.LOAD_XML_STATUS.LOADING)
                    {
                        yield return null;
                    }
                }
            }
        }

        // loadClip
        StartCoroutine(loadClip());

    }


    IEnumerator loadClip()
    {
        ResourceRequest req = null;
        string[] clipList = AudioManager.GetAudioClipNameAll();
        int pathIndex = 0;

        string[] clipPath = loadText(AddPath + DIRECTORY_TABLE_LIST);

        int currentLoad = 0;
        while (currentLoad < clipList.Length)
        {
            loadingInfo.text = "Loading AudioClip(" + currentLoad + "/" + clipList.Length + ")";
            if (req == null)
            {
                req = Resources.LoadAsync<AudioClip>(AddPath + "AudioClip/" + clipList[currentLoad]);
            }

            while (req.isDone == false)
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
                    if (clipPath != null)
                    {
                        if (pathIndex >= clipPath.Length)
                        {
                            ++currentLoad;
                            pathIndex = 0;
                            req = null;
                        }
                        else
                        {
                            req = Resources.LoadAsync<AudioClip>(AddPath + "AudioClip/" + clipPath[pathIndex] + "/" + clipList[currentLoad]);
                            ++pathIndex;
                        }
                    }
                    else
                    {
                        ++currentLoad;
                        pathIndex = 0;
                        req = null;
                    }
                }
            }
        }

        if (snapshot.Count != 0)
        {
            List<string> snapList = new List<string>();
            for (int i = 0; i < snapshot.Count; ++i)
            {
                if (snapshot[i] != null)
                {
                    snapList.Add(snapshot[i].name);
                }
            }
            USndViewer.SetSnapInfo(snapList.ToArray());
        }

        AudioManager.SetAudioClipToLabelAll();

        Debug.Log("finish laod resources.");
        SceneManager.LoadScene("USndViewer");
    }

    IEnumerator loadClipFromAB()
    {
        // 拡張子がaudioのものを読み込む
        string[] fileList = System.IO.Directory.GetFiles(assetDir);

        string[] clipList = AudioManager.GetAudioClipNameAll();
        bool[] clipLoad = new bool[clipList.Length];
        for (int i = 0; i < clipList.Length; ++i )
        {
            clipLoad[i] = false;
        }
        int currentLoad = 0;
        long version = 0;
        for (int i = 0; i < fileList.Length; ++i)
        {
            if (Path.GetExtension(fileList[i]).Equals(AB_SOUND_EXT))
            {
                version = File.GetLastWriteTime(assetDir + AB_XML).ToFileTime();
                using (WWW loader = WWW.LoadFromCacheOrDownload("file://" + fileList[i], (int)version))
                {
                    while (loader.isDone == false || loader.error != null)
                    {
                        loadingInfo.text = "Loading AudioClip(" + currentLoad + "/" + clipList.Length + ") " + (loader.progress*100) + "%";
                        yield return null;
                    }
                    //yield return loader;
                    if (loader.error != null)
                    {
						errorMsg.SetActive(true);
						messageText.text = "WWW download had an error: " + loader.error + " " + fileList[i];
						yield break;
						//throw new Exception("WWW download had an error:" + loader.error);
					}
                    AssetBundle bundle = loader.assetBundle;

					if (bundle == null)
					{
						errorMsg.SetActive(true);
						messageText.text = fileList[i] + "の読み込みに失敗しました。\nアセットバンドルの内容が正しいか確認してください。\nもしくは、USndViewerのUnityバージョンとアセットバンドルを作成したUnityバージョンを確認してください。\n\nBuild Unity " + Application.unityVersion;
						yield break;
					}

					soundABList.Add(bundle);

                    for (int j = 0; j < clipList.Length; ++j)
                    {
                        loadingInfo.text = "Loading AudioClip(" + currentLoad + "/" + clipList.Length + ")";
                        if (clipLoad[j] == false)
                        {
                            AssetBundleRequest abr = bundle.LoadAssetAsync<AudioClip>(clipList[j]);
                            /*while (abr.isDone == false)
                            {
                                loadingInfo.text = "Loading AudioClip(" + currentLoad + "/" + clipList.Length + ") " + abr.progress;
                            }*/
                            yield return abr;

                            if (abr.isDone)
                            {
                                AudioClip clip = abr.asset as AudioClip;
                                if (clip)
                                {
                                    AudioManager.AddAudioClip(clip);
                                    clipLoad[j] = true;
                                    ++currentLoad;
                                }
                            }
                        }
                    }
                }
            }
            if (currentLoad >= clipList.Length) break;
        }

        AudioManager.SetAudioClipToLabelAll();

        Debug.Log("finish laod ab.");
        SceneManager.LoadScene("USndViewer");
    }
}
