using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessPool : MonoBehaviour
    {
        private static EndlessPool _instance;
        public List<PoolItem> items = new List<PoolItem>();
        public List<PooledItem> pooledSections = new List<PooledItem>();
        public List<BarrierPoolItem> barrierItems = new List<BarrierPoolItem>();
        public List<PooledBarrierItem> pooledBarriers = new List<PooledBarrierItem>();

        List<PooledBarrierItem> _pooledBarriers = new List<PooledBarrierItem>();

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

            foreach(PoolItem item in items)
            {
                for(int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledSections.Add(new PooledItem(obj));
                }
            }

            foreach(BarrierPoolItem item in barrierItems)
            {
                for(int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledBarriers.Add(new PooledBarrierItem(obj, item.type));
                }
            }
        }

        public GameObject GetRandomBarrier(List<BarrierType> types)
        {
            return GetRandom(pooledBarriers, barrierItems, types);
        }

        public GameObject GetRandomSection()
        {
            return GetRandom(pooledSections, items);
        }

        GameObject GetRandom(List<PooledItem> pooledItems, List<PoolItem> _items)
        {
            EndlessUtils.Shuffle(pooledItems);
            for(int i = 0; i < pooledItems.Count; i++)
            {
                if(!pooledItems[i].gameObject.activeInHierarchy)
                {
                    return pooledItems[i].gameObject;
                }
            }
            
            foreach(PoolItem item in _items)
            {
                if(item.expandable)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledItems.Add(new PooledItem(obj));
                    return obj;
                }
            }
            return null;
        }

        GameObject GetRandom(List<PooledBarrierItem> pooledBarriers, List<BarrierPoolItem> possibleBarriers, List<BarrierType> types)
        {
            _pooledBarriers.Clear();
            foreach(PooledBarrierItem item in pooledBarriers)
            {
                if(types.Contains(item.type))
                {
                    _pooledBarriers.Add(item);
                }
            }

            EndlessUtils.Shuffle(_pooledBarriers);
            for(int i = 0; i < _pooledBarriers.Count; i++)
            {
                if(!_pooledBarriers[i].gameObject.activeInHierarchy)
                {
                    return _pooledBarriers[i].gameObject;
                }
            }

            foreach(BarrierPoolItem item in possibleBarriers)
            {
                if(item.expandable && types.Contains(item.type))
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledBarriers.Add(new PooledBarrierItem(obj, item.type));
                    return obj;
                }
            }
            return null;
        }
    }

    public enum ItemType
    {
        Section,
        Barrier
    }

    [System.Serializable]
    public class PoolItem
    {
        public GameObject prefab;
        public int amount;
        public bool expandable;
    }

    [System.Serializable]
    public class BarrierPoolItem: PoolItem
    {
        public BarrierType type;
    }

    [System.Serializable]
    public class PooledItem 
    {
        public GameObject gameObject;

        public PooledItem() {}

        public PooledItem(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }

    [System.Serializable]
    public class PooledBarrierItem: PooledItem
    {
        public BarrierType type;

        public PooledBarrierItem(GameObject gameObject, BarrierType type)
        {
            this.gameObject = gameObject;
            this.type = type;
        }
    }

}
