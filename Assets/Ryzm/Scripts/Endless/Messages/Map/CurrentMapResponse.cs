using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CurrentMapResponse : Message
    {
        public MapType type;

        public CurrentMapResponse(MapType type)
        {
            this.type = type;
        }
    }
}
