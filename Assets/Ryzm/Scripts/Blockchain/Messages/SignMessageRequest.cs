using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class SignMessageRequest : Message
    {
        public string sender;
        public string message;

        public SignMessageRequest(string sender, string message) 
        {
            this.sender = sender;
            this.message = message;
        }
    }
}
