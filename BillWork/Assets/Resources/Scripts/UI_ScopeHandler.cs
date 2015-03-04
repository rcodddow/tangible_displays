using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles the gameplay and UI of the scope activity.
/// </summary>

public enum EYE_IMAGE{
	NORMAL,
	PAPILLEDEMA,
	MASCULADEGENERATION,
	NONE
}

public class UI_ScopeHandler:MonoBehaviour{
	public EYE_IMAGE eyeType;					//Which version of the eye is currently being shown.
	public bool captionShown;					//Whether or not the caption for the current eye image is being shown.
	public List<GameObject>	eyeImages;			//The different images of the eye.
	public List<GameObject> eyeCaptions;		//The corresponding caption images.

	void Start(){
		eyeType=EYE_IMAGE.NORMAL;
		captionShown=false;

		ShowEye(EYE_IMAGE.NORMAL);
	}

	void Update(){

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
}
