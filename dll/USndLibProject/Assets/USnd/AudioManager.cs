// USnd ver2.15.1
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System;
using UnityEngine.Audio;

namespace USnd
{

    public partial class AudioManager : MonoBehaviour
    {

        private static AudioManager manager = null;

        static public bool IsInitialized()
        {
            return manager ? true : false;
        }

        static public void Initialize(int defaultSampleRate = 0)
        {
            if ( manager == null )
            {
                if (defaultSampleRate >= 0)
                {
                    AudioConfiguration config = AudioSettings.GetConfiguration();
                    config.sampleRate = (defaultSampleRate == 0) ? AudioDefine.DEFAULT_SAMPLE_RATE : defaultSampleRate;
                    AudioSettings.Reset(config);
                }
                GameObject obj = new GameObject();
                manager = obj.AddComponent<AudioManager>();
                DontDestroyOnLoad(manager);
            }
        }

        static public void Terminate()
        {
            if ( manager != null )
            {
                manager.removeAll();
                AudioMainPool.Terminate();
                Destroy(manager.gameObject);
                Destroy(manager);
                manager = null;
            }
        }

        // ---------------------------------------------------------------
        // Unityのミキサー情報を登録
        static public void SetAudioMixer(AudioMixer mixer)
        {
            if ( manager != null )
            {
                manager.setAudioMixer(mixer);
            }
        }

        // ---------------------------------------------------------------
        // Unityのミキサー情報を解除
        static public void UnsetAudioMixer()
        {
            if (manager != null)
            {
                manager.unsetAudioMixer();
            }
        }

        // ---------------------------------------------------------------
        // Snapshotを設定
        static public void SetSnapshot(string snapName, float time)
        {
            if (manager != null)
            {
                manager.setSnapshot(snapName, time);
            }
        }

        // ---------------------------------------------------------------
        // Exposed ParametersでMixerの値を変更
        static public void SetAudioMixerExposedParam(string paramName, float value)
        {
            if (manager != null)
            {
                manager.setAudioMixerExposedParam(paramName, value);
            }
        }

        // ---------------------------------------------------------------
        // Audio3DSettingsを登録(Json形式で読み込み)
        static public void SetAudio3DSettingsFromJson(string jsonStr)
        {
            if (manager != null)
            {
                manager.setAudio3DSettingsFromJson(jsonStr);
            }
        }

        // ---------------------------------------------------------------
        // Audio3DSettingsを登録
        static public void SetAudio3DSettings(Audio3DSettings setting)
        {
            if (manager != null)
            {
                manager.setAudio3DSettings(setting);
            }
        }

        // ---------------------------------------------------------------
        // Audio3DSettingsを登録
        static public void SetAudio3DSettings(Audio3DSettings[] settings)
        {
            if (manager != null)
            {
                manager.setAudio3DSettings(settings);
            }
        }

