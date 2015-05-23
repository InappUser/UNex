using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using System;

public class InputManager : MonoBehaviour {
	private KeyCode jump = KeyCode.Space;
	private KeyCode tmp;
	private Axis lookUD,lookLR,vert,hori;
	
	private bool listenNextKey = false;
	private bool keyNotReset = true;

	private string correctUpdateMethod;
	private Type type;
	private MethodInfo method;

	// Use this for initialization
	void Start () {
		Debug.Log ("started");

		vert = new Axis (KeyCode.W, KeyCode.S);//should really be using player prefs
		hori = new Axis (KeyCode.D, KeyCode.A);
		lookUD = new Axis (KeyCode.UpArrow, KeyCode.DownArrow);
		lookLR = new Axis (KeyCode.RightArrow, KeyCode.LeftArrow);

		type = Type.GetType("InputManager");//setting type to this class, so that methods can be called via string
	}
	public void testMethod(){
		Debug.Log ("workedwwwdw");
	}

	void OnGUI () {

		if(Event.current.keyCode != KeyCode.None)
		{tmp = Event.current.keyCode;} //allways gettig the most recent button press
	}
	void Update(){
		if (listenNextKey) {
			if(keyNotReset){
				Debug.Log("resetting key");
				tmp = KeyCode.None;
				keyNotReset=false;}//so am sure that it is the intended key
			if(tmp!=KeyCode.None){//if user has now pressed key
				Debug.Log("key is now"+tmp);
				keyNotReset=true;
				listenNextKey=false;
				method.Invoke (this, null);
			}
		}
	}

	public void GetNewForward(){
		listenNextKey = true;
		method = type.GetMethod ("UpdateForward");
	}
	public void GetNewBackward(){
		listenNextKey = true;
		method = type.GetMethod ("UpdateBackward");
	}
	public void GetNewRight(){
		listenNextKey = true;
		method = type.GetMethod ("UpdateRight");
	}
	public void GetNewLeft(){
		listenNextKey = true;
		method = type.GetMethod ("UpdateLeft");
	}
	public void UpdateForward(){vert.SetPos (tmp);}
	public void UpdateBackward(){vert.SetNeg (tmp);}
	public void UpdateRight(){hori.SetPos (tmp);}
	public void UpdateLeft(){hori.SetNeg (tmp);}



	public float Vertical(){
		return vert.Move();
	}

	public float Horizontal(){
		return hori.Move ();
	}
	public float LookUD(){
		return lookUD.Move ();
	}
	public float LookLR(){
		return lookLR.Move ();
	}

}
