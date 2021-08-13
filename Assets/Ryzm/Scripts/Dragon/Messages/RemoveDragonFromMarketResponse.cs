using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class RemoveDragonFromMarketResponse : Message
    {
        public TransactionStatus status;
        public DragonResponse data;

        public RemoveDragonFromMarketResponse(TransactionStatus status)
        {
            this.status = status;
        }

        public RemoveDragonFromMarketResponse(TransactionStatus status, DragonResponse data)
        {
            this.data = data;
            this.status = status;
        }
    }
}
