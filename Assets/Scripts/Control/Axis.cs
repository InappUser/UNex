using UnityEngine;
using System.Collections;

public class Axis{
	private Key posKey;//using my Key class to make it more dynamic
	private Key negKey;
	
	private float axisDown = 0f;
	private float axisAccelMulti = 3f;
	private float axisAccel =2f;
	
	private bool goingPos =false;
	private bool goingNeg = false;
	private bool axisBothSides = false;
	private string bothAxis;

	public Axis(KeyCode p, KeyCode n){
		posKey = new Key(p);
		negKey = new Key(n);
		axisBothSides = false;
	}
	public Axis(string axis, InputManager.AxisSide posNeg){
		if (posNeg>0) {
			posKey.SetToAxis(axis,posNeg);}
		else{
			negKey.SetToAxis(axis,posNeg);}
	}
	public void SetPos(KeyCode p){
		posKey.SetToKey(p);
		axisBothSides = false;
	}
	public void SetNeg(KeyCode n){
		negKey.SetToKey(n);
		axisBothSides = false;
	}
	public void SetBoth(KeyCode p, KeyCode n){
		posKey.SetToKey(p);
		negKey.SetToKey(n);
		axisBothSides = false;
	}
	public void SetToAxis (string axis, InputManager.AxisSide posNeg){
		if (posNeg>0) {
			posKey.SetToAxis(axis,posNeg);}
		else{
			negKey.SetToAxis(axis,posNeg);}
	}
	public void SetToAxis (string axis){
		bothAxis = axis;
		axisBothSides = true;
	}	
	public float Move(){
		if(axisBothSides){
			return Input.GetAxis(bothAxis);
		}else{
			if (posKey.GetUsed() && !negKey.GetUsed()){
				goingPos = true;
				goingNeg = false;
			}
			if (negKey.GetUsed() && !posKey.GetUsed()){
				goingNeg = true;
				goingPos = false;
			}
			
			//with vertical movement keys down
			if (posKey.GetUsed() && !goingNeg) {//if wanting to go pos and not already going back
				axisAccel = axisDown<0 ? axisAccelMulti : axisAccel;//if going pos, but have negative speed (was going neg), increase acceleration
				axisDown = axisDown >1 ? 1 : axisDown + (Time.deltaTime*axisAccel);
				axisDown = Mathf.Clamp(axisDown, axisDown, 1); //doesn't go beyond max speed
			
			}else if (negKey.GetUsed() && !goingPos) {//if wanting to go back and not already going pos
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

		
	}
}
