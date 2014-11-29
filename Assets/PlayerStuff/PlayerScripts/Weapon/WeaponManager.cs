using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {
	public GameObject machineGunModel;
	public GameObject machineGunEffect;

	public GameObject shotGunModel;
	public GameObject shotGunEffect;

	public GameObject rocketLauncherModel;
	public GameObject rocketProjectile;

	public GameObject exitBarrelEffect;
	public Weapon currentWeapon;
	public GameObject player;


	private Weapon[] weapons;
	private Weapon machineGun;
	private Weapon shotGun;
	private Weapon rocketLauncher;
	private short currentWeaponnum = 1;
	private float wheelnum = 1;
	private bool wheelChanged = false;
	private bool weaponChanged = false;
	private GameObject weaponHold;


	Weapon InstantiateMachineGun()
	{
		machineGun = new Weapon();//instantiating the object
		machineGun.SetModel(machineGunModel);
		machineGun.SetShootEffect(machineGunEffect);//assigning machine gun specific variables 
		machineGun.SetShootExitBarrel (exitBarrelEffect);
		machineGun.SetSDamage(15f);
		machineGun.SetFireRate (0.09f);
		machineGun.SetReloadTime(3.5f);
		machineGun.SetClipSize (30);//ensuring that the player does not need to reload when changing to weapon for first time
		machineGun.clipcount = machineGun.GetClipSize ();
		machineGun.SetWeaponName("Machine Gun");
		machineGun.SetWeaponType (Weapon.WeaponType.Stream);//reserving 0 for unarmed in the future; also coresponds with key pressed to activate
		return machineGun;
	}
	Weapon InstantiateShotGun()
	{
		shotGun = new Weapon();//instantiating the object
		shotGun.SetModel(shotGunModel);
		shotGun.SetShootEffect(shotGunEffect);//assigning machine gun specific variables 
		shotGun.SetShootExitBarrel (exitBarrelEffect);
		shotGun.SetSDamage(100f);
		shotGun.SetFireRate (0.5f);
		shotGun.SetReloadTime(5f);
		shotGun.SetClipSize (2);
		shotGun.clipcount = shotGun.GetClipSize ();
		shotGun.SetWeaponName("Sniper Rifle");
		shotGun.SetWeaponType (Weapon.WeaponType.SingleShot);
		return shotGun;
	}
	Weapon InstantiateRocketLauncher()
	{
		rocketLauncher = new Weapon();//instantiating the object
		rocketLauncher.SetModel(rocketLauncherModel);
		rocketLauncher.SetShootEffect(rocketProjectile);//assigning machine gun specific variables 
		rocketLauncher.SetShootExitBarrel (exitBarrelEffect);
		rocketLauncher.SetSDamage(0f);
		rocketLauncher.SetFireRate (0.7f);
		rocketLauncher.SetReloadTime(4f);
		rocketLauncher.SetClipSize (2);
		rocketLauncher.clipcount = rocketLauncher.GetClipSize ();
		rocketLauncher.SetWeaponName("Rocket Launcher");
		rocketLauncher.SetWeaponType (Weapon.WeaponType.Projectile);
		return rocketLauncher;
	}
	void Awake()
	{
		//weaponHold = new GameObject ();
		weapons = new Weapon[3];//hard coded as there will only be 3 weapons
		weapons[0] = InstantiateMachineGun ();
		weapons[1] = InstantiateShotGun ();
		weapons[2] = InstantiateRocketLauncher ();
		currentWeapon = machineGun; //machine gun by default

	}


	// Update is called once per frame
	void Update () {
		weaponChanged = false;//having the bool flicker like a trigger 
		wheelChanged = false;
		if(!Input.GetKey(KeyCode.LeftShift)){//if the shift key is not held down; shift activates equipment
			if(Input.GetAxisRaw("Mouse ScrollWheel")>0 && wheelnum <4){
				wheelnum++;
				//wheelnum += Mathf.Clamp( wheelnum+ -(int)(Input.GetAxisRaw("Mouse ScrollWheel")*100)  ,1,3);//minus is to make less confusing on mac
				//*100 so that 1 turn on the wheel increments wheelnum fully
				wheelChanged = true;
			}

			if(Input.GetAxisRaw("Mouse ScrollWheel")<0 && wheelnum >1){
				wheelnum--;
				wheelChanged = true;
			}
			//if the weapon is not already selected and either the '1' key is pressed or the scrollnum = 0

			if(currentWeaponnum!=1 && (Input.GetKeyDown(KeyCode.Alpha1)|| (wheelnum == 1 && wheelChanged == true)))
				{WeaponSelect(1);}
			else if(currentWeaponnum!=2 && (Input.GetKeyDown(KeyCode.Alpha2)|| (wheelnum == 2 && wheelChanged == true)))
				{WeaponSelect(2);}
			else if(currentWeaponnum!=3 && (Input.GetKeyDown(KeyCode.Alpha3)|| (wheelnum == 3 && wheelChanged == true)))
				{WeaponSelect(3);}

			}
	}
	void WeaponSelect(short weap)
	{
		weaponChanged = true;//so that the reload animation etc can stop if weapon has changed
		currentWeapon = weapons[weap-1];//-1 because array starts at 0 rather than 1, which would be more intuative
		ChangeWeaponModel();
		currentWeaponnum = weap;
		wheelnum =weap;//if none of these buttons are pressed then do nothing
	}

	void ChangeWeaponModel()
	{

		Destroy (gameObject.transform.parent.GetChild(1).gameObject);//getting rid of previous object
		weaponHold=(GameObject)Instantiate(currentWeapon.GetModel(),transform.parent.position,player.transform.rotation *currentWeapon.GetModel().transform.rotation);
		//^putting the player-selected object in place
		if(weaponHold){//ensuring that the model is scaled correctly
			Vector3 one = new Vector3 ();
			one.x = 1f;
			one.y = 1f;
			one.z = 1f;
			weaponHold.transform.parent = transform.parent;
			weaponHold.transform.localScale = one;
		}
	}

	public bool GetWeaponChanged()
	{
		return weaponChanged;
	}
	public void WeaponChangeComplete()
	{
		weaponChanged = false;
	}

}
