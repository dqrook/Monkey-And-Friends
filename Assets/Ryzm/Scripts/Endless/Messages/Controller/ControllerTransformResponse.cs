using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ControllerTransformResponse : Message
    {
        public Vector3 position;

        public ControllerTransformResponse(Vector3 position)
        {
            this.position = position;
        }
    }
}
