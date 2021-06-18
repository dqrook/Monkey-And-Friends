using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class BreedDragonsResponse : Message
    {
        public BreedingStatus status;
        public int dragonId;

        public BreedDragonsResponse(BreedingStatus status)
        {
            this.status = status;
        }

        public BreedDragonsResponse(BreedingStatus status, int dragonId)
        {
            this.status = status;
            this.dragonId = dragonId;
        }
    }
}
