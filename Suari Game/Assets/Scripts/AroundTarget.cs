using UnityEngine;

public class AroundTarget : MonoBehaviour {

	//public float speed;

	public Transform origo;
	public Vector3 distanceFromTarget = new Vector3(0f, 3.5f, 7);

	public GameObject targetForCameraToFollow;


	void Update () {


		targetForCameraToFollow.transform.position = origo.position;
		targetForCameraToFollow.transform.Translate(distanceFromTarget, Space.Self);

		float axisH = Input.GetAxis("CamHorizontal") * 100f * Time.deltaTime;
		float axisV = Input.GetAxis("CamVertical") * 100f * Time.deltaTime;

		Debug.Log(axisH + " - " + axisV);
		targetForCameraToFollow.transform.RotateAround(origo.position, Vector3.up, axisH);
		targetForCameraToFollow.transform.RotateAround(origo.position, Vector3.right, axisV);

		//horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		//vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

		//transform.RotateAround(Vector3.zero, Vector3.right * horInput);

		//direction = new Vector3(horInput, 0, verInput).normalized;
	}

}
