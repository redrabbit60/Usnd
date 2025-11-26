using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAudioSourceJson : MonoBehaviour {

    [MenuItem("Assets/Create AudioSource To JSON")]
    static void CreateAudioSourceJsonText()
    {
        int selectCount = Selection.gameObjects.Length;
        string savePath;
        if (selectCount == 0)
        {
            Debug.Log("出力するAudioSourceを選択してから実行してください。");
            return;
        }
        else
        {
            savePath = EditorUtility.SaveFolderPanel("保存先を選択", "", "");
        }

        for (int i = 0; i < selectCount; ++i)
        {
            GameObject obj = Selection.gameObjects[i];
            AudioSource source = obj.GetComponent<AudioSource>();
            string fileName = obj.name;

            if (source != null)
            {
                AudioSourceWrapper wrapper = new AudioSourceWrapper();
                wrapper.spatialName = source.name;
                wrapper.spatialBlend = source.spatialBlend;
                wrapper.reverbZoneMix = source.reverbZoneMix;
                wrapper.dopplerLevel = source.dopplerLevel;
                wrapper.spread = (int)source.spread;
                wrapper.rolloffMode = source.rolloffMode;
                wrapper.minDistance = source.minDistance;
                wrapper.maxDistance = source.maxDistance;
                wrapper.customRolloffCurve = source.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                wrapper.spatialBlendCurve = source.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
                wrapper.reverbZoneMixCurve = source.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix);
                wrapper.spreadCurve = source.GetCustomCurve(AudioSourceCurveType.Spread);
                


                string json = JsonUtility.ToJson(wrapper);
                string saveFile = savePath + "\\" + fileName + ".json";
                WriteText(json, saveFile);
                Debug.Log("保存完了：" + saveFile);
            }
        }
    }

    static void WriteText(string str, string savePath)
    {
        StreamWriter sw = new StreamWriter(savePath, false);
        sw.WriteLine(str);

        sw.Flush();
        sw.Close();
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
}
