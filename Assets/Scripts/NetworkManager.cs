using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;//for limiting the characters input by user

public class NetworkManager : MonoBehaviour {
	public GameObject spectator;
	public Spawning spawn;
	//allowing for other scripts to know if enemies have been spawned

	private PlayerSpawnPoint[] playerSpawnPoints;//an array object "spawnPoints" created with the class "SpawnPoint"
	private GameObject[] enemySpawnPoints;//enemy spawns
	private bool singleplayer = false;
	private bool mulitiplayer = false;
	private bool setPlayerName = false;
	private List<string> chatMessages;
	private int maxChatMessages = 5;
	private GameObject[] enemyCheck;
	private UIManager ui;

	public void OnFailedToConnectToPhoton(object parameters)
	{
		Debug.LogError("Could not connect to the stuff");
	}

	void Awake()
	{
		chatMessages = new List<string>() ;
		enemySpawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawn");
		playerSpawnPoints = GameObject.FindObjectsOfType<PlayerSpawnPoint>();//getting all player spawnpoints
		//Connection (); //removing auto connect to allow for button selection
		PhotonNetwork.player.name = PlayerPrefs.GetString("User","New Player");//setting a default value to be saved
	}

	void Start()
	{
		ui = gameObject.GetComponent<UIManager> ();
		ui.UIEnableOnly ("StartUI");
	}
	
	void OnDestroy()//this is called when the game ends
	{
		PlayerPrefs.SetString ("User", PhotonNetwork.player.name);
	}

	public void AddChatMessage(string message)//abstracting further, so no need for very lengthy rpc calls in main code
	{
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
		PhotonNetwork.ConnectUsingSettings ("Multiplayer_FPS_1.0");//is the game version (don't want people from different vers connecting)
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
			if(PhotonNetwork.player.name == "" && !setPlayerName)
			{
				ui.SetPlayerName(PhotonNetwork.player.name);
				setPlayerName = true;
			}
			PhotonNetwork.player.name = ui.GetPlayerName();
			//PhotonNetwork.player.name = Regex.Replace(^);

	}

	if(PhotonNetwork.connected)
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
		mulitiplayer = false;
		Debug.Log (singleplayer);
		PhotonNetwork.offlineMode = true;
		OnJoinedLobby(); //pretending we are conncted and everything is fine
		//am just skipping the connect phase		
	}
	public void StartMultiplayer()
	{
		mulitiplayer = true;
		singleplayer = false;
		Connection ();
	}
}
