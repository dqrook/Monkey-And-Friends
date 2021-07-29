using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class StartRunway : Message
    {
        public MapType type;

        public StartRunway(MapType type)
        {
            this.type = type;
        }
    }
}
