using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RowChange : Message
    {
        public int oldRowId;
        public int newRowId;

        public RowChange(int oldRowId, int newRowId)
        {
            this.oldRowId = oldRowId;
            this.newRowId = newRowId;
        }
    }
}
