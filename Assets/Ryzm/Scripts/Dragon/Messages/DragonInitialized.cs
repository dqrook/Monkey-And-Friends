using CodeControl;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon.Messages
{
    public class DragonInitialized : Message
    {
        public EndlessDragon dragon;
        public int id;
        
        public DragonInitialized(int id)
        {
            this.id = id;
        }
        
        public DragonInitialized(EndlessDragon dragon)
        {
            this.dragon = dragon;
            this.id = dragon.data.id;
        }
    }
}
