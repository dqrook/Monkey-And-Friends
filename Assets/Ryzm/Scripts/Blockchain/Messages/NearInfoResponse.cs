using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class NearInfoResponse : Message 
    {
        public string accountName;
        public NearInfoResponse(string accountName)
        {
            this.accountName = accountName;
        }
    }
}
