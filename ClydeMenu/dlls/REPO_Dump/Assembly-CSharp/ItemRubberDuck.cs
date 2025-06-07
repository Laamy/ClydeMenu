using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000154 RID: 340
public class ItemRubberDuck : MonoBehaviour
{
	// Token: 0x06000BA1 RID: 2977 RVA: 0x00067298 File Offset: 0x00065498
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>();
		this.hurtCollider.gameObject.SetActive(false);
		this.photonView = base.GetComponent<PhotonView>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		foreach (TrailRenderer trailRenderer in base.GetComponentsInChildren<TrailRenderer>())
		{
			this.trails.Add(trailRenderer);
		}
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00067330 File Offset: 0x00065530
	private void Update()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if ((SemiFunc.RunIsLevel() || SemiFunc.RunIsArena()) && this.rb.velocity.magnitude < 0.1f)
		{
			if (this.lilQuacksTimer > 0f)
			{
				this.lilQuacksTimer -= Time.deltaTime;
				return;
			}
			this.lilQuacksTimer = Random.Range(1f, 3f);
			this.LilQuackJump();
		}
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x000673A8 File Offset: 0x000655A8
	private void FixedUpdate()
	{
		if (this.itemEquippable.isEquipped || this.itemEquippable.wasEquippedTimer > 0f)
		{
			this.prevPosition = this.rb.position;
			return;
		}
		if (this.itemBattery.batteryLifeInt == 0)
		{
			if (!this.brokenObject.activeSelf)
			{
				this.brokenObject.SetActive(true);
				this.notBrokenObject.SetActive(false);
			}
		}
		else if (this.brokenObject.activeSelf)
		{
			this.brokenObject.SetActive(false);
			this.notBrokenObject.SetActive(true);
		}
		Vector3 vector = (this.rb.position - this.prevPosition) / Time.fixedDeltaTime;
		Vector3 normalized = (this.rb.position - this.prevPosition).normalized;
		this.prevPosition = this.rb.position;
		if (!this.physGrabObject.grabbed && this.itemBattery.batteryLife > 0f)
		{
			if (vector.magnitude > 5f)
			{
				this.playDuckLoop = true;
				this.trailTimer = 0.2f;
			}
			else
			{
				this.playDuckLoop = false;
			}
		}
		else
		{
			this.playDuckLoop = false;
		}
		if (this.trailTimer > 0f)
		{
			this.playDuckLoop = true;
			this.trailTimer -= Time.fixedDeltaTime;
			using (List<TrailRenderer>.Enumerator enumerator = this.trails.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TrailRenderer trailRenderer = enumerator.Current;
					trailRenderer.emitting = true;
				}
				goto IL_1C1;
			}
		}
		this.playDuckLoop = false;
		foreach (TrailRenderer trailRenderer2 in this.trails)
		{
			trailRenderer2.emitting = false;
		}
		IL_1C1:
		this.soundDuckLoop.PlayLoop(this.playDuckLoop, 2f, 1f, 1f);
		if (this.hurtColliderTime > 0f)
		{
			this.hurtTransform.forward = normalized;
			if (!this.hurtCollider.gameObject.activeSelf)
			{
				this.hurtCollider.gameObject.SetActive(true);
				float num = vector.magnitude * 2f;
				if (num > 50f)
				{
					num = 50f;
				}
				this.hurtCollider.physHitForce = num;
				this.hurtCollider.physHitTorque = num;
				this.hurtCollider.enemyHitForce = num;
				this.hurtCollider.enemyHitTorque = num;
				this.hurtCollider.playerTumbleForce = num;
				this.hurtCollider.playerTumbleTorque = num;
			}
			this.hurtColliderTime -= Time.fixedDeltaTime;
			return;
		}
		if (this.hurtCollider.gameObject.activeSelf)
		{
			this.hurtCollider.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x00067694 File Offset: 0x00065894
	private void LilQuackJump()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		if (this.physGrabObject.grabbed)
		{
			return;
		}
		this.rb.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
		this.rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
		this.rb.AddForce(Random.insideUnitCircle * 0.2f, ForceMode.Impulse);
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("LilQuackJumpRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.LilQuackJumpRPC();
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x0006773C File Offset: 0x0006593C
	[PunRPC]
	public void LilQuackJumpRPC()
	{
		this.soundQuack.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x00067769 File Offset: 0x00065969
	public void Squeak()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		this.soundSqueak.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x000677AC File Offset: 0x000659AC
	public void Quack()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		if (this.physGrabObject.grabbed)
		{
			return;
		}
		this.soundQuack.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.hurtColliderTime = 0.2f;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.itemBattery.batteryLife -= 2.5f;
			if (Random.Range(0, 10) == 0)
			{
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("QuackRPC", RpcTarget.All, Array.Empty<object>());
				}
				else
				{
					this.QuackRPC();
				}
			}
			if (this.rb.velocity.magnitude < 20f)
			{
				this.rb.velocity *= 5f;
				this.rb.AddTorque(Random.insideUnitSphere * 40f);
			}
		}
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x000678B4 File Offset: 0x00065AB4
	[PunRPC]
	public void QuackRPC()
	{
		this.soundDuckExplosionGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundDuckExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		ParticlePrefabExplosion particlePrefabExplosion = this.particleScriptExplosion.Spawn(base.transform.position, 0.85f, 0, 250, 1f, false, true, 1f);
		particlePrefabExplosion.SkipHurtColliderSetup = true;
		particlePrefabExplosion.HurtCollider.playerDamage = 0;
		particlePrefabExplosion.HurtCollider.enemyDamage = 250;
		particlePrefabExplosion.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
		particlePrefabExplosion.HurtCollider.physHingeDestroy = true;
		particlePrefabExplosion.HurtCollider.playerTumbleForce = 30f;
		particlePrefabExplosion.HurtCollider.playerTumbleTorque = 50f;
	}

	// Token: 0x040012E0 RID: 4832
	public Sound soundQuack;

	// Token: 0x040012E1 RID: 4833
	public Sound soundSqueak;

	// Token: 0x040012E2 RID: 4834
	public Sound soundDuckLoop;

	// Token: 0x040012E3 RID: 4835
	public Sound soundDuckExplosion;

	// Token: 0x040012E4 RID: 4836
	public Sound soundDuckExplosionGlobal;

	// Token: 0x040012E5 RID: 4837
	private Rigidbody rb;

	// Token: 0x040012E6 RID: 4838
	private PhotonView photonView;

	// Token: 0x040012E7 RID: 4839
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040012E8 RID: 4840
	private PhysGrabObject physGrabObject;

	// Token: 0x040012E9 RID: 4841
	public HurtCollider hurtCollider;

	// Token: 0x040012EA RID: 4842
	public Transform hurtTransform;

	// Token: 0x040012EB RID: 4843
	private float hurtColliderTime;

	// Token: 0x040012EC RID: 4844
	private Vector3 prevPosition;

	// Token: 0x040012ED RID: 4845
	private bool playDuckLoop;

	// Token: 0x040012EE RID: 4846
	private List<TrailRenderer> trails = new List<TrailRenderer>();

	// Token: 0x040012EF RID: 4847
	private ItemBattery itemBattery;

	// Token: 0x040012F0 RID: 4848
	private float trailTimer;

	// Token: 0x040012F1 RID: 4849
	public GameObject brokenObject;

	// Token: 0x040012F2 RID: 4850
	public GameObject notBrokenObject;

	// Token: 0x040012F3 RID: 4851
	private ItemEquippable itemEquippable;

	// Token: 0x040012F4 RID: 4852
	private float lilQuacksTimer;
}
