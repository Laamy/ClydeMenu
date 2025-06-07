using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class FloaterAttackLogic : MonoBehaviour
{
	// Token: 0x06000269 RID: 617 RVA: 0x000189FC File Offset: 0x00016BFC
	private void StateMachine()
	{
		if (this.controller.currentState == EnemyFloater.State.ChargeAttack || this.controller.currentState == EnemyFloater.State.DelayAttack || this.controller.currentState == EnemyFloater.State.Attack)
		{
			EnemyFloater.State currentState = this.controller.currentState;
			if (currentState != EnemyFloater.State.ChargeAttack)
			{
				if (currentState == EnemyFloater.State.Stun)
				{
					this.StateSet(FloaterAttackLogic.FloaterAttackState.end);
				}
			}
			else if (this.state != FloaterAttackLogic.FloaterAttackState.levitate)
			{
				this.StateSet(FloaterAttackLogic.FloaterAttackState.start);
			}
		}
		else
		{
			this.StateSet(FloaterAttackLogic.FloaterAttackState.end);
		}
		switch (this.state)
		{
		case FloaterAttackLogic.FloaterAttackState.start:
			this.StateStart();
			return;
		case FloaterAttackLogic.FloaterAttackState.levitate:
			this.StateLevitate();
			return;
		case FloaterAttackLogic.FloaterAttackState.stop:
			this.StateStop();
			return;
		case FloaterAttackLogic.FloaterAttackState.smash:
			this.StateSmash();
			return;
		case FloaterAttackLogic.FloaterAttackState.end:
			this.StateEnd();
			return;
		case FloaterAttackLogic.FloaterAttackState.inactive:
			this.StateInactive();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600026A RID: 618 RVA: 0x00018AC4 File Offset: 0x00016CC4
	private void Reset()
	{
		this.checkTimer = 0f;
		this.particleCount = 0;
		this.tumblePhysObjectCheckTimer = 0f;
		foreach (FloaterLine floaterLine in this.floaterLines)
		{
			if (floaterLine)
			{
				floaterLine.outro = true;
			}
		}
		this.capturedPlayerAvatars.Clear();
		this.capturedPhysGrabObjects.Clear();
		this.floaterLines.Clear();
		this.sphereEffects.localScale = Vector3.zero;
		this.attackLight.intensity = 0f;
		this.sphereEffects.gameObject.SetActive(false);
	}

	// Token: 0x0600026B RID: 619 RVA: 0x00018B90 File Offset: 0x00016D90
	private void StateInactive()
	{
		if (this.stateStart)
		{
			this.Reset();
			this.stateStart = false;
		}
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00018BA8 File Offset: 0x00016DA8
	private void StateEnd()
	{
		if (this.stateStart)
		{
			foreach (FloaterLine floaterLine in this.floaterLines)
			{
				if (floaterLine)
				{
					floaterLine.outro = true;
				}
			}
			this.stateStart = false;
		}
		if (this.sphereEffects.gameObject.activeSelf)
		{
			this.sphereEffects.localScale = Vector3.Lerp(this.sphereEffects.localScale, Vector3.zero, Time.deltaTime * 20f);
			this.attackLight.intensity = Mathf.Lerp(this.attackLight.intensity, 0f, Time.deltaTime * 20f);
			if (this.sphereEffects.localScale.x < 0.01f)
			{
				this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
				return;
			}
		}
		else
		{
			this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
		}
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00018CA0 File Offset: 0x00016EA0
	private void StateStart()
	{
		if (this.stateStart)
		{
			this.Reset();
			this.sphereEffects.gameObject.SetActive(true);
			this.stateStart = false;
		}
		this.sphereEffects.localScale = Vector3.Lerp(this.sphereEffects.localScale, Vector3.one * 1.2f, Time.deltaTime * 6f);
		this.attackLight.intensity = 4f * this.sphereEffects.localScale.magnitude;
		if (this.sphereEffects.localScale.x > 1.19f)
		{
			this.attackLight.intensity = 4f;
			this.sphereEffects.localScale = Vector3.one * 1.2f;
			this.StateSet(FloaterAttackLogic.FloaterAttackState.levitate);
		}
	}

	// Token: 0x0600026E RID: 622 RVA: 0x00018D74 File Offset: 0x00016F74
	private void StateLevitate()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.GetAllWithinRange();
		}
		if (this.checkTimer > 0.35f)
		{
			this.GetAllWithinRange();
			this.checkTimer = 0f;
		}
		this.checkTimer += Time.deltaTime;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PlayerAvatar playerAvatar in this.capturedPlayerAvatars)
			{
				playerAvatar.tumble.TumbleOverrideTime(2f);
				this.PlayerTumble(playerAvatar);
			}
			foreach (PhysGrabObject physGrabObject in this.capturedPhysGrabObjects)
			{
				if (physGrabObject && physGrabObject.isEnemy)
				{
					Enemy enemy = physGrabObject.GetComponent<EnemyRigidbody>().enemy;
					if (enemy && enemy.HasStateStunned && enemy.Type < EnemyType.Heavy)
					{
						enemy.StateStunned.Set(4f);
					}
				}
				physGrabObject.OverrideZeroGravity(0.1f);
			}
		}
	}

	// Token: 0x0600026F RID: 623 RVA: 0x00018EB8 File Offset: 0x000170B8
	private void StateStop()
	{
		if (this.stateStart)
		{
			this.checkTimer = 0f;
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PlayerAvatar playerAvatar in this.capturedPlayerAvatars)
			{
				if (playerAvatar)
				{
					playerAvatar.tumble.TumbleOverrideTime(2f);
				}
			}
		}
		this.checkTimer += Time.deltaTime;
		if (this.checkTimer > 0.35f)
		{
			this.RemoveAllOutOfRange();
			this.checkTimer = 0f;
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00018F70 File Offset: 0x00017170
	private void StateSmash()
	{
		if (this.stateStart)
		{
			GameDirector.instance.CameraShake.ShakeDistance(6f, 3f, 8f, base.transform.position, 0.1f);
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
			foreach (PhysGrabObject physGrabObject in this.capturedPhysGrabObjects)
			{
				if (physGrabObject)
				{
					this.downParticle.transform.position = physGrabObject.midPoint;
					this.downParticle.Emit(1);
				}
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				foreach (PlayerAvatar playerAvatar in this.capturedPlayerAvatars)
				{
					if (playerAvatar && playerAvatar.tumble.isTumbling)
					{
						playerAvatar.tumble.TumbleOverrideTime(2f);
						playerAvatar.tumble.ImpactHurtSet(2f, this.damage);
					}
				}
				foreach (PhysGrabObject physGrabObject2 in this.capturedPhysGrabObjects)
				{
					if (physGrabObject2 && physGrabObject2 && physGrabObject2.rb && !physGrabObject2.rb.isKinematic)
					{
						physGrabObject2.rb.AddForce(Vector3.down * 30f, ForceMode.Impulse);
					}
				}
			}
			foreach (FloaterLine floaterLine in this.floaterLines)
			{
				if (floaterLine)
				{
					floaterLine.outro = true;
				}
			}
			this.floaterLines.Clear();
			this.capturedPlayerAvatars.Clear();
			this.capturedPhysGrabObjects.Clear();
			this.stateStart = false;
		}
		this.sphereEffects.localScale = Vector3.Lerp(this.sphereEffects.localScale, Vector3.zero, Time.deltaTime * 2f);
		if (this.sphereEffects.localScale.x > 0.5f)
		{
			this.attackLight.intensity = Mathf.Lerp(this.attackLight.intensity, 20f, Time.deltaTime * 60f);
		}
		else
		{
			this.attackLight.intensity = 20f * this.sphereEffects.localScale.magnitude;
		}
		if (this.sphereEffects.localScale.x < 0.01f)
		{
			this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
		}
	}

	// Token: 0x06000271 RID: 625 RVA: 0x0001927C File Offset: 0x0001747C
	private void RemoveAllOutOfRange()
	{
		for (int i = this.capturedPlayerAvatars.Count - 1; i >= 0; i--)
		{
			PlayerAvatar playerAvatar = this.capturedPlayerAvatars[i];
			if (!playerAvatar)
			{
				this.capturedPlayerAvatars.RemoveAt(i);
			}
			else if (Vector3.Distance(new Vector3(playerAvatar.transform.position.x, base.transform.position.y, playerAvatar.transform.position.z), base.transform.position) > this.range * 1.2f)
			{
				this.capturedPlayerAvatars.RemoveAt(i);
				foreach (FloaterLine floaterLine in this.floaterLines)
				{
					if (floaterLine && floaterLine.lineTarget == playerAvatar.PlayerVisionTarget.VisionTransform)
					{
						floaterLine.outro = true;
					}
				}
			}
		}
		for (int j = this.capturedPhysGrabObjects.Count - 1; j >= 0; j--)
		{
			PhysGrabObject physGrabObject = this.capturedPhysGrabObjects[j];
			if (!physGrabObject)
			{
				this.capturedPhysGrabObjects.RemoveAt(j);
			}
			else if (Vector3.Distance(new Vector3(physGrabObject.transform.position.x, base.transform.position.y, physGrabObject.transform.position.z), base.transform.position) > this.range * 1.2f)
			{
				this.capturedPhysGrabObjects.RemoveAt(j);
			}
		}
	}

	// Token: 0x06000272 RID: 626 RVA: 0x00019440 File Offset: 0x00017640
	private void StateLevitateFixed()
	{
		if (this.state != FloaterAttackLogic.FloaterAttackState.levitate)
		{
			return;
		}
		if (this.tumblePhysObjectCheckTimer > 1f)
		{
			foreach (PlayerAvatar playerAvatar in this.capturedPlayerAvatars)
			{
				if (playerAvatar.tumble.isTumbling)
				{
					PhysGrabObject physGrabObject = playerAvatar.tumble.physGrabObject;
					if (!this.capturedPhysGrabObjects.Contains(physGrabObject))
					{
						this.capturedPhysGrabObjects.Add(physGrabObject);
					}
				}
			}
			this.tumblePhysObjectCheckTimer = 0f;
		}
		else
		{
			this.tumblePhysObjectCheckTimer += Time.fixedDeltaTime;
		}
		foreach (PhysGrabObject physGrabObject2 in this.capturedPhysGrabObjects)
		{
			if (physGrabObject2)
			{
				float d = 10f;
				if (physGrabObject2.GetComponent<PlayerTumble>())
				{
					d = 20f;
				}
				if (physGrabObject2 && physGrabObject2.rb && !physGrabObject2.rb.isKinematic)
				{
					physGrabObject2.rb.AddForce(Vector3.up * Time.fixedDeltaTime * d, ForceMode.Force);
					physGrabObject2.rb.AddTorque(Vector3.up * Time.fixedDeltaTime * 0.2f, ForceMode.Force);
					physGrabObject2.rb.AddTorque(Vector3.left * Time.fixedDeltaTime * 0.1f, ForceMode.Force);
					physGrabObject2.rb.velocity = Vector3.Lerp(physGrabObject2.rb.velocity, new Vector3(0f, physGrabObject2.rb.velocity.y, 0f), Time.fixedDeltaTime * 2f);
				}
			}
		}
		if (this.particleCount < this.capturedPhysGrabObjects.Count)
		{
			if (this.capturedPhysGrabObjects[this.particleCount])
			{
				Vector3 vector = this.capturedPhysGrabObjects[this.particleCount].transform.position;
				Vector3 vector2 = Random.insideUnitSphere * 2f;
				vector2.y = -Mathf.Abs(vector2.y);
				vector += vector2;
				this.upParticle.transform.position = vector;
				this.upParticle.Emit(1);
			}
			this.particleCount++;
			return;
		}
		this.particleCount = 0;
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0001970C File Offset: 0x0001790C
	private void StateStopFixed()
	{
		if (this.state != FloaterAttackLogic.FloaterAttackState.stop)
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PhysGrabObject physGrabObject in this.capturedPhysGrabObjects)
			{
				if (physGrabObject && physGrabObject.rb && !physGrabObject.rb.isKinematic)
				{
					physGrabObject.OverrideZeroGravity(0.1f);
					if (physGrabObject.isEnemy)
					{
						Enemy enemy = physGrabObject.GetComponent<EnemyRigidbody>().enemy;
						if (enemy && enemy.HasStateStunned && enemy.Type < EnemyType.Heavy)
						{
							enemy.StateStunned.Set(4f);
						}
					}
					physGrabObject.rb.velocity = Vector3.Lerp(physGrabObject.rb.velocity, Vector3.zero, Time.deltaTime * 2f);
				}
			}
		}
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00019810 File Offset: 0x00017A10
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.StateLevitateFixed();
			this.StateStopFixed();
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00019825 File Offset: 0x00017A25
	private void Update()
	{
		this.StateMachine();
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0001982D File Offset: 0x00017A2D
	public void StateSet(FloaterAttackLogic.FloaterAttackState _state)
	{
		if (this.state != _state)
		{
			this.state = _state;
			this.stateStart = true;
		}
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00019848 File Offset: 0x00017A48
	public void GetAllWithinRange()
	{
		this.RemoveAllOutOfRange();
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetAllPlayerAvatarWithinRange(this.range, base.transform.position, false, default(LayerMask)))
		{
			if (!this.capturedPlayerAvatars.Contains(playerAvatar))
			{
				this.capturedPlayerAvatars.Add(playerAvatar);
				this.PlayerTumble(playerAvatar);
				FloaterLine component = Object.Instantiate<GameObject>(this.linePrefab, base.transform.position, Quaternion.identity, base.transform).GetComponent<FloaterLine>();
				component.lineTarget = playerAvatar.PlayerVisionTarget.VisionTransform;
				component.floaterAttack = this;
				this.floaterLines.Add(component);
			}
		}
		foreach (PhysGrabObject physGrabObject in SemiFunc.PhysGrabObjectGetAllWithinRange(this.range, base.transform.position, false, default(LayerMask), null))
		{
			if (!(physGrabObject == this.enemyFloaterPhysGrabObject) && !this.capturedPhysGrabObjects.Contains(physGrabObject))
			{
				this.capturedPhysGrabObjects.Add(physGrabObject);
			}
		}
	}

	// Token: 0x06000278 RID: 632 RVA: 0x000199A4 File Offset: 0x00017BA4
	private void PlayerTumble(PlayerAvatar _player)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!_player)
		{
			return;
		}
		if (_player.isDisabled)
		{
			return;
		}
		if (!_player.tumble.isTumbling)
		{
			_player.tumble.TumbleRequest(true, false);
		}
		_player.tumble.TumbleOverrideTime(2f);
	}

	// Token: 0x06000279 RID: 633 RVA: 0x000199F5 File Offset: 0x00017BF5
	private void OnEnable()
	{
		this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
	}

	// Token: 0x0600027A RID: 634 RVA: 0x000199FE File Offset: 0x00017BFE
	private void OnDisable()
	{
		this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
		this.StateInactive();
	}

	// Token: 0x0400047A RID: 1146
	public GameObject linePrefab;

	// Token: 0x0400047B RID: 1147
	public ParticleSystem upParticle;

	// Token: 0x0400047C RID: 1148
	public ParticleSystem downParticle;

	// Token: 0x0400047D RID: 1149
	public EnemyFloater controller;

	// Token: 0x0400047E RID: 1150
	public PhysGrabObject enemyFloaterPhysGrabObject;

	// Token: 0x0400047F RID: 1151
	internal int damage = 50;

	// Token: 0x04000480 RID: 1152
	internal FloaterAttackLogic.FloaterAttackState state = FloaterAttackLogic.FloaterAttackState.inactive;

	// Token: 0x04000481 RID: 1153
	private bool stateStart = true;

	// Token: 0x04000482 RID: 1154
	public Transform sphereEffects;

	// Token: 0x04000483 RID: 1155
	public Light attackLight;

	// Token: 0x04000484 RID: 1156
	private float range = 4f;

	// Token: 0x04000485 RID: 1157
	private List<PlayerAvatar> capturedPlayerAvatars = new List<PlayerAvatar>();

	// Token: 0x04000486 RID: 1158
	private List<PhysGrabObject> capturedPhysGrabObjects = new List<PhysGrabObject>();

	// Token: 0x04000487 RID: 1159
	private List<FloaterLine> floaterLines = new List<FloaterLine>();

	// Token: 0x04000488 RID: 1160
	private float checkTimer;

	// Token: 0x04000489 RID: 1161
	private int particleCount;

	// Token: 0x0400048A RID: 1162
	private float tumblePhysObjectCheckTimer;

	// Token: 0x0200030F RID: 783
	public enum FloaterAttackState
	{
		// Token: 0x0400287F RID: 10367
		start,
		// Token: 0x04002880 RID: 10368
		levitate,
		// Token: 0x04002881 RID: 10369
		stop,
		// Token: 0x04002882 RID: 10370
		smash,
		// Token: 0x04002883 RID: 10371
		end,
		// Token: 0x04002884 RID: 10372
		inactive
	}
}
