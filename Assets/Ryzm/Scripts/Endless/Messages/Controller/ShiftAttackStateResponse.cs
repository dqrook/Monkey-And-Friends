using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ShiftAttackStateResponse : Message
    {
        public ActionState shiftAttackState;

        public ShiftAttackStateResponse(ActionState shiftAttackState)
        {
            this.shiftAttackState = shiftAttackState;
        }
    }
}
