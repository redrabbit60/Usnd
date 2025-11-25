using UnityEngine;
using System.Collections;

namespace USnd
{
    public partial class AudioManager : MonoBehaviour
    {
        public class AudioCategorySettings
        {
            public string categoryName;

            //[Range(0, 30)]
            public int maxPlaybacksNum = 0;

            //[Range(0, 1)]
            public float volume = 1;        // こちらはInspector,テーブルで変更するボリュームなのでプログラムから変更しない.

            public string masterName;
            AudioMasterSettings attachMaster;

            // programVolume, duckingVolumeはAwakeで1にするので編集はできない。現在の値をInspector上で見れるようにするためpublicにしてある.
            AudioParamUpdater volumeUpdater = new AudioParamUpdater();
            public float programVolume = 1;        // プログラム上のボリューム係数、実際に設定されるのはvolume * programVolumeの値.

            AudioParamUpdater duckingUpdater = new AudioParamUpdater();
            public float duckingVolume = 1;         // ダッキングのボリューム係数.

            public AudioCategorySettings()
            {
                programVolume = 1;
                duckingVolume = 1;
            }

            public void CopySettings(AudioCategorySettings src)
            {
                if (categoryName.CompareTo(src.categoryName) == 0)
                {
                    volume = src.volume;
                    maxPlaybacksNum = src.maxPlaybacksNum;
                    masterName = src.masterName;
                }
            }

            public void SetAttachMasterInstance(AudioMasterSettings master)
            {
                attachMaster = master;
            }

            public float GetVolumeFactor()
            {
                if (attachMaster == null) return programVolume;
                return volume * programVolume * duckingVolume * attachMaster.GetVolumeFactor();
            }

            public float GetCurrentVolume()
            {
                return programVolume;
            }

            public void SetVolumeUpdater(float start, float target, float time)
            {
                volumeUpdater.SetParam(start, target, time, false);
            }

            public void SetDuckingVolumeUpdater(float target, float time, bool isLow)
            {
                duckingUpdater.SetParam(duckingVolume, target, time, isLow);
            }

            public void ClearVolumeUpdater()
            {
                volumeUpdater.Clear();
            }

            public bool UpdateVolume()
            {
                if (volumeUpdater.active)
                {
                    programVolume = volumeUpdater.Update();
                    return true;
                }
                return false;
            }

            public bool UpdateDuckingVolume()
            {
                if (duckingUpdater.active)
                {
                    duckingVolume = duckingUpdater.Update();
                    return true;
                }
                return false;
            }
        }
    }
}

