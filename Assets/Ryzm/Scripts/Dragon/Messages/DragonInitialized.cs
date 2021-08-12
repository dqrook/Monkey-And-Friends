using CodeControl;
namespace Ryzm.Dragon.Messages
{
    public class DragonInitialized : Message
    {
        public int id;
        
        public DragonInitialized(int id)
        {
            this.id = id;
        }
    }
}
