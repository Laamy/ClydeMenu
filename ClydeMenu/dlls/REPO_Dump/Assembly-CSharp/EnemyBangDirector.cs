using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200003E RID: 62
public class EnemyBangDirector : MonoBehaviour, IPunObservable
{
	// Token: 0x0600013D RID: 317 RVA: 0x0000C3D8 File Offset: 0x0000A5D8
	private void Awake()
	{
		if (!EnemyBangDirector.instance)
		{
			EnemyBangDirector.instance = this;
			if (!Application.isEditor || (SemiFunc.IsMultiplayer() && !GameManager.instance.localTest))
			{
				this.debugDraw = false;
				this.debugOneOnly = false;
				this.debugShortIdle = false;
				this.debugLongIdle = false;
				this.debugNoFuseProgress = false;
			}
			base.transform.parent = LevelGenerator.Instance.EnemyParent.transform;
			base.StartCoroutine(this.Setup());
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x0600013E RID: 318 RVA: 0x0000C464 File Offset: 0x0000A664
	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		int num = -1;
		foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
		{
			EnemyBang component = enemyParent.Enemy.GetComponent<EnemyBang>();
			if (component)
			{
				if (this.debugOneOnly && this.units.Count > 0)
				{
					Object.Destroy(component.enemy.EnemyParent.gameObject);
				}
				else
				{
					this.units.Add(component);
					this.destinations.Add(Vector3.zero);
					component.directorIndex = this.units.IndexOf(component);
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						if (num == -1)
						{
							num = Random.Range(0, component.headObjects.Length);
						}
						if (SemiFunc.IsMultiplayer())
						{
							component.photonView.RPC("SetHeadRPC", RpcTarget.All, new object[]
							{
								num
							});
						}
						else
						{
							component.SetHeadRPC(num, default(PhotonMessageInfo));
						}
						num++;
						if (num >= component.headObjects.Length)
						{
							num = 0;
						}
						float num2 = Random.Range(0.8f, 1.25f);
						if (SemiFunc.IsMultiplayer())
						{
							component.photonView.RPC("SetVoicePitchRPC", RpcTarget.All, new object[]
							{
								num2
							});
						}
						else
						{
							component.SetVoicePitchRPC(num2, default(PhotonMessageInfo));
						}
					}
				}
			}
		}
		foreach (EnemyBang enemyBang in this.units)
		{
			foreach (EnemyBangFuse enemyBangFuse in enemyBang.enemy.EnemyParent.GetComponentsInChildren<EnemyBangFuse>(true))
			{
				enemyBangFuse.controller = enemyBang;
				enemyBangFuse.particleParent.parent = enemyBang.particleParent;
				enemyBangFuse.setup = true;
			}
		}
		this.setup = true;
		yield break;
	}

