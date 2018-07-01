using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;
	public GameObject test;
	public float smoothSpeed = 0.7f;
	public Vector3 offset;

	void FixedUpdate()
	{
		Vector3 desiredPosition = target.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
		transform.position = smoothedPosition;

		//transform.LookAt(target, Vector3.forward);

		Vector3 relativePos = target.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);


		float axisH = Input.GetAxis("CamHorizontal") * 20f;
		float axisV = Input.GetAxis("CamVertical") * 20f;

		float axis = Input.GetAxis("CamHorizontal") * 30f * Time.deltaTime;
		transform.RotateAround(target.position, Vector3.up, axis);

		transform.rotation = rotation;

	}
}
