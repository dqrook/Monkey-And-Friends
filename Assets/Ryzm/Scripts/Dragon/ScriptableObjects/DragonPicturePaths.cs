using UnityEngine;
using System.Collections.Generic;

namespace Ryzm.Dragon
{
    [CreateAssetMenu(fileName = "DragonPicturePaths", menuName = "ScriptableObjects/DragonPicturePaths", order = 2)]
    public class DragonPicturePaths : ScriptableObject
    {
        public List<DragonPicturePath> paths = new List<DragonPicturePath>();
    }

    [System.Serializable]
    public struct DragonPicturePath
    {
        public string desktopPath;
        public string bodyPath;
        public string wingPath;
        public string hornPath;
    }
}
