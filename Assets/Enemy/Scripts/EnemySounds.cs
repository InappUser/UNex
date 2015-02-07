using UnityEngine;
using System.Collections;

public class EnemySounds : MonoBehaviour {
	public AudioClip[] sources;

	private bool hasAwoken = false;
	private int rand=0;
	private AudioSource aSource;
	//private float time =0;
	// Update is called once per frame
	void Awake(){
		aSource = GetComponent<AudioSource> ();
		sources = Resources.LoadAll<AudioClip>("EnemyAwake"); 
		//Debug.Log (sources[0].name);
	}

	void Update () {
		//time += Time.deltaTime;
		if(/*time>10f*/!hasAwoken && EnemyAI.alerted){
			Debug.Log(sources[rand].name);
			hasAwoken = true;
			//time=0;
		}
	}
	public void AssignPlayRand(){
		rand = Random.Range(0,sources.Length);
		aSource.clip = sources[rand];
		aSource.Play();
	}
}
