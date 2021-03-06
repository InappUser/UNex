﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public GameObject spectator;
	public GameObject pauseUI;
	public GameObject inGameUI;
	public GameObject winUI;
	public NetworkManager networkManager;
	public static int enemyStaticsDead =0;//initialising so that game doesn't end immediately - setting this in start may be an easier way to stop games ending when loading new level
	public static int enemStaticTotal=1;
	static short level;

	//ui
	private Image winImage;
	private Text winText;
	private Text playerHealthText;
	private Text playerWeaponText;
	private Text playerAmmoCountText;
	private Text playerEquipmentText;


	private Spawning spawnEndGame;
	private Color imagecolour = new Color(1f,0f,0f,.2f);
	private Color textcolour = new Color(0f,0f,0f,.5f);
	private Color zero;
	private GameObject player;
	private Health playerHealth;
	private WeaponManager playerWeapon;
	private EquipmentManager playerEquipment;
	private Shoot weaponAmmo;
	private UIManager uiManager;
	private float restartDelay = 3f;
	private float restartTime;
	private bool ended = false; 

	public UIManager ReturnPauseUI()
	{
		return uiManager;
	}
	public GameObject ReturnSpectator()
	{
		return spectator;
	}
	void GetUI(){
		playerHealthText = inGameUI.transform.FindChild ("TxtHealth").GetComponent<Text>();
		playerWeaponText = inGameUI.transform.FindChild ("TxtWeapName").GetComponent<Text>();
		playerAmmoCountText = inGameUI.transform.FindChild ("TxtWeapAmmo").GetComponent<Text>();
		playerEquipmentText = inGameUI.transform.FindChild ("TxtEquipName").GetComponent<Text> ();
		winImage = winUI.transform.FindChild ("ImgWin").GetComponent<Image>();
		winText = winUI.transform.FindChild ("TxtWin").GetComponent<Text>();
	}
	void Awake(){
		GetUI ();
	}
	void Start()
	{

		zero = new Color (0f,0f,0f,0f);
		level = (short)Application.loadedLevel;
		uiManager = gameObject.GetComponent<UIManager> ();
		spawnEndGame = gameObject.GetComponent<Spawning> ();
		Spawning.spawnedEnemies = true;
		//enemyStaticsDead = (GameObject.FindGameObjectsWithTag ("EnemyStatic").Length + GameObject.FindGameObjectsWithTag ("EnemyAlive").Length);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("SPAWNED ENEMIES " + Spawning.spawnedEnemies);
//		if (Spawning.spawnedEnemies) { //poor way to get current amount of alive
//			PlayerScore.enemiesTotal = (GameObject.FindGameObjectsWithTag ("EnemyStatic").Length + GameObject.FindGameObjectsWithTag ("EnemyAlive").Length);
//			Spawning.spawnedEnemies = false;}

		if (!player) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		else{
			HealthCount ();
			WeaponSelection ();
			EquipmentSelection();
			AmmoCount();
			if((Input.GetKeyDown(KeyCode.Escape) || UIManager.resume) && player && player.GetComponent<Health>().GetHealth() >0){
				//Debug.Log("pausegame: "+uiManager.resume);
				UIManager.resume = false;//these were made static - there will only ever be one instance per player
				uiManager.PausedResume (player);//abstracting the pause to the pause script
			}
			if(Input.GetKeyDown(KeyCode.Tab) && !uiManager.paused){
				uiManager.showHideSB();}
			//Debug.Log ("number of enemy statics " + (PlayerScore.enemiesTotal));
			if ((PlayerScore.enemyStaticsTotal-enemyStaticsDead) < 1) {
				LoadLevel ();	
			}
		}

	}
	

	void GamePause()
	{
		//Debug.Log ("pressed"+ paused);
		uiManager.PausedResume (player);

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
		else if(playerWeaponText){
			playerWeaponText.text = playerWeapon.currentWeapon.GetWeaponName();
		}
		else{
			Debug.LogError("playerWeaponText can't be found (it seems), for some reason");
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
		else if(weaponAmmo.IsReloading()){
			playerAmmoCountText.text = "Reloading";
		}
		else{
			playerAmmoCountText.text = weaponAmmo.GetCurrentAmmo().ToString();
		}

	}

	void LoadLevel()
	{
		//Debug.Log ("Loading level");
		GameObject[] allGOs = GameObject.FindObjectsOfType<GameObject> ();
		winImage.color = Color.Lerp (winImage.color, imagecolour, 1.5f * Time.deltaTime);
		winText.color = Color.Lerp (winText.color, textcolour, 1f * Time.deltaTime);
		//Debug.Log(winImage.color = Color.Lerp(winImage.color, imagecolour ,1.5f * Time.deltaTime));
		//Debug.Log(winText.color = Color.Lerp(winText.color, textcolour ,1f * Time.deltaTime));
		restartTime += Time.deltaTime;
		Spawning.spawnedEnemies = false;//resetting spawnedEnemies for next level
		EnemyAI.alerted = false;// resetting whether the alive enemies are alterted upon level change
		enemyStaticsDead = 0;
		PlayerScore.enemyStaticsTotal = 0;
		if(!ended){
			spawnEndGame.Death (player, true);
			ended = true;
			}
		cleanPhotonObjects();
		if(restartTime >= restartDelay){
			
			winImage.color = zero;
			winText.color  = zero;

			PhotonNetwork.LeaveRoom();
			if(level <1){
				PhotonNetwork.LoadLevel(/*++*/level);
			}
			else{
				PhotonNetwork.LoadLevel(/*--*/level);
				//Debug.Log("level: "+level+"--");
			}
			Debug.Log ("Resetting colours");
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
//	public int returnPlayerTime(){
//		return (int)((timerMin*60)+timerSec);
//	}
}