using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System;
using UnityEngine.Audio;
using System.Threading;


#pragma warning disable 649 // Wrapper<T>のメンバが定義のみで警告がでるので警告OFF


namespace USnd
{

    public partial class AudioManager : MonoBehaviour
    {
#if UNITY_EDITOR
#if USND_EDIT_MODE
        bool IsActiveTool = true;       // USndTool用の処理を実行するときはtrue, 不要なときはfalse
#else
        bool IsActiveTool = false;       // USndTool用の処理を実行するときはtrue, 不要なときはfalse
#endif
#endif

        bool IsOnMute = false;

        int AndroidSoundPoolNum = 6;    // AndroidでSoundPool再生するときの最大数

        Dictionary<string, AudioPlayer> sourceDict = new Dictionary<string, AudioPlayer>();

        Dictionary<int, AudioPlayer> playAudioDict = new Dictionary<int, AudioPlayer>();        // インスタンスIDとAudioPlayerのディクショナリ.
        Dictionary<string, List<int>> playCategoryDict = new Dictionary<string, List<int>>();     // カテゴリごとの再生中AudioPlayerInstanceIDリスト.
        Dictionary<string, AudioCategorySettings> categoryDict = new Dictionary<string, AudioCategorySettings>();           // カテゴリ一覧、参照.
        Dictionary<string, AudioMasterSettings> masterDict = new Dictionary<string, AudioMasterSettings>();     // マスタ一覧、参照.
        Dictionary<string, List<string>> playDuckingTrigger = new Dictionary<string, List<string>>();   // ダッキングを発生させたラベルの停止時復帰用.

        List<int> playAudioRemoveKey = new List<int>();     // playAudioDictから削除するインスタンスIDを保存するのに使う
        HashSet<AudioPlayer> playerHashSet = new HashSet<AudioPlayer>(); // ユニークにplayerを更新するようの使い回し

        Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();      // プログラムから直接AudioSourceをつくるときに使うAudioClip

        Dictionary<string, Audio3DSettings> audio3DSettings = new Dictionary<string, Audio3DSettings>();

        enum RESULT
        {
            CONTINUE,       // 処理続行
            EXECUTE,        // 処理実行
            FINISH,         // 処理終了
        };

        // public List<AudioCategorySettings> categoryList = new List<AudioCategorySettings>();
        // public List<AudioMasterSettings> masterList = new List<AudioMasterSettings>();


        AudioMixerSettings mixerSettings = new AudioMixerSettings();

        Transform CacheTransform { get { return (_cacheTransform != null) ? _cacheTransform : (_cacheTransform = this.transform); } }
        Transform _cacheTransform;


#if UNITY_EDITOR

        public struct SoundLabelInfo
        {
            public int instance;
            public string labelName;
        }

        List<SoundLabelInfo> labelInfoList;

        List<string> logs;

        List<string> tableLogs;

        HashSet<string> callLog;    //コール履歴(重複なし)保存用



        public List<SoundLabelInfo> getLabelInfoList()
        {
            return labelInfoList;
        }

        public List<string> getLog()
        {
            return logs;
        }

        public List<string> getTableLog()
        {
            return tableLogs;
        }


        public HashSet<string> getCallLog()
        {
            return callLog;
        }

        public void clearCallLog()
        {
            callLog.Clear();
        }

        // エディタ用、Cloneしていないオリジナルのパラメータを保持
        Dictionary<string, Audio3DSettings> audio3DSetShallow = new Dictionary<string, Audio3DSettings>();

        // ---------------------------------------------------------------
        // 再生情報一覧を削除(Editor用)
        public void soundToolPlayListClear()
        {
            if (labelInfoList != null)
            {
                labelInfoList.Clear();
            }
        }

        // ---------------------------------------------------------------
        // ログをクリア(Editor用)
        public void soundToolLogsClear()
        {
            if (logs != null)
            {
                logs.Clear();
            }
        }

        // ---------------------------------------------------------------
        // ログに追加(Editor用)
        void AddLog(string str)
        {
            logs.Insert(0, DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString("000") + ": " + str);
            if (logs.Count > AudioDefine.LOG_MAX)
            {
                logs.RemoveAt(logs.Count - 1);
            }
        }

        // ---------------------------------------------------------------
        // ログに追加(Editor用)
        void AddTableLog(string str)
        {
            tableLogs.Insert(0, DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString("000") + ": " + str);
            if (tableLogs.Count > AudioDefine.LOG_MAX)
            {
                tableLogs.RemoveAt(tableLogs.Count - 1);
            }
        }

        // ---------------------------------------------------------------
        // コール履歴を保存(Editor用)
        void AddCallLog(string labelName)
        {
            callLog.Add(labelName);
        }

        // ---------------------------------------------------------------
        // 情報を登録(Editor用)
        void SetLabelInfoList(string labelName, int instance, float volume)
        {
            SoundLabelInfo info = new SoundLabelInfo();
            info.instance = instance;
            info.labelName = labelName;
            labelInfoList.Insert(0, info);
            if (labelInfoList.Count > AudioDefine.SOUNDINFO_MAX)
            {
                // とまってるやつを優先して消すようにする
                for (int i = labelInfoList.Count - 1; i >= 0; --i)
                {
                    if (getInstanceStatus(labelInfoList[i].instance) == AudioDefine.INSTANCE_STATUS.STOP)
                    {
                        labelInfoList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public AudioLabelSettings getLabelInfo(string name)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(name, out player))
            {
                return player.GetLabelSettings();
            }
            return null;
        }

        public AudioCategorySettings getCategoryInfo(string name)
        {
            AudioCategorySettings value;
            if (categoryDict.TryGetValue(name, out value))
            {
                return value;
            }
            return null;
        }


        public AudioMasterSettings getMasterInfo(string name)
        {
            AudioMasterSettings value;
            if (masterDict.TryGetValue(name, out value))
            {
                return value;
            }
            return null;
        }

        public Audio3DSettings getAudio3DSettingsInfo(string name)
        {
            Audio3DSettings value;
            if(audio3DSettings.TryGetValue(name, out value))
            {
                return value;
            }
            return null;
        }

        public bool saveAudio3DSettingsParam(Audio3DSettings audio3d)
        {
            Audio3DSettings org;
            if(audio3DSetShallow.TryGetValue(audio3d.spatialName, out org))
            {
                org.Copy(audio3d);
                return true;
            }
            return false;
        }

        public bool undoAudio3DSettingsParam(Audio3DSettings audio3d)
        {
            Audio3DSettings org;
            if (audio3DSetShallow.TryGetValue(audio3d.spatialName, out org))
            {
                audio3d.Copy(org);
                return true;
            }
            return false;
        }

        // ---------------------------------------------------------------
        // インスタンスを検索してspatialNameが名前が一致する3Dパラメータを更新する
        public void updateAudio3DSettings(Audio3DSettings audio3d)
        {
            // インスタンス全部検索
            // Label設定のSpatialNameと一致するならパラメータ更新
            foreach (KeyValuePair<int, AudioPlayer> playValue in playAudioDict)
            {
                if (playValue.Value != null)
                {
                    playValue.Value.UpdateAudio3DSettings(audio3d);
                }
            }
        }

#endif
        void OnDestroy()
        {
            USndAndroidNativePlayer.Terminate();
        }

        void OnApplicationPause(bool status)
        {

        }

        void OnApplicationFocus(bool status)
        {
            // Androidの上側メニューひらいたとき用に止める
            if (Application.platform == RuntimePlatform.Android)
            {
                // キーボードなど出たときにも止まってしまうのでここで止めるのをやめるver2.4.1
                if (status)
                {
                    USndPlugin.SetAudioFocus();
                    // focus on
                    /*
                    if ( USndPlugin.isSetAudioFocus )
                    {
	                    offPauseAll(0.1f);
	                }*/
                }
                else
                {
                    // focus off
                    /*
                    if ( USndPlugin.isSetAudioFocus )
                    {
	                    onPauseAll(0.1f);
	                }*/
                }
            }
            SetMannerMode();
        }

        void onHeadsetPlugCallback(string status)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (AudioDefine.ANDROID_MANNER_MODE_MUTE)
                {
                    if (status.CompareTo("mute_on") == 0 || status.CompareTo("mute_off") == 0)
                    {
                        SetMannerMode();
                    }
                    else if (USndPlugin.IsMannerMode() == true)
                    {
                        if (status.CompareTo("noisy") == 0)
                        {
                            // BecomingNoisyのときは必ずミュートにする(有線・Bluetooth両方の可能性あり)
                            SetMannerMode(true);
                        }
                        else if (status.CompareTo("false") == 0)
                        {
                            // マナーモードON、ヘッドホンなしなのでミュートする
                            // こちらは有線しかこない。有線ヘッドホンを抜いてBluetoothが繋がっている可能性を考慮して判定する
                            SetMannerMode();
                        }
                        else
                        {
                            // マナーモードON、ヘッドホンありなのでミュートしない
                            SetMannerMode(false);
                        }
                    }
                    else
                    {
                        SetMannerMode(false);
                    }
                }
            }
        }

        // ---------------------------------------------------------------
        // マナーモード強制ミュート設定.
        private void SetMannerMode(bool onMute)
        {
            if (AudioDefine.ANDROID_MANNER_MODE_MUTE)
            {
                foreach (KeyValuePair<string, AudioMasterSettings> value in masterDict)
                {
                    AudioMasterSettings master = value.Value;
                    master.SetMannerMode(onMute);
                }
            }
        }

        private void SetMannerMode()
        {
            if (AudioDefine.ANDROID_MANNER_MODE_MUTE)
            {
                if (USndPlugin.IsMannerMode() == true)
                {
                    if (USndPlugin.IsSpeaker())
                    {
                        // マナーモードON、ヘッドホンなしなのでミュートON
                        SetMannerMode(true);
                    }
                    else
                    {
                        // マナーモードON、ヘッドホンありなのでミュートOFF
                        SetMannerMode(false);
                    }
                }
                else
                {
                    SetMannerMode(false);
                }
            }
        }

        void Awake()
        {
#if UNITY_EDITOR
            labelInfoList = new List<SoundLabelInfo>();
            logs = new List<string>();
            tableLogs = new List<string>();
            callLog = new HashSet<string>();
#endif

            this.name = "USndAudioManager";
            AudioInstancePool.Initialize();
#if USND_DEBUG_LOG
            AudioDebugLog.Log("AudioManager Awake().");
#endif

            USndPlugin.Init(this.name, "onHeadsetPlugCallback");

            // SoundPoolの発音数はAndroidSoundPoolNumで決め打ち
            USndAndroidNativePlayer.Initialize(AndroidSoundPoolNum);

            SetMannerMode();

            AudioMainPool.Initialize(this.gameObject);
#if USND_DEBUG_LOG
            AudioDebugLog.Log("AudioManager Awake() Finish.");
#endif
        }

