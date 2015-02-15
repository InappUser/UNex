using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	private AudioSource[] aSources;
	private AudioClip[] clips;
	private NetworkManager nm;
	private bool hasStarted = false;
	// Use this for initialization
	void Start () {
		nm = GameObject.FindObjectOfType<NetworkManager> ();
		aSources = GetComponents<AudioSource> ();
		clips = Resources.LoadAll<AudioClip>("Background"); 
		aSources [0].clip = clips [0];
		aSources [1].clip = clips [0];
		aSources [0].pitch = .76f;
		aSources [1].pitch = .76f;
	}
	
	// Update is called once per frame
	void Update () {
		if(nm.gameStarted() && !hasStarted) {
			//Debug.Log("playing clips");
			aSources[0].Play();
			aSources[1].Play();
			hasStarted = true;
		}else{
			//Debug.Log("not started, name "+nm.transform.name);
		}
	}
}
