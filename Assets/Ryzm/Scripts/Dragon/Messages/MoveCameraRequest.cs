using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class MoveCameraRequest : Message
    {
        public CameraTransformType type;

        public MoveCameraRequest(CameraTransformType type)
        {
            this.type = type;
        }
    }
}