        // ---------------------------------------------------------------
        // Unityのミキサー情報を登録
        public void setAudioMixer(AudioMixer mixer)
        {
            if (mixer != null)
            {
                mixerSettings.SetAudioMixer(mixer);
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetUnityMixerInfo name:" + mixer.name + ".</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // Unityのミキサー情報を解除
        public void unsetAudioMixer()
        {
            mixerSettings.SetAudioMixer(null);
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>UnsetUnityMixerInfo.</color>");
#endif
        }

        // ---------------------------------------------------------------
        // Snapshotを設定
        public void setSnapshot(string snapName, float time)
        {
            if (mixerSettings != null)
            {
                mixerSettings.SetSnapshot(snapName, time);
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>SetSnapshot name:" + snapName + " time:" + time + ".</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // Exposed ParametersでMixerの値を変更
        public void setAudioMixerExposedParam(string paramName, float value)
        {
            if (mixerSettings != null)
            {
                mixerSettings.SetFloat(paramName, value);
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>SetAudioMixerExposedParam name:" + paramName + " value:" + value + ".</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // Audio3DSettingsを登録(Json形式で読み込み)
        public void setAudio3DSettingsFromJson(string jsonStr)
        {
            Audio3DSettings settings = D3SettingsFromJson(jsonStr);
            if ( settings.spatialName != "" )
            {
                audio3DSettings.Add(settings.spatialName, settings);
            }
        }

        // ---------------------------------------------------------------
        // Audio3DSettingsを登録
        public void setAudio3DSettings(Audio3DSettings setting)
        {
            if (setting.spatialName != "")
            {
#if USND_EDIT_MODE
                audio3DSettings.Add(setting.spatialName, (Audio3DSettings)setting.Clone());
#if UNITY_EDITOR
                audio3DSetShallow.Add(setting.spatialName, setting);
#endif
#else
                audio3DSettings.Add(setting.spatialName, setting);
#endif
            }
        }

        // ---------------------------------------------------------------
        // Audio3DSettingsを登録
        public void setAudio3DSettings(Audio3DSettings[] settings)
        {
            for (int i = 0; i < settings.Length; ++i)
            {
                if (settings[i].spatialName != "")
                {
                    audio3DSettings.Add(settings[i].spatialName, settings[i]);
                }
            }
        }

        string getChunk(byte[] tableData, int startIndex)
        {
            byte[] chunkByte = new byte[4];
            Buffer.BlockCopy(tableData, startIndex, chunkByte, 0, 4);
            return System.Text.Encoding.UTF8.GetString(chunkByte);
        }

        // ---------------------------------------------------------------
        // バイナリデータを読み込む
        public bool loadBinaryTable(byte[] tableData, int loadId)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>LoadBinaryTable loadId: " + loadId + ".</color>");
#endif
            int startIndex = 0;
            string chunk = getChunk(tableData, 0);
            startIndex += 4;

            // ver
            if ( chunk.CompareTo("ver ")  != 0 )
            {
                return false;
            }

            // バージョンを確認
            int intValue = BitConverter.ToInt32(tableData, startIndex);
            startIndex += 4;
            int tableVer = intValue;

            // このソースの対応している下位バージョン以下または上位バージョン以上のファイルならエラー
            if ( intValue < AudioDefine.TABLE_LOWER_VERSION || AudioDefine.TABLE_UPPER_VERSION < intValue)
            {
                // バージョンが一致しないので失敗
                return false;
            }

            // テーブルのIDを取得
            intValue = BitConverter.ToInt32(tableData, startIndex);
            startIndex += 4;

#if UNITY_EDITOR
            string fileName = getString(tableData, ref startIndex);
#else
            getString(tableData, ref startIndex);
#endif
            // 何のデータが入っているチャンクか
            chunk = getChunk(tableData, startIndex);
            startIndex += 4;

#if UNITY_EDITOR
            if (IsActiveTool)
            {
                AddLog("<color=cyan>   Table name=" + fileName + " ID=" + intValue + ", Type=" + chunk + ".</color>");
                AddTableLog(fileName + " ID=" + intValue + " Type=" + chunk);
            }
#if USND_DEBUG_LOG
            AudioDebugLog.Log("LoadBinaryTable name: " + fileName + " loadId: " + loadId + " type: " + chunk + " table id:" + intValue);
#endif
#endif


            if ( chunk.CompareTo("mstr") == 0)
            {
                return loadMasterBinary(tableData, ref startIndex);
            }
            else if (chunk.CompareTo("ctgr") == 0)
            {
                return loadCategoryBinary(tableData, ref startIndex);
            }
            else if ( chunk.CompareTo("lbl ") == 0)
            {
                return loadLabelBinary(tableData, ref startIndex, loadId, tableVer);
            }

            return false;
        }

        string getString(byte[] tableData, ref int startIndex)
        {
            int textSize = BitConverter.ToInt32(tableData, startIndex);
            startIndex += 4;
            if (textSize == 0) return null;

            byte[] text = new byte[textSize];
            Buffer.BlockCopy(tableData, startIndex, text, 0, textSize);
            startIndex += textSize;
            return System.Text.Encoding.UTF8.GetString(text);
        }

        // ---------------------------------------------------------------
        // Jsonデータを読み込む
        public bool loadJson(string tableData, int loadId)
        {
            if ( tableData != "" && loadJsonStatus != AudioDefine.LOAD_JSON_STATUS.LOADING)
            {
                loadJsonStatus = AudioDefine.LOAD_JSON_STATUS.LOADING;

                // コルーチンで実行
                StartCoroutine(loadJsonImpl(tableData, loadId));

                return true;
            }
            else if ( tableData == "" && loadJsonStatus != AudioDefine.LOAD_JSON_STATUS.LOADING )
            {
                // ステータスがLOADINGのときはステータス上書きしない
                loadJsonStatus = AudioDefine.LOAD_JSON_STATUS.ERROR;
            }

            return false;
        }

        private Thread jsonThread;
        private bool jsonThreadFlag;
        private string jsonStr;

        private AudioMasterSettings[] tmpMaster;
        private AudioCategorySettings[] tmpCategory;
        private AudioLabelSettings[] tmpLabel;

        IEnumerator loadJsonImpl(string tableData, int loadId)
        {
            // コルーチンの中でスレッドを起動
            jsonThreadFlag = true;
            jsonStr = tableData;

            tmpMaster = null;
            tmpCategory = null;
            tmpLabel = null;

            jsonThread = new Thread(jsonParse);
            jsonThread.Start();
            
            // スレッド終了までコルーチン待機
            while (jsonThreadFlag)
            {
                yield return null;
            }
            
            // 取得した情報を他のロードと同様にセット
            if ( tmpMaster != null )
            {
                addMasterSettings(tmpMaster);
                tmpMaster = null;
            }
            else if ( tmpCategory != null )
            {
                addCategorySettings(tmpCategory);
                tmpCategory = null;
            }
            else if ( tmpLabel != null )
            {
                // json用のlabel読み込み処理
                loadLabelJson(loadId);
                tmpLabel = null;
            }

            loadJsonStatus = AudioDefine.LOAD_JSON_STATUS.FINISH;
        }

        private void jsonParse()
        {
            // スレッド内でJson読み込み
            if (jsonStr.IndexOf("{\"master\":[") >= 0 )
            {
                tmpMaster = MasterFromJson<AudioMasterSettings>(jsonStr);
            }
            else if (jsonStr.IndexOf("{\"category\":[") >= 0)
            {
                tmpCategory = CategoryFromJson<AudioCategorySettings>(jsonStr);
            }
            else if (jsonStr.IndexOf("{\"label\":[") >= 0)
            {
                tmpLabel = LabelFromJson<AudioLabelSettings>(jsonStr);
            }
            
            // 読み込み完了
            jsonThreadFlag = false;
        }

        private T[] MasterFromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.master;
        }
        private T[] CategoryFromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.category;
        }
        private T[] LabelFromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.label;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] master;
            public T[] category;
            public T[] label;
        }

        private Audio3DSettings D3SettingsFromJson(string json)
        {
            AudioSourceWrapper settings = JsonUtility.FromJson<AudioSourceWrapper>(json);
            Audio3DSettings d3 = ScriptableObject.CreateInstance<Audio3DSettings>();//new Audio3DSettings();

            d3.spatialName = settings.spatialName;
            d3.spatialBlend = settings.spatialBlend;
            d3.reverbZoneMix = settings.reverbZoneMix;
            d3.dopplerLevel = settings.dopplerLevel;
            d3.spread = settings.spread;
            d3.rolloffMode = settings.rolloffMode;
            d3.minDistance = settings.minDistance;
            d3.maxDistance = settings.maxDistance;
            d3.customRolloffCurve = settings.customRolloffCurve;
            d3.spatialBlendCurve = settings.spatialBlendCurve;
            d3.reverbZoneMixCurve = settings.reverbZoneMixCurve;
            d3.spreadCurve = settings.spreadCurve;

            return d3;
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

        // ---------------------------------------------------------------
        // ラベルをバイナリから読み込み
        bool loadLabelJson(int loadId)
        {
            // ラベルの数を取得
            int recordNum = tmpLabel.Length;

            for (int i = 0; i < recordNum; ++i)
            {
                string labelName = tmpLabel[i].name;
                AudioPlayer player = null;
                AudioLabelSettings label = null;

                if (sourceDict.TryGetValue(labelName, out player) == true)
                {
                    // 存在するので一回削除
                    player.StopAll(0);
                    removeLabel(labelName);
                }

                player = new AudioPlayer();
                player.PlayerName = labelName;
                label = tmpLabel[i];
                label.loadId = loadId;

                bool error = false;
                AudioClip clip = null;


                //'CategoryName
                AudioCategorySettings findCategory;
                if (categoryDict.TryGetValue(label.GetCategoryName(), out findCategory))
                {
                    label.SetAttachCategoryInstance(categoryDict[label.GetCategoryName()]);
                }
                else
                {
#if USND_DEBUG_LOG
                    AudioDebugLog.Log("カテゴリ[" + label.GetCategoryName() + "]が見つからなかったので" + label.name + "を登録しませんでした。");
#endif
                    error = true;
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>    カテゴリ[" + label.GetCategoryName() + "]が見つからなかったので" + label.name + "を登録しませんでした。</color>");
#endif
                }

                //'VolRandomMax
#if UNITY_EDITOR
                if (label.isVolumeRandom && label.volumeRandomMin == label.volumeRandomMax)
                    AddLog("<color=red>" + label.name + ":ランダムボリュームの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                if (label.isVolumeRandom && label.volumeRandomMin == label.volumeRandomMax)
                    AudioDebugLog.LogWarning(label.name + ":ランダムボリュームの最大値と最小値が同じ値です.");
#endif


#if UNITY_EDITOR
                if (label.isPitchRandom && label.pitchRandomMin == label.pitchRandomMax)
                    AddLog("<color=red>" + label.name + ":ランダムピッチの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                if (label.isPitchRandom && label.pitchRandomMin == label.pitchRandomMax)
                    AudioDebugLog.LogWarning(label.name + ":ランダムピッチの最大値と最小値が同じ値です.");
#endif


#if UNITY_EDITOR
                if (label.isPanRandom && label.panRandomMin == label.panRandomMax)
                    AddLog("<color=red>" + label.name + ":ランダムパンの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                if (label.isPanRandom && label.panRandomMin == label.panRandomMax)
                    AudioDebugLog.LogWarning(label.name + ":ランダムパンの最大値と最小値が同じ値です.");
#endif

                // エラーがあるか名前がロードされてなかったらロード失敗
                if (error == true)
                {
                    deleteAudioSource(player);
                }
                else
                {
                    sourceDict.Add(player.PlayerName, player);
                    if (player.Init(clip, label.GetClipName(), label, sourceDict) == false)
                    {
#if USND_DEBUG_LOG
                            AudioDebugLog.LogWarning(player.PlayerName + "の初期化に失敗");
#endif
                    }
                    setUnityAudioMixer(player);
                }

            }

            updateRandomSourceInfoAll();

            return true;
        }


        // ---------------------------------------------------------------
        // マスターをバイナリから読み込み
        bool loadMasterBinary(byte[] tableData, ref int startIndex)
        {
            // マスターの数を取得
            int recordNum = BitConverter.ToInt32(tableData, startIndex);
            startIndex += 4;

            AudioMasterSettings[] data = new AudioMasterSettings[recordNum];

            for (int i = 0; i < recordNum; ++i)
            {
                data[i] = new AudioMasterSettings();
                data[i].masterName = getString(tableData, ref startIndex);
                data[i].volume = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
            }

            addMasterSettings(data);

            return true;
        }

        // ---------------------------------------------------------------
        // カテゴリをバイナリから読み込み
        bool loadCategoryBinary(byte[] tableData, ref int startIndex)
        {
            // カテゴリの数を取得
            int recordNum = BitConverter.ToInt32(tableData, startIndex);
            startIndex += 4;

            AudioCategorySettings[] data = new AudioCategorySettings[recordNum];

            for (int i = 0; i < recordNum; ++i)
            {
                data[i] = new AudioCategorySettings();
                data[i].categoryName = getString(tableData, ref startIndex);
                data[i].volume = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                data[i].maxPlaybacksNum = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                data[i].masterName = getString(tableData, ref startIndex);
            }

            addCategorySettings(data);

            return true;
        }

        // ---------------------------------------------------------------
        // ラベルをバイナリから読み込み
        bool loadLabelBinary(byte[] tableData, ref int startIndex, int loadId, int tableVer)
        {

            // ラベルはAudioLabelSettingsをまとめて登録するようなものはないので、
            // 既存のものがあるかチェック、あればそれを上書き、なければ新規で作成
            // そのあとプレイヤーを登録する

            // ラベルの数を取得
            int recordNum = BitConverter.ToInt32(tableData, startIndex);
            startIndex += 4;

            for (int i = 0; i < recordNum; ++i)
            {
                string labelName = getString(tableData, ref startIndex);
                AudioPlayer player = null;
                AudioLabelSettings label = null;
                bool newCreate = false;
                if (sourceDict.TryGetValue(labelName, out player) == true)
                {
                    // 存在する.
                    player.StopAll(0);
                    label = player.GetLabelSettings();
                }
                else
                {
                    // 存在しないので作る.                        
                    player = new AudioPlayer();
                    player.PlayerName = labelName;

                    label = new AudioLabelSettings();
                    label.name = labelName;

                    newCreate = true;
                }

                label.loadId = loadId;

                bool error = false;
                AudioClip clip = null;


                // ラベルの内容を読み込んで登録する

                //'ファイル名
                //putText Table(count + 1, 2).value, fileId
                label.clipName = getString(tableData, ref startIndex);
                audioClipDict.TryGetValue(label.clipName, out clip);

                //'Loop
                //PutBool Table(count + 1, 3).value, fileId
                label.isLoop = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //' Volume
                //PutFloat Table(count + 1, 4).value, fileId
                label.volume = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //' Behaviour
                //PutBehaviour Table(count + 1, 5).value, fileId
                label.maxPlaybacksBehavior = (AudioLabelSettings.BEHAVIOR)BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                //'Priority
                //PutInt Table(count + 1, 6).value, fileId
                label.priority = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                //'CategoryName
                //putText Table(count + 1, 7).value, fileId
                label.categoryName = getString(tableData, ref startIndex);
                AudioCategorySettings findCategory;
                if (categoryDict.TryGetValue(label.GetCategoryName(), out findCategory))
                {
                    label.SetAttachCategoryInstance(categoryDict[label.GetCategoryName()]);
                }
                else
                {
#if USND_DEBUG_LOG
                    AudioDebugLog.Log("カテゴリ[" + label.GetCategoryName() + "]が見つからなかったので" + label.name + "を登録しませんでした。");
#endif
                    error = true;
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>    カテゴリ[" + label.GetCategoryName() + "]が見つからなかったので" + label.name + "を登録しませんでした。</color>");
#endif
                }
                //'SingleGroup
                //PutText Table(count + 1, 7).value, fileId
                label.singleGroup = getString(tableData, ref startIndex);
                //'MaxNum
                //PutInt Table(count + 1, 8).value, fileId
                label.maxPlaybacksNum = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                //'IsStealOldest
                //PutBool Table(cuont + 1, 9).value, fileId
                label.isStealOldest = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'UnityMixerName
                //putText Table(count + 1, 10).value, fileId
                label.unityMixerName = getString(tableData, ref startIndex);
                //'SpatialGroup
                //putText Table(count + 1, 11).value, fileId
                label.spatialGroup = getString(tableData, ref startIndex);
                //'Delay
                //PutFloat Table(count + 1, 12).value, fileId
                label.playStartDelay = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                // テーブルバージョン3以降
                if (tableVer >= AudioDefine.TABLE_ADD_INTERVAL_VERSION)
                {
                    //'IsAndroidNative
                    label.playInterval = BitConverter.ToSingle(tableData, startIndex);
                    startIndex += 4;
                }

                //'Pan
                //PutFloat Table(count + 1, 13).value, fileId
                label.pan = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'Pitch
                //PutInt Table(count + 1, 14).value, fileId
                label.pitchShiftCent = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                //'IsLastSamples
                //PutBool Table(count + 1, 15).value, fileId
                label.isPlayLastSamples = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'FadeInTime
                //PutFloat Table(count + 1, 16).value, fileId
                label.fadeInTime = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'FadeOutTime
                //PutFloat Table(count + 1, 17).value, fileId
                label.fadeOutTime = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'FadeInOldSample
                //PutFloat Table(count + 1, 18).value, fileId
                label.fadeInTimeOldSamples = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'FadeOutOnPause
                //PutFloat Table(count + 1, 19).value, fileId
                label.fadeOutTimeOnPause = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'FadeInOffPause
                //PutFloat Table(count + 1, 20).value, fileId
                label.fadeInTimeOffPause = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'IsVolumeRandom
                //PutBool Table(count + 1, 21).value, fileId
                label.isVolumeRandom = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'Incvolume
                //PutBool Table(count + 1, 22).value, fileId
                label.inconsecutiveVolume = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'VolRandomMin
                //PutFloat Table(count + 1, 23).value, fileId
                label.volumeRandomMin = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'VolRandomMax
                //PutFloat Table(count + 1, 24).value, fileId
                label.volumeRandomMax = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
#if UNITY_EDITOR
                if (label.isVolumeRandom && label.volumeRandomMin == label.volumeRandomMax)
                    AddLog("<color=red>" + label.name + ":ランダムボリュームの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                if (label.isVolumeRandom && label.volumeRandomMin == label.volumeRandomMax)
                    AudioDebugLog.LogWarning(label.name + ":ランダムボリュームの最大値と最小値が同じ値です.");
#endif

                //'VolRandomUnit
                //PutFloat Table(count + 1, 25).value, fileId
                label.volumeRandomUnit = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'IsPitchRandom
                //PutBool Table(count + 1, 26).value, fileId
                label.isPitchRandom = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'IncPitch
                //PutBool Table(count + 1, 27).value, fileId
                label.inconsecutivePitch = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'PitchRandomMin
                //PutFloat Table(count + 1, 28).value, fileId
                label.pitchRandomMin = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                //'PitchRandomMax
                //PutFloat Table(count + 1, 29).value, fileId
                label.pitchRandomMax = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
#if UNITY_EDITOR
                if (label.isPitchRandom && label.pitchRandomMin == label.pitchRandomMax)
                    AddLog("<color=red>" + label.name + ":ランダムピッチの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                if (label.isPitchRandom && label.pitchRandomMin == label.pitchRandomMax)
                    AudioDebugLog.LogWarning(label.name + ":ランダムピッチの最大値と最小値が同じ値です.");
#endif

                //'PitchRandomUnit
                //PutFloat Table(count + 1, 30).value, fileId
                label.pitchRandomUnit = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                //'IsPanRandom
                //PutBool Table(count + 1, 31).value, fileId
                label.isPanRandom = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'IncPan
                //PutBool Table(count + 1, 32).value, fileId
                label.inconsecutivePan = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'PanRandomMin
                //PutFloat Table(count + 1, 33).value, fileId
                label.panRandomMin = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'PanRandomMax
                //PutFloat Table(count + 1, 34).value, fileId
                label.panRandomMax = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
#if UNITY_EDITOR
                if (label.isPanRandom && label.panRandomMin == label.panRandomMax)
                    AddLog("<color=red>" + label.name + ":ランダムパンの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                if (label.isPanRandom && label.panRandomMin == label.panRandomMax)
                    AudioDebugLog.LogWarning(label.name + ":ランダムパンの最大値と最小値が同じ値です.");
#endif

                //'PanRandomUnit
                //PutFloat Table(count + 1, 35).value, fileId
                label.panRandomUnit = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'IsRandomSrc
                //PutBool Table(count + 1, 36).value, fileId
                label.isRandomPlay = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'IncSrc
                //PutBool Table(count + 1, 37).value, fileId
                label.inconsecutiveSource = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'RandomSource
                //PutString Table(count + 1, 38).value, fileId
                int num = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                if ( num != 0 )
                {
                    label.randomSource = new string[num];
                    for(int j=0; j<num; ++j)
                    {
                        label.randomSource[j] = getString(tableData, ref startIndex);
                        //label.randomSource.Add(getString(tableData, ref startIndex));
                    }
                }
                //'IsMovePitch
                //PutBool Table(count + 1, 39).value, fileId
                label.isMovePitch = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'PitchStart
                //PutInt Table(count + 1, 40).value, fileId
                label.pitchStart = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                //'PitchEnd
                //PutInt Table(count + 1, 41).value, fileId
                label.pitchEnd = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                //'PitchMoveTime
                //PutInt Table(count + 1, 42).value, fileId
                label.pitchMoveTime = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'IsMovePan
                //PutBool Table(count + 1, 43).value, fileId
                label.isMovePan = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'PanStart
                //PutFloat Table(count + 1, 44).value, fileId
                label.panStart = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'PanEnd
                //PutFloat Table(count + 1, 45).value, fileId
                label.panEnd = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'PanMoveTime
                //PutFloat Table(count + 1, 46).value, fileId
                label.panMoveTime = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'DuckingCategory
                //PutString Table(count + 1, 47).value, fileId
                num = BitConverter.ToInt32(tableData, startIndex);
                startIndex += 4;
                if (num != 0)
                {
                    label.duckingCategories = new string[num];
                    for (int j = 0; j < num; ++j)
                    {
                        //label.duckingCategories.Add(getString(tableData, ref startIndex));
                        label.duckingCategories[j] = getString(tableData, ref startIndex);
                    }
                }
                //'DuckStart
                //PutFloat Table(count + 1, 48).value, fileId
                label.duckingStartTime = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'DuckEnd
                //PutFloat Table(count + 1, 49).value, fileId
                label.duckingEndTime = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'DuckVol
                //PutFloat Table(count + 1, 50).value, fileId
                label.duckingVolumeFactor = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                //'AutoRestore
                //PutBool Table(count + 1, 51).value, fileId
                label.autoRestoreDucking = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                startIndex += 4;
                //'RestoreTime
                //PutFloat Table(count + 1, 52).value, fileId
                label.restoreTime = BitConverter.ToSingle(tableData, startIndex);
                startIndex += 4;
                // テーブルバージョン2以降
                if (tableVer >= AudioDefine.TABLE_ADD_IS_ANDROID_NATIVE_VERSION)
                {
                    //'IsAndroidNative
                    label.isAndroidNative = BitConverter.ToInt32(tableData, startIndex) == 0 ? false : true;
                    startIndex += 4;
                }

                // エラーがあるか名前がロードされてなかったらロード失敗
                if (error == true)
                {
                    deleteAudioSource(player);
                }
                else
                {
                    if (newCreate == true)
                    {
                        sourceDict.Add(player.PlayerName, player);
                        if (player.Init(clip, label.GetClipName(), label, sourceDict) == false)
                        {
#if USND_DEBUG_LOG
                            AudioDebugLog.LogWarning(player.PlayerName + "の初期化に失敗");
#endif
                        }
                    }
                    setUnityAudioMixer(player);
                }

            }

            updateRandomSourceInfoAll();

            return true;
        }

        // ---------------------------------------------------------------
        // カテゴリをまとめて追加
        public void addCategorySettings(AudioCategorySettings[] list)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Length; ++i)
                {
                    AudioCategorySettings category = list[i];
                    addCategorySettings(category);
                }
            }
        }

        // ---------------------------------------------------------------
        // カテゴリを追加
        public bool addCategorySettings(AudioCategorySettings category)
        {
            if (category == null)
            {
                return false;
            }
            AudioCategorySettings dest;
            if (categoryDict.TryGetValue(category.categoryName, out dest) == false)
            {
                string categoryName = category.categoryName;
#if USND_DEBUG_LOG
                AudioDebugLog.Log("AudioManager add category " + categoryName);
#endif
                categoryDict.Add(categoryName, category);
                attachMasterSettings(category);

                if (playCategoryDict.ContainsKey(categoryName) == false)
                {
                    List<int> instanceList = new List<int>((category.maxPlaybacksNum > 0) ? category.maxPlaybacksNum : AudioDefine.LIST_CAPACITY);
                    playCategoryDict.Add(categoryName, instanceList);
                }

                if (playDuckingTrigger.ContainsKey(categoryName) == false)
                {
                    List<string> ducking = new List<string>();
                    playDuckingTrigger.Add(categoryName, ducking);
                }
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>AddCategorySettings name:" + category.categoryName + ".</color>");
#endif
                if (category.maxPlaybacksNum > 0)
                {
                    AudioMainPool.instance.AddEmpty(category.maxPlaybacksNum);
                }
                return true;
            }
            else
            {
#if USND_DEBUG_LOG
                AudioDebugLog.Log(category.categoryName + "は既に存在しているのでパラメータを上書きしました。");
#endif
                dest.CopySettings(category);
                attachMasterSettings(dest);
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>AddCategorySettings name:" + category.categoryName + " update.</color>");
#endif
            }
            return false;
        }

        // ---------------------------------------------------------------
        // マスターをまとめて追加
        public void addMasterSettings(AudioMasterSettings[] list)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Length; ++i)
                {
                    AudioMasterSettings master = list[i];
                    addMasterSettings(master);
                }
            }
        }

        // ---------------------------------------------------------------
        // マスターを追加
        public bool addMasterSettings(AudioMasterSettings master)
        {
            if (master == null)
            {
                return false;
            }
            AudioMasterSettings dest;
            if (masterDict.TryGetValue(master.masterName, out dest) == false)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.Log("AudioManager add master " + master.masterName);
#endif
                masterDict.Add(master.masterName, master);
                master.SetMute(IsOnMute);
                SetMannerMode();
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>AddMasterSettings name:" + master.masterName + ".</color>");
#endif
                return true;
            }
            else
            {
#if USND_DEBUG_LOG
                AudioDebugLog.Log(master.masterName + "は既に存在しているのでパラメータを上書きしました。");
#endif
                // 既に存在する場合は情報を上書き
                dest.CopySettings(master);
                dest.SetMute(IsOnMute);
                SetMannerMode();
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>AddMasterSettings name:" + master.masterName + " update.</color>");
#endif
            }
            return false;
        }

        // ---------------------------------------------------------------
        // AudioClipをまとめて登録
        public void addAudioClip(AudioClip[] clips)
        {
            for (int i = 0; i < clips.Length; ++i)
            {
                addAudioClip(clips[i]);
            }
        }

        // ---------------------------------------------------------------
        // AudioClipを登録
        public void addAudioClip(AudioClip clip)
        {
            if (!audioClipDict.ContainsKey(clip.name))
            {
                audioClipDict.Add(clip.name, clip);
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>AddAudioClip name:" + clip.name + ".</color>");
#endif
            }
            else
			{
				// 更新.
				audioClipDict[clip.name] = clip;
			}
        }

        // ---------------------------------------------------------------
        // 登録済みのAudioClipか
        public bool isExistAudioClip(string clipName)
        {
            return audioClipDict.ContainsKey(clipName);
        }

        // ---------------------------------------------------------------
        // AudioClipの登録を削除
        public void removeAudioClip(string clipName)
        {
            if (audioClipDict.ContainsKey(clipName))
            {
                audioClipDict.Remove(clipName);
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>RemoveAudioClip name:" + clipName + ".</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // AudioClipの登録をすべて削除
        public void removeAudioClipAll()
        {
            audioClipDict.Clear();
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>RemoveAudioClipAll.</color>");
#endif
        }

        // ---------------------------------------------------------------
        // ラベルが登録済みか.
        public bool findLabel(string name)
        {
            return sourceDict.ContainsKey(name);
        }

        // ---------------------------------------------------------------
        // 登録済みのカテゴリ名か.
        public bool findCategory(string name)
        {
            return categoryDict.ContainsKey(name);
        }

        // ---------------------------------------------------------------
        // 登録済みのマスタ名か.
        public bool findMaster(string name)
        {
            return masterDict.ContainsKey(name);
        }

        // ---------------------------------------------------------------
        // 指定したラベル名に関連するデータは削除可能か
        public bool canRemoveLabel(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player) == true)
            {
                if (player.GetPlayingTrueNum() != 0)
                {
                    return false;
                }
            }
            return true;
        }

        // ---------------------------------------------------------------
        // 指定したラベル名に関連するAudioClipの参照をはずす
        public bool unsetAudioClipToLabel(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player) == true)
            {
                AudioLabelSettings label = player.GetLabelSettings();
                if (label.isAndroidNative == true && Application.platform == RuntimePlatform.Android)
                {
                    int soundId = label.GetAndroidSoundId();
                    USndAndroidNativePlayer.Unload(soundId);
                }

                if (player.GetPlayingTrueNum() != 0)
                {
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>UnsetAudioClipToLabel name:" + labelName + " はまだ再生中なのでUnsetできません.</color>");
#endif
                    return false;
                }

                AudioClip clip = player.GetPlayClip();

                if (clip != null)
                {
                    audioClipDict.Remove(clip.name);
                }
                player.ResetPlayClip();

#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>UnsetAudioClipToLabel name:" + labelName + ".</color>");
#endif
                return true;
            }
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>UnsetAudioClipToLabel name:" + labelName + " error.</color>");
#endif
            // 指定されたラベル名が不正な場合はtrueを返す
            return true;
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してラベルに関連するAudioClipの参照をはずす
        public void unsetAudioClipToLabelLoadId(int loadId)
        {
            foreach (KeyValuePair<string, AudioPlayer> value in sourceDict)
            {
                AudioLabelSettings label = value.Value.GetLabelSettings();
                if (label.loadId == loadId)
                {
                    if (label.isAndroidNative == true && Application.platform == RuntimePlatform.Android)
                    {
                        int soundId = label.GetAndroidSoundId();
                        USndAndroidNativePlayer.Unload(soundId);
                    }
                    
                    AudioPlayer player = value.Value;

                    if (player.GetPlayingTrueNum() != 0)
                    {
#if UNITY_EDITOR
                        if (IsActiveTool)
                            AddLog("<color=red>UnsetAudioClipToLabelLoadId loadId:" + loadId + " " + player.PlayerName + " はまだ再生中なのでUnsetできません.</color>");
#endif
                    }
                    else
                    {
                        AudioClip clip = player.GetPlayClip();
                        if (clip != null)
                        {
                            audioClipDict.Remove(clip.name);
                        }
                        player.ResetPlayClip();
                    }
                }
            }

#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>UnsetAudioClipToLabelLoadId loadId:" + loadId + ".</color>");
#endif
        }

