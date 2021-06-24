using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class GetMarketDragonsRequest : Message
    {
        public bool stopQuery;

        public GetMarketDragonsRequest()
        {
            this.stopQuery = false;
        }

        public GetMarketDragonsRequest(bool stopQuery)
        {
            this.stopQuery = stopQuery;
        }
    }
}
