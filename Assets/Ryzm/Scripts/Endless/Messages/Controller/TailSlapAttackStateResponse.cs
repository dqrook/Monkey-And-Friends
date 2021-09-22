using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class TailSlapAttackStateResponse : Message
    {
        public ActionState tailSlapAttackState;

        public TailSlapAttackStateResponse(ActionState tailSlapAttackState)
        {
            this.tailSlapAttackState = tailSlapAttackState;
        }
    }
}