        // ---------------------------------------------------------------
        // すべてのラベルに関連するAudioClipの参照をはずす
        public void unsetAudioClipToLabelAll()
        {
            foreach (KeyValuePair<string, AudioPlayer> value in sourceDict)
            {
                AudioPlayer player = value.Value;
                if (player.GetPlayingTrueNum() != 0)
                {
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>UnsetAudioClipToLabelAll : " + player.PlayerName + " はまだ再生中なのでUnsetできません.</color>");
#endif
                }
                else
                {
                    AudioClip clip = player.GetPlayClip();
                    if (clip != null)
                    {
                        audioClipDict.Remove(clip.name);
                    }
                    player.ResetPlayClip();
                }
            }

#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>UnsetAudioClipToLabelAll.</color>");
#endif
        }

        // ---------------------------------------------------------------
        // 指定したラベル名に関連するデータを削除する
        public bool removeLabel(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player) == true)
            {
                if ( player.GetPlayingTrueNum() != 0 )
                {
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>RemoveLabel name:" + labelName + " はまだ再生中なのでRemoveできません.</color>");
#endif
                    return false;
                }

                resetDuckingBeforeUpdate(player);
                sourceDict.Remove(labelName);

                AudioClip clip = player.GetPlayClip();

                if (clip != null)
                {
                    audioClipDict.Remove(clip.name);
                }
                deleteAudioSource(player);

#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>RemoveLabel name:" + labelName + ".</color>");
#endif
                return true;
            }
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>RemoveLabel name:" + labelName + " error.</color>");
#endif
            // 指定されたラベル名が不正な場合はtrueを返す
            return true;
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してラベルに関連するデータを削除
        public void removeLabelLoadId(int loadId)
        {
            List<string> removeList = new List<string>(sourceDict.Count);
            foreach (KeyValuePair<string, AudioPlayer> value in sourceDict)
            {
                AudioLabelSettings label = value.Value.GetLabelSettings();
                if (label.loadId == loadId)
                {
                    AudioPlayer player = value.Value;

                    if (player.GetPlayingTrueNum() != 0)
                    {
#if UNITY_EDITOR
                        if (IsActiveTool)
                            AddLog("<color=red>RemoveLabelLoadId loadId:" + loadId + " " + player.PlayerName + " はまだ再生中なのでRemoveできません.</color>");
#endif
                    }
                    else
                    {
                        resetDuckingBeforeUpdate(player);
                        removeList.Add(value.Key);
                        AudioClip clip = player.GetPlayClip();
                        if (clip != null)
                        {
                            audioClipDict.Remove(clip.name);
                        }
                        deleteAudioSource(value.Value);
                    }
                }
            }

            for (int i = 0; i < removeList.Count; ++i)
            {
                sourceDict.Remove(removeList[i]);
            }

#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>RemoveLabelLoadId loadId:" + loadId + ".</color>");
#endif
        }

        // ---------------------------------------------------------------
        // すべてのラベルに関連するデータを削除.
        public void removeLabelAll()
        {
            stopAll();

            List<string> removeList = new List<string>(sourceDict.Count);

            foreach (KeyValuePair<string, AudioPlayer> value in sourceDict)
            {
                AudioPlayer player = value.Value;
                if (player.GetPlayingTrueNum() != 0)
                {
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>RemoveLabelAll : " + player.PlayerName + " はまだ再生中なのでRemoveできません.</color>");
#endif
                }
                else
                {
                    resetDuckingBeforeUpdate(player);
                    removeList.Add(value.Key);
                    AudioClip clip = player.GetPlayClip();
                    if (clip != null)
                    {
                        audioClipDict.Remove(clip.name);
                    }
                    deleteAudioSource(value.Value);
                }
            }

            for (int i = 0; i < removeList.Count; ++i)
            {
                sourceDict.Remove(removeList[i]);
            }
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>RemoveLabelAll.</color>");
#endif
        }

        // ---------------------------------------------------------------
        // すべての情報を削除.
        public void removeAll()
        {
            stopAll(0);
            removeLabelAll();
            removeAudioClipAll();

            masterDict.Clear();

            categoryDict.Clear();

            clearObjectPool();
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>RemoveAll.</color>");
#endif
        }


        void deleteAudioSource(AudioPlayer player)
        {
            player.Reset();
        }


        // ---------------------------------------------------------------
        // ランダムソースの参照をやり直す.
        public void updateRandomSourceInfo(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                player.UpdateRandomSourceInfo(sourceDict);
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>UpdateRandomSourceInfo name: " + labelName + "</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // ランダムソースの参照をやり直す(全部).
        public void updateRandomSourceInfoAll()
        {
            foreach (KeyValuePair<string, AudioPlayer> source in sourceDict)
            {
                AudioPlayer player = source.Value;
                player.UpdateRandomSourceInfo(sourceDict);
            }
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>UpdateRandomSourceInfoAll.</color>");
#endif
        }

        // ---------------------------------------------------------------
        // 指定したラベルのオーディオを事前にロードする(Preload Audio Dataのチェックがはずれているものだけ有効)
        public void loadAudioData(string labelName)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>LoadAudioData name:" + labelName + ".</color>");
#endif
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>       " + labelName + " not found.</color>");
#endif
                return;
            }
            else
            {
                player.LoadAudioData();
            }
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してオーディオを事前にロード(Preload Audio Dataのチェックがはずれているものだけ有効)
        public void loadAudioDataLoadId(int loadId)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>LoadAudioDataLoadId loadId: " + loadId + ".</color>");
#endif
            foreach (KeyValuePair<string, AudioPlayer> source in sourceDict)
            {
                AudioPlayer player = source.Value;
                AudioLabelSettings label = player.GetLabelSettings();
                if (label.loadId == loadId)
                {
                    player.LoadAudioData();
                }
            }
        }

        // ---------------------------------------------------------------
        // 指定したラベルのオーディオをアンロードする(Preload Audio Dataのチェックがはずれているものだけ有効)
        public void unloadAudioData(string labelName)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>UnloadAudioData labelName: " + labelName + ".</color>");
#endif
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>       " + labelName + " not found.</color>");
#endif
                return;
            }
            else
            {
                player.UnloadAudioData();
            }
        }

        // ---------------------------------------------------------------
        // すべてのオーディオをアンロード(Preload Audio Dataのチェックがはずれているものだけ有効)
        public void unloadAudioDataAll()
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>UnlaodAudioDataAll.</color>");
#endif
            foreach (KeyValuePair<string, AudioPlayer> source in sourceDict)
            {
                AudioPlayer player = source.Value;
                player.UnloadAudioData();
            }
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してオーディオをアンロード(Preload Audio Dataのチェックがはずれているものだけ有効)
        public void unloadAudioDataLoadId(int loadId)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>UnloadAudioDataLoadId loadId: " + loadId + ".</color>");
