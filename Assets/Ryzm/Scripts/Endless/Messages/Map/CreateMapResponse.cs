using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CreateMapResponse : Message
    {
        public MapType type;

        public CreateMapResponse(MapType type)
        {
            this.type = type;
        }
    }
}
