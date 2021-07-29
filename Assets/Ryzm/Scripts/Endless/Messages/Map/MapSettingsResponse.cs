using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class MapSettingsResponse : Message
    {
        public MapSettings settings;
        public string requestId;

        public MapSettingsResponse(MapSettings settings, string requestId)
        {
            this.settings = settings;
            this.requestId = requestId;
        }
    }
}
