using CodeControl;
using System.Collections.Generic;

namespace Ryzm.Dragon.Messages
{
    public class QueryMarketRequest : Message
    {
        public MarketQueryType type;
        public List<MarketFilter> filters;
        public int page;

        public QueryMarketRequest()
        {
            this.type = MarketQueryType.Start;
        }

        public QueryMarketRequest(List<MarketFilter> filters)
        {
            this.filters = filters;
            this.type = MarketQueryType.UpdateFilters;
        }

        public QueryMarketRequest(int page)
        {
            this.page = page;
            this.type = MarketQueryType.UpdatePage;
        }

        public QueryMarketRequest(MarketQueryType type)
        {
            this.type = type;
        }
    }
}
