using System;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class SledgehammerHit : MonoBehaviour
{
	// Token: 0x06000725 RID: 1829 RVA: 0x000441D0 File Offset: 0x000423D0
	public void Spawn(RoachTrigger roach)
	{
		this.Intro.Active = true;
		this.Intro.ActiveLerp = 1f;
		this.Roach = roach;
		base.transform.position = this.Roach.transform.position;
		base.transform.LookAt(this.LookAtTransform);
		this.DelayTimer = this.DelayTime;
		this.MeshTransform.gameObject.SetActive(false);
		this.Spawning = true;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00044250 File Offset: 0x00042450
	public void Hit()
	{
		this.MeshTransform.gameObject.SetActive(true);
		this.Controller.SoundHit.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.Spawning = false;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x000442A8 File Offset: 0x000424A8
	public void Update()
	{
		if (this.Spawning)
		{
			base.transform.position = this.Roach.RoachOrbit.transform.position;
			base.transform.LookAt(this.LookAtTransform);
			return;
		}
		if (!this.DebugDelayDisable)
		{
			this.DelayTimer -= Time.deltaTime;
			if (this.DelayTimer <= 0f)
			{
				this.Controller.HitDone();
			}
		}
	}

	// Token: 0x04000C55 RID: 3157
	public SledgehammerController Controller;

	// Token: 0x04000C56 RID: 3158
	public Transform LookAtTransform;

	// Token: 0x04000C57 RID: 3159
	public ToolActiveOffset Intro;

	// Token: 0x04000C58 RID: 3160
	public Transform MeshTransform;

	// Token: 0x04000C59 RID: 3161
	[Space]
	public Transform Outro;

	// Token: 0x04000C5A RID: 3162
	public AnimationCurve OutroCurve;

	// Token: 0x04000C5B RID: 3163
	private Vector3 OutroPositionStart;

	// Token: 0x04000C5C RID: 3164
	private Quaternion OutroRotationStart;

	// Token: 0x04000C5D RID: 3165
	[Space]
	public bool DebugDelayDisable;

	// Token: 0x04000C5E RID: 3166
	public float DelayTime;

	// Token: 0x04000C5F RID: 3167
	private float DelayTimer;

	// Token: 0x04000C60 RID: 3168
	private bool Spawning;

	// Token: 0x04000C61 RID: 3169
	private RoachTrigger Roach;
}