#endif
            foreach (KeyValuePair<string, AudioPlayer> source in sourceDict)
            {
                AudioPlayer player = source.Value;
                AudioLabelSettings label = player.GetLabelSettings();
                if (label.loadId == loadId)
                {
                    player.UnloadAudioData();
                }
            }
        }

        // ---------------------------------------------------------------
        // UnityのMixer設定を反映させる
        void setUnityAudioMixer(AudioPlayer player)
        {
            AudioLabelSettings label = player.GetLabelSettings();

            if (mixerSettings != null)
            {
                // sourceがない場合、Playerにmixerを渡しておく

                if (label.unityMixerName != null)
                {
                    AudioMixerGroup[] group = mixerSettings.FindGroup(label.unityMixerName);
                    if ( group != null )
                    {
	                    if (group.Length != 0)
	                    {
	                        player.SetAudioMixerGroup(group[0]);
	                    }
	                }
                }
            }
        }


        AudioDefine.LOAD_XML_STATUS loadXmlStatus = AudioDefine.LOAD_XML_STATUS.STANDBY;
        AudioDefine.LOAD_JSON_STATUS loadJsonStatus = AudioDefine.LOAD_JSON_STATUS.STANDBY;


        // ---------------------------------------------------------------
        // XML読み込み共通ステータス
        public AudioDefine.LOAD_XML_STATUS getLoadXmlStatus()
        {
            return loadXmlStatus;
        }

        // ---------------------------------------------------------------
        // Json読み込み共通ステータス
        public AudioDefine.LOAD_JSON_STATUS getLoadJsonStatus()
        {
            return loadJsonStatus;
        }

        // ---------------------------------------------------------------
        // Master設定ファイルを読み込む.
        public bool loadMasterXml(Stream xml, Stream xsd = null)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
            {
                AddLog("<color=cyan>LoadMasterXml.</color>");
                AddTableLog("LoadMasterXml");
            }
#endif
            if (loadXmlStatus == AudioDefine.LOAD_XML_STATUS.LOADING)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("別のXMLをロード中なので処理できません。");
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>       別のXMLをロード中なので処理できません.</color>");
#endif
                return false;
            }

            loadXmlStatus = AudioDefine.LOAD_XML_STATUS.LOADING;
            StartCoroutine(loadMasterXmlCoroutine(xml, xsd));

            xml.Dispose();
            if (xsd != null) xsd.Dispose();

            return true;
        }

        IEnumerator loadMasterXmlCoroutine(Stream xml, Stream xsd)
        {
            XmlDocument xmlDoc = null;
            if (xsd == null)
            {
                xmlDoc = AudioXmlLoad.Load(xml);
            }
            else
            {
                xmlDoc = AudioXmlLoad.Load(xsd, xml);
            }

            if (xmlDoc == null)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("MasterXMLの読み込みに失敗しました。");
#endif
                loadXmlStatus = AudioDefine.LOAD_XML_STATUS.ERROR;
                yield break;
            }

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("MasterSet");
            if (nodeList == null)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("MasterSetノードがありません。");
#endif
                loadXmlStatus = AudioDefine.LOAD_XML_STATUS.ERROR;
                yield break;
            }
            else
            {
                // 見つかったMasterSet分繰り返す.
                for (int i = 0; i < nodeList.Count; ++i)
                {
                    // 今回処理するMasterSet.
                    XmlNode node = nodeList[i];

                    if (node.HasChildNodes == false)
                    {
                        continue;
                    }

                    // ノードの順番は固定.
                    // 0は必ずMasterName
                    XmlNode dataNode = node.ChildNodes[0];
                    if (dataNode.Name.CompareTo("MasterName") != 0)
                    {
                        continue;
                    }

                    XmlNode valueNode = dataNode.ChildNodes[0];
                    AudioMasterSettings master = null;
                    bool add = false;
                    if ( valueNode == null )
                    {
#if USND_DEBUG_LOG
                        AudioDebugLog.LogWarning("MasterName Node Empty!");
#endif
                        continue;
                    }

                    if (masterDict.TryGetValue(valueNode.Value, out master) == false)
                    {
                        // 存在しないので作る.                        
                        if (dataNode.HasChildNodes == true)
                        {
                            master = new AudioMasterSettings();
                            master.masterName = valueNode.Value;

                            add = true;
                        }
                    }

                    if (master != null && node.ChildNodes.Count > 1)
                    {
                        dataNode = node.ChildNodes[1];
                        if (dataNode.Name.CompareTo("Volume") != 0)
                        {
                            continue;
                        }
                        if (dataNode.HasChildNodes == true)
                        {
                            valueNode = dataNode.ChildNodes[0];
                            master.volume = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    if (add == true)
                    {
                        addMasterSettings(master);
                    }
                    //yield return null;
                }
            }
            loadXmlStatus = AudioDefine.LOAD_XML_STATUS.FINISH;
        }

        // ---------------------------------------------------------------
        // カテゴリ設定にマスター情報を適用する
        void attachMasterSettings(AudioCategorySettings category)
        {
            AudioMasterSettings master;
            if (masterDict.TryGetValue(category.masterName, out master))
            {
                category.SetAttachMasterInstance(master);
            }
            else
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("マスター[" + category.masterName + "]は登録されていません");
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>マスター[" + category.masterName + "]は登録されていません.</color>");
#endif
            }
        }



        // ---------------------------------------------------------------
        // Category設定ファイルを読み込む.
        public bool loadCategoryXml(Stream xml, Stream xsd = null)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
            {
                AddLog("<color=cyan>LoadCategoryXml.</color>");
                AddTableLog("LoadCategoryXml");
            }
#endif
            if (loadXmlStatus == AudioDefine.LOAD_XML_STATUS.LOADING)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("別のXMLをロード中なので処理できません。");
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>       別のXMLをロード中なので処理できません.</color>");
#endif
                return false;
            }

            loadXmlStatus = AudioDefine.LOAD_XML_STATUS.LOADING;
            StartCoroutine(loadCategoryXmlCoroutine(xml, xsd));

            xml.Dispose();
            if (xsd != null) xsd.Dispose();

            return true;
        }

        IEnumerator loadCategoryXmlCoroutine(Stream xml, Stream xsd)
        {
            XmlDocument xmlDoc = null;

            if (xsd == null)
            {
                xmlDoc = AudioXmlLoad.Load(xml);
            }
            else
            {
                xmlDoc = AudioXmlLoad.Load(xsd, xml);
            }

            if (xmlDoc == null)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("CategoryXMLの読み込みに失敗しました。");
#endif
                yield break;
            }

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("CategorySet");
            if (nodeList == null)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("CategorySetノードがありません。");
#endif
                yield break;
            }
            else
            {
                // 見つかったCategorySet分繰り返す.
                for (int i = 0; i < nodeList.Count; ++i)
                {
                    // 今回処理するCategorySet.
                    XmlNode node = nodeList[i];

                    if (node.HasChildNodes == false)
                    {
                        continue;
                    }

                    // ノードの順番は固定.
                    // 0は必ずCategoryName
                    XmlNode dataNode = node.ChildNodes[0];
                    if (dataNode.Name.CompareTo("CategoryName") != 0)
                    {
                        continue;
                    }

                    XmlNode valueNode = dataNode.ChildNodes[0];
                    AudioCategorySettings category = null;
                    bool add = false;

                    if (valueNode == null)
                    {
#if USND_DEBUG_LOG
                        AudioDebugLog.Log("CategoryName Node Empty!");
#endif
                        continue;
                    }

                    if (categoryDict.TryGetValue(valueNode.Value, out category) == false)
                    {
                        // 存在しないので作る.                        
                        if (dataNode.HasChildNodes == true)
                        {
                            category = new AudioCategorySettings();
                            category.categoryName = valueNode.Value;
                            add = true;
                        }
                    }

                    if (category != null && node.ChildNodes.Count > 1)
                    {
                        // 指定がない場合はデフォルト値、順番は固定だけど要素が抜けることはある
                        for (int j = 1; j < node.ChildNodes.Count; ++j)
                        {
                            dataNode = node.ChildNodes[j];
                            if (dataNode.HasChildNodes == true)
                            {
                                valueNode = dataNode.ChildNodes[0];
                                switch (dataNode.Name)
                                {
                                    case "Volume":
                                        category.volume = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "MaxNum":
                                        category.maxPlaybacksNum = int.Parse(valueNode.Value);
                                        break;
                                    case "MasterName":
                                        category.masterName = valueNode.Value;
                                        attachMasterSettings(category);
                                        break;
                                }
                            }
                        }
                    }

                    if (add == true)
                    {
                        addCategorySettings(category);
                    }
                    //yield return null;
                }
            }
            loadXmlStatus = AudioDefine.LOAD_XML_STATUS.FINISH;
        }


        // ---------------------------------------------------------------
        // Label設定ファイルを読み込む.
        public bool loadLabelXml(int loadId, Stream xml, Stream xsd = null)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
            {
                AddLog("<color=cyan>LoadLabelXml.</color>");
                AddTableLog("LoadLabelXml");
            }
#endif
            if (loadXmlStatus == AudioDefine.LOAD_XML_STATUS.LOADING)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("別のXMLをロード中なので処理できません。");
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>       別のXMLをロード中なので処理できません.</color>");
#endif
                return false;
            }

            loadXmlStatus = AudioDefine.LOAD_XML_STATUS.LOADING;
            StartCoroutine(loadLabelXmlCoroutine(loadId, xml, xsd));

            xml.Dispose();
            if (xsd != null) xsd.Dispose();

            return true;
        }

        IEnumerator loadLabelXmlCoroutine(int loadId, Stream xml, Stream xsd)
        {
            XmlDocument xmlDoc = null;

            if (xsd == null)
            {
                xmlDoc = AudioXmlLoad.Load(xml);
            }
            else
            {
                xmlDoc = AudioXmlLoad.Load(xsd, xml);
            }

            if (xmlDoc == null)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("LabelXMLの読み込みに失敗しました。");
#endif
                yield break;
            }

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("LabelSet");
            if (nodeList == null)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning("LabelSetノードがありません。");
