using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	//This component is only enabled for "my player" (the player that belongs to the local client machine)
	public float jumpSpeed = 40f;
	public float speed = 20f;
	public float playerGravity = 5;
	public float jumpCamMove = .1f;
	public float jumpLerpTime = 1f;
	public CharacterController cc;

	private float verticalVelocity = 0f;
	private float cameraJumpAdjustment = 0f;
	private bool jumped = false;
	private bool jumpCheck = false;
	private Vector3 jumpNewCamHeight;
	private Vector3 direction = Vector3.zero; //forward/back & left/right
	private Animator anim;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		XYMovement ();

		if(cameraJumpAdjustment != 0)
		{
			cameraJumpAdjustment = 0;
		}

		if (cc.isGrounded && Input.GetButton("Jump")) {
			verticalVelocity = jumpSpeed;
			jumped = true;
		}
		if (transform.position.y < -60) {
			//Debug.Log("falling");
			Health h = gameObject.GetComponent<Health>();
			if(PhotonNetwork.offlineMode){
				h.TakeDamage(gameObject, Time.deltaTime * 100f);}
			else{
				h.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.AllBuffered,gameObject, Time.deltaTime * 60f);
			}
			//Debug.Log("damage: "+ Time.deltaTime  * 40f);
		}
		if(!cc.isGrounded && jumped){
			cameraJumpAdjustment = jumpCamMove;
			jumped = false;
			jumpCheck =true;
		}
		else if(cc.isGrounded && jumpCheck){
			cameraJumpAdjustment = -jumpCamMove;
			//jumped = true;
			jumpCheck =false;
		}
		if(cameraJumpAdjustment != 0){
			jumpNewCamHeight = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y + cameraJumpAdjustment, Camera.main.transform.position.z);
			Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, jumpNewCamHeight, .002f  );
				
		}
	}
	void XYMovement()
	{
		//in order to allow for the forward, left right bcakwards directions to be relative to the mouse look di
		//direction, just need to times by players rotation
		//wasd f,b,l,f is stored in direction
		direction = transform.rotation * new Vector3 (Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));//ensuring that
		//diagonals are not faster
		//the input is captured in the normal update because want it to feel as responsive as possible
		//tis good practice
		
		if (direction.magnitude > 1)
			direction = direction.normalized;
		
		//normalised so is able to go between 1 and 0 but not over 1
		anim.SetFloat ("FSpeed", Input.GetAxis("Vertical"));
		//forward/backwards
		anim.SetFloat ("SSpeed", Input.GetAxis("Horizontal"));
		//left/right
		//setting the correct animation to play
		//handling jump
		//verticalVelocity += Physics.gravity.y * Time.deltaTime *1.5f;
	}


	void FixedUpdate () {//as this updates once per physics loops (as oposed to every frame) do all physics stuff...
		//i.e. movement here
		//transform.position
		Vector3 distance = direction*speed*Time.deltaTime;

		if(cc.isGrounded && verticalVelocity <0)
		{//if on ground and are not starting to jump(vertical velocity not positive)
			anim.SetBool("Jump",false);
			//ensure jump anim not playing

			verticalVelocity = Physics.gravity.y * Time.deltaTime*playerGravity;
			//ensuring that arent building up velocity when on the ground
			//and that isGrounded is below 0
		}
		else{
			//either are not touching the ground or are starting to jump

			//to ensure that the jump animation is not played while walking down a slope
			//the below equation is the height at which the jump will start 
			if(Mathf.Abs(verticalVelocity) > jumpSpeed *0.75f)//without this will go into jump anim when walking down slope
			{//an absolute number ignores whether it is a negative, allways returns positive
				anim.SetBool("Jump",true);

			}
			//apply gravity to the character; timesed by time ofc
			verticalVelocity += Physics.gravity.y * Time.deltaTime* playerGravity;
		}


		distance.y = verticalVelocity * Time.deltaTime;

		cc.Move (distance);
		//need to remember move
		//move wants a distance where simple move just wants a speed
		//so if were to use cc.move would need to times direction by speed and then times by tim.deltatime also
		// simplemove will aso incorporate gravity 

	}
}
