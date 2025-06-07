using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000293 RID: 659
public class TumbleUI : MonoBehaviour
{
	// Token: 0x060014B9 RID: 5305 RVA: 0x000B6D7B File Offset: 0x000B4F7B
	private void Awake()
	{
		TumbleUI.instance = this;
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x000B6D90 File Offset: 0x000B4F90
	private void Start()
	{
		this.images1 = new Image[this.parts1.Length];
		int num = 0;
		foreach (GameObject gameObject in this.parts1)
		{
			this.images1[num] = gameObject.GetComponent<Image>();
			num++;
		}
		this.images2 = new Image[this.parts2.Length];
		num = 0;
		foreach (GameObject gameObject2 in this.parts2)
		{
			this.images2[num] = gameObject2.GetComponent<Image>();
			num++;
		}
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x000B6E20 File Offset: 0x000B5020
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (PlayerController.instance.playerAvatarScript.isTumbling && !PlayerController.instance.playerAvatarScript.isDisabled)
		{
			this.active = true;
		}
		else
		{
			this.active = false;
		}
		if (this.active != this.activePrevious)
		{
			this.activePrevious = this.active;
			this.animationLerp = 0f;
			this.updateTimer = 0f;
			this.animating = true;
		}
		this.canExit = true;
		if (this.active && (PlayerController.instance.playerAvatarScript.tumble.tumbleOverride || PlayerController.instance.tumbleInputDisableTimer > 0f))
		{
			this.canExit = false;
		}
		if (this.canExit != this.canExitPrevious)
		{
			this.canExitPrevious = this.canExit;
			if (this.canExit)
			{
				this.canExitSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.animating = true;
				this.animationLerp = 0.5f;
				this.updateTimer = 0f;
				Image[] array = this.images1;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].color = this.canExitColor1;
				}
				array = this.images2;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].color = this.canExitColor2;
				}
			}
			else
			{
				Image[] array = this.images1;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].color = this.canNotExitColor2;
				}
				array = this.images2;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].color = this.canNotExitColor1;
				}
			}
		}
		if (this.animating)
		{
			if (this.updateTimer <= 0f)
			{
				if (this.active)
				{
					if (this.animationLerp == 0f)
					{
						GameObject[] array2 = this.parts1;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(true);
						}
						array2 = this.parts2;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(true);
						}
					}
					this.animationLerp += Time.deltaTime * this.introSpeed;
					this.updateTimer = this.updateTime;
				}
				else
				{
					this.animationLerp += Time.deltaTime * this.outroSpeed;
					this.updateTimer = this.updateTime;
					if (this.animationLerp >= 1f)
					{
						GameObject[] array2 = this.parts1;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(false);
						}
						array2 = this.parts2;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(false);
						}
					}
				}
			}
			else
			{
				this.updateTimer -= Time.deltaTime;
			}
		}
		if (this.animating)
		{
			if (this.active)
			{
				base.transform.localScale = Vector3.LerpUnclamped(Vector3.one * 1.25f, Vector3.one, this.introCurve.Evaluate(this.animationLerp));
			}
			else
			{
				base.transform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one * 1.25f, this.outroCurve.Evaluate(this.animationLerp));
			}
			if (this.animationLerp >= 1f)
			{
				this.animating = false;
			}
		}
		float b = 1f;
		if (this.hideTimer > 0f)
		{
			b = 0f;
			this.hideTimer -= Time.deltaTime;
		}
		this.hideAlpha = Mathf.Lerp(this.hideAlpha, b, Time.deltaTime * 20f);
		this.canvasGroup.alpha = this.hideAlpha;
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x000B71E2 File Offset: 0x000B53E2
	public void Hide()
	{
		this.hideTimer = 0.1f;
	}

	// Token: 0x04002390 RID: 9104
	public static TumbleUI instance;

	// Token: 0x04002391 RID: 9105
	private CanvasGroup canvasGroup;

	// Token: 0x04002392 RID: 9106
	private bool active;

	// Token: 0x04002393 RID: 9107
	private bool activePrevious = true;

	// Token: 0x04002394 RID: 9108
	private bool canExit;

	// Token: 0x04002395 RID: 9109
	private bool canExitPrevious;

	// Token: 0x04002396 RID: 9110
	private bool animating;

	// Token: 0x04002397 RID: 9111
	private float animationLerp;

	// Token: 0x04002398 RID: 9112
	public Color canNotExitColor1;

	// Token: 0x04002399 RID: 9113
	public Color canNotExitColor2;

	// Token: 0x0400239A RID: 9114
	public Color canExitColor1;

	// Token: 0x0400239B RID: 9115
	public Color canExitColor2;

	// Token: 0x0400239C RID: 9116
	[Space]
	public AnimationCurve introCurve;

	// Token: 0x0400239D RID: 9117
	public float introSpeed;

	// Token: 0x0400239E RID: 9118
	[Space]
	public AnimationCurve outroCurve;

	// Token: 0x0400239F RID: 9119
	public float outroSpeed;

	// Token: 0x040023A0 RID: 9120
	[Space]
	public float updateTime;

	// Token: 0x040023A1 RID: 9121
	private float updateTimer;

	// Token: 0x040023A2 RID: 9122
	[Space]
	public GameObject[] parts1;

	// Token: 0x040023A3 RID: 9123
	public GameObject[] parts2;

	// Token: 0x040023A4 RID: 9124
	private Image[] images1;

	// Token: 0x040023A5 RID: 9125
	private Image[] images2;

	// Token: 0x040023A6 RID: 9126
	[Space]
	public Sound canExitSound;

	// Token: 0x040023A7 RID: 9127
	private float hideTimer;

	// Token: 0x040023A8 RID: 9128
	private float hideAlpha;
}
