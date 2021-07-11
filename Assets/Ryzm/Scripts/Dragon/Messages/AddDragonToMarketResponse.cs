using Ryzm.EndlessRunner;
using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class AddDragonToMarketResponse : Message
    {
        public TransactionStatus status;
        public int dragonId;
        public float price;

        public AddDragonToMarketResponse(TransactionStatus status)
        {
            this.status = status;
        }

        public AddDragonToMarketResponse(TransactionStatus status, int dragonId, float price)
        {
            this.status = status;
            this.dragonId = dragonId;
            this.price = price;
        }
    }
}
