using CodeControl;

namespace Ryzm.Blockchain.Messages
{
    public class LoginStatusUpdate : Message
    {
        public LoginStatus status;

        public LoginStatusUpdate(LoginStatus status)
        {
            this.status = status;
        }
    }
}
