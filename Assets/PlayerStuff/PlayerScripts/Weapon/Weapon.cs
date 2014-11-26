using UnityEngine;
using System.Collections;

public class Weapon{//:monobehaviour is only for scripts that are to be attached to objects
	public short clipcount;
	public float fireRateCoolDown;
	public enum WeaponType{Stream,Projectile};

	private GameObject model;
	private GameObject shootEffect;//naming convention to differentiate between variables and methods
	private GameObject shootExitBarrel;
	private float damage;
	private float fireRate;
	private float reloadTime;
	private short clipSize;
	private string weaponName;
	private WeaponType weaponType;


	public GameObject GetModel()
	{return model;}
	public void SetModel(GameObject setModel)
	{model = setModel;}

	public GameObject GetShootEffect()
	{return shootEffect;}
	public void SetShootEffect(GameObject setEffect)
	{shootEffect = setEffect;}

	public GameObject GetShootExitBarrel()
	{return shootExitBarrel;}
	public void SetShootExitBarrel(GameObject setEffect)
	{shootExitBarrel = setEffect;}

	public float GetDamage()
	{return damage;}
	public void SetSDamage(float setDamage)
	{damage = setDamage;}

	public float GetFireRate()
	{return fireRate;}
	public void SetFireRate(float setFireRate)
	{fireRate = setFireRate;}

	public float GetReloadTime()
	{return reloadTime;}
	public void SetReloadTime(float setReloadTime)
	{reloadTime = setReloadTime;}

	public short GetClipSize()
	{return clipSize;}
	public void SetClipSize(short setClipSize)
	{clipSize = setClipSize;}

	public string GetWeaponName()
	{return weaponName;}
	public void SetWeaponName(string setWeaponName)
	{weaponName = setWeaponName;}


	public void SetWeaponType(WeaponType setWeaponType)
	{weaponType = setWeaponType;}
	public WeaponType GetWeaponType()
	{return weaponType;}

}
