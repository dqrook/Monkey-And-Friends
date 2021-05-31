using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessSection : EndlessScroller
    {
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
        public bool isLastSection;

        
        List<BarrierType> _possibleBarrierTypes = new List<BarrierType>();

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

        protected override void OnEnable()
        {
            base.OnEnable();
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

        protected override void OnDisable()
        {
            isLastSection = false;
        }

        public virtual void EnterSection()
        {
            Message.Send(new CurrentSectionChange(gameObject));
        }

        public virtual void ExitSection()
        {
            if(isLastSection)
            {
                Message.Send(new CreateSectionRow());
            }
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

        public Transform GetBarrierSpawnTransform(BarrierType type)
        {
            foreach(SpawnLocation location in barrierSpawnLocations)
            {
                if(location.type == type)
                {
                    if(location.spawnTransforms.Length > 0)
                    {
                        EndlessUtils.Shuffle(location.spawnTransforms);
                        return location.spawnTransforms[0].Location;
                    }
                }
            }
            return null;
        }

        protected SpawnLocation GetSpawnLocationForBarrier(BarrierType type)
        {
            foreach(SpawnLocation location in barrierSpawnLocations)
            {
                if(location.type == type)
                {
                    return location;
                }
            }
            return new SpawnLocation();
        }

        public Transform GetSpawnTransformForBarrierByPosition(BarrierType type, int position)
        {
            SpawnLocation loc = GetSpawnLocationForBarrier(type);
            if(loc.spawnTransforms.Length == 0)
            {
                return null;
            }
            
            foreach(SpawnTransform spawnTransform in loc.spawnTransforms)
            {
                if(spawnTransform.position == position)
                {
                    return spawnTransform.Location;
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
    }
    [System.Serializable]
    public class SpawnLocation
    {
        public BarrierType type;
        public SpawnTransform[] spawnTransforms;
        [Range(0, 10)]
        public int weight = 1;
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
                return locations[Random.Range(0, locations.Length)];
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
        TSectionGrass1
    }
}
