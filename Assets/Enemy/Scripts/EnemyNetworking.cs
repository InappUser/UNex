using UnityEngine;
using System.Collections;

public class EnemyNetworking : Photon.MonoBehaviour {
	private EnemyAI eAI;
	private NavMeshAgent nav;
	public bool chasing = false;
	private GameObject temp;
	private GameObject player; 
	private int pVid=0;
	private Vector3 realPosition = Vector3.zero;//setting variables to their respective zeros
	private Quaternion realRotation = Quaternion.identity;

	void Start(){
		eAI = GetComponent<EnemyAI> ();
		nav = GetComponent<NavMeshAgent> ();
	}
	void FixedUpdate () {
				//enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		
		if (!photonView.isMine) { //checking to see if object with a pjotonview component is my player or other//if photonview is mine do nothing - the character motor is moving the player
			//Debug.Log("real position"+realPosition.ToString());
			transform.position = Vector3.Lerp (transform.position, realPosition, 0.1f); //lerp is used for transitions
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, 0.1f);
		}
	}
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//Debug.Log ("stream is wrinting: " + stream.isWriting + ". stream is reading" + stream.isReading);
		if (stream.isWriting) {

			//Debug.Log("writing");
				stream.SendNext(transform.position);//sending players position
			//Debug.Log("this players position "+transform.position);
				stream.SendNext(transform.rotation);//sending players rotation
			stream.SendNext(GetComponent<Animator>().GetBool("isWalking"));
//			Debug.Log("chasing for enemyNetworking is "+chasing);
//			if(chasing){
//				Debug.Log("setting new chase for "+GetComponent<PhotonView>().viewID+" to be "+eAI.GetPlayerID()); 
			//				stream.SendNext(eAI.GetPlayerID());}
		}else{


				realPosition =(Vector3)stream.ReceiveNext();/*as Vector3*/;//recieving position
				//Debug.Log("reading: position recieved"+ realPosition.ToString());
				//Debug.Log("other players position "+transform.position);
				realRotation =(Quaternion)stream.ReceiveNext();/*as Quaternion*/;//recieving rotation
			
			GetComponent<Animator>().SetBool("isWalking", (bool)stream.ReceiveNext());
			//Debug.Log("recieving new chase for "+GetComponent<PhotonView>().viewID+" to be "+temp.name); 
//			if((int)stream.ReceiveNext() != pVid){
//				eAI.SetIsChasingAndPlayer(FindPlayerGOByVIewID((int)stream.ReceiveNext()));//recieving id from others and finding gameobject
			//}

		}
		//Debug.Log ("new script working");
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
