using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ChangeGameSpeed : Message
    {
        public float speed;
        public float lerpTime;

        public ChangeGameSpeed() {}

        public ChangeGameSpeed(float speed)
        {
            this.speed = speed;
            this.lerpTime = 0f;
        }
        
        public ChangeGameSpeed(float speed, float lerpTime)
        {
            this.speed = speed;
            this.lerpTime = lerpTime;
        }
    }
}
