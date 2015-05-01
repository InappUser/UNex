using UnityEngine;
using System.Collections;

public class MoveMapPlayer : MonoBehaviour {
	public GameObject mapSize;

	private GameObject player;
	private Vector3 mapPlayerTransChange;
	private Vector3 playerPrevPos;


	// Use this for initialization
	void Awake () {

		player = GameObject.FindGameObjectWithTag ("Player");
		playerPrevPos = new Vector3();
		mapPlayerTransChange = new Vector3 ();
		//playerPrevPos = player.transform.pos;
	}
	
	// Update is called once per frame
	void Update () {
		mapPlayerTransChange.x = transform.localPosition.x + (player.transform.position.x - playerPrevPos.x)*mapSize.transform.localScale.x;
		mapPlayerTransChange.y = transform.localPosition.y + (player.transform.position.z - playerPrevPos.z)*mapSize.transform.localScale.z;
		//mapPlayerTransChange.z = transform.localPosition.z + (player.transform.position.z - playerPrevPos.z)*mapSize.transform.localScale.z;//was y, changed to z bc of orientation differences in map
		//getting the distance that the player has moved and then timesing by the size of the map
		//need to swap make the y equal to the z scale (z is the depth of the model and y is the depth of the ball)
		transform.localPosition = mapPlayerTransChange ;
		playerPrevPos = player.transform.position;

	}
}
