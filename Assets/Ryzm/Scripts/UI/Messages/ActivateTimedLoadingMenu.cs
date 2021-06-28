using CodeControl;

namespace Ryzm.UI.Messages
{
    public class ActivateTimedLoadingMenu : Message
    {
        public float timeoutTime;
        public bool infiniteTime;

        public ActivateTimedLoadingMenu()
        {
            this.timeoutTime = 1;
            this.infiniteTime = false;
        }

        public ActivateTimedLoadingMenu(float timeoutTime)
        {
            this.timeoutTime = timeoutTime;
            this.infiniteTime = false;
        }

        public ActivateTimedLoadingMenu(bool infiniteTime)
        {
            if(infiniteTime)
            {
                this.infiniteTime = true;
            }
            else
            {
                this.timeoutTime = 1;
                this.infiniteTime = false;
            }
        }
    }
}
