using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public abstract class EndlessBasePath : EndlessScroller
    {
        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Restart)
            {
                gameObject.SetActive(false);
            }
            else if(gameStatusResponse.status == GameStatus.Ended)
            {
                CancelDeactivation();
            }
        }

        public virtual void Enter() {}

        public virtual void Exit() {}

        public virtual void CancelDeactivation() {}
    }
}
