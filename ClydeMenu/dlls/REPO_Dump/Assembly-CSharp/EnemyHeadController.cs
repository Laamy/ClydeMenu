using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class EnemyHeadController : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060002F9 RID: 761 RVA: 0x0001DAE1 File Offset: 0x0001BCE1
	private void Awake()
	{
		this.Enemy = base.GetComponentInParent<Enemy>();
		this.Hairs = Enumerable.ToList<EnemyHeadHair>(this.HairParent.GetComponentsInChildren<EnemyHeadHair>());
	}

	// Token: 0x060002FA RID: 762 RVA: 0x0001DB08 File Offset: 0x0001BD08
	private void Update()
	{
		if (this.Enemy.CurrentState == EnemyState.Chase || this.Enemy.CurrentState == EnemyState.ChaseSlow || this.Enemy.CurrentState == EnemyState.LookUnder)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(base.transform.position, playerAvatar.transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(playerAvatar, this.Enemy.Vision.VisionTransform.position, 0.1f, base.gameObject);
				}
			}
		}
		if (this.Enemy.TeleportedTimer > 0f)
		{
			foreach (EnemyHeadHair enemyHeadHair in this.Hairs)
			{
				enemyHeadHair.transform.position = enemyHeadHair.Target.position;
			}
		}
		if (!this.Enemy.MasterClient)
		{
			float num = 1f / (float)PhotonNetwork.SerializationRate;
			float num2 = this.RotationDistance / num;
			this.LookAtTransform.rotation = Quaternion.RotateTowards(this.LookAtTransform.rotation, this.RotationTarget, num2 * Time.deltaTime);
			return;
		}
		if (this.Enemy.AttackStuckPhysObject.TargetObject)
		{
			if (this.Enemy.AttackStuckPhysObject != null)
			{
				Vector3 vector = this.Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck.transform.position;
				vector += this.Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck.transform.TransformDirection(this.Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck.CheckPosition);
				this.LookAtTransform.LookAt(vector);
			}
		}
		else if (this.Enemy.CurrentState == EnemyState.ChaseBegin)
		{
			this.LookAtTransform.LookAt(this.Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
		}
		else if (this.Enemy.CurrentState == EnemyState.Chase && this.Enemy.StateChase.VisionTimer > 0f)
		{
			if (this.Enemy.NavMeshAgent.Agent.velocity.normalized.magnitude > 0.1f)
			{
				Quaternion b = Quaternion.LookRotation(this.Enemy.NavMeshAgent.Agent.velocity.normalized);
				this.LookAtTransform.LookAt(this.Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
				this.LookAtTransform.rotation = Quaternion.Lerp(this.LookAtTransform.rotation, b, 0.25f);
			}
			else
			{
				this.LookAtTransform.LookAt(this.Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
			}
		}
		else if (this.Enemy.CurrentState == EnemyState.LookUnder)
		{
			this.LookAtTransform.LookAt(this.Enemy.StateChase.SawPlayerHidePosition);
			this.LookAtTransform.localEulerAngles = new Vector3(0f, this.LookAtTransform.localEulerAngles.y, 0f);
		}
		else if (this.Enemy.NavMeshAgent.Agent.velocity.magnitude > 0.1f)
		{
			this.LookAtTransform.rotation = Quaternion.LookRotation(this.Enemy.NavMeshAgent.Agent.velocity.normalized);
			this.LookAtTransform.localEulerAngles = new Vector3(0f, this.LookAtTransform.localEulerAngles.y, 0f);
		}
		if (this.Enemy.CurrentState == EnemyState.Despawn)
		{
			this.Enemy.Rigidbody.DisableFollowPosition(0.1f, 1f);
			this.Enemy.Rigidbody.DisableFollowRotation(0.1f, 1f);
		}
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0001DF68 File Offset: 0x0001C168
	public void VisionTriggered()
	{
		if (this.Enemy.DisableChaseTimer > 0f)
		{
			return;
		}
		if (this.Enemy.CurrentState != EnemyState.Chase && this.Enemy.CurrentState != EnemyState.LookUnder)
		{
			if (this.Enemy.CurrentState == EnemyState.ChaseSlow)
			{
				this.Enemy.CurrentState = EnemyState.Chase;
			}
			else if (this.Enemy.Vision.onVisionTriggeredCulled && !this.Enemy.Vision.onVisionTriggeredNear)
			{
				if (this.Enemy.CurrentState != EnemyState.Sneak)
				{
					if (Random.Range(0f, 100f) <= 30f)
					{
						this.Enemy.CurrentState = EnemyState.ChaseBegin;
					}
					else
					{
						this.Enemy.CurrentState = EnemyState.Sneak;
					}
				}
			}
			else if (this.Enemy.Vision.onVisionTriggeredDistance >= 7f)
			{
				this.Enemy.CurrentState = EnemyState.Chase;
				this.Enemy.StateChase.ChaseCanReach = true;
			}
			else
			{
				this.Enemy.CurrentState = EnemyState.ChaseBegin;
			}
			this.Enemy.TargetPlayerViewID = this.Enemy.Vision.onVisionTriggeredPlayer.photonView.ViewID;
			this.Enemy.TargetPlayerAvatar = this.Enemy.Vision.onVisionTriggeredPlayer;
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0001E0B0 File Offset: 0x0001C2B0
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			SemiFunc.EnemySpawn(this.Enemy);
		}
		this.Visual.Spawn();
		if (this.AnimationSystem.isActiveAndEnabled)
		{
			this.AnimationSystem.OnSpawn();
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0001E0E8 File Offset: 0x0001C2E8
	public void OnHurt()
	{
		this.AnimationSystem.Hurt.Play(this.Enemy.Rigidbody.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0001E124 File Offset: 0x0001C324
	public void OnDeath()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.Enemy.CurrentState = EnemyState.Despawn;
		}
		foreach (GameObject gameObject in this.DeathParticles)
		{
			gameObject.SetActive(true);
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.DeathSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.Enemy.EnemyParent.Despawn();
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0001E220 File Offset: 0x0001C420
	public void OnStunnedEnd()
	{
		this.Enemy.CurrentState = EnemyState.Roaming;
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0001E230 File Offset: 0x0001C430
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.LookAtTransform.rotation);
			return;
		}
		this.RotationTarget = (Quaternion)stream.ReceiveNext();
		this.RotationDistance = Quaternion.Angle(base.transform.rotation, this.RotationTarget);
	}

	// Token: 0x0400051E RID: 1310
	private Quaternion RotationTarget;

	// Token: 0x0400051F RID: 1311
	private float RotationDistance;

	// Token: 0x04000520 RID: 1312
	private Enemy Enemy;

	// Token: 0x04000521 RID: 1313
	public EnemyHeadVisual Visual;

	// Token: 0x04000522 RID: 1314
	public EnemyHeadAnimationSystem AnimationSystem;

	// Token: 0x04000523 RID: 1315
	[Space]
	public Transform LookAtTransform;

	// Token: 0x04000524 RID: 1316
	public Transform AnimationTransform;

	// Token: 0x04000525 RID: 1317
	public Transform HairParent;

	// Token: 0x04000526 RID: 1318
	private List<EnemyHeadHair> Hairs;

	// Token: 0x04000527 RID: 1319
	[Space]
	public List<GameObject> DeathParticles;

	// Token: 0x04000528 RID: 1320
	public Sound DeathSound;
}
