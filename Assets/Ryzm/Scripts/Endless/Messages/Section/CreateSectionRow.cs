using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CreateSectionRow : Message
    {
        public int numberOfSections;

        public CreateSectionRow()
        {
            this.numberOfSections = 4;
        }

        public CreateSectionRow(int numberOfSections)
        {
            this.numberOfSections = numberOfSections;
        }
    }
}
