using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimePhysicsUtils : ScriptableObject
	{
		public float groundRaycastLength;
		public float groundOffset;


		public void CorrectFloorRotation(Transform slimeTransform, Vector3 floorNormal)
		{
			Quaternion rot = Quaternion.FromToRotation(slimeTransform.up, floorNormal.normalized);
			slimeTransform.rotation = rot * slimeTransform.rotation;
		}

		public void CorrectFloorPosition(Transform slimeTransform, LayerMask groundLayerMask)
		{
			RaycastHit hit;
			Ray ray = new Ray(slimeTransform.localPosition, -slimeTransform.up);
			if (Physics.Raycast(ray, out hit, groundRaycastLength, groundLayerMask))
			{
				Vector3 correctedPos = hit.point;
				correctedPos += slimeTransform.up * groundOffset;
				slimeTransform.position = correctedPos;
			}
		}

		public void CorrectFloorPosition(Transform slimeTransform, RaycastHit hit)
		{
			Vector3 correctedPos = hit.point;
			correctedPos += slimeTransform.up * groundOffset;
			slimeTransform.position = correctedPos;
		}

		public void CorrectFloorRotation(Transform slimeTransform, RaycastHit hit)
		{
			Quaternion rot = Quaternion.FromToRotation(slimeTransform.up, hit.normal.normalized);
			float angle = Vector3.Angle(slimeTransform.up, hit.normal.normalized);
			slimeTransform.rotation = rot * slimeTransform.rotation;
		}
	}
}