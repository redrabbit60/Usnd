using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace USnd
{
    public partial class AudioManager : MonoBehaviour
    {

        [System.Serializable]
        public class AudioLabelSettings
        {
            public enum BEHAVIOR
            {
                STEAL_OLDEST,
                JUST_FAIL,
                QUEUE,
            }

            //[Header("Volume")]
            //[Range(0, 1)]
            public float volume = 1;        // 実行しながら値変更したいので別枠でボリュームを持つ.

            //[Header("Category PlayBehavior")]
            public BEHAVIOR maxPlaybacksBehavior = BEHAVIOR.STEAL_OLDEST;   // カテゴリの発音数をオーバーしたときの動作.
            //[Range(0, 127)]
            public int priority = 64;
            public string categoryName;

            public string singleGroup = null;

            //[Header("Label PlayBehavior")]
            //[Range(0, 30)]
            public int maxPlaybacksNum = 0;

            public bool isStealOldest = true;       // ラベルの発音数をオーバーしたときに古いものを消すならtrue.

            //[Header("Unity Mixer")]
            public string unityMixerName = null;

            public string spatialGroup = null;

            //[Header("Start Delay")]
            //[Range(0, 30)]
            public float playStartDelay = 0;

            public float playInterval = 0;


            public float pan = 0;

            //[Header("Pitch")]
            //[Range(-1200, 1200)]
            public int pitchShiftCent = 0;

            //[Header("StartPosition")]
            public bool isPlayLastSamples = false;

            //[Header("Fade")]
            //[Range(0, 30)]
            public float fadeInTime = 0;
            //[Range(0, 30)]
            public float fadeOutTime = 0;
            //[Range(0, 30)]
            public float fadeInTimeOldSamples = 0;
            //[Range(0, 30)]
            public float fadeOutTimeOnPause = 0;
            //[Range(0, 30)]
            public float fadeInTimeOffPause = 0;

            //[Header("Random Volume")]
            public bool isVolumeRandom = false;
            public bool inconsecutiveVolume = false;
            //[Range(0, 1)]
            public float volumeRandomMin = 0;
            //[Range(0, 1)]
            public float volumeRandomMax = 0;
            //[Range(0, 1)]
            public float volumeRandomUnit = 0f;

            //[Header("Random Pitch")]
            public bool isPitchRandom = false;
            public bool inconsecutivePitch = false;
            //[Range(-1200, 1200)]
            public int pitchRandomMin = 0;
            //[Range(-1200, 1200)]
            public int pitchRandomMax = 0;
            //[Range(0, 1000)]
            public int pitchRandomUnit = 0;


            //[Header("Random Stereo Pan")]
            public bool isPanRandom = false;
            public bool inconsecutivePan = false;
            //[Range(-1, 1)]
            public float panRandomMin = 0;
            //[Range(-1, 1)]
            public float panRandomMax = 0;
            //[Range(0, 1)]
            public float panRandomUnit = 0f;

            //[Header("Random Source")]
            public bool isRandomPlay = false;
            public bool inconsecutiveSource = false;
            //public List<string> randomSource = new List<string>();
            public string[] randomSource = null;


            //[Header("Start Move Pitch")]
            public bool isMovePitch = false;
            //[Range(-1200, 1200)]
            public int pitchStart = 0;
            //[Range(-1200, 1200)]
            public int pitchEnd = 0;
            //[Range(0, 30)]
            public float pitchMoveTime = 0;


            //[Header("Start Move Stereo Pan")]
            public bool isMovePan = false;
            //[Range(-1, 1)]
            public float panStart = 0;
            //[Range(-1, 1)]
            public float panEnd = 0;
            //[Range(0, 30)]
            public float panMoveTime = 0;

            //[Header("Category Ducking")]
            //public List<string> duckingCategories = new List<string>();
            public string[] duckingCategories = null;
            //[Range(0, 30)]
            public float duckingStartTime = 0;
            //[Range(0, 30)]
            public float duckingEndTime = 0;
            //[Range(0, 1)]
            public float duckingVolumeFactor = 0;
            public bool autoRestoreDucking = true;
            public float restoreTime = -1;      // 0以上で指定時間になったら復帰する.


            public bool isAndroidNative = false;
            int androidSoundId = 0;
            public void SetAndroidSoundId(int soundId)
            {
                androidSoundId = soundId;
            }
            public int GetAndroidSoundId()
            {
                return androidSoundId;
            }

            //[Header("Loading Index")]
            public int loadId = 0;


            public string name = null;

            AudioCategorySettings attachCategory = null;


            public string clipName = null;

            public bool isLoop = false;


            public void SetLoop(bool loop)
            {
                isLoop = loop;
            }

            public bool GetLoop()
            {
                return isLoop;
            }

            public void SetClipName(string name)
            {
                clipName = name;
            }

            public string GetClipName()
            {
                return clipName;
            }

            public void SetAttachCategoryInstance(AudioCategorySettings category)
            {
                attachCategory = category;
            }

            public AudioCategorySettings GetAttachCategory()
            {
                return attachCategory;
            }

            public string GetCategoryName()
            {
                return categoryName;
            }

        }
    }
}