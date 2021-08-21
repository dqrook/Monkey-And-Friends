using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class QueryMarketResponse : Message
    {
        public DragonCardMetadata[] dragons;
        public int page;
        public int totalNumDragons;
        public int numNewDragons; 

        public QueryMarketResponse(int numNewDraagons, int page, int totalNumDragons)
        {
            this.numNewDragons = numNewDraagons;
            this.page = page;
            this.totalNumDragons = totalNumDragons;
        }

        public QueryMarketResponse(DragonCardMetadata[] dragons, int page, int totalNumDragons)
        {
            this.dragons = dragons;
            this.numNewDragons = dragons.Length;
            this.page = page;
            this.totalNumDragons = totalNumDragons;
        }
    }
}
