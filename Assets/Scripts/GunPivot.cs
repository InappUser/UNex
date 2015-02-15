using UnityEngine;
using System.Collections;

public class GunPivot : MonoBehaviour {

    public Color gizmoColour = Color.yellow;
    public float gizmoSize = .09f;

	private float rotLerpAmount = .5f;

	void OnDrawGizmos () {
		Gizmos.color = gizmoColour;
		Gizmos.DrawWireSphere (transform.position, gizmoSize);
	}
	void Update()
	{
		if(Camera.main){
			transform.position = Camera.main.transform.position;
			if(transform.name == "GunPivot"){
			    transform.rotation = Quaternion.Slerp(transform.rotation,Camera.main.transform.rotation,rotLerpAmount);
			}else{
				transform.rotation = Camera.main.transform.rotation;
			}
		}
	}
}
