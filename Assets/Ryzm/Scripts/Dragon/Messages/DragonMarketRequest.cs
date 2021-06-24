using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DragonMarketRequest : Message
    {
        public MarketStatus status;
        public int startIndex;
        public int numberOfDragons;

        public DragonMarketRequest(MarketStatus status)
        {
            this.status = status;
        }

        public DragonMarketRequest(int startIndex, int numberOfDragons = 5)
        {
            this.status = MarketStatus.Update;
            this.startIndex = startIndex;
            this.numberOfDragons = numberOfDragons;
        }
    }
}
