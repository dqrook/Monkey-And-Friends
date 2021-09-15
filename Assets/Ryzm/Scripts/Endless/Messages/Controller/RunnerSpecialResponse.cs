using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RunnerSpecialResponse : Message
    {
        public float special;
        public int maxSpecial;

        public RunnerSpecialResponse(float special, int maxSpecial)
        {
            this.special = special;
            this.maxSpecial = maxSpecial;
        }
    }
}
