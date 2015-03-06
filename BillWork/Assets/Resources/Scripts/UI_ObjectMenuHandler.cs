using UnityEngine;
using System.Collections;

public enum OBJ_MENU_STATE{
	SELECTING,
	ROTATING,
	NONE
}

public class UI_ObjectMenuHandler:MonoBehaviour {
	public UTIL_Scripts scripts;
	public OBJ_MENU_STATE objMenuState;		//The current state of input related to the ObjectMenu.
	public GameObject root;					//The root of the GameObject

	void Start(){

	}

	void Update(){
		if(root!=null) root=scripts.ui_LayersMenuHandler.root;
		UpdateInput();
	}

	public void UpdateInput(){
		//Update the input based on the menu.
		if(objMenuState==OBJ_MENU_STATE.NONE) return;

		IN_FRAME frame=scripts.in_InputHandler.inFrame;
		Vector2 rotDir;			//Which way to rotate based on left-right or up-down.
		
		if(!frame.isInput) return;	//If any input is being done, continue.

		switch(objMenuState){
			case OBJ_MENU_STATE.ROTATING:
				//Rotate left-right
				rotDir=new Vector2(-1,1);
				if(frame.deltaPos.x<0) rotDir.x=1;
				root.transform.rotation=Quaternion.AngleAxis(Mathf.Abs(frame.deltaPos.x)*rotDir.x, Camera.main.transform.up)*root.transform.rotation;
					
				//Rotate up-down
				if(frame.deltaPos.y<0) rotDir.y=-1;
				root.transform.rotation=Quaternion.AngleAxis(Mathf.Abs(frame.deltaPos.y)*rotDir.y, Camera.main.transform.right)*root.transform.rotation;
				break;
			case OBJ_MENU_STATE.SELECTING:
				break;
		}
	}

	public void SetObjectMenuState(OBJ_MENU_STATE _objMenuState){
		//Sets the current state of the input related to the ObjectMenu.
		if(objMenuState==OBJ_MENU_STATE.SELECTING){	//If in selecting mode, deselect the object.
			scripts.in_InputHandler.DeselectObject();
		}

		objMenuState=_objMenuState;
	}
}
