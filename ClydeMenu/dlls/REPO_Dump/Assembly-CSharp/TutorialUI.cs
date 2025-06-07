using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x0200028F RID: 655
public class TutorialUI : SemiUI
{
	// Token: 0x0600149B RID: 5275 RVA: 0x000B5D70 File Offset: 0x000B3F70
	protected override void Start()
	{
		this.uiText = this.Text;
		base.Start();
		TutorialUI.instance = this;
		this.videoPlayer.clip = this.staticVideo;
		this.dummyTextTransform.gameObject.SetActive(false);
		this.dummyTextTimer = 30f;
		this.videoTransform.gameObject.SetActive(false);
		this.videoPlayer.gameObject.SetActive(false);
		base.transform.localScale = new Vector3(0f, 1f, 1f);
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x000B5E04 File Offset: 0x000B4004
	public void TutorialText(string message)
	{
		if (this.messageTimer > 0f)
		{
			return;
		}
		this.messageTimer = 0.2f;
		if (message != this.messagePrev)
		{
			this.Text.text = message;
			base.SemiUISpringShakeY(20f, 10f, 0.3f);
			base.SemiUISpringScale(0.4f, 5f, 0.2f);
			this.messagePrev = message;
		}
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x000B5E78 File Offset: 0x000B4078
	public void SetPage(VideoClip video, string text, string dummyTextString, bool transition = true)
	{
		base.SemiUISpringShakeY(10f, 8f, 0.5f);
		if (transition)
		{
			this.Text.text = "Good job! <sprite name=creepycrying>";
			this.videoPlayer.clip = this.staticVideo;
			this.nextVideo = video;
			this.nextText = text;
		}
		else
		{
			this.Text.text = text;
			this.videoPlayer.clip = video;
			this.nextVideo = video;
			this.nextText = text;
		}
		this.videoPlayer.Play();
		this.videoTransform.transform.localScale = new Vector3(1f, 1f, 1f);
		this.videoImage.color = new Color(1f, 1f, 1f, 1f);
		this.bigVideoTimer = 7f;
		this.currentDummyText = dummyTextString;
		this.dummyText.text = dummyTextString;
		this.dummyTextTimer = 30f;
		this.dummyTextAnimationEval = 0f;
		this.dummyTextTransform.gameObject.SetActive(false);
		base.StartCoroutine(this.SwitchPage());
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x000B5F9C File Offset: 0x000B419C
	public void SetTipPage(VideoClip video, string text)
	{
		this.videoTransform.gameObject.SetActive(true);
		this.videoPlayer.gameObject.SetActive(true);
		this.videoPlayer.clip = video;
		this.Text.text = text;
		this.videoPlayer.time = 0.0;
		this.videoPlayer.Play();
		this.videoTransform.transform.localScale = new Vector3(1f, 1f, 1f);
		this.videoImage.color = new Color(1f, 1f, 1f, 1f);
		this.bigVideoTimer = 6f;
		this.currentDummyText = "";
		this.dummyText.text = "";
		this.dummyTextTimer = 30f;
		this.dummyTextAnimationEval = 0f;
		this.dummyTextTransform.gameObject.SetActive(false);
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x000B6097 File Offset: 0x000B4297
	private IEnumerator SwitchPage()
	{
		yield return new WaitForSeconds(2f);
		if (this.videoPlayer.clip != this.nextVideo)
		{
			base.SemiUISpringShakeY(10f, 8f, 0.5f);
		}
		this.videoPlayer.clip = this.nextVideo;
		this.videoPlayer.Play();
		this.Text.text = this.nextText;
		yield break;
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x000B60A8 File Offset: 0x000B42A8
	protected override void Update()
	{
		base.Update();
		if (this.hideTimer <= 0f || this.showTimer > 0f)
		{
			this.videoTransform.gameObject.SetActive(true);
			this.videoPlayer.gameObject.SetActive(true);
			this.hideAllTimer = 2f;
			if (this.dummyTextTimer <= 0f)
			{
				if (this.currentDummyText != "" && !this.dummyTextTransform.gameObject.activeSelf)
				{
					this.dummyTextTransform.gameObject.SetActive(true);
					this.dummyText.text = this.currentDummyText;
					this.dummyTextAnimationEval = 0f;
					this.bigVideoTimer = 1f;
				}
				if (this.dummyTextAnimationEval < 1f)
				{
					this.dummyTextAnimationEval += Time.deltaTime * 3f;
					this.dummyTextAnimationEval = Mathf.Clamp01(this.dummyTextAnimationEval);
					float t = this.scaleInCurve.Evaluate(this.dummyTextAnimationEval);
					this.dummyTextTransform.localPosition = new Vector3(this.dummyTextTransform.localPosition.x, Mathf.LerpUnclamped(-20f, 20f, t), this.dummyTextTransform.localPosition.z);
				}
			}
			else
			{
				this.dummyTextTimer -= Time.deltaTime;
			}
			if (!SemiFunc.RunIsTutorial())
			{
				if (this.bigVideoTimer > 0f)
				{
					this.bigVideoTimer -= Time.deltaTime;
					float b = 1f;
					this.videoTransform.transform.localScale = new Vector3(Mathf.Lerp(this.videoTransform.transform.localScale.x, b, Time.deltaTime * 20f), Mathf.Lerp(this.videoTransform.transform.localScale.y, b, Time.deltaTime * 20f), Mathf.Lerp(this.videoTransform.transform.localScale.z, b, Time.deltaTime * 20f));
					float b2 = 1f;
					this.videoImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(this.videoImage.color.a, b2, Time.deltaTime * 20f));
				}
				else
				{
					float b3 = 0.7f;
					this.videoTransform.transform.localScale = new Vector3(Mathf.Lerp(this.videoTransform.transform.localScale.x, b3, Time.deltaTime * 20f), Mathf.Lerp(this.videoTransform.transform.localScale.y, b3, Time.deltaTime * 20f), Mathf.Lerp(this.videoTransform.transform.localScale.z, b3, Time.deltaTime * 20f));
					float b4 = 0.5f;
					this.videoImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(this.videoImage.color.a, b4, Time.deltaTime * 20f));
				}
			}
			this.progressBarTarget = TutorialDirector.instance.tutorialProgress;
			this.animationCurveEval += Time.deltaTime * 3f;
			this.animationCurveEval = Mathf.Clamp(this.animationCurveEval, 0f, 1f);
			float y = this.scaleInCurve.Evaluate(this.animationCurveEval);
			base.transform.localScale = new Vector3(1f, y, 1f);
			this.progressBarCurrent = this.progressBar.localScale.x;
			this.progressBar.localScale = new Vector3(Mathf.Lerp(this.progressBar.localScale.x, this.progressBarTarget, Time.deltaTime * 20f), 1f, 1f);
			if (this.currentDummyText == "" || this.dummyTextTimer > 0f)
			{
				this.dummyTextTransform.gameObject.SetActive(false);
			}
			return;
		}
		if (this.hideAllTimer > 0f)
		{
			this.hideAllTimer -= Time.deltaTime;
			return;
		}
		this.dummyTextTransform.gameObject.SetActive(false);
		this.dummyTextTimer = 30f;
		this.videoTransform.gameObject.SetActive(false);
		this.videoPlayer.gameObject.SetActive(false);
	}

	// Token: 0x04002346 RID: 9030
	public TextMeshProUGUI Text;

	// Token: 0x04002347 RID: 9031
	public Transform progressBar;

	// Token: 0x04002348 RID: 9032
	public AnimationCurve scaleInCurve;

	// Token: 0x04002349 RID: 9033
	public static TutorialUI instance;

	// Token: 0x0400234A RID: 9034
	private string messagePrev = "prev";

	// Token: 0x0400234B RID: 9035
	private Color bigMessageColor = Color.white;

	// Token: 0x0400234C RID: 9036
	private Color bigMessageFlashColor = Color.white;

	// Token: 0x0400234D RID: 9037
	private float messageTimer;

	// Token: 0x0400234E RID: 9038
	private float progressBarTarget;

	// Token: 0x0400234F RID: 9039
	internal float progressBarCurrent;

	// Token: 0x04002350 RID: 9040
	[HideInInspector]
	public float animationCurveEval;

	// Token: 0x04002351 RID: 9041
	public VideoPlayer videoPlayer;

	// Token: 0x04002352 RID: 9042
	public VideoClip staticVideo;

	// Token: 0x04002353 RID: 9043
	public VideoClip nextVideo;

	// Token: 0x04002354 RID: 9044
	private string nextText;

	// Token: 0x04002355 RID: 9045
	private float bigVideoTimer = 5f;

	// Token: 0x04002356 RID: 9046
	public Transform videoTransform;

	// Token: 0x04002357 RID: 9047
	public RawImage videoImage;

	// Token: 0x04002358 RID: 9048
	public TextMeshProUGUI dummyText;

	// Token: 0x04002359 RID: 9049
	public TextMeshProUGUI dummyTextExclamation;

	// Token: 0x0400235A RID: 9050
	public Transform dummyTextTransform;

	// Token: 0x0400235B RID: 9051
	private float dummyTextAnimationEval;

	// Token: 0x0400235C RID: 9052
	private float dummyTextTimer = 30f;

	// Token: 0x0400235D RID: 9053
	private string currentDummyText;

	// Token: 0x0400235E RID: 9054
	private float hideAllTimer;
}
