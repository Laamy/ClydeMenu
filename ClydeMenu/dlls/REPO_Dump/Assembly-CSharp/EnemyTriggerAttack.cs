using System;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class EnemyTriggerAttack : MonoBehaviour
{
	// Token: 0x060006A6 RID: 1702 RVA: 0x00040490 File Offset: 0x0003E690
	private void OnTriggerStay(Collider other)
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.TriggerCheckTimer > 0f)
		{
			return;
		}
		this.TriggerCheckTimerSet = true;
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.LookUnder)
		{
			PlayerTrigger component = other.GetComponent<PlayerTrigger>();
			if (component)
			{
				bool flag = false;
				if (this.Enemy.CurrentState == EnemyState.LookUnder && this.Enemy.StateLookUnder.WaitDone)
				{
					flag = true;
				}
				bool chaseCanReach = this.Enemy.StateChase.ChaseCanReach;
				PlayerAvatar playerAvatar = component.PlayerAvatar;
				if (playerAvatar.isDisabled || (!this.Enemy.Vision.VisionTriggered[playerAvatar.photonView.ViewID] && !flag))
				{
					return;
				}
				bool flag2 = true;
				bool flag3 = false;
				if (!chaseCanReach || flag)
				{
					flag2 = false;
					flag3 = true;
				}
				Vector3 position = playerAvatar.PlayerVisionTarget.VisionTransform.transform.position;
				RaycastHit[] array = Physics.RaycastAll(this.VisionTransform.position, position - this.VisionTransform.position, (position - this.VisionTransform.position).magnitude, this.VisionMask);
				bool flag4 = false;
				foreach (RaycastHit raycastHit in array)
				{
					if (!raycastHit.transform.CompareTag("Enemy") && !raycastHit.transform.GetComponent<PlayerTumble>())
					{
						flag4 = true;
					}
				}
				if (flag4)
				{
					if (!flag3)
					{
						flag2 = false;
					}
				}
				else if (flag3)
				{
					flag2 = true;
				}
				if (flag2)
				{
					this.Attack = true;
				}
			}
		}
		if (this.Enemy.CurrentState != EnemyState.ChaseBegin)
		{
			bool flag5 = false;
			int num = 0;
			Vector3 vector = Vector3.zero;
			PhysGrabObject componentInParent = other.GetComponentInParent<PhysGrabObject>();
			StaticGrabObject componentInParent2 = other.GetComponentInParent<StaticGrabObject>();
			if (componentInParent)
			{
				flag5 = true;
				num = componentInParent.playerGrabbing.Count;
				vector = componentInParent.midPoint;
				if (componentInParent.GetComponent<EnemyRigidbody>())
				{
					flag5 = false;
				}
			}
			else if (componentInParent2)
			{
				flag5 = true;
				num = componentInParent2.playerGrabbing.Count;
				vector = componentInParent2.transform.position;
			}
			if (flag5 && num > 0 && Vector3.Distance(base.transform.position, vector) < this.Enemy.Vision.VisionDistance)
			{
				Vector3 direction = vector - this.VisionTransform.position;
				if (Vector3.Dot(this.VisionTransform.forward, direction.normalized) > 0.8f)
				{
					RaycastHit raycastHit2;
					bool flag6 = Physics.Raycast(this.Enemy.Vision.VisionTransform.position, direction, out raycastHit2, direction.magnitude, this.VisionMask);
					bool flag7 = true;
					if (flag6)
					{
						if (componentInParent)
						{
							if (raycastHit2.collider.GetComponentInParent<PhysGrabObject>() != componentInParent)
							{
								flag7 = false;
							}
						}
						else if (componentInParent2 && raycastHit2.collider.GetComponentInParent<StaticGrabObject>() != componentInParent2)
						{
							flag7 = false;
						}
					}
					if (flag7 && this.Enemy.HasStateInvestigate)
					{
						this.Enemy.StateInvestigate.Set(vector, false);
					}
				}
			}
		}
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000407CE File Offset: 0x0003E9CE
	private void Update()
	{
		if (this.TriggerCheckTimerSet)
		{
			this.TriggerCheckTimer = 0.2f;
			this.TriggerCheckTimerSet = false;
			return;
		}
		if (this.TriggerCheckTimer > 0f)
		{
			this.TriggerCheckTimer -= Time.deltaTime;
		}
	}

	// Token: 0x04000B22 RID: 2850
	public Enemy Enemy;

	// Token: 0x04000B23 RID: 2851
	public LayerMask VisionMask;

	// Token: 0x04000B24 RID: 2852
	public Transform VisionTransform;

	// Token: 0x04000B25 RID: 2853
	private bool TriggerCheckTimerSet;

	// Token: 0x04000B26 RID: 2854
	private float TriggerCheckTimer;

	// Token: 0x04000B27 RID: 2855
	internal bool Attack;
}
