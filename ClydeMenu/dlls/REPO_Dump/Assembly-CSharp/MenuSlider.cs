using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200020C RID: 524
public class MenuSlider : MonoBehaviour
{
	// Token: 0x06001199 RID: 4505 RVA: 0x000A02E4 File Offset: 0x0009E4E4
	public void Start()
	{
		this.inputPercentSetting = base.GetComponent<MenuInputPercentSetting>();
		this.rectTransform = base.GetComponent<RectTransform>();
		if (this.inputPercentSetting)
		{
			this.inputSetting = true;
		}
		this.menuSetting = base.GetComponent<MenuSetting>();
		if (this.menuSetting)
		{
			this.menuSetting.FetchValues();
			int settingValue = this.menuSetting.settingValue;
			if (this.hasCustomOptions)
			{
				int indexFromCustomValue = this.GetIndexFromCustomValue(this.menuSetting.settingValue);
				this.menuSetting.settingValue = indexFromCustomValue;
				settingValue = this.menuSetting.settingValue;
			}
			this.settingsValue = (float)settingValue / 100f;
			this.setting = this.menuSetting.setting;
			this.elementName = this.menuSetting.settingName;
			this.elementNameText.text = this.elementName;
		}
		this.bigSettingText = base.GetComponentInChildren<MenuBigSettingText>();
		if (this.bigSettingText)
		{
			this.hasBigSettingText = true;
		}
		this.prevSettingString = "";
		this.parentPage = base.GetComponentInParent<MenuPage>();
		if (this.elementNameText)
		{
			this.elementNameText.text = this.elementName;
		}
		this.settingSegments = this.endValue - this.startValue;
		this.menuSelectableElement = base.GetComponent<MenuSelectableElement>();
		if (this.hasCustomOptions)
		{
			this.settingSegments = Mathf.Max(this.customOptions.Count - 1, 1);
			this.startValue = 0;
			this.endValue = this.customOptions.Count - 1;
			this.buttonSegmentJump = 1;
			this.settingsValue = this.settingsValue / (float)this.settingSegments * 100f;
		}
		this.barSizeRectTransform = this.barSize.GetComponent<RectTransform>();
		if (this.hasCustomOptions)
		{
			if (Mathf.Max(this.customOptions.Count - 1, 1) != this.settingSegments)
			{
				Debug.LogWarning("Segment text count is not equal to setting segments count");
			}
			else
			{
				int num = Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
				string text = this.customOptions[num].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				this.segmentText.text = text;
			}
		}
		else
		{
			this.currentValue = Mathf.RoundToInt(Mathf.Lerp((float)this.startValue, (float)this.endValue, this.settingsValue));
			this.segmentText.text = this.stringAtStartOfValue + this.currentValue.ToString() + this.stringAtEndOfValue;
		}
		this.segmentText.enableAutoSizing = false;
		this.segmentMaskText.enableAutoSizing = false;
		if (!this.hasBar && this.segmentText)
		{
			Object.Destroy(this.segmentText.gameObject);
		}
		this.SetStartPositions();
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x000A05B8 File Offset: 0x0009E7B8
	public int GetIndexFromCustomValue(int value)
	{
		int result = 0;
		for (int i = 0; i < this.customOptions.Count; i++)
		{
			if (this.customOptions[i].customValueInt == value)
			{
				return i;
			}
		}
		return result;
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x000A05F4 File Offset: 0x0009E7F4
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.elementNameText = base.GetComponentInChildren<TextMeshProUGUI>();
		this.elementNameText.text = this.elementName;
		base.gameObject.name = "Slider - " + this.elementName;
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x000A0644 File Offset: 0x0009E844
	public void SetStartPositions()
	{
		if (this.startPositionSetup)
		{
			return;
		}
		this.startPositionSetup = true;
		this.barSizeRectTransform.localPosition = new Vector3(this.barSizeRectTransform.localPosition.x + this.sneakyOffsetBecauseIWasLazy, this.barSizeRectTransform.localPosition.y, this.barSizeRectTransform.localPosition.z);
		this.originalPosition = this.rectTransform.position;
		this.originalPositionBarBG = this.sliderBG.GetComponent<RectTransform>().position;
		this.originalPositionBarSize = this.barSizeRectTransform.transform.position;
		this.originalPosition = new Vector3(this.originalPosition.x, this.originalPosition.y - 1.01f, this.originalPosition.z);
		this.barBGRectTransform = this.sliderBG.GetComponent<RectTransform>();
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x000A0728 File Offset: 0x0009E928
	public string CustomOptionGetCurrentString()
	{
		return this.customOptions[this.currentValue].customOptionText;
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x000A0740 File Offset: 0x0009E940
	public void CustomOptionAdd(string optionText, UnityEvent onOption)
	{
		this.customOptions.Add(new MenuSlider.CustomOption
		{
			customOptionText = optionText,
			onOption = onOption
		});
		this.settingSegments = Mathf.Max(this.customOptions.Count - 1, 1);
		this.startValue = 0;
		this.endValue = this.customOptions.Count - 1;
		this.buttonSegmentJump = 1;
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x000A07A8 File Offset: 0x0009E9A8
	private void Update()
	{
		if (this.hasBigSettingText && this.prevSettingString != this.segmentText.text)
		{
			int num = Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
			this.bigSettingText.textMeshPro.text = this.customOptions[num].customOptionText;
			this.prevSettingString = this.segmentText.text;
		}
		if (this.prevCurrentValue != this.currentValue || this.valueChangedImpulse)
		{
			this.valueChangedImpulse = false;
			float num2 = Mathf.Round(this.settingsValue * 100f);
			if (this.customOptions.Count > 0)
			{
				num2 = (float)Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
			}
			if (this.menuSetting)
			{
				if (!this.hasCustomOptions)
				{
					DataDirector.instance.SettingValueSet(this.setting, (int)num2);
				}
				else if (this.hasCustomValues)
				{
					MenuSlider.CustomOption customOption = this.customOptions[this.currentValue];
					DataDirector.instance.SettingValueSet(this.setting, customOption.customValueInt);
					this.customOptions[this.currentValue].onOption.Invoke();
				}
				else
				{
					DataDirector.instance.SettingValueSet(this.setting, (int)num2);
				}
			}
			if (this.inputSetting)
			{
				InputManager.instance.inputPercentSettings[this.inputPercentSetting.setting] = (int)num2;
			}
			this.onChange.Invoke();
			this.prevCurrentValue = this.currentValue;
		}
		if (this.extraBarActiveTimer > 0f)
		{
			this.extraBarActiveTimer -= Time.deltaTime;
		}
		else if (this.extraBar.gameObject.activeSelf)
		{
			this.extraBar.gameObject.SetActive(false);
		}
		if (this.hasBar)
		{
			this.settingsBar.localScale = Vector3.Lerp(this.settingsBar.localScale, new Vector3(this.settingsValue, this.settingsBar.localScale.y, this.settingsBar.localScale.z), 20f * Time.deltaTime);
			this.maskRectTransform.sizeDelta = new Vector2(this.barSizeRectTransform.sizeDelta.x * this.settingsValue, this.maskRectTransform.sizeDelta.y);
		}
		Vector3 mousePosition = Input.mousePosition;
		float num3 = (float)(Screen.width / MenuManager.instance.screenUIWidth) * 1.05f;
		float num4 = (float)(Screen.height / MenuManager.instance.screenUIHeight) * 1f;
		mousePosition = new Vector3(mousePosition.x / num3, mousePosition.y / num4, 0f);
		if (SemiFunc.UIMouseHover(this.parentPage, this.barSizeRectTransform, this.menuSelectableElement.menuID, 5f, 5f))
		{
			if (!this.hovering)
			{
				MenuManager.instance.MenuEffectHover(SemiFunc.MenuGetPitchFromYPos(this.rectTransform), -1f);
			}
			this.hovering = true;
			int num5 = 10;
			new Vector3(this.barSizeRectTransform.localPosition.x + this.barSizeRectTransform.sizeDelta.x / 2f - this.sneakyOffsetBecauseIWasLazy, this.barSizeRectTransform.localPosition.y + (float)(num5 / 2), this.barSizeRectTransform.localPosition.z);
			new Vector2(this.barSizeRectTransform.sizeDelta.x + (float)num5, this.barSizeRectTransform.sizeDelta.y + (float)num5);
			SemiFunc.MenuSelectionBoxTargetSet(this.parentPage, this.barSizeRectTransform, new Vector2(-3f, 0f), new Vector2(20f, 10f));
			if (this.hasBar)
			{
				this.PointerLogic(mousePosition);
			}
			else if (Input.GetMouseButtonDown(0))
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, this.parentPage, -1f, -1f, false);
				this.OnIncrease();
			}
		}
		else
		{
			this.hovering = false;
			if (this.barPointer.gameObject.activeSelf)
			{
				this.barPointer.localPosition = new Vector3(-999f, this.barPointer.localPosition.y, this.barPointer.localPosition.z);
				this.barPointer.gameObject.SetActive(false);
			}
		}
		if (this.segmentMaskText && this.segmentMaskText.text != this.segmentText.text)
		{
			this.segmentMaskText.text = this.segmentText.text;
		}
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x000A0C54 File Offset: 0x0009EE54
	public void ExtraBarSet(float value)
	{
		if (!this.extraBar.gameObject.activeSelf)
		{
			this.extraBar.gameObject.SetActive(true);
		}
		value = Mathf.Clamp(value, 0f, 1f);
		this.extraBar.localScale = new Vector3(value, this.extraBar.localScale.y, this.extraBar.localScale.z);
		this.extraBarActiveTimer = 0.2f;
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x000A0CD4 File Offset: 0x0009EED4
	public void SetBar(float value)
	{
		this.settingsValue = Mathf.Clamp(value, 0f, 1f);
		int num = Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
		this.currentValue = Mathf.RoundToInt(Mathf.Lerp((float)this.startValue, (float)this.endValue, this.settingsValue));
		if (this.hasCustomOptions)
		{
			this.customValue = this.GetCustomValue(num);
			if (num < this.customOptions.Count)
			{
				string text = this.customOptions[num].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				this.segmentText.text = text;
				return;
			}
		}
		else
		{
			this.segmentText.text = this.stringAtStartOfValue + this.currentValue.ToString() + this.stringAtEndOfValue;
		}
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x000A0DB8 File Offset: 0x0009EFB8
	public int GetCustomValue(int index)
	{
		if (!this.hasCustomOptions)
		{
			return this.customValueNull;
		}
		if (this.customOptions.Count == 0)
		{
			return this.customValueNull;
		}
		if (index >= this.customOptions.Count)
		{
			return this.customValueNull;
		}
		if (index < 0)
		{
			return this.customValueNull;
		}
		if (!this.hasCustomValues)
		{
			return this.customValueNull;
		}
		return this.customOptions[index].customValueInt;
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x000A0E28 File Offset: 0x0009F028
	private void PointerLogic(Vector3 mouseScreenPosition)
	{
		if (!this.barPointer)
		{
			return;
		}
		if (!this.barPointer.gameObject.activeSelf)
		{
			this.barPointer.gameObject.SetActive(true);
		}
		ref Vector2 ptr = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(this.barSizeRectTransform);
		int num = (this.endValue - this.startValue) / this.pointerSegmentJump;
		SemiFunc.UIGetRectTransformPositionOnScreen(this.barSizeRectTransform, true);
		float num2 = Mathf.Clamp01(ptr.x / this.barSizeRectTransform.sizeDelta.x);
		num2 = Mathf.Round(num2 * (float)num) / (float)num;
		float num3 = Mathf.Clamp(this.barSizeRectTransform.localPosition.x + num2 * this.barSizeRectTransform.sizeDelta.x, this.barSizeRectTransform.localPosition.x, this.barSizeRectTransform.localPosition.x + this.barSizeRectTransform.sizeDelta.x);
		this.barPointer.localPosition = new Vector3(num3 - 2f, this.barPointer.localPosition.y, this.barPointer.localPosition.z);
		if (Input.GetMouseButton(0))
		{
			this.prevSettingsValue = this.settingsValue;
			this.settingsValue = num2;
			if (this.prevSettingsValue != this.settingsValue)
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, this.parentPage, -1f, -1f, false);
			}
			int num4 = Mathf.RoundToInt(this.settingsValue * (float)num);
			if (this.hasCustomOptions && num4 < this.customOptions.Count)
			{
				this.segmentText.text = this.customOptions[num4].customOptionText;
			}
			else
			{
				this.segmentText.text = this.stringAtStartOfValue + this.currentValue.ToString() + this.stringAtEndOfValue;
			}
			this.currentValue = Mathf.RoundToInt(Mathf.Lerp((float)this.startValue, (float)this.endValue, this.settingsValue));
			if (this.hasCustomOptions)
			{
				this.UpdateSegmentTextAndValue();
				this.customValue = this.GetCustomValue(num4);
			}
		}
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x000A1040 File Offset: 0x0009F240
	public void UpdateSegmentTextAndValue()
	{
		int num = Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
		this.currentValue = Mathf.RoundToInt(Mathf.Lerp((float)this.startValue, (float)this.endValue, this.settingsValue));
		if (this.hasCustomOptions)
		{
			this.customValue = this.GetCustomValue(num);
			if (num < this.customOptions.Count)
			{
				string text = this.customOptions[num].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				this.segmentText.text = text;
				return;
			}
		}
		else
		{
			this.segmentText.text = this.stringAtStartOfValue + this.currentValue.ToString() + this.stringAtEndOfValue;
		}
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x000A110C File Offset: 0x0009F30C
	public void OnIncrease()
	{
		this.valueChangedImpulse = true;
		this.prevSettingsValue = this.settingsValue;
		float num = this.settingsValue;
		this.settingsValue += 1f / (float)this.settingSegments * (float)this.buttonSegmentJump;
		if (this.wrapAround)
		{
			this.settingsValue = ((num == 1f) ? 0f : Mathf.Clamp01(this.settingsValue));
		}
		else
		{
			this.settingsValue = Mathf.Clamp(this.settingsValue, 0f, 1f);
		}
		this.UpdateSegmentTextAndValue();
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x000A11A0 File Offset: 0x0009F3A0
	public void OnDecrease()
	{
		this.valueChangedImpulse = true;
		this.prevSettingsValue = this.settingsValue;
		float num = this.settingsValue;
		this.settingsValue -= 1f / (float)this.settingSegments * (float)this.buttonSegmentJump;
		if (this.wrapAround)
		{
			this.settingsValue = ((this.settingsValue + num < 0f) ? 1f : Mathf.Clamp01(this.settingsValue));
		}
		else
		{
			this.settingsValue = Mathf.Clamp(this.settingsValue, 0f, 1f);
		}
		this.UpdateSegmentTextAndValue();
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x000A123B File Offset: 0x0009F43B
	public void SetBarScaleInstant()
	{
		this.settingsBar.localScale = new Vector3(this.settingsValue, this.settingsBar.localScale.y, this.settingsBar.localScale.z);
	}

	// Token: 0x04001DCC RID: 7628
	public string elementName = "Element Name";

	// Token: 0x04001DCD RID: 7629
	public TextMeshProUGUI elementNameText;

	// Token: 0x04001DCE RID: 7630
	public Transform sliderBG;

	// Token: 0x04001DCF RID: 7631
	public Transform barSize;

	// Token: 0x04001DD0 RID: 7632
	public Transform barPointer;

	// Token: 0x04001DD1 RID: 7633
	public RectTransform barSizeRectTransform;

	// Token: 0x04001DD2 RID: 7634
	public Transform settingsBar;

	// Token: 0x04001DD3 RID: 7635
	public Transform extraBar;

	// Token: 0x04001DD4 RID: 7636
	private int settingSegments;

	// Token: 0x04001DD5 RID: 7637
	public int startValue;

	// Token: 0x04001DD6 RID: 7638
	public int endValue;

	// Token: 0x04001DD7 RID: 7639
	public string stringAtStartOfValue;

	// Token: 0x04001DD8 RID: 7640
	public string stringAtEndOfValue;

	// Token: 0x04001DD9 RID: 7641
	internal int currentValue;

	// Token: 0x04001DDA RID: 7642
	internal int prevCurrentValue;

	// Token: 0x04001DDB RID: 7643
	internal bool valueChangedImpulse;

	// Token: 0x04001DDC RID: 7644
	public int buttonSegmentJump = 1;

	// Token: 0x04001DDD RID: 7645
	public int pointerSegmentJump = 1;

	// Token: 0x04001DDE RID: 7646
	internal float settingsValue = 1f;

	// Token: 0x04001DDF RID: 7647
	internal float prevSettingsValue = 1f;

	// Token: 0x04001DE0 RID: 7648
	public TextMeshProUGUI segmentText;

	// Token: 0x04001DE1 RID: 7649
	public TextMeshProUGUI segmentMaskText;

	// Token: 0x04001DE2 RID: 7650
	public RectTransform maskRectTransform;

	// Token: 0x04001DE3 RID: 7651
	public bool wrapAround;

	// Token: 0x04001DE4 RID: 7652
	public bool hasBar = true;

	// Token: 0x04001DE5 RID: 7653
	public bool hasCustomOptions;

	// Token: 0x04001DE6 RID: 7654
	private MenuSelectableElement menuSelectableElement;

	// Token: 0x04001DE7 RID: 7655
	private bool hovering;

	// Token: 0x04001DE8 RID: 7656
	private RectTransform rectTransform;

	// Token: 0x04001DE9 RID: 7657
	private float sneakyOffsetBecauseIWasLazy = 3f;

	// Token: 0x04001DEA RID: 7658
	private MenuPage parentPage;

	// Token: 0x04001DEB RID: 7659
	internal MenuSetting menuSetting;

	// Token: 0x04001DEC RID: 7660
	private DataDirector.Setting setting;

	// Token: 0x04001DED RID: 7661
	private bool inputSetting;

	// Token: 0x04001DEE RID: 7662
	private MenuInputPercentSetting inputPercentSetting;

	// Token: 0x04001DEF RID: 7663
	private Vector3 originalPosition;

	// Token: 0x04001DF0 RID: 7664
	private Vector3 originalPositionBarSize;

	// Token: 0x04001DF1 RID: 7665
	private Vector3 originalPositionBarBG;

	// Token: 0x04001DF2 RID: 7666
	private RectTransform barBGRectTransform;

	// Token: 0x04001DF3 RID: 7667
	private string prevSettingString = "";

	// Token: 0x04001DF4 RID: 7668
	private bool hasBigSettingText;

	// Token: 0x04001DF5 RID: 7669
	internal MenuBigSettingText bigSettingText;

	// Token: 0x04001DF6 RID: 7670
	private int customValue;

	// Token: 0x04001DF7 RID: 7671
	private int customValueNull = -123456;

	// Token: 0x04001DF8 RID: 7672
	public bool hasCustomValues;

	// Token: 0x04001DF9 RID: 7673
	private bool startPositionSetup;

	// Token: 0x04001DFA RID: 7674
	[Space]
	public UnityEvent onChange;

	// Token: 0x04001DFB RID: 7675
	public List<MenuSlider.CustomOption> customOptions;

	// Token: 0x04001DFC RID: 7676
	private float extraBarActiveTimer;

	// Token: 0x020003E3 RID: 995
	[Serializable]
	public class CustomOption
	{
		// Token: 0x04002CB3 RID: 11443
		[Space(25f)]
		[Header("____ Custom Option ____")]
		public string customOptionText;

		// Token: 0x04002CB4 RID: 11444
		public UnityEvent onOption;

		// Token: 0x04002CB5 RID: 11445
		public int customValueInt;
	}
}
