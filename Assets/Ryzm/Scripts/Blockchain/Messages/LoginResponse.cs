using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class LoginResponse : Message
    {
        public string nearUrl;

        public LoginResponse(string nearUrl)
        {
            this.nearUrl = nearUrl;
        }
    }
}
