using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009E RID: 158
public class EnemyHealth : MonoBehaviour
{
	// Token: 0x0600063F RID: 1599 RVA: 0x0003D138 File Offset: 0x0003B338
	private void Awake()
	{
		this.enemy = base.GetComponent<Enemy>();
		this.photonView = base.GetComponent<PhotonView>();
		this.healthCurrent = this.health;
		this.hurtCurve = AssetManager.instance.animationCurveImpact;
		this.renderers = new List<MeshRenderer>();
		if (this.meshParent)
		{
			this.renderers.AddRange(this.meshParent.GetComponentsInChildren<MeshRenderer>(true));
		}
		foreach (MeshRenderer meshRenderer in this.renderers)
		{
			Material material = null;
			foreach (Material material2 in this.sharedMaterials)
			{
				if (meshRenderer.sharedMaterial.name == material2.name)
				{
					material = material2;
					meshRenderer.sharedMaterial = this.instancedMaterials[this.sharedMaterials.IndexOf(material2)];
				}
			}
			if (!material)
			{
				material = meshRenderer.sharedMaterial;
				this.sharedMaterials.Add(material);
				this.instancedMaterials.Add(meshRenderer.material);
			}
		}
		this.materialHurtColor = Shader.PropertyToID("_ColorOverlay");
		this.materialHurtAmount = Shader.PropertyToID("_ColorOverlayAmount");
		foreach (Material material3 in this.instancedMaterials)
		{
			material3.SetColor(this.materialHurtColor, Color.red);
		}
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0003D2FC File Offset: 0x0003B4FC
	private void Update()
	{
		if (this.hurtEffect)
		{
			this.hurtLerp += 2.5f * Time.deltaTime;
			this.hurtLerp = Mathf.Clamp01(this.hurtLerp);
			foreach (Material material in this.instancedMaterials)
			{
				material.SetFloat(this.materialHurtAmount, this.hurtCurve.Evaluate(this.hurtLerp));
			}
			if (this.hurtLerp > 1f)
			{
				this.hurtEffect = false;
				foreach (Material material2 in this.instancedMaterials)
				{
					material2.SetFloat(this.materialHurtAmount, 0f);
				}
			}
		}
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && this.deadImpulse)
		{
			this.deadImpulseTimer -= Time.deltaTime;
			if (this.deadImpulseTimer <= 0f)
			{
				if (!GameManager.Multiplayer())
				{
					this.DeathImpulseRPC(default(PhotonMessageInfo));
				}
				else
				{
					this.photonView.RPC("DeathImpulseRPC", RpcTarget.All, Array.Empty<object>());
				}
			}
		}
		if (this.objectHurtDisableTimer > 0f)
		{
			this.objectHurtDisableTimer -= Time.deltaTime;
		}
		if (this.onHurtImpulse)
		{
			this.onHurt.Invoke();
			this.onHurtImpulse = false;
		}
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x0003D494 File Offset: 0x0003B694
	public void OnSpawn()
	{
		if (this.hurtEffect)
		{
			this.hurtLerp = 1f;
			this.hurtEffect = false;
			foreach (Material material in this.instancedMaterials)
			{
				material.SetFloat(this.materialHurtAmount, 0f);
			}
		}
		this.healthCurrent = this.health;
		this.dead = false;
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x0003D51C File Offset: 0x0003B71C
	public void LightImpact()
	{
		if (this.impactHurt)
		{
			if (!this.enemy.IsStunned())
			{
				return;
			}
			if (this.impactLightDamage > 0)
			{
				this.Hurt(this.impactLightDamage, -this.enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw.normalized);
			}
		}
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0003D574 File Offset: 0x0003B774
	public void MediumImpact()
	{
		if (this.impactHurt)
		{
			if (!this.enemy.IsStunned())
			{
				return;
			}
			if (this.impactMediumDamage > 0)
			{
				this.Hurt(this.impactMediumDamage, -this.enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw.normalized);
			}
		}
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x0003D5CC File Offset: 0x0003B7CC
	public void HeavyImpact()
	{
		if (this.impactHurt)
		{
			if (!this.enemy.IsStunned())
			{
				return;
			}
			if (this.impactHeavyDamage > 0)
			{
				this.Hurt(this.impactHeavyDamage, -this.enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw.normalized);
			}
		}
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x0003D624 File Offset: 0x0003B824
	public void Hurt(int _damage, Vector3 _hurtDirection)
	{
		if (this.dead)
		{
			return;
		}
		this.healthCurrent -= _damage;
		if (this.healthCurrent <= 0)
		{
			this.healthCurrent = 0;
			this.Death(_hurtDirection);
			return;
		}
		if (!GameManager.Multiplayer())
		{
			this.HurtRPC(_damage, _hurtDirection);
			return;
		}
		this.photonView.RPC("HurtRPC", RpcTarget.All, new object[]
		{
			_damage,
			_hurtDirection
		});
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0003D699 File Offset: 0x0003B899
	[PunRPC]
	public void HurtRPC(int _damage, Vector3 _hurtDirection)
	{
		this.hurtDirection = _hurtDirection;
		this.hurtEffect = true;
		this.hurtLerp = 0f;
		if (this.hurtDirection == Vector3.zero)
		{
			this.hurtDirection = Random.insideUnitSphere;
		}
		this.onHurtImpulse = true;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0003D6D8 File Offset: 0x0003B8D8
	private void Death(Vector3 _deathDirection)
	{
		if (!GameManager.Multiplayer())
		{
			this.DeathRPC(_deathDirection, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("DeathRPC", RpcTarget.All, new object[]
		{
			_deathDirection
		});
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x0003D720 File Offset: 0x0003B920
	[PunRPC]
	public void DeathRPC(Vector3 _deathDirection, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.hurtDirection = _deathDirection;
		this.hurtEffect = true;
		this.hurtLerp = 0f;
		this.deadImpulseTimer = this.deathFreezeTime;
		this.enemy.Freeze(this.deathFreezeTime);
		this.onDeathStart.Invoke();
		this.deadImpulse = true;
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0003D780 File Offset: 0x0003B980
	[PunRPC]
	public void DeathImpulseRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.deadImpulse = false;
		this.dead = true;
		if (this.hurtDirection == Vector3.zero)
		{
			this.hurtDirection = Random.insideUnitSphere;
		}
		this.onDeath.Invoke();
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0003D7CC File Offset: 0x0003B9CC
	public void ObjectHurtDisable(float _time)
	{
		this.objectHurtDisableTimer = _time;
	}

	// Token: 0x04000A4B RID: 2635
	private PhotonView photonView;

	// Token: 0x04000A4C RID: 2636
	private Enemy enemy;

	// Token: 0x04000A4D RID: 2637
	public int health = 100;

	// Token: 0x04000A4E RID: 2638
	internal int healthCurrent;

	// Token: 0x04000A4F RID: 2639
	private bool deadImpulse;

	// Token: 0x04000A50 RID: 2640
	internal bool dead;

	// Token: 0x04000A51 RID: 2641
	private float deadImpulseTimer;

	// Token: 0x04000A52 RID: 2642
	public float deathFreezeTime = 0.1f;

	// Token: 0x04000A53 RID: 2643
	public bool impactHurt;

	// Token: 0x04000A54 RID: 2644
	public int impactLightDamage;

	// Token: 0x04000A55 RID: 2645
	public int impactMediumDamage;

	// Token: 0x04000A56 RID: 2646
	public int impactHeavyDamage;

	// Token: 0x04000A57 RID: 2647
	public bool objectHurt;

	// Token: 0x04000A58 RID: 2648
	public float objectHurtMultiplier = 1f;

	// Token: 0x04000A59 RID: 2649
	public bool objectHurtStun = true;

	// Token: 0x04000A5A RID: 2650
	internal float objectHurtStunTime = 2f;

	// Token: 0x04000A5B RID: 2651
	public Transform meshParent;

	// Token: 0x04000A5C RID: 2652
	private List<MeshRenderer> renderers;

	// Token: 0x04000A5D RID: 2653
	private List<Material> sharedMaterials = new List<Material>();

	// Token: 0x04000A5E RID: 2654
	internal List<Material> instancedMaterials = new List<Material>();

	// Token: 0x04000A5F RID: 2655
	public bool spawnValuable = true;

	// Token: 0x04000A60 RID: 2656
	public int spawnValuableMax = 3;

	// Token: 0x04000A61 RID: 2657
	internal int spawnValuableCurrent;

	// Token: 0x04000A62 RID: 2658
	internal Vector3 hurtDirection;

	// Token: 0x04000A63 RID: 2659
	private bool hurtEffect;

	// Token: 0x04000A64 RID: 2660
	private AnimationCurve hurtCurve;

	// Token: 0x04000A65 RID: 2661
	private float hurtLerp;

	// Token: 0x04000A66 RID: 2662
	public UnityEvent onHurt;

	// Token: 0x04000A67 RID: 2663
	private bool onHurtImpulse;

	// Token: 0x04000A68 RID: 2664
	public UnityEvent onDeathStart;

	// Token: 0x04000A69 RID: 2665
	public UnityEvent onDeath;

	// Token: 0x04000A6A RID: 2666
	public UnityEvent onObjectHurt;

	// Token: 0x04000A6B RID: 2667
	internal PlayerAvatar onObjectHurtPlayer;

	// Token: 0x04000A6C RID: 2668
	private int materialHurtColor;

	// Token: 0x04000A6D RID: 2669
	private int materialHurtAmount;

	// Token: 0x04000A6E RID: 2670
	internal float objectHurtDisableTimer;
}
