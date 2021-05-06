using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessPool : MonoBehaviour
    {
        public List<SectionPrefab> sectionPrefabs = new List<SectionPrefab>();
        public List<BarrierPrefab> barrierPrefabs = new List<BarrierPrefab>();
        
        List<PooledSection> pooledSections = new List<PooledSection>();
        List<PooledBarrier> pooledBarriers = new List<PooledBarrier>();

        List<PooledSection> possiblePooledSections = new List<PooledSection>();
        // contains only the barriers that an endless section could spawn
        List<PooledBarrier> possiblePooledBarriers = new List<PooledBarrier>();

        private static EndlessPool _instance;
        public static EndlessPool Instance { get { return _instance; } }

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

            foreach(SectionPrefab item in sectionPrefabs)
            {
                for(int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledSections.Add(new PooledSection(obj, item.isTurn));
                }
            }

            foreach(BarrierPrefab item in barrierPrefabs)
            {
                for(int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledBarriers.Add(new PooledBarrier(obj, item.type));
                }
            }
        }

        public GameObject GetRandomBarrier(List<BarrierType> types)
        {
            return GetRandom(pooledBarriers, barrierPrefabs, types);
        }

        public GameObject GetRandomSection(bool isTurn = false)
        {
            return GetRandom(pooledSections, sectionPrefabs, isTurn);
        }

        GameObject GetRandom(List<PooledSection> pooledSections, List<SectionPrefab> _items, bool isTurn)
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
            
            foreach(SectionPrefab item in _items)
            {
                if(item.expandable && item.isTurn == isTurn)
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
                if(item.expandable && types.Contains(item.type))
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledBarriers.Add(new PooledBarrier(obj, item.type));
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
        public bool isTurn;
    }

    [System.Serializable]
    public class BarrierPrefab: ItemPrefab
    {
        public BarrierType type;
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
