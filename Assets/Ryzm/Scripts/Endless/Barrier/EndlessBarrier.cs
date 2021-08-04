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
            ResetSpawns();
        }

        protected virtual void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<EndlessController>())
            {
                Message.Send(new RunnerDie());
            }
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
                        break;
                    }
                }
            }
            else
            {
                ResetSpawns();
            }
        }
        #endregion

        #region Protected Functions
        protected bool CanPlaceRow(float rowLikelihood)
        {
            return Random.Range(0, 1f) <= rowLikelihood;
        }

        protected virtual void ResetSpawns()
        {
            foreach(RowSpawn spawn in spawns)
            {
                spawn.Disable();
            }
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
        SideDragon,
        RockTree1,
        RockTree2,
        RockTree3,
        RockTree4,
        RockRabby1,
        RockSpikes,
        Krake,
        StoneMound,
        StonePillar1,
        StonePillar2,
        StonePillar3,
        StonePillar4,
        KrabRow
    }

    [System.Serializable]
    public struct RowSpawn
    {
        public int position;
        public List<Transform> rows;

        public GameObject EnableRandomRow()
        {
            if(rows.Count > 0)
            {
                EndlessUtils.Shuffle(rows);
                rows[0].gameObject.SetActive(true);
                return rows[0].gameObject;
            }
            return null;
        }

        public void Disable()
        {
            foreach(Transform row in rows)
            {
                if(row != null)
                {
                    row.gameObject.SetActive(false);
                }
            }
        }
    }
}
