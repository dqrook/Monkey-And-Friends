using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class JumpStatusResponse : Message
    {
        public bool inJump;

        public JumpStatusResponse(bool inJump) 
        {
            this.inJump = inJump;
        }
    }
}
