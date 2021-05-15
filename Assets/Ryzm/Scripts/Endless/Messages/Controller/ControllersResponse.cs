using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ControllersResponse : Message
    {
        public EndlessMonkey monkey;
        public EndlessDragon dragon;

        public ControllersResponse(EndlessMonkey monkey, EndlessDragon dragon)
        {
            this.monkey = monkey;
            this.dragon = dragon;
        }
    }
}
