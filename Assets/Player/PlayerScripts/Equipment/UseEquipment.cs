using UnityEngine;
using System.Collections;

public class UseEquipment : MonoBehaviour {
	private InputManager im;
	private EquipmentManager eqManage;
	private GameObject equipment;
	private Animator curWeapAnim;//getting the animator for the current weapon so that the correct animation can be played when animation is used
	private float useCounter = 0f;
	private bool startThrowing = false;
	private bool notVisiable =false;
	private bool thrown = false;

	void Start(){
		im = GameObject.FindObjectOfType<InputManager> ();
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
			if(im.UseEquipment() && useCounter <=0 && eqManage.currentEquipment.GetEquipmentType() == Equipment.EquipmentType.Throw){
				startThrowing = true;
				curWeapAnim.SetTrigger("Throw");
			}

			if (startThrowing && notVisiable) {
				equipment = (GameObject)Instantiate(eqManage.currentEquipment.GetEffect(), transform.position,transform.rotation);
				useCounter = eqManage.currentEquipment.GetUseRate ();
				notVisiable = false;
			}
		}
		useCounter -= Time.deltaTime;
	}
	public void SetAnim(Animator weapAnim){
		curWeapAnim = weapAnim;
		//Debug.Log ("animator chan ged to "+curWeapAnim.gameObject.name);
	}
	public void InstantiateThrowable()
	{
		notVisiable = true;
		Debug.Log ("Throwable instanted");
	}
	public void ThrownThrowable()
	{
		thrown = true;
		//Debug.Log ("Throwable thrown");
	}
}
