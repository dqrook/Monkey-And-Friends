using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimesSpawnerController : MonoBehaviour
	{
		public int numSlimesToSpawn = 1;
		public SlimeVisualDatabase slimeVisualDatabase;
		public SlimeController prefabSlime;
		public Transform slimesParent;
		public SlimeSpawner[] spawners;

		private void Awake()
		{
			Shader.WarmupAllShaders();
			SpawnSlimes(numSlimesToSpawn);
		}

		public void SpawnSlimes(int numSlimesToSpawn)
		{
			for (int i = 0; i < numSlimesToSpawn; i++)
			{
				SlimeSpawner slimeSpawner = GetRandomSpawner();
				Vector3 slimePos = slimeSpawner.GetRandomPosition(Vector3.forward, Vector3.up);
				Vector3 randomDir = slimeSpawner.GetRandomDir(Vector3.forward, Vector3.up);



				SlimeController slimeController = Instantiate<SlimeController>(prefabSlime);
				slimeController.Init();
				slimeController.SetSlimeVisual(slimeVisualDatabase.GetRandomSlimeVisualConfig());
				slimeController.transform.SetParent(slimesParent);
				slimeController.transform.position = slimePos + Vector3.up * 0.45f;
				slimeController.transform.forward = randomDir;
				slimeController.gameObject.SetActive(true);
				slimeController.GoToIdleState();
			}
		}

		private SlimeSpawner GetRandomSpawner()
		{
			int idx = Random.Range(0, spawners.Length);
			SlimeSpawner res = spawners[idx];

			return res;
		}
	}
}