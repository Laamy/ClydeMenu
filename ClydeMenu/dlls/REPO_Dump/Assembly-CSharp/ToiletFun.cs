using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class ToiletFun : MonoBehaviour
{
	// Token: 0x060008F9 RID: 2297 RVA: 0x00056566 File Offset: 0x00054766
	private void Start()
	{
		this.sphereChecker = base.GetComponentInChildren<SphereCollider>().transform;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00056588 File Offset: 0x00054788
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			bool flag = false;
			foreach (PhysGrabObject physGrabObject in this.physGrabObjects)
			{
				if (physGrabObject)
				{
					if (this.toiletActive)
					{
						Rigidbody component = physGrabObject.GetComponent<Rigidbody>();
						if (component)
						{
							component.AddTorque(Vector3.up * this.toiletCharge, ForceMode.Impulse);
							if (this.randomForceTimer <= 0f)
							{
								Vector3 a = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
								component.AddForce(a * this.toiletCharge, ForceMode.Impulse);
							}
						}
					}
					float num = Vector3.Distance(physGrabObject.midPoint, this.sphereChecker.position);
					if ((physGrabObject.impactHappenedTimer > 0f || physGrabObject.impactLightTimer > 0f || physGrabObject.impactHeavyTimer > 0f || physGrabObject.impactMediumTimer > 0f) && num < this.sphereChecker.localScale.x)
					{
						flag = true;
					}
				}
			}
			foreach (PlayerAvatar playerAvatar in this.playerAvatars)
			{
				if (playerAvatar && this.toiletActive)
				{
					playerAvatar.tumble.TumbleRequest(true, false);
					playerAvatar.tumble.TumbleOverrideTime(10f);
				}
			}
			if (this.playerController && this.toiletActive)
			{
				this.playerController.playerAvatarScript.tumble.TumbleRequest(true, false);
				this.playerController.playerAvatarScript.tumble.TumbleOverrideTime(10f);
			}
			if (this.splashTimer <= 0f && flag)
			{
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("SplashRPC", RpcTarget.All, Array.Empty<object>());
				}
				else
				{
					this.Splash();
				}
				this.splashTimer = 0.5f;
			}
			if (this.toiletActive)
			{
				if (this.hingeRattlingTimer <= 0f)
				{
					if (this.hingeRigidBody && !this.hingeRigidBody.GetComponent<PhysGrabHinge>().broken)
					{
						this.hingeRigidBody.AddForce(-Vector3.up * 2f * this.toiletCharge * 0.5f, ForceMode.Impulse);
					}
					this.hingeRattlingTimer = 0.1f;
					return;
				}
				this.hingeRattlingTimer -= Time.deltaTime;
			}
		}
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x0005686C File Offset: 0x00054A6C
	private void Update()
	{
		float loopPitch = 1f + this.toiletCharge / 8f * 2f;
		this.soundLoop.LoopPitch = loopPitch;
		this.soundLoop.PlayLoop(this.toiletActive, 1f, 1f, 1f);
		if (this.toiletActive)
		{
			float num = 1f + this.toiletCharge * 20f;
			GameDirector.instance.CameraShake.ShakeDistance(num / 30f, 0f, 3f, base.transform.position, 0.5f);
		}
		if ((!SemiFunc.IsMultiplayer() && this.explosionTimer <= 0f) || (SemiFunc.IsMasterClientOrSingleplayer() && this.explosionTimer <= 0f))
		{
			this.checkTimer += Time.deltaTime;
			if (this.checkTimer > 1f)
			{
				this.physGrabObjects.Clear();
				this.playerAvatars.Clear();
				this.playerController = null;
				foreach (Collider collider in Physics.OverlapSphere(this.sphereChecker.position, this.sphereChecker.localScale.x * 0.5f, SemiFunc.LayerMaskGetPlayersAndPhysObjects(), QueryTriggerInteraction.Ignore))
				{
					if (collider)
					{
						PhysGrabObject componentInParent = collider.GetComponentInParent<PhysGrabObject>();
						if (componentInParent)
						{
							this.physGrabObjects.Add(componentInParent);
							break;
						}
						if (collider.GetComponentInParent<PlayerAvatar>())
						{
							this.playerAvatars.Add(collider.GetComponentInParent<PlayerAvatar>());
							break;
						}
						if (collider.GetComponentInParent<PlayerController>())
						{
							this.playerController = collider.GetComponentInParent<PlayerController>();
							break;
						}
					}
				}
				this.checkTimer = 0f;
			}
		}
		if (this.splashTimer > 0f)
		{
			this.splashTimer -= Time.deltaTime;
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
		if (this.toiletActive)
		{
			if (this.randomForceTimer > 0f)
			{
				this.randomForceTimer -= Time.deltaTime;
			}
			else
			{
				this.randomForceTimer = Random.Range(0.5f, 2f);
			}
			if (!this.smallParticles.isPlaying)
			{
				this.smallParticles.Play();
			}
			if (!this.bigParticles.isPlaying)
			{
				this.bigParticles.Play();
			}
			this.toiletCharge += Time.deltaTime * 2f;
			this.toiletCharge = Mathf.Clamp(this.toiletCharge, 0f, 8f);
			if (this.toiletCharge > 7.5f)
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
			this.toiletCharge -= Time.deltaTime * 20f;
			this.toiletCharge = Mathf.Clamp(this.toiletCharge, 0f, 8f);
			this.smallParticles.Stop();
			this.bigParticles.Stop();
		}
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00056C31 File Offset: 0x00054E31
	private void Explosion()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ExplosionRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.ExplosionRPC();
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00056C57 File Offset: 0x00054E57
	[PunRPC]
	public void EndCookRPC()
	{
		this.EndCook();
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00056C5F File Offset: 0x00054E5F
	private void EndCook()
	{
		this.toiletActive = false;
		this.safetyTimer = 0f;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00056C74 File Offset: 0x00054E74
	[PunRPC]
	public void ExplosionRPC()
	{
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, base.transform.position, 0.1f);
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.explosion.gameObject.SetActive(true);
		this.hurtCollider.gameObject.SetActive(true);
		this.explosionTimer = 3f;
		this.hurtColliderTimer = 0.5f;
		this.EndCook();
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00056D46 File Offset: 0x00054F46
	private void FlushStart()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("FlushStartRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.FlushStartRPC();
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00056D6C File Offset: 0x00054F6C
	[PunRPC]
	public void FlushStartRPC()
	{
		this.soundFlush.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.toiletActive = true;
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00056DA0 File Offset: 0x00054FA0
	public void Flush()
	{
		if (!this.toiletActive && SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.FlushStart();
		}
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00056DB7 File Offset: 0x00054FB7
	[PunRPC]
	public void SplashRPC()
	{
		this.Splash();
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00056DC0 File Offset: 0x00054FC0
	private void Splash()
	{
		this.splashBigParticles.Play();
		this.splashSmallParticles.Play();
		this.soundSplash.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.splashTimer = 1f;
	}

	// Token: 0x0400106D RID: 4205
	internal Transform sphereChecker;

	// Token: 0x0400106E RID: 4206
	internal List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	// Token: 0x0400106F RID: 4207
	internal List<PlayerAvatar> playerAvatars = new List<PlayerAvatar>();

	// Token: 0x04001070 RID: 4208
	private PlayerController playerController;

	// Token: 0x04001071 RID: 4209
	internal float checkTimer;

	// Token: 0x04001072 RID: 4210
	private PhotonView photonView;

	// Token: 0x04001073 RID: 4211
	private float toiletCharge;

	// Token: 0x04001074 RID: 4212
	private float explosionTimer;

	// Token: 0x04001075 RID: 4213
	private float hurtColliderTimer;

	// Token: 0x04001076 RID: 4214
	private bool toiletActive;

	// Token: 0x04001077 RID: 4215
	public GameObject hurtCollider;

	// Token: 0x04001078 RID: 4216
	public Transform explosion;

	// Token: 0x04001079 RID: 4217
	public Sound soundLoop;

	// Token: 0x0400107A RID: 4218
	public Sound soundExplosion;

	// Token: 0x0400107B RID: 4219
	public Sound soundFlush;

	// Token: 0x0400107C RID: 4220
	public Sound soundSplash;

	// Token: 0x0400107D RID: 4221
	private float safetyTimer;

	// Token: 0x0400107E RID: 4222
	public ParticleSystem smallParticles;

	// Token: 0x0400107F RID: 4223
	public ParticleSystem bigParticles;

	// Token: 0x04001080 RID: 4224
	public ParticleSystem splashBigParticles;

	// Token: 0x04001081 RID: 4225
	public ParticleSystem splashSmallParticles;

	// Token: 0x04001082 RID: 4226
	private float splashTimer;

	// Token: 0x04001083 RID: 4227
	private float randomForceTimer;

	// Token: 0x04001084 RID: 4228
	public Rigidbody hingeRigidBody;

	// Token: 0x04001085 RID: 4229
	private float hingeRattlingTimer;
}
