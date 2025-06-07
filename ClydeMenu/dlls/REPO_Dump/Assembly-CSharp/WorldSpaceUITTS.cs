using System;
using TMPro;
using UnityEngine;

// Token: 0x0200029B RID: 667
public class WorldSpaceUITTS : WorldSpaceUIChild
{
	// Token: 0x060014DF RID: 5343 RVA: 0x000B890C File Offset: 0x000B6B0C
	private void Awake()
	{
		this.text = base.GetComponent<TextMeshProUGUI>();
		this.text.color = new Color(this.textColor.r, this.textColor.g, this.textColor.b, 0f);
		this.cameraMain = Camera.main;
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x000B8968 File Offset: 0x000B6B68
	protected override void Update()
	{
		base.Update();
		if (this.alphaCheckTimer <= 0f)
		{
			this.textAlphaTarget = 1f;
			this.alphaCheckTimer = 0.1f;
			if (!SpectateCamera.instance || SpectateCamera.instance.CheckState(SpectateCamera.State.Normal))
			{
				float num = 5f;
				float num2 = 20f;
				float num3 = Vector3.Distance(this.cameraMain.transform.position, this.worldPosition);
				if (num3 > num)
				{
					num3 = Mathf.Clamp(num3, num, num2);
					this.textAlphaTarget = 1f - (num3 - num) / (num2 - num);
				}
				if (this.ttsVoice && this.ttsVoice.playerAvatar.voiceChat.lowPassLogicTTS.LowPass)
				{
					this.textAlphaTarget *= 0.5f;
				}
			}
		}
		else
		{
			this.alphaCheckTimer -= Time.deltaTime;
		}
		if (!this.followTransform || !this.ttsVoice || !this.ttsVoice.isSpeaking || !this.playerAvatar || this.playerAvatar.isDisabled)
		{
			this.textAlphaTarget = 0f;
			if (this.textAlpha < 0.01f)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}
		this.textAlpha = Mathf.Lerp(this.textAlpha, this.textAlphaTarget, 30f * Time.deltaTime);
		if (!this.flashDone)
		{
			this.flashTimer -= Time.deltaTime;
			if (this.flashTimer <= 0f)
			{
				if (this.textColor != this.textColorTarget)
				{
					this.textColor = Color.Lerp(this.textColor, this.textColorTarget, 20f * Time.deltaTime);
				}
				else
				{
					this.flashDone = true;
				}
			}
		}
		this.text.color = new Color(this.textColor.r, this.textColor.g, this.textColor.b, this.textAlpha);
		if (this.followTransform)
		{
			this.followPosition = Vector3.Lerp(this.followPosition, this.followTransform.position, 10f * Time.deltaTime);
		}
		this.worldPosition = this.followPosition + this.curveIntro.Evaluate(this.curveLerp) * Vector3.up * 0.025f;
		this.curveLerp += Time.deltaTime * 4f;
		this.curveLerp = Mathf.Clamp01(this.curveLerp);
		if (this.ttsVoice && this.ttsVoice.currentWordTime != this.wordTime)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040023F9 RID: 9209
	internal TextMeshProUGUI text;

	// Token: 0x040023FA RID: 9210
	internal float wordTime;

	// Token: 0x040023FB RID: 9211
	internal TTSVoice ttsVoice;

	// Token: 0x040023FC RID: 9212
	internal Transform followTransform;

	// Token: 0x040023FD RID: 9213
	internal PlayerAvatar playerAvatar;

	// Token: 0x040023FE RID: 9214
	private float flashTimer = 0.1f;

	// Token: 0x040023FF RID: 9215
	private Color textColor = Color.yellow;

	// Token: 0x04002400 RID: 9216
	private Color textColorTarget = Color.white;

	// Token: 0x04002401 RID: 9217
	private bool flashDone;

	// Token: 0x04002402 RID: 9218
	public AnimationCurve curveIntro;

	// Token: 0x04002403 RID: 9219
	private float curveLerp;

	// Token: 0x04002404 RID: 9220
	internal Vector3 followPosition;

	// Token: 0x04002405 RID: 9221
	private float alphaCheckTimer;

	// Token: 0x04002406 RID: 9222
	private float textAlphaTarget;

	// Token: 0x04002407 RID: 9223
	private float textAlpha;

	// Token: 0x04002408 RID: 9224
	private Camera cameraMain;
}
