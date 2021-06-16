using UnityEngine;

namespace Ryzm
{
    [CreateAssetMenu(fileName = "Envs", menuName = "ScriptableObjects/Envs", order = 2)]
    public class Envs : ScriptableObject
    {
        public string apiUrl;
        public string getDragonsPath;
        public string breedDragonsPath;

        public string GetDragonsApiUrl
        {
            get
            {
                return apiUrl + getDragonsPath;
            }
        }

        public string BreedDragonsApiUrl
        {
            get
            {
                return apiUrl + breedDragonsPath;
            }
        }
    }

}
