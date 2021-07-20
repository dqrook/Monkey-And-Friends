using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessSection : EndlessScroller
    {
        #region Public Variables
        public SectionType type;
        public bool isTurn;
        public DeactivateSection deactivate;
        public List<SpawnLocation> barrierSpawnLocations = new List<SpawnLocation>();
        /// <summary>
        /// Position where to spawn next section
        /// </summary>
        public Transform nextSectionSpawn; 
        [Range(0, 1)]
        public float barrierLikelihood = 0.5f;
        public List<GameObject> environments = new List<GameObject>();
        
        [Header("Lane Positions")]
        public Transform position0;
        public Transform position1;
        public Transform position2;
        
        [HideInInspector]
        public int rowId;
        #endregion

        #region Private Variables
        List<BarrierType> _possibleBarrierTypes = new List<BarrierType>();
        #endregion

        #region Properties
        public List<BarrierType> PossibleBarrierTypes
        {
            get
            {
                _possibleBarrierTypes.Clear();
                foreach(SpawnLocation location in barrierSpawnLocations)
                {
                    if(location.weight > 0) 
                    {
                        _possibleBarrierTypes.Add(location.type);
                    }
                }
                return _possibleBarrierTypes;
            }
        }
        #endregion

        #region Event Functions
        protected override void OnEnable()
        {
            base.OnEnable();
            // Debug.Log(gameObject.name + " activated");
            if(environments.Count > 1)
            {
                EndlessUtils.Shuffle(environments);
                int i = 0;
                foreach(GameObject go in environments)
                {
                    go.SetActive(i == 0);
                    i++;
                }
            }
        }

        protected override void OnDisable() {}
        #endregion

        #region Listener Functions
        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Restart)
            {
                gameObject.SetActive(false);
            }
            else if(gameStatusResponse.status == GameStatus.Ended)
            {
                CancelDeactivation();
            }
        }
        #endregion

        #region Public Functions
        public virtual void EnterSection()
        {
            Message.Send(new CurrentSectionChange(gameObject, rowId));
        }

        public virtual void ExitSection()
        {
            // rowId = 0;
            deactivate.Deactivate();
        }

        public void CancelDeactivation()
        {
            deactivate.CancelDeactivation();
        }

        public virtual Transform NextSectionSpawn()
        {
            return nextSectionSpawn;
        }

        public virtual Transform GetPosition(int position)
        {
            switch(position)
            {
                case 0:
                    return position0;
                case 1:
                    return position1;
                case 2:
                    return position2;
                default:
                    return null;
            }
        }

        public Transform GetSpawnTransformForBarrier(BarrierType type, int position)
        {
            SpawnLocation location = GetSpawnLocationForBarrier(type);
            if(location == null)
            {
                return null;
            }
            
            foreach(SpawnTransform spawnTransform in location.spawnTransforms)
            {
                if(spawnTransform.position == position)
                {
                    return spawnTransform.Location;
                }
            }

            return null;
        }

        public SpawnLocation GetSpawnLocationForBarrier(BarrierType type)
        {
            foreach(SpawnLocation location in barrierSpawnLocations)
            {
                if(location.type == type)
                {
                    return location;
                }
            }
            return null;
        }

        public virtual void Shift(Direction direction, EndlessController controller)
        {
            Transform trans = controller.gameObject.transform;
            int currentPosition = controller.CurrentPosition;
            if(direction == Direction.Left && currentPosition > 0)
            {
                Transform pos = GetPosition(currentPosition - 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos, ShiftDistanceType.x);
                    controller.CurrentPosition--;
                }
            }
            else if(direction == Direction.Right && currentPosition < 2)
            {
                Transform pos = GetPosition(currentPosition + 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos, ShiftDistanceType.x);
                    controller.CurrentPosition++;
                }
            }
        }
        #endregion
    }
    [System.Serializable]
    public class SpawnLocation
    {
        public BarrierType type;
        public SpawnTransform[] spawnTransforms;
        [Range(0, 10)]
        public int weight = 1;

        public SpawnTransform RandomSpawnTransform()
        {
            EndlessUtils.Shuffle(spawnTransforms);
            return spawnTransforms[0];
        }
    }

    [System.Serializable]
    public struct SpawnTransform
    {
        public int position;
        public Transform[] locations;

        public Transform Location 
        {
            get
            {
                if(locations.Length == 0)
                {
                    return null;
                }
                int idx = Random.Range(0, locations.Length);
                return locations[idx];
            }
        }
    }

    public enum SectionType
    {
        BasicRandom,
        BasicCoin,
        BasicDiveDragon,
        BasicPathDragon,
        BasicInstantFire,
        LongRandom,
        LongCoin,
        LongDiveDragon,
        LongPathDragon,
        TSection1,
        LeftTurn1,
        BasicLauncher,
        BasicDragon,
        BasicDiveDragonCoinRow,
        TSectionBeach1,
        TSectionGrass1,
        LeftTurnGrass1,
        RightTurnGrass1,
        LeftTurnBeach1,
        FloatingTree,
        FloatingRabby,
        FloatingRabbyCoinRow,
        FloatingSpikes,
        FloatingDiveDragon,
        FloatingSideDragon
    }
}
