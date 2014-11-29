using UnityEngine;
using System.Collections;

public class NetworkViewOffOn : Photon.MonoBehaviour {
	public GameObject my;
	public GameObject other;
	// Use this for initialization
	void Start () {
		//need to include a network view component in order to work

		if(photonView.isMine){

			my.SetActive(true);
			other.SetActive(false);
			//my.GetComponentInChildren<>
		}
		else{
			other.SetActive (true);
			my.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
