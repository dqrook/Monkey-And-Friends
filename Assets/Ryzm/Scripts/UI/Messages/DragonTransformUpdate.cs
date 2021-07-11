using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DragonTransformUpdate : Message
    {
        public int dragonId;
        public DragonSpawnType spawnType;

        public DragonTransformUpdate(int dragonId, DragonSpawnType spawnType)
        {
            this.dragonId = dragonId;
            this.spawnType = spawnType;
        }
    }
}
