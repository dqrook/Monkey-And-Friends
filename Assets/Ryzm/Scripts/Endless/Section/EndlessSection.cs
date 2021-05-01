using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessSection : MonoBehaviour
    {
        public DeactivateSection deactivate;
        public Transform[] obstacleSpawnLocations;
        [Header("Lane Positions")]
        public Transform position0;
        public Transform position1;
        public Transform position2;
        Transform runner;

        void FixedUpdate()
        {
            if(runner == null)
            {
                runner = RunnerController.player.transform;
            }
            if(RunnerController.isDead)
            {
                return;
            }
            this.transform.position += runner.forward * -0.1f;

            if(GameManager.Instance.CurrentPlatform == null) return;

            if(GameManager.Instance.CurrentPlatform.tag == "stairsUp") 
            {
                this.transform.Translate(0, -0.06f, 0);
            }

            if(GameManager.Instance.CurrentPlatform.tag == "stairsDown") 
            {
                this.transform.Translate(0, 0.06f, 0);
            }
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
}
