using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ControllersResponse : Message
    {
        public EndlessDragon dragon;
        public EndlessRyz ryz;

        public ControllersResponse(EndlessRyz ryz, EndlessDragon dragon)
        {
            this.ryz = ryz;
            this.dragon = dragon;
        }
    }
}
