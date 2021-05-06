using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class SectionDeactivated : Message
    {
        public EndlessSection section;

        public SectionDeactivated() {}

        public SectionDeactivated(EndlessSection section)
        {
            this.section = section;
        }
    }
}

