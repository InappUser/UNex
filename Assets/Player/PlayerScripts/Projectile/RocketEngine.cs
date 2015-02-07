using UnityEngine;
using System.Collections;

public class RocketEngine : MonoBehaviour {
	public GameObject thruster;

	private float speed = 20f;
	private float time;
	private float speedIncrease;
	private bool increased =false;
	private bool normalised =false;

	private GameObject thrusterInstance;
	// Use this for initialization
	void Start () {
		Instantiate (thruster, transform.FindChild ("RocketThruster").transform.position, Quaternion.identity);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//thrusterInstance = (GameObject)
		transform.Translate (transform.forward * speed * Time.deltaTime, Space.World);//making the rocket relative to
		//world as opposed to relative to self, with the space.world
		time += Time.deltaTime;
		if(time >= .2f && !increased)
		{
			speed = speed*4;
			increased = true;
			//Debug.Log("speed increase");
		}
		if(time >= .4f && !normalised)
		{
			speed = speed*.7f;
			normalised = true;
			Debug.Log("speed increase");
		}
	}

}
