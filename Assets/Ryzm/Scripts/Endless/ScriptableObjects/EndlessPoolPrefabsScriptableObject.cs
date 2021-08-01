using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    [CreateAssetMenu(fileName = "EndlessPoolPrefabs", menuName = "ScriptableObjects/EndlessPoolPrefabsScriptableObject", order = 2)]
    public class EndlessPoolPrefabsScriptableObject : ScriptableObject
    {
        public MapType type;
        public List<SectionPrefab> sectionPrefabs = new List<SectionPrefab>();
        public List<BarrierPrefab> barrierPrefabs = new List<BarrierPrefab>();
        public List<EnvironmentPrefab> environmentPrefabs = new List<EnvironmentPrefab>();
        public List<WallPrefab> wallPrefabs = new List<WallPrefab>();
    }
}
