using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class TailSlapAttackStateResponse : Message
    {
        public AttackState tailSlapAttackState;

        public TailSlapAttackStateResponse(AttackState tailSlapAttackState)
        {
            this.tailSlapAttackState = tailSlapAttackState;
        }
    }
}

