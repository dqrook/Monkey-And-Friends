using CodeControl;

namespace Ryzm.UI.Messages
{
    public class ActivateTimedLoadingMenu : Message
    {
        public float timeoutTime;

        public ActivateTimedLoadingMenu()
        {
            this.timeoutTime = 1;
        }

        public ActivateTimedLoadingMenu(float timeoutTime)
        {
            this.timeoutTime = timeoutTime;
        }
    }
}
