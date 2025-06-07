using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class ItemDuckBucket : MonoBehaviour
{
	// Token: 0x06000B04 RID: 2820 RVA: 0x000623E3 File Offset: 0x000605E3
	private void Start()
	{
		base.StartCoroutine(this.DuckFinder());
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x000623F2 File Offset: 0x000605F2
	private IEnumerator DuckFinder()
	{
		for (;;)
		{
			this.active = false;
			this.enemyDuck = null;
			this.playerAvatar = null;
			foreach (Collider collider in Physics.OverlapSphere(this.sphereCollider.transform.position, this.sphereCollider.radius, LayerMask.GetMask(new string[]
			{
				"PhysGrabObject"
			})))
			{
				if (collider.gameObject.name == "Duck Collider")
				{
					EnemyRigidbody componentInParent = collider.GetComponentInParent<EnemyRigidbody>();
					if (componentInParent)
					{
						EnemyDuck component = componentInParent.enemy.GetComponent<EnemyDuck>();
						if (component)
						{
							this.enemyDuck = component;
							this.active = true;
							break;
						}
					}
				}
			}
			foreach (Collider collider2 in Physics.OverlapSphere(this.sphereCollider.transform.position, this.sphereCollider.radius, SemiFunc.LayerMaskGetPlayersAndPhysObjects()))
			{
				if (collider2.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
				{
					PlayerController componentInParent2 = collider2.transform.GetComponentInParent<PlayerController>();
					if (componentInParent2)
					{
						this.playerAvatar = componentInParent2.playerAvatarScript;
					}
					else
					{
						this.playerAvatar = collider2.transform.GetComponentInParent<PlayerAvatar>();
					}
					this.active = true;
					break;
				}
				PlayerTumble componentInParent3 = collider2.transform.GetComponentInParent<PlayerTumble>();
				if (componentInParent3)
				{
					this.playerAvatar = componentInParent3.playerAvatar;
					this.active = true;
					break;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00062404 File Offset: 0x00060604
	private void Update()
	{
		if (this.active)
		{
			if (this.enemyDuck)
			{
				this.enemyDuck.DuckBucketActive();
			}
			else if (this.playerAvatar && this.playerAvatar.isLocal)
			{
				PlayerController.instance.OverrideJumpCooldown(0.5f);
			}
		}
		if (this.active != this.activePrevious)
		{
			this.activePrevious = this.active;
			if (this.active)
			{
				this.lowPassParent.SetActive(true);
				return;
			}
			this.lowPassParent.SetActive(false);
		}
	}

	// Token: 0x040011C8 RID: 4552
	public SphereCollider sphereCollider;

	// Token: 0x040011C9 RID: 4553
	private bool active;

	// Token: 0x040011CA RID: 4554
	private bool activePrevious;

	// Token: 0x040011CB RID: 4555
	private EnemyDuck enemyDuck;

	// Token: 0x040011CC RID: 4556
	private PlayerAvatar playerAvatar;

	// Token: 0x040011CD RID: 4557
	public GameObject lowPassParent;
}
