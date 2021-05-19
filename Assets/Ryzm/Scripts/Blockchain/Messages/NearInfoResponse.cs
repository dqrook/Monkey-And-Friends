using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class NearInfoResponse : Message 
    {
        public string accountName;
        public string nearUrl;
        public bool loggedIn;
        public bool fetchingAccessKeys;

        public NearInfoResponse(string accountName, string nearUrl, bool loggedIn, bool fetchingAccessKeys = false)
        {
            this.accountName = accountName;
            this.nearUrl = nearUrl;
            this.loggedIn = loggedIn;
            this.fetchingAccessKeys = fetchingAccessKeys;
        }
    }
}
