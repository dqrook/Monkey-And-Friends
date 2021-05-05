using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CreateSectionRow : Message
    {
        public int numberOfSections;

        public CreateSectionRow()
        {
            this.numberOfSections = 5;
        }

        public CreateSectionRow(int numberOfSections)
        {
            this.numberOfSections = numberOfSections;
        }
    }
}
