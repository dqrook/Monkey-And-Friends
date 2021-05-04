using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RequestGameSpeedChange : Message
    {
        public float speed;
        public float lerpTime;

        public RequestGameSpeedChange() {}

        public RequestGameSpeedChange(float speed)
        {
            this.speed = speed;
            this.lerpTime = 0f;
        }
        
        public RequestGameSpeedChange(float speed, float lerpTime)
        {
            this.speed = speed;
            this.lerpTime = lerpTime;
        }
    }
}
