using System.Collections;
using UnityEngine;

public class Obstacles : MonoBehaviour {

	private AttackBehavior attackBehaviour;

	public int dmg = 1;
	public float pushForce = 1f;
	public float pushHeight = 1f;

	public bool triggerEnter;
	public bool collisionEnter = true;

	public string[] tagsToHit;                //tags that are here can take the effect (dmg or pushforces)


	void Awake ()
	{
		attackBehaviour = GetComponent<AttackBehavior>();
	}

	//attack on collision
	void OnCollisionEnter(Collision coll)
	{
		if (!collisionEnter)
		{
			return;
		}

		for (int i = 0; i < tagsToHit.Length; i++)
		{
			if (coll.transform.tag == tagsToHit[i])
			{
				attackBehaviour.NormalAttack(coll.gameObject, dmg, pushHeight, pushForce);
			}
		}

	}

	//attack on trigger
	void OnTriggerEnter(Collider otherObject)
	{
		if (!triggerEnter)
		{
			return;
		}

		for (int i = 0; i < tagsToHit.Length; i++)
		{
			if (otherObject.transform.tag == tagsToHit[i])
			{			
				attackBehaviour.NormalAttack(otherObject.gameObject, dmg, pushHeight, pushForce);
			}
		}

	}
}
