using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class AnimationLane2 : ScriptableObject
	{
		public enum AnimatedProperty
		{
			LOCAL_POSITION,
			LOCAL_ROTATION,
			LOCAL_SCALE,
			CALLBACKS,
		}

		public AnimatedProperty animatedProperty;
		public Keyframe[] keyframes;

		public Keyframe GetLastKeyframe()
		{
			Keyframe res = keyframes[keyframes.Length - 1];
			return res;
		}

#if UNITY_EDITOR
		public void ApplyFactorToTime(float factor)
		{
			for (int i = 0; i < keyframes.Length; i++)
			{
				keyframes[i].time *= factor;
			}

			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
	}
}