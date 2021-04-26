using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suriyun;

public class TextAttackType : MonoBehaviour {

	protected Suriyun.Controller controller;
	public UnityEngine.UI.Text text;

	void Update(){
		if (controller == null) {
			controller = GameObject.FindObjectOfType<Suriyun.Controller> ();
		}
		text.text = controller.GetAttackType ("Attack Type : ");
	}
}
