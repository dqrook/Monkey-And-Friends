using System;

namespace Neon2.SlimeSystem
{
	public class CustomAnimationRunner
	{
		private CustomAnimationBinding runningAnimation;
		private CustomAnimationBinding nextAnimation;


		public void _Update(float deltaTime)
		{
			if (runningAnimation != null)
			{
				runningAnimation.UpdateAnimation(deltaTime);
			}
		}

		public void SetUpNextAnimation()
		{
			runningAnimation = nextAnimation;
			nextAnimation = null;

			runningAnimation.ResetAnimation();
			runningAnimation.StartAnimation();
		}

		public void PrepareNextAnimation(CustomAnimationBinding nextAnimation, bool waitForEnd)
		{
			PrepareNextAnimation(nextAnimation, waitForEnd, null);
		}

		public void PrepareNextAnimation(CustomAnimationBinding nextAnimation, bool waitForEnd, Action animationFinishedCallback)
		{
			this.nextAnimation = nextAnimation;
			this.nextAnimation.animationLaneCallbacksBindings.ResetLane();

			nextAnimation.SetAnimationFinishedCallback(animationFinishedCallback);



			if (runningAnimation == null)
			{
				SetUpNextAnimation();
			}
			else
			{
				if (!waitForEnd)
				{
					runningAnimation.AnimationFinished();
				}
			}
		}

		public void CheckNextAnimation()
		{
			if (nextAnimation != null)
			{
				SetUpNextAnimation();
			}
		}

		public void CurrentAnimationFinished()
		{
			runningAnimation = null;
		}
	}
}