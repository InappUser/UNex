using UnityEngine;
using System.Collections;

public class Map_Teleporting : MonoBehaviour {

	private Quaternion tmpRotCam;

	void OnTriggerEnter(Collider col)
	{
		//Debug.Log ("triggered by "+col.transform.name);
		if(col.transform.name == "FPS_Player(Clone)"){
			//Debug.Log("Teleported player");
			SetPosition(col.transform.gameObject);
			//the first child of this GO is the location the player is sent to
			float tmpEulerY = GetYRotation(col.transform.eulerAngles.y);// - transform.GetChild(0).localEulerAngles.y;
			col.transform.eulerAngles = new Vector3(col.transform.rotation.x, tmpEulerY, col.transform.rotation.z);
			//had to use euler angles instead of quaternion - quaternion would not allow for the rotation to be changed to 90 for some reason
			//Debug.Log ("resulting y is "+transform.GetChild(0).localEulerAngles.y);
		}
	}

	void SetPosition(GameObject playerRoot){
		//playerRoot.transform.parent = transform;//setting tp stop as [arent
		float tmpPlayerX = GetPlayerZPosDiff(playerRoot.transform.position.z);
		//playerRoot.transform.parent = transform.GetChild(0);

		Vector3 tmpPos = new Vector3(transform.GetChild(0).position.x-tmpPlayerX,transform.GetChild(0).position.y,transform.GetChild(0).position.z);
		playerRoot.transform.position = tmpPos;
		//playerRoot.transform.parent = null;
	}

	float GetPlayerZPosDiff(float playerZ){
		float tmpZPos = transform.localPosition.z - playerZ;
		//Debug.Log("local teleport z: "+playerZ+"- player x:"+transform.localPosition.z+"="+ tmpZPos);
		//need to learn Vector3 rotation matrix maths to do this propperly
		return tmpZPos;
	}

	float GetYRotation(float playerEA_Y){
		float yRot = playerEA_Y + transform.GetChild (0).transform.localEulerAngles.y;
		if (yRot > 360) {
			yRot -=360;		
		}
		return yRot;
	}


}

