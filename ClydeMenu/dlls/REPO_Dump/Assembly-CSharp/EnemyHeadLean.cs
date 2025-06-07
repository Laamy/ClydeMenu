using System;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class EnemyHeadLean : MonoBehaviour
{
	// Token: 0x06000311 RID: 785 RVA: 0x0001E804 File Offset: 0x0001CA04
	private void Update()
	{
		if (this.Enemy.FreezeTimer > 0f)
		{
			return;
		}
		if (this.Enemy.NavMeshAgent.AgentVelocity.magnitude < 0.1f)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 50f * Time.deltaTime);
			return;
		}
		float x = Mathf.Clamp(this.Enemy.NavMeshAgent.AgentVelocity.magnitude * this.Amount, -this.MaxAmount, this.MaxAmount);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, Quaternion.Euler(x, 0f, 0f), this.Speed * Time.deltaTime);
	}

	// Token: 0x0400055B RID: 1371
	public Enemy Enemy;

	// Token: 0x0400055C RID: 1372
	[Space]
	public float Amount = -500f;

	// Token: 0x0400055D RID: 1373
	public float MaxAmount = 20f;

	// Token: 0x0400055E RID: 1374
	public float Speed = 10f;
}
