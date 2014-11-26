using UnityEngine;
using System.Collections;

public class EquipmentManager : MonoBehaviour {
	public GameObject grenadeEffect;
	public GameObject phaseEffect;
	public GameObject player;
	public GameObject map;
	public GameObject goPos;
	public Equipment currentEquipment;

	private Equipment Grenade;
	private Equipment Phase;
	private Equipment Map;
	private GameObject mapSpawnGO;
	private bool mapInstantiated = false;


	void InstantiateGrenade()
	{
		Grenade = new Equipment();//instantiating the object
		Grenade.SetEffect(grenadeEffect);//assigning grenade equipment specific variables 
		Grenade.SetDamage(20f);
		Grenade.SetUseRate (1f);
		Grenade.SetThrowSpeed (10f);
		Grenade.SetEquipmentName("Grenade");
		Grenade.SetEquipmentType (Equipment.EquipmentType.Throw);//reserving 0 for unarmed in the future; also coresponds with key pressed to activate
	}
	void InstantiatePhase()
	{
		Phase = new Equipment();//instantiating the object
		Phase.SetEffect(phaseEffect);//assigning phase equipment specific variables 
		Phase.SetDamage(10f);
		Phase.SetUseRate (.5f);
		Phase.SetThrowSpeed (20f);
		Phase.SetEquipmentName("Phase");
		Phase.SetEquipmentType (Equipment.EquipmentType.Throw);//reserving 0 for unarmed in the future; also coresponds with key pressed to activate
	}
	void InstantiateMap()
	{
		Map = new Equipment();//instantiating the object
		Map.SetDamage(0f);
		Map.SetUseRate (0f);
		Map.SetThrowSpeed (0f);
		Map.SetEquipmentName("Map");
		Map.SetEquipmentType (Equipment.EquipmentType.Static);//reserving 0 for unarmed in the future; also coresponds with key pressed to activate
	}

	void Awake()
	{
		InstantiateGrenade ();
		InstantiatePhase ();
		InstantiateMap ();
		currentEquipment = Grenade;
		mapSpawnGO = new GameObject ();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1)) {
			currentEquipment = Grenade;
			if(mapInstantiated == true){
				DestroyMap();}

		} else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2)){
			currentEquipment = Phase;
			if(mapInstantiated == true){
				DestroyMap();}
		}
		else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3)){
			currentEquipment = Map;
			Debug.Log("3");

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
		Quaternion playerResponRot = new Quaternion ();
		mapSpawnGO = (GameObject)Instantiate(map,goPos.transform.position, goPos.transform.rotation * map.transform.rotation);

		playerResponRot.x = player.transform.rotation.x * mapSpawnGO.transform.GetChild(0).transform.rotation.x;
		playerResponRot.y = player.transform.rotation.y * mapSpawnGO.transform.GetChild(0).transform.rotation.y;
		playerResponRot.z = player.transform.rotation.z * mapSpawnGO.transform.GetChild(0).transform.rotation.z;

		if(mapSpawnGO){
			mapSpawnGO.transform.parent = transform.GetChild(0);
			mapSpawnGO.transform.localScale = map.transform.localScale;
			mapSpawnGO.name = "MAP";
			mapInstantiated = true;
		}
	}
	void DestroyMap()
	{
		Destroy (mapSpawnGO);
		mapInstantiated = false;
	}
	

}
