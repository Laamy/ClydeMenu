using System;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class PlayerAvatarOverchargeVisuals : MonoBehaviour
{
	// Token: 0x06000F53 RID: 3923 RVA: 0x00089934 File Offset: 0x00087B34
	private void Start()
	{
		this.physGrabber = base.GetComponent<PhysGrabber>();
		this.overchargeLight = base.GetComponentInChildren<Light>();
		this.overchargeParticles = base.GetComponentInChildren<ParticleSystem>();
		PhysGrabBeam componentInParent = base.GetComponentInParent<PhysGrabBeam>();
		this.physGrabber = componentInParent.playerAvatar.GetComponent<PhysGrabber>();
		if (!this.physGrabber.isLocal)
		{
			this.physGrabBeamOrigin = componentInParent.PhysGrabPointOriginClient;
		}
		else
		{
			this.physGrabBeamOrigin = componentInParent.PhysGrabPointOrigin;
		}
		this.playerAvatar = componentInParent.playerAvatar;
		base.transform.parent = this.playerAvatar.transform.parent;
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x000899CC File Offset: 0x00087BCC
	private void Update()
	{
		base.transform.position = this.physGrabBeamOrigin.position;
		if (this.physGrabber.isLocal)
		{
			base.transform.position += this.playerAvatar.localCameraTransform.forward * 0.5f;
		}
		if (this.playerAvatar.isTumbling)
		{
			base.transform.position = this.playerAvatar.playerAvatarVisuals.transform.position;
		}
		float num = (float)this.physGrabber.physGrabBeamOverCharge / 2f;
		num /= 100f;
		if (this.playerAvatar.isDisabled)
		{
			num = 0f;
		}
		bool playing = num > 0f;
		float num2 = this.overchargeIntensityCurve.Evaluate(num);
		this.soundOverchargeLoop.LoopVolumeCurrent = 0.5f * num2;
		this.soundOverchargeLoop.PlayLoop(playing, 0.5f, 0.5f, 1f + 2f * num2);
		if (num > 0f)
		{
			if (!this.overchargeLight.enabled)
			{
				this.overchargeParticles.Play();
				this.overchargeLight.enabled = true;
			}
			float num3 = num * Mathf.Sin(Time.time * (10f + 20f * num2));
			this.overchargeLight.intensity = 8f * num2 + num3;
			this.overchargeParticles.emissionRate = num2 * 50f;
			this.overchargeParticles.transform.localScale = new Vector3(1f, 1f, 1f) * (0.1f + 0.8f * num2);
			return;
		}
		if (this.overchargeLight.enabled)
		{
			this.overchargeParticles.Stop();
			this.overchargeLight.enabled = false;
		}
	}

	// Token: 0x0400192F RID: 6447
	private Light overchargeLight;

	// Token: 0x04001930 RID: 6448
	private ParticleSystem overchargeParticles;

	// Token: 0x04001931 RID: 6449
	private PhysGrabber physGrabber;

	// Token: 0x04001932 RID: 6450
	private Transform physGrabBeamOrigin;

	// Token: 0x04001933 RID: 6451
	private PlayerAvatar playerAvatar;

	// Token: 0x04001934 RID: 6452
	public Sound soundOverchargeLoop;

	// Token: 0x04001935 RID: 6453
	public AnimationCurve overchargeIntensityCurve;
}
