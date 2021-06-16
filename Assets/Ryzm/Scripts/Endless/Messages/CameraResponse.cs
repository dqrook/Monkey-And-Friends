using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CameraResponse : Message
    {
        public GameObject camera;

        public CameraResponse(GameObject camera)
        {
            this.camera = camera;
        }
    }
}
