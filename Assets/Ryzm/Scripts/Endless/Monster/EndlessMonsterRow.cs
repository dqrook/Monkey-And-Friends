using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public abstract class EndlessMonsterRow : EndlessBarrier
    {
        #region Public Variables
        public List<EndlessMonster> monsters = new List<EndlessMonster>();
        #endregion

        #region Listener Functions
        protected override void OnSectionDeactivated(SectionDeactivated sectionDeactivated)
        {
            if(sectionDeactivated.section == parentSection)
            {
                foreach(EndlessMonster monster in monsters)
                {
                    monster.Reset();
                }
            }
            base.OnSectionDeactivated(sectionDeactivated);
        }

        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            if(gameStatusResponse.status == GameStatus.Restart)
            {
                foreach(EndlessMonster rabby in monsters)
                {
                    rabby.Reset();
                }
            }
            base.OnGameStatusResponse(gameStatusResponse);
        }
        #endregion
    }
}
