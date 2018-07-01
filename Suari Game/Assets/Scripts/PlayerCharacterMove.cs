using UnityEngine;
using System.Collections;


public class PlayerCharacterMove : MonoBehaviour
{
	[Header("References")]
	public Camera mainCam;			
	private Rigidbody rb;
	private CharacterHandler characterHandler;

	public Transform groundCheckPos;            //positions from where raycasting down to check if the player is grounded
	private Transform[] groundCheckers;
	private Quaternion screenMovementSpace;


	[Header("Movement")]
	[SerializeField]
	private string groundStatus;
	public float maxMoveSpeed = 10;				//moving on X and Z axis, as in ground
	private Vector3 direction;
	private Vector3 moveDirection;
	private Vector3 lastMoveDirection;
	private Vector3 screenMovementForward;
	private Vector3 screenMovementRight;

	private Vector3 movingObjSpeed;

	//editor values overwrites set values, check code for originals
	[SerializeField]
	private bool isGrounded;
	

	private float curAccel;
	private float curDecel;
	private float curRotateSpeed;

	public float groundAcceleration = 50f;
	public float groundDeceleration = 10f;

	public float airAcceleration = 20f;
	public float airDeceleration = 2f;

	private float inputMagnitude;
	public float iceAcceleration = 100f;
	public float iceDeceleration = 2f;

	[Range(0f, 4f)]
	public float rotateSpeed = 1f;
	public float airRotateSpeed = 0.25f;

	[Header("Slope Settings")]
	public float allowedSlopeAngle = 50;					//if slope angle is higher then you...
	public float slideAmount = 40;                          //will slide as fast as this down
	private float calculatedSlopeAngle;

	//jumping
	public Vector3 jumpForce = new Vector3(0, 15, 0);		//normal jump force, editable
	public float jumpMargin = 0.1f;							//height before hitting ground and you can still jump

	//counters
	private float airPressTime;
	private float groundedCount;




	void Awake()
	{
		if (!groundCheckPos)
		{
			Debug.LogWarning("NOTICE: No groundchecking found!");
		}

		//set tag for a player and camera to avoid editor changes
		if (tag != "Player")
		{
			tag = "Player";
			Debug.LogWarning("Player had no tag so it has been set");
		}

		mainCam = Camera.main;

		if (mainCam.tag != "MainCamera")
		{
			tag = "MainCamera";
			Debug.LogWarning("Camera had no tag so it has been set");
		}

		characterHandler = GetComponent<CharacterHandler>();
		rb = GetComponent<Rigidbody>();

		//get child objects of groundCheckPos, used to ray downwards to check isGrounded
		groundCheckers = new Transform[groundCheckPos.childCount];

		for (int i = 0; i < groundCheckers.Length; i++)
		{
			groundCheckers[i] = groundCheckPos.GetChild(i);
		}
	}


	void Update()
	{
		//waking rb for continuous coll checks
		rb.WakeUp();

		JumpProcess();

		//movement and rotates in air/ground (ground has 2 settings; ice and normal)
		if (isGrounded)
		{
			if (groundStatus == "On Ice")
			{
				curAccel = iceAcceleration;
				curDecel = iceDeceleration;
				curRotateSpeed = rotateSpeed;
			}
			else
			{
				curAccel = groundAcceleration;
				curDecel = groundDeceleration;
				curRotateSpeed = rotateSpeed;
			}
		}
		else
		{
			curAccel = airAcceleration;
			curDecel = airDeceleration;
			curRotateSpeed = airRotateSpeed;
		}


		//get movement axis relative to camera
		screenMovementSpace = Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;

		//get inputs
		float horInput = Input.GetAxisRaw("Horizontal");
		float verInput = Input.GetAxisRaw("Vertical");

		//horizontal and vertical input
		direction = (screenMovementForward * verInput) + (screenMovementRight * horInput);
		moveDirection = transform.position + direction;

	}

	//fixedUpdate syncs with physics
	void FixedUpdate()
	{
		isGrounded = IsGrounded();

		//moving and rotating player
		characterHandler.MoveTo(moveDirection, curAccel, 0.7f);

		if (rotateSpeed != 0 && direction.magnitude != 0)
		{
			characterHandler.RotateToDirection(moveDirection, curRotateSpeed * 5);
		}

		characterHandler.ManageSpeed(curDecel, maxMoveSpeed + movingObjSpeed.magnitude);

	}

	//do not allow rb to slide on slopelimit
	void OnCollisionStay(Collision _other)
	{
		//only stop movement on slopes
		if (_other.collider.tag != "Untagged" || isGrounded == false)
		{
			return;
		}
		//no movement, stop player
		if (direction.magnitude == 0 && calculatedSlopeAngle < allowedSlopeAngle && rb.velocity.magnitude < 2)
		{
			rb.velocity = Vector3.zero;
		}
	}


	private bool IsGrounded()
	{
		//get distance to ground
		float dist = GetComponent<Collider>().bounds.extents.y;

		for (int i = 0; i < groundCheckers.Length; i++)
		{
			RaycastHit hit;
			if (Physics.Raycast(groundCheckers[i].position, Vector3.down, out hit, dist + 0.05f))
			{

				//groundcheck ray hits a collider we can walk on
				if (hit.transform.GetComponent<Collider>().isTrigger == false)
				{
					//calculate angle and if angle is higher than allowed push player
					calculatedSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
					
					if (calculatedSlopeAngle > allowedSlopeAngle)
					{
						Vector3 slide = new Vector3(0f, -slideAmount, 0f);
						rb.AddForce(slide, ForceMode.Force);
					}

					if(hit.collider.gameObject.name == "Ice")
					{
						//Debug.Log("we are on ice");
						groundStatus = "On Ice";
						return true;
					}

					//groundcheck raycast hit
					groundStatus = "On Ground";
					return true;
				}
			}
		}

		movingObjSpeed = Vector3.zero;

		//groundcheck raycast did not detect anything
		groundStatus = "On Air";
		return false;
	}


	private void JumpProcess()
	{
		//keep how long we have been on the ground
		if (isGrounded)
		{
			groundedCount = groundedCount * Time.deltaTime;
		}
		else
		{
			groundedCount = 0f;
		}

		//buttonpress time while on air, used to allow jumping on air with margin
		if (Input.GetButtonDown("Jump") && !isGrounded)
		{
			airPressTime = Time.time;
		}

		//grounded and allowing slopelimit 
		if (isGrounded && calculatedSlopeAngle < allowedSlopeAngle)
		{
			if (Input.GetButtonDown("Jump") || airPressTime + jumpMargin > Time.time)
			{
				Jump(jumpForce);
			}
		}
	}

	public void Jump(Vector3 _jumpPower)
	{
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

		rb.AddRelativeForce(_jumpPower, ForceMode.Impulse);

		airPressTime = 0f;
	}

}