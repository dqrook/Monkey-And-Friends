using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class BreedDragonsResponse : Message
    {
        public TransactionStatus status;
        public int dragonId;

        public BreedDragonsResponse(TransactionStatus status)
        {
            this.status = status;
        }

        public BreedDragonsResponse(TransactionStatus status, int dragonId)
        {
            this.status = status;
            this.dragonId = dragonId;
        }
    }
}
