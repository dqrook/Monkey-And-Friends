using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeVisualDatabase : ScriptableObject
	{
		public SlimeVisualConfig[] slimesVisuals;

		public SlimeVisualConfig GetSlimeVisualByID(string id)
		{
			SlimeVisualConfig res = null;

			for (int i = 0; i < slimesVisuals.Length; i++)
			{
				if (slimesVisuals[i].slimeID == id)
				{
					res = slimesVisuals[i];
					break;
				}
			}

			return res;
		}

		public SlimeVisualConfig GetRandomSlimeVisualConfig()
		{
			SlimeVisualConfig res = null;

			int rndIndex = Random.Range(0, slimesVisuals.Length);
			res = slimesVisuals[rndIndex];

			return res;
		}

		public SlimeVisualConfig GetSlimeVisualByIndex(int index)
		{
			SlimeVisualConfig res = slimesVisuals[index];
			return res;
		}
	}
}