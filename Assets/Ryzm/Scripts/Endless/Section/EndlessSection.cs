using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessSection : EndlessScroller
    {
        public DeactivateSection deactivate;
        public List<SpawnLocation> barrierSpawnLocations = new List<SpawnLocation>();
        /// <summary>
        /// Position where to spawn next section
        /// </summary>
        public Transform nextSectionSpawn; 
        [Range(0, 1)]
        public float barrierLikelihood = 0.5f;
        
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
                    _possibleBarrierTypes.Add(location.type);
                }
                return _possibleBarrierTypes;
            }
        }

        protected override void OnDisable()
        {
            isLastSection = false;
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

        public Transform GetBarrierSpawnLocation(BarrierType type)
        {
            foreach(SpawnLocation location in barrierSpawnLocations)
            {
                if(location.type == type)
                {
                    if(location.spawnTransforms.Length > 0)
                    {
                        EndlessUtils.Shuffle(location.spawnTransforms);
                        return location.spawnTransforms[0].location;
                    }
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
            return new SpawnLocation();
        }

        public Transform GetSpawnTransformForBarrierPosition(BarrierType type, int position)
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
                    return spawnTransform.location;
                }
            }

            return null;
        }

        public virtual void Shift(Direction direction, RunnerController controller)
        {
            Transform trans = controller.gameObject.transform;
            int currentPosition = controller.CurrentPosition;
            if(direction == Direction.Left && currentPosition > 0)
            {
                Transform pos = GetPosition(currentPosition - 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos);
                    controller.CurrentPosition--;
                }
            }
            else if(direction == Direction.Right && currentPosition < 2)
            {
                Transform pos = GetPosition(currentPosition + 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos);
                    controller.CurrentPosition++;
                }
            }
        }
    }
    [System.Serializable]
    public struct SpawnLocation
    {
        public BarrierType type;
        public SpawnTransform[] spawnTransforms;
    }

    [System.Serializable]
    public struct SpawnTransform
    {
        public Transform location;
        public int position;
    }
}