#endif
                yield break;
            }
            else
            {
                // 見つかったLabelSet分繰り返す.
                for (int i = 0; i < nodeList.Count; ++i)
                {
                    // 今回処理するLabelSet.
                    XmlNode node = nodeList[i];

                    if (node.HasChildNodes == false)
                    {
                        continue;
                    }
                    // ノードの順番は固定.
                    // 0は必ずLabelName
                    XmlNode dataNode = node.ChildNodes[0];
                    if (dataNode.Name.CompareTo("LabelName") != 0)
                    {
                        continue;
                    }

                    XmlNode valueNode = dataNode.ChildNodes[0];
                    AudioLabelSettings label = null;
                    AudioPlayer player = null;
                    AudioClip clip = null;
                    bool isLoop = false;
                    bool newCreate = false;

                    if (valueNode == null)
                    {
#if USND_DEBUG_LOG
                        AudioDebugLog.LogWarning("LabelName Node Empty!");
#endif
                        continue;
                    }

                    if (sourceDict.TryGetValue(valueNode.Value, out player) == true)
                    {
                        // 存在する.
                        player.StopAll(0);
                        label = player.GetLabelSettings();
                    }
                    else
                    {
                        // 存在しないので作る.                        
                        if (dataNode.HasChildNodes == true)
                        {
                            player = new AudioPlayer();
                            player.PlayerName = valueNode.Value;

                            label = new AudioLabelSettings();
                            label.name = valueNode.Value;

                            newCreate = true;
                        }
                    }

                    label.loadId = loadId;

                    bool error = false;
                    bool loadClipName = false;
                    if (label != null && node.ChildNodes.Count > 1)
                    {
                        // 指定がない場合はデフォルト値、順番は固定だけど要素が抜けることはある
                        for (int j = 1; j < node.ChildNodes.Count; ++j)
                        {
                            dataNode = node.ChildNodes[j];
                            if (dataNode.HasChildNodes == true)
                            {
                                valueNode = dataNode.ChildNodes[0];

                                // ループON/OFFもいる

                                switch (dataNode.Name)
                                {
                                    case "FileName":
                                        audioClipDict.TryGetValue(valueNode.Value, out clip);
                                        label.SetClipName(valueNode.Value);
                                        loadClipName = true;
                                        break;
                                    case "Loop":
                                        isLoop = bool.Parse(valueNode.Value);
                                        label.SetLoop(isLoop);
                                        break;
                                    case "Volume":
                                        label.volume = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "CategoryBehavior":
                                        if (AudioLabelSettings.BEHAVIOR.STEAL_OLDEST.ToString().CompareTo(valueNode.Value) == 0)
                                        {
                                            label.maxPlaybacksBehavior = AudioLabelSettings.BEHAVIOR.STEAL_OLDEST;
                                        }
                                        else if (AudioLabelSettings.BEHAVIOR.JUST_FAIL.ToString().CompareTo(valueNode.Value) == 0)
                                        {
                                            label.maxPlaybacksBehavior = AudioLabelSettings.BEHAVIOR.JUST_FAIL;
                                        }
                                        else if (AudioLabelSettings.BEHAVIOR.QUEUE.ToString().CompareTo(valueNode.Value) == 0)
                                        {
                                            label.maxPlaybacksBehavior = AudioLabelSettings.BEHAVIOR.QUEUE;
                                        }
                                        break;
                                    case "Priority":
                                        label.priority = int.Parse(valueNode.Value);
                                        break;
                                    case "CategoryName":
                                        {
                                            label.categoryName = valueNode.Value;
                                            AudioCategorySettings tmpCat;
                                            if (categoryDict.TryGetValue(label.GetCategoryName(), out tmpCat))
                                            {
                                                label.SetAttachCategoryInstance(tmpCat);
                                            }
                                            else
                                            {
#if USND_DEBUG_LOG
                                                AudioDebugLog.LogWarning("カテゴリ[" + label.GetCategoryName() + "]が見つからなかったので" + label.name + "を登録しませんでした。");
#endif
                                                error = true;
#if UNITY_EDITOR
                                                if (IsActiveTool)
                                                    AddLog("<color=red>    カテゴリ[" + label.GetCategoryName() + "]が見つからなかったので" + label.name + "を登録しませんでした。</color>");
#endif
                                                continue;
                                            }
                                        }
                                        break;
                                    case "SingleGroup":
                                        label.singleGroup = valueNode.Value;
                                        break;
                                    case "MaxNum":
                                        label.maxPlaybacksNum = int.Parse(valueNode.Value);
                                        break;
                                    case "IsStealOldest":
                                        label.isStealOldest = bool.Parse(valueNode.Value);
                                        break;
                                    case "UnityMixerName":
                                        label.unityMixerName = valueNode.Value;
                                        break;
                                    case "SpatialGroup":
                                        label.spatialGroup = valueNode.Value;
                                        break;
                                    case "Delay":
                                        label.playStartDelay = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "Interval":
                                        label.playInterval = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "Pan":
                                        label.pan = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "Pitch":
                                        label.pitchShiftCent = int.Parse(valueNode.Value);
                                        break;
                                    case "IsLastSamples":
                                        label.isPlayLastSamples = bool.Parse(valueNode.Value);
                                        break;
                                    case "FadeInTime":
                                        label.fadeInTime = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "FadeOutTime":
                                        label.fadeOutTime = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "FadeInOldSample":
                                        label.fadeInTimeOldSamples = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "FadeOutOnPause":
                                        label.fadeOutTimeOnPause = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "FadeInOffPause":
                                        label.fadeInTimeOffPause = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "IsVolRnd":
                                        label.isVolumeRandom = bool.Parse(valueNode.Value);
                                        break;
                                    case "IncVol":
                                        label.inconsecutiveVolume = bool.Parse(valueNode.Value);
                                        break;
                                    case "VolRndMin":
                                        label.volumeRandomMin = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "VolRndMax":
                                        label.volumeRandomMax = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "VolRndUnit":
                                        label.volumeRandomUnit = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "IsPitchRnd":
                                        label.isPitchRandom = bool.Parse(valueNode.Value);
                                        break;
                                    case "IncPitch":
                                        label.inconsecutivePitch = bool.Parse(valueNode.Value);
                                        break;
                                    case "PitchRndMin":
                                        label.pitchRandomMin = int.Parse(valueNode.Value);
                                        break;
                                    case "PitchRndMax":
                                        label.pitchRandomMax = int.Parse(valueNode.Value);
                                        break;
                                    case "PitchRndUnit":
                                        label.pitchRandomUnit = int.Parse(valueNode.Value);
                                        break;
                                    case "IsPanRnd":
                                        label.isPanRandom = bool.Parse(valueNode.Value);
                                        break;
                                    case "IncPan":
                                        label.inconsecutivePan = bool.Parse(valueNode.Value);
                                        break;
                                    case "PanRndMin":
                                        label.panRandomMin = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "PanRndMax":
                                        label.panRandomMax = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "PanRndUnit":
                                        label.panRandomUnit = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "IsRndSrc":
                                        label.isRandomPlay = bool.Parse(valueNode.Value);
                                        break;
                                    case "IncSrc":
                                        label.inconsecutiveSource = bool.Parse(valueNode.Value);
                                        break;
                                    case "RndSrc":
                                        // 改行でパースしてstringをリストにする
                                        // ランダムソースの参照更新はまとめてあとでやる
                                        if (valueNode.Value != null)
                                        {
                                            string str = valueNode.Value;
                                            str.Replace("\r\n", "\n");
                                            str.Replace("\r", "\n");
                                            string[] str2 = str.Split("\n"[0]);
                                            label.randomSource = new string[str2.Length];
                                            for (int k = 0; k < str2.Length; ++k)
                                            {
                                                label.randomSource[k] = str2[k];
                                                //label.randomSource.Add(str2[k]);
                                            }
                                        }
                                        break;
                                    case "IsMovePitch":
                                        label.isMovePitch = bool.Parse(valueNode.Value);
                                        break;
                                    case "PitchStart":
                                        label.pitchStart = int.Parse(valueNode.Value);
                                        break;
                                    case "PitchEnd":
                                        label.pitchEnd = int.Parse(valueNode.Value);
                                        break;
                                    case "PitchMoveTime":
                                        label.pitchMoveTime = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "IsMovePan":
                                        label.isMovePan = bool.Parse(valueNode.Value);
                                        break;
                                    case "PanStart":
                                        label.panStart = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "PanEnd":
                                        label.panEnd = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "PanMoveTime":
                                        label.panMoveTime = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "DuckingCategory":
                                        // 改行でパースしてstringのリストを作る
                                        if (valueNode.Value != null)
                                        {
                                            string str = valueNode.Value;
                                            str.Replace("\r\n", "\n");
                                            str.Replace("\r", "\n");
                                            string[] str2 = str.Split("\n"[0]);
                                            label.duckingCategories = new string[str2.Length];
                                            for (int k = 0; k < str2.Length; ++k)
                                            {
                                                label.duckingCategories[k] = str2[k];
                                            }
                                        }
                                        break;
                                    case "DuckStart":
                                        label.duckingStartTime = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "DuckEnd":
                                        label.duckingEndTime = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "DuckVol":
                                        label.duckingVolumeFactor = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "AutoRestore":
                                        label.autoRestoreDucking = bool.Parse(valueNode.Value);
                                        break;
                                    case "RestoreTime":
                                        label.restoreTime = float.Parse(valueNode.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        break;
                                    case "IsAndroidNative":
                                        label.isAndroidNative = bool.Parse(valueNode.Value);
                                        break;
                                }
                            }
                        }
                    }

                    // エラーがあるか名前がロードされてなかったらロード失敗
                    if (error == true || loadClipName == false)
                    {
#if UNITY_EDITOR
                        if (IsActiveTool && loadClipName == false)
                            AddLog("<color=red>    " + label.name + "のAudioClip名が設定されていないので登録できませんでした。</color>");
#endif
                        deleteAudioSource(player);
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (label.isVolumeRandom && label.volumeRandomMin == label.volumeRandomMax)
                            AddLog("<color=red>" + label.name + ":ランダムボリュームの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                        if (label.isVolumeRandom && label.volumeRandomMin == label.volumeRandomMax)
                            AudioDebugLog.LogWarning(label.name + ":ランダムボリュームの最大値と最小値が同じ値です.");
#endif
#if UNITY_EDITOR
                        if (label.isPitchRandom && label.pitchRandomMin == label.pitchRandomMax)
                            AddLog("<color=red>" + label.name + ":ランダムピッチの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                        if (label.isPitchRandom && label.pitchRandomMin == label.pitchRandomMax)
                            AudioDebugLog.LogWarning(label.name + ":ランダムピッチの最大値と最小値が同じ値です.");
#endif
#if UNITY_EDITOR
                        if (label.isPanRandom && label.panRandomMin == label.panRandomMax)
                            AddLog("<color=red>" + label.name + ":ランダムパンの最大値と最小値が同じ値です.</color>");
#endif
#if USND_DEBUG_LOG
                        if (label.isPanRandom && label.panRandomMin == label.panRandomMax)
                            AudioDebugLog.LogWarning(label.name + ":ランダムパンの最大値と最小値が同じ値です.");
#endif

                        if (newCreate == true)
                        {
                            sourceDict.Add(player.PlayerName, player);
                            if (player.Init(clip, label.GetClipName(), label, sourceDict) == false)
                            {
#if USND_DEBUG_LOG
                                AudioDebugLog.LogWarning(player.PlayerName + "の初期化に失敗");
#endif
                            }
                        }
                        setUnityAudioMixer(player);
                    }

                    // 30件に1回休む
                    if ((i % 30) == 0)
                    {
                        yield return null;
                    }
                }
                updateRandomSourceInfoAll();
            }
            loadXmlStatus = AudioDefine.LOAD_XML_STATUS.FINISH;
        }

        // ---------------------------------------------------------------
        // カテゴリごとの再生インスタンスリストを整理
        void orderCategoryInstanceList(List<int> playerList)
        {
            for (int i = 0; i < playerList.Count; )
            {
                int instance = playerList[i];
                AudioPlayer playObj;
                if (playAudioDict.TryGetValue(instance, out playObj))
                {
                    if (playObj != null)
                    {
                        AudioDefine.INSTANCE_STATUS status = playObj.GetInstanceStatus(instance);
                        // 停止済み、停止予定なら再生中リストから消してもよい.
                        if (status == AudioDefine.INSTANCE_STATUS.STOP || status == AudioDefine.INSTANCE_STATUS.STOP_SOON)
                        {
                            playerList.RemoveAt(i);
                        }
                        else
                        {
                            ++i;
                        }
                    }
                    else
                    {
                        ++i;
                    }
                }
                else
                {
                    playerList.RemoveAt(i);
                }
            }
        }

        // ---------------------------------------------------------------
        // 再生インスタンス情報をディクショナリ、リストに登録
        void addPlayInfo(AudioPlayer player, int instanceId)
        {
            playAudioDict.Add(instanceId, player);
			// hashにも登録
			playerHashSet.Add(player);

			string categoryId = player.GetCategoryName();
            List<int> playerList;
            if (playCategoryDict.TryGetValue(categoryId, out playerList))
            {
                playerList.Add(instanceId);
            }
        }

        // ---------------------------------------------------------------
        // 同一グループのものがなっていたら止める
        void stopSameSingleGroup(string singleGroup, string playLabelName)
        {
            if ( singleGroup == null )
            {
                return;
            }

            foreach(KeyValuePair<int, AudioPlayer> pair in playAudioDict)
            {
                AudioPlayer player = pair.Value;
                if (player != null)
                {
                    if (playLabelName.CompareTo(pair.Value.GetLabelSettings().name) != 0)
                    {
                        if (singleGroup.CompareTo(player.GetLabelSettings().singleGroup) == 0)
                        {
                            player.Stop(pair.Key);
#if UNITY_EDITOR
                            if (IsActiveTool)
                                AddLog("<color=magenta>Stop Same SingleGroup instance:" + pair.Key + "(" + player.GetLabelSettings().name + ") group:" + singleGroup + ".</color>");
#endif
                        }
                    }
                }
            }
        }

        // ---------------------------------------------------------------
        // ラベルごとの発音数をチェックして再生してよいか確認する.
        RESULT checkLabelPlaybacksNum(AudioPlayer player)
        {
            // ラベルの発音数をチェック
            // AudioLabelSettingsに沿って発音数制御.
            if (player.GetMaxPlaybacksNum() > 0)
            {
                if (player.GetPlayingNum() + 1 > player.GetMaxPlaybacksNum())
                {
                    // 古いデータを消す設定なら停止する
                    if (player.IsStealOldest())
                    {
#if USND_DEBUG_LOG
                        AudioDebugLog.Log(player.PlayerName + "の古いインスタンスを停止");
#endif
#if UNITY_EDITOR
                        if (IsActiveTool)
                            AddLog("<color=green>Play Error! " + player.PlayerName + "の古いインスタンスを停止.</color>");
#endif
                        player.StopOldInstance();
                        return RESULT.EXECUTE;
                    }
                    else
                    {
                        // ラベル先発優先のため鳴らさずに終了.
                        return RESULT.FINISH;
                    }
                }
            }
            return RESULT.CONTINUE;
        }

        // ---------------------------------------------------------------
        // 同カテゴリ所属のラベルの発音数をチェックして再生してよいか確認する.
        RESULT checkCategoryPlaybacksNum(AudioPlayer player, ref float time, ref bool queueOn)
        {
            string categoryId = player.GetCategoryName();

            List<int> playerList;
            if (!playCategoryDict.TryGetValue(categoryId, out playerList))
            {
#if USND_DEBUG_LOG
                AudioDebugLog.Log("カテゴリ名:" + categoryId + "が見つかりませんでした。");
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=green>カテゴリ名:" + categoryId + "が見つかりませんでした。</color>");
#endif
                return RESULT.FINISH;
            }
            // 再生したいラベルの所属するカテゴリの状態をチェックする.
            AudioCategorySettings category = categoryDict[categoryId];

            // 停止済みがないかチェックして並び替えしておく.
            orderCategoryInstanceList(playerList);
            int count = playerList.Count;

            queueOn = false;

            // count+1が最大発音数を越える.
            if (count + 1 > category.maxPlaybacksNum)
            {
                if (player.GetMaxPlaybacksBehavior() == AudioLabelSettings.BEHAVIOR.JUST_FAIL)
                {
                    // 先発優先なので終了.
                    return RESULT.FINISH;
                }
                else
                {
                    // プライオリティの前に同じカテゴリ内にSingleGroupが一致するものがないかチェック、一致したらそれを優先で止める.
                    if (player.GetLabelSettings().singleGroup != null)
                    {
                        for (int i = 0; i < playerList.Count; ++i)
                        {
                            int instance = playerList[i];
                            AudioPlayer targetPlayer = playAudioDict[instance];

                            if (targetPlayer != null)
                            {
                                // SingleGroupが一致する.
                                if (player.GetLabelSettings().singleGroup.CompareTo(targetPlayer.GetLabelSettings().singleGroup) == 0)
                                {
                                    targetPlayer.Stop(instance);
#if UNITY_EDITOR
                                    if (IsActiveTool)
                                        AddLog("<color=green>シングルグループ名:" + player.GetLabelSettings().singleGroup + "の古いインスタンスを停止.</color>");
#endif
                                    // 停止したので再生できる.
                                    return RESULT.EXECUTE;
                                }
                            }
                        }
                    }


                    // 再生したいラベルと再生中のラベルのプライオリティを比較して、再生したいラベルが優位なものを止める.
                    for (int i = 0; i < playerList.Count; ++i)
                    {
                        int instance = playerList[i];
                        AudioPlayer targetPlayer = playAudioDict[instance];

                        if (targetPlayer != null)
                        {
                            // プライオリティの数値が小さいほうが優先度が高い
                            if (player.GetPriority() <= targetPlayer.GetPriority())
                            {
                                targetPlayer.Stop(instance);
                                if (player.GetMaxPlaybacksBehavior() == AudioLabelSettings.BEHAVIOR.QUEUE)
                                {
                                    time = targetPlayer.GetFadeOutTime();
                                    queueOn = true;
                                }
#if UNITY_EDITOR
                                if (IsActiveTool)
                                {
                                    AddLog("<color=green>プライオリティの低いインスタンスを停止.</color>");
                                }
#endif
                                // 停止したので再生できる.
                                return RESULT.EXECUTE;
                            }
                        }
                    }
                }
            }
            else
            {
                // あふれないので再生できる
                return RESULT.EXECUTE;
            }

            // ここまできたらポートなし、止められなかったということなので再生できない.
            return RESULT.FINISH;
        }


        // ---------------------------------------------------------------
        // ダッキング開始
        void startDucking(AudioPlayer player, int instanceId)
        {
            AudioLabelSettings labelSetting = player.GetLabelSettings();

            //for (int i = 0; i < labelSetting.duckingCategories.Count; ++i)
            if (labelSetting.duckingCategories != null)
            {
                for (int i = 0; i < labelSetting.duckingCategories.Length; ++i)
                {
                    string categoryName = labelSetting.duckingCategories[i];
                    List<string> triggerList;
                    if (playDuckingTrigger.TryGetValue(categoryName, out triggerList))
                    {
                        if (labelSetting.autoRestoreDucking)
                        {
                            if (!triggerList.Contains(player.PlayerName))
                            {
                                triggerList.Add(player.PlayerName);
                            }
                        }
                        AudioCategorySettings categoryInstance = categoryDict[categoryName];
                        categoryInstance.SetDuckingVolumeUpdater(labelSetting.duckingVolumeFactor, labelSetting.duckingStartTime, true);
#if UNITY_EDITOR
                        if (IsActiveTool)
                            AddLog("<color=cyan>Start Ducking category:" + categoryName + " .</color>");
#endif
                    }
                }
            }
        }

        // ---------------------------------------------------------------
        // 手動でダッキングをかける
        public void setDucking(string categoryName, float targetVolumeFactor, float fadeTime)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetDucking categoryName: " + categoryName + " vol: " + targetVolumeFactor + " time:" + fadeTime + "ms.</color>");
#endif
            AudioCategorySettings category;
            if (categoryDict.TryGetValue(categoryName, out category))
            {
                category.SetDuckingVolumeUpdater(targetVolumeFactor, fadeTime, true);
            }
            else
            {
#if USND_DEBUG_LOG
                AudioDebugLog.Log("not found category;" + categoryName);
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>       " + categoryName + " not found.</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // 手動でダッキング解除
        public void resetDucking(string categoryName, float fadeTime)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>ResetDucking categoryName: " + categoryName + " time:" + fadeTime + "ms.</color>");
#endif
            AudioCategorySettings category;
            if (categoryDict.TryGetValue(categoryName, out category))
            {
                category.SetDuckingVolumeUpdater(1, fadeTime, false);
            }
            else
            {
#if USND_DEBUG_LOG
                AudioDebugLog.Log("not found category;" + categoryName);
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>       " + categoryName + " not found.</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // 手動で全カテゴリのダッキング解除
        public void resetDuckingAll(float fadeTime)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>ResetDuckingAll time:" + fadeTime + "ms.</color>");
#endif
            foreach (KeyValuePair<string, AudioCategorySettings> categoryValue in categoryDict)
            {
                categoryValue.Value.SetDuckingVolumeUpdater(1, fadeTime, false);
            }
        }


        // ---------------------------------------------------------------
        // 手動でダッキング解除
        public void forceResetDucking(string categoryName, float fadeTime)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>ResetDucking categoryName: " + categoryName + " time:" + fadeTime + "ms.</color>");
#endif
            AudioCategorySettings category;
            if (categoryDict.TryGetValue(categoryName, out category))
            {
                category.SetDuckingVolumeUpdater(1, fadeTime, false);
            }
            else
            {
#if USND_DEBUG_LOG
                AudioDebugLog.Log("not found category;" + categoryName);
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>       " + categoryName + " not found.</color>");
#endif
            }

            // 強制削除
            List<string> triggerList;
            if (playDuckingTrigger.TryGetValue(categoryName, out triggerList))
            {
                triggerList.Clear();
            }
        }

        // ---------------------------------------------------------------
        // 手動で全カテゴリのダッキング解除
        public void forceResetDuckingAll(float fadeTime)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>ResetDuckingAll time:" + fadeTime + "ms.</color>");
#endif
            foreach (KeyValuePair<string, AudioCategorySettings> categoryValue in categoryDict)
            {
                categoryValue.Value.SetDuckingVolumeUpdater(1, fadeTime, false);

                // 強制削除
                List<string> triggerList;
                if (playDuckingTrigger.TryGetValue(categoryValue.Value.categoryName, out triggerList))
                {
                    triggerList.Clear();
                }
            }
        }

        // ---------------------------------------------------------------
        // Play
        public int play(string labelName, float delay = -1)
        {
#if USND_OUTPUT_CALL_LOG
#if UNITY_EDITOR
            AddCallLog(labelName);
#endif
#endif
            return playOption(labelName, AudioDefine.DEFAULT_VOLUME, AudioDefine.DEFAULT_FADE, AudioDefine.DEFAULT_PAN, AudioDefine.DEFAULT_PITCH, delay);
        }


        int prepareInstance(string labelName, float volume, float fadeTime, float pan, int pitch, float delay, ref AudioPlayer player, ref float time, ref bool queueOn, bool isForce2D)
        {
            int instanceId = AudioDefine.INSTANCE_ID_ERROR;

            if (!sourceDict.TryGetValue(labelName, out player))
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning(labelName + "は登録されていません。");
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Play Error! " + labelName + "は登録されていません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }
            if (player == null)
            {
#if USND_DEBUG_LOG
                AudioDebugLog.LogWarning(labelName + "のプレイヤー情報がありません。");
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Play Error! " + labelName + "のプレイヤー情報がありません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }

            // インターバルチェック
            if ( !player.IsPlayInterval() )
            {
#if USND_DEBUG_LOG
                AudioDebugLog.Log(labelName + "の再生インターバルにより再生できません。");
#endif
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Play Error! " + labelName + "の再生インターバルにより再生できません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }

            // AudioClip登録済みでなければ探してセット
            if (!player.IsSetPlayClip())
            {
                AudioLabelSettings label = player.GetLabelSettings();
                AudioClip tmpClip;
                if (audioClipDict.TryGetValue(label.GetClipName(), out tmpClip))
                {
                    player.SetPlayClip(audioClipDict[label.GetClipName()]);
                    updateRandomSourceInfo(labelName);
                }
            }

            // 3Dサウンド設定がされてなかったら設定する
            if (!player.IsSetSpatialGroup())
            {
                string name = player.GetSpatialGroup();
                if (name != null)
                {
                    Audio3DSettings d3set;
                    if (audio3DSettings.TryGetValue(name, out d3set))
                    {
                        player.SetAudio3DSettings(d3set);
                    }
                }
            }

            RESULT ret;
            // -----------------------------------------------------------------.
            // ラベルの発音数をチェック
            // AudioLabelSettingsに沿って発音数制御.
            if (player.GetMaxPlaybacksNum() > 0)
            {
                ret = checkLabelPlaybacksNum(player);
                if (ret == RESULT.EXECUTE)
                {
                    instanceId = player.Prepare(volume, fadeTime, pan, pitch, isForce2D);
                    if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
                    {
                        //addPlayInfo(player, instanceId);
                        //startDucking(player, instanceId);
                    }
                    return instanceId;
                }
                else if (ret == RESULT.FINISH)
                {
#if USND_DEBUG_LOG
                    // ラベル先発優先のため鳴らさずに終了.
                    AudioDebugLog.Log(labelName + "はラベル優先制御により再生できません。");
#endif
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>Play Error! " + labelName + "はラベル優先制御により再生できません.</color>");
#endif
                    return AudioDefine.INSTANCE_ID_ERROR;
                }
            }
            // -----------------------------------------------------------------.


            // -----------------------------------------------------------------.
            // カテゴリの発音数をチェック.
            if (player.GetCategoryMaxPlaybacksNum() > 0)
            {
                ret = checkCategoryPlaybacksNum(player, ref time, ref queueOn);
                if (ret == RESULT.CONTINUE || ret == RESULT.FINISH)
                {
#if USND_DEBUG_LOG
                    // ポートが準備できなかったので終了.
                    AudioDebugLog.Log(labelName + "はカテゴリ優先制御により再生できません。");
#endif
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>Play Error! " + labelName + "はカテゴリ優先制御により再生できません.</color>");
#endif
                    return AudioDefine.INSTANCE_ID_ERROR;
                }
            }

            // ポートの準備はできたはずなので再生準備する.
            return player.Prepare(volume, fadeTime, pan, pitch, isForce2D);
        }


        // ---------------------------------------------------------------
        // PlayOption
        public int playOption(string labelName, float volume, float fadeTime, float pan, int pitch, float delay)
        {
            int instanceId = AudioDefine.INSTANCE_ID_ERROR;
            // 2016.01.27 prepareOptionと同じ処理に統一、prepareInstanceを追加
            /*
            if (!sourceDict.ContainsKey(labelName))
            {
                AudioDebugLog.Log(labelName + "は登録されていません。");
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Play Error! " + labelName + "は登録されていません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }
            AudioPlayer player = sourceDict[labelName];
            if (player == null)
            {
                AudioDebugLog.Log(labelName + "のプレイヤー情報がありません。");
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Play Error! " + labelName + "のプレイヤー情報がありません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }

            // 3Dサウンド設定がされてなかったら設定する
            if ( !player.IsSetSpatialGroup() )
            {
                string name = player.GetSpatialGroup();
                if ( name != null)
                {
                    if ( audio3DSettings.ContainsKey(name))
                    {
                        player.SetAudio3DSettings(audio3DSettings[name]);
                    }
                }
            }

            RESULT ret;
            // -----------------------------------------------------------------.
            // ラベルの発音数をチェック
            // AudioLabelSettingsに沿って発音数制御.
            if (player.GetMaxPlaybacksNum() > 0)
            {
                ret = checkLabelPlaybacksNum(player);
                if (ret == RESULT.EXECUTE)
                {
                    instanceId = player.Play(volume, fadeTime, pan, pitch, delay);
                    if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
                    {
                        addPlayInfo(player, instanceId);
                        startDucking(player, instanceId);
                    }
#if UNITY_EDITOR
                    if (IsActiveTool)
                        SetLabelInfoList(labelName, instanceId, player.GetCurrentVolume(instanceId));
                    if (IsActiveTool)
                        AddLog("<color=green>Play name:" + labelName + " instance:" + instanceId + " vol:" + volume + " fade:" + fadeTime + "ms pan:" + pan + " pitch:" + pitch + " delay:" + delay + "ms.</color>");
#endif
                    return instanceId;
                }
                else if (ret == RESULT.FINISH)
                {
                    // ラベル先発優先のため鳴らさずに終了.
                    AudioDebugLog.Log(labelName + "はラベル優先制御により再生できません。");
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>Play Error! " + labelName + "はラベル優先制御により再生できません.</color>");
#endif
                    return AudioDefine.INSTANCE_ID_ERROR;
                }
            }
            // -----------------------------------------------------------------.


            // -----------------------------------------------------------------.
            // カテゴリの発音数をチェック.
            float time = 0;
            bool queueOn = false;
            if (player.GetCategoryMaxPlaybacksNum() > 0)
            {
                ret = checkCategoryPlaybacksNum(player, ref time, ref queueOn);
                if (ret == RESULT.CONTINUE || ret == RESULT.FINISH)
                {
                    // ポートが準備できなかったので終了.
                    AudioDebugLog.Log(labelName + "はカテゴリ優先制御により再生できません。");
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=red>Play Error! " + labelName + "はカテゴリ優先制御により再生できません.</color>");
#endif
                    return AudioDefine.INSTANCE_ID_ERROR;
                }
            }

            // ポートの準備はできたはずなので再生する.
            instanceId = player.Play(volume, fadeTime, pan, pitch, ((queueOn == true) ? time : delay));
             */
            float time = 0;
            bool queueOn = false;
            AudioPlayer player = null;

            instanceId = prepareInstance(labelName, volume, fadeTime, pan, pitch, delay, ref player, ref time, ref queueOn, false);
            if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
            {
                // エラーじゃないので同一グループの設定があったら、同じグループのものを止める.
                // この時点でカテゴリの中に再生中の同一グループのラベルが存在していたらそれを止めて準備済み.
                stopSameSingleGroup(player.GetLabelSettings().singleGroup, labelName);


                addPlayInfo(player, instanceId);
                startDucking(player, instanceId);
#if UNITY_EDITOR
                if (IsActiveTool)
                    SetLabelInfoList(labelName, instanceId, player.GetCurrentVolume(instanceId));
                if (IsActiveTool)
                    AddLog("<color=green>Play name:" + labelName + " instance:" + instanceId + " vol:" + volume + " fade:" + fadeTime + "ms pan:" + pan + " pitch:" + pitch + " delay:" + delay + "ms.</color>");
#endif
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Play Error! " + labelName + "は再生されませんでした</color>");
#endif
            }

            playInstance(instanceId, ((queueOn == true) ? time : delay));

            return instanceId;
        }


        // ---------------------------------------------------------------
        // Prepare
        public int prepare(string labelName, bool isForce2D = false)
        {
#if USND_OUTPUT_CALL_LOG
#if UNITY_EDITOR
            AddCallLog(labelName);
#endif
#endif
            return prepareOption(labelName, AudioDefine.DEFAULT_VOLUME, AudioDefine.DEFAULT_FADE, AudioDefine.DEFAULT_PAN, AudioDefine.DEFAULT_PITCH, isForce2D);
        }

		// ---------------------------------------------------------------
		// Prepare Option
		public int prepare(string labelName, float volume, float fadeTime, float pan, int pitch, bool isForce2D = false)
		{
#if USND_OUTPUT_CALL_LOG
#if UNITY_EDITOR
			AddCallLog(labelName);
#endif
#endif
			return prepareOption(labelName, volume, fadeTime, pan, pitch, isForce2D);
		}

		// ---------------------------------------------------------------
		// Prepare
		public int prepareOption(string labelName, float volume, float fadeTime, float pan, int pitch, bool isForce2D)
        {
            int instanceId = AudioDefine.INSTANCE_ID_ERROR;

            // 2016.01.27 prepareOptionと同じ処理に統一、prepareInstanceを追加
            /*
            if (!sourceDict.ContainsKey(labelName))
            {
                AudioDebugLog.Log(labelName + "は登録されていません。");
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Play Error! " + labelName + "は登録されていません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }
            AudioPlayer player = sourceDict[labelName];
            if (player == null)
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Prepare Error! " + labelName + "は登録されていません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }

            // 3Dサウンド設定がされてなかったら設定する
            if (!player.IsSetSpatialGroup())
            {
                string name = player.GetSpatialGroup();
                if (name != null)
                {
                    if (audio3DSettings.ContainsKey(name))
                    {
                        player.SetAudio3DSettings(audio3DSettings[name]);
                    }
                }
            }

            // -----------------------------------------------------------------.
            // ラベルの発音数をチェック
            // AudioLabelSettingsに沿って発音数制御.
            RESULT ret = checkLabelPlaybacksNum(player);
            if (ret == RESULT.EXECUTE)
            {
                instanceId = player.Prepare(volume, fadeTime, pan, pitch);
                if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
                {
                    addPlayInfo(player, instanceId);
                }
#if UNITY_EDITOR
                if (IsActiveTool)
                    SetLabelInfoList(labelName, instanceId, player.GetCurrentVolume(instanceId));
                if (IsActiveTool)
                    AddLog("<color=green>Prepare name:" + labelName + " instance:" + instanceId + " vol:" + volume + " fade:" + fadeTime + "ms pan:" + pan + " pitch:" + pitch + ".</color>");
#endif
                return instanceId;
            }
            else if (ret == RESULT.FINISH)
            {
                // ラベル先発優先のため鳴らさずに終了.
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Prepare name " + labelName + "はラベル優先制御により再生できません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }
            // -----------------------------------------------------------------.


            // -----------------------------------------------------------------.
            // カテゴリの発音数をチェック.
            float time = 0;
            bool queueOn = false;
            ret = checkCategoryPlaybacksNum(player, ref time, ref queueOn);
            if (ret == RESULT.CONTINUE)
            {
                // ポートが準備できなかったので終了.
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Prepare Error! " + labelName + "はカテゴリ優先制御により再生できません.</color>");
#endif
                return AudioDefine.INSTANCE_ID_ERROR;
            }

            // ポートの準備はできたはずなので再生する.
            instanceId = player.Prepare(volume, fadeTime, pan, pitch);
             */
            float time = 0;
            bool queueOn = false;
            AudioPlayer player = null;

            instanceId = prepareInstance(labelName, volume, fadeTime, pan, pitch, 0, ref player, ref time, ref queueOn, isForce2D);

            if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
            {
                addPlayInfo(player, instanceId);
            }
#if UNITY_EDITOR
			if (IsActiveTool)
			{
				if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
				{
					SetLabelInfoList(labelName, instanceId, player.GetCurrentVolume(instanceId));
					AddLog("<color=green>Prepare name:" + labelName + " instance:" + instanceId + " vol:" + volume + " fade:" + fadeTime + "ms pan:" + pan + " pitch:" + pitch + ".</color>");
				}
				else
				{
					AddLog("<color=red>Play Error! " + labelName + "は再生されませんでした。</color>");
				}
				//AddLog("<color=green>Prepare name:" + labelName + " instance:" + instanceId + " vol:" + volume + " fade:" + fadeTime + "ms pan:" + pan + " pitch:" + pitch + ".</color>");
			}
#endif
			return instanceId;
        }

        // ---------------------------------------------------------------
        // Prepareしたインスタンスを再生開始.
        public void playInstance(int instanceId, float delay = -1)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    // エラーじゃないので同一グループの設定があったら、同じグループのものを止める.
                    // この時点でカテゴリの中に再生中の同一グループのラベルが存在していたらそれを止めて準備済み.
                    stopSameSingleGroup(player.GetLabelSettings().singleGroup, player.GetLabelSettings().name);

                    startDucking(player, instanceId);
                    player.PlayInstance(instanceId, delay);
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=green>PlayInstance instance:" + instanceId + " delay:" + delay + "ms.</color>");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>PlayInstance Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // 追従させるオブジェクトを指定して再生.
        public int play3D(string labelName, GameObject target, float delay = -1)
        {
            int instanceId = AudioDefine.INSTANCE_ID_ERROR;
            instanceId = prepare(labelName);

            if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
            {
                setTrackingObject(instanceId, target);
                playInstance(instanceId, delay);
            }
            return instanceId;
        }

        // ---------------------------------------------------------------
        // 位置を指定して再生開始.
        public int play3D(string labelName, Vector3 position, float delay = -1)
        {
            int instanceId = AudioDefine.INSTANCE_ID_ERROR;
            instanceId = prepare(labelName);

            if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
            {
                setPosition(instanceId, position);
                playInstance(instanceId, delay);
            }
            return instanceId;
        }
        
        // ---------------------------------------------------------------
        // 追従させるオブジェクトを指定して再生.
        public int play3D(string labelName, Transform target, float delay = -1)
        {
            int instanceId = AudioDefine.INSTANCE_ID_ERROR;
            instanceId = prepare(labelName);

            if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
            {
                setTrackingObject(instanceId, target);
                playInstance(instanceId, delay);
            }
            return instanceId;
        }

        // ---------------------------------------------------------------
        // 3Dサウンド設定のあるラベルを強制的に2Dで再生
        public int play2D(string labelName, float delay = -1)
        {
            int instanceId = AudioDefine.INSTANCE_ID_ERROR;
            instanceId = prepare(labelName, true);

            if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
            {
                playInstance(instanceId, delay);
            }
            return instanceId;
        }

		// ---------------------------------------------------------------
		// 3Dサウンド設定のあるラベルを強制的に2Dで再生
		public int play2D(string labelName, float volume, float fadeTime, float pan, int pitch, float delay = -1)
		{
			int instanceId = AudioDefine.INSTANCE_ID_ERROR;
			instanceId = prepare(labelName, volume, fadeTime, pan, pitch, true);

			if (instanceId != AudioDefine.INSTANCE_ID_ERROR)
			{
				playInstance(instanceId, delay);
			}
			return instanceId;
		}

		// ---------------------------------------------------------------
		// インスタンスのポジションを追従させるオブジェクトを指定(3Dサウンドのみ有効).
		public void setTrackingObject(int instanceId, GameObject target)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.SetTrackingObject(instanceId, target);
                }
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=green>SetTrackingObject instance:" + instanceId + "</color>");
#endif
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetTrackingObject Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }
        
        // ---------------------------------------------------------------
        // インスタンスのポジションを追従させるオブジェクトを指定(3Dサウンドのみ有効).
        public void setTrackingObject(int instanceId, Transform target)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.SetTrackingObject(instanceId, target);
                }
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=green>SetTrackingObject instance:" + instanceId + "</color>");
#endif
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetTrackingObject Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // 停止.
        public void stop(int instanceId, float fadeTime = -1)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.Stop(instanceId, fadeTime);
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=magenta>Stop instance:" + instanceId + " fadeTime:" + fadeTime + "ms.</color>");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>Stop Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // 停止.
        public void stopLabel(string labelName, float fadeTime = -1)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                if (player != null)
                {
                    player.StopAll(fadeTime);
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=magenta>StopLabel name:" + labelName + " fadeTime:" + fadeTime + "ms.</color>");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>StopLabel Error! name:" + labelName + " not found.</color>");
#endif
            }
        }

        // ----------------------------------------------------------------------
        // 全インスタンス停止.
        public void stopAll(float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>StopAll fadeTime:" + fadeTime + "ms.</color>");
#endif
            foreach (KeyValuePair<int, AudioPlayer> playValue in playAudioDict)
            {
                if (playValue.Value != null)
                {
                    playValue.Value.Stop(playValue.Key, fadeTime);
                }
            }
        }

        // ----------------------------------------------------------------------
        // ポーズ設定.
        public void onPause(int instanceId, float fadeTime = -1)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.OnPause(instanceId, fadeTime);
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=magenta>OnPause instance:" + instanceId + " fadeTime:" + fadeTime + "ms.</color>");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>OnPause Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ----------------------------------------------------------------------
        // 全インスタンスポーズ設定.
        public void onPauseAll(float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OnPauseAll fadeTime:" + fadeTime + "ms.</color>");
#endif
            foreach (KeyValuePair<int, AudioPlayer> playValue in playAudioDict)
            {
                if (playValue.Value != null)
                {
                    playValue.Value.OnPause(playValue.Key, fadeTime);
                }
            }
        }

        // ----------------------------------------------------------------------
        // ポーズ解除.
        public void offPause(int instanceId, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OffPause instance:" + instanceId + " fadeTime:" + fadeTime + "ms.</color>");
#endif
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.OffPause(instanceId, fadeTime);
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>OffPause Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ----------------------------------------------------------------------
        // 全インスタンスポーズ解除.
        public void offPauseAll(float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OffPauseAll fadeTime:" + fadeTime + "ms.</color>");
#endif
            foreach (KeyValuePair<int, AudioPlayer> playValue in playAudioDict)
            {
                if (playValue.Value != null)
                {
                    playValue.Value.OffPause(playValue.Key, fadeTime);
                }
            }
        }

        // ----------------------------------------------------------------------
        // ボリューム設定.
        public void setVolume(int instanceId, float newVolume, float moveTime)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.SetVolume(instanceId, newVolume, moveTime);
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=cyan>SetVolume instance:" + instanceId + " vol:" + newVolume + " moveTime:" + moveTime + "ms current_vol:" + player.GetCurrentVolume(instanceId) + ".</color>");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetVolume Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ----------------------------------------------------------------------
        // ラベルの再生中インスタンスを一括でボリューム設定
        public void setVolume(string labelName, float newVolume, float moveTime)
        {
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>SetVolume Error! name:" + labelName + " not found.</color>");
#endif
                return;
            }
            player.SetVolumeAll(newVolume, moveTime);
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetVolume name:" + labelName + " vol:" + newVolume + " moveTime:" + moveTime + "ms.</color>");
#endif
        }

        // ----------------------------------------------------------------------
        // ピッチ設定.
        public void setPitch(int instanceId, int newPitch, float moveTime)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.SetPitch(instanceId, newPitch, moveTime);
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=cyan>SetPitch instance:" + instanceId + " pitch:" + newPitch + " moveTime:" + moveTime + "ms.</color>");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetPitch Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ----------------------------------------------------------------------
        // ラベルの再生中インスタンスを一括でピッチ設定
        public void setPitch(string labelName, int newPitch, float moveTime)
        {
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetPitch Error! name:" + labelName + " not found.</color>");
#endif
                return;
            }
            player.SetPitchAll(newPitch, moveTime);
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetPitch name:" + labelName + " pitch:" + newPitch + " moveTime:" + moveTime + "ms.</color>");
#endif
        }

        // ----------------------------------------------------------------------
        // パン設定.
        public void setPan(int instanceId, float newPan, float moveTime)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.SetPan(instanceId, newPan, moveTime);
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=cyan>SetPan instance:" + instanceId + " pan:" + newPan + " moveTime:" + moveTime + "ms.</color>");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetPan Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ----------------------------------------------------------------------
        // ラベルの再生中インスタンスを一括でピッチ設定
        public void setPan(string labelName, float newPan, float moveTime)
        {
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetPan Error! name:" + labelName + " not found.</color>");
#endif
                return;
            }
            player.SetPanAll(newPan, moveTime);
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetPan name:" + labelName + " pan:" + newPan + " moveTime:" + moveTime + "ms.</color>");
#endif
        }

        // ----------------------------------------------------------------------
        // 位置を変更(SpatialBlendが0以外のときに有効).
        public void setPosition(int instanceId, Vector3 position)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.SetPosition(instanceId, position);
