using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class ItemMeleeInflatableHammer : MonoBehaviour
{
	// Token: 0x06000C05 RID: 3077 RVA: 0x00069C1F File Offset: 0x00067E1F
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.explosionPosition = base.transform.Find("Explosion Position");
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00069C4F File Offset: 0x00067E4F
	public void OnHit()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && Random.Range(0, 19) == 0)
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("ExplosionRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
			this.ExplosionRPC();
		}
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00069C88 File Offset: 0x00067E88
	[PunRPC]
	public void ExplosionRPC()
	{
		ParticlePrefabExplosion particlePrefabExplosion = this.particleScriptExplosion.Spawn(this.explosionPosition.position, 0.5f, 0, 250, 1f, false, false, 1f);
		particlePrefabExplosion.SkipHurtColliderSetup = true;
		particlePrefabExplosion.HurtCollider.playerDamage = 0;
		particlePrefabExplosion.HurtCollider.enemyDamage = 250;
		particlePrefabExplosion.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
		particlePrefabExplosion.HurtCollider.physHingeDestroy = true;
		particlePrefabExplosion.HurtCollider.playerTumbleForce = 30f;
		particlePrefabExplosion.HurtCollider.playerTumbleTorque = 50f;
	}

	// Token: 0x0400136B RID: 4971
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x0400136C RID: 4972
	private Transform explosionPosition;

	// Token: 0x0400136D RID: 4973
	private PhotonView photonView;
}
