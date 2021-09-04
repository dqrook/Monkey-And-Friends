using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class HeadbuttAttackStateResponse : Message
    {
        public AttackState headbuttAttackState;

        public HeadbuttAttackStateResponse(AttackState headbuttAttackState)
        {
            this.headbuttAttackState = headbuttAttackState;
        }
    }
}
