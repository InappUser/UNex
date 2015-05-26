using UnityEngine;
using System.Collections;

public class Axis{
	private KeyCode pos;
	private KeyCode neg;
	
	private float axisDown = 0f;
	private float axisAccelMulti = 3f;
	private float axisAccel =2f;
	
	private bool goingPos =false;
	private bool goingNeg = false;
	private bool isSetToAxis = false;
	private string setAxis;	

	public Axis(KeyCode p, KeyCode n){
		pos = p;
		neg = n;
	}
	public void SetPos(KeyCode p){
		pos = p;
		isSetToAxis = false;
	}
	public void SetNeg(KeyCode n){
		neg = n;
		isSetToAxis = false;
	}
	public void SetBoth(KeyCode p, KeyCode n){
		pos = p;
		neg = n;
		isSetToAxis = false;
	}
	public void SetToAxis (string axis){
		setAxis = axis;
		isSetToAxis = true;
	}	
	public float Move(){
		if(!isSetToAxis){
			if (Input.GetKey(pos) && !Input.GetKey (neg)){
				goingPos = true;
				goingNeg = false;
			}
			if (Input.GetKey(neg) && !Input.GetKey (pos)){
				goingNeg = true;
				goingPos = false;
			}
			
			//with vertical movement keys down
			if (Input.GetKey(pos) && !goingNeg) {//if wanting to go pos and not already going back
				axisAccel = axisDown<0 ? axisAccelMulti : axisAccel;//if going pos, but have negative speed (was going neg), increase acceleration
				axisDown = axisDown >1 ? 1 : axisDown + (Time.deltaTime*axisAccel);
				axisDown = Mathf.Clamp(axisDown, axisDown, 1); //doesn't go beyond max speed
			
			}else if (Input.GetKey (neg) && !goingPos) {//if wanting to go back and not already going pos
				axisAccel = axisDown>0 ? axisAccelMulti : axisAccel;//if going neg, but have positive speed (was going pos), increase acceleration
				axisDown = axisDown <-1 ? -1 : axisDown - (Time.deltaTime*axisAccel);
				axisDown = Mathf.Clamp(axisDown, -1, axisDown);//making sure that the number does not go out of bounds
			
			}
			//without vertical movement keys down
			else if(axisDown>0){//if more than 0, count down until 0 --i.e. was pos now nothing
				axisDown = axisDown <=0 ? 0 : axisDown - (Time.deltaTime*axisAccel);
				axisDown = Mathf.Clamp(axisDown, 0, 1);

			}else if(axisDown<0){//if less than 0, count up until 0 ---making a smooth stop  --i.e. was neg now nothing
				axisDown = axisDown >=0 ? 0 : axisDown + (Time.deltaTime*axisAccel);
				axisDown = Mathf.Clamp(axisDown, -1, 0);}//making sure that the number does not go out of bounds

			return axisDown;}
		else{
			//Debug.Log("axis reads: "+Input.GetAxis(setAxis));
			return Input.GetAxis(setAxis);
		}
		
	}
}
