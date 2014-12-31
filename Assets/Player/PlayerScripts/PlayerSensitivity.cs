using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSensitivity : MonoBehaviour {
	private GameObject player;
	private bool foundplayer = false;
	private bool setInitPlayerSens=false;

	// Use this for initialization
	void Start()
	{
		if (gameObject.name == "ScrlSensX" &&PlayerPrefs.GetFloat ("SensitivityX")!=0f) {
			gameObject.GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat ("SensitivityX"); 		
		}
		if (gameObject.name == "ScrlSensY" && PlayerPrefs.GetFloat ("SensitivityY") !=0f) {
			gameObject.GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat ("SensitivityY"); 		
		}
	}
	void Update () {
		if (!foundplayer) {
			player = GetPlayer();		
		} else if(!setInitPlayerSens)
		{
			Debug.Log("hitting");
			player.GetComponent<MouseLook>().sensitivityX = PlayerPrefs.GetFloat ("SensitivityX");
			player.transform.GetChild(0).GetComponentInChildren<MouseLook>().sensitivityY = PlayerPrefs.GetFloat ("SensitivityY");
			setInitPlayerSens =true;
		}
		Debug.Log("Saved X: "+PlayerPrefs.GetFloat ("SensitivityX"));
		Debug.Log("Saved Y: "+PlayerPrefs.GetFloat ("SensitivityY"));	
	}
	
	// Update is called once per frame
	GameObject GetPlayer()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		for (int i=0; i<players.Length; i++) {
			if(players[i].GetComponent<PhotonView>().isMine)
			{
				foundplayer = true;
				return players[i];
			}
		}
		Debug.LogError ("Machine's player could not be found!");
		return players [0];//wont need to be executed, every machine will have a player
	}

	public void ChangeX(float newX)
	{
		if(foundplayer){//public function executed by the scrollbars in teh pause menu options used to change the player sensitivity;
			player.GetComponent<MouseLook>().sensitivityX = (newX*15f);
			PlayerPrefs.SetFloat("SensitivityX",(newX*15f));//saving the sensitivity whenever is changed
			//Debug.Log ("assigned new x:"+(newX*15f));
		}

	}

	public void ChangeY(float newY)
	{
		if(foundplayer){
			player.transform.GetChild(0).GetComponentInChildren<MouseLook>().sensitivityY = (newY*10f);
			PlayerPrefs.SetFloat("SensitivityY",(newY*10f));//saving the sensitivity whenever is changed
			//Debug.Log ("assigned new y:"+(newY*10f));
		}

	}

}
