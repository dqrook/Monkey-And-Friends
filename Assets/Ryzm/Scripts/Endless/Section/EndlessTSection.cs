using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessTSection : EndlessTurnSection
    {
        #region Public Variables
        [Header("Right Lane Positions")]
        public Transform position3;
        public Transform position4;
        public Transform position5;

        [Header("Right Spawn")]
        public Transform rightNextSectionSpawn;
        #endregion

        #region Public Functions
        public override Transform NextSectionSpawn()
        {
            if(userTurnedDirection == Direction.Left)
            {
                return nextSectionSpawn;
            }
            if(userTurnedDirection == Direction.Right)
            {
                return rightNextSectionSpawn;
            }
            return null;
        }

        public override Transform GetPosition(int position)
        {
            switch(position)
            {
                case 0:
                    return userTurnedDirection == Direction.Left ? position0 : position3;
                case 1:
                    return userTurnedDirection == Direction.Left ? position1 : position4;
                case 2:
                    return userTurnedDirection == Direction.Left ? position2 : position5;
                default:
                    return null;
            }
        }

        public override void Enter()
        {
            base.Enter();
        }
        
        public override void Exit() 
        {
            base.Exit();
        }

        public override void Shift(Direction direction, EndlessController controller, ref bool turned)
        {
            _Shift(direction, controller, turned);
            turned = true;
        }
        #endregion
    }
}
