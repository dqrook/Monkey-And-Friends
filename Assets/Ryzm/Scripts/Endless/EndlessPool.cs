using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessPool : MonoBehaviour
    {
        public EndlessPoolPrefabsScriptableObject prefabsScriptableObject;
        
        List<PooledSection> pooledSections = new List<PooledSection>();
        List<PooledBarrier> pooledBarriers = new List<PooledBarrier>();

        List<PooledSection> possiblePooledSections = new List<PooledSection>();
        // contains only the barriers that an endless section could spawn
        List<PooledBarrier> possiblePooledBarriers = new List<PooledBarrier>();

        private static EndlessPool _instance;
        public static EndlessPool Instance { get { return _instance; } }

        List<SectionPrefab> SectionPrefabs 
        {
            get
            {
                return prefabsScriptableObject.sectionPrefabs;
            }
        }

        List<BarrierPrefab> BarrierPrefabs 
        {
            get
            {
                return prefabsScriptableObject.barrierPrefabs;
            }
        }

        void Awake()
        {
            if(_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            foreach(SectionPrefab item in SectionPrefabs)
            {
                for(int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledSections.Add(new PooledSection(obj, item.IsTurn, item.Type));
                }
            }

            foreach(BarrierPrefab item in BarrierPrefabs)
            {
                for(int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledBarriers.Add(new PooledBarrier(obj, item.Type));
                }
            }
        }

        public GameObject GetRandomBarrier(List<BarrierType> types)
        {
            return GetRandom(pooledBarriers, BarrierPrefabs, types);
        }

        public GameObject GetSpecifiedSection(SectionType type)
        {
            return GetRandom(pooledSections, SectionPrefabs, type);
        }

        public GameObject GetRandomSection(bool isTurn = false)
        {
            return GetRandom(pooledSections, SectionPrefabs, isTurn);
        }

        GameObject GetRandom(List<PooledSection> pooledSections, List<SectionPrefab> _prefabs, bool isTurn)
        {
            possiblePooledSections.Clear();
            foreach(PooledSection item in pooledSections)
            {
                if(item.isTurn == isTurn)
                {
                    possiblePooledSections.Add(item);
                }
            }

            EndlessUtils.Shuffle(possiblePooledSections);
            for(int i = 0; i < possiblePooledSections.Count; i++)
            {
                if(!possiblePooledSections[i].gameObject.activeInHierarchy)
                {
                    return possiblePooledSections[i].gameObject;
                }
            }
            
            foreach(SectionPrefab item in _prefabs)
            {
                if(item.expandable && item.IsTurn == isTurn)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledSections.Add(new PooledSection(obj));
                    return obj;
                }
            }
            return null;
        }

        GameObject GetRandom(List<PooledSection> pooledSections, List<SectionPrefab> _prefabs, SectionType type)
        {
            possiblePooledSections.Clear();
            foreach(PooledSection item in pooledSections)
            {
                if(item.type == type)
                {
                    possiblePooledSections.Add(item);
                }
            }

            EndlessUtils.Shuffle(possiblePooledSections);
            for(int i = 0; i < possiblePooledSections.Count; i++)
            {
                if(!possiblePooledSections[i].gameObject.activeInHierarchy)
                {
                    return possiblePooledSections[i].gameObject;
                }
            }
            
            foreach(SectionPrefab item in _prefabs)
            {
                if(item.Type == type)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledSections.Add(new PooledSection(obj));
                    return obj;
                }
            }
            return null;
        }

        GameObject GetRandom(List<PooledBarrier> pooledBarriers, List<BarrierPrefab> possibleBarriers, List<BarrierType> types)
        {
            possiblePooledBarriers.Clear();
            foreach(PooledBarrier item in pooledBarriers)
            {
                if(types.Contains(item.type))
                {
                    possiblePooledBarriers.Add(item);
                }
            }

            EndlessUtils.Shuffle(possiblePooledBarriers);
            for(int i = 0; i < possiblePooledBarriers.Count; i++)
            {
                if(!possiblePooledBarriers[i].gameObject.activeInHierarchy)
                {
                    return possiblePooledBarriers[i].gameObject;
                }
            }

            foreach(BarrierPrefab item in possibleBarriers)
            {
                if(item.expandable && types.Contains(item.Type))
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledBarriers.Add(new PooledBarrier(obj, item.Type));
                    return obj;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public abstract class ItemPrefab
    {
        public GameObject prefab;
        public int amount;
        public bool expandable;
    }

    [System.Serializable]
    public class SectionPrefab: ItemPrefab
    {
        public bool IsTurn
        {
            get
            {
                return prefab.GetComponent<EndlessSection>().isTurn;
            }
        }

        public SectionType Type
        {
            get
            {
                return prefab.GetComponent<EndlessSection>().type;
            }
        }
    }

    [System.Serializable]
    public class BarrierPrefab: ItemPrefab
    {
        public BarrierType Type
        {
            get
            {
                return prefab.GetComponent<EndlessBarrier>().type;
            }
        }
    }

    [System.Serializable]
    public abstract class PooledItem 
    {
        public GameObject gameObject;

        public PooledItem() {}

        public PooledItem(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }

    [System.Serializable]
    public class PooledSection: PooledItem
    {
        public bool isTurn;
        public SectionType type;

        public PooledSection(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.isTurn = false;
        }

        public PooledSection(GameObject gameObject, bool isTurn)
        {
            this.gameObject = gameObject;
            this.isTurn = isTurn;
        }

        public PooledSection(GameObject gameObject, bool isTurn, SectionType type)
        {
            this.gameObject = gameObject;
            this.isTurn = isTurn;
            this.type = type;
        }
    }

    [System.Serializable]
    public class PooledBarrier: PooledItem
    {
        public BarrierType type;

        public PooledBarrier(GameObject gameObject, BarrierType type)
        {
            this.gameObject = gameObject;
            this.type = type;
        }
    }
}
