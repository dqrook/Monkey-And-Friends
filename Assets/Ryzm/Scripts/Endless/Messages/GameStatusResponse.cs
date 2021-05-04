using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class GameStatusResponse : Message
    {
       public GameStatus status;

       public GameStatusResponse() {}

        public GameStatusResponse(GameStatus status)
        {
            this.status = status;
        }
    }
}
