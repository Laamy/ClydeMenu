using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002A0 RID: 672
public class FlamethrowerValuable : MonoBehaviour
{
	// Token: 0x060014F7 RID: 5367 RVA: 0x000B9993 File Offset: 0x000B7B93
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.triggerMeshInitialEulerAngles = this.triggerMesh.localEulerAngles;
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x000B99C0 File Offset: 0x000B7BC0
	private void Update()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.fuelCountdownActive)
			{
				this.fuelTimer -= Time.deltaTime;
				if (this.fuelTimer <= 0f)
				{
					this.fuelTimer = 0f;
					this.ReleaseTrigger();
					this.fuelCountdownActive = false;
					this.SetState(FlamethrowerValuable.States.Empty);
				}
			}
			if (this.triggerStuck)
			{
				this.triggerStuckTimer -= Time.deltaTime;
				if (this.triggerStuckTimer <= 0f)
				{
					this.triggerStuckTimer = 0f;
					this.ReleaseTrigger();
					this.triggerStuck = false;
				}
			}
		}
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x000B9A5C File Offset: 0x000B7C5C
	private void GrabTriggerLogic()
	{
		this.SetTriggerMeshPosition(true);
		if (this.currentState == FlamethrowerValuable.States.Empty)
		{
			this.soundFlameEmpty.Play(this.semiFlames.transform.position, 1f, 1f, 1f, 1f);
			this.flameEndSquirt.Play();
			this.flameEndSparks.Play();
			return;
		}
		EnemyDirector.instance.SetInvestigate(base.transform.position, 5f, false);
		this.semiFlames.FlamesActive(this.semiFlames.transform.position, this.semiFlames.transform.rotation);
		this.fuelCountdownActive = true;
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x000B9B0D File Offset: 0x000B7D0D
	public void GrabTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.GrabTriggerLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("GrabTriggerRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x000B9B3F File Offset: 0x000B7D3F
	[PunRPC]
	private void GrabTriggerRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.GrabTriggerLogic();
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x000B9B50 File Offset: 0x000B7D50
	private void ReleaseTriggerLogic()
	{
		this.SetTriggerMeshPosition(false);
		if (this.currentState == FlamethrowerValuable.States.Empty)
		{
			this.flameEndSparks.Stop();
			return;
		}
		this.semiFlames.FlamesInactive();
		this.fuelCountdownActive = false;
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x000B9B80 File Offset: 0x000B7D80
	public void ReleaseTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ReleaseTriggerLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("ReleaseTriggerRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x000B9BB2 File Offset: 0x000B7DB2
	[PunRPC]
	private void ReleaseTriggerRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.ReleaseTriggerLogic();
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x000B9BC3 File Offset: 0x000B7DC3
	[PunRPC]
	public void SetStateRPC(FlamethrowerValuable.States state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = state;
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x000B9BD8 File Offset: 0x000B7DD8
	private void SetState(FlamethrowerValuable.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(state, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x000B9C28 File Offset: 0x000B7E28
	public void SetTriggerMeshPosition(bool pulled)
	{
		if (!this.triggerStuck)
		{
			if (pulled)
			{
				Vector3 localEulerAngles = new Vector3(this.triggerMeshInitialEulerAngles.x, this.triggerMeshInitialEulerAngles.y, this.triggerMeshInitialEulerAngles.z - 40f);
				this.triggerMesh.localEulerAngles = localEulerAngles;
				return;
			}
			this.triggerMesh.localEulerAngles = this.triggerMeshInitialEulerAngles;
		}
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x000B9C8C File Offset: 0x000B7E8C
	public void TriggerStuck()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.triggerStuckTimer = 0.2f;
			this.triggerStuck = true;
			this.GrabTrigger();
		}
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x000B9CB0 File Offset: 0x000B7EB0
	public void Explode()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 0.2f, 10, 20, 1f, false, false, 1f);
	}

	// Token: 0x04002448 RID: 9288
	public SemiZuperFlames semiFlames;

	// Token: 0x04002449 RID: 9289
	public Transform triggerMesh;

	// Token: 0x0400244A RID: 9290
	public Transform Center;

	// Token: 0x0400244B RID: 9291
	private Vector3 triggerMeshInitialEulerAngles;

	// Token: 0x0400244C RID: 9292
	private bool triggerStuck;

	// Token: 0x0400244D RID: 9293
	private float triggerStuckTimer = 0.2f;

	// Token: 0x0400244E RID: 9294
	public Sound soundFlameEmpty;

	// Token: 0x0400244F RID: 9295
	public float fuelTimer;

	// Token: 0x04002450 RID: 9296
	private bool fuelCountdownActive;

	// Token: 0x04002451 RID: 9297
	private PhotonView photonView;

	// Token: 0x04002452 RID: 9298
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002453 RID: 9299
	public ParticleSystem flameEndSquirt;

	// Token: 0x04002454 RID: 9300
	public ParticleSystem flameEndSparks;

	// Token: 0x04002455 RID: 9301
	internal FlamethrowerValuable.States currentState;

	// Token: 0x02000410 RID: 1040
	public enum States
	{
		// Token: 0x04002DA7 RID: 11687
		Full,
		// Token: 0x04002DA8 RID: 11688
		Empty
	}
}
