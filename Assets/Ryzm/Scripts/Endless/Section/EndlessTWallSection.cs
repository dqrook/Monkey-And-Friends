using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessTWallSection : EndlessTSection
    {
        #region Public Variables
        [Header("Wall")]
        public List<WallSpawn> wallSpawns = new List<WallSpawn>();
        #endregion

        #region Public Functions
        public override void Initialize(int rowId)
        {
            base.Initialize(rowId);
            foreach(WallSpawn spawn in wallSpawns)
            {
                GameObject wallGO = EndlessPool.Instance.GetSpecifiedWall(spawn.type);
                if(wallGO != null)
                {
                    EndlessWall wall = wallGO.GetComponent<EndlessWall>();
                    if(wall != null)
                    {
                        wall.Initialize(rowId, spawn.disableOnStart);
                        wallGO.transform.position = spawn.spawn.position;
                        wallGO.transform.rotation = spawn.spawn.rotation;
                        spawn.wall = wall;
                    }
                }
            }
        }

        public override void Shift(Direction direction, EndlessController controller, ref bool turned)
        {
            _Shift(direction, controller, turned);
            if(!turned)
            {
                foreach(WallSpawn spawn in wallSpawns)
                {
                    if(spawn.turnDirection != direction)
                    {
                        spawn.wall.gameObject.SetActive(false);
                    }
                }
            }
            turned = true;
        }
        #endregion
    }

    [System.Serializable]
    public class WallSpawn
    {
        public Direction turnDirection;
        public WallType type;
        public Transform spawn;
        public bool disableOnStart;
        [HideInInspector]
        public EndlessWall wall;
    }
}
