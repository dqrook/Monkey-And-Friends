using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    [CreateAssetMenu(fileName = "DragonPrefabs", menuName = "ScriptableObjects/DragonPrefabs", order = 2)]
    public class DragonPrefabs : ScriptableObject
    {
        public List<DragonPrefab> prefabs;

        public DragonPrefab GetPrefabByHornType(int hornType)
        {
            foreach(DragonPrefab prefab in prefabs)
            {
                if(prefab.hornType == hornType)
                {
                    return prefab;
                }
            }
            return null;
        }

        public DragonPrefab GetPrefabByHornType(string hornType)
        {
            foreach(DragonPrefab prefab in prefabs)
            {
                if(prefab.hornType.ToString() == hornType)
                {
                    return prefab;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class DragonPrefab
    {
        public int hornType;
        public GameObject dragon;
    }
}
