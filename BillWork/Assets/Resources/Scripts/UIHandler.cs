using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the main UI.
/// </summary>

public class UIHandler:MonoBehaviour{
	//Canvas Groups
	public GameObject MenuToolbar;
	public GameObject MainMenu;		//Shows the main menu options such as Reset.
	public GameObject DebugMenu;	//Shows debug info such as FPS.
	public GameObject CameraMenu;	//Shows camera controls.
	public GameObject LayersMenu;	//Shows a list of objects to show and hide.
	public GameObject ObjectMenu;	//Shows information about the selected object.
	
	public UTIL_Scripts scripts;

	void Start(){
	}

	void Update(){
	}

	public void SelectObject(GameObject _object){
		//An object has been selected. Show the ObjectMenu.
		ShowHideCanvasGroup(ObjectMenu, true, true, true);
	}

	private void ShowHideCanvasGroup(GameObject _canvasGroup, bool _show, bool _interactable, bool _blocksRaycasts){
		//Show or hide a CanvasGroup as well as set it's interactability.
		CanvasGroup tmp=_canvasGroup.GetComponent<CanvasGroup>();
		if(tmp==null){
			Debug.Log("Can't call ShowHideCanvasGroup on \""+_canvasGroup.name+"\" as no CanvasGroup is attached.");
			return;
		}

		if(_show) tmp.alpha=1;
		else tmp.alpha=0;

		tmp.interactable=_interactable;
		tmp.blocksRaycasts=_blocksRaycasts;
	}
	private void HideGroups(int _group){
		//Hides a group of menus.
		if(_group==1){	//MainMenu, CameraMenu, LayersMenu
			ShowHideCanvasGroup(MainMenu,false,false,false);
			ShowHideCanvasGroup(CameraMenu,false,false,false);
			ShowHideCanvasGroup(LayersMenu,false,false,false);
		}
	}

	// BUTTONS/TOGGLES	////////////////////////
	//MenuToolbar
	public void Button_Menu(){
		//Menu button hit.
		if(MainMenu.GetComponent<CanvasGroup>().alpha==1)
			ShowHideCanvasGroup(MainMenu,false,false,false);
		else{
			scripts.in_InputHandler.SetInState(IN_STATE.UI);
			HideGroups(1);
			ShowHideCanvasGroup(MainMenu, true, true, true);
		}
	}
	public void Button_Camera(){
		//Camera button hit.
		if(CameraMenu.GetComponent<CanvasGroup>().alpha==1){
			scripts.in_InputHandler.SetInState(IN_STATE.UI);
			ShowHideCanvasGroup(CameraMenu,false,false,false);
		}
		else{
			scripts.in_InputHandler.SetInState(IN_STATE.UI);
			HideGroups(1);
			ShowHideCanvasGroup(CameraMenu, true, true, true);
		}
		scripts.in_InputHandler.SetInState(IN_STATE.CAMERA);	//Set the input state to CAMERA.
	}
	public void Button_Select(){
		//Select button hit.
		//If not in selecting mode, go into selecting mode.
		if(scripts.in_InputHandler.inState!=IN_STATE.SELECTING) scripts.in_InputHandler.SetInState(IN_STATE.SELECTING);
		else{	//If already in selecting mode, get out of it.
			scripts.in_InputHandler.SetInState(IN_STATE.NONE);
			scripts.in_InputHandler.DeselectObject();
			ShowHideCanvasGroup(ObjectMenu, false, false, false);
		}
	}
	public void Button_Layers(){
		//Layers button hit.
		if(LayersMenu.GetComponent<CanvasGroup>().alpha==1)
			ShowHideCanvasGroup(LayersMenu,false,false,false);
		else{
			scripts.in_InputHandler.SetInState(IN_STATE.UI);
			HideGroups(1);
			ShowHideCanvasGroup(LayersMenu, true, true, true);
		}
	}
	public void Button_DebugMenu(){
		//DebugMenu button hit.
		if(DebugMenu.GetComponent<CanvasGroup>().alpha==1)
			ShowHideCanvasGroup(DebugMenu,false,false,false);
		else ShowHideCanvasGroup(DebugMenu, true, false, false);
	}
	//MainMenu
	public void Button_Reset(){
		//Reset button hit. Reload the level.
		Application.LoadLevel(Application.loadedLevel);
	}
	//CameraMenu
	public void Button_Translate(){
		//Translate button hit. Set camera to translate mode.
		scripts.cam_Camera.SetCamInputState(CAM_INPUT_STATE.TRANSLATE);
	}
	public void Button_Rotate(){
		//Rotate button hit. Set camera to rotate mode.
		scripts.cam_Camera.SetCamInputState(CAM_INPUT_STATE.ROTATE);
	}
	public void Button_Roll(){
		//Roll button hit. Set camera to roll mode.
		scripts.cam_Camera.SetCamInputState(CAM_INPUT_STATE.ROLL);
	}
	public void Button_Zoom(){
		//Zoom button hit. Set zoom to roll mode.
		scripts.cam_Camera.SetCamInputState(CAM_INPUT_STATE.ZOOM);
	}
	public void Button_Focus(){
		//Focus button hit. Center the camera on the currently selected object.
		scripts.cam_Camera.Focus();
	}
}
