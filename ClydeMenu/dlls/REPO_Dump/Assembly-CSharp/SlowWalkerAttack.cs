using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class SlowWalkerAttack : MonoBehaviour
{
	// Token: 0x060004A7 RID: 1191 RVA: 0x0002E562 File Offset: 0x0002C762
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.vacuumParticles.AddRange(this.attackVacuumBuildup.GetComponentsInChildren<ParticleSystem>());
		this.impactParticles.AddRange(this.attackImpact.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0002E59C File Offset: 0x0002C79C
	private void SuckInListUpdate()
	{
		if (!this.clubHitPoint)
		{
			return;
		}
		base.transform.position = this.clubHitPoint.position;
		RaycastHit[] array = Physics.RaycastAll(this.slowWalkerCenter, (this.clubHitPoint.position - this.slowWalkerCenter).normalized, 4f, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		bool flag = false;
		Vector3 point = this.slowWalkerCenter;
		float num = float.MaxValue;
		foreach (RaycastHit raycastHit in array)
		{
			if (raycastHit.collider.gameObject.CompareTag("Wall"))
			{
				float num2 = Vector3.Distance(this.slowWalkerCenter, raycastHit.point);
				if (num2 < num)
				{
					num = num2;
					point = raycastHit.point;
					flag = true;
				}
			}
		}
		if (flag)
		{
			this.foundPosition = point;
			this.foundPosition = Vector3.MoveTowards(this.foundPosition, this.slowWalkerCenter, 0.2f);
			this.didFindPosition = true;
		}
		point = this.slowWalkerCenter;
		num = float.MaxValue;
		RaycastHit[] array3 = Physics.RaycastAll(base.transform.position, Vector3.down, 2f, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		flag = false;
		foreach (RaycastHit raycastHit2 in array3)
		{
			if (raycastHit2.collider.gameObject.CompareTag("Wall"))
			{
				float num3 = Vector3.Distance(base.transform.position, raycastHit2.point);
				if (num3 < num)
				{
					num = num3;
					point = raycastHit2.point;
					flag = true;
				}
			}
		}
		if (flag)
		{
			this.foundPosition = point;
			this.didFindPosition = true;
		}
		if (this.didFindPosition)
		{
			base.transform.position = this.foundPosition;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.physGrabObjects.Clear();
		foreach (PhysGrabObject physGrabObject in SemiFunc.PhysGrabObjectGetAllWithinRange(this.vacuumSphere.localScale.x * 0.5f, this.vacuumSphere.position + Vector3.up * 0.5f, false, default(LayerMask), null))
		{
			RaycastHit[] array4 = Physics.RaycastAll(physGrabObject.midPoint, base.transform.position + Vector3.up * 0.5f - physGrabObject.midPoint, this.vacuumSphere.localScale.x, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
			bool flag2 = false;
			foreach (RaycastHit raycastHit3 in array4)
			{
				if (raycastHit3.collider.gameObject.CompareTag("Wall"))
				{
					flag2 = true;
				}
			}
			if (!flag2 && !physGrabObject.isPlayer && physGrabObject != this.enemyPhysGrabObject)
			{
				this.physGrabObjects.Add(physGrabObject);
			}
		}
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0002E8CC File Offset: 0x0002CACC
	private void SuckInListPlayerUpdate()
	{
		Vector3 vector = this.vacuumSphere.position + Vector3.up * 2f;
		Vector3 a = vector;
		this.playersBeingVacuumed.Clear();
		List<PlayerAvatar> list = SemiFunc.PlayerGetAllPlayerAvatarWithinRange(this.vacuumSphere.localScale.x, vector, false, default(LayerMask));
		this.playersBeingVacuumed.AddRange(list);
		this.playerTumbles.Clear();
		foreach (PlayerAvatar playerAvatar in this.playersBeingVacuumed)
		{
			vector = this.vacuumSphere.position + Vector3.up * 2f;
			Vector3 position = playerAvatar.PlayerVisionTarget.VisionTransform.position;
			Vector3 normalized = (position - vector).normalized;
			float maxDistance = Vector3.Distance(vector, position);
			RaycastHit[] array = Physics.RaycastAll(vector, normalized, maxDistance, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
			bool flag = false;
			foreach (RaycastHit raycastHit in array)
			{
				if (raycastHit.collider.gameObject.CompareTag("Wall"))
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			if (flag)
			{
				foreach (RaycastHit raycastHit2 in Physics.RaycastAll(vector, Vector3.up, this.vacuumSphere.localScale.x * 0.25f, LayerMask.GetMask(new string[]
				{
					"Default"
				})))
				{
					if (raycastHit2.collider.gameObject.CompareTag("Wall"))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					vector = a + Vector3.up * this.vacuumSphere.localScale.x * 0.25f;
					normalized = (position - vector).normalized;
					maxDistance = Vector3.Distance(vector, position);
					RaycastHit[] array3 = Physics.RaycastAll(vector, normalized, maxDistance, LayerMask.GetMask(new string[]
					{
						"Default"
					}));
					flag = false;
					foreach (RaycastHit raycastHit3 in array3)
					{
						if (raycastHit3.collider.gameObject.CompareTag("Wall"))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag && !flag2)
			{
				foreach (RaycastHit raycastHit4 in Physics.RaycastAll(vector, Vector3.up, this.vacuumSphere.localScale.x * 0.5f, LayerMask.GetMask(new string[]
				{
					"Default"
				})))
				{
					if (raycastHit4.collider.gameObject.CompareTag("Wall"))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					vector = a + Vector3.up * this.vacuumSphere.localScale.x * 0.5f;
					normalized = (position - vector).normalized;
					maxDistance = Vector3.Distance(vector, position);
					RaycastHit[] array4 = Physics.RaycastAll(vector, normalized, maxDistance, LayerMask.GetMask(new string[]
					{
						"Default"
					}));
					flag = false;
					foreach (RaycastHit raycastHit5 in array4)
					{
						if (raycastHit5.collider.gameObject.CompareTag("Wall"))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				if (playerAvatar.isTumbling)
				{
					this.playerTumbles.Add(playerAvatar.tumble);
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						playerAvatar.tumble.TumbleOverrideTime(2f);
						playerAvatar.tumble.OverrideEnemyHurt(0.5f);
					}
				}
				if (!playerAvatar.isDisabled && !playerAvatar.isTumbling)
				{
					this.playerTumbles.Add(playerAvatar.tumble);
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						playerAvatar.tumble.TumbleRequest(true, false);
						playerAvatar.tumble.TumbleOverrideTime(2f);
						playerAvatar.tumble.OverrideEnemyHurt(0.5f);
					}
				}
			}
		}
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0002ED38 File Offset: 0x0002CF38
	private void StateIdle()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart && !this.stateFixed)
		{
			this.ParticlesVacuum(false);
			this.didFindPosition = false;
			this.stateStart = false;
		}
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0002ED68 File Offset: 0x0002CF68
	private void StateCheckInitial()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart && !this.stateFixed)
		{
			this.didFindPosition = false;
			this.SuckInListUpdate();
			this.stateStart = false;
			this.stateTimer = 0.001f;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.stateTimer <= 0f)
		{
			this.StateSet(SlowWalkerAttack.State.Implosion);
		}
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0002EDCC File Offset: 0x0002CFCC
	private void StateImplosion()
	{
		if (this.stateStart && !this.stateFixed)
		{
			this.ParticlesVacuum(true);
			this.stateStart = false;
			this.stateTimer = 1.5f;
			if (this.clubHitPoint)
			{
				base.transform.position = this.clubHitPoint.position;
			}
			this.ActivateImplosionHurtColliders();
			this.ActiveHurtColliderFirstHit();
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 6f, 15f, base.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(5f, 6f, 15f, base.transform.position, 0.1f);
			this.soundVacuumImpact.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundVacuumImpactGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundVacuumBuildup.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Vector3 normalized = (base.transform.position - this.slowWalkerCenter).normalized;
			base.transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
			float y = base.transform.rotation.eulerAngles.y;
			base.transform.rotation = Quaternion.Euler(0f, y, 0f);
			this.SuckInListPlayerUpdate();
		}
		if (this.stateFixed)
		{
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			foreach (PlayerTumble playerTumble in this.playerTumbles)
			{
				if (playerTumble.isTumbling)
				{
					Vector3 normalized2 = (this.vacuumSphere.position - playerTumble.physGrabObject.transform.position).normalized;
					Rigidbody rb = playerTumble.physGrabObject.rb;
					rb.AddForce(normalized2 * 2500f * Time.fixedDeltaTime, ForceMode.Force);
					Vector3 a = SemiFunc.PhysFollowDirection(rb.transform, normalized2, rb, 10f) * 2f;
					rb.AddTorque(a / rb.mass, ForceMode.Force);
				}
			}
			foreach (PhysGrabObject physGrabObject in this.physGrabObjects)
			{
				if (physGrabObject)
				{
					Vector3 normalized3 = (this.vacuumSphere.position - physGrabObject.transform.position).normalized;
					Rigidbody rb2 = physGrabObject.rb;
					rb2.AddForce(normalized3 * 2500f * Time.fixedDeltaTime, ForceMode.Force);
					Vector3 a2 = SemiFunc.PhysFollowDirection(rb2.transform, normalized3, rb2, 10f) * 2f;
					rb2.AddTorque(a2 / rb2.mass, ForceMode.Force);
				}
			}
		}
		if (!this.stateFixed)
		{
			if (this.didFindPosition)
			{
				base.transform.position = this.foundPosition;
			}
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			if (this.enemy && this.enemy.CurrentState == EnemyState.Stunned && this.currentState != SlowWalkerAttack.State.Idle)
			{
				this.StateSet(SlowWalkerAttack.State.Idle);
			}
			this.enemy.Health.ObjectHurtDisable(0.5f);
			if (this.stateTimer <= 0f)
			{
				this.StateSet(SlowWalkerAttack.State.Attack);
			}
			if (SemiFunc.FPSImpulse5())
			{
				this.SuckInListPlayerUpdate();
			}
		}
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0002F1D8 File Offset: 0x0002D3D8
	private void StateDelay()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart && !this.stateFixed)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0002F1FA File Offset: 0x0002D3FA
	private void StateCheckAttack()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart && !this.stateFixed)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0002F21C File Offset: 0x0002D41C
	private void ActivateAttackHurtColliders()
	{
		this.attackImpactHurtColliders.SetActive(true);
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0002F22A File Offset: 0x0002D42A
	private void ActivateImplosionHurtColliders()
	{
		this.attackVacuumHurtCollider.SetActive(true);
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0002F238 File Offset: 0x0002D438
	private void ActiveHurtColliderFirstHit()
	{
		this.hurtColliderFirstHit.SetActive(true);
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0002F248 File Offset: 0x0002D448
	private void StateAttack()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart && !this.stateFixed)
		{
			this.stateStart = false;
			this.stateTimer = 3.5f;
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 6f, 15f, base.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(8f, 6f, 15f, base.transform.position, 0.1f);
			this.ParticlesPlayImpact();
			this.soundImpact.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundImpactGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Vector3 normalized = (base.transform.position - this.slowWalkerCenter).normalized;
			base.transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
			this.ActivateAttackHurtColliders();
		}
		if (this.stateTimer <= 0f)
		{
			this.StateSet(SlowWalkerAttack.State.Idle);
		}
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0002F390 File Offset: 0x0002D590
	private void StateMachine(bool _stateFixed)
	{
		if (_stateFixed)
		{
			this.stateFixed = true;
		}
		switch (this.currentState)
		{
		case SlowWalkerAttack.State.Idle:
			this.StateIdle();
			break;
		case SlowWalkerAttack.State.CheckInitial:
			this.StateCheckInitial();
			break;
		case SlowWalkerAttack.State.Implosion:
			this.StateImplosion();
			break;
		case SlowWalkerAttack.State.Attack:
			this.StateAttack();
			break;
		}
		if (_stateFixed && this.stateFixed)
		{
			this.stateFixed = false;
		}
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0002F3FE File Offset: 0x0002D5FE
	public void SlowWalkerAttackStart()
	{
		this.StateSet(SlowWalkerAttack.State.CheckInitial);
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0002F408 File Offset: 0x0002D608
	private void Update()
	{
		if (this.enemyPhysGrabObject)
		{
			this.slowWalkerCenter = this.enemyPhysGrabObject.midPoint;
		}
		if (SemiFunc.FPSImpulse1() && this.enemy && this.enemy.EnemyParent && !this.enemy.EnemyParent.Spawned && this.currentState != SlowWalkerAttack.State.Idle)
		{
			this.StateSet(SlowWalkerAttack.State.Idle);
		}
		this.StateMachine(false);
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= Time.deltaTime;
		}
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0002F4A0 File Offset: 0x0002D6A0
	private void FixedUpdate()
	{
		this.StateMachine(true);
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0002F4AC File Offset: 0x0002D6AC
	public void StateSet(SlowWalkerAttack.State state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			if (state != this.currentState)
			{
				this.StateSetRPC(state, default(PhotonMessageInfo));
				return;
			}
		}
		else if (state != this.currentState)
		{
			this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
			{
				state
			});
		}
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0002F50B File Offset: 0x0002D70B
	[PunRPC]
	public void StateSetRPC(SlowWalkerAttack.State state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0002F524 File Offset: 0x0002D724
	private void ParticlesVacuum(bool _play)
	{
		foreach (ParticleSystem particleSystem in this.vacuumParticles)
		{
			if (_play)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
				}
				particleSystem.Play();
			}
			else if (particleSystem.isPlaying)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0002F598 File Offset: 0x0002D798
	private void ParticlesPlayImpact()
	{
		foreach (ParticleSystem particleSystem in this.impactParticles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0002F5E8 File Offset: 0x0002D7E8
	public void TrudgeAttackTest()
	{
		new List<HurtCollider>().AddRange(this.attackVacuumHurtCollider.GetComponentsInChildren<HurtCollider>());
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0002F5FF File Offset: 0x0002D7FF
	private void OnDisable()
	{
		this.attackVacuumHurtCollider.SetActive(false);
		this.hurtColliderFirstHit.SetActive(false);
		this.attackImpactHurtColliders.SetActive(false);
		this.attackImpact.SetActive(false);
		this.attackVacuumBuildup.SetActive(false);
	}

	// Token: 0x0400079F RID: 1951
	public Transform vacuumSphere;

	// Token: 0x040007A0 RID: 1952
	[Space(10f)]
	public GameObject attackVacuumBuildup;

	// Token: 0x040007A1 RID: 1953
	public GameObject attackVacuumHurtCollider;

	// Token: 0x040007A2 RID: 1954
	public GameObject attackImpact;

	// Token: 0x040007A3 RID: 1955
	public GameObject attackImpactHurtColliders;

	// Token: 0x040007A4 RID: 1956
	private PhotonView photonView;

	// Token: 0x040007A5 RID: 1957
	[Space(10f)]
	private List<PlayerAvatar> playersBeingVacuumed = new List<PlayerAvatar>();

	// Token: 0x040007A6 RID: 1958
	private List<PlayerTumble> playerTumbles = new List<PlayerTumble>();

	// Token: 0x040007A7 RID: 1959
	private List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	// Token: 0x040007A8 RID: 1960
	private List<ParticleSystem> vacuumParticles = new List<ParticleSystem>();

	// Token: 0x040007A9 RID: 1961
	private List<ParticleSystem> impactParticles = new List<ParticleSystem>();

	// Token: 0x040007AA RID: 1962
	[Space(10f)]
	public Sound soundVacuumImpact;

	// Token: 0x040007AB RID: 1963
	public Sound soundVacuumImpactGlobal;

	// Token: 0x040007AC RID: 1964
	public Sound soundVacuumBuildup;

	// Token: 0x040007AD RID: 1965
	public Sound soundImpact;

	// Token: 0x040007AE RID: 1966
	public Sound soundImpactGlobal;

	// Token: 0x040007AF RID: 1967
	public PhysGrabObject enemyPhysGrabObject;

	// Token: 0x040007B0 RID: 1968
	public Enemy enemy;

	// Token: 0x040007B1 RID: 1969
	internal SlowWalkerAttack.State currentState;

	// Token: 0x040007B2 RID: 1970
	private bool stateStart;

	// Token: 0x040007B3 RID: 1971
	private bool stateFixed;

	// Token: 0x040007B4 RID: 1972
	private float stateTimer;

	// Token: 0x040007B5 RID: 1973
	public Transform clubHitPoint;

	// Token: 0x040007B6 RID: 1974
	public GameObject hurtColliderFirstHit;

	// Token: 0x040007B7 RID: 1975
	private Vector3 foundPosition;

	// Token: 0x040007B8 RID: 1976
	private bool didFindPosition;

	// Token: 0x040007B9 RID: 1977
	private Vector3 slowWalkerCenter;

	// Token: 0x0200031F RID: 799
	public enum State
	{
		// Token: 0x04002922 RID: 10530
		Idle,
		// Token: 0x04002923 RID: 10531
		CheckInitial,
		// Token: 0x04002924 RID: 10532
		Implosion,
		// Token: 0x04002925 RID: 10533
		Delay,
		// Token: 0x04002926 RID: 10534
		CheckAttack,
		// Token: 0x04002927 RID: 10535
		Attack
	}
}
