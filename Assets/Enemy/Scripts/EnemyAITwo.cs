using UnityEngine;
using System.Collections;

public class EnemyAITwo : MonoBehaviour {
	//public float chaseSpeed = 5f;
	public float foundDistance = 5f;
	public float returnDistance = 10f;
	public float attackDistance = .5f;
	public float speed = 2f;
	public Transform[] ptrolWayPoints;
	public Animator anim;
	public NavMeshAgent nav;
	public static bool alerted = false;
	//made true when an enemy dies; in the health script upon the death being and enemy



	private GameObject pauseUI;
	private GameObject player;
	private bool chasing = false;
	private float distance;
	private float attackTimeCounter =0f;
	private float timeBetweenAttacks =.2f;
	private float attackDamage = 2;
	private Transform spawnPoint;
	private GameManager gameManager;
	private PauseUI pauseGame;

	//private EnemySight enemy
	void Start()
	{
		gameManager = GameObject.FindObjectOfType<GameManager> ();
		pauseGame = gameManager.ReturnPauseUI();
		GameObject[] spawns = GameObject.FindGameObjectsWithTag("EnemySpawn");//doesn't like getting the transform of the objects
		float[] enemyToSpawn = new float[spawns.Length];
		float shortestDistance = 100f;//initialising to 100 bc will definitely be larger than the distance from any of the spawns
		//int random;

		//float e1dist1 = Vector3.Distance (transform.position, spawns [0].transform.position);
		//float e1dist2 = Vector3.Distance (transform.position, spawns [1].transform.position);

		for(int i=0;i<spawns.Length;i++)
		{//allocating the spawn of the shortest distance to and instance of an enemy
			enemyToSpawn[i] = Vector3.Distance(transform.position, spawns [i].transform.position);
			if(enemyToSpawn[i] < shortestDistance)
			{
				shortestDistance = enemyToSpawn[i];
				spawnPoint = spawns[i].transform;
			}
		}

	}

	void Update()
	{

		if(FindPlayer() && !pauseGame.paused && alerted){
			if(distance < foundDistance)
			{chasing = true;/*can't "chasing = distance < foundDistance && Find ()" bc it will == false when unwanted*/}
			else if(distance >= returnDistance){
				chasing = false;}
			if(distance < attackDistance){
				Attack ();}
			if (chasing && player && player.GetComponent<Health>().currentHitPoints >0) {
				Chase ();/*gameObject.GetComponent<PhotonView>().RPC ("Chase",PhotonTargets.AllBuffered);*/}
			else if(!chasing){
				returnToSpawn();}
		}

	}

	bool FindPlayer()
	{
		if(player != null)
		{
			distance = Vector3.Distance (transform.position, player.transform.position);
			return true;
		}
		player = GameObject.FindGameObjectWithTag ("Player");
		return false;

	}
	[RPC]//remote procedure call so that movement of enemies can be synced, similar to enemy destruction
	void Chase ()
	{
		anim.SetBool ("isWalking",true);
		// Create a vector from the enemy to the last sighting of the player.

		Vector3 sightingDeltaPos =  player.transform.position- transform.position;

		if (sightingDeltaPos.sqrMagnitude > 4f)
			//if 
			nav.destination = player.transform.position;

		// Set the appropriate speed for the NavMeshAgent.
		nav.speed = speed;
	}
	void Attack()
	{
		Health h = player.GetComponent<Health> ();
		attackTimeCounter += Time.deltaTime;
		if (h && h.currentHitPoints>0 && attackTimeCounter >= timeBetweenAttacks) {
			player.GetComponent<PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, attackDamage);//RPC is global method, am invoking it on the photonview componenth.TakeDamage (Time.deltaTime * attackDamage);
			Debug.Log("hit");
			attackTimeCounter = 0f;
		}
	}

	void returnToSpawn()
	{
		Vector3 returnDeltaPos = spawnPoint.position - transform.position;

			nav.destination = spawnPoint.position;
			if(returnDeltaPos.sqrMagnitude < 1f)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, spawnPoint.rotation, Time.deltaTime * 1);
				anim.SetBool ("isWalking", false);
			}
	}
	
	

}
