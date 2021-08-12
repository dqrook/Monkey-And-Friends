using Ryzm.Dragon;
using CodeControl;

namespace Ryzm.UI.Messages
{
    public class EnableDragonInfoPanel : Message
    {
        public DragonResponse singleDragonData;

        public EnableDragonInfoPanel(DragonResponse singleDragonData)
        {
            this.singleDragonData = singleDragonData;
        }
    }
}
