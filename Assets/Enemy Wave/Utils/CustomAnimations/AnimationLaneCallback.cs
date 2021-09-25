using System;
using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class AnimationLaneCallback : ScriptableObject
	{
		private Action callback;

		public void Init(Action callback)
		{
			this.callback = callback;
		}

		public void Fire()
		{
			if (callback != null)
			{
				callback();
			}
		}
	}
}