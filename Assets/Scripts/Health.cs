using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public float hitPoints = 100f;
	public float currentHitPoints;

	private GameManager spectatorFind;
	private GameObject spectator;
	private GameObject player;
	private Spawning spawn;

	void Update()
	{

	}
	// Use this for initialization
	void Awake () {
		spawn = GameObject.FindObjectOfType<Spawning> ();
		currentHitPoints = hitPoints;
		spectatorFind = (GameManager)GameObject.FindObjectOfType<GameManager> ();
		spectator = spectatorFind.ReturnSpectator ();
	}
	public float GetHealth()
	{
		return currentHitPoints;
	}

	
	// needs to be public bc calling from another script
	[RPC]// allows for method to be called remotely - is a remote procedure call
	public void TakeDamage (float inflicted) {
		currentHitPoints -= inflicted;
		if(currentHitPoints <=0)
		{
			//Debug.Log("executed death");	
			currentHitPoints = 0;
			if(transform.root.gameObject.tag == "Player"){
				spawn.Death(transform.root.gameObject);
			}
			else{
				Die();
			}

		}
	}
	void Die()
	{
		Debug.Log("enemyType: "+gameObject.tag);
		PhotonView pv = PhotonView.Get (this);
		if (gameObject.tag == "EnemyStatic" || gameObject.tag == "EnemyAlive") {
			GameManager.enemyCount --;

			if(gameObject.tag == "EnemyAlive"){
				EnemyAITwo.alerted = true;
				}
		}

		if(GetComponent<PhotonView>().instantiationId==0)
		{
			Destroy(gameObject);//if the gameobject doesn't have a photonview id delete normaly
			PlayerScore.EnemiesLeft --;
		}
		else{
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
		}
	}
}

}