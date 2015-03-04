using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Handles the gameplay and UI of the scope activity.
/// </summary>

public enum EYE_IMAGE{
	NORMAL,
	PAPILLEDEMA,
	MASCULADEGENERATION,
	NONE
}
public enum EYE_GAMEMODE{
	STATIC,			//Show the full eye statically.
	INSPECT,		//Allow you to zoom and scroll the eye view.
	NONE
};

public class UI_ScopeHandler:MonoBehaviour{
	public EYE_IMAGE eyeType;					//Which version of the eye is currently being shown.
	public bool captionShown;					//Whether or not the caption for the current eye image is being shown.
	public List<GameObject>	eyeImages;			//The different images of the eye.
	public List<GameObject> eyeCaptions;		//The corresponding caption images.
	
	//Menus
	public CanvasGroup MainMenu;				//Main menu with Reset.
	public CanvasGroup StaticMenu;				//Static game mode menu.
	public CanvasGroup InspectMenu;				//Inspect game mode menu.
	public CanvasGroup DebugMenu;				//The debug menu.
	public List<Toggle> mainToolbarToggles;		//The toggles for the MenuToolbar. Menu, Static, Inspect.

	//Gameplay
	public EYE_GAMEMODE eyeGameMode;			//Which mode of play the eye is in.
	public GameObject eyeImage;					//The parent object of all the eye images. This is what is scaled and translated.
	public Slider zoomSlider;					//The slider which controls the zoom.
	private float maximumZoom;					//Maximum zoom value. This is the scale of the eyeImage.

	public UTIL_Scripts scripts;

	void Start(){
		eyeType=EYE_IMAGE.NORMAL;
		captionShown=false;

		ShowEye(EYE_IMAGE.NORMAL);

		eyeGameMode=EYE_GAMEMODE.STATIC;
		maximumZoom=5;
	}

	void Update(){
		UpdateToolbarGroups();
		UpdateGameMode();
		if(eyeGameMode==EYE_GAMEMODE.STATIC) UpdateStatic();
		if(eyeGameMode==EYE_GAMEMODE.INSPECT) UpdateInspect();
	}
	private void UpdateToolbarGroups(){
		//Update which toolbar canvas groups are being shown.
		HideCanvasGroups(1);

		//MainMenu group has priority.
		if(mainToolbarToggles[0].isOn) UTIL_Utilities.ShowHideCanvasGroup(MainMenu, true, true, true);
		else{
			if(mainToolbarToggles[1].isOn) UTIL_Utilities.ShowHideCanvasGroup(StaticMenu, true, true, true);
			if(mainToolbarToggles[2].isOn) UTIL_Utilities.ShowHideCanvasGroup(InspectMenu, true, true, true);
		}
	}
	private void UpdateGameMode(){
		if((mainToolbarToggles[1].isOn)&&(!mainToolbarToggles[2].isOn)) SetGameMode(EYE_GAMEMODE.STATIC);
		else if((!mainToolbarToggles[1].isOn)&&(mainToolbarToggles[2].isOn)) SetGameMode(EYE_GAMEMODE.INSPECT);
		else if((mainToolbarToggles[1].isOn)&&(mainToolbarToggles[2].isOn)){
			if(eyeGameMode==EYE_GAMEMODE.STATIC){
				SetGameMode(EYE_GAMEMODE.INSPECT);
				mainToolbarToggles[1].isOn=false;
			}
			else if(eyeGameMode==EYE_GAMEMODE.INSPECT){
				SetGameMode(EYE_GAMEMODE.STATIC);
				mainToolbarToggles[2].isOn=false;
			}
			else if(eyeGameMode==EYE_GAMEMODE.NONE){
				SetGameMode(EYE_GAMEMODE.STATIC);
				mainToolbarToggles[2].isOn=false;
			}
		}
		else{
			SetGameMode(EYE_GAMEMODE.NONE);
		}
	}
	private void UpdateStatic(){
		//Update the static game mode.
	}
	private void UpdateInspect(){
		//Update the inspect game mode.

		//Zoom the eye based on the slider	///////////////////////////////////////
		float newZoom=1.0f+(zoomSlider.value*(maximumZoom-1.0f));
		eyeImage.GetComponent<RectTransform>().transform.localScale=new Vector3(newZoom, newZoom, newZoom);

		//Make sure the eye hasn't been scrolled too far	///////////////////////
		float currentDistance=eyeImage.GetComponent<RectTransform>().anchoredPosition.magnitude;	//The current distance from the center that the eye is at.

		float imageWidth=eyeImage.GetComponent<RectTransform>().sizeDelta.x;		//The eye's RectTransform image width.
		float newImageWidth=eyeImage.GetComponent<RectTransform>().transform.localScale.x*imageWidth;	//The eye's 'supposed' RectTransform image width based on it's scale.
		float maximumDistance=(newImageWidth-imageWidth)/2.0f;		//The maximum distance from the center that the eye should be based on it's current scale.

		if(currentDistance>maximumDistance)	//The eye is too far from the center. Snap it back.
			eyeImage.GetComponent<RectTransform>().anchoredPosition=eyeImage.GetComponent<RectTransform>().anchoredPosition.normalized*maximumDistance;
	}

