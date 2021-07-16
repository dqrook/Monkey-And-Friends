using CodeControl;

namespace Ryzm.Messages
{
    public class GameTypeResponse : Message
    {
        public GameType type;

        public GameTypeResponse(GameType type)
        {
            this.type = type;
        }
    }
}
