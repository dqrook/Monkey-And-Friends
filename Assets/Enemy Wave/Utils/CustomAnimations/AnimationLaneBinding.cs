using UnityEngine;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public class AnimationLaneBinding
	{
		public AnimationLane2 animationLane;
		public Transform animatedTransform;

		private Vector3 currentLaneValue;
		private int currentKeyframeIndex;

		public void Update(float animationTimer)
		{
			float t = GetStep(animationTimer);

			currentLaneValue = Vector3.Lerp(animationLane.keyframes[currentKeyframeIndex].value, animationLane.keyframes[currentKeyframeIndex + 1].value, t);
			ApplyLaneValue();

			if (t >= 1f)
			{
				NextKeyframe();
			}
		}

		public void ResetLane()
		{
			currentKeyframeIndex = 0;
			currentLaneValue = animationLane.GetLastKeyframe().value;
			ApplyLaneValue();
		}

		public void ApplyLaneValue()
		{
			switch (animationLane.animatedProperty)
			{
				case AnimationLane2.AnimatedProperty.LOCAL_POSITION:
					animatedTransform.localPosition = currentLaneValue;
					break;
				case AnimationLane2.AnimatedProperty.LOCAL_SCALE:
					animatedTransform.localScale = currentLaneValue;
					break;
				case AnimationLane2.AnimatedProperty.LOCAL_ROTATION:
					animatedTransform.localEulerAngles = currentLaneValue;
					break;
			}
		}

		private void NextKeyframe()
		{
			currentKeyframeIndex++;
			if (currentKeyframeIndex >= animationLane.keyframes.Length - 1)
			{
				currentKeyframeIndex = 0;
			}
		}

		private float GetStep(float timer)
		{
			float res = Mathf.InverseLerp(animationLane.keyframes[currentKeyframeIndex].time, animationLane.keyframes[currentKeyframeIndex + 1].time, timer);
			return res;
		}
	}
}