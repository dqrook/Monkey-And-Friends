using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessTurnSection : EndlessSection
    {
        public Direction turnDirection;

        // direction the user turned in
        protected Direction userTurnedDirection;

        public override void EnterSection()
        {
            base.EnterSection();
        }
        
        public override void ExitSection() 
        {
            base.ExitSection();
        }

        public virtual void Shift(Direction direction, EndlessController controller, bool turned)
        {
            if(direction != turnDirection && !turned)
            {
                return;
            }
            _Shift(direction, controller, turned);
        }

        public override void Shift(Direction direction, EndlessController controller)
        {
            Transform trans = controller.gameObject.transform;
            int currentPosition = controller.CurrentPosition;
            if(direction == Direction.Left && currentPosition > 0)
            {
                Transform pos = GetPosition(currentPosition - 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos, ShiftDistanceType.z);
                    controller.CurrentPosition--;
                }
            }
            else if(direction == Direction.Right && currentPosition < 2)
            {
                Transform pos = GetPosition(currentPosition + 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos, ShiftDistanceType.z);
                    controller.CurrentPosition++;
                }
            }
        }

        protected void _Shift(Direction direction, EndlessController controller, bool turned)
        {
            if(!turned)
            {
                Transform trans = controller.gameObject.transform;
                userTurnedDirection = direction;
                if(direction == Direction.Left)
                {
                    trans.Rotate(Vector3.up * -90);
                    Message.Send(new CreateSectionRow());

                }
                else if(direction == Direction.Right)
                {
                    trans.Rotate(Vector3.up * 90);
                    Message.Send(new CreateSectionRow());
                }
                Transform pos = GetPosition(1);
                float _shiftDistance = pos.InverseTransformPoint(trans.position).z;
                trans.Translate(_shiftDistance, 0, 0);
                controller.CurrentPosition = 1;
            }
            else
            {
                Shift(direction, controller);
            }
        }
    }
}
