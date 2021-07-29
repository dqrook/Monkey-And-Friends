using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class AddTransitionRequest : Message
    {
        public int rowId;

        public AddTransitionRequest(int rowId)
        {
            this.rowId = rowId;
        }
    }
}
