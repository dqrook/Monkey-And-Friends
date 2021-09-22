using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class UpdateCurrentCameraSpawn : Message
    {
        public int currentCameraSpawn;

        public UpdateCurrentCameraSpawn(int currentCameraSpawn)
        {
            this.currentCameraSpawn = currentCameraSpawn;
        }
    }
}
