using CodeControl;

namespace Ryzm.UI.Messages
{
    public class DisableDragonFilterPanel : Message
    {
        public bool resetValue;

        public DisableDragonFilterPanel()
        {
            this.resetValue = false;
        }

        public DisableDragonFilterPanel(bool resetValue)
        {
            this.resetValue = resetValue;
        }
    }
}
