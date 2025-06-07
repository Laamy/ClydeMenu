using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000051 RID: 81
public class EnemyGnomeDirector : MonoBehaviour
{
	// Token: 0x060002BA RID: 698 RVA: 0x0001BD10 File Offset: 0x00019F10
	private void Awake()
	{
		if (!EnemyGnomeDirector.instance)
		{
			EnemyGnomeDirector.instance = this;
			if (!Application.isEditor || (SemiFunc.IsMultiplayer() && !GameManager.instance.localTest))
			{
				this.debugDraw = false;
				this.debugOneOnly = false;
				this.debugShortIdle = false;
				this.debugLongIdle = false;
			}
			base.transform.parent = LevelGenerator.Instance.EnemyParent.transform;
			base.StartCoroutine(this.Setup());
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0001BD95 File Offset: 0x00019F95
	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
		{
			EnemyGnome component = enemyParent.Enemy.GetComponent<EnemyGnome>();
			if (component)
			{
				if (this.debugOneOnly && this.gnomes.Count > 0)
				{
					Object.Destroy(component.enemy.EnemyParent.gameObject);
				}
				else
				{
					this.gnomes.Add(component);
					this.destinations.Add(Vector3.zero);
					component.directorIndex = this.gnomes.IndexOf(component);
				}
			}
		}
		this.setup = true;
		yield break;
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0001BDA4 File Offset: 0x00019FA4
	private void Update()
	{
		if (!this.setup)
		{
			return;
		}
		if (this.debugDraw)
		{
			if (this.currentState == EnemyGnomeDirector.State.Idle)
			{
				Debug.DrawRay(base.transform.position, Vector3.up * 2f, Color.green);
				using (List<EnemyGnome>.Enumerator enumerator = this.gnomes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						EnemyGnome enemyGnome = enumerator.Current;
						Debug.DrawRay(this.destinations[this.gnomes.IndexOf(enemyGnome)], Vector3.up * 2f, Color.yellow);
					}
					goto IL_CA;
				}
			}
			if (this.currentState == EnemyGnomeDirector.State.AttackPlayer)
			{
				Debug.DrawRay(this.attackPosition, Vector3.up * 2f, Color.red);
			}
		}
		IL_CA:
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (this.currentState)
			{
			case EnemyGnomeDirector.State.Idle:
				this.StateIdle();
				return;
			case EnemyGnomeDirector.State.Leave:
				this.StateLeave();
				return;
			case EnemyGnomeDirector.State.ChangeDestination:
				this.StateChangeDestination();
				return;
			case EnemyGnomeDirector.State.Investigate:
				this.StateInvestigate();
				return;
			case EnemyGnomeDirector.State.AttackSet:
				this.StateAttackSet();
				return;
			case EnemyGnomeDirector.State.AttackPlayer:
				this.StateAttackPlayer();
				return;
			case EnemyGnomeDirector.State.AttackValuable:
				this.StateAttackValuable();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060002BD RID: 701 RVA: 0x0001BEEC File Offset: 0x0001A0EC
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
			this.UpdateState(EnemyGnomeDirector.State.ChangeDestination);
		}
		this.LeaveCheck();
	}

	// Token: 0x060002BE RID: 702 RVA: 0x0001BF88 File Offset: 0x0001A188
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
				this.UpdateState(EnemyGnomeDirector.State.Idle);
			}
		}
	}

	// Token: 0x060002BF RID: 703 RVA: 0x0001BFE0 File Offset: 0x0001A1E0
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
				this.UpdateState(EnemyGnomeDirector.State.Idle);
			}
		}
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0001C060 File Offset: 0x0001A260
	private void StateInvestigate()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.UpdateState(EnemyGnomeDirector.State.Idle);
		}
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0001C078 File Offset: 0x0001A278
	private void StateAttackSet()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.valuableTarget = null;
			float num = 0f;
			Collider[] array = Physics.OverlapSphere(this.playerTarget.transform.position, 3f, LayerMask.GetMask(new string[]
			{
				"PhysGrabObject"
			}));
			for (int i = 0; i < array.Length; i++)
			{
				ValuableObject componentInParent = array[i].GetComponentInParent<ValuableObject>();
				if (componentInParent && componentInParent.dollarValueCurrent > num)
				{
					num = componentInParent.dollarValueCurrent;
					this.valuableTarget = componentInParent.physGrabObject;
				}
			}
			if (this.valuableTarget)
			{
				this.UpdateState(EnemyGnomeDirector.State.AttackValuable);
				return;
			}
			this.UpdateState(EnemyGnomeDirector.State.AttackPlayer);
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0001C12C File Offset: 0x0001A32C
	private void StateAttackPlayer()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 3f;
		}
		this.PauseGnomeSpawnedTimers();
		if (this.playerTarget && !this.playerTarget.isDisabled)
		{
			if (this.stateTimer > 0.5f)
			{
				this.attackPosition = this.playerTarget.transform.position;
				this.attackVisionPosition = this.playerTarget.PlayerVisionTarget.VisionTransform.position;
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
			this.UpdateState(EnemyGnomeDirector.State.Idle);
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0001C1F4 File Offset: 0x0001A3F4
	private void StateAttackValuable()
	{
		if (this.stateImpulse)
		{
			this.stateImpulse = false;
			this.stateTimer = 10f;
			this.valuableAttackPositionTimer = 0f;
		}
		this.PauseGnomeSpawnedTimers();
		if (this.valuableTarget)
		{
			if (this.valuableAttackPositionTimer <= 0f)
			{
				this.valuableAttackPositionTimer = 0.2f;
				this.attackPosition = this.valuableTarget.centerPoint;
				RaycastHit raycastHit;
				if (Physics.Raycast(this.valuableTarget.centerPoint, Vector3.down, out raycastHit, 2f, LayerMask.GetMask(new string[]
				{
					"Default"
				})))
				{
					this.attackPosition = raycastHit.point;
				}
			}
			else
			{
				this.valuableAttackPositionTimer -= Time.deltaTime;
			}
			this.attackVisionPosition = this.valuableTarget.centerPoint;
		}
		else
		{
			this.stateTimer = 0f;
		}
		bool flag = false;
		using (List<EnemyGnome>.Enumerator enumerator = this.gnomes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.enemy.Rigidbody.transform.position, this.attackPosition) <= 1f)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			this.stateTimer -= Time.deltaTime;
		}
		bool flag2 = true;
		using (List<EnemyGnome>.Enumerator enumerator = this.gnomes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isActiveAndEnabled)
				{
					flag2 = false;
					break;
				}
			}
		}
		if (this.stateTimer <= 0f || flag2)
		{
			this.SetPosition(this.attackPosition);
			this.UpdateState(EnemyGnomeDirector.State.Idle);
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0001C3C4 File Offset: 0x0001A5C4
	private void UpdateState(EnemyGnomeDirector.State _state)
	{
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0001C3E0 File Offset: 0x0001A5E0
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
			float num = 360f / (float)this.gnomes.Count;
			foreach (EnemyGnome enemyGnome in this.gnomes)
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
				this.destinations[this.gnomes.IndexOf(enemyGnome)] = vector;
				base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y + num, 0f);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0001C650 File Offset: 0x0001A850
	public void Investigate(Vector3 _position)
	{
		if (this.currentState == EnemyGnomeDirector.State.Investigate || this.currentState == EnemyGnomeDirector.State.AttackSet || this.currentState == EnemyGnomeDirector.State.AttackPlayer || this.currentState == EnemyGnomeDirector.State.AttackValuable)
		{
			return;
		}
		this.SetPosition(_position);
		this.UpdateState(EnemyGnomeDirector.State.Investigate);
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0001C688 File Offset: 0x0001A888
	public void SetTarget(PlayerAvatar _player)
	{
		if (this.currentState != EnemyGnomeDirector.State.AttackSet && this.currentState != EnemyGnomeDirector.State.AttackPlayer && this.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			this.playerTarget = _player;
			this.UpdateState(EnemyGnomeDirector.State.AttackSet);
			return;
		}
		if (this.currentState == EnemyGnomeDirector.State.AttackPlayer && this.playerTarget == _player)
		{
			this.stateTimer = 2f;
		}
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x0001C6E1 File Offset: 0x0001A8E1
	public void SeeTarget()
	{
		if (this.currentState == EnemyGnomeDirector.State.AttackPlayer)
		{
			this.stateTimer = 1f;
		}
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0001C6F8 File Offset: 0x0001A8F8
	public bool CanAttack(EnemyGnome _gnome)
	{
		if (_gnome.attackCooldown > 0f || _gnome.enemy.Jump.jumping)
		{
			return false;
		}
		if (this.currentState == EnemyGnomeDirector.State.AttackPlayer)
		{
			if (Vector3.Distance(_gnome.enemy.Rigidbody.transform.position, this.attackPosition) <= 0.7f)
			{
				return true;
			}
		}
		else if (this.valuableTarget)
		{
			_gnome.overlapCheckCooldown = 1f;
			if (_gnome.overlapCheckTimer <= 0f)
			{
				_gnome.overlapCheckTimer = 0.5f;
				_gnome.overlapCheckPrevious = false;
				Collider[] array = Physics.OverlapSphere(_gnome.enemy.Rigidbody.transform.position, 0.7f, LayerMask.GetMask(new string[]
				{
					"PhysGrabObject"
				}));
				for (int i = 0; i < array.Length; i++)
				{
					ValuableObject componentInParent = array[i].GetComponentInParent<ValuableObject>();
					if (componentInParent && componentInParent.physGrabObject == this.valuableTarget)
					{
						_gnome.overlapCheckPrevious = true;
					}
				}
			}
			return _gnome.overlapCheckPrevious;
		}
		return false;
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0001C80C File Offset: 0x0001AA0C
	private void PauseGnomeSpawnedTimers()
	{
		foreach (EnemyGnome enemyGnome in this.gnomes)
		{
			enemyGnome.enemy.EnemyParent.SpawnedTimerPause(0.1f);
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0001C86C File Offset: 0x0001AA6C
	private void LeaveCheck()
	{
		bool flag = false;
		using (List<EnemyGnome>.Enumerator enumerator = this.gnomes.GetEnumerator())
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
			this.UpdateState(EnemyGnomeDirector.State.Leave);
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0001C8D4 File Offset: 0x0001AAD4
	public void OnSpawn()
	{
		foreach (EnemyGnome enemyGnome in this.gnomes)
		{
			enemyGnome.enemy.EnemyParent.DespawnedTimerSet(enemyGnome.enemy.EnemyParent.DespawnedTimer - 30f, false);
		}
	}

	// Token: 0x040004CE RID: 1230
	public static EnemyGnomeDirector instance;

	// Token: 0x040004CF RID: 1231
	public bool debugDraw;

	// Token: 0x040004D0 RID: 1232
	public bool debugOneOnly;

	// Token: 0x040004D1 RID: 1233
	public bool debugShortIdle;

	// Token: 0x040004D2 RID: 1234
	public bool debugLongIdle;

	// Token: 0x040004D3 RID: 1235
	[Space]
	public List<EnemyGnome> gnomes = new List<EnemyGnome>();

	// Token: 0x040004D4 RID: 1236
	internal List<Vector3> destinations = new List<Vector3>();

	// Token: 0x040004D5 RID: 1237
	[Space]
	public EnemyGnomeDirector.State currentState = EnemyGnomeDirector.State.ChangeDestination;

	// Token: 0x040004D6 RID: 1238
	private bool stateImpulse = true;

	// Token: 0x040004D7 RID: 1239
	private float stateTimer;

	// Token: 0x040004D8 RID: 1240
	internal bool setup;

	// Token: 0x040004D9 RID: 1241
	private PlayerAvatar playerTarget;

	// Token: 0x040004DA RID: 1242
	private PhysGrabObject valuableTarget;

	// Token: 0x040004DB RID: 1243
	internal Vector3 attackPosition;

	// Token: 0x040004DC RID: 1244
	internal Vector3 attackVisionPosition;

	// Token: 0x040004DD RID: 1245
	private float valuableAttackPositionTimer;

	// Token: 0x02000311 RID: 785
	public enum State
	{
		// Token: 0x04002894 RID: 10388
		Idle,
		// Token: 0x04002895 RID: 10389
		Leave,
		// Token: 0x04002896 RID: 10390
		ChangeDestination,
		// Token: 0x04002897 RID: 10391
		Investigate,
		// Token: 0x04002898 RID: 10392
		AttackSet,
		// Token: 0x04002899 RID: 10393
		AttackPlayer,
		// Token: 0x0400289A RID: 10394
		AttackValuable
	}
}
