using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class CreateCredentialsResponse : Message
    {
        public string nearUrl;
        
        public CreateCredentialsResponse(string nearUrl)
        {
            this.nearUrl = nearUrl;
        }
    }
}
