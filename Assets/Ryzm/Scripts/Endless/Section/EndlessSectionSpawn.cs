using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessSectionSpawn : MonoBehaviour
    {
        #region Public Variables
        public int difficultyPoints;
        public MonsterType monsterType;
        public SubSectionPosition spawnPosition;
        public List<AddOnSpawn> addOnSpawns = new List<AddOnSpawn>();
        #endregion

        #region Public Functions
        public AddOnSpawn GetAddOnSpawn()
        {
            int numAddOnSpawns = addOnSpawns.Count;
            if(numAddOnSpawns == 0)
            {
                return new AddOnSpawn();
            }
            return addOnSpawns[Random.Range(0, numAddOnSpawns)];
        }

        public AddOnSpawn GetAddOnSpawn(AddOnSpawnType spawnType)
        {
            foreach(AddOnSpawn spawn in addOnSpawns)
            {
                if(spawn.type == spawnType)
                {
                    return spawn;
                }
            }
            return new AddOnSpawn();
        }
        #endregion
    }
}
