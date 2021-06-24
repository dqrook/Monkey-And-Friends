using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class UpdateMarketDragonsRequest : Message
    {
        public int startIndex;
        public int numberOfDragons;

        public UpdateMarketDragonsRequest(int startIndex, int numberOfDragons = 5)
        {
            this.startIndex = startIndex;
            this.numberOfDragons = numberOfDragons;
        }
    }
}
