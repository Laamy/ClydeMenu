using System;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class MenuLoadingGraphics : MonoBehaviour
{
	// Token: 0x060011C0 RID: 4544 RVA: 0x000A1CC0 File Offset: 0x0009FEC0
	private void Start()
	{
		this.loadingCanvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x000A1CD0 File Offset: 0x0009FED0
	private void Update()
	{
		if (!this.loadingActive)
		{
			return;
		}
		if (this.loadingDone)
		{
			this.loadingCanvasGroup.alpha -= Time.deltaTime * 5f;
			if (this.loadingCanvasGroup.alpha <= 0.01f)
			{
				this.loadingCanvasGroup.alpha = 0f;
				this.loadingCanvasGroup.gameObject.SetActive(false);
				this.loadingActive = false;
			}
		}
		else
		{
			this.loadingCanvasGroup.alpha += Time.deltaTime * 5f;
		}
		this.loadingCircle.Rotate(new Vector3(0f, 0f, -Time.deltaTime * 360f));
		this.hourglassLerp += Time.deltaTime * 1f;
		if (this.hourglassLerp > 1f)
		{
			float pitch = MenuManager.instance.soundHover.Pitch;
			MenuManager.instance.soundHover.Pitch = 0.3f;
			MenuManager.instance.soundHover.Play(Vector3.zero, 0.5f, 1f, 1f, 1f);
			MenuManager.instance.soundHover.Pitch = pitch;
			this.hourglassLerp = 0f;
		}
		this.loadingHourglass.eulerAngles = new Vector3(0f, 0f, Mathf.LerpUnclamped(90f, 0f, this.hourglassCurve.Evaluate(this.hourglassLerp)));
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x000A1E51 File Offset: 0x000A0051
	public void Reset()
	{
		this.loadingActive = true;
		this.loadingDone = false;
		this.loadingCanvasGroup.gameObject.SetActive(true);
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x000A1E72 File Offset: 0x000A0072
	public void SetDone()
	{
		this.loadingDone = true;
	}

	// Token: 0x04001E1D RID: 7709
	private CanvasGroup loadingCanvasGroup;

	// Token: 0x04001E1E RID: 7710
	public RectTransform loadingCircle;

	// Token: 0x04001E1F RID: 7711
	public RectTransform loadingHourglass;

	// Token: 0x04001E20 RID: 7712
	public AnimationCurve hourglassCurve;

	// Token: 0x04001E21 RID: 7713
	private float hourglassLerp;

	// Token: 0x04001E22 RID: 7714
	private bool loadingActive = true;

	// Token: 0x04001E23 RID: 7715
	private bool loadingDone;
}
