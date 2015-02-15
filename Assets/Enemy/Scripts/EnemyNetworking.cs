using UnityEngine;
using System.Collections;

public class EnemyNetworking : Photon.MonoBehaviour {
	private EnemyAI eAI;
	private NavMeshAgent nav;
	private bool chasing = false;
	private GameObject temp;
	private GameObject player; 
	private int pVid=0;

	void Start(){
		eAI = GetComponent<EnemyAI> ();
		nav = GetComponent<NavMeshAgent> ();
	}
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//Debug.Log ("stream is wrinting: " + stream.isWriting + ". stream is reading" + stream.isReading);
		if (stream.isWriting) {
			Debug.Log("chasing for enemyNetworking is "+chasing);
			if(chasing){
				Debug.Log("setting new chase for "+GetComponent<PhotonView>().viewID+" to be "+eAI.GetPlayerID()); 
				stream.SendNext(eAI.GetPlayerID());
			}	
		}else{
//
			Debug.Log("recieving new chase for "+GetComponent<PhotonView>().viewID+" to be "+temp.name); 
			if((int)stream.ReceiveNext() != pVid){
				eAI.SetIsChasingAndPlayer(FindPlayerGOByVIewID((int)stream.ReceiveNext()));//recieving id from others and finding gameobject
			}

		}
		Debug.Log ("new script working");
	}
	public void SetChasing(bool isChasing){
		chasing = isChasing;
	}
	GameObject FindPlayerGOByVIewID(int vID){
		GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
		for(int i=0;i<temp.Length;i++){
			if(temp[i].GetComponent<PhotonView>().viewID == vID){
				pVid =  temp[i].GetComponent<PhotonView>().viewID;
				return temp[i];
			}
		}
		return temp[0];//just used to avoid errors
		Debug.LogError ("No player with the id " + vID + " could be found");
	}
}
