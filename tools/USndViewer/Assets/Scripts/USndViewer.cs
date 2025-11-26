using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

using USnd;

public class USndViewer : MonoBehaviour {

    public delegate void BackButtonDelegate();

    static public BackButtonDelegate backButtonMethod = null;

    Text pageInfo;
    Text masterNameInfo;
    Text categoryNameInfo;
    Text snapNameInfo;


    GameObject basePanel;
    GameObject mixerPanel;

    Text playButtonText0;
    Text playButtonText1;
    Text playButtonText2;
    Text playButtonText3;
    Text playButtonText4;


    Slider masterVolumeSlider;
    Slider categoryVolumeSlider;
    Slider snapMoveTimeSlider;

    string[] labelName = null;
    string[] masterName = null;
    string[] categoryName = null;
    static string[] snapName = null;

    static readonly int PLAYER_MAX = 5;

    int[] labelSelectIndex = new int[PLAYER_MAX];
    int masterSelectIndex = 0;
    int categorySelectIndex = 0;
    int snapSelectIndex = 0;

    int labelNum = 0;
    int masterNum = 0;
    int categoryNum = 0;
    static int snapNum = 0;

    int pageNum = 0;
    int currentPage = 0;

    List<Text> playButtonList = new List<Text>();

   public  static void SetSnapInfo(string[] name)
    {
        snapNum = name.Length;
        snapName = name;
    }

	// Use this for initialization
	void Start () {

        labelName = AudioManager.GetLabelNameList();
        masterName = AudioManager.GetMasterNameList();
        categoryName = AudioManager.GetCategoryNameList();

        labelNum = labelName.Length;
        masterNum = masterName.Length;
        categoryNum = categoryName.Length;

        for(int i=0; i<PLAYER_MAX; ++i)
        {
            labelSelectIndex[i] = i;
            if ( i >= labelNum )
            {
                labelSelectIndex[i] = -1;
            }
        }

        pageNum = (labelNum / PLAYER_MAX) + (((labelNum % PLAYER_MAX) == 0) ? 0 : 1);
        currentPage = 0;


        GameObject tmp = GameObject.Find("PageInfo");
        pageInfo = tmp.GetComponent<Text>();

        tmp = GameObject.Find("MasterName");
        masterNameInfo = tmp.GetComponent<Text>();

        tmp = GameObject.Find("CategoryName");
        categoryNameInfo = tmp.GetComponent<Text>();

        tmp = GameObject.Find("SnapName");
        snapNameInfo = tmp.GetComponent<Text>();

        // ------------------------------------------

        basePanel = GameObject.Find("BasePanel");
        mixerPanel = GameObject.Find("MixerPanel");

        // ------------------------------------------

        tmp = GameObject.Find("PlayText0");
        playButtonText0 = tmp.GetComponent<Text>();

        tmp = GameObject.Find("PlayText1");
        playButtonText1 = tmp.GetComponent<Text>();

        tmp = GameObject.Find("PlayText2");
        playButtonText2 = tmp.GetComponent<Text>();

        tmp = GameObject.Find("PlayText3");
        playButtonText3 = tmp.GetComponent<Text>();

        tmp = GameObject.Find("PlayText4");
        playButtonText4 = tmp.GetComponent<Text>();

        playButtonList.Add(playButtonText0);
        playButtonList.Add(playButtonText1);
        playButtonList.Add(playButtonText2);
        playButtonList.Add(playButtonText3);
        playButtonList.Add(playButtonText4);

        // ------------------------------------------

        tmp = GameObject.Find("PlayButton_0");
        Button tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPlay0);

