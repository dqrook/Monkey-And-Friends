using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suriyun
{
	
	public class CameraFollow : MonoBehaviour {

		public Transform target;
		public float camera_speed = 3.33f;
		protected Transform trans;
		protected Vector3 pos_offset;

		void Awake(){
			trans = GetComponent<Transform> ();
			target = GameObject.FindObjectOfType<ControllerYippy> ().transform;
			pos_offset = new Vector3 (1.65f, 1.26f, -1.28f);
		}

		void Update(){
			//trans.LookAt (target);
			if (target == null) {
				target = GameObject.FindObjectOfType<ControllerYippy> ().transform;
			}
			trans.position = Vector3.Lerp (trans.position, target.position + pos_offset,camera_speed*Time.deltaTime);
		}
	}

}
