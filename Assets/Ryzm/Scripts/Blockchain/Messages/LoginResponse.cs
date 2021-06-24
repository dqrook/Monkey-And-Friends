using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class LoginResponse : Message
    {
        public string accountName;
        public string nearUrl;
        public LoginStatus status;
        public string privateKey;
        public string secondaryPublicKey;

        public LoginResponse(string accountName, string nearUrl, LoginStatus status)
        {
            this.accountName = accountName;
            this.nearUrl = nearUrl;
            this.status = status;
            this.privateKey = "";
        }

        public LoginResponse(string accountName, string nearUrl, LoginStatus status, string privateKey)
        {
            this.accountName = accountName;
            this.nearUrl = nearUrl;
            this.status = status;
            this.privateKey = privateKey;
        }

        public LoginResponse(string accountName, string nearUrl, LoginStatus status, string privateKey, string secondaryPublicKey)
        {
            this.accountName = accountName;
            this.nearUrl = nearUrl;
            this.status = status;
            this.privateKey = privateKey;
            this.secondaryPublicKey = secondaryPublicKey;
        }
    }
}
