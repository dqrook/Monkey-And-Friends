using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class BuyDragonResponse : Message
    {
        public TransactionStatus status;
        public int dragonId;

        public BuyDragonResponse(TransactionStatus status)
        {
            this.status = status;
        }

        public BuyDragonResponse(TransactionStatus status, int dragonId)
        {
            this.status = status;
            this.dragonId = dragonId;
        }
    }
}
