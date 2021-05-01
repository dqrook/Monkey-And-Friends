using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessPool : MonoBehaviour
    {
        private static EndlessPool _instance;
        public List<PoolItem> items = new List<PoolItem>();
        public List<GameObject> pooledItems = new List<GameObject>();

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
                    pooledItems.Add(obj);
                }
            }
        }

        public GameObject GetRandom()
        {
            Utils.Shuffle(pooledItems);
            for(int i = 0; i < pooledItems.Count; i++)
            {
                if(!pooledItems[i].activeInHierarchy)
                {
                    return pooledItems[i];
                }
            }

            foreach(PoolItem item in items)
            {
                if(item.expandable)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledItems.Add(obj);
                    return obj;
                }
            }

            return null;
        }

    }

    [System.Serializable]
    public class PoolItem
    {
        public GameObject prefab;
        public int amount;
        public bool expandable;
    }

    public static class Utils
    {
        public static System.Random r = new System.Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while(n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
