﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using System;

public class InputManager : MonoBehaviour {
	public enum AxisSide{Right = 1, Left=-1, Up = 1, Down =-1};
	public GameObject controlUI;
	public UIManager ui;
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
		jump.SetToKey(KeyCode.Space);
		shoot.SetToKey(KeyCode.Mouse0);
		reload.SetToKey(KeyCode.R); 
		equipment.SetToKey(KeyCode.Mouse1); 
		weapEqSlot1.SetToKey(KeyCode.Alpha1); 
		weapEqSlot2.SetToKey(KeyCode.Alpha2); 
		weapEqSlot3.SetToKey(KeyCode.Alpha3);
		UpdateDisplay();
	}
	public void SetControlsToBoth(){
		vert.SetToAxis ("Vertical");
		hori.SetToAxis ("Horizontal");
		lookUD.SetToAxis("Mouse Y");
		lookLR.SetToAxis ("Mouse X");
		jump.SetToKey(KeyCode.Space);
		shoot.SetToKey(KeyCode.Mouse0);
		reload.SetToKey(KeyCode.R); 
		equipment.SetToKey(KeyCode.Mouse1); 
		weapEqSlot1.SetToKey(KeyCode.Alpha1); 
		weapEqSlot2.SetToKey(KeyCode.Alpha2); 
		weapEqSlot3.SetToKey(KeyCode.Alpha3); 
		UpdateDisplay();
	}
	public void SetControlsToKeyboardOnly(){
		vert.SetToAxis ("Vertical");
		hori.SetToAxis ("Horizontal");
		lookUD.SetBoth (KeyCode.UpArrow, KeyCode.DownArrow);
		lookLR.SetBoth (KeyCode.RightArrow, KeyCode.LeftArrow);
		jump.SetToKey(KeyCode.Space);
		shoot.SetToKey(KeyCode.Mouse0);
		reload.SetToKey(KeyCode.R); 
		equipment.SetToKey(KeyCode.Mouse1); 
		weapEqSlot1.SetToKey(KeyCode.Alpha1); 
		weapEqSlot2.SetToKey(KeyCode.Alpha2); 
		weapEqSlot3.SetToKey(KeyCode.Alpha3); 
		UpdateDisplay();
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
		ui = GetComponent<UIManager> ();
		controlUI = GameObject.Find ("UI").transform.FindChild ("PauseMenu").FindChild ("PnlSetControls").gameObject;

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
				ui.ActivateInput("PauseUISetControls",true);//re-enabling the input	
				Debug.Log("method name is:" +method.ToString()+" and key is "+tmp);
			}
			if(listenForMouse){//if user has set for mouse movement to be listened to
				ListenForMouse();
			}
		}
	}

	public void GetNewInput(string st){
		Debug.Log ("input being listened for");
		ui.ActivateInput("PauseUISetControls",false);//disabling the input
		listenNextKey = true;
		mName=st;}//getting name of the method to be called from the UI calling this function

	public void UpdateMup()      {lookUD.SetPos     (tmp);UpdateDisplay();}//setting the correct part of the correct axis as stated by the user 
	public void UpdateMDown()    {lookUD.SetNeg     (tmp);UpdateDisplay();}
	public void UpdateMRight()   {lookLR.SetPos     (tmp);UpdateDisplay();}
	public void UpdateMLeft()    {lookLR.SetNeg     (tmp);UpdateDisplay();}
	public void UpdateForward()  {vert.SetPos       (tmp);UpdateDisplay();} //seems as though there would be a much more efficient way of doing this. Seems to be a lot of repetition
	public void UpdateBackward() {vert.SetNeg       (tmp);UpdateDisplay();}
	public void UpdateRight()    {hori.SetPos       (tmp);UpdateDisplay();}
	public void UpdateLeft()     {hori.SetNeg       (tmp);UpdateDisplay();}
	public void UpdateJump()     {jump.SetToKey     (tmp);UpdateDisplay();}
	public void UpdateShoot()    {shoot.SetToKey    (tmp);UpdateDisplay();}
	public void UpdateReload()   {reload.SetToKey   (tmp);UpdateDisplay();}
	public void UpdateEquipment(){equipment.SetToKey(tmp);UpdateDisplay();}
	//string override methods for assigning axis, need to be public to be found via reflection
	public void UpdateMup       (string st, AxisSide posNeg){lookUD.   SetToAxis(st, posNeg);UpdateDisplay();}//lack of "proper" indentation is a small price for such legibility between such similar functions
	public void UpdateMDown     (string st, AxisSide posNeg){lookUD.   SetToAxis(st, posNeg);UpdateDisplay();}//setting the correct axis to the correct 
	public void UpdateMRight    (string st, AxisSide posNeg){lookLR.   SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateMLeft     (string st, AxisSide posNeg){lookLR.   SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateForward   (string st, AxisSide posNeg){vert.     SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateBackward  (string st, AxisSide posNeg){vert.     SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateRight     (string st, AxisSide posNeg){hori.     SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateLeft      (string st, AxisSide posNeg){hori.     SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateJump      (string st, AxisSide posNeg){jump.     SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateShoot     (string st, AxisSide posNeg){shoot.    SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateReload    (string st, AxisSide posNeg){reload.   SetToAxis(st, posNeg);UpdateDisplay();}
	public void UpdateEquipment (string st, AxisSide posNeg){equipment.SetToAxis(st, posNeg);UpdateDisplay();}

	
	public float Vertical(){return vert.Move();}
	public float Horizontal(){return hori.Move ();}
	public float LookUD(){return lookUD.Move ();}
	public float LookLR(){return lookLR.Move ();}

	public void SetListenForMouse(){listenForMouse = !listenForMouse;}//flipping whether the mouse is listened for
	private void ListenForMouse(){
		if(Input.GetAxis("Mouse X")>0 && Input.GetAxis("Mouse X")> Mathf.Abs(Input.GetAxis("Mouse Y"))){// if right is more pronounced than up or down
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string),typeof(AxisSide)});
			method.Invoke(this, new object[]{"Mouse X", AxisSide.Right});
			Debug.Log("Mouse X >");
		}
		if(Input.GetAxis("Mouse X")<0 && Input.GetAxis("Mouse X")< -Mathf.Abs(Input.GetAxis("Mouse Y"))){ // if left is more pronounced than up or down
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string),typeof(AxisSide)});
			method.Invoke(this, new object[]{"Mouse X", AxisSide.Left});
			Debug.Log("Mouse X <");
		}
		if(Input.GetAxis("Mouse Y")>0 && Input.GetAxis("Mouse Y")> Mathf.Abs(Input.GetAxis("Mouse X"))){// if up is more pronounced than left or right
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string),typeof(AxisSide)});
			method.Invoke(this, new object[]{"Mouse Y", AxisSide.Up});
			Debug.Log("Mouse Y >");
		}
		if(Input.GetAxis("Mouse Y")<0 && Input.GetAxis("Mouse Y")< -Mathf.Abs(Input.GetAxis("Mouse X"))){// if up is more pronounced than left or right
			listenNextKey = false;
			method = typeof(InputManager).GetMethod(mName, new Type[]{typeof(string),typeof(AxisSide)});
			method.Invoke(this, new object[]{"Mouse Y", AxisSide.Down});
			Debug.Log("Mouse Y <");
		}
		if (!listenNextKey) {
			ui.ActivateInput("PauseUISetControls",true);//re-enabling the input		
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
	private void UpdateDisplay(){
		controlUI.transform.FindChild ("inptMUP")      .GetComponent<InputField>().text = lookUD.GetPosName();//Debug.Log ("mouse up: "+lookUD.GetPosName());
		controlUI.transform.FindChild ("inptMDOWN")    .GetComponent<InputField>().text = lookUD.GetNegName();//Debug.Log ("mouse down: "+lookUD.GetPosName());
		controlUI.transform.FindChild ("inptMRIGHT")   .GetComponent<InputField>().text = lookLR.GetPosName();//Debug.Log ("mouse right: "+lookLR.GetPosName());
		controlUI.transform.FindChild ("inptMLEFT")    .GetComponent<InputField>().text = lookLR.GetNegName();//Debug.Log ("mouse left: "+lookLR.GetPosName());
		controlUI.transform.FindChild ("inptForward")  .GetComponent<InputField>().text = vert.GetPosName()  ;//Debug.Log ("vert up: "+vert.GetPosName());
		controlUI.transform.FindChild ("inptBackward") .GetComponent<InputField>().text = vert.GetNegName()  ;//Debug.Log ("vert down: "+vert.GetPosName());
		controlUI.transform.FindChild ("inptRight")    .GetComponent<InputField>().text = hori.GetPosName()  ;//Debug.Log ("hori up: "+hori.GetPosName());
		controlUI.transform.FindChild ("inptLeft")     .GetComponent<InputField>().text = hori.GetNegName()  ;//Debug.Log ("hori down: "+hori.GetNegName());
		controlUI.transform.FindChild ("inptJump")     .GetComponent<InputField>().text = jump.GetName()     ;//Debug.Log ("jump: "+jump.GetName() );
		controlUI.transform.FindChild ("inptShoot")    .GetComponent<InputField>().text = shoot.GetName()    ;//Debug.Log ("shoot: "+shoot.GetName() );
		controlUI.transform.FindChild ("inptReload")   .GetComponent<InputField>().text = reload.GetName()   ;//Debug.Log ("reload: "+reload.GetName() );
		controlUI.transform.FindChild ("inptEquipment").GetComponent<InputField>().text = equipment.GetName();//Debug.Log ("equipment: "+equipment.GetName());

	}

	public bool Jump(){return jump.GetUsed();}
	public bool Shoot(){return shoot.GetUsed();} 
	public bool Reload(){return reload.GetUsed();}
	public bool UseEquipment(){return equipment.GetUsed();}
}                 
