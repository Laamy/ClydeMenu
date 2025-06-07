using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class EnemyGrounded : MonoBehaviour
{
	// Token: 0x06000639 RID: 1593 RVA: 0x0003D01C File Offset: 0x0003B21C
	private void Awake()
	{
		this.enemy.Grounded = this;
		this.enemy.HasGrounded = true;
		if (!this.boxCollider.isTrigger)
		{
			Debug.LogError("EnemyGrounded: Collider is not a trigger on " + this.enemy.EnemyParent.name);
		}
		if (this.boxCollider.transform.localScale != Vector3.one)
		{
			Debug.LogError("EnemyGrounded: Scale is not 1 on " + this.enemy.EnemyParent.name);
		}
		if (this.boxCollider.transform.localPosition != Vector3.zero)
		{
			Debug.LogError("EnemyGrounded: Position is not 0 on " + this.enemy.EnemyParent.name);
		}
		base.StartCoroutine(this.ColliderCheck());
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x0003D0F0 File Offset: 0x0003B2F0
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.logicActive = false;
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x0003D0FF File Offset: 0x0003B2FF
	private void OnEnable()
	{
		if (!this.logicActive)
		{
			base.StartCoroutine(this.ColliderCheck());
		}
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x0003D116 File Offset: 0x0003B316
	private IEnumerator ColliderCheck()
	{
		this.logicActive = true;
		yield return new WaitForSeconds(0.1f);
		for (;;)
		{
			this.grounded = false;
			Vector3 vector = this.boxCollider.transform.TransformVector(this.boxCollider.size * 0.5f);
			vector.x = Mathf.Abs(vector.x);
			vector.y = Mathf.Abs(vector.y);
			vector.z = Mathf.Abs(vector.z);
			Collider[] array = Physics.OverlapBox(this.boxCollider.bounds.center, vector, this.boxCollider.transform.rotation, LayerMask.GetMask(new string[]
			{
				"Default",
				"PhysGrabObject",
				"PhysGrabObjectHinge",
				"PhysGrabObjectCart"
			}), QueryTriggerInteraction.Ignore);
			if (array.Length != 0)
			{
				foreach (Collider collider in array)
				{
					if (!collider.GetComponentInParent<EnemyRigidbody>())
					{
						if (this.enemy.HasJump && this.enemy.Jump.surfaceJump)
						{
							EnemyJumpSurface component = collider.GetComponent<EnemyJumpSurface>();
							if (component)
							{
								Vector3 rhs = this.enemy.transform.forward;
								if (this.enemy.HasRigidbody)
								{
									rhs = this.enemy.transform.position - this.enemy.Rigidbody.transform.position;
								}
								if (Vector3.Dot(component.transform.TransformDirection(component.jumpDirection), rhs) > 0.5f)
								{
									this.enemy.Jump.SurfaceJumpTrigger(component.transform.TransformDirection(component.jumpDirection));
								}
							}
						}
						if (this.groundedDisableTimer <= 0f)
						{
							this.grounded = true;
						}
					}
				}
			}
			if (this.enemy.HasJump && this.enemy.Jump)
			{
				this.groundedDisableTimer -= 0.05f;
				yield return new WaitForSeconds(0.05f);
			}
			else
			{
				this.groundedDisableTimer -= 0.25f;
				yield return new WaitForSeconds(0.25f);
			}
		}
		yield break;
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x0003D125 File Offset: 0x0003B325
	public void GroundedDisable(float _time)
	{
		this.groundedDisableTimer = _time;
	}

	// Token: 0x04000A46 RID: 2630
	public Enemy enemy;

	// Token: 0x04000A47 RID: 2631
	internal bool grounded;

	// Token: 0x04000A48 RID: 2632
	public BoxCollider boxCollider;

	// Token: 0x04000A49 RID: 2633
	private bool logicActive;

	// Token: 0x04000A4A RID: 2634
	private float groundedDisableTimer;
}
