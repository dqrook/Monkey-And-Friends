using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suriyun;

public class TextFPS : MonoBehaviour {

	public UnityEngine.UI.Text text;

	void Update(){
		text.text = "FPS : "+(int)(1f / Time.deltaTime);
	}
}
