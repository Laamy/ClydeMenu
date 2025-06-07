using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200024F RID: 591
public class GrandfatherClockTrap : Trap
{
	// Token: 0x06001328 RID: 4904 RVA: 0x000AB0D8 File Offset: 0x000A92D8
	protected override void Start()
	{
		base.Start();
		this.ticVolume = this.Tic.Volume;
		this.tocVolume = this.Toc.Volume;
		this.bellVolume = this.Bell.Volume;
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x000AB12C File Offset: 0x000A932C
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.GrandfatherClockActivate();
		}
		this.Tic.Volume = this.ticVolume * this.masterVolume;
		this.Toc.Volume = this.tocVolume * this.masterVolume;
		this.Bell.Volume = this.bellVolume * this.masterVolume;
		if (!this.angleLerpRev)
		{
			this.angleLerp += Time.deltaTime + this.pendulumSpeed * Time.deltaTime;
			if (this.angleLerp > 1f)
			{
				this.angleLerpRev = true;
			}
		}
		else
		{
			this.angleLerp -= Time.deltaTime + this.pendulumSpeed * Time.deltaTime;
			if (this.angleLerp < 0f)
			{
				this.angleLerpRev = false;
			}
		}
		float y = Mathf.Lerp(-this.angle, this.angle, this.angleCurve.Evaluate(this.angleLerp));
		this.pendulum.localEulerAngles = new Vector3(0f, y, 0f);
		if (this.angleLerp >= 1f - this.offset && !this.ticPlayed)
		{
			this.Tic.Play(this.pendulum.position, 1f, 1f, 1f, 1f);
			this.ticPlayed = true;
			this.tocPlayed = false;
		}
		if (this.angleLerp <= 0f + this.offset && !this.tocPlayed)
		{
			this.Toc.Play(this.pendulum.position, 1f, 1f, 1f, 1f);
			this.tocPlayed = true;
			this.ticPlayed = false;
		}
		if (this.trapTriggered)
		{
			this.pendulumSpeedLerp += Time.deltaTime / 15f;
			this.pendulumSpeed = Mathf.Lerp(this.pendulumSpeedMin, this.pendulumSpeedMax, this.pendulumSpeedCurve.Evaluate(this.pendulumSpeedLerp));
		}
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x000AB338 File Offset: 0x000A9538
	public void GrandfatherClockBell()
	{
		if (this.bellRingCount < 3)
		{
			this.Bell.Play(this.pendulum.position, 1f, 1f, 1f, 1f);
			this.enemyInvestigate = true;
			this.enemyInvestigateRange = 60f;
			this.bellRingCount++;
			this.bellRingTimer.Invoke();
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 6f, 12f, this.pendulum.position, 0.2f);
		}
	}

	// Token: 0x0600132B RID: 4907 RVA: 0x000AB3D8 File Offset: 0x000A95D8
	public void GrandfatherClockActivate()
	{
		if (!this.trapTriggered)
		{
			this.Bell.Play(this.pendulum.position, 1f, 1f, 1f, 1f);
			this.trapTriggered = true;
			this.bellRingTimer.Invoke();
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 6f, 12f, this.pendulum.position, 0.2f);
		}
	}

	// Token: 0x04002095 RID: 8341
	public UnityEvent bellRingTimer;

	// Token: 0x04002096 RID: 8342
	[Header("Trap Activated Animation")]
	[Header("Pendulum")]
	public Transform pendulum;

	// Token: 0x04002097 RID: 8343
	private float angle = 6.25f;

	// Token: 0x04002098 RID: 8344
	private bool angleLerpRev;

	// Token: 0x04002099 RID: 8345
	private float angleLerp;

	// Token: 0x0400209A RID: 8346
	public AnimationCurve angleCurve;

	// Token: 0x0400209B RID: 8347
	private float pendulumSpeedMin;

	// Token: 0x0400209C RID: 8348
	public float pendulumSpeedMax = 5f;

	// Token: 0x0400209D RID: 8349
	private float pendulumSpeed;

	// Token: 0x0400209E RID: 8350
	public AnimationCurve pendulumSpeedCurve;

	// Token: 0x0400209F RID: 8351
	private float pendulumSpeedLerp;

	// Token: 0x040020A0 RID: 8352
	private float offset = 0.49f;

	// Token: 0x040020A1 RID: 8353
	private bool ticPlayed;

	// Token: 0x040020A2 RID: 8354
	private bool tocPlayed;

	// Token: 0x040020A3 RID: 8355
	[Header("Sounds")]
	public Sound Tic;

	// Token: 0x040020A4 RID: 8356
	private float ticVolume;

	// Token: 0x040020A5 RID: 8357
	public Sound Toc;

	// Token: 0x040020A6 RID: 8358
	private float tocVolume;

	// Token: 0x040020A7 RID: 8359
	public Sound Bell;

	// Token: 0x040020A8 RID: 8360
	private float bellVolume;

	// Token: 0x040020A9 RID: 8361
	private int bellRingCount;

	// Token: 0x040020AA RID: 8362
	private float masterVolume = 1f;

	// Token: 0x040020AB RID: 8363
	private Rigidbody rb;
}
