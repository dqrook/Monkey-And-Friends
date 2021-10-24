using UnityEngine;
using System.Collections;

public class SceneScript : MonoBehaviour 
{
	public Transform  Cam ; // rendering main camera
	public Material SkinMaterial ; // ballerina trnder material
	public Texture2D[] ModelSkin; //Extra Textures
	int TextureIndex = 0; // array of Extra Textures

	Transform Ballerina ; //
	Vector3 StarPos ; // scene Start location data_if too faraway from startposition, will walk back. 
	float Dist ; // distance data form scene start position and camera position.
	Animator  Ani ; // animation controler of ballerina
	AnimatorStateInfo stateInfo ; // current animation data of ballerina
	float Interval ; //after dance motion how long will ballerina will walk around place
	float MotionIndex; // motioon bender control index
	Vector3 Dir ; // direction data to start position or camera position


	// Use this for initialization
	void Start () 
	{
		Ballerina = transform; // set ballerina character 
		StarPos = Ballerina.position; // Store character position
		Ani = Ballerina.GetComponent<Animator>();
		MotionIndex = 0; // motion sequence index
		Interval = Time.time + Random.Range(0.3f,1f); // how long the character walk around, in tjis case random 0.3-1 second.
		SkinMaterial.mainTexture = ModelSkin[0]; // material main texture 

		if (!Cam) // if Main rerder Cam not assigned, this will find mainRenderCamera before scene Start. 
		{
			Cam = Camera.main.transform;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		Dist = Vector3.Distance(Ballerina.position, StarPos); // distance between ballerina and start pos
		Cam.LookAt(Ballerina); // camera will alway look at ballerina
		
		stateInfo = Ani.GetCurrentAnimatorStateInfo(0); // getting the ballerina current animation state 
		
		if(Dist < 5 && !Ani.GetBool("Return")) // within 5m away from start pos,  ballerina will dance around
		{
			
			if(stateInfo.IsName("Base Layer.Walk"))
			{
				Dir = Cam.position-Ballerina.position; // gettiing ballerina rptation toward camera
				Dir.y = 0;
				Dir.Normalize();
				
				Ballerina.forward = Vector3.Lerp(Ballerina.forward,Dir,Time.deltaTime); // ballerina will ratate toward camera
				
				if(Interval < Time.time) // if walking random interval ends 
				{
					Interval = Time.time + Random.Range(0.3f,1f); // recalculate random walking interval 
					
					if(MotionIndex == 0)  // circle through dance motions in this case 1 and 2 
					{
						MotionIndex = 1;
					}else
					{
						MotionIndex = 0;
					}
					Ani.SetFloat("MotionIndex",MotionIndex); // now set the motion braning id 
					
					Ani.SetBool("Dance",true); // now start the ballet dance 
				}
			}
			
			if(stateInfo.IsName("Base Layer.Dance"))
			{
				Ani.SetBool("Dance",false); // will set the "Dance" parameter false so that motion flollow to walking
			}

		}else // more than 5m away from start pos,  ballerina will go back to sart pos
		{
			if(!Ani.GetBool("Return")) // will double check ballerina going to start pos or doing something else
			{
				Ani.SetBool("Return",true); // going back to srat position 
			}
			
			if(stateInfo.IsName("Base Layer.Jump"))
			{
				Dir = StarPos-Ballerina.position; // ballerina rotation toward start pos 
				Dir.y = 0;
				Dir.Normalize();
				
				Ballerina.forward = Vector3.Lerp(Ballerina.forward,Dir,Time.deltaTime*3); // ballerina will rotatoe toward start position 
				
				if(Dist > 1 && Ani.GetBool("Return")) // if ballerina almost walked to start position 
				{
					Ani.SetBool("Return",false); // ballerina will dance and walking to camera
					Interval = Time.time + Random.Range(1f,8f); // set new walking around interval time
				}
			}
		}
	}

	void ChangeTexture () // button click call function to change Main 2D texture of ballerina material.
	{
		TextureIndex++;
			
		if (ModelSkin.Length-1< TextureIndex) // check if txture array length and never go beyond the list of Texture.  
		{
			TextureIndex = 0;
		}
		
		SkinMaterial.mainTexture = ModelSkin[TextureIndex]; // Applying texture to the material.
	}

	void ResetTime() //At the end of Motion_1 ,Motion_2 character animation call this fanction to how long the character walk around.
	{
		Interval = Time.time + Random.Range(3f,8f);
	}
}
