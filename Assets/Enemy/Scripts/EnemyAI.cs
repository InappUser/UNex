using UnityEngine;
using System.Collections;

public class EnemyAI : Photon.MonoBehaviour {
	//public float chaseSpeed = 5f;
	public float speed = 2f;
	public Animator anim;
	public NavMeshAgent nav;
	public UIManager pauseGame;
	public static bool alerted = false;
	//made true when an enemy dies; in the health script upon the death being and enemy
	
	private GameObject player;
	private Vector3 sightingDeltaPos;//the distance between the player and the enemy
	private bool returnedToSpawn = false;
	private bool wasChasing = false;//used to ensure that the enemy is not told to return to spawn over and over again
	private bool chasing = false;
	private bool tooCloseTooLong=false;
	private float tooLong = 4f;
	private float tooLongTimer = 0;
	
	private float distance;
	private float attackDistance = 1f;
	private float attackTimeCounter =0f;
	private float timeBetweenAttacks =.5f;
	private float attackDamage = 2;
	private bool hitPlayer = false;
	
	private Transform spawnPoint;
	//private bool foundUI = false;
	private float chaseDist; //making chase distance random -- this is how far away the player can be from the enemy and still have the enemy attempt to chase them
	private float returnDist = 10f;//this is the distance from the spawn point that the enemy will get before they will return to spawn and give up chasing the player
	private NavMeshPath navPath;//Used to check if the path between the enemy and the player is navigable
	private EnemyJumping jump;//used to allow the enemy to go to a jumping position and jumping
	
	//private EnemySight enemy
	void Start()
	{
		navPath = new NavMeshPath ();
		jump = new EnemyJumping(gameObject);

		AssignEnemySpawns ();
		RandomiseDist ();//initially random
	}
	
	void Update()
	{
		if (FoundPlayer () && !pauseGame.paused) {
			//if the player is too close
			if(distance < attackDistance*2){//if the above is not met and they player is too far away, then reset tooCloseTooLong
				tooLongTimer += Time.deltaTime;
				tooCloseTooLong = tooLongTimer >=tooLong;}//if the timer is more than or equal to too long then tooCloseTooLong = true, else false
			
			//calculating if a chase is eligable
			if ((distance < chaseDist && alerted)||tooCloseTooLong) {//chasing the player if they get too close and are alerted, or got too close for too long
				SetChasing(true);//chasing = true;/*can't "chasing = distance < foundDistance && Find ()" bc it will == false when unwanted*/
				tooLongTimer = 0f;//resetting timer, so that player once again needs to be too close for too long
			}else if (distance > returnDist) {//when the enemy will return to their spawn
				SetChasing(false);//chasing = false;
			}
			
			//Chasing or Returning to spawn
			if (chasing && /*player &&*/ player.GetComponent<Health> ().currentHitPoints > 0) {//if found the player, they aren't dead and are supposed to chase, chase
				Chase ();/*gameObject.GetComponent<PhotonView>().RPC ("Chase",PhotonTargets.AllBuffered);*/
			} else if (!chasing && !returnedToSpawn) {
				returnToSpawn ();
				wasChasing = false;//ensuring return to spawn is only run once 
			}
		}
		
	}
	
	bool FoundPlayer()
	{
		if(player != null)
		{
			distance = Vector3.Distance (spawnPoint.transform.position, player.transform.position);
			return true;
		}
		player = GameObject.FindGameObjectWithTag ("Player");
		return false;
		
	}
	
