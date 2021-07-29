using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessRabbyRow : EndlessBarrier
    {
        #region Public Variables
        public List<EndlessRabby> rabbies = new List<EndlessRabby>();
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
        }
        #endregion

        #region Public Functions
        public override void Initialize(Transform parentTransform, int position)
        {
            base.Initialize(parentTransform, position);
        }
        #endregion

        #region Listener Functions
        protected override void OnSectionDeactivated(SectionDeactivated sectionDeactivated)
        {
            if(sectionDeactivated.section == parentSection)
            {
                foreach(EndlessRabby rabby in rabbies)
                {
                    rabby.Reset();
                }
            }
            base.OnSectionDeactivated(sectionDeactivated);
        }

        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            if(gameStatusResponse.status == GameStatus.Restart)
            {
                foreach(EndlessRabby rabby in rabbies)
                {
                    rabby.Reset();
                }
            }
            base.OnGameStatusResponse(gameStatusResponse);
        }
        #endregion
    }
}
