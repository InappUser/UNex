using UnityEngine;
using System.Collections;

public class EnemyJumping{

	private GameObject[] jumpStarts; //all of the levels jump spots
	private GameObject currentJumpSpot;
	private GameObject myEnemy;
	private Vector3 playerPos;
	private NavMeshAgent enemyNav;
	private Animator enemyAnim;

	private float timeBetweenFinds = 5f;
	private float counter;
	private bool hasJumped = false;


	public EnemyJumping (GameObject enemy) {
		myEnemy = enemy;
		enemyNav = enemy.GetComponent<NavMeshAgent> ();
		enemyAnim = enemy.GetComponent<Animator> ();
		jumpStarts = GameObject.FindGameObjectsWithTag("EnemyJumpStart");
		counter=timeBetweenFinds;//so find runs first time and tehn waits
	}

	public bool Jump(GameObject player){
		playerPos = player.transform.position;
		counter += Time.deltaTime;
		//Debug.Log ("count is "+counter);
		if(enemyNav.enabled && counter>=timeBetweenFinds){
			enemyNav.SetDestination (FindClosestJump());
			counter = 0;//optimising - ensuring that searches only occur every 5 seconds
			//Debug.Log("enemy heading to "+currentJumpSpot.transform.position);
		}
		if(Vector3.Distance(myEnemy.transform.position, enemyNav.destination) < .1){
			MakeTransition();
		}
		else{
			return false;
		}
		return true;
	}

	Vector3 FindClosestJump(){//need to take into account the player jumping from one elevated position to another - see if namesh path is partial (cant go there) and then, if so, tempereroly remove it from options (have array of "cant get tos" that is cleared upon every jump) and then use this to exlude jump spots in this funciton
		if(hasJumped == false){ //if they haven't jumped yet, then go to the jump spot, later they will land on the jumpspot's child
			currentJumpSpot = jumpStarts[0];
			Debug.Log("player position: "+playerPos);
			foreach (GameObject spot in jumpStarts) {
				//Debug.Log("option: "+spot+" at "+spot.transform.position);
				if((Vector3.Distance(playerPos,spot.transform.position) < Vector3.Distance(playerPos,currentJumpSpot.transform.position))/*&& enemyNav.pathStatus == NavMeshPathStatus.PathComplete*/){// if the distance between the new spot and the player is smaller then that of the player and the current closest spot, then make the new spot the closest spot
					currentJumpSpot = spot; //means that the enemy will go to the closest jumping spot to the player
				}
				//Debug.Log("selection: "+currentJumpSpot+" at "+currentJumpSpot.transform.position);
			}


		}else{//if they have jumped , then go to the jumpspot's child, later they will land on the jumpspot
			currentJumpSpot = jumpStarts[0].transform.GetChild(0).gameObject;
			foreach (GameObject spot in jumpStarts) {//thing is: the jumpspot closest to the player is not necisarily what makes sense for the enemy to "land on"- could go to closest to enemy but feel as though the teleporting makes for an interesting mechanic
				if((Vector3.Distance(playerPos,spot.transform.GetChild(0).position) < Vector3.Distance(playerPos,currentJumpSpot.transform.position))/*&& enemyNav.pathStatus == NavMeshPathStatus.PathComplete*/){// if the distance between the new spot and the player is smaller then that of the player and the current closest spot, then make the new spot the closest spot
					currentJumpSpot = spot.transform.GetChild(0).gameObject; //means that the enemy will go to the closest jumping spot to the player -- am also checking to see if the path is navigable
				}
			}

		}
		return currentJumpSpot.transform.position;
	}

	void MakeTransition(){
		enemyNav.enabled = false;//if the nav mesh is enabled, then the enemy won't be able to move off of it (wont be able to move on the y)
		if(hasJumped){
			myEnemy.transform.position = currentJumpSpot.transform.parent.position;
			hasJumped=false;
		}else if(hasJumped ==false){
			myEnemy.transform.position = currentJumpSpot.transform.GetChild(0).position;
			hasJumped=true;
		}
		//hasJumped= hasJumped? false : true; //if true then =false, otherwise equal true
		counter = timeBetweenFinds;//resetting the counter after every jump so that find is executed the first time, upon every iteration
		enemyNav.enabled = true;
	}



}
