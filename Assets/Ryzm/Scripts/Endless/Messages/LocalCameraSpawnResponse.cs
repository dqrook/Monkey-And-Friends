using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class LocalCameraSpawnResponse : Message
    {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;

        public LocalCameraSpawnResponse(CameraSpawn cameraSpawn)
        {
            this.localPosition = cameraSpawn.localPosition;
            this.localEulerAngles = cameraSpawn.localEulerAngles;
        }
    }
}
