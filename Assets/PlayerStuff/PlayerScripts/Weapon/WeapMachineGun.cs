using UnityEngine;
using System.Collections;

public class WeapMachineGun : MonoBehaviour {
	public GameObject effect;//
	public float range = 100f;
	public float cooldown = 2f;
	public float damage =20f;//
	public int mouseButtonActive = 0;

	LineRenderer shootLine;
	Vector3 shootLinePlacement;
	float cooldownRemaining = 0f;

	// Use this for initialization
	void Start () {
		shootLine = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		cooldownRemaining -= Time.deltaTime;
		GetShootLinePos ();

		if (Input.GetMouseButton (mouseButtonActive) && cooldownRemaining <= 0) {//using raycast gunshot
			ShootGun ();}//while Mousebutton is held down fire gun


		if(cooldownRemaining<=cooldown * 0.02f)
		{shootLine.enabled = false;}
	}
	void GetShootLinePos()
	{
		shootLinePlacement = Camera.main.transform.position;
		shootLinePlacement.y -= .2f;
		shootLinePlacement.x +=  Camera.main.transform.rotation.x;
	}

	void ShootGun()
	{
		shootLine.enabled = true;
		shootLine.SetPosition (0, shootLinePlacement);
		//turning the line renderer on and setting its initial position to players face
		cooldownRemaining = cooldown;
		Ray ray = new Ray( Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hitInfo; //gets informatino about what ray hit

		if(Physics.Raycast(ray, out hitInfo, range))
		{
			Vector3 hitPoint =hitInfo.point; //is good for effects on what is being hit - will instantiate
			GameObject go = hitInfo.collider.gameObject;
			//Debug.Log ("object: " + go.name);
			//Debug.Log ("point: " + hitInfo.point);
			if(effect != null)//ensuring prefab is assigned before useing
				Instantiate(effect, hitPoint, Quaternion.identity);

			Health h = go.GetComponent<Health>();

			if(h!=null)//making sure that the object has a health script
				h.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.AllBuffered,damage);

			shootLine.SetPosition (1,hitInfo.point);
		}
		else{
			shootLine.SetPosition (1,ray.origin + ray.direction *10f);
		}


	}
}
