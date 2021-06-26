using UnityEngine;
using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DragonCameraResponse : Message
    {
        public GameObject camera;

        public DragonCameraResponse(GameObject camera)
        {
            this.camera = camera;
        }
    }
}