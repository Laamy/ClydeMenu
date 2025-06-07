using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class Cauldron : MonoBehaviour
{
	// Token: 0x06000906 RID: 2310 RVA: 0x00056E37 File Offset: 0x00055037
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (this.liquid)
		{
			this.liquidRenderer = this.liquid.GetComponent<Renderer>();
		}
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00056E64 File Offset: 0x00055064
	private void Update()
	{
		if (!this.liquid)
		{
			return;
		}
		float loopPitch = 1f;
		if (this.lightGreen.gameObject.activeSelf)
		{
			loopPitch = 1f + this.lightGreen.intensity / 8f * 5f;
		}
		this.soundLoop.LoopPitch = loopPitch;
		this.soundLoop.PlayLoop(this.cauldronActive, 1f, 1f, 1f);
		if (this.cauldronActive && this.lightGreen.gameObject.activeSelf)
		{
			float num = 1f + this.lightGreen.intensity * 20f;
			GameDirector.instance.CameraShake.ShakeDistance(num / 30f, 0f, 3f, this.liquid.position, 0.5f);
		}
		if ((!SemiFunc.IsMultiplayer() && this.explosionTimer <= 0f) || (SemiFunc.IsMasterClient() && this.explosionTimer <= 0f))
		{
			this.checkTimer += Time.deltaTime;
			if (this.checkTimer > 1f)
			{
				bool flag = this.cauldronActive;
				this.cauldronActive = false;
				foreach (Collider collider in Physics.OverlapSphere(this.sphereChecker.position, this.sphereChecker.localScale.x * 0.5f, SemiFunc.LayerMaskGetPlayersAndPhysObjects(), QueryTriggerInteraction.Ignore))
				{
					if (collider)
					{
						if (collider.GetComponentInParent<PhysGrabObject>())
						{
							this.cauldronActive = true;
							break;
						}
						if (collider.GetComponentInParent<PlayerAvatar>())
						{
							this.cauldronActive = true;
							break;
						}
						if (collider.GetComponentInParent<PlayerController>())
						{
							this.cauldronActive = true;
							break;
						}
					}
				}
				if (this.cauldronActive && !flag)
				{
					this.CookStart();
				}
				if (!this.cauldronActive && flag)
				{
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("EndCookRPC", RpcTarget.All, Array.Empty<object>());
					}
					else
					{
						this.EndCookRPC();
					}
				}
				this.checkTimer = 0f;
			}
		}
		if (this.explosionTimer > 0f)
		{
			this.explosionTimer -= Time.deltaTime;
			if (this.explosionTimer <= 0f && this.explosion)
			{
				this.explosion.gameObject.SetActive(false);
			}
		}
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				if (this.hurtCollider)
				{
					this.hurtCollider.SetActive(false);
				}
			}
			else if (this.hurtCollider)
			{
				this.hurtCollider.SetActive(true);
			}
		}
		if (this.cauldronActive)
		{
			if (!this.sparkParticles.isPlaying)
			{
				this.sparkParticles.Play();
			}
			if (!this.windParticles.isPlaying)
			{
				this.windParticles.Play();
			}
			if (!this.lightGreen.gameObject.activeSelf)
			{
				this.lightGreen.gameObject.SetActive(true);
				this.lightGreen.intensity = 0f;
				return;
			}
			this.lightGreen.intensity += Time.deltaTime * 2f;
			this.lightGreen.intensity = Mathf.Clamp(this.lightGreen.intensity, 0f, 8f);
			float num2 = Mathf.Abs(this.lightGreen.intensity / 8f);
			this.lightGreen.range = 4f + 1f * Mathf.Sin(Time.time * 10f) * num2;
			if (this.lightGreen.intensity > 7.5f)
			{
				this.safetyTimer += Time.deltaTime;
				if (this.safetyTimer > 3f)
				{
					this.EndCook();
				}
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					this.Explosion();
					return;
				}
			}
		}
		else
		{
			if (this.lightGreen.gameObject.activeSelf)
			{
				this.lightGreen.intensity -= Time.deltaTime * 20f;
				this.lightGreen.intensity = Mathf.Clamp(this.lightGreen.intensity, 0f, 8f);
				if (this.lightGreen.intensity < 0.1f)
				{
					this.lightGreen.gameObject.SetActive(false);
				}
			}
			this.sparkParticles.Stop();
			this.windParticles.Stop();
		}
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00057302 File Offset: 0x00055502
	private void Explosion()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ExplosionRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.ExplosionRPC();
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00057328 File Offset: 0x00055528
	[PunRPC]
	public void EndCookRPC()
	{
		this.EndCook();
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00057330 File Offset: 0x00055530
	private void EndCook()
	{
		this.cauldronActive = false;
		this.safetyTimer = 0f;
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00057344 File Offset: 0x00055544
	[PunRPC]
	public void ExplosionRPC()
	{
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.liquid.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, this.liquid.position, 0.1f);
		this.soundExplosion.Play(this.liquid.position, 1f, 1f, 1f, 1f);
		this.explosion.gameObject.SetActive(true);
		this.hurtCollider.gameObject.SetActive(true);
		this.explosionTimer = 3f;
		this.hurtColliderTimer = 0.5f;
		this.EndCook();
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00057416 File Offset: 0x00055616
	private void CookStart()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("CookStartRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.CookStartRPC();
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x0005743C File Offset: 0x0005563C
	[PunRPC]
	public void CookStartRPC()
	{
		this.cauldronActive = true;
	}

	// Token: 0x04001086 RID: 4230
	public Transform sphereChecker;

	// Token: 0x04001087 RID: 4231
	public ParticleSystem sparkParticles;

	// Token: 0x04001088 RID: 4232
	public ParticleSystem windParticles;

	// Token: 0x04001089 RID: 4233
	public Light lightGreen;

	// Token: 0x0400108A RID: 4234
	public GameObject explosion;

	// Token: 0x0400108B RID: 4235
	public GameObject hurtCollider;

	// Token: 0x0400108C RID: 4236
	private float checkTimer;

	// Token: 0x0400108D RID: 4237
	private bool cauldronActive;

	// Token: 0x0400108E RID: 4238
	private float explosionTimer;

	// Token: 0x0400108F RID: 4239
	private float hurtColliderTimer;

	// Token: 0x04001090 RID: 4240
	private PhotonView photonView;

	// Token: 0x04001091 RID: 4241
	public Transform liquid;

	// Token: 0x04001092 RID: 4242
	private Renderer liquidRenderer;

	// Token: 0x04001093 RID: 4243
	public Sound soundExplosion;

	// Token: 0x04001094 RID: 4244
	public Sound soundLoop;

	// Token: 0x04001095 RID: 4245
	private float safetyTimer;
}
