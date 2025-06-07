using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000254 RID: 596
public class TrapRadio : Trap
{
	// Token: 0x06001341 RID: 4929 RVA: 0x000AC220 File Offset: 0x000AA420
	protected override void Start()
	{
		base.Start();
		this.initialRadioRotation = this.Radio.transform.localRotation;
		this.initialRadioMeterRotation = this.RadioMeter.localRotation;
		this.RadioLight.enabled = false;
		this.RadioDisplay.enabled = false;
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x000AC274 File Offset: 0x000AA474
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.RadioTrapActivated();
		}
		this.RadioLoop.PlayLoop(this.RadioPlaying, 2f, 2f, 1f);
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			if (this.RadioFlickerIntro || this.RadioFlickerOutro)
			{
				float num = this.RadioFlickerCurve.Evaluate(this.RadioFlickerTimer / this.RadioFlickerTime);
				this.RadioFlickerTimer += 1f * Time.deltaTime;
				if (num > 0.5f)
				{
					this.RadioLight.enabled = true;
					this.RadioDisplay.enabled = true;
				}
				else
				{
					this.RadioLight.enabled = false;
					this.RadioDisplay.enabled = false;
				}
				if (this.RadioFlickerTimer > this.RadioFlickerTime)
				{
					this.RadioFlickerIntro = false;
					this.RadioFlickerTimer = 0f;
					if (this.RadioFlickerOutro)
					{
						this.RadioLight.enabled = false;
						this.RadioDisplay.enabled = false;
					}
					else
					{
						this.RadioLight.enabled = true;
						this.RadioDisplay.enabled = true;
					}
				}
			}
			this.RadioPlaying = true;
			float num2 = 1f;
			if (this.StartSequenceProgress == 0f && !this.StartSequence)
			{
				this.StartSequence = true;
				this.RadioStart.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				this.RadioLight.enabled = true;
				this.RadioDisplay.enabled = true;
			}
			if (this.StartSequence)
			{
				num2 += this.RadioStartCurve.Evaluate(this.StartSequenceProgress) * this.RadioStartIntensity;
				this.StartSequenceProgress += Time.deltaTime / this.RadioStartDuration;
				if (this.StartSequenceProgress >= 1f)
				{
					this.StartSequence = false;
				}
			}
			if (this.endSequence)
			{
				num2 += this.RadioEndCurve.Evaluate(this.EndSequenceProgress) * this.RadioEndIntensity;
				this.EndSequenceProgress += Time.deltaTime / this.RadioEndDuration;
				if (this.EndSequenceProgress >= 1f)
				{
					this.EndSequenceDone();
				}
			}
			float num3 = 1f * num2;
			float num4 = 40f;
			float num5 = num3 * Mathf.Sin(Time.time * num4);
			float z = num3 * Mathf.Sin(Time.time * num4 + 1.5707964f);
			this.Radio.transform.localRotation = this.initialRadioRotation * Quaternion.Euler(num5, 0f, z);
			num4 = 20f;
			float num6 = num3 * Mathf.Sin(Time.time * num4);
			this.RadioMeter.localRotation = this.initialRadioMeterRotation * Quaternion.Euler(0f, num6 * 90f, 0f);
			this.Radio.transform.localPosition = new Vector3(this.Radio.transform.localPosition.x, this.Radio.transform.localPosition.y - num5 * 0.005f * Time.deltaTime, this.Radio.transform.localPosition.z);
		}
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x000AC5A4 File Offset: 0x000AA7A4
	private void EndSequenceDone()
	{
		this.endSequence = false;
		this.RadioLight.enabled = false;
		this.RadioDisplay.enabled = false;
		this.RadioPlaying = false;
		this.trapActive = false;
		this.Radio.transform.localRotation = this.initialRadioRotation;
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x000AC5F4 File Offset: 0x000AA7F4
	public void RadioTrapStop()
	{
		this.RadioEnd.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		this.endSequence = true;
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x000AC628 File Offset: 0x000AA828
	public void RadioTrapActivated()
	{
		if (!this.trapTriggered)
		{
			this.radioTimer.Invoke();
			this.trapTriggered = true;
			this.trapActive = true;
		}
	}

	// Token: 0x040020D1 RID: 8401
	public UnityEvent radioTimer;

	// Token: 0x040020D2 RID: 8402
	public MeshRenderer RadioDisplay;

	// Token: 0x040020D3 RID: 8403
	public Light RadioLight;

	// Token: 0x040020D4 RID: 8404
	public Transform RadioMeter;

	// Token: 0x040020D5 RID: 8405
	public AnimationCurve RadioFlickerCurve;

	// Token: 0x040020D6 RID: 8406
	public float RadioFlickerTime = 0.5f;

	// Token: 0x040020D7 RID: 8407
	private float RadioFlickerTimer;

	// Token: 0x040020D8 RID: 8408
	private bool RadioFlickerIntro = true;

	// Token: 0x040020D9 RID: 8409
	private bool RadioFlickerOutro;

	// Token: 0x040020DA RID: 8410
	[Space]
	[Header("Gramophone Components")]
	public GameObject Radio;

	// Token: 0x040020DB RID: 8411
	[Space]
	[Header("Sounds")]
	public Sound RadioStart;

	// Token: 0x040020DC RID: 8412
	public Sound RadioEnd;

	// Token: 0x040020DD RID: 8413
	public Sound RadioLoop;

	// Token: 0x040020DE RID: 8414
	[Space]
	[Header("Radio Animation")]
	public AnimationCurve RadioStartCurve;

	// Token: 0x040020DF RID: 8415
	public float RadioStartIntensity;

	// Token: 0x040020E0 RID: 8416
	public float RadioStartDuration;

	// Token: 0x040020E1 RID: 8417
	[Space]
	public AnimationCurve RadioEndCurve;

	// Token: 0x040020E2 RID: 8418
	public float RadioEndIntensity;

	// Token: 0x040020E3 RID: 8419
	public float RadioEndDuration;

	// Token: 0x040020E4 RID: 8420
	private bool StartSequence;

	// Token: 0x040020E5 RID: 8421
	private bool endSequence;

	// Token: 0x040020E6 RID: 8422
	private float StartSequenceProgress;

	// Token: 0x040020E7 RID: 8423
	private float EndSequenceProgress;

	// Token: 0x040020E8 RID: 8424
	private bool RadioPlaying;

	// Token: 0x040020E9 RID: 8425
	private Quaternion initialRadioRotation;

	// Token: 0x040020EA RID: 8426
	private Quaternion initialRadioMeterRotation;
}
