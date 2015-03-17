using UnityEngine;
using System.Collections;

public class EnemySounds : MonoBehaviour {
	private AudioSource aSource;
	private AudioClip[] clips;
	private bool hasAwoken = false;
	private int rand=0;

	//private float time =0;
	// Update is called once per frame
	void Awake(){
		aSource = GetComponent<AudioSource> ();
		clips = Resources.LoadAll<AudioClip>("EnemyAwake"); 
		//Debug.Log (sources[0].name);
	}

	void Update () {
		//time += Time.deltaTime;
		if(/*time>10f*/(!hasAwoken && EnemyAI.alerted) || Input.GetKey(KeyCode.CapsLock)){
			//Debug.Log(clips[rand].name);
			AssignPlayRand();
			hasAwoken = true;
			//time=0;
		}
	}
	public void AssignPlayRand(){
		rand = Random.Range(0,clips.Length);
		aSource.clip = clips[rand];
		aSource.Play();
	}
}
