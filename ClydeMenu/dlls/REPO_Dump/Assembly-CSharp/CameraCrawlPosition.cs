using System;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class CameraCrawlPosition : MonoBehaviour
{
	// Token: 0x060000C0 RID: 192 RVA: 0x0000770B File Offset: 0x0000590B
	private void Start()
	{
		this.Player = PlayerController.instance;
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00007718 File Offset: 0x00005918
	private void Update()
	{
		if (this.Player.Crawling || this.Player.Sliding)
		{
			this.Active = true;
		}
		else
		{
			this.Active = false;
		}
		if (this.Active != this.ActivePrev)
		{
			if (this.Active)
			{
				PlayerController.instance.playerAvatarScript.CrouchToCrawl();
			}
			else
			{
				PlayerController.instance.playerAvatarScript.CrawlToCrouch();
			}
			GameDirector.instance.CameraShake.Shake(1f, 0.1f);
			this.ActivePrev = this.Active;
		}
		float num = this.PositionSpeed;
		if (this.Player.Sliding)
		{
			num *= 2f;
		}
		if (this.Active)
		{
			this.Lerp += Time.deltaTime * num;
		}
		else
		{
			this.Lerp -= Time.deltaTime * num;
		}
		this.Lerp = Mathf.Clamp01(this.Lerp);
		base.transform.localPosition = new Vector3(0f, this.AnimationCurve.Evaluate(this.Lerp) * this.Position, 0f);
	}

	// Token: 0x040001F7 RID: 503
	public CameraCrouchPosition CrouchPosition;

	// Token: 0x040001F8 RID: 504
	[Space]
	public float Position;

	// Token: 0x040001F9 RID: 505
	public float PositionSpeed;

	// Token: 0x040001FA RID: 506
	public AnimationCurve AnimationCurve;

	// Token: 0x040001FB RID: 507
	private float Lerp;

	// Token: 0x040001FC RID: 508
	[Space]
	public Sound IntroSound;

	// Token: 0x040001FD RID: 509
	[Space]
	public Sound OutroSound;

	// Token: 0x040001FE RID: 510
	[HideInInspector]
	public bool Active;

	// Token: 0x040001FF RID: 511
	private bool ActivePrev;

	// Token: 0x04000200 RID: 512
	private PlayerController Player;
}
