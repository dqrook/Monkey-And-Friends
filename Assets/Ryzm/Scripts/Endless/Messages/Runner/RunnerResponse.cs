using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RunnerResponse : Message
    {
        public RunnerController runner;

        public RunnerResponse() {}

        public RunnerResponse(RunnerController runner)
        {
            this.runner = runner;
        }
    }
}
