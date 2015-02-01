using UnityEngine;
using System.Collections;

public class Map_Teleporting : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		Debug.Log ("triggered by "+col.transform.name);
		if(col.transform.name == "FPS_Player(Clone)"){
			Debug.Log("Teleported player");
			col.transform.position = transform.GetChild(0).position;
		}
	}
}
