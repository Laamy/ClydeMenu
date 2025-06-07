using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class EnemyHeadAnimationSystem : MonoBehaviour
{
	// Token: 0x060002D0 RID: 720 RVA: 0x0001CAB8 File Offset: 0x0001ACB8
	private void Awake()
	{
		this.PhotonView = base.GetComponent<PhotonView>();
		this.Animator.keepAnimatorStateOnDisable = true;
		this.IdleTeethTime = Random.Range(this.IdleTeethTimeMin, this.IdleTeethTimeMax);
		this.AnimatorIdle = Animator.StringToHash("Idle");
		this.AnimatorIdleTeeth = Animator.StringToHash("IdleTeeth");
		this.AnimatorIdleBite = Animator.StringToHash("IdleBite");
		this.AnimatorChaseBite = Animator.StringToHash("ChaseBite");
		this.AnimatorChaseBegin = Animator.StringToHash("ChaseBegin");
		this.AnimatorChase = Animator.StringToHash("Chase");
		this.AnimatorDespawn = Animator.StringToHash("Despawn");
		this.AnimatorSpawn = Animator.StringToHash("Spawn");
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0001CB74 File Offset: 0x0001AD74
	public void SetChaseBeginToChase()
	{
		this.ChaseBeginToChase.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0001CC08 File Offset: 0x0001AE08
	public void SetChaseToIdle()
	{
		this.ChaseToIdle.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0001CC9C File Offset: 0x0001AE9C
	public void SetChaseBegin()
	{
		this.ChaseBegin.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.ChaseBeginGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0001CD5B File Offset: 0x0001AF5B
	public void PlayTeethChatter()
	{
		this.TeethChatter.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0001CD88 File Offset: 0x0001AF88
	public void MaterialImpact()
	{
		Materials.Instance.Impulse(base.transform.position, Vector3.down, Materials.SoundType.Heavy, false, this.MaterialTrigger, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0001CE14 File Offset: 0x0001B014
	public void Slide()
	{
		Materials.Instance.Slide(base.transform.position, this.MaterialTrigger, 1f, false);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0001CE9E File Offset: 0x0001B09E
	public void PlayMoveLong()
	{
		this.MoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x0001CECB File Offset: 0x0001B0CB
	public void PlayMoveShort()
	{
		this.MoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0001CEF8 File Offset: 0x0001B0F8
	public void AttackStuckPhysObject()
	{
		if (!this.IdleBiteTrigger)
		{
			this.IdleBiteTrigger = true;
			this.IdleBite();
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0001CF10 File Offset: 0x0001B110
	private void Update()
	{
		if (this.Enemy.CurrentState == EnemyState.Spawn || this.Enemy.CurrentState == EnemyState.Roaming || this.Enemy.CurrentState == EnemyState.Investigate || this.Enemy.CurrentState == EnemyState.ChaseEnd || this.Enemy.IsStunned())
		{
			if (this.Enemy.MasterClient && this.IdleTeethTime > 0f)
			{
				this.IdleTeethTime -= Time.deltaTime;
				if (this.IdleTeethTime <= 0f)
				{
					this.IdleTeeth();
					this.IdleTeethTime = Random.Range(this.IdleTeethTimeMin, this.IdleTeethTimeMax);
				}
			}
			if (this.IdleTeethTrigger)
			{
				this.Animator.SetTrigger(this.AnimatorIdleTeeth);
				this.IdleTeethTrigger = false;
			}
			this.Animator.SetBool(this.AnimatorIdle, true);
		}
		else
		{
			this.Animator.SetBool(this.AnimatorIdle, false);
			if (this.IdleTeethTime < this.IdleTeethTimeMin)
			{
				this.IdleTeethTime = Random.Range(this.IdleTeethTimeMin, this.IdleTeethTimeMax);
			}
		}
		if (this.Enemy.CurrentState == EnemyState.ChaseBegin)
		{
			this.Animator.SetBool(this.AnimatorChaseBegin, true);
		}
		else
		{
			this.Animator.SetBool(this.AnimatorChaseBegin, false);
		}
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			this.Animator.SetBool(this.AnimatorChase, true);
		}
		else
		{
			this.Animator.SetBool(this.AnimatorChase, false);
		}
		if (this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Chase Bite"))
		{
			this.EnemyTriggerAttack.Attack = false;
		}
		else if (this.EnemyTriggerAttack.Attack)
		{
			this.ChaseBiteTrigger();
		}
		if (this.Enemy.CurrentState == EnemyState.LookUnder && this.Enemy.StateLookUnder.WaitDone)
		{
			this.EnemyRigidbody.OverrideFollowPosition(0.1f, 50f, -1f);
			this.EnemyRigidbody.OverrideFollowRotation(0.1f, 2f);
			this.LookUnderOffset.Active(0.1f);
			this.EnemyHeadFloat.Disable(0.5f);
		}
		this.ChaseLoop.PlayLoop(this.ChaseLoopActive, 5f, 5f, 1f);
		this.ChaseLoop2.PlayLoop(this.ChaseLoopActive, 5f, 5f, 1f);
		if ((this.Enemy.CurrentState == EnemyState.Despawn || this.Enemy.Health.dead) && !this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Despawn") && !this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Chase Bite"))
		{
			this.Animator.SetTrigger(this.AnimatorDespawn);
		}
		if (this.Enemy.CurrentState == EnemyState.Spawn && !this.Animator.GetCurrentAnimatorStateInfo(0).IsName("Spawn"))
		{
			this.Animator.SetTrigger(this.AnimatorSpawn);
		}
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0001D22C File Offset: 0x0001B42C
	private void IdleTeeth()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.IdleTeethRPC(default(PhotonMessageInfo));
			return;
		}
		this.PhotonView.RPC("IdleTeethRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060002DC RID: 732 RVA: 0x0001D26B File Offset: 0x0001B46B
	[PunRPC]
	private void IdleTeethRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.IdleTeethTrigger = true;
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0001D280 File Offset: 0x0001B480
	private void ChaseBiteTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ChaseBiteTriggerRPC(default(PhotonMessageInfo));
			return;
		}
		this.PhotonView.RPC("ChaseBiteTriggerRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0001D2BF File Offset: 0x0001B4BF
	[PunRPC]
	private void ChaseBiteTriggerRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.Animator.SetTrigger(this.AnimatorChaseBite);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x0001D2DB File Offset: 0x0001B4DB
	public void PlayChaseBiteStart()
	{
		this.BiteStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0001D308 File Offset: 0x0001B508
	public void PlayChaseBiteImpact()
	{
		this.Enemy.Rigidbody.GrabRelease();
		this.BiteEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.EnemyRigidbody.DisableFollowPosition(0.25f, 1f);
		this.EnemyRigidbody.DisableFollowRotation(0.25f, 2f);
		this.EnemyRigidbody.rb.AddForce(this.EnemyRigidbody.transform.forward * 10f, ForceMode.Impulse);
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0001D401 File Offset: 0x0001B601
	public void PlayBiteStart()
	{
		this.BiteStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0001D430 File Offset: 0x0001B630
	private void IdleBite()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.IdleBiteRPC(default(PhotonMessageInfo));
			return;
		}
		this.PhotonView.RPC("IdleBiteRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0001D46F File Offset: 0x0001B66F
	[PunRPC]
	private void IdleBiteRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.Animator.SetTrigger(this.AnimatorIdleBite);
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0001D48C File Offset: 0x0001B68C
	public void IdleBiteImpact()
	{
		this.BiteEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.EnemyRigidbody.DisableFollowPosition(1f, 1f);
		this.EnemyRigidbody.DisableFollowRotation(1f, 1f);
		this.EnemyRigidbody.rb.AddForce(this.EnemyRigidbody.transform.forward * 10f, ForceMode.Impulse);
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0001D51C File Offset: 0x0001B71C
	public void IdleBiteDone()
	{
		this.Enemy.AttackStuckPhysObject.Reset();
		this.IdleBiteTrigger = false;
		this.EnemyRigidbody.DisableFollowPosition(0.2f, 1f);
		this.EnemyRigidbody.DisableFollowRotation(0.5f, 1f);
		this.EnemyRigidbody.rb.AddForce(-this.EnemyRigidbody.transform.forward * 2f, ForceMode.Impulse);
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0001D59A File Offset: 0x0001B79A
	public void OnSpawn()
	{
		this.Animator.Play("Spawn", 0, 0f);
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x0001D5B4 File Offset: 0x0001B7B4
	public void PlayDespawn()
	{
		this.Despawn.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x0001D648 File Offset: 0x0001B848
	public void PlaySpawn()
	{
		this.Spawn.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x0001D6DC File Offset: 0x0001B8DC
	public void TeleportParticlesStart()
	{
		this.TeleportParticlesTop.Play();
		this.TeleportParticlesBot.Play();
	}

	// Token: 0x060002EA RID: 746 RVA: 0x0001D6F4 File Offset: 0x0001B8F4
	public void TeleportParticlesStop()
	{
		this.TeleportParticlesTop.Stop();
		this.TeleportParticlesBot.Stop();
	}

	// Token: 0x060002EB RID: 747 RVA: 0x0001D70C File Offset: 0x0001B90C
	private void DespawnSet()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.DespawnSetRPC(default(PhotonMessageInfo));
			return;
		}
		this.PhotonView.RPC("DespawnSetRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060002EC RID: 748 RVA: 0x0001D74B File Offset: 0x0001B94B
	[PunRPC]
	private void DespawnSetRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.Enemy.StateDespawn.Despawn();
		}
	}

	// Token: 0x040004E3 RID: 1251
	private PhotonView PhotonView;

	// Token: 0x040004E4 RID: 1252
	public Enemy Enemy;

	// Token: 0x040004E5 RID: 1253
	public Animator Animator;

	// Token: 0x040004E6 RID: 1254
	public Materials.MaterialTrigger MaterialTrigger;

	// Token: 0x040004E7 RID: 1255
	public EnemyRigidbody EnemyRigidbody;

	// Token: 0x040004E8 RID: 1256
	public AnimatedOffset LookUnderOffset;

	// Token: 0x040004E9 RID: 1257
	public EnemyHeadFloat EnemyHeadFloat;

	// Token: 0x040004EA RID: 1258
	public ParticleSystem TeleportParticlesTop;

	// Token: 0x040004EB RID: 1259
	public ParticleSystem TeleportParticlesBot;

	// Token: 0x040004EC RID: 1260
	public EnemyTriggerAttack EnemyTriggerAttack;

	// Token: 0x040004ED RID: 1261
	[Space]
	public Sound ChaseBegin;

	// Token: 0x040004EE RID: 1262
	public Sound ChaseBeginGlobal;

	// Token: 0x040004EF RID: 1263
	public Sound ChaseBeginToChase;

	// Token: 0x040004F0 RID: 1264
	public Sound ChaseToIdle;

	// Token: 0x040004F1 RID: 1265
	public bool ChaseLoopActive;

	// Token: 0x040004F2 RID: 1266
	public Sound ChaseLoop;

	// Token: 0x040004F3 RID: 1267
	public Sound ChaseLoop2;

	// Token: 0x040004F4 RID: 1268
	public Sound TeethChatter;

	// Token: 0x040004F5 RID: 1269
	public Sound MoveLong;

	// Token: 0x040004F6 RID: 1270
	public Sound MoveShort;

	// Token: 0x040004F7 RID: 1271
	public Sound BiteStart;

	// Token: 0x040004F8 RID: 1272
	public Sound BiteEnd;

	// Token: 0x040004F9 RID: 1273
	public Sound Spawn;

	// Token: 0x040004FA RID: 1274
	public Sound Despawn;

	// Token: 0x040004FB RID: 1275
	public Sound Hurt;

	// Token: 0x040004FC RID: 1276
	[Space]
	public float IdleTeethTimeMin;

	// Token: 0x040004FD RID: 1277
	public float IdleTeethTimeMax;

	// Token: 0x040004FE RID: 1278
	private float IdleTeethTime;

	// Token: 0x040004FF RID: 1279
	private bool IdleTeethTrigger;

	// Token: 0x04000500 RID: 1280
	private bool IdleBiteTrigger;

	// Token: 0x04000501 RID: 1281
	private int AnimatorIdle;

	// Token: 0x04000502 RID: 1282
	private int AnimatorChaseBegin;

	// Token: 0x04000503 RID: 1283
	private int AnimatorChase;

	// Token: 0x04000504 RID: 1284
	private int AnimatorIdleTeeth;

	// Token: 0x04000505 RID: 1285
	private int AnimatorIdleBite;

	// Token: 0x04000506 RID: 1286
	private int AnimatorChaseBite;

	// Token: 0x04000507 RID: 1287
	private int AnimatorDespawn;

	// Token: 0x04000508 RID: 1288
	private int AnimatorSpawn;

	// Token: 0x04000509 RID: 1289
	private bool Idle;
}
