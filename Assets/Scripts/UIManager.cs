using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;


public class UIManager : MonoBehaviour {
	public bool paused = false;
	public static bool resume = false;
	private GameObject[] uis;//an array of UI gameobjects
	private Text playerName;


	// Use this for initialization
	GameObject[] FindGameObjectByLayer(int layer)
	{
		GameObject[] suspectedGOs = FindObjectsOfType<GameObject> ();//getting all of the game objects
		List<GameObject> layerGOsList = new List<GameObject> ();// creating and instantiating a list
		for (int i =0; i < suspectedGOs.Length; i++) {//adding gameobject to the list if it has the same layer as passed
			if(suspectedGOs[i].layer == layer){
				layerGOsList.Add(suspectedGOs[i]);
				if(suspectedGOs[i].name == "TxtPlayerName")
					playerName = suspectedGOs[i].GetComponent<Text>();
			}
		}
		if(layerGOsList.Count == 0){//returning null if no gameobjects found on that layer
			return null;
		}
		return layerGOsList.ToArray ();//returning all of the gameobjects in the list in array form
	}

	void Awake () {
		uis = FindGameObjectByLayer (5);//5 is index for the "UI" layer
		for (int i =0; i< uis.Length; i++) {
			if(uis[i].name == "InputPlayerName"){
				ChangeUIColor(uis[i],0.5f);
				//UnityEngine.Debug.Log("hit the shizzel");
			}
			else{
				ChangeUIColor(uis[i],1);}		
		}
		//UIEnableOnly ("InGameUI");
	}

	public void PausedResume(GameObject player)
	{
		try{
			if(!paused){//is paused
				paused = true;
			}
			else{//is not paused
				paused = false;
			}
			Screen.lockCursor = !paused;
			//UnityEngine.Debug.Log (player.GetComponentInChildren<Shoot>() == null);
			player.GetComponentInChildren<Shoot>().enabled = !paused;
			player.GetComponentInChildren<WeaponManager>().enabled = !paused;
			player.GetComponentInChildren<UseEquipment>().enabled = !paused;
			player.GetComponentInChildren<EquipmentManager>().enabled = !paused;
			player.GetComponent<MouseLook>().enabled = !paused;
			player.GetComponent<PlayerMovement>().enabled = !paused;
			player.GetComponentInChildren<Camera> ().GetComponent<MouseLook>().enabled = !paused;
			//UnityEngine.Debug.Log (player.GetComponent<MouseLook> ().enabled);
			if(!paused){
				UIEnableOnly ("InGameUI");
			}
			else{
				UIEnableOnly ("PauseUI","PauseUIMain");
				UIDisable("PauseUIOptions");
			}
		}catch(System.Exception ex){
			UnityEngine.Debug.LogError("Don't know about player!\nYou probably left an instance in the hierarchy\n"+ex);}
	}
	public void UIEnableOnly(string enable)
	{
		for(int i=0;i<uis.Length;i++){//setting all UI to inacive at start
			if(uis[i].tag != enable){//will break if disable more that the other ui
				uis[i].SetActive(false);
				}
			else{
				uis[i].SetActive(true);
			}
		}
	}
	void UIEnableOnly(string enable1, string enable2)
	{ 
		for(int i=0;i<uis.Length;i++){//setting all UI to inacive at start
			if(uis[i].tag != enable1 && uis[i].tag != enable2){//will break if disable more that the other ui
				uis[i].SetActive(false);
			}
			else{
				uis[i].SetActive(true);
			}
		}
	}
	public void UIEnable(string enable)
	{ 
		for (int i =0; i<uis.Length; i++){
			if(uis[i].tag == enable){
				uis [i].SetActive (true);
			}
		}	
	}
	public void UIDisable(string disable)
	{
		//despite there being only ine object with tag, need to utilise an array
		GameObject[] disabledGOs = GameObject.FindGameObjectsWithTag (disable);
		for (int i =0; i<disabledGOs.Length; i++){
			disabledGOs [i].SetActive (false);
		}
	}
	void ChangeUIColor(GameObject uiGO, float colourTrue)
	{
		if (uiGO.GetComponent<Text>() != null) {
			Color textColour = new Color();
			textColour = uiGO.GetComponent<Text>().color;
			textColour.a = colourTrue;
			uiGO.GetComponent<Text>().color =  textColour;
		}
		if (uiGO.GetComponent<Image>()) {
			Color textColour = new Color();
			textColour = uiGO.GetComponent<Image>().color;
			textColour.a = colourTrue;
			uiGO.GetComponent<Image>().color =  textColour;
		}
	}

	public void ResumeGame ()
	{
		resume = true;
	}


	public void ExitGame()
	{
		Application.Quit ();
	}
	public void EnterOptions()
	{
		UIDisable ("PauseUIMain");
		UIEnable("PauseUIOptions");
	}
	public void ExitOptions()
	{
		UIDisable ("PauseUIOptions");
		UIEnable("PauseUIMain");
	}
	public string GetPlayerName()
	{
		return playerName.text;
	}
	public void SetPlayerName(string text)
	{
		playerName.text = text;
	}

	public void ToggleFullScreen()
	{
		UnityEngine.Debug.Log ("Hit");
		if(!Screen.fullScreen){
			UnityEngine.Debug.Log ("full");
			Screen.fullScreen = true;}
		else{
			UnityEngine.Debug.Log ("not full");
			Screen.fullScreen = false;
		}
	}
	public void UpdateGame()
	{
		try{
			Process uProc = new Process();
			uProc.StartInfo.FileName = "update.exe";
			uProc.Start();
			Application.Quit ();
		}catch{
			//			print("Unable to update game. Do you have an update.exe file in the same direcotry as he game?
			//Go to: https://github.com/InappUser/UNexUpdateScript/archive/master.zip to download, if not.");
			//ui.ShowUpdateFailed();
			UIEnable("StartUIUpdateFailed");
		}
		//UIEnable("StartUIUpdateFailed");

	}
	public void HideUpdateFailed()
	{
		UIDisable ("StartUIUpdateFailed");
	}

	
}
