using System;
using UnityEngine;

// Token: 0x0200029E RID: 670
public class AnimalCrateTrap : Trap
{
	// Token: 0x060014E9 RID: 5353 RVA: 0x000B92C0 File Offset: 0x000B74C0
	protected override void Start()
	{
		base.Start();
		this.initialAnimalCrateRotation = this.Crate.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.material = this.Crate.GetComponent<Renderer>().material;
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x000B9323 File Offset: 0x000B7523
	protected override void Update()
	{
		base.Update();
		this.berzerk.PlayLoop(this.poundActive, 1f, 1f, 1f);
		if (this.physgrabobject.grabbed)
		{
			this.TrapActivate();
		}
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x000B9360 File Offset: 0x000B7560
	private void FixedUpdate()
	{
		if (this.poundActive)
		{
			if (this.shakeFrequencyMultiplier > 5f)
			{
				this.material.EnableKeyword("_EMISSION");
			}
			GameDirector.instance.CameraImpact.ShakeDistance(0.75f * this.poundIntensityMuilplier, 1f, 6f, base.transform.position, 0.01f * this.poundIntensityMuilplier);
			float num = this.shakeAmplitudeMultiplier * (1f - this.poundLerp);
			float num2 = this.shakeFrequencyMultiplier * (1f - this.poundLerp);
			float x = num * Mathf.Sin(Time.time * num2);
			float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
			this.Crate.transform.localRotation = this.initialAnimalCrateRotation * Quaternion.Euler(x, 0f, z);
			this.poundLerp += this.poundSpeed / this.poundIntensityMuilplier * Time.deltaTime;
			if (this.poundLerp >= 1f)
			{
				this.poundLerp = 0f;
				this.material.DisableKeyword("_EMISSION");
				this.poundActive = false;
			}
			this.Crate.transform.localScale = new Vector3(1f + this.poundCurve.Evaluate(this.poundLerp) * (this.poundIntensity * this.poundIntensityMuilplier), 1f + this.poundCurve.Evaluate(this.poundLerp) * (this.poundIntensity * this.poundIntensityMuilplier), 1f + this.poundCurve.Evaluate(this.poundLerp) * (this.poundIntensity * this.poundIntensityMuilplier));
		}
		if (this.trapActive && Vector3.Dot(base.transform.up, Vector3.up) < 0.5f)
		{
			if (this.timeToPound > 0f)
			{
				this.timeToPound -= Time.deltaTime;
				return;
			}
			this.poundActive = true;
			this.BigBump();
			this.timeToPound = 1f;
			this.physgrabobject.lightBreakImpulse = true;
		}
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x000B9580 File Offset: 0x000B7780
	public void TinyBump()
	{
		this.particleBitsTiny.Play();
		this.tinyBump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.shakeAmplitudeMultiplier = 0.2f;
		this.shakeFrequencyMultiplier = 5f;
		this.poundIntensityMuilplier = 1.5f;
		this.enemyInvestigate = true;
		this.poundActive = true;
		if (this.isLocal)
		{
			float d = Random.Range(0.05f, 0.2f);
			this.rb.AddForce(Vector3.up * d, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 2f, ForceMode.Impulse);
		}
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x000B9648 File Offset: 0x000B7848
	public void SmallBump()
	{
		this.particleBitsSmall.Play();
		this.smallBump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.shakeAmplitudeMultiplier = 0.5f;
		this.shakeFrequencyMultiplier = 10f;
		this.poundIntensityMuilplier = 2f;
		this.enemyInvestigate = true;
		this.poundActive = true;
		if (this.isLocal)
		{
			float d = Random.Range(0.1f, 0.5f);
			this.rb.AddForce(Vector3.up * d, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 3f, ForceMode.Impulse);
		}
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x000B9710 File Offset: 0x000B7910
	public void MediumBump()
	{
		this.particleBitsMedium.Play();
		this.mediumBump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.shakeAmplitudeMultiplier = 1f;
		this.shakeFrequencyMultiplier = 20f;
		this.poundIntensityMuilplier = 3f;
		this.enemyInvestigate = true;
		this.poundActive = true;
		if (this.isLocal)
		{
			float d = Random.Range(0.3f, 1f);
			this.rb.AddForce(Vector3.up * d, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 5f, ForceMode.Impulse);
		}
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x000B97D8 File Offset: 0x000B79D8
	public void BigBump()
	{
		this.particleBitsBig.Play();
		this.bigBump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.shakeAmplitudeMultiplier = 2f;
		this.shakeFrequencyMultiplier = 20f;
		this.poundIntensityMuilplier = 4f;
		this.enemyInvestigate = true;
		this.poundActive = true;
		if (this.isLocal)
		{
			float d = Random.Range(0.8f, 2f);
			this.rb.AddForce(Vector3.up * d, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 6f, ForceMode.Impulse);
		}
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x000B989E File Offset: 0x000B7A9E
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x000B98B8 File Offset: 0x000B7AB8
	public void TrapStop()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 1f, 50, 300, 1f, false, false, 1f);
	}

	// Token: 0x0400242A RID: 9258
	private PhysGrabObject physgrabobject;

	// Token: 0x0400242B RID: 9259
	[Space]
	[Header("Animal Crate Components")]
	public GameObject Crate;

	// Token: 0x0400242C RID: 9260
	public Transform Center;

	// Token: 0x0400242D RID: 9261
	private Material material;

	// Token: 0x0400242E RID: 9262
	[Space]
	[Header("Sounds")]
	public Sound tinyBump;

	// Token: 0x0400242F RID: 9263
	public Sound smallBump;

	// Token: 0x04002430 RID: 9264
	public Sound mediumBump;

	// Token: 0x04002431 RID: 9265
	public Sound bigBump;

	// Token: 0x04002432 RID: 9266
	public Sound berzerk;

	// Token: 0x04002433 RID: 9267
	[Space]
	private Quaternion initialAnimalCrateRotation;

	// Token: 0x04002434 RID: 9268
	private Rigidbody rb;

	// Token: 0x04002435 RID: 9269
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002436 RID: 9270
	public ParticleSystem particleBitsTiny;

	// Token: 0x04002437 RID: 9271
	public ParticleSystem particleBitsSmall;

	// Token: 0x04002438 RID: 9272
	public ParticleSystem particleBitsMedium;

	// Token: 0x04002439 RID: 9273
	public ParticleSystem particleBitsBig;

	// Token: 0x0400243A RID: 9274
	private Vector3 randomTorque;

	// Token: 0x0400243B RID: 9275
	private float timeToPound = 1f;

	// Token: 0x0400243C RID: 9276
	[Space]
	[Header("Pound Animation")]
	public AnimationCurve poundCurve;

	// Token: 0x0400243D RID: 9277
	private bool poundActive;

	// Token: 0x0400243E RID: 9278
	public float poundSpeed;

	// Token: 0x0400243F RID: 9279
	public float poundIntensity;

	// Token: 0x04002440 RID: 9280
	private float poundIntensityMuilplier;

	// Token: 0x04002441 RID: 9281
	private float poundLerp;

	// Token: 0x04002442 RID: 9282
	private float shakeAmplitudeMultiplier;

	// Token: 0x04002443 RID: 9283
	private float shakeFrequencyMultiplier;
}
