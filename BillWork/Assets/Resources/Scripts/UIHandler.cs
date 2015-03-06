using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Handles the main UI.
/// </summary>

public class UIHandler:MonoBehaviour{
	//Canvas Groups
	public GameObject MenuToolbar;
	public GameObject MainMenu;			//Shows the main menu options such as Reset.
	public GameObject DebugMenu;		//Shows debug info such as FPS.
	public GameObject CameraMenu;		//Shows camera controls.
	public GameObject ObjectMenu;		//Shows a list of controls for manipulating the object.
	public GameObject LayersMenu;		//Shows a list of objects to show and hide.
	public GameObject ObjectInfoMenu;	//Shows information about the selected object.
	
	public UTIL_Scripts scripts;

	void Start(){
	}

	void Update(){
	}

	public void SelectObject(GameObject _object){
		//An object has been selected. Show the ObjectMenu.
		UTIL_Utilities.ShowHideCanvasGroup(ObjectInfoMenu, true, true, true);
	}

	private void HideGroups(int _group){
		//Hides a group of menus.
		if(_group==1){	//MainMenu, CameraMenu, LayersMenu
			UTIL_Utilities.ShowHideCanvasGroup(MainMenu,false,false,false);
			UTIL_Utilities.ShowHideCanvasGroup(CameraMenu,false,false,false);
			UTIL_Utilities.ShowHideCanvasGroup(ObjectMenu,false,false,false);
			UTIL_Utilities.ShowHideCanvasGroup(LayersMenu,false,false,false);
		}
	}

	// BUTTONS/TOGGLES	////////////////////////
	//MenuToolbar
	public void Button_Menu(){
		//Menu button hit.
		if(MainMenu.GetComponent<CanvasGroup>().alpha==1)
			UTIL_Utilities.ShowHideCanvasGroup(MainMenu,false,false,false);
		else{
			scripts.in_InputHandler.SetInState(IN_STATE.UI);
			HideGroups(1);
			UTIL_Utilities.ShowHideCanvasGroup(MainMenu, true, true, true);
		}
	}
	public void Button_Camera(){
		//Camera button hit.
		if(CameraMenu.GetComponent<CanvasGroup>().alpha==1){
			scripts.in_InputHandler.SetInState(IN_STATE.UI);
			UTIL_Utilities.ShowHideCanvasGroup(CameraMenu,false,false,false);
		}
		else{
			scripts.in_InputHandler.SetInState(IN_STATE.CAMERA);
			HideGroups(1);
			UTIL_Utilities.ShowHideCanvasGroup(CameraMenu, true, true, true);
		}
		//scripts.in_InputHandler.SetInState(IN_STATE.NONE);	//Set the input state to NONE.
	}
	public void Button_Object(){
		//Object menu button hit.
		if(ObjectMenu.GetComponent<CanvasGroup>().alpha==1){
			scripts.in_InputHandler.SetInState(IN_STATE.UI);
			UTIL_Utilities.ShowHideCanvasGroup(ObjectMenu,false,false,false);
		}
		else{
			scripts.in_InputHandler.SetInState(IN_STATE.OBJECT);
			HideGroups(1);
			UTIL_Utilities.ShowHideCanvasGroup(ObjectMenu, true, true, true);
		}
		//scripts.in_InputHandler.SetInState(IN_STATE.NONE);	//Set the input state to NONE.
	}
	public void Button_Layers(){
		//Layers button hit.
		if(LayersMenu.GetComponent<CanvasGroup>().alpha==1)
			UTIL_Utilities.ShowHideCanvasGroup(LayersMenu,false,false,false);
		else{
			scripts.in_InputHandler.SetInState(IN_STATE.UI);
			HideGroups(1);
			UTIL_Utilities.ShowHideCanvasGroup(LayersMenu, true, true, true);
		}
	}
	public void Button_DebugMenu(){
		//DebugMenu button hit.
		if(DebugMenu.GetComponent<CanvasGroup>().alpha==1)
			UTIL_Utilities.ShowHideCanvasGroup(DebugMenu,false,false,false);
		else UTIL_Utilities.ShowHideCanvasGroup(DebugMenu, true, false, false);
	}
	//MainMenu
	public void Button_Reset(){
		//Reset button hit. Reload the level.
		Application.LoadLevel(Application.loadedLevel);
	}
	public void Button_Scope(){
		//Scope button hit. Switch to the scope activity.
		scripts.mainHandler.SetGameMode(GAME_MODE.SCOPE);
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
	//ObjectMenu
	public void Button_ObjectMenu_Select(){
		//Select button hit.
		//If not in selecting mode, go into selecting mode.
		if(scripts.ui_ObjectMenuHandler.objMenuState!=OBJ_MENU_STATE.SELECTING) scripts.ui_ObjectMenuHandler.SetObjectMenuState(OBJ_MENU_STATE.SELECTING);
		else{	//If already in selecting mode, get out of it.
			scripts.ui_ObjectMenuHandler.SetObjectMenuState(OBJ_MENU_STATE.NONE);
			UTIL_Utilities.ShowHideCanvasGroup(ObjectInfoMenu, false, false, false);
		}
	}
	public void Button_ObjectMenu_Rotate(){
		//Rotate button hit. Able to rotate the object.
		if(scripts.ui_ObjectMenuHandler.objMenuState!=OBJ_MENU_STATE.ROTATING) scripts.ui_ObjectMenuHandler.SetObjectMenuState(OBJ_MENU_STATE.ROTATING);
		else{	//If already in rotating mode, get out of it.
			scripts.ui_ObjectMenuHandler.SetObjectMenuState(OBJ_MENU_STATE.NONE);
		}
	}
}
