using UnityEngine;
using System.Collections;

public class Detonation : MonoBehaviour {
	public GameObject explosionPrefab;
	public float damage = 2000f;
	public float explosionRadius = 100f;
	public float killDistance = 4f;
	//remeber- these are public and are overriden by values set in unity
	void OnTriggerEnter(Collider col)
	{
		if(col.transform.name == "FPS_Player(Clone)" && col.transform.GetComponent<PhotonView>().isMine){//if is the player and the player is owned by the client, the dont do damage
			Debug.Log("did not run");
			return;
		}else{
			Detonate ();
		}
	}
	void OnCollisionEnter(Collision col)
	{
		if(col.transform.name == "FPS_Player(Clone)" && col.transform.GetComponent<PhotonView>().isMine){//if is the player and the player is owned by the client, the dont do damage
			Debug.Log("did not run");
			return;
		}else{
			Detonate ();
		}
	}
	/*void FixedUpdate()
	{
		Ray ray = new Ray (transform.position, transform.forward);
		Physics.Raycast (ray/*, speed * Time.DeltaTime*//*);
	}only need to worry about if is going really fast, though probably want to use raycasts anyway*/ 
	void Detonate()
	{
		if(explosionPrefab !=null)//checking to ensure prefab has been assigned
		{
			Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		}
		

		Collider[] colliders = Physics.OverlapSphere (transform.position, explosionRadius);
		foreach(Collider c in colliders)
		{
			Health h = c.GetComponent<Health>();
			//if there is a health script and if that objeect has a photon view component
			if(h && h.GetComponent<PhotonView>())//checking if h has a photonview component, will get 
			//an uninstantiated object error otherwise 
		    {
				float distance = Vector3.Distance(transform.position, c.transform.position);
				float damageRatio = (distance / explosionRadius);
				if(distance>killDistance)
				{
					damage= damage/2;
				}
			Debug.Log(h.name);
			if(PhotonNetwork.offlineMode)
				{
				if(h.name=="FPS_Player(Clone)"){
					h.TakeDamage(h.gameObject.name, (damage*damageRatio)/2);
				}else{
					h.TakeDamage(h.gameObject.name, damage*damageRatio);}
			}else{
				h.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.All,h.gameObject.name, damage*damageRatio);}
			}

		}
		Destroy (gameObject);
	}
}
