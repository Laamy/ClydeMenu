using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000297 RID: 663
public class VideoOverlay : MonoBehaviour
{
	// Token: 0x060014CC RID: 5324 RVA: 0x000B806A File Offset: 0x000B626A
	private void Awake()
	{
		VideoOverlay.Instance = this;
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x000B8072 File Offset: 0x000B6272
	public void Override(float time, float amount, float speed)
	{
		this.OverrideTimer = time;
		this.OverrideAmount = amount;
		this.OverrideSpeed = speed;
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x000B808C File Offset: 0x000B628C
	private void Update()
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			this.RawImage.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 5);
			return;
		}
		if ((GameDirector.instance.currentState == GameDirector.gameState.Start && LoadingUI.instance.levelAnimationCompleted) || GameDirector.instance.currentState == GameDirector.gameState.Outro)
		{
			this.RawImage.color = Color.Lerp(this.RawImage.color, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 50), 20f * Time.deltaTime);
			return;
		}
		if (this.IntroLerp < 1f)
		{
			this.IntroLerp += Time.deltaTime * 0.5f;
			float num = this.IntroCurve.Evaluate(this.IntroLerp);
			this.RawImage.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(5f * num));
			return;
		}
		if (this.OverrideTimer > 0f)
		{
			this.OverrideTimer -= Time.deltaTime;
			this.RawImage.color = Color.Lerp(this.RawImage.color, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(255f * this.OverrideAmount)), Time.deltaTime * this.OverrideSpeed);
			return;
		}
		float num2 = 0f;
		if (this.IdleTimer > 0f)
		{
			this.IdleTimer -= Time.deltaTime;
			num2 = this.IdleAlpha;
			if (!GraphicsManager.instance.glitchLoop || VideoGreenScreen.instance)
			{
				num2 = 0f;
			}
			if (this.IdleTimer <= 0f)
			{
				this.IdleCooldown = Random.Range(this.IdleCooldownMin, this.IdleCooldownMax);
			}
		}
		else if (this.IdleCooldown > 0f)
		{
			this.IdleCooldown -= Time.deltaTime;
		}
		else
		{
			this.IdleTimer = Random.Range(this.IdleTimeMin, this.IdleTimeMax);
		}
		this.RawImage.color = Color.Lerp(this.RawImage.color, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(100f * num2)), Time.deltaTime * 1f);
	}

	// Token: 0x040023D8 RID: 9176
	public static VideoOverlay Instance;

	// Token: 0x040023D9 RID: 9177
	public RawImage RawImage;

	// Token: 0x040023DA RID: 9178
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x040023DB RID: 9179
	public float IntroSpeed;

	// Token: 0x040023DC RID: 9180
	private float IntroLerp;

	// Token: 0x040023DD RID: 9181
	[Space]
	public float IdleAlpha;

	// Token: 0x040023DE RID: 9182
	public float IdleCooldownMin;

	// Token: 0x040023DF RID: 9183
	public float IdleCooldownMax;

	// Token: 0x040023E0 RID: 9184
	private float IdleCooldown;

	// Token: 0x040023E1 RID: 9185
	public float IdleTimeMin;

	// Token: 0x040023E2 RID: 9186
	public float IdleTimeMax;

	// Token: 0x040023E3 RID: 9187
	private float IdleTimer = 0.1f;

	// Token: 0x040023E4 RID: 9188
	private float OverrideTimer;

	// Token: 0x040023E5 RID: 9189
	private float OverrideAmount;

	// Token: 0x040023E6 RID: 9190
	private float OverrideSpeed;
}
