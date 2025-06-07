using System;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class ToolHide : MonoBehaviour
{
	// Token: 0x06000750 RID: 1872 RVA: 0x00045AB4 File Offset: 0x00043CB4
	public void Show()
	{
		this.ShowTimer = 0.02f;
		base.transform.localPosition = this.ToolController.CurrentHidePosition;
		base.transform.localRotation = Quaternion.Euler(this.ToolController.CurrentHideRotation.x, this.ToolController.CurrentHideRotation.y, this.ToolController.CurrentHideRotation.z);
		this.ActiveLerp = 0f;
		this.Active = true;
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00045B34 File Offset: 0x00043D34
	public void Hide()
	{
		this.ActiveLerp = 0f;
		this.Active = false;
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00045B48 File Offset: 0x00043D48
	private void Update()
	{
		if (this.ActiveLerp < 1f)
		{
			this.ActiveLerp += this.ToolController.CurrentHideSpeed * Time.deltaTime;
			this.ActiveLerp = Mathf.Clamp01(this.ActiveLerp);
			if (this.ActiveLerp >= 1f && !this.Active)
			{
				this.ToolController.HideTool();
			}
		}
		if (this.Active)
		{
			base.transform.localPosition = Vector3.LerpUnclamped(this.ToolController.CurrentHidePosition, new Vector3(0f, 0f, 0f), this.ShowCurve.Evaluate(this.ActiveLerp));
			base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.ToolController.CurrentHideRotation.x, this.ToolController.CurrentHideRotation.y, this.ToolController.CurrentHideRotation.z), Quaternion.Euler(0f, 0f, 0f), this.ShowCurve.Evaluate(this.ActiveLerp));
			base.transform.localScale = Vector3.LerpUnclamped(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), this.ShowScaleCurve.Evaluate(this.ActiveLerp));
		}
		else if (this.ActiveLerp < 1f)
		{
			base.transform.localPosition = Vector3.LerpUnclamped(new Vector3(0f, 0f, 0f), this.ToolController.CurrentHidePosition, this.HideCurve.Evaluate(this.ActiveLerp));
			base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(this.ToolController.CurrentHideRotation.x, this.ToolController.CurrentHideRotation.y, this.ToolController.CurrentHideRotation.z), this.HideCurve.Evaluate(this.ActiveLerp));
			base.transform.localScale = Vector3.LerpUnclamped(new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 0f), this.HideScaleCurve.Evaluate(this.ActiveLerp));
		}
		else
		{
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
		if (this.ShowTimer > 0f)
		{
			this.ShowTimer -= 1f * Time.deltaTime;
			if (this.ShowTimer <= 0f)
			{
				this.ToolController.ShowTool();
			}
		}
	}

	// Token: 0x04000CCE RID: 3278
	public ToolController ToolController;

	// Token: 0x04000CCF RID: 3279
	public AnimationCurve ShowCurve;

	// Token: 0x04000CD0 RID: 3280
	public AnimationCurve ShowScaleCurve;

	// Token: 0x04000CD1 RID: 3281
	public AnimationCurve HideCurve;

	// Token: 0x04000CD2 RID: 3282
	public AnimationCurve HideScaleCurve;

	// Token: 0x04000CD3 RID: 3283
	[HideInInspector]
	public bool Active;

	// Token: 0x04000CD4 RID: 3284
	[HideInInspector]
	public float ActiveLerp;

	// Token: 0x04000CD5 RID: 3285
	private float ShowTimer;
}
