using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class CustomAnimation : ScriptableObject
	{
		public enum CallbackMethod
		{
			CANDY_EATEN,
			SLIME_JUMP,
		}

		public bool looped;
		public AnimationLane2[] lanes;

		public float GetTotalAnimationTime()
		{
			float res = 0f;

			for (int i = 0; i < lanes.Length; i++)
			{
				float laneTime = lanes[i].GetLastKeyframe().time;
				if (laneTime > res)
				{
					res = laneTime;
				}
			}

			return res;
		}

#if UNITY_EDITOR
		public void ApplyTimeFactor(float timeFactor)
		{
			for (int i = 0; i < lanes.Length; i++)
			{
				lanes[i].ApplyFactorToTime(timeFactor);
			}

			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
	}
}