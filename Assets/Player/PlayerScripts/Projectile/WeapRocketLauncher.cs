using UnityEngine;
using System.Collections;

public class WeapRocketLauncher : MonoBehaviour {
	public GameObject ProjectilePrefab;
	public float cooldown = 0.2f;
	public int mouseButtonActive = 1;
	 float cooldownRemaining = 0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		cooldownRemaining -= Time.deltaTime;
		if(Input.GetMouseButton(mouseButtonActive) && cooldownRemaining <=0)
		{//using raycast gunshot
			cooldownRemaining = cooldown;
			Instantiate(ProjectilePrefab, Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.rotation);
		}
			//while Mousebutton is held down fire gun
	}
}
