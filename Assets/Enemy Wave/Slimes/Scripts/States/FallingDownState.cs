using UnityEngine;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public class FallingDownState : AbstractSlimeState
	{
		private const float MAX_VERTICAL_SPEED = 20f;
		private const float MAX_RAYCAST_LENGTH_RATIO = 4f;

		public float gravity;
		public float groundRaycastLength;
		public float floorOffset;

		public MeshRenderer meshRendererShadow;

		private float forwardSpeed;
		private Vector3 vecGravity;
		private Vector3 vecForwardVelocity;
		private Vector3 vecTotalVelocity;

		private float normalizedVerticalSpeed;

		private const string TAG = "SlimeController->FallingDownState";

		public void SetValues(float forwardSpeed)
		{
			this.forwardSpeed = forwardSpeed;
		}

		public override void Enter()
		{
			slimeCore.slimeAnimatorController.GoToFalling();

			vecGravity = Vector3.down * gravity;
			vecForwardVelocity = slimeTransform.forward * forwardSpeed;

			vecTotalVelocity = vecForwardVelocity;

			meshRendererShadow.enabled = false;

			//slimeTransform.up = Vector3.up;

			//CustomLogger.Log(TAG, "Enter!");
		}

		public override void FixedUpdate(float fixedDeltaTime)
		{
			Move(fixedDeltaTime);
			CheckGround();
		}

		//public override void Update(float deltaTime)
		//{
		//	Move(deltaTime);
		//	CheckGround();
		//}

		private void Move(float deltaTime)
		{
			Vector3 newPos = slimeTransform.position;
			newPos += vecTotalVelocity * deltaTime;

			slimeTransform.position = newPos;

			vecTotalVelocity += vecGravity * deltaTime;
			vecTotalVelocity.y = Mathf.Clamp(vecTotalVelocity.y, -MAX_VERTICAL_SPEED, 0f);
			normalizedVerticalSpeed = Mathf.InverseLerp(0f, -MAX_VERTICAL_SPEED, vecTotalVelocity.y);
			//Debug.Log("TOTAL VELOCITY: " + vecTotalVelocity.magnitude);
		}

		private void CheckGround()
		{
			RaycastHit hit;
			Ray ray = new Ray(slimeTransform.position, Vector3.down);



			float _groundRaycastLength = Mathf.Lerp(groundRaycastLength, groundRaycastLength * MAX_RAYCAST_LENGTH_RATIO, normalizedVerticalSpeed);
			if (Physics.Raycast(ray, out hit, _groundRaycastLength, slimeController.layerMaskCollisions))
			{
				//Vector3 floorPos = hit.point + slimeTransform.up * floorOffset;
				//slimeCore.transform.position = floorPos;


				slimePhysicsUtils.CorrectFloorPosition(slimeCore.transform, hit);
				slimePhysicsUtils.CorrectFloorRotation(slimeCore.transform, hit);
				slimeCore.GoToJumpState();
			}
		}
	}
}