using System;
using UnityEngine;

// Token: 0x020002AE RID: 686
public class ValuableBoombox : Trap
{
	// Token: 0x06001575 RID: 5493 RVA: 0x000BD62F File Offset: 0x000BB82F
	protected override void Start()
	{
		base.Start();
		this.light1.enabled = false;
		this.light2.enabled = false;
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x000BD650 File Offset: 0x000BB850
	protected override void Update()
	{
		base.Update();
		if (!this.trapTriggered && this.physGrabObject.grabbed)
		{
			this.trapStart = true;
		}
		if (this.physGrabObject.grabbed)
		{
			this.soundBoomboxMusic.PlayLoop(true, 2f, 2f, 1f);
			this.enemyInvestigate = true;
			float num = 1f + Mathf.Sin(Time.time * 80f) * 0.005f;
			this.speaker1.localScale = new Vector3(num, num, num);
			this.speaker2.localScale = new Vector3(num, num, num);
			Color color = Color.Lerp(this.speaker1.GetComponent<Renderer>().material.GetColor("_EmissionColor"), Color.HSVToRGB(Mathf.PingPong(Time.time * 0.5f, 1f), 1f, 1f), Time.deltaTime * 6000f);
			this.speaker1.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
			this.speaker2.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
			this.light1.enabled = true;
			this.light2.enabled = true;
			this.light1.color = color;
			this.light2.color = color;
			this.light1.intensity = Mathf.Lerp(this.light1.intensity, 4f, Time.deltaTime);
			this.light2.intensity = Mathf.Lerp(this.light2.intensity, 4f, Time.deltaTime);
		}
		else
		{
			this.soundBoomboxMusic.PlayLoop(false, 2f, 2f, 1f);
			if (this.light1.enabled)
			{
				Color value = Color.Lerp(this.speaker1.GetComponent<Renderer>().material.GetColor("_EmissionColor"), Color.black, Time.deltaTime);
				this.speaker1.GetComponent<Renderer>().material.SetColor("_EmissionColor", value);
				this.speaker2.GetComponent<Renderer>().material.SetColor("_EmissionColor", value);
				this.speaker1.localScale = Vector3.Lerp(this.speaker1.localScale, Vector3.one, Time.deltaTime);
				this.speaker2.localScale = Vector3.Lerp(this.speaker2.localScale, Vector3.one, Time.deltaTime);
				this.light1.intensity = Mathf.Lerp(this.light1.intensity, 0f, Time.deltaTime);
				this.light2.intensity = Mathf.Lerp(this.light2.intensity, 0f, Time.deltaTime);
				if (this.light1.intensity < 0.01f)
				{
					this.speaker1.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
					this.speaker2.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
					this.light1.enabled = false;
					this.light2.enabled = false;
					this.speaker1.localScale = Vector3.one;
					this.speaker2.localScale = Vector3.one;
				}
			}
		}
		if (this.physGrabObject.grabbed)
		{
			float amount = Mathf.Sin(Time.time * 15f) * 0.5f;
			float num2 = Mathf.Sin(Time.time * 15f) * 25f;
			foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
			{
				if (physGrabber.isLocal)
				{
					CameraAim.Instance.AdditiveAimY(amount);
					physGrabber.OverrideGrabDistance(1f);
					physGrabber.OverrideDisableRotationControls();
					physGrabber.playerAvatar.playerExpression.OverrideExpressionSet(4, 100f);
					PlayerExpressionsUI.instance.playerExpression.OverrideExpressionSet(4, 100f);
					PlayerExpressionsUI.instance.playerAvatarVisuals.HeadTiltOverride(num2 * 0.5f);
				}
				else
				{
					physGrabber.playerAvatar.playerAvatarVisuals.HeadTiltOverride(num2);
				}
			}
		}
	}

	// Token: 0x04002544 RID: 9540
	[Header("Sounds")]
	public Sound soundBoomboxStart;

	// Token: 0x04002545 RID: 9541
	public Sound soundBoomboxStop;

	// Token: 0x04002546 RID: 9542
	public Sound soundBoomboxMusic;

	// Token: 0x04002547 RID: 9543
	[Space]
	public Transform speaker1;

	// Token: 0x04002548 RID: 9544
	public Transform speaker2;

	// Token: 0x04002549 RID: 9545
	[Space]
	public Light light1;

	// Token: 0x0400254A RID: 9546
	public Light light2;
}
