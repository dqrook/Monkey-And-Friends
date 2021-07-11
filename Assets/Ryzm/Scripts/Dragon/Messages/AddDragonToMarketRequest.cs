using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class AddDragonToMarketRequest : Message
    {
        public int dragonId;
        public float price;

        public AddDragonToMarketRequest(int dragonId, float price)
        {
            this.dragonId = dragonId;
            this.price = price;
        }
    }
}
