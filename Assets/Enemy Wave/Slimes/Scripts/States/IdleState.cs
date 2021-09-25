using UnityEngine;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public class IdleState : AbstractSlimeState
	{
		private const string TAG = "SlimeController->IdleState";

		private float timer;
		private float selectedRestTime;
		public float[] restTimes;


		public SlimeMovement slimeMovement;
		public MeshRenderer meshRendererShadow;

		public override void Init(SlimeController slimeController, SlimeCore slimeCore)
		{
			base.Init(slimeController, slimeCore);
		}

		public override void Enter()
		{
			base.Enter();

			//CustomLogger.LogWarning(TAG, "ENTERING ON IDLE STATE!");

			slimeCore.slimeAnimatorController.GoToIdle();

			ResetTimer();
			slimeMovement.ResetInitialValues(true);

			meshRendererShadow.enabled = true;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);

			timer += deltaTime;
			if (timer >= selectedRestTime)
			{
				if (SlimeManager.instance != null)
				{
					if (SlimeManager.instance.CanPassToRunningState())
					{
						slimeCore.GoToJumpState();
					}
				}
				else
				{
					slimeCore.GoToJumpState();
				}

				timer = 0f;
			}
		}

		private void ResetTimer()
		{
			int rndIndex = UnityEngine.Random.Range(0, restTimes.Length);
			selectedRestTime = restTimes[rndIndex];
			timer = 0f;
		}

		public override void Exit()
		{
			base.Exit();

			//CustomLogger.Log(TAG, "Exit");
		}
	}
}