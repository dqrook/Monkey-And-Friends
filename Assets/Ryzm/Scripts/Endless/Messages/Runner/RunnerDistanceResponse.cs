using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RunnerDistanceResponse : Message
    {
        public float distance;

        public RunnerDistanceResponse() {}

        public RunnerDistanceResponse(float distance)
        {
            this.distance = distance;
        }
    }
}
