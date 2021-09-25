using System;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public class AnimationLaneCallbacksBinding
	{
		public AnimationLane2 animationLane;

		private int currentKeyframeIndex;
		private int lastCallbackFiredIndex;
		private Action<CustomAnimation.CallbackMethod> action;

		public void Init(Action<CustomAnimation.CallbackMethod> action)
		{
			this.action = action;
		}

		public void Update(float animationTimer)
		{
			if (animationLane == null) { return; }
			if (animationLane.keyframes.Length <= 0) { return; }


			if (animationTimer >= animationLane.keyframes[currentKeyframeIndex].time && lastCallbackFiredIndex < currentKeyframeIndex)
			{
				action(animationLane.keyframes[currentKeyframeIndex].methodName);
				lastCallbackFiredIndex = currentKeyframeIndex;
				currentKeyframeIndex++;

				if (currentKeyframeIndex >= animationLane.keyframes.Length)
				{
					currentKeyframeIndex = 0;
				}
			}
		}

		public void ResetLane()
		{
			currentKeyframeIndex = 0;
			lastCallbackFiredIndex = -1;
		}
	}
}