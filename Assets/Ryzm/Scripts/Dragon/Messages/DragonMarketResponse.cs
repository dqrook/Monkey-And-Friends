using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DragonMarketResponse : Message
    {
        public MarketStatus status;
        public MarketDragonData[] data;

        public DragonMarketResponse(MarketStatus status)
        {
            this.status = status;
        }

        public DragonMarketResponse(MarketStatus status, MarketDragonData[] data)
        {
            this.status = status;
            this.data = data;
        }
    }
}
