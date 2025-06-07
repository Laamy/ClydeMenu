using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000228 RID: 552
public class MenuElementServer : MonoBehaviour
{
	// Token: 0x06001249 RID: 4681 RVA: 0x000A52CF File Offset: 0x000A34CF
	private void Awake()
	{
		this.menuElementHover = base.GetComponent<MenuElementHover>();
		this.initialFadeAlpha = this.fadePanel.color.a;
		this.menuButtonPopUp = base.GetComponent<MenuButtonPopUp>();
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x000A5300 File Offset: 0x000A3500
	private void Update()
	{
		this.UpdateIntro();
		if (this.menuElementHover.isHovering)
		{
			Color color = this.fadePanel.color;
			color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 10f);
			this.fadePanel.color = color;
			if (SemiFunc.InputDown(InputKey.Confirm) || Input.GetMouseButtonDown(0))
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, -1f, -1f, false);
				MenuButtonPopUp component = base.GetComponent<MenuButtonPopUp>();
				MenuManager.instance.PagePopUpTwoOptions(component, component.headerText, component.headerColor, component.bodyText, component.option1Text, component.option2Text, component.richText);
				return;
			}
		}
		else
		{
			Color color2 = this.fadePanel.color;
			color2.a = Mathf.Lerp(color2.a, this.initialFadeAlpha, Time.deltaTime * 10f);
			this.fadePanel.color = color2;
		}
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x000A53FC File Offset: 0x000A35FC
	private void UpdateIntro()
	{
		if (!this.introDone)
		{
			if (this.introType == MenuElementServer.IntroType.Vertical)
			{
				this.introLerp += Time.deltaTime * 5f;
				this.animationTransform.anchoredPosition = new Vector3(0f, this.introCurve.Evaluate(this.introLerp) * 8f, 0f);
			}
			else if (this.introType == MenuElementServer.IntroType.Right)
			{
				this.introLerp += Time.deltaTime * 5f;
				this.animationTransform.anchoredPosition = new Vector3(this.introCurve.Evaluate(this.introLerp) * 30f, 0f, 0f);
			}
			else if (this.introType == MenuElementServer.IntroType.Left)
			{
				this.introLerp += Time.deltaTime * 5f;
				this.animationTransform.anchoredPosition = new Vector3(-this.introCurve.Evaluate(this.introLerp) * 30f, 0f, 0f);
			}
			if (this.introLerp > 1f)
			{
				this.introDone = true;
			}
		}
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x000A5534 File Offset: 0x000A3734
	public void OnButton()
	{
		DataDirector.instance.networkJoinServerName = this.roomName;
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, 1f, 1f, false);
		RunManager.instance.ResetProgress();
		StatsManager.instance.saveFileCurrent = "";
		GameManager.instance.SetConnectRandom(true);
		GameManager.instance.localTest = false;
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.LobbyMenu);
		RunManager.instance.lobbyJoin = true;
	}

	// Token: 0x04001EC9 RID: 7881
	internal MenuElementServer.IntroType introType;

	// Token: 0x04001ECA RID: 7882
	internal string roomName;

	// Token: 0x04001ECB RID: 7883
	internal MenuButtonPopUp menuButtonPopUp;

	// Token: 0x04001ECC RID: 7884
	public Image fadePanel;

	// Token: 0x04001ECD RID: 7885
	private MenuElementHover menuElementHover;

	// Token: 0x04001ECE RID: 7886
	private float initialFadeAlpha;

	// Token: 0x04001ECF RID: 7887
	public TextMeshProUGUI textName;

	// Token: 0x04001ED0 RID: 7888
	public TextMeshProUGUI textPlayers;

	// Token: 0x04001ED1 RID: 7889
	[Space]
	public RectTransform animationTransform;

	// Token: 0x04001ED2 RID: 7890
	public AnimationCurve introCurve;

	// Token: 0x04001ED3 RID: 7891
	private float introLerp;

	// Token: 0x04001ED4 RID: 7892
	private bool introDone;

	// Token: 0x020003EB RID: 1003
	public enum IntroType
	{
		// Token: 0x04002CCC RID: 11468
		Vertical,
		// Token: 0x04002CCD RID: 11469
		Right,
		// Token: 0x04002CCE RID: 11470
		Left
	}
}
