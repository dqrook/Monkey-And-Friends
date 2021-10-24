using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm
{
    public class PrefabLighting : MonoBehaviour
    {
        public UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        public Color skyColor;
        public Color equatorColor;
        public Color groundColor;
    }
}
