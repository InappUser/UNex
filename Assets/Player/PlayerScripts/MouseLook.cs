using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
   float m_SensitivityX = 15F;
   float m_SensitivityY = 10F;

    public float SensitivityX { set { m_SensitivityX = value;} }
    public float SensitivityY { set { m_SensitivityX = value; } }

    float minimumX = -360F;
	float maximumX = 360F;

	float minimumY = -80F;
	float maximumY = 80F;

	public bool usemyUDmouse = true;
	public bool usemyLRmouse = true;

	private float mouseUD;
	private float mouseLR;
	private InputManager inputMan;
	private float rotationY = 0F;
	private float rotationX = 0.0f;
	
	private float headbobSpeed = 30f;
	private float headbobStepCounter = 0f;
	private float headbobAmountX = 40f;
	private float headbobAmountY = 30f;
	private float eyeHeightRatio = 90f;
	private Vector3 parentLastPos;
	public float newLocalPosX;
	public float newLocalPosY;
	private bool gotParent = false;
	private GameObject sensScroll;//only need one because the x and the y of the characters view are controlled by different instances of the script
	private Scrollbar scroll;
	private bool foundSens = false;
	private bool resetRot = false;
	
	private Quaternion zero;
	
	void Awake()
	{
		zero = new Quaternion(0,0,0,0);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		//lock cursor 


	}
	void Start ()
	{
		inputMan = GameObject.FindObjectOfType<InputManager> ();
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}

	void Update ()
	{
       // Debug.Log("m-ud: "+ inputMan.LookUD());
		mouseUD = /*usemyUDmouse ? */inputMan.LookUD ()/* : Input.GetAxis ("Mouse Y")*/;
		mouseLR = /*usemyLRmouse ? */inputMan.LookLR () /*: Input.GetAxis ("Mouse X")*/;

		headbobbing ();
		AlterRotation ();
		FindSensitivity ();
		if (Input.GetKeyDown (KeyCode.B)) {
			Debug.Log("pressed b");
			resetRot = true;
		}

	}

	void AlterRotation()
	{
		if (resetRot) {
			transform.rotation = zero;
			rotationY = 0;
			rotationX = 0;
			resetRot = false;
			return;
		}

		if (axes == RotationAxes.MouseXAndY)
		{//moving view left to right
			resetRot = false;
			rotationX = transform.localEulerAngles.y + mouseLR * /*5f*/m_SensitivityX;
            //Debug.Log("rot y 1 " + rotationY);
            rotationY += mouseUD * /*.75f;//*/m_SensitivityY;
           // Debug.Log("rot y 2" + rotationY);
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            //Debug.Log("rot y 3" + rotationY);
            //Debug.Log("min: " + minimumY + "max "+ maximumY);
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

		}
		else if (axes == RotationAxes.MouseX)
		{
			resetRot = false;
            rotationY += mouseLR * /*.75f;//*/m_SensitivityY;
            //rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            transform.localEulerAngles = new Vector3(0,rotationY, 0);
        }
		else
		{//moving view up and down
           // Debug.Log("rot y 1 " + mouseUD);
            resetRot = false;
			rotationX += mouseUD * /*3f;*/m_SensitivityY;
           // Debug.Log("rot y 2 " + rotationX+" sensy "+ m_SensitivityY);
            rotationX = Mathf.Clamp (rotationX, minimumY, maximumY);
            //Debug.Log("rot y 2 " + rotationX);
            transform.localEulerAngles = new Vector3(-rotationX, 0, 0);
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
	void FindSensitivity()
	{
		if (!foundSens) {
			try {
				foundSens = true;
				//				if (gameObject.name == "FPS_Player") {
				//					Debug.Log ("found");
				//					//sensScroll = GameObject.Find("ScrlSensX").GetComponent<Scrollbar>();
				//				} 
				//else {
				
				sensScroll = GameObject.Find ("ScrlSensY");
				if (sensScroll.GetComponentInChildren<Scrollbar> ()) {
					scroll = sensScroll.GetComponentInChildren<Scrollbar> ();
				};
				//}
			} 
			catch {
				//Debug.Log ("Nope");
				foundSens = false;
			}
		}
		
		if (foundSens) {
			ChangeSensitivity(scroll.value);		
		}
	}
	
	void ChangeSensitivity(float newSensitivity)
	{
		if(gameObject.name == "FPS_Player(Clone)"){//if name is "FPS_Player(Clone)" then it is the body making the request, so the x sens is the one that needs changing
			m_SensitivityX = (newSensitivity *15f);
		}else{
			m_SensitivityY = (newSensitivity *15f);
		}
	}

	public void ResetRotation(Quaternion rotation)
	{
		zero = rotation;
		//Debug.Log ("resetting");
		resetRot = true;
	}

}