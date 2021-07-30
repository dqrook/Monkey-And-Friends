using CodeControl;

namespace Ryzm.UI.Messages
{
    public class UpdateLoadingFadeMenu : Message
    {
        public float fadeFraction;

        public UpdateLoadingFadeMenu(float fadeFraction)
        {
            this.fadeFraction = fadeFraction < 0 ? 0 : fadeFraction > 1 ? 1 : fadeFraction;
        }
    }
}
