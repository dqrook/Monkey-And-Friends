using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ControllerModeResponse : Message
    {
        public ControllerMode mode;

        public ControllerModeResponse(ControllerMode mode)
        {
            this.mode = mode;
        }
    }
}
