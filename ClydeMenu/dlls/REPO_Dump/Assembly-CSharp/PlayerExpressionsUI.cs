using System;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class PlayerExpressionsUI : SemiUI
{
	// Token: 0x0600142D RID: 5165 RVA: 0x000B213D File Offset: 0x000B033D
	private void Awake()
	{
		PlayerExpressionsUI.instance = this;
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x000B2145 File Offset: 0x000B0345
	protected override void Start()
	{
		base.Start();
		this.uiText = null;
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x000B2160 File Offset: 0x000B0360
	protected override void Update()
	{
		if (!LevelGenerator.Instance.Generated || SemiFunc.MenuLevel())
		{
			return;
		}
		base.Update();
		base.Hide();
		if (this.isHidden)
		{
			this.ShrinkReset();
			if (this.PlayerAvatarMenu.activeSelf)
			{
				this.PlayerAvatarMenu.SetActive(false);
			}
		}
		else if (!this.PlayerAvatarMenu.activeSelf)
		{
			this.PlayerAvatarMenu.SetActive(true);
		}
		if (this.shrinkTimer > 0f)
		{
			if (!this.shrinkActive)
			{
				this.shrinkActive = true;
				this.shrinkLerp = 0f;
				this.shrinkScalePrevious = this.rectShrink.localScale;
				this.canvasGroupAlphaPrevious = this.canvasGroup.alpha;
			}
			this.shrinkTimer -= Time.deltaTime;
			if (this.shrinkLerp < 1f)
			{
				this.shrinkLerp += Time.deltaTime * 2f;
				this.rectShrink.localScale = Vector3.LerpUnclamped(this.shrinkScalePrevious, Vector3.one, this.shrinkCurve.Evaluate(this.shrinkLerp));
				this.canvasGroup.alpha = Mathf.LerpUnclamped(this.canvasGroupAlphaPrevious, 1f, this.shrinkCurve.Evaluate(this.shrinkLerp));
				return;
			}
		}
		else
		{
			if (this.shrinkActive)
			{
				this.shrinkActive = false;
				this.shrinkLerp = 0f;
				this.shrinkScalePrevious = this.rectShrink.localScale;
				this.canvasGroupAlphaPrevious = this.canvasGroup.alpha;
			}
			if (this.shrinkLerp < 1f)
			{
				this.shrinkLerp += Time.deltaTime * 0.5f;
				this.rectShrink.localScale = Vector3.LerpUnclamped(this.shrinkScalePrevious, Vector3.one * 0.7f, this.shrinkCurve.Evaluate(this.shrinkLerp));
				this.canvasGroup.alpha = Mathf.LerpUnclamped(this.canvasGroupAlphaPrevious, 0.35f, this.shrinkCurve.Evaluate(this.shrinkLerp));
			}
		}
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x000B2371 File Offset: 0x000B0571
	public void ShrinkReset()
	{
		this.shrinkTimer = 5f;
	}

	// Token: 0x04002284 RID: 8836
	public static PlayerExpressionsUI instance;

	// Token: 0x04002285 RID: 8837
	public GameObject PlayerAvatarMenu;

	// Token: 0x04002286 RID: 8838
	public PlayerExpression playerExpression;

	// Token: 0x04002287 RID: 8839
	public PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x04002288 RID: 8840
	[Space]
	public AnimationCurve shrinkCurve;

	// Token: 0x04002289 RID: 8841
	public RectTransform rectShrink;

	// Token: 0x0400228A RID: 8842
	private CanvasGroup canvasGroup;

	// Token: 0x0400228B RID: 8843
	private float shrinkLerp;

	// Token: 0x0400228C RID: 8844
	private bool shrinkActive;

	// Token: 0x0400228D RID: 8845
	private float shrinkTimer;

	// Token: 0x0400228E RID: 8846
	private Vector3 shrinkScalePrevious;

	// Token: 0x0400228F RID: 8847
	private float canvasGroupAlphaPrevious;
}
