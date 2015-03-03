using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handles the Object Menu, which shows additional information about the currently selected object.
/// </summary>

public class UI_ObjectMenuHandler:MonoBehaviour{
	public UTIL_Scripts scripts;

	void Start() {

	}

	void Update() {

	}

	public void SelectObject(GameObject _object){
		//An object is selected. Show the menu.
		scripts.uiHandler.SelectObject(_object);	//Show the menu.
		scripts.uiHandler.ObjectMenu.transform.FindChild("TEXT_Name").gameObject.GetComponent<Text>().text=_object.name;
		scripts.uiHandler.ObjectMenu.transform.FindChild("TEXT_Position").gameObject.GetComponent<Text>().text="Position: "+_object.transform.localPosition;
	}
	public void DeselectObject(){
		//Object has been selected to nothing, reset the menu.
		Reset();
	}

	public void Button_Toggle(){
		//Toggles the visibility of the currently selected object.
		scripts.ui_LayersMenuHandler.ToggleObject(scripts.in_InputHandler.selectedObject);
	}
	public void Button_Focus(){
		//Focus button hit. Center the camera on the currently selected object.
		scripts.cam_Camera.Focus();
	}

	private void Reset(){
		//Reset the menu to defaults.
		scripts.uiHandler.ObjectMenu.transform.FindChild("TEXT_Name").gameObject.GetComponent<Text>().text="NAME";
		scripts.uiHandler.ObjectMenu.transform.FindChild("TEXT_Position").gameObject.GetComponent<Text>().text="Position: ";
	}
}
