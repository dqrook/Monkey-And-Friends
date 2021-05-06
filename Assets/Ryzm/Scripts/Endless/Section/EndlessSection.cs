using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessSection : EndlessScroller
    {
        public DeactivateSection deactivate;
        public List<SpawnLocation> barrierSpawnLocations = new List<SpawnLocation>();
        public bool isLastSection; // true
        /// <summary>
        /// Position where to spawn next section
        /// </summary>
        public Transform nextSectionSpawn; 
        
        [Header("Lane Positions")]
        public Transform position0;
        public Transform position1;
        public Transform position2;
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
                    if(location.locations.Length > 0)
                    {
                        EndlessUtils.Shuffle(location.locations);
                        return location.locations[0];
                    }
                    break;
                }
            }
            return null;
        }

        public virtual void Shift(Direction direction, RunnerController controller)
        {
            Transform trans = controller.gameObject.transform;
            int currentPosition = controller.currentPosition;
            if(direction == Direction.Left && currentPosition > 0)
            {
                Transform pos = GetPosition(currentPosition - 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos);
                    controller.currentPosition--;
                }
            }
            else if(direction == Direction.Right && currentPosition < 2)
            {
                Transform pos = GetPosition(currentPosition + 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos);
                    controller.currentPosition++;
                }
            }
        }
    }
    [System.Serializable]
    public struct SpawnLocation
    {
        public Transform[] locations;
        public BarrierType type;
    }
}
