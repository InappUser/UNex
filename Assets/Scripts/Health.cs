using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public float hitPoints = 100f;
	public float currentHitPoints;

	private GameManager gm;
	private GameObject spectator;
	private GameObject player;
	private Spawning spawn;
	private PlayerScore ps;
	private bool alreadyDead = false;
	
	private float eHeadHealthDamage = 1.5f;
	private float eTorsoHealthDamage = 1f;
	private float eArmsHealthDamage = .6f;
	private float eLegsHealthDamage = .5f;

	void Update()
	{

	}
	// Use this for initialization
	void Awake () {
		spawn = GameObject.FindObjectOfType<Spawning> ();
		currentHitPoints = hitPoints;
		gm = (GameManager)GameObject.FindObjectOfType<GameManager> ();
		spectator = gm.ReturnSpectator ();
		ps = GameObject.FindObjectOfType<PlayerScore> ();//can use this simple search method as only one instance of the PlayerScore class exists in a players game
	}
	public float GetHealth()
	{
		return currentHitPoints;
	}
	public float GetTotalHealth()
	{
		return hitPoints;
	}
	
	// needs to be public bc calling from another script
	[RPC]// allows for method to be called remotely - is a remote procedure call
	public void TakeDamage (string hitGOName, float inflicted) {
		
		currentHitPoints -= CaluclateEnemyHealth(hitGOName, inflicted);
		if(currentHitPoints <=0 && !alreadyDead)
		{
			alreadyDead = true;
			//Debug.Log("executed death");	
			currentHitPoints = 0;
			if(transform.root.gameObject.tag == "Player"){
				spawn.Death(transform.root.gameObject, false);
				alreadyDead = false;
			}
			else{
				Die();
			}

		}
	}
	void Die()
	{
		//Debug.Log("enemyType: "+gameObject.tag);
		PhotonView pv = PhotonView.Get (this);
		if (gameObject.tag == "EnemyStatic") {
			GameManager.enemyStaticsDead ++;
			ps.AddStaticsKiled();
			//Debug.Log("hasRun");
		}
		if(gameObject.tag == "EnemyAlive"){
			EnemyAI.alerted = true;
			ps.AddAlivesKiled();
		}

		if(GetComponent<PhotonView>().instantiationId==0)
		{
			Destroy(gameObject);//if the gameobject doesn't have a photonview id delete normaly
		}
		else if(GetComponent<PhotonView>()){
			if(gameObject !=null && pv.isMine){//if gameobject is instantiated by photon destroy this way
				//gameObject.GetComponent<WeaponManager>().enabled = false;
				//gameObject.GetComponent	<Shoot>().enabled = false;//futureproofing

				try{
					PhotonNetwork.Destroy(gameObject);
				}//and make sure only one thing can destroy it (using master)
				catch{
				Debug.Log("can't destroy "+gameObject.name+" for some reason");
				}
			}
			if (transform.name == "FPS_Player(Clone)") {
				spectator.SetActive (true);
			}else{
				//Debug.Log("weird killing error.");
			}
		}
	}

	float CaluclateEnemyHealth(string hitGOName, float inflicted){
		if(hitGOName == "HitBoxLegL" || hitGOName == "HitBoxLegR"){
			return inflicted * eLegsHealthDamage;
		}
		else if(hitGOName == "HitBoxArmL" || hitGOName == "HitBoxArmR"){
			return inflicted * eArmsHealthDamage;
		}
		else if(hitGOName == "HitBoxTorso"){
			return inflicted * eTorsoHealthDamage;
		}
		else if(hitGOName == "HitBoxHead"){
			return inflicted * eHeadHealthDamage;
		}
		else{
			return inflicted;
		}
	}

	public void RestetHP(){
		currentHitPoints = hitPoints;

	}

}