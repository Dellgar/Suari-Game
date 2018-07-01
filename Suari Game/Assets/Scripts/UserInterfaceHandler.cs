using UnityEngine;

public class UserInterfaceHandler : MonoBehaviour {

	public GameObject playerObject;
	//private PlayerCharacterMove playerCharacterMove;
	private Health health;

	void Awake () {
		if(!playerObject)
		{
			Debug.LogWarning("Player object is left empty on " + gameObject.name);
		}

		health = playerObject.GetComponent<Health>();
	}
	

	void Update () {
		
	}
}
