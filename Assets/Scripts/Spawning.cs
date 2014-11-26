using UnityEngine;
using System.Collections;

public class Spawning : MonoBehaviour {
	public NetworkManager NM;
	public GameObject spectator;

	private PlayerSpawnPoint[] playerSpawnPoints;

	// Use this for initialization
	void Start () {
		playerSpawnPoints = GameObject.FindObjectsOfType<PlayerSpawnPoint>();
	}
	void EnablePlayer(GameObject player, bool enable)
	{
		if (player && player.transform.root.gameObject.GetPhotonView ().isMine) {
			spectator.SetActive (!enable);
			player.transform.FindChild ("Main Camera").gameObject.SetActive (enable);
					
			
			((MonoBehaviour)player.GetComponent ("MouseLook")).enabled = enable;
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
		NM.AddChatMessage ("Spawning "+PhotonNetwork.player.name);
		
		if(playerSpawnPoints == null)
		{
			Debug.LogError("There are no player spawn points available");
			return;
		}
		
		PlayerSpawnPoint playerSpawn = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
		//could use normal instantiate but other people will not be able to see me
		GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate("FPS_Player",playerSpawn.transform.position,playerSpawn.transform.rotation,0 );
		//instantiate returns a value which is this player (not other players in game)
		//the (Gameobject) part is called "casting"
		
		//myPlayerGO.transform.FindChild ("Main Camera").gameObject.SetActive(true);
		//myPlayerGO.transform.FindChild ("Main Camera").transform.FindChild ("Equipment").gameObject.SetActive(true);
		//need to go into the transform apparently to find children, then grab the game object and set it to active
		EnablePlayer (myPlayerGO, true);
	}
	public void Death(GameObject player)
	{

		//instantiate returns a value which is this player (not other players in game)
		//the (Gameobject) part is called "casting"

		//spectator.SetActive (true);
		//myPlayerGO.transform.FindChild ("Main Camera").transform.FindChild ("Equipment").gameObject.SetActive(true);
		//need to go into the transform apparently to find children, then grab the game object and set it to active
		EnablePlayer (player, false);
		StartCoroutine(RespawnPlayer (player));
	}
	IEnumerator RespawnPlayer(GameObject player)
	{//if the player continues to lose health after death, they will be sent to a bunch of different spawns 
		yield return new  WaitForSeconds (1);
		//spectator.SetActive (false);
		NM.AddChatMessage ("Respawning "+PhotonNetwork.player.name);
		
		if(playerSpawnPoints == null)
		{
			Debug.LogError("There are no player spawn points available");
			yield return false;
		}
		
		PlayerSpawnPoint playerSpawn = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
		//could use normal instantiate but other people will not be able to see me
		player.transform.position = playerSpawn.transform.position;
		player.transform.rotation = playerSpawn.transform.rotation;

		EnablePlayer (player, true);
		Health h = player.GetComponent<Health> ();
		h.currentHitPoints = h.hitPoints;
		//yield return new WaitForSeconds (0);
	}

}
