using System;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public class CustomAnimationBinding
	{
		public enum State
		{
			RUNNING,
			STOPPED,
		}

		public CustomAnimation animation;
		public AnimationLaneBinding[] animationLanesBindings;
		public AnimationLaneCallbacksBinding animationLaneCallbacksBindings;

		private CustomAnimationRunner animationRunner;
		private State state;
		private Action onAnimationFinishedCallback;
		private float totalAnimationTime;
		private float animationTimer;


		public void Init(Action<CustomAnimation.CallbackMethod> actionCallbacks, CustomAnimationRunner animationRunner)
		{
			animationLaneCallbacksBindings.Init(actionCallbacks);

			this.animationRunner = animationRunner;

			animationTimer = 0f;
			totalAnimationTime = 0f;
			state = State.STOPPED;
		}

		public void SetAnimationFinishedCallback(Action onAnimationFinishedCallback)
		{
			this.onAnimationFinishedCallback = onAnimationFinishedCallback;
		}

		public void StartAnimation()
		{
			state = State.RUNNING;

			totalAnimationTime = animation.GetTotalAnimationTime();
			animationTimer = 0f;
		}

		public void StopAnimation()
		{
			state = State.STOPPED;
		}

		public void UpdateAnimation(float deltaTime)
		{
			if (state == State.RUNNING)
			{
				for (int i = 0; i < animationLanesBindings.Length; i++)
				{
					animationLanesBindings[i].Update(animationTimer);
				}

				animationLaneCallbacksBindings.Update(animationTimer);

				if (animationTimer >= totalAnimationTime)
				{
					AnimationFinished();
				}

				animationTimer += deltaTime;
			}
		}

		public void AnimationFinished()
		{
			if (state == State.RUNNING)
			{
				if (animation.looped)
				{
					ResetAnimation();
				}
				else
				{
					StopAnimation();
					animationRunner.CurrentAnimationFinished();
				}

				if (onAnimationFinishedCallback != null)
				{
					onAnimationFinishedCallback();
					onAnimationFinishedCallback = null;
				}


				animationRunner.CheckNextAnimation();
			}
		}

		public void ResetAnimation()
		{
			for (int i = 0; i < animationLanesBindings.Length; i++)
			{
				animationLanesBindings[i].ResetLane();
			}

			animationLaneCallbacksBindings.ResetLane();

			totalAnimationTime = animation.GetTotalAnimationTime();
			animationTimer = 0f;
		}

		public State GetState()
		{
			return state;
		}


#if UNITY_EDITOR
		public void SetUpLanesBindings()
		{
			animationLanesBindings = new AnimationLaneBinding[animation.lanes.Length];
			for (int i = 0; i < animationLanesBindings.Length; i++)
			{
				animationLanesBindings[i] = new AnimationLaneBinding();
				animationLanesBindings[i].animationLane = animation.lanes[i];
			}
		}
#endif
	}
}