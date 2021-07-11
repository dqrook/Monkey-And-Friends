using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class RemoveDragonFromMarketResponse : Message
    {
        public int dragonId;
        public TransactionStatus status;

        public RemoveDragonFromMarketResponse(TransactionStatus status)
        {
            this.status = status;
        }

        public RemoveDragonFromMarketResponse(TransactionStatus status, int dragonId)
        {
            this.dragonId = dragonId;
            this.status = status;
        }
    }
}
