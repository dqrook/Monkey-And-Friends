using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class MoveCameraRequest : Message
    {
        public TransformType type;

        public MoveCameraRequest(TransformType type)
        {
            this.type = type;
        }
    }
}
