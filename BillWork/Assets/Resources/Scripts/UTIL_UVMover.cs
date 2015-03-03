using UnityEngine;
using System.Collections;

/// <summary>
/// Used to move the UVs of a material attached to the GameObject this is attached to.
/// </summary>

public class UTIL_UVMover:MonoBehaviour{
	public bool on;					//On or not.
	public Vector2 direction;		//Direction to move the UVs.
	public float speed;				//How fast to move the UVs.
	public int matIndex;			//The index on the model of which material to move the UVs on.

	private Vector2 curUVs;			//Current UVs.

	void Start(){
		on=true;
		direction=new Vector2(0.5f,0.2f);
		speed=0.004f;
	}

	void Update(){
		if(on){
			if(matIndex>=renderer.materials.Length){	//Trying to move the UVs on a material that doesn't exist.
				Debug.Log("Trying to move UVs on \""+gameObject.name+"\" when it doesn't have "+(matIndex+1)+" materials.");
				return;	
			}
						
			curUVs=renderer.materials[matIndex].GetTextureOffset("_MainTex");								//Current UVs
			renderer.materials[matIndex].SetTextureOffset("_MainTex",curUVs+direction.normalized*speed);	//New UVs
		}
	}
}
