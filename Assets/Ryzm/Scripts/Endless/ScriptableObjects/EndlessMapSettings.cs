using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    [CreateAssetMenu(fileName = "EndlessMapSettings", menuName = "ScriptableObjects/EndlessMapSettings", order = 2)]
    public class EndlessMapSettings : ScriptableObject
    {
        public List<MapSettings> settings = new List<MapSettings>();
    }
}