#if UNITY_EDITOR
                    if (IsActiveTool)
                        AddLog("<color=cyan>SetPosition instance:" + instanceId + " x:" + position.x + " y:" + position.y + " z:" + position.z + ".</color>");
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetPosition Error! instance:" + instanceId + " not found.</color>");
#endif
            }
        }

        // ----------------------------------------------------------------------
        // 位置を変更(SpatialBlendが0以外のときに有効).
        public void setPosition(string labelName, Vector3 position)
        {
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetPosition Error! name:" + labelName + " not found.</color>");
#endif
                return;
            }
            player.SetPositionAll(position);
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetPosition name:" + labelName + " x:" + position.x + " y:" + position.y + " z:" + position.z + ".</color>");
#endif
        }

        // ----------------------------------------------------------------------
        // 再生位置をリセット.
        public void resetPlayPosition(string labelName)
        {
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>ResetPosition Error! name:" + labelName + " not found.</color>");
#endif
                return;
            }
            player.ResetPlayPosition();
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>ResetPlayPosition name:" + labelName + ".</color>");
#endif
        }

        // ---------------------------------------------------------------
        // すべての再生位置をリセット.
        public void resetPlayPositionAll()
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>ResetPlayPositionAll.</color>");
#endif
            foreach (KeyValuePair<string, AudioPlayer> sourceValue in sourceDict)
            {
                if (sourceValue.Value != null)
                {
                    sourceValue.Value.ResetPlayPosition();
                }
            }
        }

        // ---------------------------------------------------------------
        // 現在のインスタンスボリュームを取得.
        public float getInstanceVolume(int instanceId)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                return player.GetCurrentVolume(instanceId);
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // 最終的なインスタンスボリュームを取得.
        public float getInstanceCalcVolume(int instanceId)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                return player.GetCalcVolume(instanceId);
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // マスターボリュームを設定.
        public void setMasterVolume(string masterName, float volume, float moveTime = 0)
        {
            AudioMasterSettings master;
            if (masterDict.TryGetValue(masterName, out master))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>SetMasterVolume name:" + masterName + " vol:" + volume + " moveTime: " + moveTime + "ms current_vol:" + master.GetCurrentVolume() + ".</color>");
#endif
                master.SetVolumeUpdater(master.GetCurrentVolume(), volume, moveTime);

                // ボリューム更新
                if (moveTime <= 0)
                {
                    master.UpdateVolume();
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetMasterVolume Error! name:" + masterName + " not found.</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // 現在のマスターボリュームを取得.
        public float getMasterVolume(string masterName)
        {
            AudioMasterSettings master;
            if (masterDict.TryGetValue(masterName, out master))
            {
                return master.GetCurrentVolume();
            }
            return 1;
        }

        // ---------------------------------------------------------------
        // カテゴリボリュームを設定
        public void setCategoryVolume(string categoryName, float volume, float moveTime = 0)
        {
            AudioCategorySettings category;
            if (categoryDict.TryGetValue(categoryName, out category))
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=cyan>SetCategoryVolume name:" + categoryName + " vol:" + volume + " moveTime: " + moveTime + "ms current_vol:" + category.GetCurrentVolume() + ".</color>");
#endif
                category.SetVolumeUpdater(category.GetCurrentVolume(), volume, moveTime);


                // ボリューム更新
                if (moveTime <= 0)
                {
                    category.UpdateVolume();
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsActiveTool)
                    AddLog("<color=red>SetCategoryVolume Error! name:" + categoryName + " not found.</color>");
#endif
            }
        }

        // ---------------------------------------------------------------
        // 現在のカテゴリボリュームを取得.
        public float getCategoryVolume(string categoryName)
        {
            AudioCategorySettings category;
            if (categoryDict.TryGetValue(categoryName, out category))
            {
                return category.GetCurrentVolume();
            }
            return 1;
        }

        // ---------------------------------------------------------------
        // 現在のラベルボリュームを取得.
        public float getLabelVolume(string labelName)
        {
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
                return 0.0f;
            }
            return player.GetLabelSettings().volume;
        }

        // ---------------------------------------------------------------
        // マスターを指定して停止.
        public void stopMaster(string masterName, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=green>StopMaster name:" + masterName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
            // マスター名が設定されているカテゴリに対して処理する.
            foreach (KeyValuePair<string, AudioCategorySettings> categoryValue in categoryDict)
            {
                AudioCategorySettings category = categoryValue.Value;
                string master = category.masterName;
                if (master != null)
                {
                    if (masterName.CompareTo(master) == 0)
                    {
                        stopCategory(categoryValue.Key, fadeTime);
                    }
                }

            }
        }

        // ---------------------------------------------------------------
        // マスターを指定してポーズ設定.
        public void onPauseMaster(string masterName, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OnPauseMaster name:" + masterName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
            // マスター名が設定されているカテゴリに対して処理する.
            foreach (KeyValuePair<string, AudioCategorySettings> categoryValue in categoryDict)
            {
                AudioCategorySettings category = categoryValue.Value;
                string master = category.masterName;
                if (master != null)
                {
                    if (masterName.CompareTo(master) == 0)
                    {
                        onPauseCategory(categoryValue.Key, fadeTime);
                    }
                }

            }
        }


        // ---------------------------------------------------------------
        // マスターを指定してポーズ解除.
        public void offPauseMaster(string masterName, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OffPauseMaster name:" + masterName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
            // マスター名が設定されているカテゴリに対して処理する.
            foreach (KeyValuePair<string, AudioCategorySettings> categoryValue in categoryDict)
            {
                AudioCategorySettings category = categoryValue.Value;
                string master = category.masterName;
                if (master != null)
                {
                    if (masterName.CompareTo(master) == 0)
                    {
                        offPauseCategory(categoryValue.Key, fadeTime);
                    }
                }

            }
        }

        // ---------------------------------------------------------------
        // カテゴリを指定して停止.
        public void stopCategory(string categoryName, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>StopCategory name:" + categoryName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
            List<int> instanceList;
            if (playCategoryDict.TryGetValue(categoryName, out instanceList))
            {
                for (int i = 0; i < instanceList.Count; ++i)
                {
                    stop(instanceList[i], fadeTime);
                }
            }
        }

        // ---------------------------------------------------------------
        // ラベル単位で一時停止.
        public void onPauseLabel(string labelName, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OnPauseLabel name:" + labelName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                if (player != null)
                {
                    player.OnPauseAll(fadeTime);
                }
            }
        }

        // ---------------------------------------------------------------
        // ラベル単位で一時停止解除.
        public void offPauseLabel(string labelName, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OffPauseLabel name:" + labelName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                if (player != null)
                {
                    player.OffPauseAll(fadeTime);
                }
            }
        }

        // ---------------------------------------------------------------
        // カテゴリを指定してポーズ設定.
        public void onPauseCategory(string categoryName, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OnPauseCategory name:" + categoryName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
            List<int> instanceList;
            if (playCategoryDict.TryGetValue(categoryName, out instanceList))
            {
                for (int i = 0; i < instanceList.Count; ++i)
                {
                    onPause(instanceList[i], fadeTime);
                }
            }
        }

        // ---------------------------------------------------------------
        // カテゴリを指定してポーズ解除.
        public void offPauseCategory(string categoryName, float fadeTime = -1)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=magenta>OffPauseCategory name:" + categoryName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
            List<int> instanceList;
            if (playCategoryDict.TryGetValue(categoryName, out instanceList))
            {
                for (int i = 0; i < instanceList.Count; ++i)
                {
                    offPause(instanceList[i], fadeTime);
                }
            }
        }

		// ---------------------------------------------------------------
		// 指定したインスタンスを除いてカテゴリを指定してポーズ解除..
		public void offPauseCategory(string categoryName, List<int> instanceList, float fadeTime = -1)
		{
#if UNITY_EDITOR
			if (IsActiveTool)
				AddLog("<color=magenta>OffPauseCategory name:" + categoryName + " fadeTime: " + fadeTime + "ms.</color>");
#endif
			List<int> allInstanceList;
			bool find = false;
			if (playCategoryDict.TryGetValue(categoryName, out allInstanceList))
			{
				for (int i = 0; i < allInstanceList.Count; ++i)
				{
					for (int j = 0; j < instanceList.Count; ++j)
					{
						if (instanceList[j] == allInstanceList[i])
						{
							find = true;
							break;
						}
					}
					if (find == false)
					{
						offPause(allInstanceList[i], fadeTime);
					}
				}
			}
		}

		// ---------------------------------------------------------------
		// インスタンスのステータスを取得.
		public AudioDefine.INSTANCE_STATUS getInstanceStatus(int instanceId)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                return player.GetInstanceStatus(instanceId);
            }
            return AudioDefine.INSTANCE_STATUS.STOP;
        }

        // ---------------------------------------------------------------
        // ラベルが再生中か.
        public bool isPlayingLabel(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                if (player.GetPlayingNum() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // ---------------------------------------------------------------
        // 読み込んでいるラベルの総数を取得
        public int getLabelNum()
        {
            return sourceDict.Count;
        }

        // ---------------------------------------------------------------
        // 読み込んでいるラベルの名前リストを取得
        public string[] getLabelNameList()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, AudioPlayer> value in sourceDict)
            {
                list.Add(value.Key);
            }
            return list.ToArray();
        }

        // ---------------------------------------------------------------
        // カテゴリ数を取得
        public int getCategoryNum()
        {
            return categoryDict.Count;
        }

        // ---------------------------------------------------------------
        // カテゴリ名リストを取得
        public string[] getCategoryNameList()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, AudioCategorySettings> value in categoryDict)
            {
                list.Add(value.Key);
            }
            return list.ToArray();
        }

        // ---------------------------------------------------------------
        // マスター数を取得
        public int getMasterNum()
        {
            return masterDict.Count;
        }

        // ---------------------------------------------------------------
        // マスター名リストを取得
        public string[] getMasterNameList()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, AudioMasterSettings> value in masterDict)
            {
                list.Add(value.Key);
            }
            return list.ToArray();
        }

        // ---------------------------------------------------------------
        // 3D設定数を取得
        public int getAudio3DSettingsNum()
        {
            return audio3DSettings.Count;
        }

        // ---------------------------------------------------------------
        // 3D設定名リストを取得
        public string[] getAudio3DSettingsNameList()
        {
            List<string> list = new List<string>();
            foreach(KeyValuePair<string, Audio3DSettings> value in audio3DSettings)
            {
                list.Add(value.Key);
            }

            return list.ToArray();
        }

		// ---------------------------------------------------------------
		// Playerに設定されているUnityミキサー指定を更新する（エディタのみ）
		public void updateUnityMixerName(string labelName, string newMixerName)
		{
			AudioPlayer player;
			if (sourceDict.TryGetValue(labelName, out player))
			{
				if (mixerSettings != null)
				{
					AudioMixerGroup[] group = mixerSettings.FindGroup(newMixerName);
					if (group != null)
					{
						if (group.Length != 0)
						{
							player.SetAudioMixerGroup(group[0]);
						}
					}
				}
			}
			return;
		}

		// ---------------------------------------------------------------
		// Playerに設定されているSpatialGroup指定を更新する（エディタのみ）
		public void updateSpatialGroupName(string labelName, string newGroupName)
		{
			AudioPlayer player;
			if (sourceDict.TryGetValue(labelName, out player))
			{
				if (name != null)
				{
					Audio3DSettings d3set;
					if (audio3DSettings.TryGetValue(newGroupName, out d3set))
					{
						player.SetAudio3DSettings(d3set);
					}
				}
			}
			return;
		}

		// ---------------------------------------------------------------
		// ラベルに設定されているカテゴリ名を取得.
		public string getCategoryNameSettingOfLabel(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                return player.GetCategoryName();
            }
            return null;
        }

        // ---------------------------------------------------------------
        // カテゴリに設定されているマスター名を取得.
        public string getMasterNameSettingOfCategory(string categoryName)
        {
            AudioCategorySettings category;
            if (categoryDict.TryGetValue(categoryName, out category))
            {
                return category.masterName;
            }
            return null;
        }

        // ---------------------------------------------------------------
        // 現在の再生時間を取得(総再生時間ではなく波形上何秒の位置か).
        public float getPlayTime(int instanceId)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    return player.GetPlayTime(instanceId);
                }
            }
            return -1;  // 既に停止
        }

        // ---------------------------------------------------------------
        // 現在の再生サンプル位置を取得.
        public int getPlaySamples(int instanceId)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    return player.GetPlaySamples(instanceId);
                }
            }
            return -1;  // 既に停止
        }

        // ----------------------------------------------------------------------
        // 再生位置を設定(波形上何秒の位置か).
        public void setTime(int instanceId, float time)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.SetTime(instanceId, time);
                }
            }
        }

        // ----------------------------------------------------------------------
        // 再生位置を設定(サンプルで指定).
        public void setTimeSamples(int instanceId, int samples)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    player.SetTimeSamples(instanceId, samples);
                }
            }
        }

        // ---------------------------------------------------------------
        // ミュート設定.
        public void setMute(bool onMute)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetMute " + IsOnMute.ToString() + " => " + onMute.ToString() + "</color>");
