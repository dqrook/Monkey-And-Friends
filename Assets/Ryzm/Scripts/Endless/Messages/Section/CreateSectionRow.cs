using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CreateSectionRow : Message
    {
        public int numberOfSections;

        public CreateSectionRow()
        {
            this.numberOfSections = 3;
        }

        public CreateSectionRow(int numberOfSections)
        {
            this.numberOfSections = numberOfSections;
        }
    }
}
