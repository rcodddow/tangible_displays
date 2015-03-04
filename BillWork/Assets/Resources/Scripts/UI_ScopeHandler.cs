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

	//Gameplay
	public EYE_GAMEMODE eyeGameMode;			//Which mode of play the eye is in.
	public GameObject eyeImage;					//The parent object of all the eye images. This is what is scaled and translated.
	public Slider zoomSlider;					//The slider which controls the zoom.
	private float maximumZoom;					//Maximum zoom value. This is the scale of the eyeImage.

	void Start(){
		eyeType=EYE_IMAGE.NORMAL;
		captionShown=false;

		ShowEye(EYE_IMAGE.NORMAL);

		eyeGameMode=EYE_GAMEMODE.STATIC;
		maximumZoom=5;
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.T)) SetGameMode(EYE_GAMEMODE.STATIC);
		if(Input.GetKeyDown(KeyCode.Y)) SetGameMode(EYE_GAMEMODE.INSPECT);

		if(eyeGameMode==EYE_GAMEMODE.STATIC) UpdateStatic();
		if(eyeGameMode==EYE_GAMEMODE.INSPECT) UpdateInspect();
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
		eyeGameMode=_eyeGameMode;

		if(eyeGameMode==EYE_GAMEMODE.STATIC){
			//Return to the static view.
			eyeImage.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,0);
			//eyeImage.transform.position=new Vector3(0,0,0);
			eyeImage.transform.localScale=new Vector3(1,1,1);
		}
		if(eyeGameMode==EYE_GAMEMODE.INSPECT){
			//Switch to the game mode.
		}
	}

	public void Button_Captions(){
		//Toggles showing the captions or not.

	}
}
