using UnityEngine;
using System.Collections;

/// <summary>
/// Manages tracking what action touching the game world performs as well as performing that action.
/// </summary>

public enum IN_STATE{	//The different states of the input.
	SELECTING,		//Touching an object to select it.
	CAMERA,			//Touching the screen to control the camera.
	NONE
};

public class IN_InputHandler:MonoBehaviour{
	public IN_STATE inState;			//The current state of the input. Denotes what touching the game world performs.
	public GameObject selectedObject;	//The currently selected object.
	public UTIL_Scripts scripts;

	void Start(){
		inState=IN_STATE.NONE;
		selectedObject=null;
	}

	void Update(){
		CheckTouch();
	}

	private void CheckTouch(){
		//Check any new touches or mouse clicks.
		if(Input.GetMouseButtonDown(0)){	//Left click.
			switch(inState){
				case IN_STATE.SELECTING:	//Trying to select an object.
					RaycastHit rayHit;
					Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
					if(Physics.Raycast(ray,out rayHit, Mathf.Infinity)){
						GameObject _selectedObject=GetToggleableObject(rayHit.transform.gameObject);
						if(_selectedObject!=null){	//Successfully clicked on a piece of the model.
							if(selectedObject!=null) UTIL_Utilities.RemoveHighlight(selectedObject, true);
							selectedObject=_selectedObject;
							UTIL_Utilities.AddHighlight(selectedObject);
							scripts.ui_ObjectMenuHandler.SelectObject(selectedObject);
						}
					}
					break;
			}
		}
	}

	public void SetInState(IN_STATE _inState){
		//Changes the current state of the input.
		inState=_inState;
	}
	public void DeselectObject(){
		//Deselects the current object.
		if(selectedObject!=null) UTIL_Utilities.RemoveHighlight(selectedObject, true);
		selectedObject=null;
		scripts.ui_ObjectMenuHandler.DeselectObject();
	}

	private GameObject GetToggleableObject(GameObject _object){
		//Finds whether or not _object is a child of the main model. If it's a child's child, it finds the main parent.
		//Returns the ancestor object of the inputed _object that is a child of the main model.
		if(_object.transform.parent==scripts.ui_LayersMenuHandler.root.transform) return _object;	//The object is already a first child of the root of the model.
		
		//See if the parent of _object, or that parent's parent's, are in the UI_LayersMenuhandler.toggleableParts.
		bool keepLooking=true;
		GameObject tmpParent=_object.transform.parent.gameObject;
		while(keepLooking){
			for(int i=0; i<scripts.ui_LayersMenuHandler.toggleableParts.Count; i++){
				if(scripts.ui_LayersMenuHandler.toggleableParts[i]==tmpParent){
					keepLooking=false;
					return scripts.ui_LayersMenuHandler.toggleableParts[i];
				}
			}
			if(tmpParent.transform.parent!=null) tmpParent=tmpParent.transform.parent.gameObject;	//Move to the next parent level up.
			else{
				keepLooking=false;
				Debug.Log("\""+_object.name+"\" is not part of the main model hierarchy.");
				return null;
			}
		}

		return null;
	}
}