        // ---------------------------------------------------------------
        // バイナリデータを読み込む
        static public bool LoadBinaryTable(byte[] tableData, int loadId = 0)
        {
            if ( manager != null )
            {
                return manager.loadBinaryTable(tableData, loadId);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // Jsonを読み込む
        static public bool LoadJson(string tableData, int loadId = 0)
        {
            if (manager != null)
            {
                return manager.loadJson(tableData, loadId);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // AudioClipをまとめて登録
        static public void AddAudioClip(AudioClip[] clips)
        {
            if (manager != null)
            {
                manager.addAudioClip(clips);
            }
        }

        // ---------------------------------------------------------------
        // AudioClipを登録
        static public void AddAudioClip(AudioClip clip)
        {
            if (manager != null)
            {
                manager.addAudioClip(clip);
            }
        }

        // ---------------------------------------------------------------
        // 登録済みのAudioClipか
        static public bool IsExistAudioClip(string clipName)
        {
            if ( manager != null )
            {
                return manager.isExistAudioClip(clipName);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // AudioClipの登録を削除
        static public void RemoveAudioClip(string clipName)
        {
            if (manager != null)
            {
                manager.removeAudioClip(clipName);
            }
        }

        // ---------------------------------------------------------------
        // AudioClipの登録をすべて削除
        static public void RemoveAudioClipAll()
        {
            if (manager != null)
            {
                manager.removeAudioClipAll();
            }
        }

        // ---------------------------------------------------------------
        // ラベルが登録済みか.
        static public bool FindLabel(string name)
        {
            if (manager != null)
            {
                return manager.findLabel(name);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // 登録済みのカテゴリ名か.
        static public bool FindCategory(string name)
        {
            if (manager != null)
            {
                return manager.findCategory(name);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // 登録済みのマスタ名か.
        static public bool FindMaster(string name)
        {
            if (manager != null)
            {
                return manager.findMaster(name);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // 指定したラベル名に関連するデータが削除可能か
        static public bool CanRemoveLabel(string labelName)
        {
            if (manager != null)
            {
                return manager.canRemoveLabel(labelName);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // 指定したラベル名に関連するAudioClipの参照をはずす
        static public bool UnsetAudioClipToLabel(string labelName)
        {
            if (manager != null)
            {
                return manager.unsetAudioClipToLabel(labelName);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してラベルに関連するAudioClipの参照をはずす
        static public void UnsetAudioClipToLabelLoadId(int loadId)
        {
            if (manager != null)
            {
                manager.unsetAudioClipToLabelLoadId(loadId);
            }
        }

        // ---------------------------------------------------------------
        // すべてのラベルに関連するAudioClipの参照をはずす
        static public void UnsetAudioClipToLabelAll()
        {
            if (manager != null)
            {
                manager.unsetAudioClipToLabelAll();
            }
        }

        // ---------------------------------------------------------------
        // 指定したラベル名に関連するデータを削除する
        static public bool RemoveLabel(string labelName)
        {
            if (manager != null)
            {
                return manager.removeLabel(labelName);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してラベルに関連するデータを削除
        static public void RemoveLabelLoadId(int loadId)
        {
            if (manager != null)
            {
                manager.removeLabelLoadId(loadId);
            }
        }

        // ---------------------------------------------------------------
        // すべてのラベルに関連するデータを削除.
        static public void RemoveLabelAll()
        {
            if (manager != null)
            {
                manager.removeLabelAll();
            }
        }

        // ---------------------------------------------------------------
        // すべての情報を削除.
        static public void RemoveAll()
        {
            if (manager != null)
            {
                manager.removeAll();
            }
        }


        // ---------------------------------------------------------------
        // ランダムソースの参照をやり直す.
        static public void UpdateRandomSourceInfo(string labelName)
        {
            if (manager != null)
            {
                manager.updateRandomSourceInfo(labelName);
            }
        }

        // ---------------------------------------------------------------
        // ランダムソースの参照をやり直す(全部).
        static public void UpdateRandomSourceInfoAll()
        {
            if ( manager != null )
            {
                manager.updateRandomSourceInfoAll();
            }
        }

        // ---------------------------------------------------------------
        // 指定したラベルのオーディオを事前にロードする(Preload Audio Dataのチェックがはずれているものだけ有効)
        static public void LoadAudioData(string labelName)
        {
            if (manager != null)
            {
                manager.loadAudioData(labelName);
            }
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してオーディオを事前にロード(Preload Audio Dataのチェックがはずれているものだけ有効)
        static public void LoadAudioDataLoadId(int loadId)
        {
            if (manager != null)
            {
                manager.loadAudioDataLoadId(loadId);
            }
        }

        // ---------------------------------------------------------------
        // 指定したラベルのオーディオをアンロードする(Preload Audio Dataのチェックがはずれているものだけ有効)
        static public void UnloadAudioData(string labelName)
        {
            if (manager != null)
            {
                manager.unloadAudioData(labelName);
            }
        }

        // ---------------------------------------------------------------
        // すべてのオーディオをアンロード(Preload Audio Dataのチェックがはずれているものだけ有効)
        static public void UnloadAudioDataAll()
        {
            if (manager != null)
            {
                manager.unloadAudioDataAll();
            }
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してオーディオをアンロード(Preload Audio Dataのチェックがはずれているものだけ有効)
        static public void UnloadAudioDataLoadId(int loadId)
        {
            if (manager != null)
            {
                manager.unloadAudioDataLoadId(loadId);
            }
        }

        // ---------------------------------------------------------------
        // XML読み込み共通ステータス
        static public AudioDefine.LOAD_XML_STATUS GetLoadXmlStatus()
        {
            if (manager != null)
            {
                return manager.getLoadXmlStatus();
            }
            return AudioDefine.LOAD_XML_STATUS.ERROR;
        }

        // ---------------------------------------------------------------
        // Json読み込み共通ステータス
        static public AudioDefine.LOAD_JSON_STATUS GetLoadJsonStatus()
        {
            if (manager != null)
            {
                return manager.getLoadJsonStatus();
            }
            return AudioDefine.LOAD_JSON_STATUS.ERROR;
        }

        // ---------------------------------------------------------------
        // Master設定ファイルを読み込む.
        static public bool LoadMasterXml(Stream xml, Stream xsd = null)
        {
            if (manager != null)
            {
                return manager.loadMasterXml(xml, xsd);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // Category設定ファイルを読み込む.
        static public bool LoadCategoryXml(Stream xml, Stream xsd = null)
        {
            if (manager != null)
            {
                return manager.loadCategoryXml(xml, xsd);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // Label設定ファイルを読み込む.
        static public bool LoadLabelXml(int loadId, Stream xml, Stream xsd = null)
        {
            if (manager != null)
            {
                return manager.loadLabelXml(loadId, xml, xsd);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // 手動でダッキングをかける
        static public void SetDucking(string categoryName, float targetVolumeFactor, float fadeTime)
        {
            if (manager != null)
            {
                manager.setDucking(categoryName, targetVolumeFactor, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // 手動でダッキング解除
        static public void ResetDucking(string categoryName, float fadeTime)
        {
            if (manager != null)
            {
                manager.resetDucking(categoryName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // 手動で全カテゴリのダッキング解除
        static public void ResetDuckingAll(float fadeTime)
        {
            if (manager != null)
            {
                manager.resetDuckingAll(fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // 強制でダッキング解除
        static public void ForceResetDucking(string categoryName, float fadeTime)
        {
            if (manager != null)
            {
                manager.forceResetDucking(categoryName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // 強制で全カテゴリのダッキング解除
        static public void ForceResetDuckingAll(float fadeTime)
        {
            if (manager != null)
            {
                manager.forceResetDuckingAll(fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // Play
        static public int Play(string labelName, float delay = -1)
        {
            if (manager != null)
            {
                return manager.play(labelName, delay);
            }
            return AudioDefine.INSTANCE_ID_ERROR;
        }

        // ---------------------------------------------------------------
        // PlayOption
        static public int PlayOption(string labelName, float volume, float fadeTime, float pan, int pitch, float delay = -1)
        {
            if (manager != null)
            {
                return manager.playOption(labelName, volume, fadeTime, pan, pitch, delay);
            }
            return AudioDefine.INSTANCE_ID_ERROR;
        }


        // ---------------------------------------------------------------
        // Prepare
        static public int Prepare(string labelName)
        {
            if (manager != null)
            {
                return manager.prepare(labelName);
            }
            return AudioDefine.INSTANCE_ID_ERROR;
        }

        // ---------------------------------------------------------------
        // Prepare
        static public int PrepareOption(string labelName, float volume, float fadeTime, float pan, int pitch)
        {
            if (manager != null)
            {
                return manager.prepareOption(labelName, volume, fadeTime, pan, pitch, false);
            }
            return AudioDefine.INSTANCE_ID_ERROR;
        }

        // ---------------------------------------------------------------
        // Prepareしたインスタンスを再生開始.
        static public void PlayInstance(int instanceId, float delay = -1)
        {
            if (manager != null)
            {
                manager.playInstance(instanceId, delay);
            }
        }

        // ---------------------------------------------------------------
        // 追従させるオブジェクトを指定して再生(3Dサウンド指定ラベルのみ有効).
        static public int Play3D(string labelName, GameObject target, float delay = -1)
        {
            if (manager != null)
            {
                return manager.play3D(labelName, target, delay);
            }
            return AudioDefine.INSTANCE_ID_ERROR;
        }

        // ---------------------------------------------------------------
        // 位置を指定して再生開始(3Dサウンド指定ラベルのみ有効).
        static public int Play3D(string labelName, Vector3 position, float delay = -1)
        {
            if (manager != null)
            {
                return manager.play3D(labelName, position, delay);
            }
            return AudioDefine.INSTANCE_ID_ERROR;
        }

        // ---------------------------------------------------------------
        // Transformを指定して再生開始(3Dサウンド指定ラベルのみ有効).
        static public int Play3D(string labelName, Transform target, float delay = -1)
        {
            if (manager != null)
            {
                return manager.play3D(labelName, target, delay);
            }
            return AudioDefine.INSTANCE_ID_ERROR;
        }

        // ---------------------------------------------------------------
        // 3Dサウンド設定されているラベルを強制的に2Dで再生する.
        static public int Play2D(string labelName, float delay = -1)
        {
            if (manager != null)
            {
                return manager.play2D(labelName, delay);
            }
            return AudioDefine.INSTANCE_ID_ERROR;
        }

        // ---------------------------------------------------------------
        // インスタンスのポジションを追従させるオブジェクトを指定(3Dサウンド指定ラベルのみ有効).
        static public void SetTrackingObject(int instanceId, GameObject target)
        {
            if (manager != null)
            {
                manager.setTrackingObject(instanceId, target);
            }
        }


        // ---------------------------------------------------------------
        // インスタンスのポジションを追従させるオブジェクトを指定(3Dサウンド指定ラベルのみ有効).
        static public void SetTrackingObject(int instanceId, Transform target)
        {
            if (manager != null)
            {
                manager.setTrackingObject(instanceId, target);
            }
        }

        // ---------------------------------------------------------------
        // 停止.
        static public void Stop(int instanceId, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.stop(instanceId, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // 停止.
        static public void StopLabel(string labelName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.stopLabel(labelName, fadeTime);
            }
        }

        // ----------------------------------------------------------------------
        // 全インスタンス停止.
        static public void StopAll(float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.stopAll(fadeTime);
            }
        }
        
        // ----------------------------------------------------------------------
        // ポーズ設定.
        static public void OnPause(int instanceId, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.onPause(instanceId, fadeTime);
            }
        }

        // ----------------------------------------------------------------------
        // 全インスタンスポーズ設定.
        static public void OnPauseAll(float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.onPauseAll(fadeTime);
            }
        }

        // ----------------------------------------------------------------------
        // ポーズ解除.
        static public void OffPause(int instanceId, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.offPause(instanceId, fadeTime);
            }
        }

        // ----------------------------------------------------------------------
        // 全インスタンスポーズ解除.
        static public void OffPauseAll(float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.offPauseAll(fadeTime);
            }
        }

        // ----------------------------------------------------------------------
        // ボリューム設定.
        static public void SetVolume(int instanceId, float newVolume, float moveTime)
        {
            if (manager != null)
            {
                manager.setVolume(instanceId, newVolume, moveTime);
            }
        }

        // ----------------------------------------------------------------------
        // ラベルの再生中インスタンスを一括でボリューム設定
        static public void SetVolume(string labelName, float newVolume, float moveTime)
        {
            if (manager != null)
            {
                manager.setVolume(labelName, newVolume, moveTime);
            }
        }

        // ----------------------------------------------------------------------
        // ピッチ設定.
        static public void SetPitch(int instanceId, int newPitch, float moveTime)
        {
            if (manager != null)
            {
                manager.setPitch(instanceId, newPitch, moveTime);
            }
        }

        // ----------------------------------------------------------------------
        // ラベルの再生中インスタンスを一括でピッチ設定
        static public void SetPitch(string labelName, int newPitch, float moveTime)
        {
            if (manager != null)
            {
                manager.setPitch(labelName, newPitch, moveTime);
            }
        }

        // ----------------------------------------------------------------------
        // パン設定.
        static public void SetPan(int instanceId, float newPan, float moveTime)
        {
            if (manager != null)
            {
                manager.setPan(instanceId, newPan, moveTime);
            }
        }

        // ----------------------------------------------------------------------
        // ラベルの再生中インスタンスを一括でピッチ設定
        static public void SetPan(string labelName, float newPan, float moveTime)
        {
            if (manager != null)
            {
                manager.setPan(labelName, newPan, moveTime);
            }
        }

        // ----------------------------------------------------------------------
        // 位置を変更(SpatialBlendが0以外のときに有効).
        static public void SetPosition(int instanceId, Vector3 position)
        {
            if (manager != null)
            {
                manager.setPosition(instanceId, position);
            }
        }

        // ----------------------------------------------------------------------
        // 位置を変更(SpatialBlendが0以外のときに有効).
        static public void SetPosition(string labelName, Vector3 position)
        {
            if (manager != null)
            {
                manager.setPosition(labelName, position);
            }
        }

        // ----------------------------------------------------------------------
        // 再生位置をリセット.
        static public void ResetPlayPosition(string labelName)
        {
            if (manager != null)
            {
                manager.resetPlayPosition(labelName);
            }
        }

        // ---------------------------------------------------------------
        // すべての再生位置をリセット.
        static public void ResetPlayPositionAll()
        {
            if (manager != null)
            {
                manager.resetPlayPositionAll();
            }
        }

        // ---------------------------------------------------------------
        // 現在のインスタンスボリュームを取得.
        static public float GetInstanceVolume(int instanceId)
        {
            if (manager != null)
            {
                return manager.getInstanceVolume(instanceId);
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // 最終的なインスタンスボリュームを取得.
        static public float GetInstanceCalcVolume(int instanceId)
        {
            if (manager != null)
            {
                return manager.getInstanceCalcVolume(instanceId);
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // マスターボリュームを設定.
        static public void SetMasterVolume(string masterName, float volume, float moveTime = 0)
        {
            if (manager != null)
            {
                manager.setMasterVolume(masterName, volume, moveTime);
            }
        }

        // ---------------------------------------------------------------
        // 現在のマスターボリュームを取得.
        static public float GetMasterVolume(string masterName)
        {
            if (manager != null)
            {
                return manager.getMasterVolume(masterName);
            }
            return 1;
        }

        // ---------------------------------------------------------------
        // カテゴリボリュームを設定
        static public void SetCategoryVolume(string categoryName, float volume, float moveTime = 0)
        {
            if (manager != null)
            {
                manager.setCategoryVolume(categoryName, volume, moveTime);
            }
        }

        // ---------------------------------------------------------------
        // 現在のカテゴリボリュームを取得.
        static public float GetCategoryVolume(string categoryName)
        {
            if (manager != null)
            {
                return manager.getCategoryVolume(categoryName);
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // 現在のラベルボリュームを取得.
        static public float GetLabelVolume(string labelName)
        {
            if (manager != null)
            {
                return manager.getCategoryVolume(labelName);
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // マスターを指定して停止.
        static public void StopMaster(string masterName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.stopMaster(masterName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // マスターを指定してポーズ設定.
        static public void OnPauseMaster(string masterName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.onPauseMaster(masterName, fadeTime);
            }
        }


        // ---------------------------------------------------------------
        // マスターを指定してポーズ解除.
        static public void OffPauseMaster(string masterName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.offPauseMaster(masterName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // カテゴリを指定して停止.
        static public void StopCategory(string categoryName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.stopCategory(categoryName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // ラベル単位で一時停止.
        static public void OnPauseLabel(string labelName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.onPauseLabel(labelName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // ラベル単位で一時停止解除.
        static public void OffPauseLabel(string labelName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.offPauseLabel(labelName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // カテゴリを指定してポーズ設定.
        static public void OnPauseCategory(string categoryName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.onPauseCategory(categoryName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // カテゴリを指定してポーズ解除.
        static public void OffPauseCategory(string categoryName, float fadeTime = -1)
        {
            if (manager != null)
            {
                manager.offPauseCategory(categoryName, fadeTime);
            }
        }

        // ---------------------------------------------------------------
        // インスタンスのステータスを取得.
        static public AudioDefine.INSTANCE_STATUS GetInstanceStatus(int instanceId)
        {
            if (manager != null)
            {
                return manager.getInstanceStatus(instanceId);
            }
            return AudioDefine.INSTANCE_STATUS.STOP;
        }

        // ---------------------------------------------------------------
        // ラベルが再生中か.
        static public bool IsPlayingLabel(string labelName)
        {
            if (manager != null)
            {
                return manager.isPlayingLabel(labelName);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // 読み込んでいるラベルの総数を取得
        static public int GetLabelNum()
        {
            if (manager != null)
            {
                return manager.getLabelNum();
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // 読み込んでいるラベルの名前リストを取得
        static public string[] GetLabelNameList()
        {
            if (manager != null)
            {
                return manager.getLabelNameList();
            }
            return null;
        }

        // ---------------------------------------------------------------
        // カテゴリ数を取得
        static public int GetCategoryNum()
        {
            if (manager != null)
            {
                return manager.getCategoryNum();
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // カテゴリ名リストを取得
        static public string[] GetCategoryNameList()
        {
            if (manager != null)
            {
                return manager.getCategoryNameList();
            }
            return null;
        }

        // ---------------------------------------------------------------
        // マスター数を取得
        static public int GetMasterNum()
        {
            if (manager != null)
            {
                return manager.getMasterNum();
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // マスター名リストを取得
        static public string[] GetMasterNameList()
        {
            if (manager != null)
            {
                return manager.getMasterNameList();
            }
            return null;
        }

        // ---------------------------------------------------------------
        // ラベルに設定されているカテゴリ名を取得.
        static public string GetCategoryNameSettingOfLabel(string labelName)
        {
            if (manager != null)
            {
                return manager.getCategoryNameSettingOfLabel(labelName);
            }
            return null;
        }

        // ---------------------------------------------------------------
        // カテゴリに設定されているマスター名を取得.
        static public string GetMasterNameSettingOfCategory(string categoryName)
        {
            if (manager != null)
            {
                return manager.getMasterNameSettingOfCategory(categoryName);
            }
            return null;
        }

        // ---------------------------------------------------------------
        // 現在の再生時間を取得(総再生時間ではなく波形上何秒の位置か).
        static public float GetPlayTime(int instanceId)
        {
            if (manager != null)
            {
                return manager.getPlayTime(instanceId);
            }
            return -1;  // 既に停止
        }

        // ---------------------------------------------------------------
        // 現在の再生サンプル位置を取得.
        static public int GetPlaySamples(int instanceId)
        {
            if (manager != null)
            {
                return manager.getPlaySamples(instanceId);
            }
            return -1;  // 既に停止
        }

        // ----------------------------------------------------------------------
        // 再生位置を設定(波形上何秒の位置か).
        static public void SetTime(int instanceId, float time)
        {
            if (manager != null)
            {
                manager.setTime(instanceId, time);
            }
        }

        // ----------------------------------------------------------------------
        // 再生位置を設定(サンプルで指定).
        static public void SetTimeSamples(int instanceId, int samples)
        {
            if (manager != null)
            {
                manager.setTimeSamples(instanceId, samples);
            }
        }

        // ---------------------------------------------------------------
        // ミュート設定.
        static public void SetMute(bool onMute)
        {
            if (manager != null)
            {
                manager.setMute(onMute);
            }
        }

        // ---------------------------------------------------------------
        // ミュート設定状態を取得.
        static public bool GetMuteStatus()
        {
            if (manager != null)
            {
                return manager.getMuteStatus();
            }
            return false;
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してラベルのAudioClip名リストを取得
        static public string[] GetAudioClipNameLoadId(int loadId)
        {
            if (manager != null)
            {
                return manager.getAudioClipNameLoadId(loadId);
            }
            return null;
        }

        // ---------------------------------------------------------------
        // すべてのラベルのAudioClip名リストを取得
        static public string[] GetAudioClipNameAll()
        {
            if (manager != null)
            {
                return manager.getAudioClipNameAll();
            }
            return null;
        }

        // ---------------------------------------------------------------
        // 指定したラベルのAudioClip名リストを取得
        static public string GetAudioClipName(string labelName)
        {
            if (manager != null)
            {
                return manager.getAudioClipName(labelName);
            }
            return null;
        }

        // ---------------------------------------------------------------
        // 指定したラベルのAudioClip名リストを取得
        static public string[] GetAudioClipNames(string labelName)
        {
            if (manager != null)
            {
                return manager.getAudioClipNames(labelName);
            }
            return null;
        }

        // ---------------------------------------------------------------
        // 指定したラベルのRandomSource(ラベル名)を取得
        static public string[] GetRandomSourceNames(string labelName)
        {
            if (manager != null)
            {
                return manager.getRandomSourceNames(labelName);
            }
            return null;
        }

        // ---------------------------------------------------------------
        // ロードIDを指定してラベルにAuidoClipを割り当て
        static public void SetAudioClipToLabelLoadId(int loadId)
        {
            if (manager != null)
            {
                manager.setAudioClipToLabelLoadId(loadId);
            }
        }

        // ---------------------------------------------------------------
        // すべてのラベルにAuidoClipを割り当て
        static public void SetAudioClipToLabelAll()
        {
            if (manager != null)
            {
                manager.setAudioClipToLabelAll();
            }
        }

        // ---------------------------------------------------------------
        // 指定したラベルにAuidoClipを割り当て
        static public void SetAudioClipToLabel(string labelName)
        {
            if (manager != null)
            {
                manager.setAudioClipToLabel(labelName);
            }
        }

        // ---------------------------------------------------------------
        // 指定したラベルにAndroidNative再生用のファイルパスをセットしてロード
        static public void SetAndroidNativeToLabel(string labelName, string filePath, string className, string funcName)
        {
            if (manager != null)
            {
                manager.setAndroidNativeToLabel(labelName, filePath, className, funcName);
            }
        }

        // ---------------------------------------------------------------
        // すべてのオブジェクトプールをクリア
        static public void ClearObjectPool()
        {
            if (manager != null)
            {
                manager.clearObjectPool();
            }
        }

        // ---------------------------------------------------------------
        // ラベルの総再生時間を秒単位で取得
        static public float GetLabelLength(string labelName)
        {
            if (manager != null)
            {
                return manager.getLabelLength(labelName);
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // ラベルの総再生時間をサンプル単位で取得
        static public int GetLabelSamples(string labelName)
        {
            if (manager != null)
            {
                return manager.getLabelSamples(labelName);
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // 再生中インスタンスのスペクトラムを取得
        static public bool GetSpectrumData(int instanceId, float[] sample, int channel, FFTWindow window)
        {
            if ( manager != null )
            {
                return manager.getSpectrumData(instanceId, sample, channel, window);
            }
            return false;
        }

#if UNITY_EDITOR
        static public AudioLabelSettings GetLabelInfo(string name)
        {
            if (manager != null)
            {
                return manager.getLabelInfo(name);
            }
            return null;
        }

        static public AudioCategorySettings GetCategoryInfo(string name)
        {
            if (manager != null)
            {
                return manager.getCategoryInfo(name);
            }
            return null;
        }

        static public AudioMasterSettings GetMasterInfo(string name)
        {
            if (manager != null)
            {
                return manager.getMasterInfo(name);
            }
            return null;
        }

        static public List<SoundLabelInfo> GetLabelInfoList()
        {
            if (manager != null)
            {
                return manager.getLabelInfoList();
            }
            return null;
        }

        static public Audio3DSettings GetAudio3DSettingsInfo(string name)
        {
            if(manager != null)
            {
                return manager.getAudio3DSettingsInfo(name);
            }
            return null;
        }

        // ---------------------------------------------------------------
        // エディタ用にとっておいたScriptableObjectへパラメータを上書き
        static public bool SaveAudio3DSettingsParam(Audio3DSettings audio3d)
        {
            if (manager != null)
            {
                return manager.saveAudio3DSettingsParam(audio3d);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // エディタ用にとっておいたScriptableObjectの状態に戻す
        static public bool UndoAudio3DSettingsParam(Audio3DSettings audio3d)
        {
            if (manager != null)
            {
                return manager.undoAudio3DSettingsParam(audio3d);
            }
            return false;
        }

        // ---------------------------------------------------------------
        // インスタンスを検索してspatialNameが名前が一致する3Dパラメータを更新する
        static public void UpdateAudio3DSettings(Audio3DSettings audio3d)
        {
            if (manager != null)
            {
                manager.updateAudio3DSettings(audio3d);
            }
        }


        static public List<string> GetLog()
        {
            if (manager != null)
            {
                return manager.getLog();
            }
            return null;
        }

        static public List<string> GetTableLog()
        {
            if (manager != null)
            {
                return manager.getTableLog();
            }
            return null;
        }

        static public HashSet<string> GetCallLog()
        {
            if (manager != null)
            {
                return manager.getCallLog();
            }
            return null;
        }

        static public void ClearCallLog()
        {
            if (manager != null)
            {
                manager.clearCallLog();
            }
        }

        // ---------------------------------------------------------------
        // 再生情報一覧を削除(Editor用)
        static public void SoundToolPlayListClear()
        {
            if (manager != null)
            {
                manager.soundToolPlayListClear();
            }
        }

        // ---------------------------------------------------------------
        // ログをクリア(Editor用)
        static public void SoundToolLogsClear()
        {
            if (manager != null)
            {
                manager.soundToolLogsClear();
            }
        }

        // ---------------------------------------------------------------
        // USndToolログに出力(Editor用)
        static public void AddLogA(string str)
        {
            if (manager != null)
            {
                manager.AddLog(str);
            }
        }

        // ---------------------------------------------------------------
        // USndデバッグがアクティブか(Editor用)
        static public bool IsOnDebug()
        {
            if ( manager != null )
            {
                return manager.IsActiveTool;
            }
            return false;
        }

        // ---------------------------------------------------------------
        // 3D設定数を取得
        static public int GetAudio3DSettingsNum()
        {
            if (manager != null)
            {
                return manager.getAudio3DSettingsNum();
            }
            return 0;
        }

        // ---------------------------------------------------------------
        // 3D設定名リストを取得
        static public string[] GetAudio3DSettingsNameList()
        {
            if (manager != null)
            {
                return manager.getAudio3DSettingsNameList();
            }
            return null;
        }

#endif
    }

}