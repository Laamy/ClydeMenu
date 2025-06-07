using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class CameraTarget : MonoBehaviour
{
	// Token: 0x060000AE RID: 174 RVA: 0x00006D84 File Offset: 0x00004F84
	private void Update()
	{
		if (this.targetActiveImpulse)
		{
			if (this.targetActive)
			{
				if (this.targetActive)
				{
					this.targetToggleAudio.Play(1f);
					this.camNoise.NoiseOverride(1.5f, 1f, 2f, 0.5f, 0.5f);
				}
				this.targetActive = false;
			}
			else
			{
				if (!this.targetActive)
				{
					this.targetToggleAudio.Play(1f);
					this.camNoise.NoiseOverride(1.5f, 1f, 2f, 0.5f, 0.5f);
				}
				this.targetActive = true;
			}
			this.targetActiveImpulse = false;
		}
		if (this.targetActive)
		{
			this.targetLerpAmount = Mathf.Clamp(this.targetLerpAmount + this.targetLerpSpeed * Time.deltaTime, 0f, 1f);
			return;
		}
		this.targetLerpAmount = Mathf.Clamp(this.targetLerpAmount - this.targetLerpSpeed * Time.deltaTime, 0f, 1f);
	}

	// Token: 0x040001BC RID: 444
	[HideInInspector]
	public bool targetActiveImpulse;

	// Token: 0x040001BD RID: 445
	[HideInInspector]
	public bool targetActive;

	// Token: 0x040001BE RID: 446
	[HideInInspector]
	public float targetLerpAmount;

	// Token: 0x040001BF RID: 447
	public float targetLerpSpeed = 0.01f;

	// Token: 0x040001C0 RID: 448
	public AnimationCurve targetLerpCurve;

	// Token: 0x040001C1 RID: 449
	public AnimNoise camNoise;

	// Token: 0x040001C2 RID: 450
	public AudioPlay targetToggleAudio;
}
