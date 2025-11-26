/// <summary>
/// AudioPlayer
/// AudioInstanceを生成して再生を行う。
/// 親となるAudioSourceにランダムが設定されている場合、親のランダムに従って再生される。
/// 子のランダムは無効だが、ランダムでないボリューム、パン、ピッチは個別にもてる。発音数は親の設定に従う。
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace USnd
{
    public partial class AudioManager : MonoBehaviour
    {

        private class AudioPlayer
        {
            private class PlayData
            {
                public AudioClip clip;
                public AudioLabelSettings info;
            };
            List<AudioInstance> playInstance = new List<AudioInstance>(4);	// 再生中インスタンス.

            List<PlayData> playSource = new List<PlayData>();		// このプレイヤーの再生ソース.
            AudioLabelSettings playSettings = null;		// デフォルトの再生設定をコピーしたもの.

            AudioClip playClip = null;  // XMLから読み込み時のAudioClip保管用
            bool isSetClip = false;

            int prevPlayIndex = -1;

            float prevVolumeRandom = -10000;
            float prevPitchRandom = -10000;
            float prevPanRandom = -10000;

            List<int> prevPlaySamplesList = new List<int>(4);

            string playerName = null;

            AudioMixerGroup mixer = null;

            Audio3DSettings spatialSettings = null;

            float nextInterval = 0;     // 残り待機時間
            float prevPlayTime = 0;     // 前回再生時間

            bool force2D = false;


            public string PlayerName
            {
                set { this.playerName = value; }
                get { return this.playerName; }
            }

            public void SetAudioMixerGroup(AudioMixerGroup _mixer)
            {
                mixer = _mixer;
            }

            // ----------------------------------------------------------------------
            // ランダムソース情報を取得しなおす
            public void UpdateRandomSourceInfo(Dictionary<string, AudioPlayer> dict)
            {
                if (playSettings.isRandomPlay)
                {
                    playSource.Clear();
                    PlayData data = new PlayData();
                    data.clip = playClip;
                    data.info = playSettings;
                    playSource.Add(data);

                    if (playSettings.randomSource != null)
                    {
                        //for (int i = 0; i < playSettings.randomSource.Count; ++i)
                        for (int i = 0; i < playSettings.randomSource.Length; ++i)
                        {
                            AudioPlayer player;
                            if (dict.TryGetValue(playSettings.randomSource[i], out player) == true)
                            {
                                AudioClip audioClip = player.GetPlayClip();
                                AudioLabelSettings labelInfo = player.GetLabelSettings();
                                PlayData rndData = new PlayData();
                                rndData.clip = null;
                                rndData.info = null;
                                if (audioClip != null)
                                {
#if USND_DEBUG_LOG
                                AudioDebugLog.Log(playerName + " random suorce add " + playSettings.randomSource[i] + " info:" + labelInfo);
#endif
                                    rndData.clip = audioClip;
                                    rndData.info = labelInfo;
                                    playSource.Add(rndData);
                                    prevPlaySamplesList.Add(0);
                                }
                                else
                                {
#if USND_DEBUG_LOG
                                AudioDebugLog.LogWarning(playerName + "に設定されているランダムソース" + playSettings.randomSource[i] + "はAudioSourceかAudioClipを含んでいません。");
#endif
                                }
                            }
                            else
                            {
#if USND_DEBUG_LOG
                            AudioDebugLog.LogWarning(playerName + "に設定されているランダムソース" + playSettings.randomSource[i] + "はロードされていません。");
#endif
                            }
                        }
                    }
                }
            }

            public AudioClip GetPlayClip()
            {
                return playClip;
            }

            public bool IsSetPlayClip()
            {
                return isSetClip;
            }

            public void SetPlayClip(AudioClip clip)
            {
                playClip = clip;
                isSetClip = true;
                if (playSource.Count != 0)
                {
                    playSource[0].clip = clip;
                    playSource[0].info = playSettings;
                }
            }

            public float GetClipLength()
            {
                if ( playClip != null )
                {
                    return playClip.length;
                }
                return 0;
            }

            public int GetClipSamples()
            {
                if (playClip != null)
                {
                    return playClip.samples;
                }
                return 0;
            }

            // ----------------------------------------------------------------------
            // 初期化
            public bool Init(AudioClip clip, string name, AudioLabelSettings label, Dictionary<string, AudioPlayer> dict)
            {
                playClip = clip;
                if (playerName == null) playerName = name;
                PlayData data = new PlayData();
                data.clip = playClip;
                data.info = label;
                playSource.Add(data);
                prevPlaySamplesList.Add(0);
                nextInterval = 0;

                playSettings = label;

                if (label.maxPlaybacksNum > 0)
                {
                    AudioInstancePool.instance.AddEmpty(label.maxPlaybacksNum);
                }

                if (playSettings == null)
                {
#if USND_DEBUG_LOG
                    AudioDebugLog.LogWarning(playerName + "はAudioLabelSettingが設定されていません。");
#endif
                    return false;
                }

                UpdateRandomSourceInfo(dict);

                initRandomSettins();

                return true;
            }

            void initRandomSettins()
            {
                if (playSettings.isVolumeRandom)
                {
                    if (playSettings.volumeRandomMax < playSettings.volumeRandomMin)
                    {
                        float tmp = playSettings.volumeRandomMin;
                        playSettings.volumeRandomMin = playSettings.volumeRandomMax;
                        playSettings.volumeRandomMax = tmp;
                    }
                }

                if (playSettings.isPitchRandom)
                {
                    if (playSettings.pitchRandomMax < playSettings.pitchRandomMin)
                    {
                        int tmp = playSettings.pitchRandomMin;
                        playSettings.pitchRandomMin = playSettings.pitchRandomMax;
                        playSettings.pitchRandomMax = tmp;
                    }
                }

                if (playSettings.isPanRandom)
                {
                    if (playSettings.panRandomMax < playSettings.panRandomMin)
                    {
                        float tmp = playSettings.panRandomMin;
                        playSettings.panRandomMin = playSettings.panRandomMax;
                        playSettings.panRandomMax = tmp;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // AudioClipをリセット
            public void ResetPlayClip()
            {
                playClip = null;
                isSetClip = false;
                for (int i = 0; i < playSource.Count; ++i)
                {
                    playSource[i].clip = null;
                    playSource[i].info = null;
                }
            }

            // ----------------------------------------------------------------------
            // リセット
            public void Reset()
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.ForceStop();
                    instance.Reset(AudioMainPool.instance);
                    AudioInstancePool.instance.Deactive(instance);
                }
                playInstance.Clear();
                for (int i = 0; i < playSource.Count; ++i)
                {
                    playSource[i].clip = null;
                    playSource[i].info = null;
                }
                playSource.Clear();
                playSettings = null;
                playClip = null;
                isSetClip = false;
                mixer = null;
                nextInterval = 0;
            }

            // ----------------------------------------------------------------------
            // 事前にデータをロードする(Preload Audio Dataのチェックがはずれているもののみ有効)
            public void LoadAudioData()
            {
                for (int i = 0; i < playSource.Count; ++i)
                {
                    PlayData data = playSource[i];
                    if (data.clip != null)
                    {
                        data.clip.LoadAudioData();
                    }
                }
            }

            // ----------------------------------------------------------------------
            // ロードしたデータの削除
            public void UnloadAudioData()
            {
                for (int i = 0; i < playSource.Count; ++i)
                {
                    PlayData data = playSource[i];
                    if (data.clip != null)
                    {
                        if (data.clip.preloadAudioData == false)
                        {
                            data.clip.UnloadAudioData();
                        }
                    }
                }
            }

            // ----------------------------------------------------------------------
            // random value
            float getRandomValue(float min, float max, float unit, bool isconsecutive, float prevValue)
            {
                float value = 0;
                bool isc_tmp = isconsecutive;

                // min==maxになっていると無限ループになってしまうのでfalseにしておく
                if ( min == max )
                {
                    isc_tmp = false;
                }

                do
                {
                    if (unit != 0)
                    {
                        float range = (min > max) ? (min - max) : (max - min);
                        float tmpValue = Random.Range(0, range / unit);
                        tmpValue = Mathf.Round(tmpValue);
                        value = tmpValue * unit + min;
                    }
                    else
                    {
                        value = Random.Range(min, max);
                    }
                } while (isc_tmp && (prevValue == value));
                return value;
            }

            // ----------------------------------------------------------------------
            // 再生開始位置をリセット.
            public void ResetPlayPosition()
            {
                for (int i = 0; i < prevPlaySamplesList.Count; ++i)
                {
                    prevPlaySamplesList[i] = 0;
                }
            }

            // ----------------------------------------------------------------------
            // 再生中のインスタンス数を取得.
            public int GetPlayingNum()
            {
                // ポーズも再生中に含む,playInstanceの更新はupdateで行っている.
                // もうすぐ停止する予定のものだけ省く.
                int stop_soon = 0;
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    AudioDefine.INSTANCE_STATUS status = instance.GetStatus();
                    if (status == AudioDefine.INSTANCE_STATUS.STOP_SOON || status == AudioDefine.INSTANCE_STATUS.STOP)
                    {
                        ++stop_soon;
                    }
                }
                return playInstance.Count - stop_soon;
            }

            // ----------------------------------------------------------------------
            // 停止予定も含んだ再生中インスタンス数を取得.
            public int GetPlayingTrueNum()
            {
                return playInstance.Count;
            }

            // ----------------------------------------------------------------------
            // 最大発音数を取得
            public int GetMaxPlaybacksNum()
            {
                if (playSettings == null)
                {
                    return 0;
                }
                return playSettings.maxPlaybacksNum;
            }

            // ----------------------------------------------------------------------
            // ラベルの発音数オーバーのときに古い再生を止めるか
            public bool IsStealOldest()
            {
                if (playSettings == null)
                {
                    return true;
                }
                return playSettings.isStealOldest;
            }

            // ----------------------------------------------------------------------
            // 所属するカテゴリ名を取得
            public string GetCategoryName()
            {
                if (playSettings == null) return null;
                if (playSettings.GetAttachCategory() == null) return null;
                return playSettings.GetAttachCategory().categoryName;
            }

            // ----------------------------------------------------------------------
            // 所属するカテゴリの発音数を取得
            public int GetCategoryMaxPlaybacksNum()
            {
                if (playSettings == null)
                {
                    return 0;
                }
                AudioCategorySettings category = playSettings.GetAttachCategory();
                return category.maxPlaybacksNum;
            }

            // ----------------------------------------------------------------------
            // プライオリティを取得
            public int GetPriority()
            {
                if (playSettings == null)
                {
                    return 0;
                }
                return playSettings.priority;
            }

            // ----------------------------------------------------------------------
            // カテゴリ発音オーバー時のふるまい
            public AudioLabelSettings.BEHAVIOR GetMaxPlaybacksBehavior()
            {
                if (playSettings == null)
                {
                    return AudioLabelSettings.BEHAVIOR.STEAL_OLDEST;
                }
                return playSettings.maxPlaybacksBehavior;
            }

            // ----------------------------------------------------------------------
            // フェードアウト時間を取得
            public float GetFadeOutTime()
            {
                if (playSettings == null)
                {
                    return 0;
                }
                return playSettings.fadeOutTime;
            }

            // ----------------------------------------------------------------------
            // カテゴリを返す
            public AudioCategorySettings GetCategorySettings()
            {
                if (playSettings == null)
                {
                    return null;
                }
                return playSettings.GetAttachCategory();
            }

            // ----------------------------------------------------------------------
            // ラベル設定を返す
            public AudioLabelSettings GetLabelSettings()
            {
                return playSettings;
            }

            // ----------------------------------------------------------------------
            // インスタンスステータスを取得.
            public AudioDefine.INSTANCE_STATUS GetInstanceStatus(int instanceId)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        return instance.GetStatus();
                    }
                }
                return AudioDefine.INSTANCE_STATUS.STOP;
            }

            // ----------------------------------------------------------------------
            // 再生が一番古いインスタンスを停止.
            public void StopOldInstance()
            {
                // 先頭のほうが古いインスタンス.
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    AudioDefine.INSTANCE_STATUS status = instance.GetStatus();
                    // 再生中、ポーズ中、ポーズ予定だったら停止.
                    if (status == AudioDefine.INSTANCE_STATUS.PLAY || status == AudioDefine.INSTANCE_STATUS.PAUSE || status == AudioDefine.INSTANCE_STATUS.PAUSE_SOON)
                    {
                        instance.Stop(AudioDefine.DEFAULT_FADE);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 再生準備～再生.
            int prepareImpl(float volume, float fadeTime, float pan, int pitch, float delay, bool isStart, bool isForce2D)
            {
                int playIndex = 0;
                if (playSettings.isRandomPlay && playSource.Count > 1)
                {
                    do
                    {
                        playIndex = (int)Mathf.Round(Random.Range(0, playSource.Count));
                    } while (playSettings.inconsecutiveSource && prevPlayIndex == playIndex);
                }

                prevPlayIndex = playIndex;
                AudioClip clip = null;
                AudioSource play = null;
                AudioLabelSettings info = null;
                if (playSource[playIndex].clip != null)
                {
                    clip = playSource[playIndex].clip;
                    info = playSource[playIndex].info;
                }
                else
                {
#if USND_DEBUG_LOG
                    AudioDebugLog.Log(PlayerName + " Random source[" + playIndex + "] is null (1)");
#endif
                    playIndex = 0;
                    clip = playSource[playIndex].clip;
                    info = playSettings;
                    if (play == null && clip == null)
                    {
#if USND_DEBUG_LOG
                        AudioDebugLog.Log(PlayerName + " Random source[" + playIndex + "] is null (2)");
#endif
                        return AudioDefine.INSTANCE_ID_ERROR;
                    }
                }

                bool getSource = false;
                play = AudioMainPool.instance.GetClone();
#if UNITY_EDITOR
                // ログ出力用でしか使わないのでエディタ時のみ有効にする
                play.name = clip.name;
#endif
                play.clip = clip;
                play.playOnAwake = false;
                play.loop = info.GetLoop();

                play.spatialBlend = 0;
                force2D = isForce2D;
                // 3D設定
                if (spatialSettings != null && !isForce2D)
                {
                    play.spatialBlend = spatialSettings.spatialBlend;
                    play.reverbZoneMix = spatialSettings.reverbZoneMix;
                    play.dopplerLevel = spatialSettings.dopplerLevel;
                    play.spread = spatialSettings.spread;
                    play.rolloffMode = spatialSettings.rolloffMode;
                    play.minDistance = spatialSettings.minDistance;
                    play.maxDistance = spatialSettings.maxDistance;
                    play.SetCustomCurve(AudioSourceCurveType.CustomRolloff, spatialSettings.customRolloffCurve);
                    play.SetCustomCurve(AudioSourceCurveType.SpatialBlend, spatialSettings.spatialBlendCurve);
                    play.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, spatialSettings.reverbZoneMixCurve);
                    play.SetCustomCurve(AudioSourceCurveType.Spread, spatialSettings.spreadCurve);
                }


                getSource = true;

                if (mixer != null)
                {
                    play.outputAudioMixerGroup = mixer;
                }
                else
                {
                    play.outputAudioMixerGroup = null;  // 設定がないならリセット
                }

                if (play.clip == null)
                {
#if USND_DEBUG_LOG
                    AudioDebugLog.Log(PlayerName + " AudioClip is null.");
#endif
                    return AudioDefine.INSTANCE_ID_ERROR;
                }

                AudioInstance instance = AudioInstancePool.instance.AddComponent();

                // cloneの初期パラメータを変えて再生する場合は変更.
                // パラメータ指定なら指定されたパラメータにする.
                // ランダムなら値を決定.

                float setVolume = volume;
                if (info.isVolumeRandom)
                {
                    setVolume = getRandomValue(info.volumeRandomMin, info.volumeRandomMax,
                                                   info.volumeRandomUnit, info.inconsecutiveVolume, prevVolumeRandom);
                    prevVolumeRandom = setVolume;
                }
                float setPan = pan;
                if (info.isPanRandom)
                {
                    setPan = getRandomValue(info.panRandomMin, playSettings.panRandomMax,
                                                      info.panRandomUnit, info.inconsecutivePan, prevPanRandom);
                    prevPanRandom = setPan;
                }
                int setPitch = pitch;
                if (info.isPitchRandom)
                {
                    setPitch = (int)getRandomValue(info.pitchRandomMin, info.pitchRandomMax,
                                                  info.pitchRandomUnit, info.inconsecutivePitch, prevPitchRandom);
                    prevPitchRandom = setPitch;
                }

                float setDelay = delay;
                if (delay < 0)
                {
                    setDelay = info.playStartDelay;
                }

                if (getSource == true)
                {
                    instance.Init(play,
                                  info,
                                  (playSettings.GetAttachCategory() != null) ? playSettings.GetAttachCategory().GetVolumeFactor() : 1,
                                  prevPlayIndex);
                }
                play.gameObject.SetActive(true);     // クローンしたオブジェクトもアクティブにする、再生用.

                playInstance.Add(instance);
                instance.Prepare(setVolume, fadeTime, setPan, setPitch, prevPlaySamplesList[prevPlayIndex]);

                if (isStart)
                {
                    instance.Play(setDelay);
                    setInterval();
                }

                return instance.GetInstanceID();
            }

            // ----------------------------------------------------------------------
            // 再生準備.
            public int Prepare(float volume, float fadeTime, float pan, int pitch, bool isForce2D)
            {
                return prepareImpl(volume, fadeTime, pan, pitch, 0, false, isForce2D);
            }

            // ----------------------------------------------------------------------
            // 準備して再生開始.
            public int Play(float volume, float fadeTime, float pan, int pitch, float delay, bool isForce2D)
            {
                int instanceId = prepareImpl(volume, fadeTime, pan, pitch, delay, true, isForce2D);
                return instanceId;
            }

            // ----------------------------------------------------------------------
            // 指定インスタンスを再生開始.
            public void PlayInstance(int instanceId, float delay = 0)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.Play(delay);
                        setInterval();
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // インスタンスのポジションを追従させるオブジェクトを指定.
            public void SetTrackingObject(int instanceId, GameObject target)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.SetTrackingObject(target);
                        return;
                    }
                }
            }
            
            // ----------------------------------------------------------------------
            // インスタンスのポジションを追従させるオブジェクトを指定.
            public void SetTrackingObject(int instanceId, Transform target)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.SetTrackingObject(target);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 指定インスタンスを停止.
            public void Stop(int instanceId, float fadeTime = -1)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.Stop(fadeTime);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 全インスタンス停止.
            public void StopAll(float fadeTime = -1)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.Stop(fadeTime);
                }
            }


            // ----------------------------------------------------------------------
            // ポーズ設定.
            public void OnPause(int instanceId, float fadeTime = -1)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.OnPause(fadeTime);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 全インスタンスポーズ設定.
            public void OnPauseAll(float fadeTime = -1)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.OnPause(fadeTime);
                }
            }

            // ----------------------------------------------------------------------
            // ポーズ解除.
            public void OffPause(int instanceId, float fadeTime = -1)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.OffPause(fadeTime);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 全インスタンスポーズ解除.
            public void OffPauseAll(float fadeTime = -1)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.OffPause(fadeTime);
                }
            }

            // ----------------------------------------------------------------------
            // ボリューム設定.
            public void SetVolume(int instanceId, float newVolume, float moveTime)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.SetVolume(newVolume, moveTime);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 再生中インスタンスの現在のボリュームを取得.
            public float GetCurrentVolume(int instanceId)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        return instance.GetCurrentVolume();
                    }
                }
                return 0;
            }

            // ----------------------------------------------------------------------
            // 再生中インスタンスの最終的なボリュームを取得.
            public float GetCalcVolume(int instanceId)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        return instance.GetCalcVolume();
                    }
                }
                return 0;
            }

            // ----------------------------------------------------------------------
            // 全インスタンスのボリューム設定.
            public void SetVolumeAll(float newVolume, float moveTime)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.SetVolume(newVolume, moveTime);
                }
            }

            // ----------------------------------------------------------------------
            // ピッチ設定.
            public void SetPitch(int instanceId, int newPitch, float moveTime)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.SetPitch(newPitch, moveTime);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 全員スタンスのピッチ設定.
            public void SetPitchAll(int newPitch, float moveTime)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.SetPitch(newPitch, moveTime);
                }
            }

            // ----------------------------------------------------------------------
            // パン設定.
            public void SetPan(int instanceId, float newPan, float moveTime)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.SetPan(newPan, moveTime);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 全インスタンスのパン設定.
            public void SetPanAll(float newPan, float moveTime)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.SetPan(newPan, moveTime);
                }
            }

            // ----------------------------------------------------------------------
            // 位置設定.
            public void SetPosition(int instanceId, Vector3 position)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.SetPosition(position);
                        return;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // 位置設定.
            public void SetPositionAll(Vector3 position)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.SetPosition(position);
                }
            }

            // ----------------------------------------------------------------------
            // ボリューム係数を更新.
            public void UpdateVolumeFactor(float volumeFactor)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    instance.UpdateVolumeFactor(volumeFactor);
                }
            }

            // ----------------------------------------------------------------------
            // 再生時間を取得.
            public float GetPlayTime(int instanceId)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        return instance.GetPlayTime();
                    }
                }
                return -1;
            }

            // ----------------------------------------------------------------------
            // 最後に再生したインスタンスの再生時間を取得.
            public float GetPlayTime()
            {
                if (playInstance.Count != 0)
                {
                    AudioInstance instance = playInstance[playInstance.Count - 1];
                    return instance.GetPlayTime();
                }
                return -1;
            }

            // ----------------------------------------------------------------------
            // 再生サンプル位置を取得.
            public int GetPlaySamples(int instanceId)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        return instance.GetPlaySamples();
                    }
                }
                return -1;
            }

            // ----------------------------------------------------------------------
            // インスタンスの再生位置を設定.
            public void SetTime(int instanceId, float time)
            {
                if (playInstance.Count != 0)
                {
                    AudioInstance instance = playInstance[playInstance.Count - 1];
                    instance.SetTime(time);
                }
            }

            // ----------------------------------------------------------------------
            // インスタンスの再生サンプル位置を設定.
            public void SetTimeSamples(int instanceId, int samples)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        instance.SetTimeSamples(samples);
                    }
                }
            }

            // ---------------------------------------------------------------
            // 再生中インスタンスのスペクトラムを取得
            public bool GetSpectrumData(int instanceId, float[] sample, int channel, FFTWindow window)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (instance.GetInstanceID() == instanceId)
                    {
                        return instance.GetSpectrumData(instanceId, sample, channel, window);
                    }
                }
                return false;
            }

            // ----------------------------------------------------------------------
            public void Update()
            {
                for (int i = 0; i < playInstance.Count; )
                {
                    AudioInstance instance = playInstance[i];
                    instance.Update();

                    if (playSettings.isPlayLastSamples)
                    {
                        // 一番新しい再生のみ再生位置をとっておく.
                        if (i == (playInstance.Count - 1))
                        {
                            prevPlaySamplesList[instance.GetRandomIndex()] = instance.GetPrevPlaySamples();
                        }
                    }
                    if (instance.GetStatus() == AudioDefine.INSTANCE_STATUS.STOP)
                    {
                        instance.Reset(AudioMainPool.instance);
                        playInstance.RemoveAt(i);
                        AudioInstancePool.instance.Deactive(instance);
                        if (playInstance.Count <= 0)
                        {
                            // randomの考慮も必要,複数ラベルで同じclipをランダム再生していた場合はこれだと考慮されていないので止まる可能性あり
                            for (int j = 0; j < playSource.Count; ++j)
                            {
                                if (playSource[j].clip != null && playSource[j].clip.preloadAudioData == false)
                                {
                                    // ver2.6.3 自動的にUnloadすると別ラベルで同一ファイルを再生するときに停止してしまうので自動ではUnloadしなように変更
                                    //playSource[j].clip.UnloadAudioData();
                                }
                            }
                        }
                    }
                    else
                    {
                        ++i;
                    }
                }
            }

            // ----------------------------------------------------------------------
            // SpatialGroupを取得
            public string GetSpatialGroup()
            {
                if (playSettings == null)
                {
                    return null;
                }
                return playSettings.spatialGroup;
            }

            // ----------------------------------------------------------------------
            // Audio3DSettingsが設定済みか
            public bool IsSetSpatialGroup()
            {
                return (spatialSettings == null) ? false : true;
            }

            // ----------------------------------------------------------------------
            // Audio3DSettingsを設定
            public void SetAudio3DSettings(Audio3DSettings setting)
            {
                spatialSettings = setting;
            }

            // ----------------------------------------------------------------------
            // インターバルが0以下か調べる
            public bool IsPlayInterval()
            {
                bool isPlay = true;
                if ( playSettings.playInterval > 0 )
                {
                    if ( nextInterval > 0 )
                    {
                        float currentTime = Time.unscaledTime;
                        nextInterval -= (currentTime - prevPlayTime);
                        prevPlayTime = currentTime;
                    }
                    // nextIntervalが0以下なら再生可能
                    if ( nextInterval <= 0 )
                    {
                        isPlay = true;
                        nextInterval = 0;
                    }
                    else
                    {
                        isPlay = false;
                    }
                }

                return isPlay;
            }

            private void setInterval()
            {
                if (playSettings.playInterval > 0)
                {
                    prevPlayTime = Time.unscaledTime;
                    nextInterval = playSettings.playInterval;
                }
            }

            // ----------------------------------------------------------------------
            // 3Dパラメータを更新
            public void UpdateAudio3DSettings(Audio3DSettings settings)
            {
                for (int i = 0; i < playInstance.Count; ++i)
                {
                    AudioInstance instance = playInstance[i];
                    if (force2D == false)
                    {
                        instance.UpdateAudio3DSettings(settings);
                    }
                }
            }

        }
    }
}