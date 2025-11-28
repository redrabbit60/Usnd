using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SkySoundDesigner
{
    /// <summary>
    /// Unity Audio APIを使用した高機能オーディオプレーヤー
    /// 同時再生、USndパラメータ対応
    /// </summary>
    public class UnityAudioPlayer : MonoBehaviour
    {
        private Dictionary<int, AudioSource> activeSources = new Dictionary<int, AudioSource>();
        private int nextInstanceId = 0;
        
        /// <summary>
        /// USndパラメータ付きで音声を再生
        /// </summary>
        public int PlayWithParameters(
            AudioClip clip, 
            bool loop, 
            float volume, 
            float pan, 
            int pitchCent, 
            float delay, 
            float fadeInTime)
        {
            if (clip == null)
            {
                Debug.LogWarning("AudioClipがnullです");
                return -1;
            }
            
            // 新しいAudioSourceを生成
            GameObject sourceObj = new GameObject($"Audio_{nextInstanceId}_{clip.name}");
            sourceObj.transform.SetParent(transform);
            AudioSource source = sourceObj.AddComponent<AudioSource>();
            
            // 基本パラメータ設定
            source.clip = clip;
            source.loop = loop;
            source.volume = fadeInTime > 0 ? 0 : volume;
            source.panStereo = Mathf.Clamp(pan, -1f, 1f);
            source.playOnAwake = false;
            
            // Pitchをセント単位から変換
            // 100cent = 1半音 = 2^(1/12) ≈ 1.059倍
            float semitones = pitchCent / 100f;
            source.pitch = Mathf.Pow(2f, semitones / 12f);
            
            // 管理に追加
            int instanceId = nextInstanceId++;
            activeSources[instanceId] = source;
            
            // 再生開始
            if (delay > 0)
            {
                source.PlayDelayed(delay);
            }
            else
            {
                source.Play();
            }
            
            // フェードイン
            if (fadeInTime > 0)
            {
                StartCoroutine(FadeIn(source, instanceId, volume, fadeInTime));
            }
            
            Debug.Log($"再生開始: {clip.name} [ID:{instanceId}, Loop:{loop}, Vol:{volume:F2}, Pan:{pan:F2}, Pitch:{pitchCent}cent]");
            
            return instanceId;
        }
        
        /// <summary>
        /// 指定インスタンスを停止
        /// </summary>
        public void Stop(int instanceId, float fadeOutTime = 0)
        {
            if (activeSources.TryGetValue(instanceId, out AudioSource source))
            {
                if (fadeOutTime > 0)
                {
                    StartCoroutine(FadeOut(source, instanceId, fadeOutTime));
                }
                else
                {
                    source.Stop();
                    Destroy(source.gameObject);
                    activeSources.Remove(instanceId);
                    Debug.Log($"停止: ID:{instanceId}");
                }
            }
        }
        
        /// <summary>
        /// 全てのインスタンスを停止
        /// </summary>
        public void StopAll(float fadeOutTime = 0)
        {
            List<int> instanceIds = new List<int>(activeSources.Keys);
            foreach (int id in instanceIds)
            {
                Stop(id, fadeOutTime);
            }
        }
        
        /// <summary>
        /// 指定インスタンスが再生中か確認
        /// </summary>
        public bool IsPlaying(int instanceId)
        {
            if (activeSources.TryGetValue(instanceId, out AudioSource source))
            {
                return source.isPlaying;
            }
            return false;
        }
        
        /// <summary>
        /// アクティブなインスタンス数を取得
        /// </summary>
        public int GetActiveInstanceCount()
        {
            return activeSources.Count;
        }
        
        /// <summary>
        /// フェードイン処理
        /// </summary>
        private IEnumerator FadeIn(AudioSource source, int instanceId, float targetVolume, float duration)
        {
            float elapsed = 0;
            while (elapsed < duration && activeSources.ContainsKey(instanceId))
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(0, targetVolume, elapsed / duration);
                yield return null;
            }
            
            if (activeSources.ContainsKey(instanceId))
            {
                source.volume = targetVolume;
            }
        }
        
        /// <summary>
        /// フェードアウト処理
        /// </summary>
        private IEnumerator FadeOut(AudioSource source, int instanceId, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
                yield return null;
            }
            
            if (activeSources.ContainsKey(instanceId))
            {
                source.Stop();
                Destroy(source.gameObject);
                activeSources.Remove(instanceId);
                Debug.Log($"フェードアウト停止: ID:{instanceId}");
            }
        }
        
        /// <summary>
        /// リアルタイムでボリュームを変更
        /// </summary>
        public void SetVolume(int instanceId, float volume, float duration = 0)
        {
            if (activeSources.TryGetValue(instanceId, out AudioSource source))
            {
                if (duration > 0)
                {
                    StartCoroutine(ChangeVolume(source, volume, duration));
                }
                else
                {
                    source.volume = Mathf.Clamp01(volume);
                }
            }
        }
        
        private IEnumerator ChangeVolume(AudioSource source, float targetVolume, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                yield return null;
            }
            
            source.volume = targetVolume;
        }
        
        private void OnDestroy()
        {
            StopAll();
        }
    }
}

