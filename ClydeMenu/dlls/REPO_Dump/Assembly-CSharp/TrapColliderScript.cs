using System;
using UnityEngine;

// Token: 0x02000256 RID: 598
public class TrapColliderScript : MonoBehaviour
{
	// Token: 0x06001348 RID: 4936 RVA: 0x000AC670 File Offset: 0x000AA870
	private void OnTriggerEnter(Collider other)
	{
		PlayerTrigger component = other.GetComponent<PlayerTrigger>();
		if (component && !GameDirector.instance.LevelCompleted)
		{
			PlayerAvatar playerAvatar = component.PlayerAvatar;
			if (playerAvatar.isLocal && other.gameObject.CompareTag("Player") && !GameDirector.instance.LevelEnemyChasing && TrapDirector.instance.TrapCooldown <= 0f && !PlayerController.instance.Crouching)
			{
				this.TrapCollision = true;
				this.triggerPlayer = playerAvatar;
			}
		}
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x000AC6F0 File Offset: 0x000AA8F0
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.2f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	// Token: 0x0600134A RID: 4938 RVA: 0x000AC72F File Offset: 0x000AA92F
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.5f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	// Token: 0x040020EB RID: 8427
	[HideInInspector]
	public bool TrapCollision;

	// Token: 0x040020EC RID: 8428
	[HideInInspector]
	public float TrapCollisionForce;

	// Token: 0x040020ED RID: 8429
	public PlayerAvatar triggerPlayer;
}
