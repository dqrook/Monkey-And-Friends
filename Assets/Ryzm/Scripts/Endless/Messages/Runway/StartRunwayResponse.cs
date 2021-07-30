using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class StartRunwayResponse : Message
    {
        public bool hasRunway;
        public MapType type;

        public StartRunwayResponse(bool hasRunway, MapType type)
        {
            this.hasRunway = hasRunway;
            this.type = type;
        }
    }
}
