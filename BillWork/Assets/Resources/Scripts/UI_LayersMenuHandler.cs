using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Handles the Layers menu to show and hide parts of the model.
/// </summary>

public class UI_LayersMenuHandler:MonoBehaviour {
	public GameObject togglesParent;			//The parent object for all the toggle buttons.
	public GameObject root;						//The root of the model in the scene being shown.
	public List<GameObject> toggleableParts;	//The different parts of the model that can be toggled on and off.
	public List<Toggle> allToggles;				//The list of all created toggles.

	void Start() {
		CreateToggles();
	}

	void Update(){
		CheckToggles();
	}
	private void CheckToggles(){
		//Checks whether any of the objects need to be visible or not.
		for(int i=0; i<allToggles.Count; i++){
			bool toggleValue=allToggles[i].isOn;
			if(toggleValue!=toggleableParts[i].activeSelf) toggleableParts[i].SetActive(toggleValue);
		}
	}

	public void ToggleObject(GameObject _object){
		//Toggles the visibility of an input object if it is a part of the toggleableParts list.
		for(int i=0; i<toggleableParts.Count; i++){
			if(toggleableParts[i]==_object){
				allToggles[i].isOn=!allToggles[i].isOn;
				toggleableParts[i].SetActive(allToggles[i].isOn);
				return;
			}
		}

		//Part not in the list.
		Debug.Log("Trying to toggle the visibility of object \""+_object.name+"\" but it is not in the toggleableParts list.");
	}

	private void CreateToggles(){
		//Creates all the toggles needed for each of the toggleableParts.
		RectTransform togglesParentRect=togglesParent.transform.GetChild(0).GetComponent<RectTransform>();
		
		if(togglesParentRect==null){
			Debug.Log("Can't CreateToggles because the togglesParent doesn't have the \"TOG_Test\" child.");
			return;
		}

		Vector2 pos=togglesParentRect.anchoredPosition;												//The anchored position of the first toggle.
		Vector2 size=new Vector2(togglesParentRect.rect.width,togglesParentRect.rect.height);		//The width and height of the toggle.

		//Create all the toggles needed based on the toggleableParts.
		allToggles=new List<Toggle>();
		GameObject temp=togglesParent.transform.GetChild(0).gameObject;
		for(int i=0; i<toggleableParts.Count; i++){
			GameObject newChild=(GameObject)Instantiate(temp);
			newChild.transform.SetParent(togglesParent.transform,false);
			newChild.GetComponent<RectTransform>().anchoredPosition=new Vector2(pos.x,pos.y-size.y*i);
			newChild.name="TOG_"+toggleableParts[i].name;											//Set GameObject name
			newChild.transform.GetChild(0).GetComponent<Text>().text=toggleableParts[i].name;		//Set button text
			allToggles.Add(newChild.GetComponent<Toggle>());
		}

		//Set the TOG_Test to inactive now that it's been used as the template.
		togglesParent.transform.GetChild(0).gameObject.SetActive(false);

		//Need to expand the height of the Toggles so that all the toggles fit in the scroll view properly.
		float newHeight=(size.y*toggleableParts.Count)+30;
		togglesParent.GetComponent<RectTransform>().sizeDelta=new Vector2(togglesParent.GetComponent<RectTransform>().rect.width,newHeight);
	}
}
