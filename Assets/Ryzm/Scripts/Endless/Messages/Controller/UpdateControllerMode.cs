using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class UpdateControllerMode : Message
    {
        public ControllerMode mode;

        public UpdateControllerMode(ControllerMode mode)
        {
            this.mode = mode;
        }
    }
}
