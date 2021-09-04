using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ShiftAttackStateResponse : Message
    {
        public AttackState shiftAttackState;

        public ShiftAttackStateResponse(AttackState shiftAttackState)
        {
            this.shiftAttackState = shiftAttackState;
        }
    }
}
