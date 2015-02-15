using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;//for limiting the characters input by user

public class NetworkManager : MonoBehaviour {
	public Spawning spawn;
	//allowing for other scripts to know if enemies have been spawned

	private bool singleplayer = false;
	private bool multiplayer = false;
	private bool setPlayerName = false;
	private List<string> chatMessages;
	private int maxChatMessages = 5;
	private UIManager ui;

	public void OnFailedToConnectToPhoton()
	{
		UnityEngine.Debug.LogError("Could not connect to the stuff");
	}

	void Awake()
	{
		gameObject.AddComponent<PhotonView> ();
		chatMessages = new List<string>() ;
		/*enemySpawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawn");
		playerSpawnPoints = GameObject.FindObjectsOfType<PlayerSpawnPoint>();*///getting all player spawnpoints
		//Connection (); //removing auto connect to allow for button selection
		PhotonNetwork.player.name = PlayerPrefs.GetString("User","New Player");//setting a default value to be saved
	}

	void Start()
	{
		ui = gameObject.GetComponent<UIManager> ();
		ui.UIEnableOnly ("StartUI");
		Screen.lockCursor = false;
	}
	
	void OnDestroy()//this is called when the game ends
	{
		PlayerPrefs.SetString ("User", PhotonNetwork.player.name);
	}

	public void AddChatMessage(string message)//abstracting further, so no need for very lengthy rpc calls in main code
	{
		//GetComponent<PhotonView> ().viewID ++;
		GetComponent<PhotonView> ().RPC ("AddChatMessage_RPC",PhotonTargets.AllBuffered,message);
	}
	[RPC]
	void AddChatMessage_RPC(string message)
	{
		while (chatMessages.Count >= maxChatMessages) {
			chatMessages.RemoveAt(0);}
		chatMessages.Add (message);
	}

	void Connection()
	{
		PhotonNetwork.ConnectUsingSettings ("UNex_1.0");//is the game version (don't want people from different vers connecting)
		//using the part of the photon stuff that holds settings regarding connectionl
	}

	void OnGUI()
	{
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString());//quick and easy way to display connection status
//		if(haveConnected){
//			PhotonNetwork.automaticallySyncScene = true;
//			//http://forum.exitgames.com/viewtopic.php?f=17&t=2575
//			Connection();
/*		}else*/ if(!PhotonNetwork.connected){//if user isn't currently connected 
			PhotonNetwork.automaticallySyncScene = true;
			//http://forum.exitgames.com/viewtopic.php?f=17&t=2575
			//UnityEngine.Debug.Log("name "+PhotonNetwork.player.name+"is set? "+setPlayerName);
			if(ui.GetPlayerName() !=""){
				PhotonNetwork.player.name = ui.GetPlayerName();}

			if(PhotonNetwork.player.name != "" && !setPlayerName)
			{
				ui.SetPlayerName(PhotonNetwork.player.name);
				//UnityEngine.Debug.Log(ui.GetPlayerName());
				setPlayerName = true;
			}
			//PhotonNetwork.player.name = Regex.Replace(^);

		}

		if(PhotonNetwork.connected && (singleplayer || multiplayer))
		{
			GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
			GUILayout.BeginVertical();//pushing the label down to the bottom
			GUILayout.FlexibleSpace();
			foreach(string message in chatMessages){
				GUILayout.Label(message);
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
	void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom ();
	}
	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom (null);//passing null because don't care what the room is called
	}

	void OnJoinedRoom()
	{
		spawn.SpawnPlayer ();
		ui.UIEnableOnly ("InGameUI");//setting the game up for play
		gameObject.GetComponent<GameManager> ().enabled = true;
		if (PhotonNetwork.isMasterClient) {//ensuring that only one set of enemies are spawned
			spawn.SpawnEnemies();
		}
	}

	public void StartSingleplayer()
	{
		singleplayer = true;
		multiplayer = false;
		//UnityEngine.Debug.Log (singleplayer);
		PhotonNetwork.offlineMode = true;
		OnJoinedLobby(); //pretending we are conncted and everything is fine
		//am just skipping the connect phase	
	}
	public void StartMultiplayer()
	{
		multiplayer = true;
		singleplayer = false;
		Connection ();
	}

	public bool gameStarted(){
		if(singleplayer || multiplayer){
			return true;
			UnityEngine.Debug.Log("returning true");
		}
		return false;
	}
	/*public void UpdateGame()
	{
		try{
			Process uProc = new Process();
			uProc.StartInfo.FileName = "update.exe";
			uProc.Start();
			Application.Quit ();
		}catch{
//			print("Unable to update game. Do you have update.exe in the same direcotry as "+
//			                            "the game?\ngo to: https://github.com/InappUser/UNexUpdateScript/archive/master.zip\n to download.");
			//ui.ShowUpdateFailed();
			ui.UIEnable("StartUIUpdateFailed");
		}
	}*/
}
