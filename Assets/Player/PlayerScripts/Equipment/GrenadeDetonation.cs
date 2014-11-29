using UnityEngine;
using System.Collections;

public class GrenadeDetonation : MonoBehaviour {
	public float explosionDelay = 5f;
	public float explosionRadius = 5f;
	//public float explosionLift = 1f;
	public GameObject explosionEffect;
	
	private Vector3 grenadeOrigin;
	private Collider[] colliders;
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("detonation");
		StartCoroutine(Explode ());
	}
	IEnumerator Explode()
	{
		yield return new WaitForSeconds (.4f);
		Instantiate (explosionEffect, transform.position, transform.rotation);

		grenadeOrigin = transform.position;
		colliders = Physics.OverlapSphere (grenadeOrigin, explosionRadius);
		foreach (Collider hit in colliders) {
			if(hit.gameObject != gameObject){
				GameObject go = hit.gameObject;
				Health hitGOHealth = go.GetComponent<Health>();
				if(hitGOHealth){
					hitGOHealth.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.AllBuffered,5f);
					Debug.Log("hurt");
				}
			}		
		}

	}
}
