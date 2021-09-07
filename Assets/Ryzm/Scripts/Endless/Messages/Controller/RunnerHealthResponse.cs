using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RunnerHealthResponse : Message
    {
        public int health;
        public int maxHealth;

        public RunnerHealthResponse(int health, int maxHealth)
        {
            this.health = health;
            this.maxHealth = maxHealth;
        }
    }
}
