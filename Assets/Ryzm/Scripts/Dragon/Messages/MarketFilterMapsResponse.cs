using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class MarketFilterMapsResponse : Message
    {
        public MarketFilterMaps marketFilterMaps;
        public string receiver;

        public MarketFilterMapsResponse(MarketFilterMaps marketFilterMaps, string receiver = "")
        {
            this.marketFilterMaps = marketFilterMaps;
            this.receiver = receiver;
        }
    }
}
