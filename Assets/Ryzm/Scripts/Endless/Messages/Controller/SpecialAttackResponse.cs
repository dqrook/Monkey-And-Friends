using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class SpecialAttackResponse : Message
    {
        public AttackState state;

        public SpecialAttackResponse(AttackState state)
        {
            this.state = state;
        }
    }
}
