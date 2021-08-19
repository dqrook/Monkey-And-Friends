using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    [CreateAssetMenu(fileName = "MarketFilterMaps", menuName = "ScriptableObjects/MarketFilterMaps", order = 2)]
    public class MarketFilterMaps : ScriptableObject
    {
        public List<MarketFilter> filters = new List<MarketFilter>();

        // public string GetQueryString(FilterType type, string value)
        // {
        //     foreach(MarketFilter filter in filters)
        //     {
        //         if(filter.type == type)
        //         {
        //             return filter.GetQueryString(value);
        //         }
        //     }
        //     return "";
        // }
    }

    [System.Serializable]
    public struct MarketFilter 
    {
        public FilterType type;
        public string name;
        [HideInInspector]
        public string value;

        public string GetQueryString()
        {
            return name + "=" + value;
        }
    }

    public enum FilterType
    {
        PrimaryColor,
        SecondaryColor,
        BodyGenes,
        WingGenes,
        HornGenes,
        HornType
    }
}
