using UnityEngine;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public class Keyframe
	{
		public float time;
		public Vector3 value;
		public CustomAnimation.CallbackMethod methodName;
	}
}