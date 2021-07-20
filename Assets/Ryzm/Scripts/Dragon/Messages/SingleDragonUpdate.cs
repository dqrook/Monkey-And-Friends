using CodeControl;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon.Messages
{
    public class SingleDragonUpdate : Message
    {
        public BaseDragon dragon;

        public SingleDragonUpdate(BaseDragon dragon)
        {
            this.dragon = dragon;
        }
    }
}
