using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DisplayDragonZoomResponse : Message
    {
        public bool failed;

        public DisplayDragonZoomResponse()
        {
            this.failed = false;
        }

        public DisplayDragonZoomResponse(bool failed)
        {
            this.failed = failed;
        }
    }
}
