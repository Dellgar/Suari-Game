using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{

	public int currentHealth = 1;                   //current health or durability
	public bool canRespawn;                         //should this object respawn
	private int setHealth;							//health helper that stores value of original health and hp is filled full (this) on respawn

	public bool canTakeImpactDmg;                   //can take damage frm impacts at all, fall dmg
	public bool onlyRigidbodyImpact;                //when impact dmg is on, if this true as well take dmg from rb that is moving
	public string[] impactFilterTag;                //filter tags that this gameobject doesnt take dmg from

	private int hitForce;

	public bool hasOnDeathObjects;					//if we have on-death objects that we want to instantiate this must be checked true on editor
	public Vector3 instantiateOnDeathPos;			//on-death objects's location, default is center of this object
	public GameObject[] instantiateOnDeath;         //objects to spawn on death of this object
	public bool isDead;
	public Vector3 respawnPos;



	void Awake()
	{
		if (currentHealth <= 0)
			Debug.LogWarning(transform.name + " has health set to 0 at start!");


		if (instantiateOnDeathPos == new Vector3(0, 0, 0) )
			Debug.LogWarning(transform.name + "'s on-death objects positions are at center of this.");


		if (instantiateOnDeath != null)
			Debug.LogWarning(transform.name + " has on-death objects but hasOnDeathObjects is " + hasOnDeathObjects + "! Objects are not spawned on death.");


		setHealth = currentHealth;
		respawnPos = transform.position;
	}

	//damage and death
	void Update()
	{
		if (currentHealth <= 0)
		{
			isDead = true;
		}
		else
		{
			isDead = false;
		}

		if (isDead)
		{
			Death();
		}
	}

	//respawn object or destroy + create on-death objects
	void Death()
	{

		if (canRespawn)
		{
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb != null)	rb.velocity *= 0;

			transform.position = respawnPos;
			isDead = false;
			currentHealth = setHealth;
		}
		else
		{
			Destroy(gameObject);
		}


		if (hasOnDeathObjects)
			for (int i = 0; i < instantiateOnDeath.Length; i++)
			{
				Instantiate(instantiateOnDeath[i], instantiateOnDeathPos, Quaternion.Euler(Vector3.zero));
			}

	}

	//calculate impact damage on collision
	void OnCollisionEnter(Collision coll)
	{

		//stop method if we cannot take impact dmg
		if (!canTakeImpactDmg)
			return;

		//check filter list, if coll object has specific tag stop method because this object doesnt want to take dmg from these objects
		for (int i = 0; i < impactFilterTag.Length; i++)
		{
			if (coll.transform.tag == impactFilterTag[i])
				return;
		}

		//stopping method because coll object doesnt have rigidbody and we can ONLY take dmg from rigidbodies
		if (onlyRigidbodyImpact && !coll.rigidbody)
			return;

		//calculate damage
		if (coll.rigidbody)
		{
			hitForce = (int)(coll.rigidbody.velocity.magnitude / 4 * coll.rigidbody.mass);
		}
		else
		{
			hitForce = (int)coll.relativeVelocity.magnitude / 6;
		}
		Debug.Log("relVmag " + (int)coll.relativeVelocity.magnitude);
		Debug.Log(transform.name + " takes: " + hitForce + " dmg in collision with " + coll.transform.name);
		currentHealth -= hitForce;

	}

}