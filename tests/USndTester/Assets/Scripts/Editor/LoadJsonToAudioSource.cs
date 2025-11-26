using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using USnd;
using System.IO;

public class LoadJson : MonoBehaviour {

    [MenuItem("Assets/Load JSON To AudioSource")]
    static void LoadJson_ToAudioSource()
    {
        int selectCount = Selection.gameObjects.Length;
        if (selectCount == 0)
        {
            Debug.Log("ロード先のAudioSourceを選択してから実行してください。");
            return;
        }

        string loadFile = EditorUtility.OpenFilePanelWithFilters("ロードするJSONを選択", "", new string[] { "json", "json" });

        if (File.Exists(loadFile))
        {
            StreamReader sr = File.OpenText(loadFile);
            string json = sr.ReadToEnd();
            sr.Close();

            AudioSourceWrapper settings = JsonUtility.FromJson<AudioSourceWrapper>(json);


            for (int i = 0; i < selectCount; ++i)
            {
                GameObject obj = Selection.gameObjects[i];
                AudioSource source = obj.GetComponent<AudioSource>();

                if (source != null)
                {
                    if ( source.name.Equals(settings.spatialName))
                    {
                        source.name = settings.spatialName;
                        source.spatialBlend = settings.spatialBlend;
                        source.reverbZoneMix = settings.reverbZoneMix;
                        source.dopplerLevel = settings.dopplerLevel;
                        source.spread = (int)settings.spread;
                        source.rolloffMode = settings.rolloffMode;
                        source.minDistance = settings.minDistance;
                        source.maxDistance = settings.maxDistance;
                        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, settings.customRolloffCurve);
                        source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, settings.spatialBlendCurve);
                        source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, settings.reverbZoneMixCurve);
                        source.SetCustomCurve(AudioSourceCurveType.Spread, settings.spreadCurve);
						Debug.Log("読み込み完了：" + settings.spatialName);
					}
					else
					{
						Debug.Log("JSONのSpatialName：" + settings.spatialName + "とコピー先のSpatialName：" + source.name + "が一致しないので読み込みできません。");
					}
                }
            }
        }
    }

    [MenuItem("Assets/Load JSON To Audio3DSettings")]
    static void LoadJson_ToAudio3DSettings()
    {
        int selectCount = Selection.assetGUIDs.Length;
        if (selectCount == 0)
        {
            Debug.Log("ロード先のAudio3DSettingsを選択してから実行してください。");
            return;
        }

        string loadFile = EditorUtility.OpenFilePanelWithFilters("ロードするJSONを選択", "", new string[] { "json", "json" });

        if (File.Exists(loadFile))
        {
            StreamReader sr = File.OpenText(loadFile);
            string json = sr.ReadToEnd();
            sr.Close();

            AudioSourceWrapper settings = JsonUtility.FromJson<AudioSourceWrapper>(json);


            for (int i = 0; i < selectCount; ++i)
            {
                string guid = Selection.assetGUIDs[i];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                Audio3DSettings source = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Audio3DSettings)) as Audio3DSettings;

				if (source != null)
				{
					if (source.spatialName.Equals(settings.spatialName))
					{
						source.spatialBlend = settings.spatialBlend;
						source.reverbZoneMix = settings.reverbZoneMix;
						source.dopplerLevel = settings.dopplerLevel;
						source.spread = (int)settings.spread;
						source.rolloffMode = settings.rolloffMode;
						source.minDistance = settings.minDistance;
						source.maxDistance = settings.maxDistance;
						source.customRolloffCurve = new AnimationCurve();
						for (int j = 0; j < settings.customRolloffCurve.keys.Length; ++j)
						{
							source.customRolloffCurve.AddKey(settings.customRolloffCurve.keys[j]);
						}

						source.spatialBlendCurve = new AnimationCurve();
						for (int j = 0; j < settings.spatialBlendCurve.keys.Length; ++j)
						{
							source.spatialBlendCurve.AddKey(settings.spatialBlendCurve.keys[j]);
						}

						source.reverbZoneMixCurve = new AnimationCurve();
						for (int j = 0; j < settings.reverbZoneMixCurve.keys.Length; ++j)
						{
							source.reverbZoneMixCurve.AddKey(settings.reverbZoneMixCurve.keys[j]);
						}

						source.spreadCurve = new AnimationCurve();
						for (int j = 0; j < settings.spreadCurve.keys.Length; ++j)
						{
							source.spreadCurve.AddKey(settings.spreadCurve.keys[j]);
						}
						//                        source.spatialBlendCurve = settings.spatialBlendCurve;
						//                       source.reverbZoneMixCurve = settings.reverbZoneMixCurve;
						//                       source.spreadCurve = settings.spreadCurve;

						Debug.Log("読み込み完了：" + settings.spatialName);
					}
					else
					{
						Debug.Log("JSONのSpatialName：" + settings.spatialName + "とコピー先のSpatialName：" + source.name + "が一致しないので読み込みできません。");
					}
				}
            }
        }
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
        public AnimationCurve customRolloffCurve = null;

        public AnimationCurve spatialBlendCurve = null;

        public AnimationCurve reverbZoneMixCurve = null;

        public AnimationCurve spreadCurve = null;
    }
}
