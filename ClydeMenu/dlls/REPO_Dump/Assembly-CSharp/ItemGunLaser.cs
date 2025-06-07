using System;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class ItemGunLaser : MonoBehaviour
{
	// Token: 0x06000CA2 RID: 3234 RVA: 0x0006FAE0 File Offset: 0x0006DCE0
	private void Start()
	{
		this.itemGun = base.GetComponent<ItemGun>();
		this.materialGunEnergyPlop = this.transformGunEnergyPlop.GetComponent<Renderer>().material;
		this.materialGunEnergyPlop.SetColor("_EmissionColor", Color.black);
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0006FB30 File Offset: 0x0006DD30
	private void FixedUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemGun.stateCurrent == ItemGun.State.Shooting)
		{
			Vector3 force = -this.muzzleTransform.forward * 10f;
			this.physGrabObject.rb.AddForce(force, ForceMode.Force);
			Vector3 torque = -this.muzzleTransform.right * 0.6f;
			this.physGrabObject.rb.AddTorque(torque, ForceMode.Force);
		}
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
	private void Update()
	{
		if (this.shootImpulseTimer > 0f)
		{
			if (!this.shooting)
			{
				this.muzzleFlashParticle.Play(true);
				this.shooting = true;
			}
			this.shootImpulseTimer -= Time.deltaTime;
		}
		else if (this.shooting)
		{
			this.muzzleFlashParticle.Stop(true);
			this.shooting = false;
		}
		if (this.buildupImpulseTimer > 0f)
		{
			this.buildupImpulseTimer -= Time.deltaTime;
		}
		bool playing = this.buildupImpulseTimer > 0f;
		float pitchMultiplier = Mathf.Lerp(3f, 1f, 1f - this.itemGun.stateTimer / this.itemGun.stateTimeMax);
		this.soundBuildup.PlayLoop(playing, 1f, 1f, pitchMultiplier);
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0006FC88 File Offset: 0x0006DE88
	public void OnStateShootStart()
	{
		this.soundInitialCrack.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.particleInitialCrack.Play(true);
		GameDirector.instance.CameraImpact.ShakeDistance(7f, 6f, 12f, base.transform.position, 0.1f);
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0006FCFA File Offset: 0x0006DEFA
	public void OnStateShootUpdate()
	{
		this.LaserShooting();
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0006FD04 File Offset: 0x0006DF04
	public void OnStateReloadStart()
	{
		this.soundReload.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.particleBuildUp.Stop(true);
		this.particleOverHeat.Play(true);
		this.soundReload.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundReload2Played = false;
		this.transformGunEnergyPlop.localPosition = new Vector3(0f, 0f, -0.6f);
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x0006FDA8 File Offset: 0x0006DFA8
	public void OnStateReloadUpdate()
	{
		float a = -0.6f;
		float time = this.itemGun.stateTimer / (this.itemGun.stateTimeMax / 2f);
		float num = this.buildupImpulseCurve.Evaluate(time);
		if (num > 1f)
		{
			num = 1f;
		}
		this.transformGunEnergyPlop.localPosition = new Vector3(0f, 0f, Mathf.Lerp(a, 0f, num));
		Color value = Color.Lerp(Color.yellow, Color.black, num);
		this.materialGunEnergyPlop.SetColor("_EmissionColor", value);
		time = this.itemGun.stateTimer / this.itemGun.stateTimeMax;
		num = this.backLatchCurve.Evaluate(time);
		float num2 = 130f * num;
		this.transformBackPlingPlong.localRotation = Quaternion.Euler(-num2, 0f, 0f);
		time = this.itemGun.stateTimer / this.itemGun.stateTimeMax;
		num = this.heatLatchCurve.Evaluate(time);
		float num3 = 84f * num;
		this.transformOverHeatLatch.localRotation = Quaternion.Euler(-num3, 0f, 0f);
		if (!this.soundReload2Played && this.itemGun.stateTimer > this.itemGun.stateTimeMax * 0.8f)
		{
			this.soundReload2.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundReload2Played = true;
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x0006FF2C File Offset: 0x0006E12C
	public void OnStateIdleStart()
	{
		this.particleBuildUp.Stop(true);
		this.transformGunEnergyPlop.localPosition = new Vector3(0f, 0f, 0f);
		this.materialGunEnergyPlop.SetColor("_EmissionColor", Color.black);
		this.transformOverHeatLatch.localRotation = Quaternion.Euler(0f, 0f, 0f);
		this.transformBackPlingPlong.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0006FFB7 File Offset: 0x0006E1B7
	public void OnStateBuildStart()
	{
		this.particleBuildUp.Play(true);
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0006FFC8 File Offset: 0x0006E1C8
	public void OnStateBuildUpdate()
	{
		this.buildupImpulseTimer = 0.1f;
		float b = -0.6f;
		float time = this.itemGun.stateTimer / this.itemGun.stateTimeMax;
		float num = this.buildupImpulseCurve.Evaluate(time);
		if (num > 1f)
		{
			num = 1f;
		}
		this.transformGunEnergyPlop.localPosition = new Vector3(0f, 0f, Mathf.Lerp(0f, b, num));
		Color value = Color.Lerp(Color.black, Color.yellow, num);
		this.materialGunEnergyPlop.SetColor("_EmissionColor", value);
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x00070064 File Offset: 0x0006E264
	private void LaserShooting()
	{
		float gunRange = this.itemGun.gunRange;
		Vector3 endPosition = this.muzzleTransform.position + this.muzzleTransform.forward * gunRange;
		bool isHitting = false;
		RaycastHit raycastHit;
		if (Physics.Raycast(this.muzzleTransform.position, this.muzzleTransform.forward, out raycastHit, gunRange, SemiFunc.LayerMaskGetVisionObstruct()))
		{
			endPosition = raycastHit.point;
			isHitting = true;
		}
		this.semiLaser.LaserActive(this.muzzleTransform.position, endPosition, isHitting);
		this.shootImpulseTimer = 0.1f;
	}

	// Token: 0x04001456 RID: 5206
	public SemiLaser semiLaser;

	// Token: 0x04001457 RID: 5207
	public Transform muzzleTransform;

	// Token: 0x04001458 RID: 5208
	private float shootImpulseTimer;

	// Token: 0x04001459 RID: 5209
	private bool shooting;

	// Token: 0x0400145A RID: 5210
	public ParticleSystem muzzleFlashParticle;

	// Token: 0x0400145B RID: 5211
	private ItemGun itemGun;

	// Token: 0x0400145C RID: 5212
	public Sound soundInitialCrack;

	// Token: 0x0400145D RID: 5213
	public Sound soundBuildup;

	// Token: 0x0400145E RID: 5214
	public Sound soundReload;

	// Token: 0x0400145F RID: 5215
	public Sound soundReload2;

	// Token: 0x04001460 RID: 5216
	private float buildupImpulseTimer;

	// Token: 0x04001461 RID: 5217
	public ParticleSystem particleOverHeat;

	// Token: 0x04001462 RID: 5218
	public AnimationCurve buildupImpulseCurve;

	// Token: 0x04001463 RID: 5219
	public AnimationCurve heatLatchCurve;

	// Token: 0x04001464 RID: 5220
	public AnimationCurve backLatchCurve;

	// Token: 0x04001465 RID: 5221
	public Transform transformGunEnergyPlop;

	// Token: 0x04001466 RID: 5222
	public Transform transformOverHeatLatch;

	// Token: 0x04001467 RID: 5223
	public Transform transformBackPlingPlong;

	// Token: 0x04001468 RID: 5224
	private Material materialGunEnergyPlop;

	// Token: 0x04001469 RID: 5225
	public ParticleSystem particleBuildUp;

	// Token: 0x0400146A RID: 5226
	public ParticleSystem particleInitialCrack;

	// Token: 0x0400146B RID: 5227
	private bool soundReload2Played;

	// Token: 0x0400146C RID: 5228
	private PhysGrabObject physGrabObject;
}
