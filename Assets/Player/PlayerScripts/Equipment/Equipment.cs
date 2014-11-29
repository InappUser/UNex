using UnityEngine;
using System.Collections;

public class Equipment{//:monobehaviour is only for scripts that are to be attached to objects
	public float useRateCoolDown;
	public enum EquipmentType{Throw,Static};

	private GameObject effect;//naming convention to differentiate between variables and methods

	private float damage;
	private float useRate;
	private float throwSpeed;
	private string equipmentName;
	private EquipmentType equipmentType;


	public GameObject GetEffect()
	{return effect;}
	public void SetEffect(GameObject setEffect)
	{effect = setEffect;}

	public float GetDamage()
	{return damage;}
	public void SetDamage(float setDamage)
	{damage = setDamage;}

	public float GetThrowSpeed()
	{return throwSpeed;}
	public void SetThrowSpeed(float setThrowSpeed)
	{throwSpeed = setThrowSpeed;}

	public float GetUseRate()
	{return useRate;}
	public void SetUseRate(float setUseRate)
	{useRate = setUseRate;}

	public void SetEquipmentType(EquipmentType setEquipmentType)
	{equipmentType = setEquipmentType;}
	public EquipmentType GetEquipmentType()
	{return equipmentType;}

	public void SetEquipmentName(string setEquipmentName)
	{equipmentName = setEquipmentName;}
	public string GetEquipmentName()
	{return equipmentName;}

}
