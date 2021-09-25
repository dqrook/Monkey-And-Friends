using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeController : MonoBehaviour
	{
		[SerializeField] private SlimeCore slimeCore;
		[HideInInspector] public LayerMask _layerMaskAvoidable;

		[Header("General")]
		public bool initOnAwake;
		public SlimeAudioController slimeAudioController;
		[SerializeField] public LayerMask layerMaskCollisions;
		[SerializeField] private LayerMask layerSlimes;
		//public string tagWall;
		public bool avoidSlimes;

		[Header("Movement")]
		public float forwardSpeed;
		public float angularSpeed;
		[Tooltip("Maximum slope that can be climbed")]public float maxSlopeAngle = 70f;

		[Header("Idle State Configuration")]
		public float minIdleTime;
		public float maxIdleTime;
		public bool idleStateEnabled;

		[Header("Jump State Configuration")]
		public float minJumpTime;
		public float maxJumpTime;
		public bool jumpStateEnabled;
		public AudioClip[] clipsJump;

		private bool intialized;

		private void Reset()
		{
			slimeCore = GetComponent<SlimeCore>();

			this.forwardSpeed = 1.5f;

			this.minIdleTime = 2f;
			this.maxIdleTime = 4f;
			this.idleStateEnabled = true;

			this.minJumpTime = 2f;
			this.maxJumpTime = 4f;
			this.jumpStateEnabled = true;

			this.maxSlopeAngle = 70f;

			this._layerMaskAvoidable = layerMaskCollisions;
		}

		public void Init()
		{
			slimeCore.Init(this);

			this._layerMaskAvoidable = layerMaskCollisions;
			if (avoidSlimes)
			{
				this._layerMaskAvoidable = this._layerMaskAvoidable | layerSlimes;
			}

			intialized = true;
		}

		private void Awake()
		{
			if (initOnAwake)
			{
				Init();
				GoToJumpState();
			}
		}


		private void FixedUpdate()
		{
			if (intialized)
			{
				float fixedDeltaTime = Time.fixedDeltaTime;

				slimeCore._FixedUpdate(fixedDeltaTime);
			}
		}

		private void Update()
		{
			if (intialized)
			{
				float deltaTime = Time.deltaTime;
				
				slimeCore._Update(deltaTime);
			}
		}

		private void LateUpdate()
		{
			if (intialized)
			{
				float deltaTime = Time.deltaTime;
				slimeCore._LateUpdate(deltaTime);
			}
		}

		public void GoToIdleState()
		{
			if (idleStateEnabled)
			{
				slimeCore.GoToIdleState();
			}
		}

		public void GoToJumpState()
		{
			if (jumpStateEnabled)
			{
				slimeCore.GoToJumpState();
			}
		}

		public void SetForwardSpeed(float newForwardSpeed)
		{
			this.forwardSpeed = newForwardSpeed;
			slimeCore.UpdateForwardSpeed();
		}

		public void SetAngularSpeed(float newAngularSpeed)
		{
			this.angularSpeed = newAngularSpeed;
			slimeCore.UpdateAngularSpeed();
		}

		public void SetSlimeVisual(SlimeVisualConfig slimeVisualConfig)
		{
			slimeCore.SetSlimeVisual(slimeVisualConfig);
		}
	}
}