using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CameraSpawnsResponse : Message
    {
        public EndlessCameraSpawns cameraSpawns;

        public CameraSpawnsResponse(EndlessCameraSpawns cameraSpawns)
        {
            this.cameraSpawns = cameraSpawns;
        }
    }
}
