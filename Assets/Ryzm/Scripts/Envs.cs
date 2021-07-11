using UnityEngine;

namespace Ryzm
{
    [CreateAssetMenu(fileName = "Envs", menuName = "ScriptableObjects/Envs", order = 2)]
    public class Envs : ScriptableObject
    {
        public string apiUrl;
        public string getDragonsPath;
        public string breedDragonsPath;
        public string getMarketDragonsPath;
        public string breedDragonsTxHashPath;
        public string dragonIdsPath;
        public string dragonByIdPath;
        public string buyDragonTxHashPath;
        public string addDragonToMarketPath;
        public string removeDragonFromMarketPath;

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

        public string GetMarketDragonsApiUrl
        {
            get
            {
                return apiUrl + getMarketDragonsPath;
            }
        }

        public string BreedDragonsTxHashApiUrl
        {
            get
            {
                return apiUrl + breedDragonsTxHashPath;
            }
        }

        public string BuyDragonTxHashApiUrl
        {
            get
            {
                return apiUrl + buyDragonTxHashPath;
            }
        }

        public string AddDragonToMarketApiUrl
        {
            get
            {
                return apiUrl + addDragonToMarketPath;
            }
        }

        public string RemoveDragonFromMarketApiUrl
        {
            get
            {
                return apiUrl + removeDragonFromMarketPath;
            }
        }

        public string DragonIdsApiUrl(string account)
        {
            return apiUrl + dragonIdsPath + "?owner=" + account;
        }

        public string DragonByIdApiUrl(int id)
        {
            return apiUrl + dragonByIdPath + "?dragon_id=" + id.ToString();
        }
    }

}
