using System;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class EnemyHeadUp : MonoBehaviour
{
	// Token: 0x06000319 RID: 793 RVA: 0x0001EB13 File Offset: 0x0001CD13
	private void Start()
	{
		this.startPosition = base.transform.localPosition.y;
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0001EB2C File Offset: 0x0001CD2C
	private void Update()
	{
		if (!this.enemy.NavMeshAgent.IsDisabled() && this.enemy.CurrentState == EnemyState.Chase && this.enemy.StateChase.VisionTimer > 0f && !this.enemy.TargetPlayerAvatar.isDisabled && this.enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position.y > this.startPosition)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(base.transform.position.x, this.enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position.y, base.transform.position.z), 1f * Time.deltaTime);
			return;
		}
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, this.startPosition, 0f), 5f * Time.deltaTime);
		if (this.enemy.CurrentState == EnemyState.Despawn || this.enemy.NavMeshAgent.IsDisabled())
		{
			base.transform.localPosition = new Vector3(0f, this.startPosition, 0f);
		}
	}

	// Token: 0x0400056B RID: 1387
	public Enemy enemy;

	// Token: 0x0400056C RID: 1388
	private float startPosition;
}
