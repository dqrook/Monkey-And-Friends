using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RowComplete : Message
    {
        public int rowId;

        public RowComplete(int rowId)
        {
            this.rowId = rowId;
        }
    }
}
