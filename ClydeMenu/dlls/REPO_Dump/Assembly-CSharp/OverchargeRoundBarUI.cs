using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000282 RID: 642
public class OverchargeRoundBarUI : MonoBehaviour
{
	// Token: 0x06001422 RID: 5154 RVA: 0x000B193C File Offset: 0x000AFB3C
	private void Start()
	{
		this.roundBarScale = new SpringFloat();
		this.roundBarScale.speed = 40f;
		this.roundBarScale.damping = 0.5f;
		this.originalColor = this.roundBar.color;
		this.overchargeWarningIcon.enabled = false;
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x000B199D File Offset: 0x000AFB9D
	private void Show()
	{
		this.showTimer = 0.2f;
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x000B19AC File Offset: 0x000AFBAC
	private void Update()
	{
		if (WorldSpaceUIParent.instance)
		{
			this.canvasGroup.alpha = WorldSpaceUIParent.instance.canvasGroup.alpha;
		}
		if (this.showTimer <= 0f && this.show)
		{
			this.show = false;
		}
		if (this.showTimer > 0f)
		{
			this.show = true;
			this.showTimer -= Time.deltaTime;
		}
		if (this.show)
		{
			this.scaleTarget = 1f;
			if (!this.roundBar.enabled)
			{
				this.roundBarScale.springVelocity = 50f;
				this.roundBarScale.speed = 40f;
				this.roundBarScale.damping = 0.5f;
				this.roundBar.enabled = true;
				this.roundBarBG.enabled = true;
			}
		}
		else if (this.scaleTarget != 0f)
		{
			this.roundBarScale.speed = 50f;
			this.roundBarScale.damping = 0.9f;
			this.roundBarScale.springVelocity = 50f;
			this.scaleTarget = 0f;
		}
		if (!this.show && this.roundBarObject.transform.localScale.x <= 0f && this.roundBar.enabled)
		{
			this.roundBar.enabled = false;
			this.roundBarBG.enabled = false;
			this.overchargeWarningIcon.enabled = false;
			this.roundBarObject.transform.localScale = Vector3.zero;
			this.roundBarScale.springVelocity = 0f;
			this.roundBarScale.lastPosition = 0f;
		}
		this.FlashColorLogic();
		float num = Mathf.Ceil(PhysGrabber.instance.physGrabBeamOverChargeFloat * 100f);
		if (num > 0f && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			this.Show();
		}
		this.roundBarFillAmount = num / 100f;
		this.roundBar.fillAmount = Mathf.Lerp(this.roundBar.fillAmount, this.roundBarFillAmount, Time.deltaTime * 5f);
		float num2 = SemiFunc.SpringFloatGet(this.roundBarScale, this.scaleTarget, -1f);
		this.roundBarObject.transform.localScale = new Vector3(num2, num2, num2);
		if (PhysGrabber.instance.grabbed && PhysGrabber.instance.grabbedPhysGrabObject && PhysGrabber.instance.grabbedPhysGrabObject.isEnemy)
		{
			if (num > 70f)
			{
				this.warningTimer += Time.deltaTime * 2f;
			}
			if (num > 80f)
			{
				this.warningTimer += Time.deltaTime * 2f;
			}
			if (num > 90f)
			{
				this.warningTimer += Time.deltaTime * 2f;
			}
			if (num > 95f)
			{
				this.warningTimer += Time.deltaTime * 2f;
			}
		}
		else if (num > 80f)
		{
			this.warningTimer += Time.deltaTime * 2f;
		}
		if (this.warningTimer > 1f)
		{
			Color color = new Color(1f, 0.6f, 0f);
			this.soundOverchargeWarning.Play(PlayerAvatar.instance.transform.position, 1f, 1f, 1f, 1f);
			this.FlashColor(color, 0.2f);
			this.roundBarScale.springVelocity = 100f;
			this.warningTimer = 0f;
		}
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x000B1D54 File Offset: 0x000AFF54
	private void FlashColor(Color color, float time)
	{
		this.flashColor = color;
		this.flashColorTime = time;
		this.flashColorTimer = time;
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x000B1D6C File Offset: 0x000AFF6C
	private void FlashColorLogic()
	{
		if (this.flashColorTimer > 0f)
		{
			this.overchargeWarningIcon.enabled = true;
			this.roundBar.color = Color.Lerp(this.flashColor, this.originalColor, this.flashColorTimer / this.flashColorTime);
			this.overchargeWarningIcon.color = Color.Lerp(this.flashColor, this.originalColor, this.flashColorTimer / this.flashColorTime);
			this.flashColorTimer -= Time.deltaTime;
			return;
		}
		this.overchargeWarningIcon.enabled = false;
		this.overchargeWarningIcon.color = this.originalColor;
		this.roundBar.color = this.originalColor;
	}

	// Token: 0x04002269 RID: 8809
	public Image roundBar;

	// Token: 0x0400226A RID: 8810
	public Image roundBarBG;

	// Token: 0x0400226B RID: 8811
	public Image overchargeWarningIcon;

	// Token: 0x0400226C RID: 8812
	public GameObject roundBarObject;

	// Token: 0x0400226D RID: 8813
	internal float roundBarFillAmount;

	// Token: 0x0400226E RID: 8814
	private SpringFloat roundBarScale;

	// Token: 0x0400226F RID: 8815
	private float scaleTarget;

	// Token: 0x04002270 RID: 8816
	private float warningTimer;

	// Token: 0x04002271 RID: 8817
	private float flashColorTime;

	// Token: 0x04002272 RID: 8818
	private float flashColorTimer;

	// Token: 0x04002273 RID: 8819
	private Color flashColor;

	// Token: 0x04002274 RID: 8820
	private Color originalColor;

	// Token: 0x04002275 RID: 8821
	public Sound soundOverchargeWarning;

	// Token: 0x04002276 RID: 8822
	private float showTimer;

	// Token: 0x04002277 RID: 8823
	private bool show;

	// Token: 0x04002278 RID: 8824
	private CanvasGroup canvasGroup;
}
