using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using System;

public class InputManager : MonoBehaviour {
	public enum ControlScheme{KeyboardOnly,MouseOnly,Both}
	public enum AxisSide{positive = 1, negative=-1};
	
	private ControlScheme controlScheme;
	private Key jump;
	private Key shoot; 
	private Key reload; 
	private Key equipment; 
	private Key weapEqSlot1; 
	private Key weapEqSlot2; 
	private Key weapEqSlot3; 

	private Axis lookUD,lookLR,vert,hori;
	private KeyCode tmp;//getting most recent key pressed
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
		jump = new Key (KeyCode.Space);
		shoot = new Key (KeyCode.Mouse0);
		reload = new Key (KeyCode.R); 
		equipment = new Key (KeyCode.Mouse1); 
		weapEqSlot1 = new Key (KeyCode.Alpha1); 
		weapEqSlot2 = new Key (KeyCode.Alpha2); 
		weapEqSlot3  = new Key (KeyCode.Alpha3); 

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
				tmp = KeyCode.None;
				keyNotReset=false;}else{keyNotReset=true;}//so am sure that it is the intended key
			if(tmp!=KeyCode.None || MouseWasClicked()){//if user has now pressed key
				listenNextKey=false;
				method = typeof(InputManager).GetMethod(mName, new Type[0]);
				method.Invoke(this, new object[0]);
				Debug.Log("method name is:" +method.ToString()+" and key is "+tmp);
			}
			if(listenForMouse){//if user has set for mouse movement to be listened to
				ListenForMouse();
			}
		}
	}

	public void GetNewInput(string st){
		listenNextKey = true;
		mName=st;}//getting name of the method to be called from the UI calling this function

	public void UpdateMup()      {lookUD.SetPos (tmp);}//setting the correct part of the correct axis as stated by the user 
	public void UpdateMDown()    {lookUD.SetNeg (tmp);}
	public void UpdateMRight()   {lookLR.SetPos (tmp);}
	public void UpdateMLeft()    {lookLR.SetNeg (tmp);}
	public void UpdateForward()  {vert.SetPos (tmp);} //seems as though there would be a much more efficient way of doing this. Seems to be a lot of repetition
	public void UpdateBackward() {vert.SetNeg (tmp);}
	public void UpdateRight()    {hori.SetPos (tmp);}
	public void UpdateLeft()     {hori.SetNeg (tmp);}
	public void UpdateJump()     {jump.SetToKey(tmp); Debug.Log ("key is " + tmp);jump.shouldListen = true;}
	public void UpdateShoot()    {shoot.SetToKey(tmp);}
	public void UpdateReload()   {reload.SetToKey(tmp);}
	public void UpdateEquipment(){equipment.SetToKey(tmp);}

	public void UpdateMup       (string st, AxisSide posNeg){lookUD.   SetToAxis(st, posNeg);}
	public void UpdateMDown     (string st, AxisSide posNeg){lookUD.   SetToAxis(st, posNeg);}//setting the correct axis to the correct 
	public void UpdateMRight    (string st, AxisSide posNeg){lookLR.   SetToAxis(st, posNeg);}
	public void UpdateMLeft     (string st, AxisSide posNeg){lookLR.   SetToAxis(st, posNeg);}
	public void UpdateForward   (string st, AxisSide posNeg){vert.     SetToAxis(st, posNeg);}
	public void UpdateBackward  (string st, AxisSide posNeg){vert.     SetToAxis(st, posNeg);}
	public void UpdateRight     (string st, AxisSide posNeg){hori.     SetToAxis(st, posNeg);}
	public void UpdateLeft      (string st, AxisSide posNeg){hori.     SetToAxis(st, posNeg);}
	public void UpdateJump      (string st, AxisSide posNeg){jump.     SetToAxis(st, posNeg);}
	public void UpdateShoot     (string st, AxisSide posNeg){shoot.    SetToAxis(st, posNeg);}
	public void UpdateReload    (string st, AxisSide posNeg){reload.   SetToAxis(st, posNeg);}
	public void UpdateEquipment (string st, AxisSide posNeg){equipment.SetToAxis(st, posNeg);}

	
	public float Vertical(){return vert.Move();}
	public float Horizontal(){return hori.Move ();}
	public float LookUD(){return lookUD.Move ();}
	public float LookLR(){return lookLR.Move ();}

	public void SetListenForMouse(){listenForMouse = !listenForMouse;}//flipping whether the mouse is listened for
	private void ListenForMouse(){
		if(Input.GetAxis("Mouse X")>0 && Input.GetAxis("Mouse X")> Mathf.Abs(Input.GetAxis("Mouse Y"))){// if right is more pronounced than up or down
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string),typeof(AxisSide)});
			method.Invoke(this, new object[]{"Mouse X", AxisSide.positive});
			Debug.Log("Mouse X >");
		}
		if(Input.GetAxis("Mouse X")<0 && Input.GetAxis("Mouse X")< -Mathf.Abs(Input.GetAxis("Mouse Y"))){ // if left is more pronounced than up or down
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string),typeof(AxisSide)});
			method.Invoke(this, new object[]{"Mouse X", AxisSide.negative});
			Debug.Log("Mouse X <");
		}
		if(Input.GetAxis("Mouse Y")>0 && Input.GetAxis("Mouse Y")> Mathf.Abs(Input.GetAxis("Mouse X"))){// if up is more pronounced than left or right
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string),typeof(AxisSide)});
			method.Invoke(this, new object[]{"Mouse Y", AxisSide.positive});
			Debug.Log("Mouse Y >");
		}
		if(Input.GetAxis("Mouse Y")<0 && Input.GetAxis("Mouse Y")< -Mathf.Abs(Input.GetAxis("Mouse X"))){// if up is more pronounced than left or right
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string),typeof(AxisSide)});
			method.Invoke(this, new object[]{"Mouse Y", AxisSide.negative});
			Debug.Log("Mouse Y <");
		}
	}
	private bool MouseWasClicked(){//Event.current.keyCode will not return mouse clicks of any kind
		if(Input.GetKey(KeyCode.Mouse0)){tmp = KeyCode.Mouse0;return true;}//maybe bad practice to not indent, but find it so much easier to read this way
		if(Input.GetKey(KeyCode.Mouse1)){tmp = KeyCode.Mouse1;return true;}
		if(Input.GetKey(KeyCode.Mouse2)){tmp = KeyCode.Mouse2;return true;}
		if(Input.GetKey(KeyCode.Mouse3)){tmp = KeyCode.Mouse3;return true;}
		if(Input.GetKey(KeyCode.Mouse4)){tmp = KeyCode.Mouse4;return true;}
		if(Input.GetKey(KeyCode.Mouse5)){tmp = KeyCode.Mouse5;return true;}
		if(Input.GetKey(KeyCode.Mouse6)){tmp = KeyCode.Mouse6;return true;}
		return false;
	}

	public bool Jump(){return jump.GetUsed();}
	public bool Shoot(){return shoot.GetUsed();} 
	public bool Reload(){return reload.GetUsed();}
	public bool UseEquipment(){return equipment.GetUsed();}
}                 
