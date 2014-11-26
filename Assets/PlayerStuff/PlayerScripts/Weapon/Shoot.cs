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


	//private float fRate;

	void Start()
	{
		weapon = gameObject.GetComponent<WeaponManager> ();
		//weapon.currentWeapon.clipcount = weapon.currentWeapon.GetClipSize ();


	}

	void Update()
	{

		KeepGunCurrent ();

		if(Input.GetButton("Fire1") && (fRateCool<0) && weapon.currentWeapon.clipcount >0 && !reloading){
			Fire ();}
//		if (weapon.currentWeapon.fireRateCoolDown >= weapon.currentWeapon.GetFireRate()) {
//			shootLine.enabled = false;}		
		if(!reloading && (Input.GetKey (KeyCode.R) || weapon.currentWeapon.clipcount <=0) && weapon.currentWeapon.clipcount < weapon.currentWeapon.GetClipSize()){
			StartCoroutine("Reload");}

		if (reloading && weapon.GetWeaponChanged () == true) {
			Debug.Log("weapon changed while reloading");
			StopCoroutine("Reload");
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

	void Fire()
	{
		weapon.currentWeapon.clipcount --;
		//shootLine.enabled = true;
		//shootLine.SetPosition (0, shootExitPos);//giving the line renderer a starting position
		weapon.currentWeapon.fireRateCoolDown = weapon.currentWeapon.GetFireRate();// reseting the fire rate cooldown

		if(weapon.currentWeapon.GetShootEffect()){
			//giving the end of the gun sparks - doing both of these things regardless of whether anything is hit by the raycast
			muzzleFlash = (GameObject) Instantiate(weapon.currentWeapon.GetShootExitBarrel(),transform.position,shootExit.transform.rotation);

			//shooting the rocket further forward if the weapon is a rocket
			if(weapon.currentWeapon.GetWeaponType() == Weapon.WeaponType.Projectile){
				Instantiate (weapon.currentWeapon.GetShootEffect(), shootExitPos + (shootExit.forward*0.1f), Camera.main.transform.rotation);
				return;//if is the rocket launcher then the rocket will deal damage etc.
			}
		}

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
		}else{//if nothing was hit
			//shootLine.SetPosition(1,ray.origin+ray.direction);//sending it off into the direction from where was fired until range
		}	
		Debug.DrawRay(transform.position,transform.forward,Color.blue);

	}

	IEnumerator Reload()
	{
		if(!reloading){
			reloading = true;
			if(anim !=null){
				anim.SetBool ("Reloading", true);}
			}
		yield return new WaitForSeconds (weapon.currentWeapon.GetReloadTime());
		reloading = false;
		anim.SetBool ("Reloading", false);
		//StartCoroutine(WaitForAnimation());
		weapon.currentWeapon.clipcount = weapon.currentWeapon.GetClipSize ();

	}
//	IEnumerator WaitForAnimation()
//	{
//		yield return new WaitForSeconds (weapon.currentWeapon.GetReloadTime());
//		reloading = false;
//		anim.SetBool ("Reloading", false);
//	}

	public short GetCurrentAmmo()
	{
		return weapon.currentWeapon.clipcount;
	}

}
