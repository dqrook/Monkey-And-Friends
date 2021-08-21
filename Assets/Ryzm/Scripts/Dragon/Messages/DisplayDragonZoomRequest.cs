using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DisplayDragonZoomRequest : Message
    {
        public int displayDragonIndex;
        public string dragonId;

        public DisplayDragonZoomRequest(int displayDragonIndex, string dragonId)
        {
            this.displayDragonIndex = displayDragonIndex;
            this.dragonId = dragonId;
        }
    }

}
