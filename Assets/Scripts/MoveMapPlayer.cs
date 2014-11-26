using UnityEngine;
using System.Collections;

public class MoveMapPlayer : MonoBehaviour {
	public GameObject mapSize;

	private GameObject player;
	private Vector3 MapPlayerTransChange;
	private Transform MapPlayerTransform;
	private Vector3 playerPrevPos;


	// Use this for initialization
	void Awake () {

		player = GameObject.FindGameObjectWithTag ("Player");
		MapPlayerTransform = new GameObject ().transform;
		playerPrevPos = new Vector3();
		MapPlayerTransChange = new Vector3 ();
		//playerPrevPos = player.transform.pos;
	}
	
	// Update is called once per frame
	void Update () {
		MapPlayerTransChange.x = transform.localPosition.x + (player.transform.position.x - playerPrevPos.x)*mapSize.transform.localScale.x;
		MapPlayerTransChange.y = transform.localPosition.y + (player.transform.position.y - playerPrevPos.y)*mapSize.transform.localScale.z;
		MapPlayerTransChange.z = transform.localPosition.z + (player.transform.position.z - playerPrevPos.z)*mapSize.transform.localScale.y;
		//getting the distance that the player has moved and then timesing by the size of the map
		//need to swap make the y equal to the z scale (z is the depth of the model and y is the depth of the ball)
		transform.localPosition = MapPlayerTransChange ;
		playerPrevPos = player.transform.position;

	}
}
