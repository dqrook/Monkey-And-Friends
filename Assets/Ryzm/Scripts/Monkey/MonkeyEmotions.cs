using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Monkey
{
    public class MonkeyEmotions : MonoBehaviour
    {
        public MonkeyEmotionsScriptableObject emotionsScriptableObject;

        public MonkeyEmotionPrefab GetEmotion(MonkeyEmotion emotion)
        {
            return emotionsScriptableObject.GetEmotion(emotion);
        }
    }

    [System.Serializable]
    public class MonkeyEmotionPrefab
    {
        public MonkeyEmotion emotion;
        public Material eyes;
        public Material eyebrows;
        public Material mouth;
        public Material blush;
    }

    public enum MonkeyEmotion
    {
        Happy,
        Angry,
        Sad,
        Focused
    }
}
