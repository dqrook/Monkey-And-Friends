using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessSection : MonoBehaviour
    {
        public Transform position0;
        public Transform position1;
        public Transform position2;

        void FixedUpdate()
        {
            this.transform.position += RunnerController.player.transform.forward * -0.1f;

            if(RunnerController.CurrentPlatform == null) return;

            if(RunnerController.CurrentPlatform.tag == "stairsUp") 
            {
                this.transform.Translate(0, -0.06f, 0);
            }

            if(RunnerController.CurrentPlatform.tag == "stairsDown") 
            {
                this.transform.Translate(0, 0.06f, 0);
            }
        }

        protected virtual Transform GetPosition(int position)
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

        public virtual void Shift(Direction direction, RunnerController controller)
        {
            Transform trans = controller.gameObject.transform;
            int currentPosition = controller.currentPosition;
            if(direction == Direction.Left && currentPosition > 0)
            {
                Transform pos = GetPosition(currentPosition - 1);
                if(pos != null)
                {
                    float _shiftDistance = pos.InverseTransformPoint(trans.position).x;
                    trans.Translate(_shiftDistance, 0, 0);
                    controller.currentPosition--;
                }
            }
            else if(direction == Direction.Right && currentPosition < 2)
            {
                Transform pos = GetPosition(currentPosition + 1);
                if(pos != null)
                {
                    float _shiftDistance = pos.InverseTransformPoint(trans.position).x;
                    trans.Translate(_shiftDistance, 0, 0);
                    controller.currentPosition++;
                }
            }
        }
    }
}
