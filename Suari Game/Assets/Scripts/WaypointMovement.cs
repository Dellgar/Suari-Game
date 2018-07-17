using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : MonoBehaviour {


	private Rigidbody rb;
	//private CharacterHandler characterHandler;

	[Header("Waypoints")]
	public List<Vector3> waypoints = new List<Vector3>();
	public float delayOnWaypoint;

	[Range(0.1f, 4f)]
	public float arrivalProximity;
	private bool hasArrived = false;
	private float arrivalTime;

	[SerializeField]
	private int currentWaypoint;

	[Header("Movement")]
	public Type movementType;
	public enum Type { Once, Loop, PingPong }
	// Once; Go throught the waypoints once and stop at the end
	// Loop; Looping so when the final waypoint is reached head towards the first waypoint again
	// PingPong; When final waypoint is reached, turn around and head to waypoint that was before the final waypoint

	private Vector3 movingDir;
	public float movingSpeed;
	private bool forward = true;



	//setup
	void Awake()
	{
		rb = GetComponent<Rigidbody>();

		rb.isKinematic = true;
		rb.useGravity = false;
		rb.interpolation = RigidbodyInterpolation.Interpolate;

	}


	void Update()
	{

		//when arrived set a course to next
		if (waypoints.Count > 0)
		{
			if (!hasArrived)
			{
				if (Vector3.Distance(transform.position, waypoints[currentWaypoint]) < arrivalProximity)
				{
					arrivalTime = Time.time;
					hasArrived = true;
				}
			}
			else
			{
				if (Time.time > (arrivalTime + delayOnWaypoint))
				{
					NextWaypoint();
					hasArrived = false;
				}
			}
		}

	}

	void FixedUpdate()
	{
			if (!hasArrived && waypoints.Count > 0)
			{
				movingDir = waypoints[currentWaypoint] - transform.position;

				rb.MovePosition(transform.position + (movingDir.normalized * movingSpeed * Time.fixedDeltaTime));
			}
	}

	void NextWaypoint()
	{
		if (movementType == Type.Once)
		{
			currentWaypoint++;

			if (currentWaypoint == waypoints.Count)
				enabled = false;
		}

		if (movementType == Type.Loop)
		{
			if(currentWaypoint == waypoints.Count - 1)
			{
				currentWaypoint = 0;
			}
			else
			{
				currentWaypoint += 1;
			}
		}

		if (movementType == Type.PingPong)
		{
			if (currentWaypoint == waypoints.Count - 1)
			{
				forward = false;
			}
			else if (currentWaypoint == 0)
			{
				forward = true;
			}

			if (forward)
			{
				currentWaypoint += 1;
			}
			else
			{
				currentWaypoint -= 1;
			}
		}
	}

	//draw gizmos for waypoint indicators
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		UnityEditor.Handles.color = Color.red;
		
		for (int i = 0; i < waypoints.Count; i++)
		{
			Gizmos.DrawSphere(waypoints[i], 1f);

			Vector3 gizmoLabel = waypoints[i] + new Vector3(0f, 2f, 0f);
			UnityEditor.Handles.Label( gizmoLabel, i.ToString() );
		}




	}
}
