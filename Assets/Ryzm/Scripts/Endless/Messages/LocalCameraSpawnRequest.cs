using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class LocalCameraSpawnRequest : Message
    {
        public string sender;

        public LocalCameraSpawnRequest()
        {
            this.sender = "All";
        }

        public LocalCameraSpawnRequest(string sender)
        {
            this.sender = sender;
        }
    }
}
