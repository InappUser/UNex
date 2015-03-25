using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {
	public GameObject machineGunModel;
	public GameObject machineGunEffect;
	public AudioClip machineGunSound;

	public GameObject shotGunModel;
	public GameObject shotGunEffect;
	public AudioClip shotGunSound;

	public GameObject rocketLauncherModel;
	public GameObject rocketProjectile;
	public AudioClip rocketLauncherSound;

	public GameObject exitBarrelEffect;
	public Weapon currentWeapon;
	public GameObject player;
	public UseEquipment useEquipment;// using this to pass the appropriate animator when a weapon has been changed as the animation needs to be played for the weapon

	private Weapon[] weapons;
	private Weapon machineGun;
	private Weapon shotGun;
	private Weapon rocketLauncher;
	private Animator animOUT;
	private GameObject weaponHolding;
	private AudioSource[] aSources;
	private Vector3 one;
	private short currentWeaponNum = 1;
	private float wheelNum = 1;
	private bool wheelChanged = false;
	private bool weaponChanged = false;

	
	void Awake()
	{
		one = new Vector3 ();
		one.x = 1f;
		one.y = 1f;
		one.z = 1f;

		weapons = new Weapon[3];//hard coded as there will only be 3 weapons
		weapons[0] = InstantiateMachineGun ();
		weapons[1] = InstantiateShotgun ();
		weapons[2] = InstantiateRocketLauncher ();

		aSources = GetComponents<AudioSource> ();
	}
	void Start(){
		currentWeapon = machineGun; //machine gun by default
		weaponHolding = (GameObject)Instantiate (machineGunModel, transform.position,player.transform.rotation *currentWeapon.GetModel().transform.rotation);
		weaponHolding.transform.parent = transform;
		weaponHolding.transform.localScale = one;
		useEquipment.SetAnim (weaponHolding.transform.GetChild(1).GetComponent<Animator> ());//passing the correct animator to the useEquipment at the beggining
		animOUT = transform.root.GetComponent<Animator> ();
		ChangeWeaponSound(currentWeapon.GetShootSound ());//defaulting the shooting sound
	}

	// Update is called once per frame
	void Update () {
		weaponChanged = false;//having the bool flicker like a trigger 
		wheelChanged = false;
		if(!Input.GetKey(KeyCode.LeftShift)){//if the shift key is not held down; shift activates equipment
			if(Input.GetAxisRaw("Mouse ScrollWheel")>0 && wheelNum <4){
				wheelNum++;
				//wheelNum += Mathf.Clamp( wheelNum+ -(int)(Input.GetAxisRaw("Mouse ScrollWheel")*100)  ,1,3);//minus is to make less confusing on mac
				//*100 so that 1 turn on the wheel increments wheelNum fully
				wheelChanged = true;
			}

			if(Input.GetAxisRaw("Mouse ScrollWheel")<0 && wheelNum >1){
				wheelNum--;
				wheelChanged = true;
			}
			//if the weapon is not already selected and either the '1' key is pressed or the scrollnum = 0

			if(currentWeaponNum!=1 && (Input.GetKeyDown(KeyCode.Alpha1)|| (wheelNum == 1 && wheelChanged == true)))
				{WeaponSelect(1);}
			else if(currentWeaponNum!=2 && (Input.GetKeyDown(KeyCode.Alpha2)|| (wheelNum == 2 && wheelChanged == true)))
				{WeaponSelect(2);}
			else if(currentWeaponNum!=3 && (Input.GetKeyDown(KeyCode.Alpha3)|| (wheelNum == 3 && wheelChanged == true)))
				{WeaponSelect(3);}

			}
	}
	void WeaponSelect(short weap)
	{
		weaponChanged = true;//so that the reload animation etc can stop if weapon has changed
		currentWeapon = weapons[weap-1];//-1 because array starts at 0 rather than 1, which would be more intuative
		animOUT.SetInteger ("ActiveWeapon", weap - 1);//changing the outside players weapon accordingly
		ChangeWeaponModel();//changing what the player sees
		currentWeaponNum = weap;
		wheelNum =weap;//if none of these buttons are pressed then do nothing
		useEquipment.SetAnim (weaponHolding.transform.GetChild(1).GetComponent<Animator> ());//passing the correct animator to the useEquipment at the beggining//changing the animator for use equipment appropriately. This ensures that animations can be played when using equipment
		ChangeWeaponSound(currentWeapon.GetShootSound ());
	}

	void ChangeWeaponModel()
	{
		//Debug.Log ("trying to destroy: "+gameObject.transform.parent.GetChild (1).gameObject.name);
		Destroy (gameObject.transform.GetChild(0).gameObject);//getting rid of previous object
		weaponHolding=(GameObject)Instantiate(currentWeapon.GetModel(),transform.position,player.transform.rotation *currentWeapon.GetModel().transform.rotation);
		//^putting the player-selected object in place
		if(weaponHolding){//ensuring that the model is scaled correctly
			weaponHolding.transform.parent = transform;
			weaponHolding.transform.localScale = one;
		}
	}

	void ChangeWeaponSound(AudioClip clip){
		foreach (AudioSource aS in aSources) {
			aS.clip = clip; //setting all of the sources clips to the gun sounds mp3	
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

	Weapon InstantiateMachineGun()
	{
		machineGun = new Weapon();//instantiating the object
		machineGun.SetModel(machineGunModel);
		machineGun.SetShootEffect(machineGunEffect);//assigning machine gun specific variables 
		machineGun.SetShootExitBarrel (exitBarrelEffect);
		machineGun.SetSDamage(6f);
		machineGun.SetFireRate (0.05f);
		machineGun.SetReloadTime(2.55f);
		machineGun.SetWaitBeforeInitFireTime(0f);
		machineGun.SetClipSize (20);//ensuring that the player does not need to reload when changing to weapon for first time
		machineGun.clipcount = machineGun.GetClipSize ();
		machineGun.SetWeaponName("BAAL Pattern Boltgun");
		machineGun.SetWeaponType (Weapon.WeaponType.Stream);//reserving 0 for unarmed in the future; also coresponds with key pressed to activate
		machineGun.SetshootSound (machineGunSound);
		return machineGun;
	}
	Weapon InstantiateShotgun()
	{
		shotGun = new Weapon();//instantiating the object
		shotGun.SetModel(shotGunModel);
		shotGun.SetShootEffect(shotGunEffect);//assigning machine gun specific values. shhot effect refering to the bullet hole left at the hit 
		shotGun.SetShootExitBarrel (exitBarrelEffect);
		shotGun.SetSDamage(80f);
		shotGun.SetFireRate (0.7f);
		shotGun.SetReloadTime(4f);
		shotGun.SetWaitBeforeInitFireTime(.06f);
		shotGun.SetClipSize (4);
		shotGun.clipcount = shotGun.GetClipSize ();
		shotGun.SetWeaponName("Shotgun");
		shotGun.SetWeaponType (Weapon.WeaponType.SingleShot);
		shotGun.SetshootSound (shotGunSound);
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
		rocketLauncher.SetReloadTime(6f);
		rocketLauncher.SetWaitBeforeInitFireTime(.21f);
		rocketLauncher.SetClipSize (2);
		rocketLauncher.clipcount = rocketLauncher.GetClipSize ();
		rocketLauncher.SetWeaponName("Rocket Launcher");
		rocketLauncher.SetWeaponType (Weapon.WeaponType.Projectile);
		rocketLauncher.SetshootSound (machineGunSound);
		return rocketLauncher;
	}

}
