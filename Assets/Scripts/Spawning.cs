using UnityEngine;
using System.Collections;

public class Spawning : MonoBehaviour {

	public UIManager ui;
	public GameObject spectator;
	public static bool spawnedEnemies = false;

	private NetworkManager nm;
	private GameObject[] enemyTotalSpawnPoints;
	private PlayerSpawnPoint[] playerSpawnPoints;
	private float respawnTime = 2.5f;
	private bool firstSpawn = false;

	// Use this for initialization
	void Start () {
		playerSpawnPoints = GameObject.FindObjectsOfType<PlayerSpawnPoint>();
		enemyTotalSpawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawn");
		nm = GetComponent<NetworkManager>();
	}
	void EnablePlayer(GameObject player, bool enable)
	{
		if (player && player.transform.root.gameObject.GetPhotonView ().isMine) {
			spectator.SetActive (!enable);
			player.transform.FindChild ("Main Camera").gameObject.SetActive (enable);
			//Debug.Log ("Setting main camera to " + enable);		
			
			((MonoBehaviour)player.GetComponent ("MouseLook")).enabled = enable;//how to reference disabled components, etc.
			if(firstSpawn){
				player.GetComponent<MouseLook>().SensitivityX = PlayerPrefs.GetFloat("SensitivityX"); //setting the saved sensitivity
				player.transform.GetChild(0).GetComponentInChildren<MouseLook>().SensitivityY = PlayerPrefs.GetFloat ("SensitivityY");
				firstSpawn = false;
			}
			((MonoBehaviour)player.GetComponent ("PlayerMovement")).enabled = enable;
			((MonoBehaviour)player.GetComponentInChildren<EquipmentManager>()).enabled = enable;

			((MonoBehaviour)player.GetComponentInChildren<UseEquipment>()).enabled = enable;
			//have to do cast (monobehaviour) bc of the scripts being javascript
			Shoot[] shoots = player.GetComponentsInChildren<Shoot> ();//enabling two of the same script
			for(int i=0;i<shoots.Length;i++)
			{
				shoots[i].enabled = enable;
			}
			WeaponManager[] weaps = player.GetComponentsInChildren<WeaponManager> ();//enabling two of the same script
			for(int i=0;i<weaps.Length;i++)
			{
				weaps[i].enabled = enable;
			}
			GunPivot[] gms = player.GetComponentsInChildren<GunPivot> ();//enabling two of the same script
			for(int i=0;i<gms.Length;i++)
			{
				gms[i].enabled = enable;
			}
			MeshRenderer[] meshes = player.transform.GetComponentsInChildren<MeshRenderer> ();
			for(int i=0;i<meshes.Length;i++)
			{
				meshes[i].enabled = enable;
			}
		}
	}
	
	public void SpawnPlayer()
	{
		firstSpawn = true;
		nm.AddChatMessage ("Spawning "+PhotonNetwork.player.name);
		
		if(playerSpawnPoints == null){
			Debug.LogError("There are no player spawn points available");
			return;}
		
		PlayerSpawnPoint playerSpawn = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
		//could use normal instantiate but other people will not be able to see me
		GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate("FPS_Player",playerSpawn.transform.position,playerSpawn.transform.rotation,0 );
		EnablePlayer (myPlayerGO, true);
	}
	public void Death(GameObject player, bool isEnd)
	{
		ui.UIDisable ("InGameUI");
		EnablePlayer (player, false);
		if(!isEnd){
			StartCoroutine(RespawnPlayer (player));
		}else{
			ui.UIEnableOnly("WinUI");
		}
	}
	IEnumerator RespawnPlayer(GameObject player)
	{//if the player continues to lose health after death, they will be sent to a bunch of different spawns 
		if(playerSpawnPoints == null)
		{
			Debug.LogError("There are no player spawn points available");
			yield return false;
		}

		PlayerSpawnPoint playerSpawn = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
		//getting a random spawnpoint
		player.transform.position = playerSpawn.transform.position;
		MouseLook bodyRot = (MouseLook)player.transform.GetComponent("MouseLook");
		MouseLook cameraRot = (MouseLook)player.transform.GetChild(0).transform.GetComponent("MouseLook");
		bodyRot.ResetRotation (playerSpawn.transform.rotation);
		cameraRot.ResetRotation (playerSpawn.transform.rotation);
		//ressetting player position and rotation prior to spawning so that spawning is less jaring

		yield return new  WaitForSeconds (respawnTime);

		nm.AddChatMessage ("Respawning "+PhotonNetwork.player.name);
		ui.UIEnable ("InGameUI");
		EnablePlayer (player, true);
		Health h = player.GetComponent<Health> ();
		h.RestetHP ();
	}

	public void SpawnEnemies()
	{
		int amountToSpawn = Random.Range ((int)(enemyTotalSpawnPoints.Length/1.7f),(int)(enemyTotalSpawnPoints.Length/1.1f));
		int[] toSpawnList = new int[amountToSpawn];
		int getRandNum = 0;
		bool validNumber=true;
		int enemyTypeToSpawn =0;
		//Debug.Log ("amount to spawn"+amountToSpawn);
		if(enemyTotalSpawnPoints == null)
		{
			Debug.LogError("There are no enemy spawn points available");
			return;
		}

		for (int i=0; i<amountToSpawn; i++) {
			//Debug.Log("spawnlist index:"+toSpawnList[i]);
			do// need this: if the first randomly generated number is already used then need to generate another 
			{//before moving on to next index
				validNumber = true;
				getRandNum = Random.Range(1,enemyTotalSpawnPoints.Length+1);//need to be more than zero as that is what each array value is by
				//default. correcting this by -1 afterwards
				for(int j=0;j<amountToSpawn;j++){//checking to see if spawn is already assigned
					if(getRandNum == toSpawnList[j])
						validNumber = false;
				}
				if(validNumber){//if random num is good, then assign it and reset validNumber. if this is not called then the current index of 
					toSpawnList[i] = getRandNum;//toSpawnList will remain 0 and while will repeat
				}
			}while(!validNumber);
		}

		//generate random number between total spawns and total spawns minus 3 or whatever
		//have for loop for(int i=0;i<amountToSpawn;i++)
		//in for loop create array to keep track of what number has already been picked (id of spawnpoint, essentialy) - add to it until
		//its length is == to amountToSpawn
		//ignore above for --- while(toSpawnList.length<amountToSpawn)
		//temporery variable made random upon each iteration (nested loop to ensure that the number chosen has not allready been selected -
		//checking against each index in toSpawnList)
		//Debug.Log("total amount of spawns: "+enemyTotalSpawnPoints.Length);
		for (int i=0; i<amountToSpawn; i++) {
			if(enemyTypeToSpawn>0){//if more than 0, spawn alives
				PhotonNetwork.Instantiate("EnemyAlive",enemyTotalSpawnPoints[toSpawnList[i]-1].transform.position,enemyTotalSpawnPoints[toSpawnList[i]-1].transform.rotation,0);		
			}else{
				PhotonNetwork.Instantiate("EnemyStatic",enemyTotalSpawnPoints[toSpawnList[i]-1].transform.position,enemyTotalSpawnPoints[toSpawnList[i]-1].transform.rotation,0);		
				PlayerScore.enemyStaticsTotal ++;//making total static enemies accessable
			}
			enemyTypeToSpawn = Random.Range(0,3);//putting at the bottom so that there will allways be at least one static

		}
	}


}