	// Token: 0x0600013F RID: 319 RVA: 0x0000C474 File Offset: 0x0000A674
	private void Update()
	{
		if (!this.setup)
		{
			return;
		}
		if (this.debugDraw)
		{
			Debug.DrawRay(base.transform.position, Vector3.up * 2f, Color.green);
			foreach (EnemyBang enemyBang in this.units)
			{
				Debug.DrawRay(this.destinations[this.units.IndexOf(enemyBang)], Vector3.up * 2f, Color.yellow);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (this.currentState)
			{
			case EnemyBangDirector.State.Idle:
				this.StateIdle();
				return;
			case EnemyBangDirector.State.Leave:
				this.StateLeave();
				break;
			case EnemyBangDirector.State.ChangeDestination:
				this.StateChangeDestination();
				return;
			case EnemyBangDirector.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyBangDirector.State.AttackSet:
				this.StateAttackSet();
				return;
			case EnemyBangDirector.State.AttackPlayer:
				this.StateAttackPlayer();
				return;
			case EnemyBangDirector.State.AttackCart:
				this.StateAttackCart();
				return;
			default:
				return;
			}
		}
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000C58C File Offset: 0x0000A78C
	private void StateIdle()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = Random.Range(20f, 30f);
			if (this.debugShortIdle)
			{
				this.stateTimer *= 0.5f;
			}
			if (this.debugLongIdle)
			{
				this.stateTimer *= 2f;
			}
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			return;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.UpdateState(EnemyBangDirector.State.ChangeDestination);
		}
		this.LeaveCheck();
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0000C628 File Offset: 0x0000A828
	private void StateChangeDestination()
	{
		if (this.stateImpulse)
		{
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGet(base.transform.position, 10f, 25f);
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGet(base.transform.position, 0f, 999f);
			}
			if (levelPoint)
			{
				flag = this.SetPosition(levelPoint.transform.position);
			}
			if (flag)
			{
				this.stateImpulse = false;
				this.UpdateState(EnemyBangDirector.State.Idle);
			}
		}
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000C6A8 File Offset: 0x0000A8A8
	private void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.UpdateState(EnemyBangDirector.State.Idle);
		}
	}

	// Token: 0x06000143 RID: 323 RVA: 0x0000C6C0 File Offset: 0x0000A8C0
	private void StateAttackSet()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.UpdateState(EnemyBangDirector.State.AttackPlayer);
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000C6D8 File Offset: 0x0000A8D8
	private void StateAttackPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 3f;
		}
		this.PauseSpawnedTimers();
		if (this.playerTarget && !this.playerTarget.isDisabled)
		{
			this.playerTargetCrawling = this.playerTarget.isCrawling;
			if (this.stateTimer > 0.5f)
			{
				this.attackPosition = this.playerTarget.transform.position;
				this.attackVisionPosition = this.playerTarget.PlayerVisionTarget.VisionTransform.position;
				if (!this.playerTargetCrawling)
				{
					this.attackVisionPosition += Vector3.up * 0.25f;
				}
			}
		}
		else
		{
			this.stateTimer = 0f;
		}
		this.stateTimer -= Time.deltaTime;
		if (this.stateTimer <= 0f)
		{
			this.SetPosition(this.attackPosition);
			this.UpdateState(EnemyBangDirector.State.Idle);
		}
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0000C7D9 File Offset: 0x0000A9D9
	private void StateAttackCart()
	{
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000C7DC File Offset: 0x0000A9DC
	private void StateLeave()
	{
		if (this.stateImpulse)
		{
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			if (levelPoint)
			{
				flag = this.SetPosition(levelPoint.transform.position);
			}
			if (flag)
			{
				this.stateImpulse = false;
				this.UpdateState(EnemyBangDirector.State.Idle);
			}
		}
	}

	// Token: 0x06000147 RID: 327 RVA: 0x0000C834 File Offset: 0x0000AA34
	private void UpdateState(EnemyBangDirector.State _state)
	{
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
	}

	// Token: 0x06000148 RID: 328 RVA: 0x0000C850 File Offset: 0x0000AA50
	private bool SetPosition(Vector3 _initialPosition)
	{
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(_initialPosition, out navMeshHit, 5f, -1) && Physics.Raycast(navMeshHit.position, Vector3.down, 5f, LayerMask.GetMask(new string[]
		{
			"Default"
		})) && !SemiFunc.EnemyPhysObjectSphereCheck(navMeshHit.position, 1f))
		{
			base.transform.position = navMeshHit.position;
			base.transform.rotation = Quaternion.identity;
			float num = 360f / (float)this.units.Count;
			foreach (EnemyBang enemyBang in this.units)
			{
				float num2 = 0f;
				Vector3 vector = base.transform.position;
				Vector3 vector2 = base.transform.position;
				while (num2 < 2f)
				{
					vector = vector2;
					vector2 = navMeshHit.position + base.transform.forward * num2;
					NavMeshHit navMeshHit2;
					if (!NavMesh.SamplePosition(vector2, out navMeshHit2, 5f, -1) || !Physics.Raycast(vector2, Vector3.down, 5f, LayerMask.GetMask(new string[]
					{
						"Default"
					})))
					{
						break;
					}
					Vector3 normalized = (vector2 + Vector3.up * 0.5f - (navMeshHit.position + Vector3.up * 0.5f)).normalized;
					if (Physics.Raycast(vector2 + Vector3.up * 0.5f, normalized, normalized.magnitude, LayerMask.GetMask(new string[]
					{
						"Default",
						"PhysGrabObjectHinge"
					})) || (num2 > 0.5f && Random.Range(0, 100) < 15))
					{
						break;
					}
					num2 += 0.1f;
				}
				this.destinations[this.units.IndexOf(enemyBang)] = vector;
				base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y + num, 0f);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x0000CAC0 File Offset: 0x0000ACC0
	private void LeaveCheck()
	{
		bool flag = false;
		using (List<EnemyBang>.Enumerator enumerator = this.units.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (SemiFunc.EnemyForceLeave(enumerator.Current.enemy))
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			this.UpdateState(EnemyBangDirector.State.Leave);
		}
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000CB28 File Offset: 0x0000AD28
	public void OnSpawn()
	{
		foreach (EnemyBang enemyBang in this.units)
		{
			enemyBang.enemy.EnemyParent.DespawnedTimerSet(enemyBang.enemy.EnemyParent.DespawnedTimer - 30f, false);
		}
	}

	// Token: 0x0600014B RID: 331 RVA: 0x0000CB9C File Offset: 0x0000AD9C
	public void Investigate(Vector3 _position)
	{
		if (this.currentState == EnemyBangDirector.State.Investigate)
		{
			return;
		}
		this.SetPosition(_position);
		this.UpdateState(EnemyBangDirector.State.Investigate);
	}

	// Token: 0x0600014C RID: 332 RVA: 0x0000CBB8 File Offset: 0x0000ADB8
	public void SetTarget(PlayerAvatar _player)
	{
		if (this.currentState != EnemyBangDirector.State.AttackSet && this.currentState != EnemyBangDirector.State.AttackPlayer && this.currentState != EnemyBangDirector.State.AttackCart)
		{
			this.playerTarget = _player;
			this.UpdateState(EnemyBangDirector.State.AttackSet);
			return;
		}
		if (this.currentState == EnemyBangDirector.State.AttackPlayer && this.playerTarget == _player)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x0600014D RID: 333 RVA: 0x0000CC11 File Offset: 0x0000AE11
	public void SeeTarget()
	{
		if (this.currentState == EnemyBangDirector.State.AttackPlayer)
		{
			this.stateTimer = 1f;
		}
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000CC28 File Offset: 0x0000AE28
	public void TriggerNearby(Vector3 _position)
	{
		foreach (EnemyBang enemyBang in this.units)
		{
			if (Vector3.Distance(enemyBang.transform.position, _position) < 2f)
			{
				enemyBang.OnVision();
			}
		}
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000CC94 File Offset: 0x0000AE94
	private void PauseSpawnedTimers()
	{
		foreach (EnemyBang enemyBang in this.units)
		{
			enemyBang.enemy.EnemyParent.SpawnedTimerPause(0.1f);
		}
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000CCF4 File Offset: 0x0000AEF4
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.attackVisionPosition);
			return;
		}
		this.attackVisionPosition = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x040002B3 RID: 691
	public static EnemyBangDirector instance;

	// Token: 0x040002B4 RID: 692
	public bool debugDraw;

	// Token: 0x040002B5 RID: 693
	public bool debugOneOnly;

	// Token: 0x040002B6 RID: 694
	public bool debugShortIdle;

	// Token: 0x040002B7 RID: 695
	public bool debugLongIdle;

	// Token: 0x040002B8 RID: 696
	public bool debugNoFuseProgress;

	// Token: 0x040002B9 RID: 697
	[Space]
	public List<EnemyBang> units = new List<EnemyBang>();

	// Token: 0x040002BA RID: 698
	internal List<Vector3> destinations = new List<Vector3>();

	// Token: 0x040002BB RID: 699
	[Space]
	public EnemyBangDirector.State currentState = EnemyBangDirector.State.ChangeDestination;

	// Token: 0x040002BC RID: 700
	private bool stateImpulse = true;

	// Token: 0x040002BD RID: 701
	private float stateTimer;

	// Token: 0x040002BE RID: 702
	internal bool setup;

	// Token: 0x040002BF RID: 703
	internal PlayerAvatar playerTarget;

	// Token: 0x040002C0 RID: 704
	internal bool playerTargetCrawling;

	// Token: 0x040002C1 RID: 705
	internal Vector3 attackPosition;

	// Token: 0x040002C2 RID: 706
	internal Vector3 attackVisionPosition;

	// Token: 0x02000305 RID: 773
	public enum State
	{
		// Token: 0x04002826 RID: 10278
		Idle,
		// Token: 0x04002827 RID: 10279
		Leave,
		// Token: 0x04002828 RID: 10280
		ChangeDestination,
		// Token: 0x04002829 RID: 10281
		Investigate,
		// Token: 0x0400282A RID: 10282
		AttackSet,
		// Token: 0x0400282B RID: 10283
		AttackPlayer,
		// Token: 0x0400282C RID: 10284
		AttackCart
	}
}
