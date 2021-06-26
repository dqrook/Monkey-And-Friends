using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class NumberOfMarketDragonsResponse : Message
    {
        public int numberOfDragonsOnMarket;
        public NumberOfMarketDragonsResponse(int numberOfDragonsOnMarket)
        {
            this.numberOfDragonsOnMarket = numberOfDragonsOnMarket;
        }
    }
}
