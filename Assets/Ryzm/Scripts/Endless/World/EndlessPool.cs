using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessPool : MonoBehaviour
    {
        #region Public Variables
        public List<EndlessPoolPrefabsScriptableObject> prefabs = new List<EndlessPoolPrefabsScriptableObject>();
        public static EndlessPool Instance { get { return _instance; } }
        #endregion
        
        #region Private Variables
        List<PooledSection> pooledSections = new List<PooledSection>();
        List<PooledBarrier> pooledBarriers = new List<PooledBarrier>();
        List<PooledEnvironment> pooledEnvironments = new List<PooledEnvironment>();
        List<PooledWall> pooledWalls = new List<PooledWall>();
        List<PooledMonster> pooledMonsters = new List<PooledMonster>();

        List<PooledSection> _possiblePooledSections = new List<PooledSection>();
        // contains only the barriers that an endless section could spawn
        List<PooledBarrier> _possiblePooledBarriers = new List<PooledBarrier>();
        List<PooledEnvironment> _possiblePooledEnvironments = new List<PooledEnvironment>();
        List<PooledWall> _possiblePooledWalls = new List<PooledWall>();
        List<PooledMonster> _possiblePooledMonsters = new List<PooledMonster>();

        private static EndlessPool _instance;
        bool madeWorld;
        bool gotCurrentMap;
        IEnumerator _makeWorld;
        EndlessPoolPrefabsScriptableObject currentPrefab;
        GameDifficulty difficulty = GameDifficulty.Easy;
        #endregion

        #region Properties
        List<SectionPrefab> SectionPrefabs 
        {
            get
            {
                return currentPrefab.sectionPrefabs;
            }
        }

        List<BarrierPrefab> BarrierPrefabs 
        {
            get
            {
                return currentPrefab.barrierPrefabs;
            }
        }

        List<EnvironmentPrefab> EnvironmentPrefabs 
        {
            get
            {
                return currentPrefab.environmentPrefabs;
            }
        }

        List<WallPrefab> WallPrefabs 
        {
            get
            {
                return currentPrefab.wallPrefabs;
            }
        }

        List<MonsterPrefab> MonsterPrefabs 
        {
            get
            {
                return currentPrefab.monsterPrefabs;
            }
        }
        #endregion

        #region Event Functions
        void Awake()
        {
            currentPrefab = prefabs[0];
            if(_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
            Message.AddListener<MakeWorld>(OnMakeWorld);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<CurrentMapResponse>(OnCurrentMapResponse);
        }

        void OnDestroy()
        {
            Message.RemoveListener<MakeWorld>(OnMakeWorld);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<CurrentMapResponse>(OnCurrentMapResponse);
        }
        #endregion

        #region Listener Functions
        void OnMakeWorld(MakeWorld makeWorld)
        {
            if(!madeWorld)
            {
                _makeWorld = _MakeWorld();
                StartCoroutine(_makeWorld);
            }
            else
            {
                Message.Send(new MadeWorld());
            }
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.Exit)
            {
                madeWorld = false;
                gotCurrentMap = false;
                pooledBarriers.Clear();
                pooledSections.Clear();
                pooledEnvironments.Clear();
                pooledWalls.Clear();
                difficulty = GameDifficulty.Easy;
            }
        }

        void OnCurrentMapResponse(CurrentMapResponse response)
        {
            foreach(EndlessPoolPrefabsScriptableObject prefab in prefabs)
            {
                if(prefab.type == response.type)
                {
                    currentPrefab = prefab;
                    break;
                }
            }
            gotCurrentMap = true;
        }

        void OnGameDifficultyResponse(GameDifficultyResponse response)
        {
            difficulty = response.difficulty;
        }

        #endregion

        #region Public Functions
        public GameObject GetRandomBarrier(List<BarrierType> types)
        {
            return GetRandom(pooledBarriers, BarrierPrefabs, types);
        }

        public GameObject GetSpecifiedOrRandomSection(SectionType type, bool isTurn)
        {
            GameObject section = _GetSpecifiedSection(pooledSections, SectionPrefabs, type);
            if(section == null)
            {
                return GetRandomSection(isTurn);
            }
            return section;
        }

        public GameObject GetRandomSection(bool isTurn = false)
        {
            return GetRandom(pooledSections, SectionPrefabs, isTurn);
        }

        public GameObject GetSpecifiedEnvironment(EnvironmentType type)
        {
            return GetRandom(pooledEnvironments, EnvironmentPrefabs, type);
        }

        public GameObject GetSpecifiedWall(WallType type)
        {
            return GetRandom(pooledWalls, WallPrefabs, type);
        }

        public GameObject GetSpecifiedMonster(MonsterType type)
        {
            return GetRandom(pooledMonsters, MonsterPrefabs, type);
        }
        #endregion

        #region Private Functions
        void MakeWorld()
        {
            madeWorld = true;
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

            foreach(EnvironmentPrefab item in EnvironmentPrefabs)
            {
                for(int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledEnvironments.Add(new PooledEnvironment(obj, item.Type));
                }
            }

            foreach(WallPrefab item in WallPrefabs)
            {
                for(int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledWalls.Add(new PooledWall(obj, item.Type));
                }
            }
            Message.Send(new MadeWorld());
        }
        GameObject GetRandom(List<PooledSection> pooledSections, List<SectionPrefab> _prefabs, bool isTurn)
        {
            _possiblePooledSections.Clear();
            foreach(PooledSection item in pooledSections)
            {
                if(item.isTurn == isTurn)
                {
                    _possiblePooledSections.Add(item);
                }
            }

            EndlessUtils.Shuffle(_possiblePooledSections);
            for(int i = 0; i < _possiblePooledSections.Count; i++)
            {
                if(!_possiblePooledSections[i].gameObject.activeInHierarchy)
                {
                    return _possiblePooledSections[i].gameObject;
                }
            }
            
            foreach(SectionPrefab item in _prefabs)
            {
                if(item.expandable && item.IsTurn == isTurn)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledSections.Add(new PooledSection(obj));
                    Debug.Log("creating section");
                    return obj;
                }
            }
            return null;
        }

        GameObject _GetSpecifiedSection(List<PooledSection> pooledSections, List<SectionPrefab> _prefabs, SectionType type)
        {
            _possiblePooledSections.Clear();
            foreach(PooledSection item in pooledSections)
            {
                if(item.type == type)
                {
                    _possiblePooledSections.Add(item);
                }
            }

            EndlessUtils.Shuffle(_possiblePooledSections);
            for(int i = 0; i < _possiblePooledSections.Count; i++)
            {
                if(!_possiblePooledSections[i].gameObject.activeInHierarchy)
                {
                    return _possiblePooledSections[i].gameObject;
                }
            }
            
            return null;
        }

        GameObject GetRandom(List<PooledBarrier> pooledBarriers, List<BarrierPrefab> possibleBarriers, List<BarrierType> types)
        {
            _possiblePooledBarriers.Clear();
            foreach(PooledBarrier item in pooledBarriers)
            {
                if(types.Contains(item.type))
                {
                    _possiblePooledBarriers.Add(item);
                }
            }

            EndlessUtils.Shuffle(_possiblePooledBarriers);
            for(int i = 0; i < _possiblePooledBarriers.Count; i++)
            {
                if(!_possiblePooledBarriers[i].gameObject.activeInHierarchy)
                {
                    return _possiblePooledBarriers[i].gameObject;
                }
            }

            foreach(BarrierPrefab item in possibleBarriers)
            {
                if(item.expandable && types.Contains(item.Type))
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledBarriers.Add(new PooledBarrier(obj, item.Type));
                    Debug.Log("creating barrier");
                    return obj;
                }
            }
            return null;
        }

        GameObject GetRandom(List<PooledEnvironment> pooledEnvironments, List<EnvironmentPrefab> _prefabs, EnvironmentType type)
        {
            _possiblePooledEnvironments.Clear();
            foreach(PooledEnvironment item in pooledEnvironments)
            {
                if(item.type == type)
                {
                    _possiblePooledEnvironments.Add(item);
                }
            }

            EndlessUtils.Shuffle(_possiblePooledEnvironments);
            for(int i = 0; i < _possiblePooledEnvironments.Count; i++)
            {
                if(!_possiblePooledEnvironments[i].gameObject.activeInHierarchy)
                {
                    return _possiblePooledEnvironments[i].gameObject;
                }
            }
            
            foreach(EnvironmentPrefab item in _prefabs)
            {
                if(item.expandable && item.Type == type)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledEnvironments.Add(new PooledEnvironment(obj, type));
                    Debug.Log("creating environment");
                    return obj;
                }
            }
            return null;
        }

        GameObject GetRandom(List<PooledWall> pooledWalls, List<WallPrefab> _prefabs, WallType type)
        {
            _possiblePooledWalls.Clear();
            foreach(PooledWall item in pooledWalls)
            {
                if(item.type == type)
                {
                    _possiblePooledWalls.Add(item);
                }
            }

            EndlessUtils.Shuffle(_possiblePooledWalls);
            for(int i = 0; i < _possiblePooledWalls.Count; i++)
            {
                if(!_possiblePooledWalls[i].gameObject.activeInHierarchy)
                {
                    return _possiblePooledWalls[i].gameObject;
                }
            }
            
            foreach(WallPrefab item in _prefabs)
            {
                if(item.expandable && item.Type == type)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledWalls.Add(new PooledWall(obj, type));
                    Debug.Log("creating environment");
                    return obj;
                }
            }
            return null;
        }

        GameObject GetRandom(List<PooledMonster> pooledMonsters, List<MonsterPrefab> _prefabs, MonsterType type)
        {
            _possiblePooledMonsters.Clear();
            foreach(PooledMonster item in pooledMonsters)
            {
                if(item.type == type)
                {
                    _possiblePooledMonsters.Add(item);
                }
            }

            EndlessUtils.Shuffle(_possiblePooledMonsters);
            for(int i = 0; i < _possiblePooledMonsters.Count; i++)
            {
                if(!_possiblePooledMonsters[i].Monster.IsActive)
                {
                    return _possiblePooledMonsters[i].gameObject;
                }
            }
            
            foreach(MonsterPrefab item in _prefabs)
            {
                if(item.expandable && item.Type == type)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledMonsters.Add(new PooledMonster(obj, type));
                    Debug.Log("creating monster");
                    return obj;
                }
            }
            return null;
        }
        #endregion

        #region Coroutines
        IEnumerator _MakeWorld()
        {
            Message.Send(new CurrentMapRequest());
            while(!gotCurrentMap)
            {
                yield return null;
            }
            MakeWorld();
            yield break;
        }
        #endregion
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
    public class EnvironmentPrefab: ItemPrefab
    {
        public EnvironmentType Type
        {
            get
            {
                return prefab.GetComponent<EndlessEnvironment>().type;
            }
        }
    }

    [System.Serializable]
    public class WallPrefab: ItemPrefab
    {
        public WallType Type
        {
            get
            {
                return prefab.GetComponent<EndlessWall>().type;
            }
        }
    }

    [System.Serializable]
    public class MonsterPrefab: ItemPrefab
    {
        public MonsterType Type
        {
            get
            {
                return prefab.GetComponent<EndlessMonster>().type;
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

    [System.Serializable]
    public class PooledEnvironment: PooledItem
    {
        public EnvironmentType type;

        public PooledEnvironment(GameObject gameObject, EnvironmentType type)
        {
            this.gameObject = gameObject;
            this.type = type;
        }
    }

    [System.Serializable]
    public class PooledWall: PooledItem
    {
        public WallType type;

        public PooledWall(GameObject gameObject, WallType type)
        {
            this.gameObject = gameObject;
            this.type = type;
        }
    }

    [System.Serializable]
    public class PooledMonster: PooledItem
    {
        public MonsterType type;

        public EndlessMonster Monster
        {
            get
            {
                return this.gameObject.GetComponent<EndlessMonster>();
            }
        }

        public PooledMonster(GameObject gameObject, MonsterType type)
        {
            this.gameObject = gameObject;
            this.type = type;
        }
    }
}
