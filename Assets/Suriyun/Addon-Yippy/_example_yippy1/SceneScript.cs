using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Suriyun;


public class SceneScript : MonoBehaviour {

	protected ControllerYippy yippy;

	void Awake(){
		Application.targetFrameRate = 120;
	}
		

	public void Reset(){
		CheckYippy ();
		yippy.gameObject.transform.position = new Vector3 (-1.12f,0.89f, 0f);
	}

	public virtual void ClearState(){
		CheckYippy ();
		yippy.ClearState ();
	}

	public virtual void Sit(){
		CheckYippy ();
		yippy.Sit ();
	}

	public virtual void Die(){
		CheckYippy ();
		yippy.Die ();
	}

	public virtual void Cheer(){
		CheckYippy ();
		yippy.Cheer ();
	}

	public virtual void Hit(){
		CheckYippy ();
		yippy.Hit ();
	}

	public virtual void SwitchAttackType(){
		CheckYippy ();
		yippy.SwitchAttackType ();
	}

	public void CheckYippy(){
		if (yippy == null) {
			yippy = GameObject.FindObjectOfType<ControllerYippy> ();
		}
	}

}
