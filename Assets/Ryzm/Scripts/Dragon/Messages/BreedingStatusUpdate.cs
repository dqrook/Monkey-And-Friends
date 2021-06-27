using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class BreedingStatusUpdate : Message
    {
        public TransactionStatus status;

        public BreedingStatusUpdate(TransactionStatus status)
        {
            this.status = status;
        }
    }
}
