using Ryzm.EndlessRunner;
using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class AddDragonToMarketResponse : Message
    {
        public TransactionStatus status;
        public DragonResponse data;

        public AddDragonToMarketResponse(TransactionStatus status)
        {
            this.status = status;
        }

        public AddDragonToMarketResponse(TransactionStatus status, DragonResponse data)
        {
            this.status = status;
            this.data = data;
        }
    }
}
