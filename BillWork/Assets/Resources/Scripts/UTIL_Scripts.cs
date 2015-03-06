using UnityEngine;
using System.Collections;

/// <summary>
/// Contains references to all scripts so that it's easy to grab them.
/// </summary>

public class UTIL_Scripts:MonoBehaviour{
	public UIHandler uiHandler;
	public UI_LayersMenuHandler ui_LayersMenuHandler;
	public UI_ObjectMenuHandler ui_ObjectMenuHandler;
	public UI_ObjectInfoMenuHandler ui_ObjectInfoMenuHandler;
	public IN_InputHandler in_InputHandler;
	public CAM_Camera cam_Camera;
	public MainHandler mainHandler;
	public NET_Handler netHandler;

	void Start(){
	}

	void Update(){
	}
}
