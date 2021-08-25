using System.Collections.Generic;
using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class UpdateDragonFilters : Message
    {
        public MarketFilter[] marketFilters;
        public bool sendApiRequest;

        public List<MarketFilter> MarketFilterList
        {
            get
            {
                List<MarketFilter> filters = new List<MarketFilter>();
                foreach(MarketFilter filter in marketFilters)
                {
                    filters.Add(filter);
                }
                return filters;
            }
        }

        public UpdateDragonFilters(List<MarketFilter> filters)
        {
            CreateFilters(filters);
        }

        public UpdateDragonFilters(List<MarketFilter> filters, bool sendApiRequest)
        {
            CreateFilters(filters);
            this.sendApiRequest = sendApiRequest;
        }

        void CreateFilters(List<MarketFilter> filters)
        {
            marketFilters = new MarketFilter[filters.Count];
            filters.CopyTo(marketFilters);
        }
    }
}
