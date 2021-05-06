using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CurrentPositionResponse : Message
    {
        public int position;

        public CurrentPositionResponse() {}

        public CurrentPositionResponse(int position)
        {
            this.position = position;
        }
    }
}
