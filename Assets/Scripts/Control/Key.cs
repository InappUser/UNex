using UnityEngine;
using System.Collections;

public class Key{
	private KeyCode myKey;
	private string myAxis;
	private InputManager.AxisSide axisPosNeg;
	private bool isAxis=false;//true if key
	public bool shouldListen=false;

	public Key(KeyCode key){
		myKey = key;
		isAxis = false;
	}
	public Key(string axis, InputManager.AxisSide posNeg){//which axis and whether the positive or negative side should be listened to
		myAxis = axis;
		axisPosNeg = posNeg;
		isAxis = true;
	}

	public void SetToKey(KeyCode key){
		myKey = key;
		isAxis = false;
	}
	public void SetToAxis(string axis, InputManager.AxisSide posNeg){//which axis and whether the positive or negative side should be listened to
		myAxis = axis;
		axisPosNeg = posNeg;
		isAxis = true;
	}
	public bool GetUsed(){
		if (isAxis) {
			Debug.Log("axis raw for "+myAxis+" is "+Input.GetAxisRaw(myAxis)+" it is "+axisPosNeg+" and "+(Input.GetAxisRaw(myAxis) == (float)axisPosNeg)+" is being returned"); 
			if(axisPosNeg>0){
				return Input.GetAxis(myAxis) > 0;}
			else{
				return Input.GetAxis(myAxis)<0;}
		}else{
			if(shouldListen){Debug.Log("returning "+myKey);}
			return Input.GetKey(myKey);
		}
	}

}
