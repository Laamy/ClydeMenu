using System;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class ToolActiveOffset : MonoBehaviour
{
	// Token: 0x06000732 RID: 1842 RVA: 0x00044A84 File Offset: 0x00042C84
	private void Update()
	{
		if (this.Active != this.ActivePrev && this.ActiveLerp >= 1f)
		{
			if (this.MoveSoundAutomatic || this.MoveSoundManual)
			{
				this.MoveSoundManual = false;
				this.MoveSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.ActiveLerp = 0f;
			this.ActivePrev = this.Active;
			this.ActiveCurrent = this.Active;
		}
		else
		{
			if (this.ActiveCurrent)
			{
				this.ActiveLerp += this.IntroSpeed * Time.deltaTime;
			}
			else
			{
				this.ActiveLerp += this.OutroSpeed * Time.deltaTime;
			}
			this.ActiveLerp = Mathf.Clamp01(this.ActiveLerp);
		}
		if (this.ActiveCurrent)
		{
			base.transform.localPosition = Vector3.LerpUnclamped(this.InactivePosition, this.ActivePosition, this.IntroCurve.Evaluate(this.ActiveLerp));
			base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.InactiveRotation.x, this.InactiveRotation.y, this.InactiveRotation.z), Quaternion.Euler(this.ActiveRotation.x, this.ActiveRotation.y, this.ActiveRotation.z), this.IntroCurve.Evaluate(this.ActiveLerp));
			return;
		}
		base.transform.localPosition = Vector3.LerpUnclamped(this.ActivePosition, this.InactivePosition, this.OutroCurve.Evaluate(this.ActiveLerp));
		base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.ActiveRotation.x, this.ActiveRotation.y, this.ActiveRotation.z), Quaternion.Euler(this.InactiveRotation.x, this.InactiveRotation.y, this.InactiveRotation.z), this.OutroCurve.Evaluate(this.ActiveLerp));
	}

	// Token: 0x04000C80 RID: 3200
	public bool Active;

	// Token: 0x04000C81 RID: 3201
	private bool ActivePrev;

	// Token: 0x04000C82 RID: 3202
	private bool ActiveCurrent;

	// Token: 0x04000C83 RID: 3203
	[HideInInspector]
	public float ActiveLerp = 1f;

	// Token: 0x04000C84 RID: 3204
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x04000C85 RID: 3205
	public float IntroSpeed = 1.5f;

	// Token: 0x04000C86 RID: 3206
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x04000C87 RID: 3207
	public float OutroSpeed = 1.5f;

	// Token: 0x04000C88 RID: 3208
	[Space]
	public Vector3 InactivePosition;

	// Token: 0x04000C89 RID: 3209
	public Vector3 InactiveRotation;

	// Token: 0x04000C8A RID: 3210
	[Space]
	public Vector3 ActivePosition;

	// Token: 0x04000C8B RID: 3211
	public Vector3 ActiveRotation;

	// Token: 0x04000C8C RID: 3212
	[Space]
	[Header("Sound")]
	public bool MoveSoundAutomatic;

	// Token: 0x04000C8D RID: 3213
	[HideInInspector]
	public bool MoveSoundManual;

	// Token: 0x04000C8E RID: 3214
	public Sound MoveSound;
}
