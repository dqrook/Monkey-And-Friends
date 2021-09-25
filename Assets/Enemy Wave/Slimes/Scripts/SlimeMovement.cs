using System;
using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeMovement : MonoBehaviour
	{
		public enum AvoidingState
		{
			AVOIDING_OBSTACLE,
			FINISHED,
		}

		private const string TAG = "SlimeMovement";

		private float raycastTimer;
		private AnimationState animationState;


		public float gravity;

		public float raycastLength;
		public float groundRaycastLength;
		
		private RealTimeTimer timerAngularSpeed;
		public float timeToCheckRaycast;
		public float timeToCheckAngularSpeed;


		private SlimeController slimeController;
		private SlimeCore slimeCore;
		public Transform slimeTransform;
		public SlimePhysicsUtils slimePhysicsUtils;

		private float dirAvoidObstacle;
		private float dirAngularSpeed;
		private float currentAngularSpeed;

		private Vector3 lastUpVector;
		private float currentForwardSpeed;
		private Vector3 vecVelocity;
		private Vector3 vecGravity;

		private Action checkAngularVelocityAction;


		private AvoidingState avoidingState;


		public bool drawDebug;
		private float minAngularSpeed;
		private float maxAngularSpeed;

		private const float MAX_ANGULAR_SPEED_FACTOR = 4f;

		public float securityRaycastLength;
		public LayerMask securityRaycastLayerMask;
		public Transform[] securityRaycastOrigins;


		private bool nearbyWallDetected;


		public float maxDistance = 4f;
		public float minDistance = 2f;

		public void Init(SlimeController slimeController, SlimeCore slimeCore)
		{
			this.slimeController = slimeController;
			this.slimeCore = slimeCore;

			avoidingState = AvoidingState.FINISHED;
			checkAngularVelocityAction = CheckAngularVelocity;
			nearbyWallDetected = false;

			timerAngularSpeed = new RealTimeTimer();

			UpdateAngularSpeed();
		}

		public void ResetInitialValues(bool allowRotation)
		{
			vecVelocity = Vector3.zero;
			currentForwardSpeed = slimeController.forwardSpeed;

			dirAvoidObstacle = Neon2.Neon2Utils.GetRandomBool() ? 1f : -1f;
			vecGravity = Vector3.down * gravity;

			lastUpVector = slimeTransform.up;

			slimePhysicsUtils.CorrectFloorPosition(slimeTransform, slimeController.layerMaskCollisions);


			dirAngularSpeed = 1f;
			if (allowRotation)
			{
				currentAngularSpeed = minAngularSpeed;

				timerAngularSpeed.Setup(timeToCheckAngularSpeed, TimeUnit.SECONDS, false, checkAngularVelocityAction);
				timerAngularSpeed.RunTimer();
			}
			else
			{
				currentAngularSpeed = 0f;

				timerAngularSpeed.ResetTimer();
				timerAngularSpeed.StopTimer();
			}

			CheckFallingDownRaycast();
			CheckRaycast();

			raycastTimer = 0f;
		}

		public void _Update(float deltaTime)
		{
			Move(deltaTime);

			raycastTimer += deltaTime;
			if (raycastTimer >= timeToCheckRaycast)
			{
				CheckRaycast();
				CheckFallingDownRaycast();

				if (nearbyWallDetected)
				{
					CheckSecurityRaycasts();
				}

				raycastTimer = 0f;
			}

			timerAngularSpeed.Update(deltaTime);
		}

		public void Move(float deltaTime)
		{
			vecVelocity = currentForwardSpeed * slimeTransform.forward;

			Vector3 pos = slimeTransform.position;
			pos += vecVelocity * deltaTime;

			slimeTransform.position = pos;

			slimeTransform.Rotate(0f, currentAngularSpeed * dirAngularSpeed * deltaTime, 0f);
		}

		private void CheckRaycast()
		{
			RaycastHit hit;
			Ray ray = new Ray(slimeTransform.position, slimeTransform.forward);


			Color debugRayColor = Color.green;
			if (Physics.Raycast(ray, out hit, raycastLength, slimeController._layerMaskAvoidable))
			{
				if (IsAvoidableObstacle(hit))
				{
					avoidingState = AvoidingState.AVOIDING_OBSTACLE;

					if (!nearbyWallDetected)
					{
						if (hit.distance > maxDistance)
						{
							FarObstacleDetected(hit);
						}
						else if (hit.distance <= maxDistance && hit.distance > minDistance)
						{
							MiddleDistanceObstacleDetected(hit);
							debugRayColor = Color.blue;
						}
						else if (hit.distance <= minDistance)
						{
							NearObstacleDetected(hit);
							debugRayColor = Color.red;
						}
						else
						{
							FarObstacleDetected(hit);
						}
					}

					float tAngularSpeed = Mathf.InverseLerp(maxDistance, minDistance, hit.distance);
					currentAngularSpeed = Mathf.Lerp(minAngularSpeed, maxAngularSpeed, tAngularSpeed);
				}
			}
			else
			{
				currentForwardSpeed = slimeController.forwardSpeed;
				avoidingState = AvoidingState.FINISHED;
			}

			if (drawDebug)
			{
				Debug.DrawLine(slimeTransform.position, slimeTransform.position + ray.direction * raycastLength, debugRayColor, 2f);
			}
		}

		private void FarObstacleDetected(RaycastHit hit)
		{
			currentForwardSpeed = slimeController.forwardSpeed;

			timerAngularSpeed.ResetTimer();
			timerAngularSpeed.RunTimer();
		}

		private void MiddleDistanceObstacleDetected(RaycastHit hit)
		{
			currentForwardSpeed = slimeController.forwardSpeed;

			timerAngularSpeed.ResetTimer();
			timerAngularSpeed.RunTimer();
		}

		private void NearObstacleDetected(RaycastHit hit)
		{
			currentForwardSpeed = 0f;

			nearbyWallDetected = true;

			//if (hit.collider.CompareTag(slimeController.tagWall))
			//{
			//	nearbyWallDetected = true;
			//}
		}

		private void NearbyWallDetected()
		{

		}

		private void NormalObstacleDetected()
		{

		}

		private void CheckSecurityRaycasts()
		{
			bool wallDetected = false;
			for (int i = 0; i < securityRaycastOrigins.Length; i++)
			{
				RaycastHit hit;
				Ray ray = new Ray(securityRaycastOrigins[i].position, securityRaycastOrigins[i].forward);
				if (Physics.Raycast(ray, out hit, securityRaycastLength, securityRaycastLayerMask))
				{
					wallDetected = true;
					break;
				}
			}

			if (!wallDetected)
			{
				timerAngularSpeed.ResetTimer();
				timerAngularSpeed.RunTimer();
			}

			this.nearbyWallDetected = wallDetected;

			if (drawDebug)
			{
				for (int i = 0; i < securityRaycastOrigins.Length; i++)
				{
					Debug.DrawLine(securityRaycastOrigins[i].position,
						securityRaycastOrigins[i].position + securityRaycastOrigins[i].forward * securityRaycastLength,
						Color.yellow, 2f);
				}
			}
		}

		private void AvoidObstacle(Vector3 obstacleHitNormal)
		{
			Quaternion rotation = slimeTransform.rotation;


			slimeTransform.forward = obstacleHitNormal;
			
			//CustomLogger.LogWarning(TAG, "AVOIDING OBSTACLE!!!!");
		}

		private void CheckAngularVelocity()
		{
			if (avoidingState == AvoidingState.FINISHED)
			{
				bool changeAngularDir = Neon2.Neon2Utils.GetRandomBool();
				bool stopAngularSpeed = Neon2.Neon2Utils.GetRandomBool();

				currentAngularSpeed = stopAngularSpeed ? 0f : minAngularSpeed;
				dirAngularSpeed = changeAngularDir ? -dirAngularSpeed : dirAngularSpeed;

				timerAngularSpeed.ResetTimer();
				timerAngularSpeed.RunTimer();
			}
		}

		private void CheckFallingDownRaycast()
		{
			RaycastHit hit; 
			Ray ray = new Ray(slimeTransform.position, Vector3.down);


			if (!Physics.Raycast(ray, out hit, groundRaycastLength, slimeController.layerMaskCollisions))
			{
				slimeCore.GoToFallingDownState(currentForwardSpeed);
			}
			else
			{
				Quaternion rot = Quaternion.FromToRotation(slimeTransform.up, hit.normal.normalized);

				float angle = Vector3.Angle(slimeTransform.up, hit.normal.normalized);
				slimeTransform.rotation = rot * slimeTransform.rotation;

				if (slimeTransform.up != lastUpVector)
				{
					lastUpVector = slimeTransform.up;
					slimePhysicsUtils.CorrectFloorPosition(slimeTransform, slimeController.layerMaskCollisions);
				}
			}
		}

		public void GoBack()
		{
			slimeTransform.forward = -slimeTransform.forward;
		}

		public void StopAngularVelocity()
		{
			currentAngularSpeed = 0f;

			timerAngularSpeed.ResetTimer();
			timerAngularSpeed.StopTimer();
		}

		public void ResumeAngularVelocity()
		{
			currentAngularSpeed = 0f;

			timerAngularSpeed.ResetTimer();
			timerAngularSpeed.RunTimer();
		}


		public void UpdateForwardSpeed()
		{
			this.currentForwardSpeed = slimeController.forwardSpeed;
		}

		public void UpdateAngularSpeed()
		{
			this.minAngularSpeed = slimeController.angularSpeed;
			this.maxAngularSpeed = this.minAngularSpeed * MAX_ANGULAR_SPEED_FACTOR;
			this.currentAngularSpeed = minAngularSpeed;
		}

		private bool IsAvoidableObstacle(RaycastHit hit)
		{
			bool res = false;

			float angleCollision = Vector3.Angle(hit.normal, Vector3.up);
			if (angleCollision > slimeController.maxSlopeAngle)
			{
				res = true;
			}

			return res;
		}

		private void DrawDebugCollision(RaycastHit hit)
		{
			float angle = Vector3.Angle(hit.normal, Vector3.up);
			Debug.LogWarning("Angle: " + angle);
			Debug.DrawLine(hit.point, hit.point + hit.normal * 10f, Color.blue, 2f);
			Debug.DrawLine(hit.point, hit.point + Vector3.up * 10f, Color.green, 2f);
		}
	}
}