using UnityEngine;
using System.Collections;

public class RocketEngine : MonoBehaviour {
	public float speed = 10f;
	public GameObject thruster;

	private GameObject thrusterInstance;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate (transform.forward * speed * Time.deltaTime, Space.World);//making the rocket relative to
		//world as oposed to relative to self, with the space.world

	}
	void Update()
	{

			thrusterInstance = (GameObject)Instantiate (thruster, transform.FindChild ("RocketThruster").transform.position, Quaternion.identity);
		if (thruster.transform.parent) {
			thruster.transform.parent = transform.FindChild("RocketThruster").transform;/*making the thruster stick to the */	}	
				

	}
}
