using UnityEngine;
using System.Collections;

public class WeapSniper : MonoBehaviour {
	public GameObject ricochet;//
	public float damage = 25f;//
	public float fireRate = 0.5f;
	public float reloadTime = 3f;

	float cooldown = 0f;//an instance of the firerate (the firerate being the fixed amount of time between shots)
	int clipCount = 4;
	string clipCountString;
	bool reloading = false;
	Vector3 shootLinePlacement;
	LineRenderer shootLine;

	void Start()
	{
		shootLine = GetComponent<LineRenderer>();
		clipCountString = clipCount.ToString ();
	}

	// Update is called once per frame
	void Update () {
		cooldown -= Time.deltaTime;
		GetShootLinePos ();

		if(Input.GetButton("Fire1"))
		{//player wants to shoot
			Fire ();
		}
		if (!reloading &&(Input.GetKey (KeyCode.R)|| clipCount <=0)) {//if the player is not already reloading and their clip has run out or they press 'r'
			StartCoroutine (Reload());//if they arn't already reloading then reload
			reloading = true;//say are reloading so is only executed once
		}
	}

	void GetShootLinePos()
	{
		shootLinePlacement = Camera.main.transform.position;
		shootLinePlacement.y -= .2f;
		shootLinePlacement.x +=  Camera.main.transform.rotation.x;
	}

	void Fire(){
		//checking to see if the player is able to fire again
		if(cooldown >0){
			shootLine.enabled = false;
			return;
			//can't shoot before cooldown
		}
		//checking to see if the player has any ammo in clip
		if (clipCount <= 0) {//if no ammo then dont allow to shoot (return)
			return;
		}

		clipCount--;
		clipCountString = clipCount.ToString();//converting int to string so that "reloading" string can be used while reloading
		shootLine.enabled = true;
		shootLine.SetPosition (0,shootLinePlacement);

		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		//ray needs both position and drection
		RaycastHit hitInfo = FindClosestHitInfo(ray);
		//gathering information regaring what was hit by ray
		if(hitInfo.collider != null){
			Vector3 hitPoint = hitInfo.point;//this is required, will error if try putting hitInfo.point directly in instantiation

			Instantiate(ricochet, hitPoint, Quaternion.identity);
			Health h = hitInfo.transform.GetComponent<Health>();

			//causing a lot of issues regarding the assignment of transform
			//while(h == null && hitInfo.transform.parent){
			//	hitInfo.transform.Equals(hitInfo.transform.parent);//if the thing hit doesn't have health, maybe its parent does
			//	h = hitInfo.transform.GetComponent<health>();
				//looping until item has health
			//}//will run out of parents or find thing with health
			//keep in mind that hitinfo may have changed by this point

			if(h!=null){
				//h.TakeDamage(damage); //old call, new involves using the RPC
				if(h.GetComponent<PhotonView>() == null){
					Debug.Log("No PhotonView component added to this object");
				}else{
					h.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.AllBuffered,damage);
				}
			}//sending to everyone who is connected
			//https://www.youtube.com/watch?v=AtzlbjhfoRQ&list=PLbghT7MmckI7BDIGqNl_TgizCpJiXy0n9&index=12
			//7:07 for explanation
			shootLine.SetPosition(1,hitInfo.point);
		}
		else{
			shootLine.SetPosition(1,ray.origin+ray.direction*100f);
		}
		cooldown = fireRate;
		//reseting cooldown
	}

		//function for returning not player
	RaycastHit FindClosestHitInfo(Ray ray){
		RaycastHit[] hits = Physics.RaycastAll (ray);
		//all means that every item in raycasts way is returned, not just first thing 
		//- solving player hitting self
		RaycastHit closestHit = new RaycastHit();
		float distance = 0f;
		foreach(RaycastHit hit in hits){
			if(hit.transform != this.transform && (closestHit.collider==null || hit.distance < distance)){//if not the player
			//we have hit 1) the first thing
			//2) not us 
			//3) 2 r the thing closest
				closestHit = hit;
				distance = hit.distance;
			}
		}
		//closest hit is now either still null or it contains the clostest valid hit
		return closestHit;
	}
	

	IEnumerator Reload()//wait for seconds reequires an ienumerator return type
	{
		clipCountString = "Reloading";
		yield return new WaitForSeconds (reloadTime);
		clipCount = 4;
		clipCountString = clipCount.ToString ();
		reloading = false;
	}

	public string GetClipCount()
	{
		return clipCountString;
	}
}
