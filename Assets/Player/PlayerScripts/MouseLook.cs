using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
//[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	private float rotationY = 0F;
	
	private float headbobSpeed = 30f;
	private float headbobStepCounter = 0f;
	private float headbobAmountX = 40f;
	private float headbobAmountY = 30f;
	private float eyeHeightRatio = 90f;
	private Vector3 parentLastPos;
	public float newLocalPosX;
	public float newLocalPosY;
	private bool gotParent = false;

	void Awake()
	{

		Screen.lockCursor = true;
		//lock cursor 

	}
	void Start ()
	{

		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}

	void Update ()
	{
		headbobbing ();

		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}
	void headbobbing()
	{
		if (transform.parent && !gotParent) {
			parentLastPos = transform.parent.position;
			gotParent = true;}
		if (gotParent /*&& transform.parent.GetComponent<PlayerMovement> ().cc.isGrounded*/) {
			//Debug.Log("localscale before: "+transform.localScale.x+" + "+transform.localScale.y);
			headbobStepCounter += Vector3.Distance(parentLastPos, transform.parent.position) * headbobSpeed;
			/*transform.localPosition.x*///newLocalPosX  = Mathf.Sin(headbobStepCounter * headbobAmountX);//can add current aim ratio later when implementing ads (just add "*currentAimRatio")
			/*transform.localPosition.y*///newLocalPosY = (Mathf.Cos(headbobStepCounter *2)* headbobAmountY * -1)+ (transform.parent.localScale.y* eyeHeightRatio)-(transform.localScale.y/2);
			//-1 just before headbobamounty so that it starts at the bottom rather than the top of the cos wave
			//-(transform.localScale.y/2) so that the equation starts at the feet rather than the center of the character
			transform.localScale = new Vector3(newLocalPosX,newLocalPosY);
//			Debug.Log("localscale after: "+transform.localScale.x+" + "+transform.localScale.y);
//			Debug.Log("meant to be: "+transform.localScale.x+" + "+transform.localScale.y);
			parentLastPos = transform.parent.position;
		}
	}

}