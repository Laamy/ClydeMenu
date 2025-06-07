using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class SemiUI : MonoBehaviour
{
	// Token: 0x06001432 RID: 5170 RVA: 0x000B2388 File Offset: 0x000B0588
	protected virtual void Start()
	{
		if (this.uiText == null)
		{
			this.uiText = base.GetComponent<TextMeshProUGUI>();
		}
		if (this.uiText == null)
		{
			this.uiText = base.GetComponentInChildren<TextMeshProUGUI>();
		}
		this.initialPosition = base.transform.localPosition;
		if (this.uiText)
		{
			this.originalTextColor = this.uiText.color;
		}
		if (this.uiText)
		{
			this.originalFontColor = this.uiText.fontMaterial.GetColor(ShaderUtilities.ID_FaceColor);
		}
		if (this.uiText)
		{
			this.uiTextEnabledPrevious = this.uiText.enabled;
		}
		if (!this.textRectTransform)
		{
			this.textRectTransform = base.GetComponent<RectTransform>();
		}
		this.originalScale = this.textRectTransform.localScale;
		if (this.uiText)
		{
			this.originalGlowColor = this.uiText.fontMaterial.GetColor(ShaderUtilities.ID_GlowColor);
		}
		if (!this.animateTheEntireObject)
		{
			if (this.showPosition == new Vector2(0f, 0f))
			{
				this.showPosition = this.textRectTransform.localPosition;
			}
		}
		else if (this.showPosition == new Vector2(0f, 0f))
		{
			this.showPosition = base.transform.localPosition;
		}
		this.hidePosition += this.showPosition;
		base.StartCoroutine(this.LateStart());
		if (!this.animateTheEntireObject)
		{
			this.textRectTransform.localPosition = this.hidePosition;
		}
		else
		{
			base.transform.localPosition = this.hidePosition;
		}
		this.hidePositionCurrent = this.hidePosition;
		this.hideAnimationEvaluation = 1f;
		if (this.uiText && !this.animateTheEntireObject)
		{
			this.uiText.enabled = false;
		}
		this.hideTimer = 0.2f;
		this.allChildren = new List<GameObject>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.allChildren.Add(transform.gameObject);
		}
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x000B25FC File Offset: 0x000B07FC
	private void AllChildrenSetActive(bool active)
	{
		foreach (GameObject gameObject in this.allChildren)
		{
			bool flag = false;
			foreach (GameObject y in this.doNotDisable)
			{
				if (gameObject == y)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				gameObject.SetActive(active);
			}
		}
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x000B269C File Offset: 0x000B089C
	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.2f);
		this.animationCurveWooshAway = AssetManager.instance.animationCurveWooshAway;
		this.animationCurveWooshIn = AssetManager.instance.animationCurveWooshIn;
		this.animationCurveInOut = AssetManager.instance.animationCurveInOut;
		this.initialized = true;
		yield break;
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x000B26AC File Offset: 0x000B08AC
	protected virtual void Update()
	{
		if (!this.initialized)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.scootTimer >= 0f)
		{
			this.scootTimer -= deltaTime;
		}
		this.FlashColorLogic(deltaTime);
		this.HideAnimationLogic(deltaTime);
		this.HideTimer(deltaTime);
		this.SpringScaleLogic(deltaTime);
		this.ScootPositionLogic(deltaTime);
		this.SpringShakeLogic(deltaTime);
		this.UpdatePositionLogic();
		this.prevShowTimer = this.showTimer;
		this.prevHideTimer = this.hideTimer;
		this.prevScootTimer = this.scootTimer;
		this.prevStopHidingTimer = this.stopHidingTimer;
		this.prevStopShowingTimer = this.stopShowingTimer;
		if (this.hideTimer >= 0f)
		{
			this.hideTimer -= deltaTime;
		}
		if (this.showTimer >= 0f)
		{
			this.showTimer -= deltaTime;
		}
		if (this.stopShowingTimer >= 0f)
		{
			this.stopShowingTimer -= deltaTime;
		}
		if (this.stopHidingTimer >= 0f)
		{
			this.stopHidingTimer -= deltaTime;
		}
		if (this.stopScootingTimer >= 0f)
		{
			this.stopScootingTimer -= deltaTime;
		}
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x000B27D6 File Offset: 0x000B09D6
	public void SemiUISpringScale(float amount, float frequency, float time)
	{
		this.scaleTime = 0f;
		this.scaleAmount = amount;
		this.scaleFrequency = frequency;
		this.scaleDuration = time;
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x000B27F8 File Offset: 0x000B09F8
	private void ScootPositionLogic(float deltaTime)
	{
		if (this.scootTimer <= 0f && this.prevScootTimer <= 0f)
		{
			if (this.scootPositionCurrent != Vector2.zero)
			{
				if (this.scootHideImpulse)
				{
					this.scootPositionStart = this.scootPositionCurrent;
					this.scootHideImpulse = false;
				}
				if (this.scootEval >= 1f)
				{
					this.scootPositionCurrent = Vector2.zero;
					this.scootAnimationEvaluation = 0f;
					this.scootEval = 0f;
				}
				else
				{
					this.scootAnimationEvaluation += 4f * deltaTime;
					this.scootAnimationEvaluation = Mathf.Clamp01(this.scootAnimationEvaluation);
					this.scootEval = this.animationCurveInOut.Evaluate(this.scootAnimationEvaluation);
					this.scootPositionCurrent = Vector2.LerpUnclamped(this.scootPositionStart, Vector2.zero, this.scootEval);
				}
			}
			else
			{
				this.scootHideImpulse = true;
			}
		}
		else
		{
			this.scootHideImpulse = true;
		}
		if (this.scootTimer > 0f && this.prevScootTimer > 0f)
		{
			this.stopScootingTimer = 0.1f;
			if (this.scootPositionCurrent != this.scootPosition)
			{
				if (this.scootEval >= 1f)
				{
					this.scootPositionCurrent = this.scootPosition;
					this.scootAnimationEvaluation = 0f;
					this.scootEval = 0f;
					return;
				}
				this.scootAnimationEvaluation += 4f * deltaTime;
				this.scootAnimationEvaluation = Mathf.Clamp01(this.scootAnimationEvaluation);
				this.scootEval = this.animationCurveInOut.Evaluate(this.scootAnimationEvaluation);
				this.scootPositionCurrent = Vector2.LerpUnclamped(this.scootPositionStart, this.scootPosition, this.scootEval);
			}
		}
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x000B29BC File Offset: 0x000B0BBC
	private void UpdatePositionLogic()
	{
		if (!this.animateTheEntireObject)
		{
			this.textRectTransform.localPosition = this.hidePositionCurrent + this.scootPositionCurrent + new Vector2(this.SpringShakeX, this.SpringShakeY);
			return;
		}
		base.transform.localPosition = this.hidePositionCurrent + this.scootPositionCurrent + new Vector2(this.SpringShakeX, this.SpringShakeY);
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x000B2A40 File Offset: 0x000B0C40
	private void SpringScaleLogic(float deltaTime)
	{
		if (this.scaleTime < this.scaleDuration)
		{
			float num = this.CalculateSpringOffset(this.scaleTime, this.scaleAmount, this.scaleFrequency, this.scaleDuration);
			Vector3 vector = this.originalScale * (1f + num);
			vector = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
			if (!this.animateTheEntireObject)
			{
				this.textRectTransform.localScale = vector;
			}
			else
			{
				base.transform.localScale = vector;
			}
			this.scaleTime += deltaTime;
			return;
		}
		if (!this.animateTheEntireObject)
		{
			this.textRectTransform.localScale = this.originalScale;
			return;
		}
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x000B2B14 File Offset: 0x000B0D14
	private void HideAnimationLogic(float deltaTime)
	{
		if (this.hideTimer <= 0f && this.prevHideTimer <= 0f)
		{
			if (this.showTimer <= 0f && this.prevShowTimer <= 0f)
			{
				this.animationEval = 0f;
				this.showAnimationEvaluation = 0f;
				this.hideAnimationEvaluation = 0f;
			}
			this.showTimer = 0.1f;
		}
		if (this.showTimer > 0f && this.prevShowTimer > 0f)
		{
			this.stopShowingTimer = 0.1f;
			if (this.hidePositionCurrent != this.showPosition)
			{
				this.hidePositionCurrent = Vector2.LerpUnclamped(this.hidePosition + this.scootPositionCurrent, this.showPosition, this.animationEval);
				if (this.showAnimationEvaluation >= 1f)
				{
					this.hidePositionCurrent = this.showPosition;
					this.showAnimationEvaluation = 0f;
					this.animationEval = 0f;
				}
				else
				{
					this.showAnimationEvaluation += 4f * deltaTime;
					this.showAnimationEvaluation = Mathf.Clamp01(this.showAnimationEvaluation);
					this.animationEval = this.animationCurveWooshIn.Evaluate(this.showAnimationEvaluation);
				}
			}
		}
		if (this.hideTimer > 0f && this.prevHideTimer > 0f && this.showTimer <= 0f && this.prevShowTimer <= 0f)
		{
			this.stopHidingTimer = 0.1f;
			if (this.hidePositionCurrent != this.hidePosition)
			{
				this.hidePositionCurrent = Vector2.LerpUnclamped(this.showPosition, this.hidePosition, this.animationEval);
				if (this.hideAnimationEvaluation >= 1f)
				{
					this.hidePositionCurrent = this.hidePosition;
					this.hideAnimationEvaluation = 0f;
					this.animationEval = 0f;
					return;
				}
				this.hideAnimationEvaluation += 4f * deltaTime;
				this.hideAnimationEvaluation = Mathf.Clamp01(this.hideAnimationEvaluation);
				this.animationEval = this.animationCurveWooshAway.Evaluate(this.hideAnimationEvaluation);
			}
		}
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x000B2D44 File Offset: 0x000B0F44
	private void HideTimer(float deltaTime)
	{
		if (this.showTimer > 0f && this.prevShowTimer > 0f && this.hideTimer <= 0f && this.prevHideTimer <= 0f)
		{
			if (!this.animateTheEntireObject)
			{
				if (this.uiText && !this.uiText.enabled)
				{
					this.uiText.enabled = true;
					this.isHidden = false;
					this.AllChildrenSetActive(true);
				}
			}
			else
			{
				this.isHidden = false;
				this.AllChildrenSetActive(true);
			}
			this.hideTimer = 0f;
			return;
		}
		if (this.hideTimer <= 0f && this.prevHideTimer <= 0f && this.stopHidingTimer <= 0f && this.prevStopHidingTimer <= 0f && this.hideAnimationEvaluation == 0f)
		{
			if (!this.animateTheEntireObject)
			{
				if (this.uiText && !this.uiText.enabled)
				{
					this.uiText.enabled = true;
					this.isHidden = false;
					this.AllChildrenSetActive(true);
				}
			}
			else
			{
				this.isHidden = true;
				this.AllChildrenSetActive(true);
			}
		}
		if (this.hideTimer > 0f && this.hideAnimationEvaluation >= 1f)
		{
			if (!this.animateTheEntireObject)
			{
				if (this.uiText && this.uiText.enabled)
				{
					this.uiText.enabled = false;
					this.AllChildrenSetActive(false);
					this.isHidden = true;
					return;
				}
			}
			else
			{
				this.AllChildrenSetActive(false);
				this.isHidden = true;
			}
		}
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x000B2ED4 File Offset: 0x000B10D4
	public void SemiUIResetAllShakeEffects()
	{
		this.shakeTimeX = 0f;
		this.shakeTimeY = 0f;
		this.shakeAmountX = 0f;
		this.shakeAmountY = 0f;
		this.shakeFrequencyX = 0f;
		this.shakeFrequencyY = 0f;
		this.shakeDurationX = 0f;
		this.shakeDurationY = 0f;
		this.SpringShakeX = 0f;
		this.SpringShakeY = 0f;
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x000B2F50 File Offset: 0x000B1150
	private void FlashColorLogic(float deltaTime)
	{
		if (!this.uiText)
		{
			return;
		}
		if (this.flashColorTime > 0f)
		{
			this.flashColorTime -= deltaTime;
			this.uiText.color = this.flashColor;
			this.uiText.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, this.flashColor);
			this.uiText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, this.flashColor);
			if (this.flashColorTime <= 0f)
			{
				this.uiText.color = this.originalTextColor;
				this.uiText.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, this.originalFontColor);
				this.uiText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, this.originalGlowColor);
			}
		}
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x000B3024 File Offset: 0x000B1224
	public void SemiUISpringShakeY(float amount, float frequency, float time)
	{
		this.shakeTimeY = 0f;
		this.shakeAmountY = amount;
		this.shakeFrequencyY = frequency;
		this.shakeDurationY = time;
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x000B3046 File Offset: 0x000B1246
	public void SemiUISpringShakeX(float amount, float frequency, float time)
	{
		this.shakeTimeX = 0f;
		this.shakeAmountX = amount;
		this.shakeFrequencyX = frequency;
		this.shakeDurationX = time;
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x000B3068 File Offset: 0x000B1268
	public void SemiUITextFlashColor(Color color, float time)
	{
		this.flashColor = color;
		this.flashColorTime = time;
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x000B3078 File Offset: 0x000B1278
	private void SpringShakeLogic(float deltaTime)
	{
		float num = 0f;
		float num2 = 0f;
		if (this.shakeTimeX < this.shakeDurationX)
		{
			num = this.CalculateSpringOffset(this.shakeTimeX, this.shakeAmountX, this.shakeFrequencyX, this.shakeDurationX);
			this.SpringShakeX = num;
			this.shakeTimeX += deltaTime;
		}
		if (this.shakeTimeY < this.shakeDurationY)
		{
			num2 = this.CalculateSpringOffset(this.shakeTimeY, this.shakeAmountY, this.shakeFrequencyY, this.shakeDurationY);
			this.SpringShakeY = num2;
			this.shakeTimeY += deltaTime;
		}
		base.transform.localPosition = this.initialPosition + new Vector3(num, num2, 0f);
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x000B3138 File Offset: 0x000B1338
	private float CalculateSpringOffset(float currentTime, float amount, float frequency, float duration)
	{
		float num = currentTime / duration;
		float num2 = frequency * (1f - num);
		return amount * Mathf.Sin(num2 * num * 3.1415927f * 2f) * (1f - num);
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x000B3174 File Offset: 0x000B1374
	public void Hide()
	{
		if (this.hideTimer <= 0f && this.prevHideTimer <= 0f)
		{
			this.hideAnimationEvaluation = 0f;
			this.showAnimationEvaluation = 0f;
			this.animationEval = 0f;
			if (!this.animateTheEntireObject && this.uiText && !this.uiText.enabled)
			{
				this.uiText.enabled = false;
				this.AllChildrenSetActive(false);
				this.isHidden = true;
			}
			this.hidePositionCurrent = this.showPosition;
		}
		this.hideTimer = 0.1f;
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x000B3210 File Offset: 0x000B1410
	public void Show()
	{
		if (this.showTimer <= 0f && this.prevShowTimer <= 0f)
		{
			this.showAnimationEvaluation = 0f;
			this.hideAnimationEvaluation = 0f;
			this.animationEval = 0f;
			if (!this.animateTheEntireObject)
			{
				if (this.uiText && !this.uiText.enabled)
				{
					this.uiText.enabled = true;
					this.AllChildrenSetActive(true);
					this.isHidden = false;
				}
			}
			else
			{
				this.AllChildrenSetActive(true);
				this.isHidden = false;
			}
			this.hidePositionCurrent = this.hidePosition;
		}
		this.showTimer = 0.1f;
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x000B32C0 File Offset: 0x000B14C0
	public void SemiUIScoot(Vector2 position)
	{
		this.scootPosition = position;
		if ((this.scootTimer <= 0f && this.prevScootTimer <= 0f) || this.scootPositionPrev != this.scootPosition)
		{
			this.scootEval = 0f;
			this.scootAnimationEvaluation = 0f;
			this.scootPositionStart = this.scootPositionCurrent;
			this.scootPositionPrev = this.scootPosition;
		}
		this.scootTimer = 0.2f;
	}

	// Token: 0x04002290 RID: 8848
	internal Vector3 initialPosition;

	// Token: 0x04002291 RID: 8849
	private float shakeTimeX;

	// Token: 0x04002292 RID: 8850
	private float shakeTimeY;

	// Token: 0x04002293 RID: 8851
	private float shakeAmountX;

	// Token: 0x04002294 RID: 8852
	private float shakeAmountY;

	// Token: 0x04002295 RID: 8853
	private float shakeFrequencyX;

	// Token: 0x04002296 RID: 8854
	private float shakeFrequencyY;

	// Token: 0x04002297 RID: 8855
	private float shakeDurationX;

	// Token: 0x04002298 RID: 8856
	private float shakeDurationY;

	// Token: 0x04002299 RID: 8857
	public bool animateTheEntireObject;

	// Token: 0x0400229A RID: 8858
	[HideInInspector]
	public TextMeshProUGUI uiText;

	// Token: 0x0400229B RID: 8859
	private Color originalTextColor;

	// Token: 0x0400229C RID: 8860
	private Color originalFontColor;

	// Token: 0x0400229D RID: 8861
	private Color originalGlowColor;

	// Token: 0x0400229E RID: 8862
	private Color flashColor;

	// Token: 0x0400229F RID: 8863
	private float flashColorTime;

	// Token: 0x040022A0 RID: 8864
	private Material textMaterial;

	// Token: 0x040022A1 RID: 8865
	internal float hideTimer;

	// Token: 0x040022A2 RID: 8866
	internal float showTimer;

	// Token: 0x040022A3 RID: 8867
	private bool uiTextEnabledPrevious;

	// Token: 0x040022A4 RID: 8868
	private float scaleTime;

	// Token: 0x040022A5 RID: 8869
	private float scaleAmount;

	// Token: 0x040022A6 RID: 8870
	private float scaleFrequency;

	// Token: 0x040022A7 RID: 8871
	private float scaleDuration;

	// Token: 0x040022A8 RID: 8872
	private Vector3 originalScale;

	// Token: 0x040022A9 RID: 8873
	[HideInInspector]
	public Transform textRectTransform;

	// Token: 0x040022AA RID: 8874
	private AnimationCurve animationCurveWooshAway;

	// Token: 0x040022AB RID: 8875
	private AnimationCurve animationCurveWooshIn;

	// Token: 0x040022AC RID: 8876
	private AnimationCurve animationCurveInOut;

	// Token: 0x040022AD RID: 8877
	private float hideAnimationEvaluation;

	// Token: 0x040022AE RID: 8878
	private float showAnimationEvaluation;

	// Token: 0x040022AF RID: 8879
	public Vector2 hidePosition = new Vector2(0f, 0f);

	// Token: 0x040022B0 RID: 8880
	[HideInInspector]
	public Vector2 showPosition = new Vector2(0f, 0f);

	// Token: 0x040022B1 RID: 8881
	private Vector2 hidePositionCurrent = new Vector2(0f, 0f);

	// Token: 0x040022B2 RID: 8882
	private bool initialized;

	// Token: 0x040022B3 RID: 8883
	private Vector2 scootPosition = new Vector2(0f, 0f);

	// Token: 0x040022B4 RID: 8884
	private Vector2 scootPositionStart = new Vector2(0f, 0f);

	// Token: 0x040022B5 RID: 8885
	private float scootTimer = -123f;

	// Token: 0x040022B6 RID: 8886
	private Vector2 scootPositionPrev = new Vector2(0f, 0f);

	// Token: 0x040022B7 RID: 8887
	private bool scootHideImpulse = true;

	// Token: 0x040022B8 RID: 8888
	private float scootAnimationEvaluation;

	// Token: 0x040022B9 RID: 8889
	private Vector2 originalScootPosition = new Vector2(0f, 0f);

	// Token: 0x040022BA RID: 8890
	private Vector2 scootPositionCurrent = new Vector2(0f, 0f);

	// Token: 0x040022BB RID: 8891
	[HideInInspector]
	public bool isHidden;

	// Token: 0x040022BC RID: 8892
	private List<GameObject> allChildren = new List<GameObject>();

	// Token: 0x040022BD RID: 8893
	private float SpringShakeX;

	// Token: 0x040022BE RID: 8894
	private float SpringShakeY;

	// Token: 0x040022BF RID: 8895
	private float stopScootingTimer;

	// Token: 0x040022C0 RID: 8896
	private float stopHidingTimer;

	// Token: 0x040022C1 RID: 8897
	private float stopShowingTimer;

	// Token: 0x040022C2 RID: 8898
	private float prevShowTimer;

	// Token: 0x040022C3 RID: 8899
	private float prevHideTimer;

	// Token: 0x040022C4 RID: 8900
	private float prevScootTimer;

	// Token: 0x040022C5 RID: 8901
	private float animationEval;

	// Token: 0x040022C6 RID: 8902
	private float prevStopHidingTimer;

	// Token: 0x040022C7 RID: 8903
	private float prevStopShowingTimer;

	// Token: 0x040022C8 RID: 8904
	private float scootEval;

	// Token: 0x040022C9 RID: 8905
	public List<GameObject> doNotDisable;
}
