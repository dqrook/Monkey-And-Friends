using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Monkey
{
    [CreateAssetMenu(fileName = "MonkeyEmotions", menuName = "ScriptableObjects/MonkeyEmotionsScriptableObject", order = 2)]
    public class MonkeyEmotionsScriptableObject : ScriptableObject
    {
        public List<MonkeyEmotionPrefab> emotions = new List<MonkeyEmotionPrefab>();

        public MonkeyEmotionPrefab GetEmotion(MonkeyEmotion emotion)
        {
            foreach(MonkeyEmotionPrefab emotionPrefab in emotions)
            {
                if(emotionPrefab.emotion == emotion)
                {
                    return emotionPrefab;
                }
            }
            return null;
        }
    }
}

