using UnityEngine;

public class CharacterHandler : MonoBehaviour {


	private Rigidbody rb;

	[HideInInspector]
	public Vector3 currentSpeed;

	[HideInInspector]
	public float distanceToTarget;


	void Awake()
	{
		rb = GetComponent<Rigidbody>();

		rb.interpolation = RigidbodyInterpolation.Interpolate;
		rb.constraints = RigidbodyConstraints.FreezeRotation;

		//physics material for character
		if (GetComponent<Collider>().material.name == "Default (Instance)")
		{
			PhysicMaterial pMat = new PhysicMaterial();

			pMat.name = "Frictionless";
			pMat.frictionCombine = PhysicMaterialCombine.Multiply;
			pMat.bounceCombine = PhysicMaterialCombine.Multiply;
			pMat.dynamicFriction = 0f;
			pMat.staticFriction = 0f;

			GetComponent<Collider>().material = pMat;

			Debug.LogWarning("Created physics material");
		}
	}

	//move rigidbody to targetpos and return bool
	public bool MoveTo(Vector3 destination, float acceleration, float stopDistance)
	{
		Vector3 relativePos = (destination - transform.position);

		relativePos.y = 0;


		//how far the target is
		distanceToTarget = relativePos.magnitude;
		//Debug.Log(distanceToTarget + " _dist");

		if (distanceToTarget <= stopDistance)
		{
			//arrived to location
			return true;
		}
		else
		{
			rb.AddForce(relativePos.normalized * acceleration * Time.deltaTime, ForceMode.VelocityChange);
			return false;
		}

	}

	//rotate rigidbody to characters velocity its going to
	public void RotateToVelocity(float turnSpeed)
	{
		Vector3 direction;
	
		direction = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		


		if (direction.magnitude > 0.1)
		{
			Quaternion direction_Q = Quaternion.LookRotation(direction);
			Quaternion slerp = Quaternion.Slerp(transform.rotation, direction_Q, direction.magnitude * turnSpeed * Time.deltaTime);

			rb.MoveRotation(slerp);
		}
	}

	//rotate rb to a set direction
	public void RotateToDirection(Vector3 lookDir, float turnSpeed)
	{
		Vector3 charPos = transform.position;

		charPos.y = 0;
		lookDir.y = 0;

		Vector3 charFacingDir = lookDir - charPos;
		Quaternion charFacingDir_Q = Quaternion.LookRotation(charFacingDir);
		Quaternion slerp = Quaternion.Slerp(transform.rotation, charFacingDir_Q, turnSpeed * Time.deltaTime);

		rb.MoveRotation(slerp);
	}

	//friction to rigidbody and not going past of max allowed speed
	public void ManageSpeed(float deceleration, float maxSpeed)
	{ 

		currentSpeed = rb.velocity;
		
		currentSpeed.y = 0;
		

		if (currentSpeed.magnitude > 0)
		{
			rb.AddForce((currentSpeed * -1) * deceleration * Time.deltaTime, ForceMode.VelocityChange);

			if (rb.velocity.magnitude > maxSpeed)
			{
				rb.AddForce((currentSpeed * -1) * deceleration * Time.deltaTime, ForceMode.VelocityChange);
			}
		}
	}

}
