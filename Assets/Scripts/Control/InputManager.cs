using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using System;

public class InputManager : MonoBehaviour {
	public enum ControlScheme{KeyboardOnly,MouseOnly,Both}

	private ControlScheme controlScheme;
	private KeyCode jump = KeyCode.Space;
	private KeyCode tmp;
	private Axis lookUD,lookLR,vert,hori;
	
	private bool listenNextKey = false;
	private bool listenForMouse = false;
	private bool keyNotReset = true;

	private string correctUpdateMethod;
	private Type type;
	private MethodInfo method;
	private object[] paramArr;
	private string mName;

	public void SetControlsToMouseOnly(){
		vert.SetToAxis("Mouse Y");
		hori.SetToAxis("Mouse X");
		lookUD.SetBoth(KeyCode.W, KeyCode.S);
		lookLR.SetBoth(KeyCode.D, KeyCode.A);
	}
	public void SetControlsToBoth(){
		vert.SetBoth(KeyCode.W, KeyCode.S);
		hori.SetBoth(KeyCode.D, KeyCode.A);
		lookUD.SetToAxis("Mouse Y");
		lookLR.SetToAxis ("Mouse X");

	}
	public void SetControlsToKeyboardOnly(){
		vert.SetBoth(KeyCode.UpArrow, KeyCode.DownArrow);
		hori.SetBoth(KeyCode.RightArrow, KeyCode.LeftArrow);
		lookUD.SetBoth(KeyCode.W, KeyCode.S);
		lookLR.SetBoth(KeyCode.D, KeyCode.A);
		
	}
	void Start () {
		vert = new Axis (KeyCode.W, KeyCode.S);//should really be using player prefs
		hori = new Axis (KeyCode.D, KeyCode.A);
		lookUD = new Axis (KeyCode.UpArrow, KeyCode.DownArrow);
		lookLR = new Axis (KeyCode.RightArrow, KeyCode.LeftArrow);

		type = Type.GetType("InputManager");//setting type to this class, so that methods can be called via string
		SetControlsToBoth ();
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
				keyNotReset=false;}else{keyNotReset=true;}//so am sure that it is the intended key
			if(tmp!=KeyCode.None){//if user has now pressed key
				listenNextKey=false;
				method = typeof(InputManager).GetMethod(mName, new Type[0]);
				Debug.Log("running methiod" +mName);
				method.Invoke(this, new object[0]);
			}
			if(listenForMouse){//if user has set for mouse movement to be listened to
				ListenForMouse();
			}
		}
	}

	public void GetNewInput(string st){
		listenNextKey = true;
		mName=st;}//getting name of the method to be called from the UI calling this function

	public void UpdateMup(){lookUD.SetPos (tmp);}//setting the correct part of the correct axis as stated by the user 
	public void UpdateMDown(){lookUD.SetNeg (tmp);}
	public void UpdateMRight(){lookLR.SetPos (tmp);}
	public void UpdateMLeft(){lookLR.SetNeg (tmp);}
	public void UpdateForward(){vert.SetPos (tmp);}
	public void UpdateBackward(){vert.SetNeg (tmp);}
	public void UpdateRight(){
				hori.SetPos (tmp);
		}//Debug.Log("I was run!!");}
	public void UpdateLeft(){hori.SetNeg (tmp);}

	public void UpdateMup  (string st){lookUD.SetToAxis(st);}
	public void UpdateMDown(string st){lookUD.SetToAxis(st);Debug.Log("string ver called with axis: "+st);}//setting the correct axis to the correct 
	public void UpdateMRight(string st){lookLR.SetToAxis(st);Debug.Log("string ver called with axis: "+st);}
	public void UpdateMLeft (string st){lookLR.SetToAxis(st);Debug.Log("string ver called with axis: "+st);}
	public void UpdateForward (string st){vert.SetToAxis(st);Debug.Log("string ver called with axis: "+st);}
	public void UpdateBackward(string st){vert.SetToAxis(st);Debug.Log("string ver called with axis: "+st);}
	public void UpdateRight(string st){hori.SetToAxis(st);Debug.Log("string hori right with axis: "+st);}
	public void UpdateLeft (string st){hori.SetToAxis(st);Debug.Log("string hori left with axis: "+st);}

	
	public float Vertical(){return vert.Move();}
	public float Horizontal(){return hori.Move ();}
	public float LookUD(){return lookUD.Move ();}
	public float LookLR(){return lookLR.Move ();}

	public void SetListenForMouse(){listenForMouse = !listenForMouse;}//flipping whether the mouse is listened for
	private void ListenForMouse(){
		if(Input.GetAxis("Mouse X")>0 && Input.GetAxis("Mouse X")> Mathf.Abs(Input.GetAxis("Mouse Y"))){// if right is more pronounced than up or down
			Debug.Log("rmouse");
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string)});
			method.Invoke(this, new object[]{"Mouse X"});
		}
		if(Input.GetAxis("Mouse X")<0 && Input.GetAxis("Mouse X")< -Mathf.Abs(Input.GetAxis("Mouse Y"))){ // if left is more pronounced than up or down
			Debug.Log("lmouse");
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string)});
			method.Invoke(this, new object[]{"Mouse X"});
		}
		if(Input.GetAxis("Mouse Y")>0 && Input.GetAxis("Mouse Y")> Mathf.Abs(Input.GetAxis("Mouse X"))){// if up is more pronounced than left or right
			Debug.Log("upouse");
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string)});
			method.Invoke(this, new object[]{"Mouse Y"});
		}
		if(Input.GetAxis("Mouse Y")<0 && Input.GetAxis("Mouse Y")< -Mathf.Abs(Input.GetAxis("Mouse X"))){// if up is more pronounced than left or right
			Debug.Log("dmouse");
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string)});
			method.Invoke(this, new object[]{"Mouse Y"});
		}
	}

}