#endif
            IsOnMute = onMute;
            foreach (KeyValuePair<string, AudioMasterSettings> value in masterDict)
            {
                AudioMasterSettings master = value.Value;
                master.SetMute(onMute);
            }
        }

        // ---------------------------------------------------------------
        // ミュート設定状態を取得.
        public bool getMuteStatus()
        {
            return IsOnMute;
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してラベルのAudioClip名リストを取得
        public string[] getAudioClipNameLoadId(int loadId)
        {
            List<string> nameList = new List<string>();
            foreach (KeyValuePair<string, AudioPlayer> source in sourceDict)
            {
                AudioPlayer player = source.Value;
                AudioLabelSettings label = player.GetLabelSettings();
                if (label.loadId == loadId)
                {
                    nameList.Add(label.GetClipName());
                }
            }
            return nameList.ToArray();
        }

        // ---------------------------------------------------------------
        // すべてのラベルのAudioClip名リストを取得
        public string[] getAudioClipNameAll()
        {
            List<string> nameList = new List<string>();
            foreach (KeyValuePair<string, AudioPlayer> source in sourceDict)
            {
                AudioPlayer player = source.Value;
                AudioLabelSettings label = player.GetLabelSettings();
                nameList.Add(label.GetClipName());
            }
            return nameList.ToArray();
        }

        // ---------------------------------------------------------------
        // 指定したラベルのAudioClip名リストを取得
        public string getAudioClipName(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                AudioLabelSettings label = player.GetLabelSettings();
                return label.GetClipName();
            }
            return null;
        }

        // ---------------------------------------------------------------
        // 指定したラベルのAudioClip名リストを取得
        public string[] getAudioClipNames(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                AudioLabelSettings label = player.GetLabelSettings();
                List<string> nameList = new List<string>();
                nameList.Add(label.GetClipName());
                if (label.isRandomPlay == true && label.randomSource != null)
                {
                    for (int i = 0; i < label.randomSource.Length; ++i)
                    {
                        string name = getAudioClipName(label.randomSource[i]);
                        if ( name != null )
                        {
                            nameList.Add(name);
                        }
                    }
                }
                return nameList.ToArray();
            }
            return null;
        }

        // ---------------------------------------------------------------
        // 指定したラベルのRandomSource(ラベル名)を取得
        public string[] getRandomSourceNames(string labelName)
        {
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                AudioLabelSettings label = player.GetLabelSettings();
                List<string> nameList = new List<string>();
                nameList.Add(label.name);
                if (label.isRandomPlay == true && label.randomSource != null)
                {
                    for (int i = 0; i < label.randomSource.Length; ++i)
                    {
                        if (!nameList.Contains(label.randomSource[i]))
                        {
                            nameList.Add(label.randomSource[i]);
                        }
                    }
                }
                return nameList.ToArray();
            }
            return null;
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してラベルにAuidoClipを割り当て
        public void setAudioClipToLabelLoadId(int loadId)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetAudioClipToLabelLoadId loadId:" + loadId + ".</color>");
