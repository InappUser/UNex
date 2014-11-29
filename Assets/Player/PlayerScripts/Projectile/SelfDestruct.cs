using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {
	public float duration = 1f;

	// Update is called once per frame
	void Update () {
		if(duration <=0)
		{
			Destroy(transform.gameObject);
		}
		duration -= Time.deltaTime;
	}

}