	[RPC]//remote procedure call so that movement of enemies can be synced, similar to enemy destruction
	void Chase ()
	{
		nav.speed = speed;// Set the appropriate speed for the NavMeshAgent.
		wasChasing = true;
		// Create a vector from the enemy to the last sighting of the player.
		sightingDeltaPos =  player.transform.position- transform.position;
		
		anim.SetBool ("isWalking",true);//making enemy walk if they are chasing player
		nav.CalculatePath (player.transform.position, navPath);//getting the path between the player and the enemy - only used for checking if the player is in an unreachable spot
		
		if(navPath.status == NavMeshPathStatus.PathPartial){//if the player is out of reach, find jump spot and get to them
			if(!jump.Jump(player)) //keep doing this until the "Jump" method in jump returns true, at which point the chase can resume
				//Debug.Log("trying to jump");//essentially:keep trying to execute this method until it returns true
				return;
		} else if(navPath.status == NavMeshPathStatus.PathInvalid){
			Debug.Log("navPath is invalid");
		}
		
		
		if (sightingDeltaPos.sqrMagnitude >10.5f){//if they are too far to attack, just chase
			nav.destination = player.transform.position;
			//Debug.Log("chasing player");
			anim.SetBool("Attacking",false);
		}
		else{
			//Debug.Log("attacking player");
			StartCoroutine(Attack ());}
	}
	
	IEnumerator Attack()
	{
		anim.SetBool ("Attacking",true);
		returnedToSpawn = false;//resetting so that they can return to spawn again
		//yield return new WaitForSeconds (timeBetweenAttacks);	
		Health h = player.GetComponent<Health> ();
		attackTimeCounter += Time.deltaTime;//using this method rather than wait for as this function is executed a over and over

		if (sightingDeltaPos.sqrMagnitude <1.5f){
			anim.SetBool ("isWalking",false);
			nav.destination = transform.position;//stopping the enemy when they get close enough to the player
		}else{
			nav.destination = player.transform.position;
		}

		
		if (h && h.currentHitPoints>0 && attackTimeCounter >= timeBetweenAttacks && hitPlayer) {
			hitPlayer = false;
			if(PhotonNetwork.offlineMode){
				h.TakeDamage(player.name, attackDamage);}//ensuring that, when a new level is loaded within singleplayer, damage can be taken
			else{
				player.GetComponent<PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, player.name, attackDamage);//RPC is global method, am invoking it on the photonview componenth.TakeDamage (Time.deltaTime * attackDamage);
			}
			attackTimeCounter = 0f;
			yield return new WaitForSeconds(1);
			anim.SetBool("Attacking",false);
		}
	}
	
	void returnToSpawn()
	{
		anim.SetBool ("Attacking", false);//ensuring that attack animation has stopped
		nav.destination = spawnPoint.position;
		if(Vector3.Distance(transform.position, spawnPoint.position) < 2f)
		{
			//Debug.Log("hit at all");
			transform.rotation = Quaternion.Slerp (transform.rotation, spawnPoint.rotation, Time.deltaTime);
			anim.SetBool ("isWalking", false);
			//Debug.Log("rot of enemy"+transform.eulerAngles+ " rot of spawn"+spawnPoint.eulerAngles);
			if(transform.eulerAngles == spawnPoint.eulerAngles){//checking to see if they have finished turning around -- eulerAnlges are essentially a normalised version of rotation (rotation won't work in this scenario)
				RandomiseDist ();//when enemy has returned, re-randomise the distance at which chases player
				returnedToSpawn = true;//so that this function is not run unnecessarily
			}
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
	
	void RandomiseDist()
	{
		chaseDist = Random.Range (3f,7f); //making chase distance random
		chaseDist = Random.Range (3f,chaseDist); //favouring the lower side
		
		returnDist = Random.Range (15f,20f); //also making return distance random
		returnDist = Random.Range (returnDist,22f); //favouring the higher side
		
		//Debug.Log ("chase distance for "+spawnPoint.transform.position+" is: "+chaseDist+", return distance is: "+returnDist);
		
	}
	void SetChasing(bool isChasing){
		chasing = isChasing;
		GetComponent<EnemyNetworking> ().SetChasing (isChasing);
	}
	public void SetIsChasingAndPlayer(GameObject newPlayer){
		chasing = true;
		player = newPlayer;
	}
	public int GetPlayerID(){
		return player.GetComponent<PhotonView>().viewID;
	}
	public void Hit()//this function is referenced within an "event" within the enemy alive's attack animation (when the enemies hand hits the [player)
	{//meaning that the player will take damage when they appear to be hit
		hitPlayer = true;
	}


	
	
	
}
