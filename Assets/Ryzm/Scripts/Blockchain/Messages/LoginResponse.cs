using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class LoginResponse : Message
    {
        public string accountName;
        public string nearUrl;
        public LoginStatus status;

        public LoginResponse(string accountName, string nearUrl, LoginStatus status)
        {
            this.accountName = accountName;
            this.nearUrl = nearUrl;
            this.status = status;
        }
    }
}