	public void ShowEye(EYE_IMAGE _eyeType, bool _showCaption=false){
		for(int i=0; i<eyeImages.Count; i++)
			eyeImages[i].SetActive(false);

		eyeType=_eyeType;
		ToggleCaption(_showCaption);

		if(eyeType==EYE_IMAGE.NONE){
			Debug.Log("Trying to ShowEye for the scope activity for a NONE EYE_IMAGE.");
			return;
		}
		eyeImages[(int)eyeType].SetActive(true);
	}
	public void ToggleCaption(bool _show){
		for(int i=0; i<eyeCaptions.Count; i++)
			eyeCaptions[i].SetActive(false);

		if(eyeType==EYE_IMAGE.NONE){
			Debug.Log("Trying to ToggleCaption for the scope activity for a NONE EYE_IMAGE.");
			return;
		}

		eyeCaptions[(int)eyeType].SetActive(_show);
	}

	public void SetGameMode(EYE_GAMEMODE _eyeGameMode){
		if(eyeGameMode==_eyeGameMode) return;

		eyeGameMode=_eyeGameMode;	//Switch game mode.

		if(eyeGameMode==EYE_GAMEMODE.STATIC){
			//Return to the static view.
			eyeImage.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,0);
			eyeImage.transform.localScale=new Vector3(1,1,1);
		}
		else if(eyeGameMode==EYE_GAMEMODE.INSPECT){
			//Switch to the game mode.
			zoomSlider.value=0;	//Reset zoom to 0.
		}
		else if(eyeGameMode==EYE_GAMEMODE.NONE){
		}
	}

	private void HideCanvasGroups(int _group){
		//Hides a group of menus.
		if(_group==1){	//MainMenu, StaticMenu, InspectMenu
			UTIL_Utilities.ShowHideCanvasGroup(MainMenu,false,false,false);
			UTIL_Utilities.ShowHideCanvasGroup(StaticMenu,false,false,false);
			UTIL_Utilities.ShowHideCanvasGroup(InspectMenu,false,false,false);
		}
	}
	private void SetToggles(List<Toggle> _toggles, bool _value){
		//Sets a group of toggles to an 'isOn' value.
		for(int i=0; i<_toggles.Count; i++)
			_toggles[i].isOn=_value;
	}

	// BUTTONS/TOGGLES	//////////////////////////////////
	//Scope_MenuToolbar
	public void Button_Menu(){
		//Toggles showing the MainMenu or not.
		if(mainToolbarToggles[0].isOn){
			HideCanvasGroups(1);
			UTIL_Utilities.ShowHideCanvasGroup(MainMenu, true, true, true);
		}
		else{
			HideCanvasGroups(1);
		}
	}
	public void Button_Captions(Toggle _caller){
		//Toggles showing the captions or not.
		captionShown=!captionShown;
		_caller.isOn=captionShown;
		ToggleCaption(captionShown);
	}
	public void Button_Debug(Toggle _caller){
		//Toggles showing the DebugMenu or not.
		if(DebugMenu.GetComponent<CanvasGroup>().alpha==1)
			UTIL_Utilities.ShowHideCanvasGroup(DebugMenu,false,false,false);
		else UTIL_Utilities.ShowHideCanvasGroup(DebugMenu, true, false, false);
	}
	//Scope_MainMenu
	public void Button_Game(){
		//Game button hit. Switch to the game activity.
		scripts.mainHandler.SetGameMode(GAME_MODE.GAME);
	}
}
