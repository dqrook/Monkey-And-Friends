using UnityEngine;

[System.Serializable]
public class SlimeSpawner : MonoBehaviour
{
	public float radius;

	public Vector3 GetRandomPosition(Vector3 circleDir, Vector3 circleNormal)
	{
		float rndAngle = Random.Range(0f, 360f);
		Vector3 dir = Quaternion.AngleAxis(rndAngle, circleNormal) * circleDir;

		float rndRadius = Random.Range(0, radius);

		Vector3 res = transform.position + (dir * rndRadius);

		return res;
	}

	public Vector3 GetRandomDir(Vector3 circleDir, Vector3 circleNormal)
	{
		float rndAngle = Random.Range(0f, 360f);
		Vector3 res = Quaternion.AngleAxis(rndAngle, circleNormal) * circleDir;
		res = res.normalized;

		return res;
	}
}