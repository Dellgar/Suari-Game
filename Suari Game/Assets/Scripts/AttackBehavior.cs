using UnityEngine;

public class AttackBehavior : MonoBehaviour {

	private Health health;
	private Rigidbody targetRb;

	public void NormalAttack(GameObject _attackTarget, int _dmg, float _pushHeight, float _pushForce)
	{
		health = _attackTarget.GetComponent<Health>();
		targetRb = _attackTarget.GetComponent<Rigidbody>();

	
		Vector3 pushDir = (_attackTarget.transform.position - transform.position);

		pushDir.y = 0f;
		pushDir.y = _pushHeight * 0.1f;

		if (targetRb && !targetRb.isKinematic)
		{
			targetRb.velocity = new Vector3(0, 0, 0);
			targetRb.AddForce(pushDir.normalized * _pushForce, ForceMode.VelocityChange);
			targetRb.AddForce(Vector3.up * _pushHeight, ForceMode.VelocityChange);
		}

		//remove the hp from attack target's hp
		if (health) health.currentHealth -= _dmg;
	}

	public void FlatAttack(GameObject _attackTarget, int _dmg, float _flattenYAmount, float _speedMultiplierWhileFlat)
	{
		health = _attackTarget.GetComponent<Health>();
		targetRb = _attackTarget.GetComponent<Rigidbody>();

		if (targetRb && !targetRb.isKinematic)
		{
			targetRb.velocity = new Vector3(0, 0, 0);
		}

		//remove the hp from attack target's hp
		if (health) health.currentHealth -= _dmg;

		_attackTarget.transform.localScale = 
			new Vector3(_attackTarget.transform.localScale.x, (_attackTarget.transform.localScale.y - _flattenYAmount), _attackTarget.transform.localScale.z);

		targetRb.velocity *= _speedMultiplierWhileFlat;

	}
}
