using CodeControl;
using Ryzm.Dragon;

namespace Ryzm.UI.Messages
{
    public class DragonCardMetadataResponse : Message
    {
        public DragonCardMetadata[] dragonCards;

        public DragonCardMetadataResponse(DragonCardMetadata[] dragonCards)
        {
            this.dragonCards = dragonCards;
        }
    }
}
