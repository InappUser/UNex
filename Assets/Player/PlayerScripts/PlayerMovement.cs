using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	//This component is only enabled for "my player" (the player that belongs to the local client machine)
	public float jumpSpeed = 40f;
	public float speed = 20f;
	public float playerGravity = 5;
	public CharacterController cc;
	public bool useMyVertical =true;
	public bool useMyHorizontal =true;

	private InputManager inputMan;
	private Animator anim;
	private Vector3 direction = Vector3.zero; //forward/back & left/right
	private Vector3 distance;
	private float verticalVelocity = 0f;
	private float playerDeathHeight = -60f;
	private float vertical;
	private float horizontal;


	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
		anim = GetComponent<Animator> ();
		PlayerScore.enemyStaticsTotal = (GameObject.FindGameObjectsWithTag ("EnemyStatic").Length);//very messy, though does not work when run after player has spawned
		//UnityEngine.Debug.Log ("enemies = "+PlayerScore.enemyStaticsTotal);
		inputMan = GameObject.FindObjectOfType<InputManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		XYMovement ();
		CheckIfFalling();
		if (cc.isGrounded && Input.GetButton("Jump")) {
			verticalVelocity = jumpSpeed;
		}


	}
	void XYMovement()
	{
		//in order to allow for the forward, left right bcakwards directions to be relative to the mouse look di
		//direction, just need to times by players rotation
		//wasd f,b,l,f is stored in direction
		//Debug.Log ("native vertical" + Input.GetAxis ("Vertical"));
		//Debug.Log ("my vertical" + inputMan.Vertical());
		vertical = useMyVertical ? inputMan.Vertical () : Input.GetAxis ("Vertical");
		horizontal = useMyHorizontal ? inputMan.Horizontal () : Input.GetAxis ("Horizontal");
		direction = transform.rotation * new Vector3 (horizontal,0,vertical);//ensuring that
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
	
	void CheckIfFalling(){
		if (transform.position.y < playerDeathHeight) {
			Health h = gameObject.GetComponent<Health>();
			if(PhotonNetwork.offlineMode){
				h.TakeDamage(gameObject.name, Time.deltaTime * 100f);}
			else{
				h.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.AllBuffered,gameObject.name, Time.deltaTime * 60f);
			}
		}
	}


	void FixedUpdate () {//as this updates once per physics loops (as oposed to every frame) do all physics stuff...
		//i.e. movement here
		//transform.position
		distance = direction*speed*Time.deltaTime;

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
			{//an absolute number ignores whether it is a negative, always returns positive
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
