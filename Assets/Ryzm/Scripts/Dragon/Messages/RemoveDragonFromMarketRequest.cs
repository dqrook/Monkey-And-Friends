using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class RemoveDragonFromMarketRequest : Message
    {
        public int dragonId;

        public RemoveDragonFromMarketRequest(int dragonId)
        {
            this.dragonId = dragonId;
        }
    }
}
