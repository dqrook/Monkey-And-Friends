using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    [System.Serializable]
    public class MarketDragonMetadata
    {
        public string id;
        public string price;
        public string media;
        public string owner;
    }

    [System.Serializable]
    public class MarketQueryGetResponse
    {
        public List<int> dragonIds;
        public List<MarketDragonMetadata> dragons;

        public static MarketQueryGetResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<MarketQueryGetResponse>(jsonString);
        }
    }

    [System.Serializable]
    public struct DragonCardMetadata
    {
        public string id;
        public string price;
        public string media;
        public string owner;
        public Texture image;

        public bool Initialized 
        {
            get
            {
                return image != null;
            }
        }
    }
}
