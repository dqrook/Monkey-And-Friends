using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class HeadbuttAttackStateResponse : Message
    {
        public ActionState headbuttAttackState;

        public HeadbuttAttackStateResponse(ActionState headbuttAttackState)
        {
            this.headbuttAttackState = headbuttAttackState;
        }
    }
}
