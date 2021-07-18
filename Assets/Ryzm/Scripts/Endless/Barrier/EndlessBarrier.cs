using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessBarrier : EndlessScroller
    {
        public BarrierType type;
        // the section that the barrier belongs to
        public EndlessSection parentSection;
        public List<RowSpawn> spawns = new List<RowSpawn>();
        [Range(0f, 1f)]
        public float rowLikelihood = 0.5f;
        protected int runnerPosition;

        #region Properties
        public bool IsCoinRow
        {
            get
            {
                return type == BarrierType.CoinRow1 || type == BarrierType.CoinRow2 || type == BarrierType.CoinRow3;
            }
        }
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<SectionDeactivated>(OnSectionDeactivated);
            Message.AddListener<CurrentPositionResponse>(OnCurrentPositionResponse);
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Message.Send(new CurrentPositionRequest());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            foreach(RowSpawn spawn in spawns)
            {
                spawn.Disable();
            }
        }

        protected void OnCollisionEnter(Collision other)
        {
            Message.Send(new RunnerDie());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<SectionDeactivated>(OnSectionDeactivated);
            Message.RemoveListener<CurrentPositionResponse>(OnCurrentPositionResponse);
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
        }

        protected virtual void OnSectionDeactivated(SectionDeactivated sectionDeactivated)
        {
            if(sectionDeactivated.section == parentSection)
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnCurrentPositionResponse(CurrentPositionResponse response)
        {
            runnerPosition = response.position;
        }
        #endregion

        #region Public Functions
        public virtual void Initialize(Transform parentTransform, int position)
        {
            if(CanPlaceRow(rowLikelihood))
            {
                foreach(RowSpawn spawn in spawns)
                {
                    if(spawn.position == position)
                    {
                        spawn.EnableRandomRow();
                    }
                }
            }
            else
            {
                foreach(RowSpawn spawn in spawns)
                {
                    spawn.Disable();
                }
            }
        }
        #endregion

        #region Private Functions
        bool CanPlaceRow(float rowLikelihood)
        {
            return Random.Range(0, 1f) <= rowLikelihood;
        }
        #endregion
    }

    public enum BarrierType
    {
        Fire,
        DiveDragon,
        InstantFire,
        PathDragon,
        CoinRow1,
        CoinRow2,
        CoinRow3,
        Tree1,
        Tree2,
        Tree3,
        Tree4,
        Rabby1,
        RabbyCoinRow1,
        Spikes,
        SideDragon
    }

    [System.Serializable]
    public struct RowSpawn
    {
        public int position;
        public List<Transform> rows;

        public void EnableRandomRow()
        {
            EndlessUtils.Shuffle(rows);
            rows[0].gameObject.SetActive(true);
        }

        public void Disable()
        {
            foreach(Transform row in rows)
            {
                row.gameObject.SetActive(false);
            }
        }
    }
}
