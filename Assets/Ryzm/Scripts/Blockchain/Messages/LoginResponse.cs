using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class LoginResponse : Message
    {
        public string accountName;
        public string nearUrl;
        public bool loggedIn;
        public bool fetchingAccessKeys;

        public LoginResponse(string accountName, string nearUrl, bool loggedIn, bool fetchingAccessKeys)
        {
            this.accountName = accountName;
            this.nearUrl = nearUrl;
            this.loggedIn = loggedIn;
            this.fetchingAccessKeys = fetchingAccessKeys;
        }
    }
}
