using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public Image winImage;
	public Text winText;
	public Text playerScore;
	public Text playerHealthText;
	public Text playerWeaponText;
	public Text playerAmmoCountText;
	public Text playerEquipmentText;
	public Text playerEquipmentLeftText;
	public GameObject pauseUI;
	public NetworkManager networkManager;
	public GameObject spectator;
	public static int enemyCount;

	private float restartDelay = 3f;
	private float restartTime;
	private Color imagecolour = new Color(1f,0f,0f,.6f);
	private Color textcolour = new Color(0f,0f,0f,1f);
	private GameObject player;
	private Health playerHealth;
	private WeaponManager playerWeapon;
	private EquipmentManager playerEquipment;
	private Shoot weaponAmmo;
	private bool paused = false;
	private UIManager pauseGame;
	static short level=1;

	public UIManager ReturnPauseUI()
	{
		return pauseGame;
	}
	public GameObject ReturnSpectator()
	{
		return spectator;
	}
	
	void Start()
	{
		pauseGame = gameObject.GetComponent<UIManager> ();
		enemyCount = (GameObject.FindGameObjectsWithTag ("EnemyStatic").Length + GameObject.FindGameObjectsWithTag ("EnemyAlive").Length);
		Debug.Log (enemyCount);
	}
	
	// Update is called once per frame
	void Update () {
		if (NetworkManager.spawnedEnemies) {
			enemyCount = (GameObject.FindGameObjectsWithTag ("EnemyStatic").Length + GameObject.FindGameObjectsWithTag ("EnemyAlive").Length);
			Debug.Log (enemyCount);
			NetworkManager.spawnedEnemies = false;}

		playerScore.text = enemyCount.ToString();

		if (!player) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		else{
			HealthCount ();
			WeaponSelection ();
			EquipmentSelection();
			AmmoCount();
			if((Input.GetKeyDown(KeyCode.Escape) || UIManager.resume) && player){
				//Debug.Log("pausegame: "+pauseGame.resume);
				UIManager.resume = false;//these were made static - there will only ever be one instance per player
				pauseGame.PausedResume (player);//abstracting the pause to the pause script
			}
		}

		if (enemyCount <= 0) {
			LoadLevel ();	
		}
	}
	

	void GamePause()
	{
		Debug.Log ("pressed"+ paused);
		pauseGame.PausedResume (player);

	}
	
	void HealthCount()
	{
		if(!playerHealth){
			playerHealth = player.GetComponent<Health> ();
		}
		else{
			playerHealthText.text = "Health: "+ (int)playerHealth.GetHealth();
		}
	}

	void WeaponSelection()
	{
		if(!playerWeapon){
			playerWeapon = player.GetComponentInChildren<WeaponManager>();
		}
		else{
			playerWeaponText.text = playerWeapon.currentWeapon.GetWeaponName();

		}
	 }
	void EquipmentSelection()
	{
		if(!playerEquipment){
			playerEquipment = player.GetComponentInChildren<EquipmentManager>();
		}
		else{
			playerEquipmentText.text = playerEquipment.GetEquipmentName();
			
		}
	}

	void AmmoCount()
	{
		if (!weaponAmmo) {
			weaponAmmo = player.GetComponentInChildren<Shoot>();		
		}
		else if(weaponAmmo.reloading){
			playerAmmoCountText.text = "Reloading";
		}
		else{
			playerAmmoCountText.text = weaponAmmo.GetCurrentAmmo().ToString();
		}

	}

	void LoadLevel()
	{
		GameObject[] allGOs = GameObject.FindObjectsOfType<GameObject> ();

		winImage.color = Color.Lerp(winImage.color, imagecolour ,1f * Time.deltaTime);
		winText.color = Color.Lerp(winText.color, textcolour ,4f * Time.deltaTime);
		restartTime += Time.deltaTime;
		cleanPhotonObjects();
		if(restartTime >= restartDelay){

			PhotonNetwork.LeaveRoom();
			networkManager.haveConnected = true;
			//Destroy(networkManager);
			if(level <1){
				/*Application.*/PhotonNetwork.LoadLevel(level);
				level += 1;
				Debug.Log("level: "+level+"++");
			}
			else{
				PhotonNetwork.LoadLevel(level--);
				Debug.Log("level: "+level+"--");
			}
			
				//Application.LoadLevel(level++);
			
		}
	}

	static void cleanPhotonObjects()
	{//http://blog.diabolicalgame.co.uk/2014/04/avoid-photon-ondestroy-warnings.html
		PhotonView[] pViews = GameObject.FindObjectsOfType<PhotonView>();

		foreach(PhotonView view in pViews)
		{
			GameObject isMineGO = view.gameObject;

			if(isMineGO != null && view.isMine && PhotonNetwork.networkingPeer.instantiatedObjects.ContainsKey(view.instantiationId))//client destroying own gameobject
			{//the second clause is due to an error occuring with photon network trying to remove the enemies (the dont have an instantiation id
				//because they arn't phonnetwork instantiated)
				PhotonNetwork.Destroy(isMineGO);
			}
			else
			{
				if(PhotonNetwork.networkingPeer.instantiatedObjects.ContainsKey(view.instantiationId))
				{//if is not this clients object remove its key- don't worry about it anymore
					PhotonNetwork.networkingPeer.instantiatedObjects.Remove(view.instantiationId);
				}
			}
		}
		PhotonNetwork.SetSendingEnabled (0, false);
		PhotonNetwork.isMessageQueueRunning = false;
	}
}