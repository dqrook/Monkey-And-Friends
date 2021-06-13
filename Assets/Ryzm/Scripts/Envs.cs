using UnityEngine;

namespace Ryzm
{
    [CreateAssetMenu(fileName = "Envs", menuName = "ScriptableObjects/Envs", order = 2)]
    public class Envs : ScriptableObject
    {
        public string apiUrl;
        public string getDragonsPath;

        public string GetDragonsApiUrl
        {
            get
            {
                return apiUrl + getDragonsPath;
            }
        }
    }

}
