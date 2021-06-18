using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class BreedingStatusUpdate : Message
    {
        public BreedingStatus status;

        public BreedingStatusUpdate(BreedingStatus status)
        {
            this.status = status;
        }
    }
}
