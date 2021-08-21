using CodeControl;
using System.Collections.Generic;

namespace Ryzm.Dragon.Messages
{
    public class MarketCameraTransformsResponse : Message
    {
        public List<CameraTransform> transforms = new List<CameraTransform>();

        public MarketCameraTransformsResponse(List<CameraTransform> transforms)
        {
            this.transforms = transforms;
        }
    }
}
