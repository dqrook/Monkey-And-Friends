using System;
using UnityEngine;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public class JumpState : AbstractSlimeState
	{
		private const string TAG = "SlimeController->JumpState";

		private enum AnimationState
		{
			WAITING_FOR_RANDOM_INIT,
			WAITING_FOR_FIRST_JUMP,
			JUMPING,
		}

		public SlimeMovement slimeMovement;

		public float minTimeToStart;
		public float maxTimeToStart;
		private float timeToStart;
		private float auxTimer;
		private AnimationState animationState;


		public MeshRenderer meshRendererShadow;



		public RealTimeTimer idleStateTimer;


		//DELEGATES
		private Action goToIdleAction;



		public override void Init(SlimeController slimeController, SlimeCore slimeCore)
		{
			base.Init(slimeController, slimeCore);

			goToIdleAction = GoToIdle;
		}

		public override void Enter()
		{
			base.Enter();
			//CustomLogger.Log(TAG, "Enter!");



			float jumpTime = UnityEngine.Random.Range(slimeController.minJumpTime, slimeController.maxJumpTime);
			idleStateTimer.Setup(jumpTime, TimeUnit.SECONDS, false, goToIdleAction);

			idleStateTimer.RunTimer();

			animationState = AnimationState.WAITING_FOR_RANDOM_INIT;
			auxTimer = 0f;
			this.timeToStart = UnityEngine.Random.Range(minTimeToStart, maxTimeToStart);

			meshRendererShadow.enabled = true;

			slimeMovement.ResetInitialValues(true);

			if (SlimeManager.instance != null)
			{
				SlimeManager.instance.SlimeOnRunningState();
			}
		}

		public override void FixedUpdate(float fixedDeltaTime)
		{
			base.FixedUpdate(fixedDeltaTime);

			switch (animationState)
			{
				case AnimationState.JUMPING:
					slimeMovement._Update(fixedDeltaTime);
					break;
			}
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);

			switch (animationState)
			{
				case AnimationState.WAITING_FOR_RANDOM_INIT:
					UpdateWaitingForRandomInit(deltaTime);
					break;
				case AnimationState.WAITING_FOR_FIRST_JUMP:
					UpdateWaitingForFirstJump(deltaTime);
					break;
				case AnimationState.JUMPING:
					UpdateJumping(deltaTime);
					break;
			}
		}

		private void UpdateWaitingForRandomInit(float deltaTime)
		{
			if (auxTimer >= timeToStart)
			{
				animationState = AnimationState.WAITING_FOR_FIRST_JUMP;
				auxTimer = 0f;

				slimeCore.slimeAnimatorController.GoToJump(true);
			}

			auxTimer += deltaTime;
		}

		private void UpdateWaitingForFirstJump(float deltaTime)
		{
			if (auxTimer >= 0.75f)
			{
				animationState = AnimationState.JUMPING;
				auxTimer = 0f;
			}

			auxTimer += deltaTime;
		}

		private void UpdateJumping(float deltaTime)
		{
			//slimeMovement._Update(deltaTime);
			idleStateTimer.Update(deltaTime);
		}

		private void GoToIdle()
		{
			//CustomLogger.LogWarning(TAG, "GOING TO IDLE STATE!");
			slimeCore.GoToIdleState();
			idleStateTimer.StopTimer();
		}

		public override void Exit()
		{
			base.Exit();

			if (SlimeManager.instance != null)
			{
				SlimeManager.instance.SlimeOutOfRunningState();
			}
		}
	}
}