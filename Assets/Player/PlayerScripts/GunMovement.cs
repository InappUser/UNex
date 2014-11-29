using UnityEngine;
using System.Collections;

public class GunMovement : MonoBehaviour {
	public GameObject inheritMovementFrom;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ( gunPos =Camera.main.transform.rotation.z);
		if(Camera.main != null){
			//transform.RotateAround(Camera.main.transform.position,Vector3.up, -25 * Time.deltaTime);
				//inheritMovementFrom.transform.rotation;
		}
	}
//	Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
//	{
//		Vector3 direction = point - pivot; //getting the direction relative to the pivot
//		direction = Quaternion.Euler (angles) * direction;//rotating the point
//		point = direction + pivot; //getting the point of rotation
//		return point;
//	}
}
