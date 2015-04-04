using UnityEngine;
using System.Collections;

public class MyNetworkCharacter : Photon.MonoBehaviour {//photon's verison of monbehaviour -used for shortcut
	//to photonView.isMine
	public GameObject gunFrom;
	public GameObject gunTo;

	private Vector3 realPosition = Vector3.zero;//setting variables to their respective zeros
	private Quaternion realRotation = Quaternion.identity;
	private Quaternion realGunRotation = Quaternion.identity;
	private Animator anim;
	private bool recivedFirstUpdate = false;
	private GameObject[] enemies;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		if(!anim)
		{
			Debug.LogError("An animator is not attached to character");
		}
		enemies = GameObject.FindGameObjectsWithTag ("EnemyAlive");
	}
	
	// Fixed because funciton is dealing with movement
	void FixedUpdate () {
		//enemies = GameObject.FindGameObjectsWithTag ("Enemy");

		if (!photonView.isMine) //checking to see if object with a pjotonview component is my player or other
		{//if photonview is mine do nothing - the character motor is moving the player

			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f); //lerp is used for transitions
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
			//Debug.Log("Found gun: "+transform.FindChild("OtherPlayer"));
			//gunTo.transform.rotation = Quaternion.Lerp(transform.rotation,realGunRotation,.5f);
			//little bit of a lag, but is much smoother
		}//float represents smoothing speed
	}
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//Debug.Log ("Photon serialising for " + transform.name);
		if(anim == null){
			anim = GetComponent<Animator>();//this is required in order to get rid of the "Object reference not set to an instance of an object photon view"
		}
		//Debug.Log ("player stream is wrinting: " + stream.isWriting + ". stream is reading" + stream.isReading);
		if (stream.isWriting) 
		{//this is our player, transform information needs to be sent to the network

			stream.SendNext(transform.position);//sending players position
			//Debug.Log("this players position "+transform.position);
			stream.SendNext(transform.rotation);//sending players rotation
			//stream.SendNext(gunFrom.transform.rotation); //sending gun rotation
  			stream.SendNext(anim.GetFloat("FSpeed"));//sending the players forward/backward animation info
			stream.SendNext(anim.GetFloat("SSpeed"));//sending the players left/right animation info
			stream.SendNext(anim.GetBool("Jump"));//sending the players jump animation info
			stream.SendNext(anim.GetBool("Reloading"));//sending the players reloading animation info
			stream.SendNext(anim.GetInteger("ActiveWeapon"));//sending which weapon the player is holding (0=MG, 1=SG, 2=RL)

			//for(int i=0;i <enemies.Length;i++){
				//(enemies[i]!= null){
				//	stream.SendNext(enemies[i].transform.position);
				//}
				//Debug.Log ("i: "+i);
			//}

		}
		else
		{//this is someone else's player, need to recieve their position (as of a few miliseconds ago) and update
		//our version of this player


			realPosition =(Vector3)stream.ReceiveNext()/*as Vector3*/;//recieving position
			//Debug.Log("other players position "+transform.position);
			realRotation =(Quaternion)stream.ReceiveNext()/*as Quaternion*/;//recieving rotation
			//realGunRotation = (Quaternion)stream.ReceiveNext();
			//this is two ways of "casting" - ensuring that stream is correct format
			anim.SetFloat("FSpeed",(float)stream.ReceiveNext());
			anim.SetFloat("SSpeed",(float)stream.ReceiveNext());//the order in which these are sent needs to be mirrored when recieved
			anim.SetBool("Jump",(bool)stream.ReceiveNext());
			anim.SetBool("Reloading",(bool)stream.ReceiveNext()); //recieving information for reloading from other players
			anim.SetInteger("ActiveWeapon",(int)stream.ReceiveNext()); 
			
			//for(int i=0;i <enemies.Length;i++)
				//enemies[i].transform.position =(Vector3)stream.ReceiveNext();//fundamentally flawed- doesn't need to go through every enemy. this repeatedly sets the
			//position of unmoving enemies unnecisarily, causing a lot of lag. This also means that the way in which 
			if(!recivedFirstUpdate)//if the first update has not been recieved
			{
				transform.position = realPosition;
				transform.rotation = realRotation;
				recivedFirstUpdate = true;
				//stopping the character from appearing in the center and movin to real position
			}//teleporting them
		}
	}
}
