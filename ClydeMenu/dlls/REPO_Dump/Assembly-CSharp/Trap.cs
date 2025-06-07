using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class Trap : MonoBehaviour
{
	// Token: 0x06001350 RID: 4944 RVA: 0x000AC802 File Offset: 0x000AAA02
	protected virtual void Start()
	{
		this.enemyInvestigateTimer = this.enemyInvestigateTimerMax;
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.isLocal = true;
		}
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x000AC844 File Offset: 0x000AAA44
	protected virtual void Update()
	{
		if (this.isLocal)
		{
			if (this.enemyInvestigate)
			{
				if (!this.enemyInvestigatePrev)
				{
					this.enemyInvestigateTimer = this.enemyInvestigateTimerMax;
				}
				this.enemyInvestigateTimer += Time.deltaTime;
				if (this.enemyInvestigateTimer > this.enemyInvestigateTimerMax)
				{
					EnemyDirector.instance.SetInvestigate(base.transform.position, this.enemyInvestigateRange, false);
					this.enemyInvestigateTimer = 0f;
				}
			}
			this.enemyInvestigatePrev = this.enemyInvestigate;
			this.enemyInvestigate = false;
			if (this.triggerOnTimer)
			{
				if (this.physGrabObject.grabbed)
				{
					if (Application.isEditor && (!GameManager.Multiplayer() || GameManager.instance.localTest) && Input.GetKeyDown(KeyCode.B))
					{
						this.TrapActivateSync();
					}
					if (this.trapActivateTimer > 0f)
					{
						this.trapActivateTimer -= Time.deltaTime;
						if (this.trapActivateTimer <= 0f)
						{
							this.trapActivateTimer = Random.Range(5f, 15f);
							if (SemiFunc.ValuableTrapActivatedDiceRoll((int)this.trapActivateRarityLevel))
							{
								this.TrapActivateSync();
								return;
							}
						}
					}
				}
				else
				{
					this.trapActivateTimer = Random.Range(0f, 15f);
				}
			}
		}
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x000AC980 File Offset: 0x000AAB80
	private void TrapActivateSync()
	{
		if (this.trapTriggered)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.photonView.RPC("TrapActivateSyncRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.TrapActivateSyncRPC(default(PhotonMessageInfo));
		}
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x000AC9CA File Offset: 0x000AABCA
	[PunRPC]
	public void TrapActivateSyncRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (this.physGrabObject.grabbedLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		this.trapStart = true;
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x000AC9F4 File Offset: 0x000AABF4
	public void TrapStart()
	{
		if (this.trapTriggered)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.ValuableTrapActivatedDiceRoll((int)this.trapActivateRarityLevel))
			{
				this.photonView.RPC("TrapStartRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else if (SemiFunc.ValuableTrapActivatedDiceRoll((int)this.trapActivateRarityLevel))
		{
			this.TrapStartRPC(default(PhotonMessageInfo));
		}
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x000ACA58 File Offset: 0x000AAC58
	[PunRPC]
	public void TrapStartRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (this.physGrabObject.grabbedLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		this.trapStart = true;
	}

	// Token: 0x040020F2 RID: 8434
	protected PhotonView photonView;

	// Token: 0x040020F3 RID: 8435
	[HideInInspector]
	public bool enemyInvestigate;

	// Token: 0x040020F4 RID: 8436
	private bool enemyInvestigatePrev;

	// Token: 0x040020F5 RID: 8437
	protected float enemyInvestigateRange = 35f;

	// Token: 0x040020F6 RID: 8438
	private float enemyInvestigateTimer = 1f;

	// Token: 0x040020F7 RID: 8439
	private float enemyInvestigateTimerMax = 1f;

	// Token: 0x040020F8 RID: 8440
	[HideInInspector]
	public bool isLocal;

	// Token: 0x040020F9 RID: 8441
	[HideInInspector]
	public bool trapTriggered;

	// Token: 0x040020FA RID: 8442
	[HideInInspector]
	public bool trapActive;

	// Token: 0x040020FB RID: 8443
	[HideInInspector]
	public bool trapStart;

	// Token: 0x040020FC RID: 8444
	private float trapActivateTimer = 10f;

	// Token: 0x040020FD RID: 8445
	protected PhysGrabObject physGrabObject;

	// Token: 0x040020FE RID: 8446
	public bool triggerOnTimer;

	// Token: 0x040020FF RID: 8447
	public Trap.TrapActivateRarityLevel trapActivateRarityLevel;

	// Token: 0x020003FE RID: 1022
	public enum TrapActivateRarityLevel
	{
		// Token: 0x04002D5F RID: 11615
		no_rarity,
		// Token: 0x04002D60 RID: 11616
		level1,
		// Token: 0x04002D61 RID: 11617
		level2,
		// Token: 0x04002D62 RID: 11618
		level3
	}
}
