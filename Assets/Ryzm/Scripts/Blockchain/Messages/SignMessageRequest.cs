using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class SignMessageRequest : Message
    {
        public string action;
        public string message;

        public SignMessageRequest(string action, string message) 
        {
            this.action = action;
            this.message = message;
        }
    }
}
