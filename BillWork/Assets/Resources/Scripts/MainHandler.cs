using UnityEngine;
using System.Collections;

/// <summary>
/// Main program handler.
/// </summary>

public enum GAME_MODE{
	GAME,
	SCOPE,
	NONE
};

public class MainHandler:MonoBehaviour {
	public GAME_MODE gameMode;		//The current game mode.
	public CanvasGroup CG_MainUI;	//The main game UI.
	public CanvasGroup CG_ScopeUI;	//The scope activity UI.

	void Start(){
	}

	void Update(){
	}

	public void SetGameMode(GAME_MODE _gameMode){
		//Set the main game mode.
		gameMode=_gameMode;

		if(gameMode==GAME_MODE.GAME){	//Switch to the main game.
			UTIL_Utilities.ShowHideCanvasGroup(CG_MainUI, true, true, true);
			UTIL_Utilities.ShowHideCanvasGroup(CG_ScopeUI, false, false, false);
		}
		else if(gameMode==GAME_MODE.SCOPE){	//Switch to the scope mode.
			UTIL_Utilities.ShowHideCanvasGroup(CG_MainUI, false, false, false);
			UTIL_Utilities.ShowHideCanvasGroup(CG_ScopeUI, true, true, true);
		}
	}
}
