using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000229 RID: 553
public class MenuPagePassword : MonoBehaviour
{
	// Token: 0x0600124E RID: 4686 RVA: 0x000A55B7 File Offset: 0x000A37B7
	private void Start()
	{
		this.menuPage = base.GetComponent<MenuPage>();
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x000A55C8 File Offset: 0x000A37C8
	private void Update()
	{
		this.showUI.hideTimer = 0f;
		this.copyUI.hideTimer = 0f;
		this.tooLongDenyCooldown -= Time.deltaTime;
		MenuManager.instance.TextInputActive();
		if (this.password == "\b")
		{
			this.password = "";
		}
		this.password += Input.inputString;
		this.password = this.password.Replace("\n", "");
		this.password = this.password.Replace(" ", "");
		this.password = this.password.ToUpper();
		if (Input.inputString == "\b")
		{
			this.password = this.password.Remove(Mathf.Max(this.password.Length - 2, 0));
		}
		this.password = this.password.Replace("\r", "");
		if (this.password.Length > 10)
		{
			this.password = this.passwordPrev;
			if (this.tooLongDenyCooldown <= 0f)
			{
				this.tooLongDenyCooldown = 0.25f;
				this.passwordSemiUI.SemiUITextFlashColor(Color.red, 0.2f);
				this.passwordSemiUI.SemiUISpringShakeX(10f, 10f, 0.3f);
				this.passwordSemiUI.SemiUISpringScale(0.05f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, true);
			}
		}
		this.PasswordTextSet();
		if (this.passwordPrev != this.password)
		{
			if (this.password.Length > this.passwordPrev.Length)
			{
				this.passwordSemiUI.SemiUITextFlashColor(Color.yellow, 0.1f);
				this.passwordSemiUI.SemiUISpringShakeY(2f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 2f, 0.2f, true);
			}
			else
			{
				this.passwordSemiUI.SemiUITextFlashColor(Color.red, 0.2f);
				this.passwordSemiUI.SemiUISpringShakeX(5f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Dud, null, 2f, 1f, true);
			}
		}
		this.passwordPrev = this.password;
		if (SemiFunc.InputDown(InputKey.Confirm))
		{
			this.ConfirmButton();
		}
		if (!this.confirmButtonState)
		{
			if (this.password.Length > 0)
			{
				this.confirmButton.buttonTextString = "Confirm";
				this.confirmButton.buttonText.text = this.confirmButton.buttonTextString;
				this.confirmButtonAnimationLerp = 0f;
				this.confirmButtonState = true;
			}
		}
		else if (this.password.Length <= 0)
		{
			this.confirmButton.buttonTextString = "Skip";
			this.confirmButton.buttonText.text = this.confirmButton.buttonTextString;
			this.confirmButtonAnimationLerp = 0f;
			this.confirmButtonState = false;
		}
		if (this.confirmButtonAnimationLerp < 1f)
		{
			this.confirmButtonAnimationLerp += Time.deltaTime * 5f;
			this.confirmButtonAnimationLerp = Mathf.Clamp01(this.confirmButtonAnimationLerp);
			this.confirmButtonAnimationTransform.anchoredPosition = new Vector3(0f, this.confirmButtonAnimationCurve.Evaluate(this.confirmButtonAnimationLerp) * 5f, 0f);
		}
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x000A5960 File Offset: 0x000A3B60
	private void PasswordTextSet()
	{
		if (this.showing)
		{
			this.passwordText.text = this.password;
		}
		else
		{
			string text = "";
			for (int i = 0; i < this.password.Length; i++)
			{
				text += "*";
			}
			this.passwordText.text = text;
		}
		float num = this.passwordText.renderedWidth;
		if (num < 0f)
		{
			num = 0f;
		}
		Vector3 position = this.passwordText.transform.position + new Vector3(num + 1f, 1f, 0f);
		this.passwordCursor.transform.position = position;
		if (Mathf.Sin(Time.time * 8f) > 0f)
		{
			this.passwordCursor.text = "|";
			return;
		}
		this.passwordCursor.text = "";
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x000A5A4C File Offset: 0x000A3C4C
	public void ToggleShowButton()
	{
		this.passwordSemiUI.SemiUITextFlashColor(Color.white, 0.1f);
		this.passwordSemiUI.SemiUISpringShakeY(2f, 5f, 0.2f);
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, 2f, 0.5f, false);
		this.showing = !this.showing;
		if (this.showing)
		{
			this.passwordText.alignment = TextAlignmentOptions.Left;
			this.showImage.texture = this.showIconOff.texture;
		}
		else
		{
			this.passwordText.alignment = TextAlignmentOptions.MidlineLeft;
			this.showImage.texture = this.showIconOn.texture;
		}
		this.showUI.SemiUISpringShakeY(2f, 5f, 0.2f);
		this.PasswordTextSet();
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x000A5B24 File Offset: 0x000A3D24
	public void CopyButton()
	{
		this.copyUI.SemiUISpringShakeY(2f, 5f, 0.2f);
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, 2f, 0.5f, false);
		GUIUtility.systemCopyBuffer = this.password;
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x000A5B62 File Offset: 0x000A3D62
	public void ConfirmButton()
	{
		MenuManager.instance.PageReactivatePageUnderThisPage(this.menuPage);
		MenuManager.instance.MenuEffectPopUpClose();
		this.menuPage.PageStateSet(MenuPage.PageState.Closing);
		DataDirector.instance.networkPassword = this.password;
	}

	// Token: 0x04001ED5 RID: 7893
	internal MenuPage menuPage;

	// Token: 0x04001ED6 RID: 7894
	public TextMeshProUGUI passwordText;

	// Token: 0x04001ED7 RID: 7895
	public TextMeshProUGUI passwordCursor;

	// Token: 0x04001ED8 RID: 7896
	public SemiUI passwordSemiUI;

	// Token: 0x04001ED9 RID: 7897
	public MenuButton confirmButton;

	// Token: 0x04001EDA RID: 7898
	public RectTransform confirmButtonAnimationTransform;

	// Token: 0x04001EDB RID: 7899
	public AnimationCurve confirmButtonAnimationCurve;

	// Token: 0x04001EDC RID: 7900
	private float confirmButtonAnimationLerp = 1f;

	// Token: 0x04001EDD RID: 7901
	private bool confirmButtonState;

	// Token: 0x04001EDE RID: 7902
	private string password = "";

	// Token: 0x04001EDF RID: 7903
	private string passwordPrev = "";

	// Token: 0x04001EE0 RID: 7904
	private float tooLongDenyCooldown;

	// Token: 0x04001EE1 RID: 7905
	private bool showing;

	// Token: 0x04001EE2 RID: 7906
	[Space]
	public Sprite showIconOn;

	// Token: 0x04001EE3 RID: 7907
	public Sprite showIconOff;

	// Token: 0x04001EE4 RID: 7908
	public RawImage showImage;

	// Token: 0x04001EE5 RID: 7909
	[Space]
	public SemiUI showUI;

	// Token: 0x04001EE6 RID: 7910
	public SemiUI copyUI;
}
