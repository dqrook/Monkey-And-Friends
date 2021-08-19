using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class QueryMarketResponse : Message
    {
        public DragonCardMetadata[] dragons;
        public int page;
        public int totalNumDragons;

        public QueryMarketResponse(DragonCardMetadata[] dragons, int page, int totalNumDragons)
        {
            this.dragons = dragons;
            
            this.page = page;
            this.totalNumDragons = totalNumDragons;
        }
    }
}
