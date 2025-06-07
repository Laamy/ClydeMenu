using System;
using UnityEngine;

// Token: 0x020001D5 RID: 469
public class PlayerReviveEffects : MonoBehaviour
{
	// Token: 0x06001008 RID: 4104 RVA: 0x000935FA File Offset: 0x000917FA
	private void Start()
	{
		this.reviveLightIntensityDefault = this.reviveLight.intensity;
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00093610 File Offset: 0x00091810
	private void Update()
	{
		if (this.triggered)
		{
			this.reviveLight.intensity = Mathf.Lerp(this.reviveLight.intensity, 0f, Time.deltaTime * 1f);
			if (this.impactParticle.isStopped && this.swirlParticle.isStopped && this.reviveLight.intensity < 0.01f)
			{
				this.triggered = false;
				this.reviveLight.intensity = this.reviveLightIntensityDefault;
				this.enableTransform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x000936A8 File Offset: 0x000918A8
	public void Trigger()
	{
		base.transform.position = this.PlayerAvatar.playerDeathHead.physGrabObject.centerPoint;
		if (SemiFunc.RunIsTutorial())
		{
			base.transform.position = PlayerAvatar.instance.transform.position;
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.triggered = true;
		this.enableTransform.gameObject.SetActive(true);
		this.reviveSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04001B3E RID: 6974
	private bool triggered;

	// Token: 0x04001B3F RID: 6975
	public PlayerAvatar PlayerAvatar;

	// Token: 0x04001B40 RID: 6976
	public Transform enableTransform;

	// Token: 0x04001B41 RID: 6977
	[Space]
	public Light reviveLight;

	// Token: 0x04001B42 RID: 6978
	private float reviveLightIntensityDefault;

	// Token: 0x04001B43 RID: 6979
	public ParticleSystem impactParticle;

	// Token: 0x04001B44 RID: 6980
	public ParticleSystem swirlParticle;

	// Token: 0x04001B45 RID: 6981
	[Space]
	public Sound reviveSound;
}
