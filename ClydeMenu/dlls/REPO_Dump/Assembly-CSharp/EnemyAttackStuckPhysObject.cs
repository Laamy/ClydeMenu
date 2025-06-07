using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009C RID: 156
public class EnemyAttackStuckPhysObject : MonoBehaviour
{
	// Token: 0x06000633 RID: 1587 RVA: 0x0003CE54 File Offset: 0x0003B054
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x0003CE64 File Offset: 0x0003B064
	private void Update()
	{
		if (this.AttackedTimer > 0f)
		{
			this.AttackedTimer -= Time.deltaTime;
		}
		if (this.CheckTimer > 0f)
		{
			this.CheckTimer -= Time.deltaTime;
			if (this.CheckTimer <= 0f)
			{
				this.CheckTimer = 0f;
				return;
			}
		}
		else if (this.Active)
		{
			this.Reset();
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x0003CED6 File Offset: 0x0003B0D6
	public bool Check()
	{
		this.CheckTimer = 0.1f;
		if (this.Active)
		{
			return false;
		}
		if (this.Enemy.StuckCount >= this.StuckCount)
		{
			this.Get();
			return true;
		}
		return false;
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x0003CF0C File Offset: 0x0003B10C
	public void Get()
	{
		if (!this.Active)
		{
			Collider[] array = Physics.OverlapSphere(this.Enemy.Vision.VisionTransform.position, this.Range, LayerMask.GetMask(new string[]
			{
				"PhysGrabObject"
			}));
			float num = 1000f;
			PhysGrabObject physGrabObject = null;
			foreach (Collider collider in array)
			{
				if (!collider.GetComponentInParent<EnemyRigidbody>())
				{
					PhysGrabObject componentInParent = collider.GetComponentInParent<PhysGrabObject>();
					float num2 = Vector3.Distance(this.Enemy.Vision.VisionTransform.position, componentInParent.centerPoint);
					if (num2 < num)
					{
						num = num2;
						physGrabObject = componentInParent;
					}
				}
			}
			if (physGrabObject)
			{
				this.Active = true;
				this.TargetObject = physGrabObject;
				this.OnActiveImpulse.Invoke();
				this.Enemy.StuckCount = 0;
			}
		}
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x0003CFE5 File Offset: 0x0003B1E5
	public void Reset()
	{
		this.Enemy.StuckCount = 0;
		this.TargetObject = null;
		this.Active = false;
	}

	// Token: 0x04000A3E RID: 2622
	private Enemy Enemy;

	// Token: 0x04000A3F RID: 2623
	public float Range = 1f;

	// Token: 0x04000A40 RID: 2624
	public int StuckCount = 3;

	// Token: 0x04000A41 RID: 2625
	[Space]
	public UnityEvent OnActiveImpulse;

	// Token: 0x04000A42 RID: 2626
	internal bool Active;

	// Token: 0x04000A43 RID: 2627
	internal PhysGrabObject TargetObject;

	// Token: 0x04000A44 RID: 2628
	internal float AttackedTimer;

	// Token: 0x04000A45 RID: 2629
	private float CheckTimer;
}
