using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessTurnSection : EndlessSection
    {
        #region Public Variables
        public Direction turnDirection;
        
        [Header("Row Spawn")]
        public RowSpawn spawn;
        [Range(0f, 1f)]
        public float rowLikelihood = 0.5f;
        #endregion

        #region Protected Variables
        // direction the user turned in
        protected Direction userTurnedDirection;
        #endregion

        #region Event Functions
        protected override void OnEnable()
        {
            base.OnEnable();
            spawn.Disable();
            if(CanPlaceRow(rowLikelihood))
            {
                spawn.EnableRandomRow();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            spawn.Disable();
        }
        #endregion

        #region Public Functions
        public virtual void Shift(Direction direction, EndlessController controller, ref bool turned)
        {
            if(direction != turnDirection && !turned)
            {
                return;
            }
            _Shift(direction, controller, turned);
            turned = true;
        }

        public override void Shift(Direction direction, EndlessController controller)
        {
            Transform trans = controller.gameObject.transform;
            int currentPosition = controller.CurrentPosition;
            ShiftDistanceType _shiftDistancetype = shiftDistanceType == ShiftDistanceType.x ? ShiftDistanceType.z : ShiftDistanceType.x;
            if(direction == Direction.Left && currentPosition > 0)
            {
                Transform pos = GetPosition(currentPosition - 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos, _shiftDistancetype);
                    controller.CurrentPosition--;
                }
            }
            else if(direction == Direction.Right && currentPosition < 2)
            {
                Transform pos = GetPosition(currentPosition + 1);
                if(pos != null)
                {
                    controller.ShiftToPosition(pos, _shiftDistancetype);
                    controller.CurrentPosition++;
                }
            }
        }
        #endregion

        #region Protected Functions
        protected void _Shift(Direction direction, EndlessController controller, bool turned)
        {
            if(!turned)
            {
                Transform trans = controller.gameObject.transform;
                userTurnedDirection = direction;
                if(direction == Direction.Left)
                {
                    trans.Rotate(Vector3.up * -90);
                    Message.Send(new RowComplete(rowId));

                }
                else if(direction == Direction.Right)
                {
                    trans.Rotate(Vector3.up * 90);
                    Message.Send(new RowComplete(rowId));
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
        #endregion

        #region Private Functions
        bool CanPlaceRow(float rowLikelihood)
        {
            return Random.Range(0, 1f) <= rowLikelihood;
        }
        #endregion
    }
}
