using CodeControl;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon.Messages
{
    public class SingleDragonUpdate : Message
    {
        public EndlessDragon dragon;

        public SingleDragonUpdate(EndlessDragon dragon)
        {
            this.dragon = dragon;
        }
    }
}
