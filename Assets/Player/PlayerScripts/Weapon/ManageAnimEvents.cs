using UnityEngine;
using System.Collections;

public class ManageAnimEvents : MonoBehaviour {
	UseEquipment commUseEquipment;
	// Use this for initialization
	void Start () {
		commUseEquipment = gameObject.transform.root.GetComponentInChildren<UseEquipment> ();
		//Debug.Log ("this class is actually used");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void InstantiateThrowable()
	{
		commUseEquipment.InstantiateThrowable ();
		Debug.Log ("Throwable instanted");

	}
}
