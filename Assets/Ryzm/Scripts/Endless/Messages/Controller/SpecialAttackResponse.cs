using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class SpecialAttackResponse : Message
    {
        public ActionState state;

        public SpecialAttackResponse(ActionState state)
        {
            this.state = state;
        }
    }
}
