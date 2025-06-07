using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001F7 RID: 503
public class MenuButton : MonoBehaviour
{
	// Token: 0x06001101 RID: 4353 RVA: 0x0009C40C File Offset: 0x0009A60C
	private void Awake()
	{
		if (!this.customColors)
		{
			this.colorNormal = Color.gray;
			this.colorHover = Color.white;
			this.colorClick = AssetManager.instance.colorYellow;
		}
		this.menuButtonPopUp = base.GetComponent<MenuButtonPopUp>();
		this.menuSelectableElement = base.GetComponent<MenuSelectableElement>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.rectTransform = base.GetComponent<RectTransform>();
		this.button = base.GetComponent<Button>();
		this.buttonText = base.GetComponentInChildren<TextMeshProUGUI>();
		this.buttonTextSelectedOriginalPos = this.buttonText.transform.localPosition;
		if (this.buttonTextString != "BUTTON")
		{
			this.buttonText.text = this.buttonTextString;
		}
		this.originalText = this.buttonText.text;
		Vector2 sizeDelta = this.rectTransform.sizeDelta;
		this.buttonPitch = SemiFunc.MenuGetPitchFromYPos(this.rectTransform);
		float fontSize = this.buttonText.fontSize;
		this.buttonText.fontSize = fontSize;
		this.buttonText.enableAutoSizing = false;
		TextAlignmentOptions alignment = this.buttonText.alignment;
		this.buttonText.alignment = TextAlignmentOptions.MidlineLeft;
		this.buttonPadding = 0f;
		Vector2 sizeDelta2 = this.rectTransform.sizeDelta;
		this.rectTransform.sizeDelta = new Vector2(this.buttonText.GetPreferredValues(this.originalText, 0f, 0f).x + this.buttonPadding, this.buttonText.GetPreferredValues(this.originalText, 0f, 0f).y + this.buttonPadding / 2f);
		this.buttonText.alignment = alignment;
		if (alignment == TextAlignmentOptions.Midline)
		{
			this.buttonText.enableAutoSizing = true;
		}
		if (this.middleAlignFix)
		{
			this.rectTransform.position += new Vector3((sizeDelta2.x - this.rectTransform.sizeDelta.x) / 2f, 0f, 0f);
			this.buttonText.enableAutoSizing = false;
		}
		if (this.customHoverArea)
		{
			this.rectTransform.sizeDelta = sizeDelta;
		}
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x0009C63C File Offset: 0x0009A83C
	private void Update()
	{
		this.button.image.color = new Color(0f, 0f, 0f, 0f);
		this.HoverLogic();
		switch (this.buttonState)
		{
		case 0:
			this.<Update>g__ButtonHover|34_1();
			this.buttonStateStart = false;
			break;
		case 1:
			this.<Update>g__ButtonClicked|34_2();
			this.buttonStateStart = false;
			break;
		case 2:
			this.<Update>g__ButtonNormal|34_0();
			this.buttonStateStart = false;
			break;
		}
		if (this.hoverTimer > 0f)
		{
			this.hoverTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x0009C6E0 File Offset: 0x0009A8E0
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		if (this.buttonTextString != "BUTTON")
		{
			this.buttonText = base.GetComponentInChildren<TextMeshProUGUI>();
			if (this.buttonText.text != this.buttonTextString)
			{
				this.buttonText.text = this.buttonTextString;
			}
			if (base.gameObject.name != "Menu Button - " + this.buttonTextString)
			{
				base.gameObject.name = "Menu Button - " + this.buttonTextString;
			}
		}
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x0009C77C File Offset: 0x0009A97C
	private void HoverLogic()
	{
		int num = 0;
		if (!this.customHoverArea)
		{
			num = 10;
		}
		if (SemiFunc.UIMouseHover(this.parentPage, this.rectTransform, this.menuSelectableElement.menuID, (float)num, 0f))
		{
			if (!this.hovering)
			{
				this.OnHoverStart();
				this.hovering = true;
			}
			this.hoverTimer = 0.01f;
		}
		if (this.hovering || (this.clicked && this.hovering))
		{
			if (Input.GetMouseButtonDown(0) && this.clickCooldown <= 0f)
			{
				this.OnSelect();
				this.holdTimer = 0f;
				this.clickTimer = 0.2f;
				if (!this.hasHold)
				{
					this.clickCooldown = 0.25f;
				}
			}
			if (this.hasHold)
			{
				if (Input.GetMouseButton(0))
				{
					this.holdTimer += Time.deltaTime;
				}
				else
				{
					this.holdTimer = 0f;
					this.clickFrequencyTicker = 0f;
					this.clickFrequency = 0.2f;
				}
			}
		}
		this.clickCooldown -= Time.deltaTime;
		if (this.clickTimer > 0f)
		{
			this.clickTimer -= Time.deltaTime;
			this.clicked = true;
		}
		else
		{
			if (this.clicked)
			{
				this.OnSelectEnd();
			}
			this.clicked = false;
		}
		if (this.hoverTimer <= 0f)
		{
			if (this.hovering)
			{
				this.OnHoverEnd();
			}
			this.hovering = false;
		}
		if (this.hoverTimer > 0f)
		{
			this.OnHovering();
			this.hovering = true;
		}
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x0009C90D File Offset: 0x0009AB0D
	private void ButtonStateSet(int state)
	{
		this.buttonState = state;
		this.buttonStateStart = true;
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x0009C91D File Offset: 0x0009AB1D
	private void OnHoverStart()
	{
		this.ButtonStateSet(0);
		this.buttonStateStart = true;
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x0009C930 File Offset: 0x0009AB30
	public void OnHovering()
	{
		this.buttonText.transform.localPosition = new Vector3(this.buttonTextSelectedOriginalPos.x, this.buttonTextSelectedOriginalPos.y + this.buttonTextSelectedScootPos, this.buttonTextSelectedOriginalPos.z);
		Vector2 sizeDelta = this.rectTransform.sizeDelta;
		new Vector3(sizeDelta.x / 2f, sizeDelta.y / 2f, 0f) + (base.transform.localPosition - new Vector3(this.buttonPadding / 2f, 0f, 0f));
		SemiFunc.MenuSelectionBoxTargetSet(this.parentPage, this.rectTransform, default(Vector2), default(Vector2));
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x0009C9FC File Offset: 0x0009ABFC
	private void OnHoverEnd()
	{
		this.ButtonStateSet(2);
		this.buttonStateStart = true;
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x0009CA0C File Offset: 0x0009AC0C
	private void OnSelect()
	{
		if (this.disabled)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, -1f, -1f, false);
			return;
		}
		if (this.doButtonEffect)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, this.parentPage, -1f, -1f, false);
		}
		this.ButtonStateSet(1);
		if (!this.menuButtonPopUp)
		{
			this.button.onClick.Invoke();
			return;
		}
		MenuManager.instance.PagePopUpTwoOptions(this.menuButtonPopUp, this.menuButtonPopUp.headerText, this.menuButtonPopUp.headerColor, this.menuButtonPopUp.bodyText, this.menuButtonPopUp.option1Text, this.menuButtonPopUp.option2Text, this.menuButtonPopUp.richText);
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x0009CAD4 File Offset: 0x0009ACD4
	private void OnSelectEnd()
	{
		if (!this.hovering)
		{
			this.ButtonStateSet(2);
			return;
		}
		this.ButtonStateSet(0);
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x0009CAED File Offset: 0x0009ACED
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.OnHoverStart();
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x0009CAF5 File Offset: 0x0009ACF5
	public void OnPointerExit(PointerEventData eventData)
	{
		this.OnHoverEnd();
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x0009CAFD File Offset: 0x0009ACFD
	public void OnPointerClick(PointerEventData eventData)
	{
		this.OnSelect();
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x0009CB08 File Offset: 0x0009AD08
	private void HoldTimer()
	{
		if (!this.holdLogic)
		{
			return;
		}
		if (this.holdTimer > 0.5f)
		{
			if (this.clickFrequencyTicker <= 0f)
			{
				this.OnSelect();
				this.clickFrequencyTicker = this.clickFrequency;
				this.clickFrequency -= this.clickFrequency * 0.2f;
				this.clickFrequency = Mathf.Clamp(this.clickFrequency, 0.025f, 0.2f);
				return;
			}
			this.clickFrequencyTicker -= Time.deltaTime;
		}
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x0009CBE8 File Offset: 0x0009ADE8
	[CompilerGenerated]
	private void <Update>g__ButtonNormal|34_0()
	{
		bool flag = this.buttonStateStart;
		this.holdTimer = 0f;
		this.buttonText.transform.localPosition = this.buttonTextSelectedOriginalPos;
		this.buttonText.color = this.colorNormal;
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x0009CC23 File Offset: 0x0009AE23
	[CompilerGenerated]
	private void <Update>g__ButtonHover|34_1()
	{
		if (this.buttonStateStart)
		{
			MenuManager.instance.MenuEffectHover(this.buttonPitch, -1f);
		}
		this.HoldTimer();
		this.buttonText.color = this.colorHover;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x0009CC59 File Offset: 0x0009AE59
	[CompilerGenerated]
	private void <Update>g__ButtonClicked|34_2()
	{
		bool flag = this.buttonStateStart;
		this.HoldTimer();
		this.buttonText.color = this.colorClick;
	}

	// Token: 0x04001CDE RID: 7390
	public string buttonTextString = "BUTTON";

	// Token: 0x04001CDF RID: 7391
	internal TextMeshProUGUI buttonText;

	// Token: 0x04001CE0 RID: 7392
	public bool customHoverArea;

	// Token: 0x04001CE1 RID: 7393
	public bool doButtonEffect = true;

	// Token: 0x04001CE2 RID: 7394
	public bool holdLogic = true;

	// Token: 0x04001CE3 RID: 7395
	private Button button;

	// Token: 0x04001CE4 RID: 7396
	internal bool hovering;

	// Token: 0x04001CE5 RID: 7397
	private float hoverTimer;

	// Token: 0x04001CE6 RID: 7398
	private float clickTimer;

	// Token: 0x04001CE7 RID: 7399
	private float clickCooldown;

	// Token: 0x04001CE8 RID: 7400
	internal bool clicked;

	// Token: 0x04001CE9 RID: 7401
	private float buttonPitch = 1f;

	// Token: 0x04001CEA RID: 7402
	private string originalText;

	// Token: 0x04001CEB RID: 7403
	private RectTransform rectTransform;

	// Token: 0x04001CEC RID: 7404
	public bool hasHold;

	// Token: 0x04001CED RID: 7405
	private float holdTimer;

	// Token: 0x04001CEE RID: 7406
	private float clickFrequency = 0.2f;

	// Token: 0x04001CEF RID: 7407
	private float clickFrequencyTicker;

	// Token: 0x04001CF0 RID: 7408
	private MenuSelectableElement menuSelectableElement;

	// Token: 0x04001CF1 RID: 7409
	private float buttonPadding;

	// Token: 0x04001CF2 RID: 7410
	private MenuPage parentPage;

	// Token: 0x04001CF3 RID: 7411
	private MenuButtonPopUp menuButtonPopUp;

	// Token: 0x04001CF4 RID: 7412
	public bool middleAlignFix;

	// Token: 0x04001CF5 RID: 7413
	public bool customColors;

	// Token: 0x04001CF6 RID: 7414
	[Header("Custom Colors")]
	public Color colorNormal;

	// Token: 0x04001CF7 RID: 7415
	public Color colorHover;

	// Token: 0x04001CF8 RID: 7416
	public Color colorClick;

	// Token: 0x04001CF9 RID: 7417
	private int buttonState = 2;

	// Token: 0x04001CFA RID: 7418
	private bool buttonStateStart;

	// Token: 0x04001CFB RID: 7419
	private float buttonTextSelectedScootPos = 1f;

	// Token: 0x04001CFC RID: 7420
	private Vector3 buttonTextSelectedOriginalPos;

	// Token: 0x04001CFD RID: 7421
	internal bool disabled;

	// Token: 0x020003D5 RID: 981
	private enum ButtonState
	{
		// Token: 0x04002C8C RID: 11404
		Hover,
		// Token: 0x04002C8D RID: 11405
		Clicked,
		// Token: 0x04002C8E RID: 11406
		Normal
	}
}
