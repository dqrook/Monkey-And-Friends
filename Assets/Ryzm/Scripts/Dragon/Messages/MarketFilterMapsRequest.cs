using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class MarketFilterMapsRequest : Message
    {
        public string sender;
        
        public MarketFilterMapsRequest(string sender = "")
        {
            this.sender = sender;
        }
    }
}
