using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class SignMessageResponse : Message
    {
        public string action;
        public string message;
        public byte[] signedMessageBytes;
        public string publicKey;
        public string accountId;
        public bool isSuccess;

        public SignMessageResponse()
        {
            this.isSuccess = false;
        }
        public SignMessageResponse(string action, string message, byte[] signedMessageBytes, string publicKey, string accountId)
        {
            this.action = action;
            this.message = message;
            this.signedMessageBytes = signedMessageBytes;
            this.publicKey = publicKey;
            this.accountId = accountId;
            this.isSuccess = true;
        }

    }
}
