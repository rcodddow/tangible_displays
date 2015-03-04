using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handles the debug menu to show additional information such as FPS.
/// </summary>

public class UI_ScopeDebugMenuHandler:MonoBehaviour {
	public Text textFPS;			//Current FPS.
	public Text textTouches;		//Number of touches currently being registered.

	void Start() {

	}

	void Update() {
		UpdateText();
	}

	private void UpdateText(){
		if(textFPS) textFPS.text="FPS: "+1.0f/Time.smoothDeltaTime;
		if(textTouches) textTouches.text="Touches: "+Input.touchCount;
	}
}
