using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles controlling cameras. Either attach this to a camera or attach cameras to this.
/// </summary>

public enum CAM_MODE{			//The control mode of the camera.
	ARCBALL,
	NONE
};
public enum CAM_INPUT_STATE{	//Which input state the camera is in. Determines what touch controls do.
	TRANSLATE,
	ROTATE,
	ROLL,
	ZOOM,
	NONE
};

public class CAM_Camera:MonoBehaviour{
	public CAM_MODE camMode;				//Which control mode the camera is in.
	public CAM_INPUT_STATE camInputState;	//Which input state the camera is in.
	public Vector3 eye;						//The eye of the camera.
	public Vector3 target;					//What the camera is looking at.
	public Vector3 dir;						//Direction vector of the camera.
	public float distance;					//Distance from the eye to the target. Zoom level.
	public Quaternion rot;					//Camera's rotation.
	public List<GameObject> cameras;		//The cameras being controlled. If null after the first frame, use THIS.
	public UTIL_Scripts scripts;

	private bool isTouch;					//Whether touch controls are being used.

	void Start(){
		camMode=CAM_MODE.ARCBALL;
		camInputState=CAM_INPUT_STATE.NONE;

		rot=Quaternion.FromToRotation(new Vector3(0,0,1),dir);

		if(cameras==null){	//Add THIS as the main camera if none are already added.
			cameras=new List<GameObject>();
			cameras.Add(this.gameObject);
		}

		isTouch=false;
	}

	void Update(){
		if(Input.touchCount>0) isTouch=true;

		if(cameras.Count>0){
			UpdateInput();
			UpdateCamMode();
		}
	}

	private void UpdateInput(){
		Vector2 deltaPos;
		Vector2 rotDir;
		if(isTouch) deltaPos=Input.GetTouch(0).deltaPosition;
		else deltaPos=new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
		
		if((!Input.GetMouseButton(0))&&(!Input.GetMouseButton(1))&&(Input.touchCount==0)) return;	//If any input is being done, continue.

		if(camMode==CAM_MODE.ARCBALL){
			switch(camInputState){
				case CAM_INPUT_STATE.TRANSLATE:	//1 finger to translate along camera plane. 2 will roll and zoom. Left mouse to translate, right mouse to zoom.
					//Translate
					if((Input.GetMouseButton(0))||(Input.touchCount>0)){
						target+=cameras[0].transform.right*deltaPos.x;
						target+=cameras[0].transform.up*deltaPos.y;
					}
					break;
				case CAM_INPUT_STATE.ROTATE:	//1 finger to rotate. Left mouse to rotate.
					//Rotate left-right
					rotDir=new Vector2(1,-1);
					if(deltaPos.x<0) rotDir.x=-1;
					rot=Quaternion.AngleAxis(Mathf.Abs(deltaPos.x)*rotDir.x, cameras[0].transform.up)*rot;
					
					//Rotate up-down
					if(deltaPos.y<0) rotDir.y=1;
					rot=Quaternion.AngleAxis(Mathf.Abs(deltaPos.y)*rotDir.y, cameras[0].transform.right)*rot;

					dir=rot*new Vector3(0,0,1);
					break;
				case CAM_INPUT_STATE.ROLL:		//1 finger to roll. Left mouse to roll.
					rotDir.x=1;			//Direction of roll modifier.
					if(deltaPos.x<0) rotDir.x=-1;

					rot=Quaternion.AngleAxis(Mathf.Abs(deltaPos.x)*rotDir.x,dir)*rot;
					break;
				case CAM_INPUT_STATE.ZOOM:		//1 finger to zoom. Left mouse to zoom.
					distance+=deltaPos.y;
					if(distance<0.01f) distance=0.01f;
					for(int i=0; i<cameras.Count; i++)
						if(cameras[i].GetComponent<Camera>().isOrthoGraphic) cameras[i].GetComponent<Camera>().orthographicSize=distance*0.6f;
					break;
			}
		}
	}
	private void UpdateCamMode(){
		//Update the camera transform based on the camMode.
		switch(camMode){
			case CAM_MODE.ARCBALL:
				eye=target-dir*distance;
				
				for(int i=0; i<cameras.Count; i++){
					cameras[i].transform.localPosition=eye;
					cameras[i].transform.localRotation=rot;
				}
				break;
		}
	}

	public void SetCamInputState(CAM_INPUT_STATE _state){
		//Switch the camera input state.
		camInputState=_state;
	}
	public void Focus(){
		//Center the camera on the currently selected part of the model, if any.
		GameObject selected=scripts.in_InputHandler.selectedObject;
		if(selected)
			if(camMode==CAM_MODE.ARCBALL)
				target=selected.transform.position;
	}
}
