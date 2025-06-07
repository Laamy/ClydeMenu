using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class EnemyThinMan : MonoBehaviour
{
	// Token: 0x06000516 RID: 1302 RVA: 0x000321A8 File Offset: 0x000303A8
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x000321C4 File Offset: 0x000303C4
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			switch (this.currentState)
			{
			case EnemyThinMan.State.Stand:
				this.StateStand();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.OnScreen:
				this.StateOnScreen();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.Notice:
				this.StateNotice();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.Attack:
				this.StateAttack();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.TentacleExtend:
				this.StateTentacleExtend();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.Damage:
				this.StateDamage();
				this.PlayerLookAt();
				break;
			case EnemyThinMan.State.Despawn:
				this.StateDespawn();
				break;
			case EnemyThinMan.State.Stunned:
				this.StateStunned();
				break;
			}
			if (this.enemy.IsStunned())
			{
				this.enemy.EnemyParent.SpawnedTimerSet(0f);
				this.UpdateState(EnemyThinMan.State.Stunned);
			}
		}
		this.TeleportLogic();
		this.SetFollowTargetToPosition();
		this.TentacleLogic();
		this.LocalEffect();
		this.HurtColliderLogic();
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x000322C8 File Offset: 0x000304C8
	private void StateStand()
	{
		if (this.stateImpulse)
		{
			this.SetTarget(null);
			this.stateTimer = 5f;
			this.stateImpulse = false;
		}
		if (!this.playerTarget)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				if (!playerAvatar.isDisabled && this.enemy.OnScreen.GetOnScreen(playerAvatar))
				{
					this.SetTarget(playerAvatar);
					this.UpdateState(EnemyThinMan.State.OnScreen);
					return;
				}
			}
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		if (this.teleportRoamTimer > 0f)
		{
			this.teleportRoamTimer -= Time.deltaTime;
		}
		else if (this.Teleport(false, false))
		{
			this.SetRoamTimer();
		}
		if (SemiFunc.EnemyForceLeave(this.enemy))
		{
			this.Teleport(false, true);
		}
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x000323C0 File Offset: 0x000305C0
	private void StateOnScreen()
	{
		if (this.stateImpulse)
		{
			this.tpTimer = Random.Range(0f, 5f);
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		bool flag = false;
		if (this.enemy.OnScreen.GetOnScreen(this.playerTarget))
		{
			this.stateTimer = 0.2f;
			flag = true;
		}
		if (this.tpTimer > 0f)
		{
			this.tpTimer -= Time.deltaTime;
		}
		if (flag)
		{
			if (this.tentacleLerp >= 1f)
			{
				this.UpdateState(EnemyThinMan.State.Notice);
				return;
			}
			if (this.tentacleLerp > 0.05f && this.tentacleLerp < 0.15f && this.tpTimer <= 0f)
			{
				if (Random.Range(0f, 1f) < 0.5f)
				{
					if (this.Teleport(false, false))
					{
						this.tpTimer = 5f;
					}
				}
				else
				{
					this.tpTimer = 5f;
				}
			}
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyThinMan.State.Stand);
		}
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x000324E4 File Offset: 0x000306E4
	private void StateNotice()
	{
		if (!GameManager.Multiplayer())
		{
			this.NoticeRPC(default(PhotonMessageInfo));
		}
		else
		{
			this.photonView.RPC("NoticeRPC", RpcTarget.All, Array.Empty<object>());
		}
		this.UpdateState(EnemyThinMan.State.Attack);
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x00032528 File Offset: 0x00030728
	private void StateAttack()
	{
		if (this.stateImpulse)
		{
			if (!this.playerTarget)
			{
				this.UpdateState(EnemyThinMan.State.Despawn);
			}
			this.stateTimer = 1f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyThinMan.State.TentacleExtend);
		}
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x0003258C File Offset: 0x0003078C
	private void StateTentacleExtend()
	{
		if (this.stateImpulse)
		{
			if (!this.playerTarget)
			{
				this.UpdateState(EnemyThinMan.State.Despawn);
			}
			this.stateTimer = 0.1f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyThinMan.State.Damage);
		}
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x000325F0 File Offset: 0x000307F0
	private void StateDamage()
	{
		if (this.stateImpulse)
		{
			if (!this.playerTarget)
			{
				this.UpdateState(EnemyThinMan.State.Despawn);
			}
			this.stateImpulse = false;
		}
		this.playerTarget.playerHealth.HurtOther(30, this.playerTarget.transform.position, false, SemiFunc.EnemyGetIndex(this.enemy));
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ActivateHurtColliderRPC", RpcTarget.All, new object[]
			{
				this.playerTarget.transform.position
			});
		}
		else
		{
			this.ActivateHurtColliderRPC(this.playerTarget.transform.position, default(PhotonMessageInfo));
		}
		this.UpdateState(EnemyThinMan.State.Despawn);
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x000326AC File Offset: 0x000308AC
	private void StateDespawn()
	{
		if (this.stateImpulse)
		{
			this.stateTimer = 0.4f;
			this.stateImpulse = false;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.enemy.EnemyParent.SpawnedTimerSet(0f);
		}
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x00032707 File Offset: 0x00030907
	private void StateStunned()
	{
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0003270C File Offset: 0x0003090C
	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.otherEnemyFetch)
			{
				this.otherEnemyFetch = false;
				foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
				{
					EnemyThinMan componentInChildren = enemyParent.GetComponentInChildren<EnemyThinMan>(true);
					if (componentInChildren && componentInChildren != this)
					{
						this.otherEnemies.Add(componentInChildren);
					}
				}
			}
			if (SemiFunc.EnemySpawnIdlePause())
			{
				this.lastTeleportPosition = base.transform.position;
				SemiFunc.EnemySpawn(this.enemy);
				this.teleportPosition = base.transform.position;
				this.teleporting = true;
				this.UpdateState(EnemyThinMan.State.Stand);
				return;
			}
			if (this.Teleport(true, false))
			{
				this.SetRoamTimer();
				this.UpdateState(EnemyThinMan.State.Stand);
				return;
			}
			this.enemy.EnemyParent.Despawn();
			this.enemy.EnemyParent.DespawnedTimerSet(3f, false);
		}
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x0003281C File Offset: 0x00030A1C
	public void OnHurt()
	{
		this.anim.hurtSound.Play(this.anim.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x00032854 File Offset: 0x00030A54
	private void UpdateState(EnemyThinMan.State _nextState)
	{
		this.stateTimer = 0f;
		this.stateImpulse = true;
		this.currentState = _nextState;
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateStateRPC", RpcTarget.Others, new object[]
			{
				_nextState
			});
		}
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x000328A4 File Offset: 0x00030AA4
	private bool Teleport(bool _spawn, bool _leave = false)
	{
		List<LevelPoint> list = new List<LevelPoint>();
		if (_leave)
		{
			list.Add(SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f));
		}
		else
		{
			list = SemiFunc.LevelPointGetWithinDistance(base.transform.position, 3f, 30f);
			if (list == null)
			{
				list = SemiFunc.LevelPointGetWithinDistance(base.transform.position, 3f, 50f);
				if (list == null)
				{
					list = SemiFunc.LevelPointGetWithinDistance(base.transform.position, 0f, 999f);
				}
			}
		}
		if (list == null)
		{
			return false;
		}
		bool flag = Random.Range(0, 100) < 3;
		if (this.playerTarget)
		{
			flag = (Random.Range(0, 100) < 30);
		}
		if (flag && !_leave)
		{
			list = SemiFunc.LevelPointsGetAllCloseToPlayers();
		}
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		LevelPoint levelPoint = list[Random.Range(0, list.Count)];
		if (levelPoint == null)
		{
			return false;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(levelPoint.transform.position + Vector3.up * 0.1f, Vector3.up, out raycastHit, 3.5f, LayerMask.GetMask(new string[]
		{
			"Default"
		})))
		{
			return false;
		}
		foreach (EnemyThinMan enemyThinMan in this.otherEnemies)
		{
			if (enemyThinMan && enemyThinMan.isActiveAndEnabled && Vector3.Distance(enemyThinMan.rb.position, levelPoint.transform.position) <= 2f)
			{
				return false;
			}
		}
		if (Vector3.Distance(levelPoint.transform.position, this.lastTeleportPosition) < 1f)
		{
			return false;
		}
		if (SemiFunc.EnemyPhysObjectBoundingBoxCheck(base.transform.position, levelPoint.transform.position, this.enemy.Rigidbody.rb))
		{
			return false;
		}
		this.lastTeleportPosition = base.transform.position;
		this.teleportPosition = levelPoint.transform.position;
		this.teleporting = true;
		if (!_spawn)
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("TeleportEffectRPC", RpcTarget.All, new object[]
				{
					this.lastTeleportPosition,
					true
				});
			}
			else
			{
				this.TeleportEffectRPC(this.lastTeleportPosition, true, default(PhotonMessageInfo));
			}
		}
		else
		{
			this.enemy.EnemyTeleported(this.teleportPosition);
		}
		return true;
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x00032B38 File Offset: 0x00030D38
	private void TentacleLogic()
	{
		if (this.currentState == EnemyThinMan.State.OnScreen)
		{
			this.tentacleLerp += this.anim.tentacleSpeed * Time.deltaTime;
		}
		else if (this.currentState == EnemyThinMan.State.Attack || this.currentState == EnemyThinMan.State.TentacleExtend)
		{
			if (this.currentState == EnemyThinMan.State.TentacleExtend)
			{
				this.tentacleLerp -= 10f * Time.deltaTime;
			}
		}
		else if (this.currentState == EnemyThinMan.State.Stunned)
		{
			this.tentacleLerp -= 0.4f * Time.deltaTime;
		}
		else
		{
			this.tentacleLerp -= this.anim.tentacleSpeed * 0.5f * Time.deltaTime;
		}
		this.tentacleLerp = Mathf.Clamp(this.tentacleLerp, 0f, 1f);
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x00032C08 File Offset: 0x00030E08
	private void TeleportLogic()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.teleportTimer <= 0f)
			{
				if (this.teleporting)
				{
					this.enemy.EnemyTeleported(this.teleportPosition);
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("TeleportEffectRPC", RpcTarget.All, new object[]
						{
							this.teleportPosition,
							false
						});
					}
					else
					{
						this.TeleportEffectRPC(this.teleportPosition, false, default(PhotonMessageInfo));
					}
					this.teleporting = false;
					return;
				}
			}
			else
			{
				this.teleportTimer -= Time.deltaTime;
			}
		}
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x00032CAC File Offset: 0x00030EAC
	private void PlayerLookAt()
	{
		if (this.playerTarget)
		{
			Quaternion b = Quaternion.Euler(0f, Quaternion.LookRotation(this.playerTarget.PlayerVisionTarget.VisionTransform.position - this.enemy.Rigidbody.transform.position).eulerAngles.y, 0f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 50f * Time.deltaTime);
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00032D3E File Offset: 0x00030F3E
	private void SetFollowTargetToPosition()
	{
		this.enemy.transform.position = this.teleportPosition;
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00032D56 File Offset: 0x00030F56
	public void SmokeEffect(Vector3 pos)
	{
		this.anim.particleSmokeCalmFill.Play();
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x00032D68 File Offset: 0x00030F68
	private void Rattle()
	{
		this.anim.notice.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.anim.rattleImpulse = true;
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x00032DA6 File Offset: 0x00030FA6
	private void LocalEffect()
	{
		if (this.currentState == EnemyThinMan.State.OnScreen && this.playerTarget && this.playerTarget.isLocal)
		{
			SemiFunc.DoNotLookEffect(base.gameObject, true, true, true, true, true, false);
		}
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x00032DDC File Offset: 0x00030FDC
	private void SetRoamTimer()
	{
		this.teleportRoamTimer = Random.Range(8f, 22f);
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x00032DF3 File Offset: 0x00030FF3
	private void HurtColliderLogic()
	{
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				this.hurtCollider.SetActive(false);
			}
		}
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x00032E2D File Offset: 0x0003102D
	[PunRPC]
	private void UpdateStateRPC(EnemyThinMan.State _nextState, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _nextState;
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x00032E3F File Offset: 0x0003103F
	[PunRPC]
	private void NoticeRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.anim.NoticeSet();
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x00032E58 File Offset: 0x00031058
	[PunRPC]
	public void TeleportEffectRPC(Vector3 position, bool intro, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.SmokeEffect(position);
		if (intro)
		{
			this.anim.teleportIn.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		else
		{
			this.anim.teleportOut.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.anim.rattleImpulse = true;
		this.teleportTimer = 0.1f;
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x00032EF4 File Offset: 0x000310F4
	private void SetTarget(PlayerAvatar _player)
	{
		if (_player == this.playerTarget)
		{
			return;
		}
		this.playerTarget = _player;
		bool flag = true;
		int num = -1;
		if (!this.playerTarget)
		{
			flag = false;
		}
		if (flag)
		{
			this.Rattle();
			num = this.playerTarget.photonView.ViewID;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("SetTargetRPC", RpcTarget.Others, new object[]
			{
				num,
				flag
			});
		}
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x00032F74 File Offset: 0x00031174
	[PunRPC]
	public void SetTargetRPC(int playerID, bool hasTarget, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!hasTarget)
		{
			this.playerTarget = null;
			return;
		}
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			if (playerAvatar.photonView.ViewID == playerID)
			{
				this.playerTarget = playerAvatar;
				break;
			}
		}
		this.Rattle();
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x00032FF0 File Offset: 0x000311F0
	[PunRPC]
	public void ActivateHurtColliderRPC(Vector3 _position, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.hurtCollider.transform.position = _position;
		this.hurtCollider.transform.rotation = Quaternion.LookRotation(this.enemy.Vision.VisionTransform.position - _position);
		this.hurtCollider.SetActive(true);
		this.hurtColliderTimer = 0.25f;
	}

	// Token: 0x04000833 RID: 2099
	private PhotonView photonView;

	// Token: 0x04000834 RID: 2100
	private Enemy enemy;

	// Token: 0x04000835 RID: 2101
	public EnemyThinManAnim anim;

	// Token: 0x04000836 RID: 2102
	public GameObject tentacleR1;

	// Token: 0x04000837 RID: 2103
	public GameObject tentacleR2;

	// Token: 0x04000838 RID: 2104
	public GameObject tentacleR3;

	// Token: 0x04000839 RID: 2105
	public GameObject tentacleL1;

	// Token: 0x0400083A RID: 2106
	public GameObject tentacleL2;

	// Token: 0x0400083B RID: 2107
	public GameObject tentacleL3;

	// Token: 0x0400083C RID: 2108
	public GameObject extendedTentacles;

	// Token: 0x0400083D RID: 2109
	public GameObject head;

	// Token: 0x0400083E RID: 2110
	public GameObject hurtCollider;

	// Token: 0x0400083F RID: 2111
	private float hurtColliderTimer;

	// Token: 0x04000840 RID: 2112
	public float tentacleLerp;

	// Token: 0x04000841 RID: 2113
	public EnemyThinMan.State currentState;

	// Token: 0x04000842 RID: 2114
	private float stateTimer;

	// Token: 0x04000843 RID: 2115
	private bool stateImpulse;

	// Token: 0x04000844 RID: 2116
	private float tpTimer;

	// Token: 0x04000845 RID: 2117
	public Rigidbody rb;

	// Token: 0x04000846 RID: 2118
	internal PlayerAvatar playerTarget;

	// Token: 0x04000847 RID: 2119
	private bool otherEnemyFetch = true;

	// Token: 0x04000848 RID: 2120
	public List<EnemyThinMan> otherEnemies;

	// Token: 0x04000849 RID: 2121
	private Vector3 teleportPosition;

	// Token: 0x0400084A RID: 2122
	private Vector3 lastTeleportPosition;

	// Token: 0x0400084B RID: 2123
	private float teleportTimer;

	// Token: 0x0400084C RID: 2124
	private bool teleporting;

	// Token: 0x0400084D RID: 2125
	private float teleportRoamTimer;

	// Token: 0x02000321 RID: 801
	public enum State
	{
		// Token: 0x0400293B RID: 10555
		Stand,
		// Token: 0x0400293C RID: 10556
		OnScreen,
		// Token: 0x0400293D RID: 10557
		Notice,
		// Token: 0x0400293E RID: 10558
		Attack,
		// Token: 0x0400293F RID: 10559
		TentacleExtend,
		// Token: 0x04002940 RID: 10560
		Damage,
		// Token: 0x04002941 RID: 10561
		Despawn,
		// Token: 0x04002942 RID: 10562
		Stunned
	}
}
