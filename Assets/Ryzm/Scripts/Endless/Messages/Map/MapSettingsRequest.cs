using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class MapSettingsRequest : Message
    {
        public MapType type;
        public bool getCurrent;
        public string requestId;

        public MapSettingsRequest()
        {
            this.getCurrent = true;
            this.requestId = "";
        }

        public MapSettingsRequest(string requestId)
        {
            this.getCurrent = true;
            this.requestId = requestId;
        }

        public MapSettingsRequest(MapType type, string requestId)
        {
            this.type = type;
            this.requestId = requestId;
        }
    }
}
