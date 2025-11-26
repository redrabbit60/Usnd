using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using USnd;

public class CreateAudio3DJson : MonoBehaviour {

    [MenuItem("Assets/Create Audio3DSettings To JSON")]
	static void CreateAudio3DSettingsJson()
    {
        int selectCount = Selection.assetGUIDs.Length;
        string savePath;
        if ( selectCount == 0 )
        {
            Debug.Log("出力するAudio3DSettingsを選択してから実行してください。");
            return;
        }
        else
        {
            savePath = EditorUtility.SaveFolderPanel("保存先を選択", "", "");
        }

        for(int i=0; i<selectCount; ++i)
        {
            string guid = Selection.assetGUIDs[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            Audio3DSettings settings = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Audio3DSettings)) as Audio3DSettings;
            if ( settings != null )
            {
                AudioSourceWrapper wrapper = new AudioSourceWrapper();

                wrapper.spatialName = settings.spatialName;
                wrapper.spatialBlend = settings.spatialBlend;
                wrapper.reverbZoneMix = settings.reverbZoneMix;
                wrapper.dopplerLevel = settings.dopplerLevel;
                wrapper.spread = settings.spread;
                wrapper.rolloffMode = settings.rolloffMode;
                wrapper.minDistance = settings.minDistance;
                wrapper.maxDistance = settings.maxDistance;
                wrapper.customRolloffCurve = settings.customRolloffCurve;
                wrapper.spatialBlendCurve = settings.spatialBlendCurve;
                wrapper.reverbZoneMixCurve = settings.reverbZoneMixCurve;
                wrapper.spreadCurve = settings.spreadCurve;

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
