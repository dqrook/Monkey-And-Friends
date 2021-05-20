using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class AttemptLogin : Message
    {
        public string accountName;

        public AttemptLogin(string accountName)
        {
            this.accountName = accountName;
        }
    }
}
