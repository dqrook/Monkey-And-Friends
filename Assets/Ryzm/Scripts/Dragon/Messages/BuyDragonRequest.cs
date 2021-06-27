using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class BuyDragonRequest : Message
    {
        public int dragonId;
        public float price;

        public BuyDragonRequest(int dragonId, float price)
        {
            this.dragonId = dragonId;
            this.price = price;
        }
    }
}