#endif
            foreach (KeyValuePair<string, AudioPlayer> source in sourceDict)
            {
                AudioPlayer player = source.Value;
                AudioLabelSettings label = player.GetLabelSettings();
                if (label.loadId == loadId)
                {
                    AudioClip tmpClip;
                    if (audioClipDict.TryGetValue(label.GetClipName(), out tmpClip))
                    {
                        player.SetPlayClip(tmpClip);
                    }
                }
            }
            updateRandomSourceInfoAll();
        }

        // ---------------------------------------------------------------
        // すべてのラベルにAuidoClipを割り当て
        public void setAudioClipToLabelAll()
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetAudioClipToLabelAll.</color>");
#endif
            foreach (KeyValuePair<string, AudioPlayer> source in sourceDict)
            {
                AudioPlayer player = source.Value;
                AudioLabelSettings label = player.GetLabelSettings();
                AudioClip tmpClip;
                if (audioClipDict.TryGetValue(label.GetClipName(), out tmpClip))
                {
                    player.SetPlayClip(tmpClip);
                }
            }
            updateRandomSourceInfoAll();
        }

        // ---------------------------------------------------------------
        // 指定したラベルにAuidoClipを割り当て
        public void setAudioClipToLabel(string labelName)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetAudioClipToLabel name:" + labelName + ".</color>");
#endif
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                AudioLabelSettings label = player.GetLabelSettings();
                AudioClip tmpClip;
                if (audioClipDict.TryGetValue(label.GetClipName(), out tmpClip))
                {
                    player.SetPlayClip(audioClipDict[label.GetClipName()]);
                    updateRandomSourceInfo(labelName);
                }
            }
        }

        // ---------------------------------------------------------------
        // 指定したラベルにAuidoClipを割り当て
        public void setAndroidNativeToLabel(string labelName, string filePath, string className, string funcName)
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>SetAudioClipToLabel name:" + labelName + ".</color>");
#endif
            AudioPlayer player;
            if (sourceDict.TryGetValue(labelName, out player))
            {
                AudioLabelSettings label = player.GetLabelSettings();
                label.SetAndroidSoundId(USndAndroidNativePlayer.LoadData(filePath, className, funcName));
            }
        }

        // ---------------------------------------------------------------
        // すべてのオブジェクトプールをクリア
        public void clearObjectPool()
        {
#if UNITY_EDITOR
            if (IsActiveTool)
                AddLog("<color=cyan>ClearObjectPool.</color>");
#endif
            if (AudioMainPool.instance != null)
            {
                AudioMainPool.instance.Clear();
            }
        }


        // ---------------------------------------------------------------
        // ラベルの総再生時間を秒単位で取得
        public float getLabelLength(string labelName)
        {
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
                return 0.0f;
            }
            return player.GetClipLength();
        }

        // ---------------------------------------------------------------
        // ラベルの総再生時間をサンプル単位で取得
        public int getLabelSamples(string labelName)
        {
            AudioPlayer player;
            if (!sourceDict.TryGetValue(labelName, out player))
            {
                return 0;
            }
            return player.GetClipSamples();
        }

        // ---------------------------------------------------------------
        // 再生中インスタンスのスペクトラムを取得
        public bool getSpectrumData(int instanceId, float[] sample, int channel, FFTWindow window)
        {
            AudioPlayer player;
            if (playAudioDict.TryGetValue(instanceId, out player))
            {
                if (player != null)
                {
                    return player.GetSpectrumData(instanceId, sample, channel, window);
                }
            }
            return false;
        }

		// ---------------------------------------------------------------
		// ラベルがループするか
		public bool isLoop(string labelName)
		{
			AudioPlayer player;
			if (sourceDict.TryGetValue(labelName, out player))
			{
				AudioLabelSettings label = player.GetLabelSettings();
				return label.GetLoop();
			}
			return false;
		}

		// ---------------------------------------------------------------
		// ラベルの最大発音数を取得
		public int getLabelMaxPlaybacksNum(string labelName)
		{
			AudioPlayer player;
			if (sourceDict.TryGetValue(labelName, out player))
			{
				AudioLabelSettings label = player.GetLabelSettings();
				return label.maxPlaybacksNum;
			}
			return -1;
		}

		// ---------------------------------------------------------------
		// カテゴリの最大発音数を取得
		public int getCategoryMaxPlaybacksNum(string categoryName)
		{
			AudioCategorySettings category;
			if (categoryDict.TryGetValue(categoryName, out category))
			{
				return category.maxPlaybacksNum;
			}
			return -1;
		}

		// ---------------------------------------------------------------
		// ラベルに指定されているカテゴリの最大発音数を取得
		public int getCategoryMaxPlaybacksNumFromLabel(string labelName)
		{
			AudioPlayer player;
			if (sourceDict.TryGetValue(labelName, out player))
			{
				AudioLabelSettings label = player.GetLabelSettings();
				AudioCategorySettings category;
				if (categoryDict.TryGetValue(label.categoryName, out category))
				{
					return category.maxPlaybacksNum;
				}
			}
			return -1;
		}

		// ---------------------------------------------------------------
		// 指定したラベルがインターバル期間中か
		public bool isInterval(string labelName)
		{
			AudioPlayer player;
			if (sourceDict.TryGetValue(labelName, out player))
			{
				// 再生OKならtrueが帰ってくるので逆を返す
				return (player.IsPlayInterval() ? false : true);
			}
			return false;
		}

		// ---------------------------------------------------------------
		// 現在発音中の総数を取得
		public int getCurrentPlayNum()
		{
			return playAudioDict.Count;
		}


		// ---------------------------------------------------------------
		// 再生中リストの整理.
		void Update()
        {
            // ダッキングボリューム更新
            foreach (KeyValuePair<string, List<string>> duckingValue in playDuckingTrigger)
            {
				if (duckingValue.Value.Count == 0)
				{
					// 毎フレームTryGetValueしないようにする
					continue;
				}
				AudioCategorySettings category;
                if (categoryDict.TryGetValue(duckingValue.Key, out category))
                {
                    // 停止していたらダッキング解除をかける
                    for (int i = 0; i < duckingValue.Value.Count; )
                    {
                        AudioPlayer player;
                        if (sourceDict.TryGetValue(duckingValue.Value[i], out player))
                        {
                            AudioLabelSettings label = player.GetLabelSettings();
                            if (player.GetPlayingNum() == 0)
                            {
                                // 再生されていないならダッキングOFF
                                if (0 == (duckingValue.Value.Count - 1))
                                {
                                    // 最後の再生なのでダッキングから復帰
                                    // ダッキング完了は1に戻す
                                    if (label.autoRestoreDucking)
                                    {
                                        category.SetDuckingVolumeUpdater(1, label.duckingEndTime, false);
#if UNITY_EDITOR
                                        if (IsActiveTool)
                                            AddLog("<color=cyan>Update Ducking Resume category:" + category.categoryName + " .</color>");
#endif
                                    }
                                }
                                duckingValue.Value.RemoveAt(i);
                            }
                            else
                            {
                                if (label.autoRestoreDucking)
                                {
                                    if (label.restoreTime >= 0)
                                    {
                                        // 復帰時間の指定がある
                                        float time = player.GetPlayTime();
                                        if (time > label.restoreTime)
                                        {
                                            // 最後の再生だったらダッキングから復帰させる
                                            if (0 == (duckingValue.Value.Count - 1))
                                            {
                                                category.SetDuckingVolumeUpdater(1, label.duckingEndTime, false);
                                                duckingValue.Value.RemoveAt(i);
#if UNITY_EDITOR
                                                if (IsActiveTool)
                                                    AddLog("<color=cyan>Update Ducking Resume category:" + category.categoryName + " Restore " + player.PlayerName + " time: " + time + ".</color>");
#endif
                                                continue;
                                            }
                                            else
                                            {
                                                duckingValue.Value.RemoveAt(i);
                                            }
                                        }
                                    }
                                }
                                ++i;
                            }
                        }
                        else
                        {
                            ++i;
                        }
                    }

                    // ダッキングパラメータ更新
					// ver2.18.2 カテゴリボリュームの更新と一緒に行うのでここでは更新しないようにする
                    //category.UpdateDuckingVolume();
                }
            }

            // ボリューム更新
            foreach (KeyValuePair<string, AudioMasterSettings> masterValue in masterDict)
            {
                AudioMasterSettings master = masterValue.Value;
                master.UpdateVolume();
            }
            foreach (KeyValuePair<string, AudioCategorySettings> categoryValue in categoryDict)
            {
                AudioCategorySettings category = categoryValue.Value;
				// ver2.18.2 ダッキング復帰時の更新が必要なのでここで行う
				category.UpdateDuckingVolume();
				category.UpdateVolume();
			}

            // カテゴリごとの再生インスタンスリストを更新.
            foreach (KeyValuePair<string, List<int>> dictValue in playCategoryDict)
            {
                List<int> list = dictValue.Value;
                orderCategoryInstanceList(list);
            }

            // 再生インスタンスとプレイヤーのディクショナリを更新.
            if (playAudioDict.Count != 0)
            {
                playAudioRemoveKey.Clear();
				/*
                playerHashSet.Clear();
                foreach (AudioPlayer item in playAudioDict.Values) {
                    playerHashSet.Add(item);
                }*/
                foreach (var player in playerHashSet) {
                    if (player == null)
                        continue;

                    player.Update();
                    AudioCategorySettings cateogry = categoryDict[player.GetCategoryName()];
					// 再生中ならボリューム係数更新.
					if (cateogry != null)
					{
						if (player != null)
						{
							player.UpdateVolumeFactor(cateogry.GetVolumeFactor());
						}
					}
                }
                foreach (KeyValuePair<int, AudioPlayer> playValue in playAudioDict)
                {
                    AudioPlayer playObj = playValue.Value;
                    if (playObj != null)
                    {
                        AudioDefine.INSTANCE_STATUS status = playObj.GetInstanceStatus(playValue.Key);
                        // 停止済みなら再生中リストから消してもよい.
                        if (status == AudioDefine.INSTANCE_STATUS.STOP)
                        {
                            playAudioRemoveKey.Add(playValue.Key);
							// インスタンスが0なら更新用のHashリストから外す.
							if ( playValue.Value.GetPlayingTrueNum() == 0)
							{
								playerHashSet.Remove(playValue.Value);
							}
                        }
                    }
                }
                for (int i = 0; i < playAudioRemoveKey.Count; ++i)
                {
                    playAudioDict.Remove(playAudioRemoveKey[i]);
                }
            }
        }

        // ---------------------------------------------------------------
        // ダッキング解除がまだ未実行の場合に処理を行う
        private void resetDuckingBeforeUpdate(AudioPlayer player)
        {
            AudioLabelSettings labelSettings = player.GetLabelSettings();
            string labelName = labelSettings.name;
            if (labelSettings.duckingCategories != null && labelSettings.autoRestoreDucking)
            {
                for (int i = 0; i < labelSettings.duckingCategories.Length; ++i)
                {
                    string categoryName = labelSettings.duckingCategories[i];
                    List<string> triggerList;
                    if (playDuckingTrigger.TryGetValue(categoryName, out triggerList))
                    {
                        for (int j = 0; j < triggerList.Count; ++j)
                        {
                            if ( triggerList[j].Equals(labelName) )
                            {
                                // 再生されていないならダッキングOFF
                                if (0 == (triggerList.Count - 1))
                                {
                                    // 最後の再生なのでダッキングから復帰
                                    // ダッキング完了は1に戻す
                                    if (labelSettings.autoRestoreDucking)
                                    {
                                        AudioCategorySettings category = categoryDict[categoryName];
                                        // 再生中ならボリューム係数更新.
                                        if (category != null) category.SetDuckingVolumeUpdater(1, labelSettings.duckingEndTime, false);

#if UNITY_EDITOR
                                        if (IsActiveTool)
                                            AddLog("<color=cyan>Reset Ducking Before Update:" + category.categoryName + " " + labelName + ".</color>");
#endif
                                    }
                                }
                                triggerList.RemoveAt(j);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

}
