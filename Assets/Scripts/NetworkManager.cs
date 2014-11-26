using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;//for limiting the characters input by user

public class NetworkManager : MonoBehaviour {
	public GameObject spectator;
	public Spawning spawn;
	public bool connecting = false;
	public bool haveConnected = false;
	public static bool spawnedEnemies = false;
	//allowing for other scripts to know if enemies have been spawned

	private PlayerSpawnPoint[] playerSpawnPoints;//an array object "spawnPoints" created with the class "SpawnPoint"
	private GameObject[] enemySpawnPoints;//enemy spawns
	private bool connectFailed = false;
	private List<string> chatMessages;
	private int maxChatMessages =5;
	private GameObject[] enemyCheck;

	public void OnFailedToConnectToPhoton(object parameters)
	{
		this.connectFailed = true;
	}

	void Awake()
	{
		chatMessages = new List<string>() ;
		enemySpawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawn");
		playerSpawnPoints = GameObject.FindObjectsOfType<PlayerSpawnPoint>();//getting all player spawnpoints
		//Connection (); //removing auto connect to allow for button selection
		PhotonNetwork.player.name = PlayerPrefs.GetString("User","New Player");//setting a default value to be saved
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
			chatMessages.RemoveAt(0);
		}
		chatMessages.Add (message);
	}

	void Connection()
	{
		PhotonNetwork.ConnectUsingSettings ("Multiplayer_FPS_1.0");//is the game version (don't want people from different vers connecting)
		//using the part of the photon stuff that holds settings regarding connection
		//PhotonNetwork.offlineMode = true;//for debugging, otherwise is cool
	}

	void OnGUI()
	{
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString());
		if(haveConnected){
			PhotonNetwork.automaticallySyncScene = true;
			//http://forum.exitgames.com/viewtopic.php?f=17&t=2575
			Connection();
		}else if(!PhotonNetwork.connected){		//if we arn't currently connected 
			PhotonNetwork.automaticallySyncScene = true;
			//http://forum.exitgames.com/viewtopic.php?f=17&t=2575
			//this stuff is really antiquated, chnge to enabling and disabling buttons within the ui
			GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Player Name");
			PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name, 25);//max lenght of player name
			//PhotonNetwork.player.name = Regex.Replace(^);
			GUILayout.EndHorizontal();
			
			if(GUILayout.Button("Single Player") && !connecting){
				connecting=true;//getting rid of the buttons re-appearing after connection is set
				PhotonNetwork.offlineMode = true;
				OnJoinedLobby(); //pretending we are conncted and everything is fine
				//am just skipping the connect phase
			}
			if(GUILayout.Button("Multiplayer") && !connecting){
				connecting=true;
				Connection ();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
	}


		//		Debug.Log("connecting:"+connecting );
		if(PhotonNetwork.connected && !connecting)
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
		if (this.connectFailed) {
			Debug.Log("connection failed");
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
		if (PhotonNetwork.isMasterClient) {
			SpawnEnemies ();
				}
		connecting = false;


	}
	void SpawnEnemies()
	{
		enemyCheck = GameObject.FindGameObjectsWithTag("Enemy");

		if (enemyCheck.Length > 10) {
			spawnedEnemies = true;
			Debug.Log("Too many enemies");
		}

		if (spawnedEnemies == true) {
			Debug.Log("Too many enemies");
			//return;		
		}
		if(enemySpawnPoints == null)
		{
			Debug.LogError("There are no enemy spawn points available");
			return;
		}
		for (int i=0; i < enemySpawnPoints.Length; i++) {
			/*GameObject enemyGO = (GameObject)*/PhotonNetwork.Instantiate("Enemy",enemySpawnPoints[i].transform.position, enemySpawnPoints[i].transform.rotation,0);		
		}
		spawnedEnemies = true;
	}

}
