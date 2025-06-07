using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class EnemyHiddenOld : MonoBehaviour
{
	// Token: 0x06000353 RID: 851 RVA: 0x000214B1 File Offset: 0x0001F6B1
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.startPosition = base.transform.position;
		this.footStepPosition = this.startPosition;
		this.previousPosition = this.startPosition;
	}

	// Token: 0x06000354 RID: 852 RVA: 0x000214E8 File Offset: 0x0001F6E8
	private void Update()
	{
		this.StateRoam();
		this.StatePlayerNotice();
		this.StateGetPlayer();
		this.StateGoToTarget();
		this.StatePickUpTarget();
		this.StateFindFarawayPoint();
		this.StateKidnapTarget();
		this.StateTauntTarget();
		this.StateDropTarget();
		this.StateDespawn();
		this.FootstepLogic();
		this.SprintTick();
		this.BreathTick();
		this.Breathing();
		this.stateEnd = false;
		if (this.stateTimer > 0f)
		{
			if (this.initialStateTime == 0f)
			{
				this.initialStateTime = this.stateTimer;
			}
			this.stateTimer -= Time.deltaTime;
			this.stateTimer = Mathf.Max(0f, this.stateTimer);
		}
		else if (!this.stateEnd && this.stateTimer != -123f)
		{
			this.stateEnd = true;
			this.stateTimer = -123f;
			this.initialStateTime = 0f;
		}
		if (this.stateSetTo != -1)
		{
			this.currentState = this.stateSetTo;
			this.stateStart = true;
			this.settingState = false;
			this.stateEnd = false;
			this.stateSetTo = -1;
		}
		float num = 0.5f;
		base.transform.position = this.startPosition + new Vector3(Mathf.Sin(Time.time * num) * 1f, 0f, Mathf.Cos(Time.time * num) * 1f);
	}

	// Token: 0x06000355 RID: 853 RVA: 0x00021650 File Offset: 0x0001F850
	private void FootstepLogic()
	{
		Vector3 normalized = (base.transform.position - this.previousPosition).normalized;
		base.transform.LookAt(base.transform.position + normalized);
		Debug.DrawRay(base.transform.position, normalized, Color.green, 0.1f);
		this.previousPosition = base.transform.position;
		float num = 1f;
		if (this.isSprinting)
		{
			num = 1.8f;
		}
		if (Vector3.Distance(this.footStepPosition, this.grounded.position) > 0.5f * num)
		{
			Vector3 a = Vector3.Cross(Vector3.up, base.transform.forward);
			Vector3 a2 = -a;
			Vector3 vector = Vector3.down + (this.rightFoot ? (a * 0.2f) : (a2 * 0.2f));
			vector += base.transform.forward * 0.3f * num;
			this.rightFoot = !this.rightFoot;
			Debug.DrawRay(base.transform.position, vector, Color.red, 0.1f);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, vector * 2f, out raycastHit, 3f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.footStepPosition = this.grounded.position;
				this.footstepParticlesTransform.position = raycastHit.point;
				this.footstepParticleSmoke.Play();
				this.footstepParticlesTransform.transform.LookAt(base.transform.position + normalized);
				this.footstepParticleFoot.Play();
				if (this.isSprinting)
				{
					Materials.Instance.Impulse(raycastHit.point, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
					this.soundFootstepSprint.Play(raycastHit.point, 1f, 1f, 1f, 1f);
				}
				else
				{
					Materials.Instance.Impulse(raycastHit.point, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
					this.soundFootstepWalk.Play(raycastHit.point, 1f, 1f, 1f, 1f);
				}
				Quaternion.LookRotation(base.transform.forward);
				Debug.DrawRay(base.transform.position, normalized, Color.blue, 2f);
				ParticleSystem.MainModule main = this.footstepParticleFoot.main;
				main.startRotation3D = true;
				Vector2 to = new Vector2(base.transform.forward.x, base.transform.forward.z);
				float num2 = Vector2.SignedAngle(Vector2.up, to) + 90f;
				float constant = (this.rightFoot ? -90f : 90f) * 0.017453292f;
				float num3 = this.rightFoot ? -90f : 90f;
				num3 += num2;
				num3 *= 0.017453292f;
				main.startRotationX = new ParticleSystem.MinMaxCurve(constant);
				main.startRotationY = new ParticleSystem.MinMaxCurve(num3);
				main.startRotationZ = new ParticleSystem.MinMaxCurve(0f);
			}
		}
	}

	// Token: 0x06000356 RID: 854 RVA: 0x000219AC File Offset: 0x0001FBAC
	private void StateSet(EnemyHiddenOld.State newState)
	{
		if (this.settingState)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient() && this.stateSetTo == -1)
			{
				this.settingState = true;
				this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
				{
					(int)newState
				});
				return;
			}
		}
		else if (this.stateSetTo == -1)
		{
			this.settingState = true;
			this.StateSetRPC((int)newState);
		}
	}

	// Token: 0x06000357 RID: 855 RVA: 0x00021A18 File Offset: 0x0001FC18
	[PunRPC]
	public void StateSetRPC(int state)
	{
		this.stateSetTo = state;
		this.stateTimer = 0f;
		this.stateEnd = true;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00021A33 File Offset: 0x0001FC33
	private bool StateIs(EnemyHiddenOld.State state)
	{
		return this.currentState == (int)state;
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00021A3E File Offset: 0x0001FC3E
	private void Sprinting()
	{
		this.sprintingTime = 0.2f;
		this.isSprinting = true;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00021A52 File Offset: 0x0001FC52
	private void SprintTick()
	{
		if (this.sprintingTime > 0f)
		{
			this.sprintingTime -= Time.deltaTime;
			return;
		}
		this.isSprinting = false;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00021A7B File Offset: 0x0001FC7B
	private void Breathing()
	{
		this.breathTimer = 0.2f;
		this.isBreathing = true;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00021A90 File Offset: 0x0001FC90
	private void BreathTick()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.breathTimer > 0f)
		{
			this.breathTimer -= Time.deltaTime;
		}
		else
		{
			this.isBreathing = false;
		}
		if (this.isBreathing)
		{
			this.breathCycleTimer += Time.deltaTime;
			float num = 3f;
			if (this.breatheIn)
			{
				num = 4.5f;
			}
			if (this.breathCycleTimer > num)
			{
				this.breathCycleTimer = 0f;
				if (this.breatheIn)
				{
					this.BreatheIn();
				}
				else
				{
					this.BreatheOut();
				}
				this.breatheIn = !this.breatheIn;
			}
		}
	}

	// Token: 0x0600035D RID: 861 RVA: 0x00021B34 File Offset: 0x0001FD34
	private void BreatheIn()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("BreatheInRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.BreatheInRPC();
		}
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00021B61 File Offset: 0x0001FD61
	[PunRPC]
	public void BreatheInRPC()
	{
		this.soundBreatheIn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00021B8E File Offset: 0x0001FD8E
	private void BreatheOut()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("BreatheOutRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.BreatheOutRPC();
		}
	}

	// Token: 0x06000360 RID: 864 RVA: 0x00021BBB File Offset: 0x0001FDBB
	[PunRPC]
	public void BreatheOutRPC()
	{
		this.soundBreatheOut.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.breathParticles.Play();
	}

	// Token: 0x06000361 RID: 865 RVA: 0x00021BF3 File Offset: 0x0001FDF3
	private void StateRoam()
	{
		if (!this.StateIs(EnemyHiddenOld.State.Roam))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000362 RID: 866 RVA: 0x00021C15 File Offset: 0x0001FE15
	private void StatePlayerNotice()
	{
		if (!this.StateIs(EnemyHiddenOld.State.PlayerNotice))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x00021C37 File Offset: 0x0001FE37
	private void StateGetPlayer()
	{
		if (!this.StateIs(EnemyHiddenOld.State.GetPlayer))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x00021C59 File Offset: 0x0001FE59
	private void StateGoToTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.GoToTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000365 RID: 869 RVA: 0x00021C7B File Offset: 0x0001FE7B
	private void StatePickUpTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.PickUpTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000366 RID: 870 RVA: 0x00021C9D File Offset: 0x0001FE9D
	private void StateFindFarawayPoint()
	{
		if (!this.StateIs(EnemyHiddenOld.State.FindFarawayPoint))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00021CBF File Offset: 0x0001FEBF
	private void StateKidnapTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.KidnapTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00021CE1 File Offset: 0x0001FEE1
	private void StateTauntTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.TauntTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000369 RID: 873 RVA: 0x00021D03 File Offset: 0x0001FF03
	private void StateDropTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.DropTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00021D25 File Offset: 0x0001FF25
	private void StateDespawn()
	{
		if (!this.StateIs(EnemyHiddenOld.State.Despawn))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x040005EA RID: 1514
	private Vector3 startPosition;

	// Token: 0x040005EB RID: 1515
	private Vector3 footStepPosition;

	// Token: 0x040005EC RID: 1516
	public Materials.MaterialTrigger material;

	// Token: 0x040005ED RID: 1517
	public Transform grounded;

	// Token: 0x040005EE RID: 1518
	public Transform footstepParticlesTransform;

	// Token: 0x040005EF RID: 1519
	public ParticleSystem footstepParticleSmoke;

	// Token: 0x040005F0 RID: 1520
	private bool rightFoot = true;

	// Token: 0x040005F1 RID: 1521
	private Vector3 previousPosition;

	// Token: 0x040005F2 RID: 1522
	public ParticleSystem footstepParticleFoot;

	// Token: 0x040005F3 RID: 1523
	private bool isSprinting;

	// Token: 0x040005F4 RID: 1524
	private int currentState;

	// Token: 0x040005F5 RID: 1525
	private bool settingState;

	// Token: 0x040005F6 RID: 1526
	private int stateSetTo = -1;

	// Token: 0x040005F7 RID: 1527
	private PhotonView photonView;

	// Token: 0x040005F8 RID: 1528
	private float stateTimer;

	// Token: 0x040005F9 RID: 1529
	private bool stateEnd;

	// Token: 0x040005FA RID: 1530
	private bool stateStart;

	// Token: 0x040005FB RID: 1531
	private float initialStateTime;

	// Token: 0x040005FC RID: 1532
	private float sprintingTime;

	// Token: 0x040005FD RID: 1533
	public ParticleSystem breathParticles;

	// Token: 0x040005FE RID: 1534
	private float breathTimer;

	// Token: 0x040005FF RID: 1535
	private bool isBreathing;

	// Token: 0x04000600 RID: 1536
	private bool breatheIn = true;

	// Token: 0x04000601 RID: 1537
	private float breathCycleTimer;

	// Token: 0x04000602 RID: 1538
	public Sound soundBreatheIn;

	// Token: 0x04000603 RID: 1539
	public Sound soundBreatheOut;

	// Token: 0x04000604 RID: 1540
	public Sound soundFootstepWalk;

	// Token: 0x04000605 RID: 1541
	public Sound soundFootstepSprint;

	// Token: 0x02000316 RID: 790
	private enum State
	{
		// Token: 0x040028BB RID: 10427
		Roam,
		// Token: 0x040028BC RID: 10428
		PlayerNotice,
		// Token: 0x040028BD RID: 10429
		GetPlayer,
		// Token: 0x040028BE RID: 10430
		GoToTarget,
		// Token: 0x040028BF RID: 10431
		PickUpTarget,
		// Token: 0x040028C0 RID: 10432
		FindFarawayPoint,
		// Token: 0x040028C1 RID: 10433
		KidnapTarget,
		// Token: 0x040028C2 RID: 10434
		TauntTarget,
		// Token: 0x040028C3 RID: 10435
		DropTarget,
		// Token: 0x040028C4 RID: 10436
		Despawn
	}
}
