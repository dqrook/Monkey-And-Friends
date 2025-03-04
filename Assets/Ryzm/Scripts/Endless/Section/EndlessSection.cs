﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessSection : EndlessBasePath
    {
        #region Public Variables
        public SectionType type;
        public bool isTurn;
        public DeactivateSection deactivate;
        public List<BarrierSpawnLocation> barrierSpawnLocations = new List<BarrierSpawnLocation>();
        /// <summary>
        /// Position where to spawn next section
        /// </summary>
        public Transform nextSectionSpawn; 
        [Range(0, 1)]
        public float barrierLikelihood = 0.5f;
        public BaseSectionPrefab baseSection;
        public EndlessSectionCombinations combinations;

        [Header("Environment")]
        public List<GameObject> environments = new List<GameObject>();
        [Range(0, 1)]
        public float environmentLikelihood = 1f;
        
        [Header("Lane Positions")]
        public Transform position0;
        public Transform position1;
        public Transform position2;
        
        [HideInInspector]
        public int rowId;
        #endregion

        #region Protected Variables
        protected ShiftDistanceType shiftDistanceType;
        #endregion

        #region Private Variables
        List<BarrierType> _possibleBarrierTypes = new List<BarrierType>();
        float runnerDistance;
        #endregion

        #region Properties
        public List<BarrierType> PossibleBarrierTypes
        {
            get
            {
                _possibleBarrierTypes.Clear();
                foreach(BarrierSpawnLocation location in barrierSpawnLocations)
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
        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.Send(new RunnerDistanceRequest());
            if(baseSection != null)
            {
                baseSection.Activate(runnerDistance);
            }
            if(environments.Count > 0 && CanPlaceEnvironment(environmentLikelihood))
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
            if(baseSection != null)
            {
                baseSection.Deactivate();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
        }
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

        void OnRunnerDistanceResponse(RunnerDistanceResponse response)
        {
            runnerDistance = response.distance;
        }
        #endregion

        #region Public Functions
        public virtual void Initialize(int rowId, ShiftDistanceType shiftDistanceType = ShiftDistanceType.x)
        {
            this.rowId = rowId;
            this.shiftDistanceType = shiftDistanceType;
        }

        public override void Enter()
        {
            Message.Send(new CurrentSectionChange(gameObject, rowId));
        }

        public override void Exit()
        {
            // rowId = 0;
            deactivate.Deactivate();
        }

        public override void CancelDeactivation()
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
            BarrierSpawnLocation location = GetSpawnLocationForBarrier(type);
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

        public BarrierSpawnLocation GetSpawnLocationForBarrier(BarrierType type)
        {
            foreach(BarrierSpawnLocation location in barrierSpawnLocations)
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
                    controller.ShiftToPosition(pos, shiftDistanceType);
                    controller.CurrentPosition--;
                }
            }
            else if(direction == Direction.Right && currentPosition < 2)
            {
                Transform pos = GetPosition(currentPosition + 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos, shiftDistanceType);
                    controller.CurrentPosition++;
                }
            }
        }
        #endregion

        #region Private Functions
        bool CanPlaceEnvironment(float likelihood)
        {
            return Random.Range(0, 1f) <= likelihood;
        }
        #endregion
    }
    [System.Serializable]
    public class BarrierSpawnLocation
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

    [System.Serializable]
    public class SectionEnvironment
    {
        public GameObject mainEnvironment;
        public GameObject[] subEnvironments;
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
        FloatingSideDragon,
        RockRightTurn1,
        RockLeftTurn1,
        RockTSection1,
        RockTree,
        RockRabby,
        RockSpikes,
        RockTWallSection1,
        WaterKrake1,
        WaterBridgeKrab,
        WaterKrake2,
        WaterStone,
        WaterBeachPillar,
        FloatingMonster1,
        FloatingMonster2,
        FloatingMonster3,
        FloatingMonster4,
        FloatingMonster5,
        BariaMonster1
    }
}
