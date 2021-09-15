using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    [CreateAssetMenu(fileName = "EndlessMonsterMetadata", menuName = "ScriptableObjects/EndlessMonsterMetadata", order = 2)]
    public class EndlessMonsterMetadata : ScriptableObject
    {
        public List<MonsterMetadata> monsterMetadatas = new List<MonsterMetadata>();

        public MonsterMetadata GetMonsterMetadata(MonsterType type)
        {
            foreach(MonsterMetadata metadata in monsterMetadatas)
            {
                if(metadata.monsterType == type)
                {
                    return metadata;
                }
            }
            return new MonsterMetadata();
        }
    }

    [System.Serializable]
    public struct MonsterMetadata
    {
        public int difficultyPoints;
        public MonsterType monsterType;
        public int physicalDamage;
        public int specialDamage;
    }

    public enum AttackType
    {
        Physical,
        Special
    }
}
