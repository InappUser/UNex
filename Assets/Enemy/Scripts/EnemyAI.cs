using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {
	//public float chaseSpeed = 5f;
	public float returnDistance = 10f;
	public float speed = 2f;
	public Transform[] ptrolWayPoints;
	public Animator anim;
	public NavMeshAgent nav;
	public UIManager pauseGame;
	public static bool alerted = false;
	//made true when an enemy dies; in the health script upon the death being and enemy
	
	private GameObject player;
	private bool wasChasing = false;//used to ensure that the enemy is not told to return to spawn over and over again
	private bool chasing = false;
	private bool tooCloseTooLong=false;
	private float tooLong = 4f;
	private float tooLongTimer = 0;

	private float distance;
	private float attackDistance = 1f;
	private float attackTimeCounter =0f;
	private float timeBetweenAttacks =1f;
	private float attackDamage = 20;

	private Transform spawnPoint;
	private GameManager gameManager;
	private bool foundUI = false;
	private float chaseDist; //making chase distance random
	private Vector3 testVect;

	//private EnemySight enemy
	void Start()
	{
		gameManager = GameObject.FindObjectOfType<GameManager> ();
		AssignEnemySpawns ();
		RandomiseChase ();//initially random
	}

	void Update()
	{

		if (FindPlayer () && !pauseGame.paused) {
			if ((distance < chaseDist && alerted)||tooCloseTooLong) {
				chasing = true;/*can't "chasing = distance < foundDistance && Find ()" bc it will == false when unwanted*/
				tooLongTimer = 0f;//resetting timer, so that player once again needs to be too close for too long
			} else if (distance >= returnDistance) {
				if(chasing){
					RandomiseChase ();//when enemy has returned, re-randomise the distance at which chases player
				}
				chasing = false;
			}

			if (chasing && distance < attackDistance) {
				StartCoroutine ( Attack ());
			}else if(distance < attackDistance*2){
				tooLongTimer += Time.deltaTime;
				tooCloseTooLong = tooLongTimer >=tooLong;}//if the timer is more than or equal to too long then tooCloseTooLong = true, else false

			if (chasing && player && player.GetComponent<Health> ().currentHitPoints > 0) {
				Chase ();/*gameObject.GetComponent<PhotonView>().RPC ("Chase",PhotonTargets.AllBuffered);*/
			} else if (!chasing) {
				returnToSpawn ();
				wasChasing = false;//ensuring return to spawn is only run once 
			}
		}
		
		testVect = new Vector3 (transform.position.x,transform.position.y,transform.position.z+ chaseDist);
		Ray ray = new Ray(testVect, transform.position);
		Physics.Raycast(ray,chaseDist);
		Debug.DrawRay (testVect, transform.position, Color.red, 10f);
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
		Debug.Log ("set anim walking to true");
		// Create a vector from the enemy to the last sighting of the player.

		Vector3 sightingDeltaPos =  player.transform.position- transform.position;
		if (sightingDeltaPos.sqrMagnitude >1.5 && player.transform.position.y < transform.position.y+.5f){//ensuring that the player is not chased if too high
			//if 
			nav.destination = player.transform.position;
		}else{
			StartCoroutine(Attack ());
			
			Debug.Log ("set anim walking to false");
		}

		// Set the appropriate speed for the NavMeshAgent.
		nav.speed = speed;

		wasChasing = true;
	}

	IEnumerator Attack()
	{
		anim.SetBool ("isWalking",false);
		anim.SetBool ("Attacking",true);
		//yield return new WaitForSeconds (timeBetweenAttacks);
		Health h = player.GetComponent<Health> ();
		attackTimeCounter += Time.deltaTime;//using this method rather than wait for as this function is executed a over and over
		if (h && h.currentHitPoints>0 && attackTimeCounter >= timeBetweenAttacks) {
			//anim.SetBool("Attacking",true);
			if(PhotonNetwork.offlineMode){
				h.TakeDamage(attackDamage);}//ensuring that, when a new level is loaded within singleplayer, damage can be taken
			else{
				player.GetComponent<PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, attackDamage);//RPC is global method, am invoking it on the photonview componenth.TakeDamage (Time.deltaTime * attackDamage);
			}
			attackTimeCounter = 0f;
			yield return new WaitForSeconds(2);
			anim.SetBool("Attacking",false);
		}
	}

	void returnToSpawn()
	{
		//Vector3 returnDeltaPos = spawnPoint.position - transform.position;

			nav.destination = spawnPoint.position;
			if(Vector3.Distance(transform.position, spawnPoint.position) < 2f)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, spawnPoint.rotation, Time.deltaTime * 1);
				anim.SetBool ("isWalking", false);
			
				Debug.Log ("set anim walking to false");
				//if(anim.GetBool("isWalking") == false)
					//tooCloseTooLong = false;	
			}


	}

	void AssignEnemySpawns()
	{
		
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

	void RandomiseChase()
	{
		chaseDist = Random.Range (3f,7f); //making chase distance random
		chaseDist = Random.Range (3f,chaseDist); //favouring the lower side
	}
	
	

}
