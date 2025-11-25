using UnityEngine;
using System.Collections;

namespace USnd
{


    [CreateAssetMenu(menuName = "USnd/ Create Audio3DSettings Instance")]
    public class Audio3DSettings : ScriptableObject
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
        public AnimationCurve customRolloffCurve = AnimationCurve.Linear(0, 1, 1, 0);

        public AnimationCurve spatialBlendCurve = new AnimationCurve(new Keyframe(0, 1));

        public AnimationCurve reverbZoneMixCurve = new AnimationCurve(new Keyframe(0, 1));

        public AnimationCurve spreadCurve = new AnimationCurve(new Keyframe(0, 0));

    }

}
