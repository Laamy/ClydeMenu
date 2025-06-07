using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200026D RID: 621
public class LoadingUI : MonoBehaviour
{
	// Token: 0x060013B5 RID: 5045 RVA: 0x000AF01C File Offset: 0x000AD21C
	private void Awake()
	{
		LoadingUI.instance = this;
		this.fadeBehindImage.gameObject.SetActive(false);
		this.animator = base.GetComponent<Animator>();
		this.animator.enabled = false;
		this.animator.keepAnimatorStateOnDisable = true;
		if (RunManager.instance)
		{
			this.fadeImage.color = RunManager.instance.loadingFadeColor;
			this.animator.Play("Idle", 0, RunManager.instance.loadingAnimationTime);
		}
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x000AF0A0 File Offset: 0x000AD2A0
	private void LateUpdate()
	{
		if (RunManager.instance.skipLoadingUI)
		{
			return;
		}
		float num = Time.deltaTime;
		if (!this.levelAnimationStarted)
		{
			num = Mathf.Min(num, 0.01f);
		}
		this.animator.Update(num);
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait || (GameDirector.instance.currentState == GameDirector.gameState.Start && !this.levelAnimationCompleted))
		{
			this.fadeImage.color = Color.Lerp(this.fadeImage.color, new Color(0f, 0f, 0f, 0f), 5f * num);
		}
		else
		{
			this.fadeImage.color = Color.Lerp(this.fadeImage.color, Color.black, 20f * num);
		}
		RunManager.instance.loadingFadeColor = this.fadeImage.color;
		RunManager.instance.loadingAnimationTime = this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		if (GameDirector.instance.PlayerList.Count > 0)
		{
			bool flag = true;
			using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.levelAnimationCompleted)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.levelAnimationCompleted = true;
			}
		}
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x000AF220 File Offset: 0x000AD420
	public void StopLoading()
	{
		RunManager.instance.loadingFadeColor = Color.black;
		RunManager.instance.skipLoadingUI = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x000AF248 File Offset: 0x000AD448
	public void StartLoading()
	{
		this.levelAnimationStarted = false;
		base.gameObject.SetActive(true);
		this.fadeImage.color = RunManager.instance.loadingFadeColor;
		this.animator.Play("Idle", 0, RunManager.instance.loadingAnimationTime);
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x000AF298 File Offset: 0x000AD498
	public void LevelAnimationStart()
	{
		this.levelAnimationStarted = true;
		if (!RunManager.instance.skipLoadingUI && !this.debugDisableLevelAnimation && (SemiFunc.RunIsLevel() || SemiFunc.RunIsShop() || SemiFunc.RunIsArena()))
		{
			this.loadingGraphic01.sprite = LevelGenerator.Instance.Level.LoadingGraphic01;
			if (!LevelGenerator.Instance.Level.LoadingGraphic01)
			{
				this.loadingGraphic01.color = Color.clear;
			}
			this.loadingGraphic02.sprite = LevelGenerator.Instance.Level.LoadingGraphic02;
			if (!LevelGenerator.Instance.Level.LoadingGraphic02)
			{
				this.loadingGraphic02.color = Color.clear;
			}
			this.loadingGraphic03.sprite = LevelGenerator.Instance.Level.LoadingGraphic03;
			if (!LevelGenerator.Instance.Level.LoadingGraphic03)
			{
				this.loadingGraphic03.color = Color.clear;
			}
			if (SemiFunc.RunIsShop())
			{
				this.levelNumberText.text = "SHOP";
			}
			else if (SemiFunc.RunIsArena())
			{
				this.levelNumberText.text = "GAME OVER";
				this.levelNumberText.color = Color.red;
			}
			else
			{
				this.levelNumberText.text = "LEVEL " + (RunManager.instance.levelsCompleted + 1).ToString();
			}
			this.levelNameText.text = LevelGenerator.Instance.Level.NarrativeName.ToUpper();
			this.animator.SetTrigger("Level");
			return;
		}
		this.levelAnimationCompleted = true;
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x000AF440 File Offset: 0x000AD640
	public void LevelAnimationComplete()
	{
		PlayerController.instance.playerAvatarScript.LoadingLevelAnimationCompleted();
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x000AF451 File Offset: 0x000AD651
	public void PlayTurn()
	{
		this.soundTurn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x000AF47E File Offset: 0x000AD67E
	public void PlayRevUp()
	{
		this.soundRevUp.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x000AF4AB File Offset: 0x000AD6AB
	public void PlayCrash()
	{
		this.soundCrash.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x000AF4D8 File Offset: 0x000AD6D8
	public void PlayTextLevel()
	{
		this.soundtextLevel.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060013BF RID: 5055 RVA: 0x000AF505 File Offset: 0x000AD705
	public void PlayTextName()
	{
		this.soundtextName.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040021D7 RID: 8663
	public static LoadingUI instance;

	// Token: 0x040021D8 RID: 8664
	public Image fadeImage;

	// Token: 0x040021D9 RID: 8665
	public Image fadeBehindImage;

	// Token: 0x040021DA RID: 8666
	[Space]
	public TextMeshProUGUI levelNumberText;

	// Token: 0x040021DB RID: 8667
	public TextMeshProUGUI levelNameText;

	// Token: 0x040021DC RID: 8668
	[Space]
	public Image loadingGraphic01;

	// Token: 0x040021DD RID: 8669
	public Image loadingGraphic02;

	// Token: 0x040021DE RID: 8670
	public Image loadingGraphic03;

	// Token: 0x040021DF RID: 8671
	private Animator animator;

	// Token: 0x040021E0 RID: 8672
	internal bool levelAnimationStarted;

	// Token: 0x040021E1 RID: 8673
	internal bool levelAnimationCompleted;

	// Token: 0x040021E2 RID: 8674
	internal bool debugDisableLevelAnimation;

	// Token: 0x040021E3 RID: 8675
	[Space]
	public Sound soundTurn;

	// Token: 0x040021E4 RID: 8676
	public Sound soundRevUp;

	// Token: 0x040021E5 RID: 8677
	public Sound soundCrash;

	// Token: 0x040021E6 RID: 8678
	public Sound soundtextLevel;

	// Token: 0x040021E7 RID: 8679
	public Sound soundtextName;
}
