using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RunnerHit : Message
    {
        public int damage;

        public RunnerHit()
        {
            this.damage = 1000;
        }

        public RunnerHit(int damage)
        {
            this.damage = damage;
        }
    }
}
