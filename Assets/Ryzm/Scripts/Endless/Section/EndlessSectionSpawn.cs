using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessSectionSpawn : MonoBehaviour
    {
        public int difficultyPoints;
        public List<EndlessMonsterSpawn> spawns = new List<EndlessMonsterSpawn>();

        void OnDisable()
        {
            Deactivate();
        }

        public void Activate()
        {
            foreach(EndlessMonsterSpawn spawn in spawns)
            {
                GameObject monsterObj = EndlessPool.Instance.GetSpecifiedMonster(spawn.monsterType);
                if(monsterObj != null)
                {
                    monsterObj.transform.position = spawn.spawn.position;
                    monsterObj.transform.rotation = spawn.spawn.rotation;
                    EndlessMonster monster = monsterObj.GetComponent<EndlessMonster>();
                    spawn.monster = monster;
                    monster.IsActive = true;
                }
            }
        }

        public void Deactivate()
        {
            foreach(EndlessMonsterSpawn spawn in spawns)
            {
                if(spawn.monster != null)
                {
                    spawn.monster.IsActive = false;
                    spawn.monster = null;
                }
            }
        }
    }

    [System.Serializable]
    public class EndlessMonsterSpawn
    {
        public Transform spawn;
        public MonsterType monsterType;
        [HideInInspector]
        public EndlessMonster monster;
    }
}
