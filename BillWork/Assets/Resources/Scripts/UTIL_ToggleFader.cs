using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UTIL_ToggleFader:MonoBehaviour{
	public Color colorOn;		//The normal color for when the toggle is on.
	public Color colorOff;		//The normal color for when the toggle if off.
	public bool state;			//The current state of the toggle.

	private bool updated;		//Whether the color needs to be updated this frame or not.

	void Start(){
		updated=false;
	}

	void Update(){
		UpdateFade();
	}

	public void UpdateFade(){
		//Set the fade state of the toggle.
		if(GetComponent<Toggle>()!=null){
			if(state!=GetComponent<Toggle>().isOn) updated=true;
			state=GetComponent<Toggle>().isOn;
		}

		if((GetComponent<Image>()!=null)&&(updated)){
			if(state) GetComponent<Image>().color=colorOn;
			else GetComponent<Image>().color=colorOff;
			updated=false;
		}
	}
}
