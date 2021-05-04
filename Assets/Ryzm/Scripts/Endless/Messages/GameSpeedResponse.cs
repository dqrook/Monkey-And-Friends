using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class GameSpeedResponse : Message
    {
        public float speed;

        public GameSpeedResponse() {}

        public GameSpeedResponse(float speed)
        {
            this.speed = speed;
        }
    }
}