        tmp = GameObject.Find("PlayButton_1");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPlay1);

        tmp = GameObject.Find("PlayButton_2");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPlay2);

        tmp = GameObject.Find("PlayButton_3");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPlay3);

        tmp = GameObject.Find("PlayButton_4");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPlay4);

        // ------------------------------------------

        tmp = GameObject.Find("StopButton_0");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickStop0);

        tmp = GameObject.Find("StopButton_1");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickStop1);

        tmp = GameObject.Find("StopButton_2");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickStop2);

        tmp = GameObject.Find("StopButton_3");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickStop3);

        tmp = GameObject.Find("StopButton_4");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickStop4);

        // ------------------------------------------

        tmp = GameObject.Find("PrevButton_1");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPagePrev1);

        tmp = GameObject.Find("PrevButton_10");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPagePrev10);

        tmp = GameObject.Find("PrevButton_100");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPagePrev100);

        // ------------------------------------------

        tmp = GameObject.Find("NextButton_1");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPageNext1);

        tmp = GameObject.Find("NextButton_10");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPageNext10);

        tmp = GameObject.Find("NextButton_100");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickPageNext100);

        // ------------------------------------------

        tmp = GameObject.Find("MasterSelectButton");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickMasterSelect);

        tmp = GameObject.Find("MasterVolumeSlider");
        masterVolumeSlider = tmp.GetComponent<Slider>();
        masterVolumeSlider.onValueChanged.AddListener(OnValueChangeMasterVolume);

        tmp = GameObject.Find("CategorySelectButton");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickCategorySelect);

        tmp = GameObject.Find("CategoryVolumeSlider");
        categoryVolumeSlider = tmp.GetComponent<Slider>();
        categoryVolumeSlider.onValueChanged.AddListener(OnValueChangeCategoryVolume);


        // ------------------------------------------

        tmp = GameObject.Find("BackButton");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickMoveSelectTable);

        tmp = GameObject.Find("StopAllButton");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickStopAll);

        tmp = GameObject.Find("MixerMenuButton");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickMoveMixerPanel);

        // -----------------------------------------

        tmp = GameObject.Find("SnapSelectButton");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickSelectSnapshot);

        tmp = GameObject.Find("MixerMoveTimeSlider");
        snapMoveTimeSlider = tmp.GetComponent<Slider>();
        snapMoveTimeSlider.onValueChanged.AddListener(OnValueChangeSnapMoveTime);

        tmp = GameObject.Find("ChangeSnapButton");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickChangeSnapshot);

        tmp = GameObject.Find("BackPlayerButton");
        tmpButton = tmp.GetComponent<Button>();
        tmpButton.onClick.AddListener(OnClickBackMixerPanel);


        basePanel.SetActive(true);
        mixerPanel.SetActive(false);


        setButtonText();

        masterSelectIndex = 0;
        categorySelectIndex = 0;
        masterVolumeSlider.value = AudioManager.GetMasterVolume(masterName[masterSelectIndex]);
        categoryVolumeSlider.value = AudioManager.GetCategoryVolume(categoryName[categorySelectIndex]);

        setMasterNameInfo();
        setCategoryNameInfo();

        snapSelectIndex = 0;
        snapMoveTimeSlider.value = 1;
        setSnapNameInfo();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // 全ボタンのラベルを変更
    void setButtonText()
    {
        for(int i=0; i<PLAYER_MAX; ++i)
        {
            labelSelectIndex[i] = (currentPage * PLAYER_MAX) + i;
            if ( labelSelectIndex[i] >= labelNum )
            {
                labelSelectIndex[i] = -1;
            }

            int index = labelSelectIndex[i];
            if (index >= 0)
            {
                playButtonList[i].text = " " + (index+1) + ": " + labelName[index];
            }
            else
            {
                playButtonList[i].text = " " + (index+1) + ": " + "empty";
            }
        }

        pageInfo.text = "(" + (currentPage+1) + "/" + pageNum + ")";
    }

    // 選択中スナップショットの情報を変更
    void setSnapNameInfo()
    {
        if (snapName != null )
        {
            if (snapSelectIndex < 0 || snapNum <= snapSelectIndex)
            {
                snapSelectIndex = 0;
            }
            if ( snapName.Length != 0 )
            {
	            snapNameInfo.text = "Snapshot:" + snapName[snapSelectIndex] + ":" + snapMoveTimeSlider.value + "sec";
	        }
        }
        else
        {
            snapNameInfo.text = "Snapshot:none";
        }
    }

    // 選択中マスターの情報を変更
    void setMasterNameInfo()
    {
        if (masterSelectIndex < 0 || masterNum <= masterSelectIndex )
        {
            masterSelectIndex = 0;
        }
        masterNameInfo.text = "Master:" + masterName[masterSelectIndex] + ":" + AudioManager.GetMasterVolume(masterName[masterSelectIndex]);
    }

    void setMasterNameInfo(float newVolume)
    {
        if (masterSelectIndex < 0 || masterNum <= masterSelectIndex)
        {
            masterSelectIndex = 0;
        }
        masterNameInfo.text = "Master:" + masterName[masterSelectIndex] + ":" + newVolume;
    }


    // 選択中カテゴリの情報を変更
    void setCategoryNameInfo()
    {
        if (categorySelectIndex < 0 || categoryNum <= categorySelectIndex)
        {
            categorySelectIndex = 0;
        }
        categoryNameInfo.text = "Category:" + categoryName[categorySelectIndex] + ":" + AudioManager.GetCategoryVolume(categoryName[categorySelectIndex]);
    }

    void setCategoryNameInfo(float newVolume)
    {
        if (categorySelectIndex < 0 || categoryNum <= categorySelectIndex)
        {
            categorySelectIndex = 0;
        }
        categoryNameInfo.text = "Category:" + categoryName[categorySelectIndex] + ":" + newVolume;
    }

    // 選択マスターを変更
    void changeSelectMaster()
    {
        if (0 <= masterSelectIndex && masterSelectIndex < masterNum)
        {
            masterVolumeSlider.value = AudioManager.GetMasterVolume(masterName[masterSelectIndex]);
        }
    }

    // 選択カテゴリを変更
    void changeSelectCategory()
    {
        if (0 <= categorySelectIndex && categorySelectIndex < categoryNum)
        {
            categoryVolumeSlider.value = AudioManager.GetCategoryVolume(categoryName[categorySelectIndex]);
        }
    }

    //////////////////////////////////////
    // Mixer

    public void OnClickBackMixerPanel()
    {
        basePanel.SetActive(true);
        mixerPanel.SetActive(false);
    }

    public void OnClickSelectSnapshot()
    {
        ++snapSelectIndex;
        if (snapSelectIndex >= snapNum)
        {
            snapSelectIndex = 0;
        }
        setSnapNameInfo();
    }

    public void OnClickChangeSnapshot()
    {
        if (snapName != null)
        {
            AudioManager.SetSnapshot(snapName[snapSelectIndex], snapMoveTimeSlider.value);
        }
    }

    public void OnValueChangeSnapMoveTime(float value)
    {
        setSnapNameInfo();
    }

    //////////////////////////////////////
    // Base

    public void OnClickMoveMixerPanel()
    {
        if (snapNum != 0)
        {
            basePanel.SetActive(false);
            mixerPanel.SetActive(true);
        }
    }

    public void OnClickMoveSelectTable()
    {
        if (backButtonMethod != null)
        {
            backButtonMethod();
        }
    }

    public void OnClickStopAll()
    {
        AudioManager.StopAll();
    }

    public void OnClickCategorySelect()
    {
        ++categorySelectIndex;
        if (categorySelectIndex >= categoryNum)
        {
            categorySelectIndex = 0;
        }
        Debug.Log("category index:" + categorySelectIndex);
        changeSelectCategory();
        setCategoryNameInfo();
    }

    public void OnClickMasterSelect()
    {
        ++masterSelectIndex;
        if (masterSelectIndex >= masterNum)
        {
            masterSelectIndex = 0;
        }
        changeSelectMaster();
        setMasterNameInfo();
    }

    public void OnValueChangeCategoryVolume(float value)
    {
        AudioManager.SetCategoryVolume(categoryName[categorySelectIndex], value);
        setCategoryNameInfo(value);
    }

    public void OnValueChangeMasterVolume(float value)
    {
        AudioManager.SetMasterVolume(masterName[masterSelectIndex], value);
        setMasterNameInfo(value);
    }

    public void OnClickPagePrev100()
    {
        currentPage -= 100;
        if ( currentPage < 0 )
        {
            currentPage = pageNum - 1;
        }
        setButtonText();
    }

    public void OnClickPageNext100()
    {
        currentPage += 100;
        if (currentPage >= pageNum)
        {
            currentPage = 0;
        }
        setButtonText();
    }

    public void OnClickPagePrev10()
    {
        currentPage -= 10;
        if (currentPage < 0)
        {
            currentPage = pageNum - 1;
        }
        setButtonText();
    }

    public void OnClickPageNext10()
    {
        currentPage += 10;
        if (currentPage >= pageNum)
        {
            currentPage = 0;
        }
        setButtonText();
    }

    public void OnClickPagePrev1()
    {
        --currentPage;
        if (currentPage < 0)
        {
            currentPage = pageNum - 1;
        }
        setButtonText();
    }

    public void OnClickPageNext1()
    {
        ++currentPage;
        if (currentPage >= pageNum)
        {
            currentPage = 0;
        }
        setButtonText();
    }

    public void OnClickPlay0()
    {
        AudioManager.Play(labelName[labelSelectIndex[0]]);
    }

    public void OnClickStop0()
    {
        AudioManager.StopLabel(labelName[labelSelectIndex[0]]);
    }

    public void OnClickPlay1()
    {
        AudioManager.Play(labelName[labelSelectIndex[1]]);
    }

    public void OnClickStop1()
    {
        AudioManager.StopLabel(labelName[labelSelectIndex[1]]);
    }
    public void OnClickPlay2()
    {
        AudioManager.Play(labelName[labelSelectIndex[2]]);
    }

    public void OnClickStop2()
    {
        AudioManager.StopLabel(labelName[labelSelectIndex[2]]);
    }
    public void OnClickPlay3()
    {
        AudioManager.Play(labelName[labelSelectIndex[3]]);
    }

    public void OnClickStop3()
    {
        AudioManager.StopLabel(labelName[labelSelectIndex[3]]);
    }
    public void OnClickPlay4()
    {
        AudioManager.Play(labelName[labelSelectIndex[4]]);
    }

    public void OnClickStop4()
    {
        AudioManager.StopLabel(labelName[labelSelectIndex[4]]);
    }

}
