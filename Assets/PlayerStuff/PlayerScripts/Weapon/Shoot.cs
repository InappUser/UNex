using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	public LineRenderer shootLine;
	public bool reloading = false;

	private Animator anim;
	
	private Health hitGOHealth;//ammo size will never be big enough to justify full int
	private	Transform shootExit;
	private	Vector3 shootExitPos;
	private WeaponManager weapon; 
	private float fRateCool;
	private GameObject muzzleFlash;
	private bool firing = false;


	//private float fRate;

	void Start()
	{
		weapon = gameObject.GetComponent<WeaponManager> ();
		//weapon.currentWeapon.clipcount = weapon.currentWeapon.GetClipSize ();


	}

	void Update()
	{

		KeepGunCurrent ();

		if(Input.GetButton("Fire1") && (fRateCool<0) && weapon.currentWeapon.clipcount >0 && !reloading && !firing){
			StartCoroutine( Fire());}//is an ienumerator so that a delay can occur with the shotgun
//		if (weapon.currentWeapon.fireRateCoolDown >= weapon.currentWeapon.GetFireRate()) {
//			shootLine.enabled = false;}		
		if(!reloading && (Input.GetKey (KeyCode.R) || weapon.currentWeapon.clipcount <=0) && weapon.currentWeapon.clipcount < weapon.currentWeapon.GetClipSize()){
			StartCoroutine(ActivateAnim("Reloading", true, weapon.currentWeapon.GetReloadTime()));}

		if (reloading && weapon.GetWeaponChanged () == true) {
			Debug.Log("weapon changed while reloading");
			StopCoroutine("ActivateAnim");
			reloading = false;
			if(anim)
				anim.SetBool ("Reloading", false);
			weapon.currentWeapon.clipcount = weapon.currentWeapon.GetClipSize ();
			weapon.WeaponChangeComplete();
		}

		if(muzzleFlash){
			muzzleFlash.transform.parent = transform;}
	}

	void KeepGunCurrent()
	{
		shootExitPos = transform.position;
		shootExitPos.y -= .2f;
		shootExitPos.x +=  1.3f * transform.rotation.x;
		shootExit = transform;

		anim = transform.parent.GetComponentInChildren<Animator>();
		//assigning anim the value of the animator available in the currentweapon. Ensuring that it is kept current
		weapon.currentWeapon.fireRateCoolDown -= Time.deltaTime;//decrementing 
		fRateCool = weapon.currentWeapon.fireRateCoolDown;//these exist purley for legibility 
		//(ensures that the ammo will change as soon as the weapon has)
	}

	IEnumerator Fire()
	{


		StartCoroutine (ActivateAnim ("Shooting", false, weapon.currentWeapon.GetFireRate ()));
		if (weapon.currentWeapon.GetWeaponType () == Weapon.WeaponType.SingleShot) {
			Debug.Log("is singleshot");
			firing = true;
			yield return new WaitForSeconds (weapon.currentWeapon.GetFireRate()*.2f);
			firing = false;
		}
		weapon.currentWeapon.clipcount --;
		weapon.currentWeapon.fireRateCoolDown = weapon.currentWeapon.GetFireRate();// reseting the fire rate cooldown

		if(weapon.currentWeapon.GetShootEffect()){
			//shooting the rocket further forward if the weapon is a rocket
			if(weapon.currentWeapon.GetWeaponType() == Weapon.WeaponType.Projectile){
				Instantiate (weapon.currentWeapon.GetShootEffect(), shootExitPos + (shootExit.forward*0.1f), Camera.main.transform.rotation);
				return false;//if is the rocket launcher then the rocket will deal damage etc.
			}
			//giving the end of the gun sparks - doing both of these things regardless of whether anything is hit by the raycast
			muzzleFlash = (GameObject) Instantiate(weapon.currentWeapon.GetShootExitBarrel(),transform.position,shootExit.transform.rotation);
		}

		DrawRay ();




	}
	void DrawRay()
	{
		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hitInfo;
		
		if (Physics.Raycast (ray, out hitInfo, 100f)) {
			if(weapon.currentWeapon.GetShootEffect()){
				Instantiate(weapon.currentWeapon.GetShootEffect(),hitInfo.point,Quaternion.identity);}
			
			GameObject gO = hitInfo.collider.gameObject;//getting the hit gameobject
			
			hitGOHealth =gO.GetComponent<Health>();
			if(hitGOHealth){
				if(!hitGOHealth.GetComponent<PhotonView>())
				{Debug.Log("No photonview copmonent found of this game object");}
				else{
					hitGOHealth.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.AllBuffered,weapon.currentWeapon.GetDamage());//RPC is global method, am invoking it on the photonview component
				}//of the hit object, i.e. sending the message to every object with a photonview component
			}
			//shootLine.SetPosition(1,hitInfo.point);
		}
		Debug.DrawRay(transform.position,transform.forward,Color.blue);
	}

	IEnumerator ActivateAnim(string boolName, bool isReloading, float waitTime)
	{
		if(anim !=null){ //making sure that the animator is assigned
			if(isReloading){
				reloading = true;//saying that am currently reloading
			}//set the appropriate animator bool to true
			anim.SetBool (boolName, true);
			}
		yield return new WaitForSeconds (waitTime);

		anim.SetBool (boolName, false);

		if(isReloading){
			reloading = false;//state that reloading has finished, if this was used for reloading, and reset ammo
			weapon.currentWeapon.clipcount = weapon.currentWeapon.GetClipSize ();
		}

	}

	public short GetCurrentAmmo()
	{
		return weapon.currentWeapon.clipcount;
	}

}
