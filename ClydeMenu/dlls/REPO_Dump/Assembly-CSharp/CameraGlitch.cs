using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class CameraGlitch : MonoBehaviour
{
	// Token: 0x060000CD RID: 205 RVA: 0x00007B72 File Offset: 0x00005D72
	private void Awake()
	{
		CameraGlitch.Instance = this;
		this.targetCameraFOV = this.targetCamera.fieldOfView;
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00007B8B File Offset: 0x00005D8B
	private void Start()
	{
		this.ActiveParent.SetActive(false);
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00007B9C File Offset: 0x00005D9C
	private void Update()
	{
		float num = this.targetCamera.fieldOfView / this.targetCameraFOV;
		if (num > 1.5f)
		{
			num *= 1.2f;
		}
		if (num < 0.5f)
		{
			num *= 0.8f;
		}
		base.transform.localScale = new Vector3(num, num, num);
		if (this.doNotLookEffectTimer <= 0f)
		{
			this.doNotLookEffectSound.PlayLoop(false, 2f, 1f, 1f);
			return;
		}
		this.doNotLookEffectSound.PlayLoop(true, 2f, 1f, 1f);
		this.doNotLookEffectTimer -= Time.deltaTime;
		if (this.doNotLookEffectImpulseTimer <= 0f)
		{
			this.PlayShort();
			this.doNotLookEffectImpulseTimer = Random.Range(0.3f, 1f);
			return;
		}
		this.doNotLookEffectImpulseTimer -= Time.deltaTime;
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00007C80 File Offset: 0x00005E80
	public void DoNotLookEffectSet()
	{
		this.doNotLookEffectTimer = 0.1f;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00007C90 File Offset: 0x00005E90
	public void PlayLong()
	{
		this.Animator.SetTrigger("Long");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchLongCount));
		this.GlitchLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.Shake(2f, 0.3f);
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00007D10 File Offset: 0x00005F10
	public void PlayShort()
	{
		this.Animator.SetTrigger("Short");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchShortCount));
		this.GlitchShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.Shake(2f, 0.1f);
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00007D90 File Offset: 0x00005F90
	public void PlayTiny()
	{
		this.Animator.SetTrigger("Tiny");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchTinyCount));
		this.GlitchTiny.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00007DF4 File Offset: 0x00005FF4
	public void PlayLongHurt()
	{
		this.Animator.SetTrigger("HurtLong");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchLongCount));
		this.HurtLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlitchLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(3f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(5f, 0.2f);
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00007EB8 File Offset: 0x000060B8
	public void PlayShortHurt()
	{
		this.Animator.SetTrigger("HurtShort");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchShortCount));
		this.HurtShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlitchShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(3f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(3f, 0.2f);
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00007F7C File Offset: 0x0000617C
	public void PlayLongHeal()
	{
		this.Animator.SetTrigger("HealLong");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchLongCount));
		this.HealLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlitchLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(1.5f, 0.2f);
		GameDirector.instance.CameraImpact.Shake(2.5f, 0.2f);
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00008040 File Offset: 0x00006240
	public void PlayShortHeal()
	{
		this.Animator.SetTrigger("HealShort");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchShortCount));
		this.HealShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlitchShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(1.5f, 0.2f);
		GameDirector.instance.CameraImpact.Shake(1.5f, 0.2f);
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00008104 File Offset: 0x00006304
	public void PlayUpgrade()
	{
		this.Animator.SetTrigger("Upgrade");
		this.Animator.SetInteger("Index", Random.Range(0, this.GlitchShortCount));
		this.Upgrade.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(2f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
	}

	// Token: 0x04000214 RID: 532
	public static CameraGlitch Instance;

	// Token: 0x04000215 RID: 533
	public Camera targetCamera;

	// Token: 0x04000216 RID: 534
	private float targetCameraFOV;

	// Token: 0x04000217 RID: 535
	public Animator Animator;

	// Token: 0x04000218 RID: 536
	public GameObject ActiveParent;

	// Token: 0x04000219 RID: 537
	[Space]
	public int GlitchLongCount;

	// Token: 0x0400021A RID: 538
	public int GlitchShortCount;

	// Token: 0x0400021B RID: 539
	public int GlitchTinyCount;

	// Token: 0x0400021C RID: 540
	[Space]
	public Sound GlitchLong;

	// Token: 0x0400021D RID: 541
	public Sound GlitchShort;

	// Token: 0x0400021E RID: 542
	public Sound GlitchTiny;

	// Token: 0x0400021F RID: 543
	[Space]
	public Sound HurtShort;

	// Token: 0x04000220 RID: 544
	public Sound HurtLong;

	// Token: 0x04000221 RID: 545
	[Space]
	public Sound HealShort;

	// Token: 0x04000222 RID: 546
	public Sound HealLong;

	// Token: 0x04000223 RID: 547
	[Space]
	public Sound Upgrade;

	// Token: 0x04000224 RID: 548
	[Space]
	public Sound doNotLookEffectSound;

	// Token: 0x04000225 RID: 549
	private float doNotLookEffectTimer;

	// Token: 0x04000226 RID: 550
	private float doNotLookEffectImpulseTimer;
}
