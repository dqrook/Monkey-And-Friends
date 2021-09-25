using UnityEngine;


namespace Neon2.SlimeSystem
{
	public class SlimeCore : MonoBehaviour
	{
		public enum State
		{
			DISABLED,
			ENABLED,
		}

		private const string TAG = "SlimeController";


		public FSM fsm;
		public SlimeVisual slimeVisual;
		public float minimumY;
		public SlimeAudioController slimeAudioController;
		public Transform _transform { get; set; }
		public SlimeAnimatorController slimeAnimatorController;
		public SlimeMovement slimeMovement;


		[Header("States")]
		public JumpState jumpState;
		public IdleState idleState;
		public FallingDownState fallingDownState;
		public DisabledState disabledState;


		private SlimeController slimeController;

		public void Init(SlimeController slimeController)
		{
			this.slimeController = slimeController;

			slimeMovement.Init(slimeController, this);

			jumpState.Init(slimeController, this);
			idleState.Init(slimeController, this);
			fallingDownState.Init(slimeController, this);
			disabledState.Init(slimeController, this);

			fsm = new FSM();

			slimeAnimatorController.Init(slimeController.slimeAudioController);

			this.slimeVisual.Init();
			this.slimeVisual.UpdatePropertyBlock();



			slimeAudioController.Init(slimeController.clipsJump);


			_transform = transform;

			GoToDisabledState();
		}

		public void SetSlimeVisual(SlimeVisualConfig slimeVisualConfig)
		{
			this.slimeVisual.SetupVisual(slimeVisualConfig);
		}

		public void Spawn(SlimeVisualConfig slimeVisualConfig, bool randomInitialState)
		{
			this.slimeVisual.SetupVisual(slimeVisualConfig);
		}

		public void UpdateForwardSpeed()
		{
			slimeMovement.UpdateForwardSpeed();
		}

		public void UpdateAngularSpeed()
		{
			slimeMovement.UpdateAngularSpeed();
		}

		public void SetLocalPosition(Vector3 localPosition)
		{
			_transform.localPosition = localPosition;
		}

		public void SetWorldPosition(Vector3 worldPosition)
		{
			_transform.position = worldPosition;
		}

		public void ResetValues()
		{
		}

		public void _FixedUpdate(float fixedDeltaTime)
		{
			fsm.FixedUpdateFSM(fixedDeltaTime);
		}

		public void _Update(float deltaTime)
		{
			fsm.UpdateFSM(deltaTime);
		}

		public void _LateUpdate(float deltaTime)
		{
			fsm.LateUpdateFSM(deltaTime);
		}

		#region STATES
		public void GoToIdleState()
		{
			if (slimeController.idleStateEnabled)
			{
				fsm.ChangeState(idleState);
			}
		}

		public void GoToJumpState()
		{
			if (SlimeManager.instance != null)
			{
				if (SlimeManager.instance.CanPassToRunningState() && slimeController.jumpStateEnabled)
				{
					fsm.ChangeState(jumpState);
				}
				else
				{
					GoToIdleState();
				}
			}
			else
			{
				fsm.ChangeState(jumpState);
			}
		}

		public void GoToDisabledState()
		{
			fsm.ChangeState(disabledState);
		}

		public void RandomInitState()
		{
			int randomInt = UnityEngine.Random.Range(0, 2);
			if (randomInt == 0)
			{
				GoToIdleState();
			}
			else
			{
				GoToJumpState();
			}
		}

		public void GoToFallingDownState(float forwardSpeed)
		{
			fallingDownState.SetValues(forwardSpeed);
			fsm.ChangeState(fallingDownState);
		}

		private AbstractSlimeState GetCurrentSlimeState()
		{
			AbstractSlimeState res = (AbstractSlimeState)fsm.currentState;
			return res;
		}
		#endregion


		public void SlimeVisible()
		{
			enabled = true;
			slimeAnimatorController.enabled = true;
		}

		public void SlimeInvisible()
		{
			enabled = false;
			slimeAnimatorController.enabled = false;
		}
	}
}