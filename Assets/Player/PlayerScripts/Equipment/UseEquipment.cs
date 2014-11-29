using UnityEngine;
using System.Collections;

public class UseEquipment : MonoBehaviour {
	private EquipmentManager eqManage;
	private GameObject equipment;
	private float useCounter = 0f;
	// Use this for initialization

	void Start()
	{

	}

	void FixedUpdate()
	{

		if (transform != null && equipment && eqManage.currentEquipment.GetEquipmentType() == Equipment.EquipmentType.Throw) {
			equipment.transform.Translate(equipment.transform.forward * eqManage.currentEquipment.GetThrowSpeed() * Time.deltaTime, Space.World);
		}
	}
	// Update is called once per frame
	void Update () {
		if(eqManage == null)
		{
			eqManage = transform.GetComponent<EquipmentManager> ();
		}

		else{//making sure is right clicking, enough time has passed and not the map before throwing 
			if (Input.GetButtonDown ("Fire2") && useCounter <=0 && eqManage.currentEquipment.GetEquipmentType() == Equipment.EquipmentType.Throw) {
				equipment = (GameObject)Instantiate(eqManage.currentEquipment.GetEffect(), transform.position,transform.rotation);
				useCounter = eqManage.currentEquipment.GetUseRate ();
			}
		}
		useCounter -= Time.deltaTime;
	}
}
