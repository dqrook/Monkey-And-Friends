using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeManager : Singleton<SlimeManager>
	{
		public int maxRunningSlimes;
		private int currentAmountRunningSlimes;

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			currentAmountRunningSlimes = 0;
		}

		public bool CanPassToRunningState()
		{
			bool res = false;

			if (currentAmountRunningSlimes < maxRunningSlimes)
			{
				res = true;
			}

			return res;
		}

		public void SlimeOnRunningState()
		{
			currentAmountRunningSlimes++;
		}

		public void SlimeOutOfRunningState()
		{
			currentAmountRunningSlimes--;
			currentAmountRunningSlimes = Mathf.Clamp(currentAmountRunningSlimes, 0, maxRunningSlimes);
		}
	}
}