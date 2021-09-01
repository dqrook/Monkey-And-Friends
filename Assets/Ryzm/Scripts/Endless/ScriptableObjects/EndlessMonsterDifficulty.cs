using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    [CreateAssetMenu(fileName = "EndlessMonsterDifficulty", menuName = "ScriptableObjects/EndlessMonsterDifficulty", order = 2)]
    public class EndlessMonsterDifficulty : ScriptableObject
    {
        public List<MonsterDifficultyMap> difficultyMaps = new List<MonsterDifficultyMap>();

        public MonsterDifficultyMap GetMonsterDifficultyMap(MonsterType type)
        {
            foreach(MonsterDifficultyMap map in difficultyMaps)
            {
                if(map.type == type)
                {
                    return map;
                }
            }
            return new MonsterDifficultyMap();
        }
    }

    [System.Serializable]
    public struct MonsterDifficultyMap
    {
        public int difficultyPoints;
        public MonsterType type;
    }
}
