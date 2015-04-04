using UnityEngine;
using System.Collections;

public class EquipmentManager : MonoBehaviour {
	public GameObject grenadeEffect;
	public GameObject phaseEffect;
	public GameObject mapModel;
	public GameObject goPos;
	public Equipment currentEquipment;

	private Equipment grenade;
	private Equipment phase;
	private Equipment map;
	private GameObject mapSpawnGO;
	private bool mapInstantiated = false;




	void Awake()
	{
		InstantiateGrenade ();
		InstantiatePhase ();
		InstantiateMap ();
		currentEquipment = grenade;
		mapSpawnGO = new GameObject ();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1)) {
			currentEquipment = grenade;
			if(mapInstantiated == true){
				DestroyMap();}

		} else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2)){
			currentEquipment = phase;
			if(mapInstantiated == true){
				DestroyMap();}
		}
		else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3)){
			currentEquipment = map;
			if(mapInstantiated == false){
				CreateMap();}
			else{
				DestroyMap();}

		}//if none of these buttons are pressed then do nothing
	}

	public string GetEquipmentName()
	{
		return currentEquipment.GetEquipmentName();
	}

	void CreateMap()
	{ 
//		Quaternion playerResponRot = new Quaternion ();
		mapSpawnGO = (GameObject)Instantiate(mapModel,goPos.transform.position, goPos.transform.rotation * mapModel.transform.rotation);

//		playerResponRot.x = player.transform.rotation.x * mapSpawnGO.transform.GetChild(0).transform.rotation.x;
//		playerResponRot.y = player.transform.rotation.y * mapSpawnGO.transform.GetChild(0).transform.rotation.y;
//		playerResponRot.z = player.transform.rotation.z * mapSpawnGO.transform.GetChild(0).transform.rotation.z;

		if(mapSpawnGO){
			mapSpawnGO.transform.parent = transform.GetChild(0);
			mapSpawnGO.transform.localScale = mapModel.transform.localScale;
			mapSpawnGO.name = "MAP";
			mapInstantiated = true;
		}
	}
	void DestroyMap()
	{
		Destroy (mapSpawnGO);
		mapInstantiated = false;
	}
	
		void InstantiateGrenade()
	{
		grenade = new Equipment();//instantiating the object
		grenade.SetEffect(grenadeEffect);//assigning grenade equipment specific variables 
		grenade.SetDamage(20f);
		grenade.SetUseRate (1f);
		grenade.SetThrowSpeed (10f);
		grenade.SetEquipmentName("Grenade");
		grenade.SetEquipmentType (Equipment.EquipmentType.Throw);//reserving 0 for unarmed in the future; also coresponds with key pressed to activate
	}
	void InstantiatePhase()
	{
		phase = new Equipment();//instantiating the object
		phase.SetEffect(phaseEffect);//assigning phase equipment specific variables 
		phase.SetDamage(10f);
		phase.SetUseRate (.5f);
		phase.SetThrowSpeed (20f);
		phase.SetEquipmentName("Phase");
		phase.SetEquipmentType (Equipment.EquipmentType.Throw);//reserving 0 for unarmed in the future; also coresponds with key pressed to activate
	}
	void InstantiateMap()
	{
		map = new Equipment();//instantiating the object
		map.SetDamage(0f);
		map.SetUseRate (0f);
		map.SetThrowSpeed (0f);
		map.SetEquipmentName("Map");
		map.SetEquipmentType (Equipment.EquipmentType.Static);//reserving 0 for unarmed in the future; also coresponds with key pressed to activate
	}
	

}
