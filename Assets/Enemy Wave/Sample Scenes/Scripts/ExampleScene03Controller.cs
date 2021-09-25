using UnityEngine;

public class ExampleScene03Controller : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			Time.timeScale = 0f;
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			Time.timeScale = 1f;
		}
	}
}