using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200020E RID: 526
public class MenuTwoOptions : MonoBehaviour
{
	// Token: 0x060011AD RID: 4525 RVA: 0x000A15E8 File Offset: 0x0009F7E8
	private void Start()
	{
		if (this.option1TextMesh)
		{
			this.option1TextMesh.text = this.option1Text;
		}
		if (this.option1TextMesh)
		{
			this.option2TextMesh.text = this.option2Text;
		}
		this.StartFetch();
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x000A1638 File Offset: 0x0009F838
	private void StartFetch()
	{
		if (this.customEvents && this.customFetch)
		{
			this.fetchSetting.Invoke();
		}
		else
		{
			bool flag = DataDirector.instance.SettingValueFetch(this.setting) == 1;
			this.startSettingFetch = flag;
		}
		if (this.startSettingFetch)
		{
			this.OnOption1();
		}
		else
		{
			this.OnOption2();
		}
		this.fetchComplete = true;
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x000A169C File Offset: 0x0009F89C
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		if (this.option1TextMesh)
		{
			this.option1TextMesh.text = this.option1Text;
		}
		if (this.option1TextMesh)
		{
			this.option2TextMesh.text = this.option2Text;
		}
		TextMeshProUGUI componentInChildren = base.GetComponentInChildren<TextMeshProUGUI>();
		if (componentInChildren)
		{
			componentInChildren.text = this.settingName;
		}
		base.gameObject.name = "Bool Setting - " + this.settingName;
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x000A1723 File Offset: 0x0009F923
	private void OnEnable()
	{
		this.StartFetch();
	}

	// Token: 0x060011B1 RID: 4529 RVA: 0x000A172C File Offset: 0x0009F92C
	private void Update()
	{
		if (!this.optionsBox)
		{
			return;
		}
		this.optionsBox.localPosition = Vector3.Lerp(this.optionsBox.localPosition, this.targetPosition, 20f * Time.deltaTime);
		this.optionsBox.localScale = Vector3.Lerp(this.optionsBox.localScale, this.targetScale / 10f, 20f * Time.deltaTime);
		this.optionsBoxBehind.localPosition = Vector3.Lerp(this.optionsBoxBehind.localPosition, this.targetPosition, 20f * Time.deltaTime);
		this.optionsBoxBehind.localScale = Vector3.Lerp(this.optionsBoxBehind.localScale, new Vector3(this.targetScale.x + 4f, this.targetScale.y + 2f, 1f) / 10f, 20f * Time.deltaTime);
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x000A1831 File Offset: 0x0009FA31
	public void SetTarget(Vector3 pos, Vector3 scale)
	{
		this.targetPosition = pos;
		this.targetScale = scale;
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x000A1844 File Offset: 0x0009FA44
	public void OnOption1()
	{
		this.SetTarget(new Vector3(37.8f, 12.3f, 0f), new Vector3(73f, 22f, 1f));
		if (this.fetchComplete)
		{
			if (this.customEvents)
			{
				if (this.settingSet)
				{
					DataDirector.instance.SettingValueSet(this.setting, 1);
				}
				this.onOption1.Invoke();
				return;
			}
			DataDirector.instance.SettingValueSet(this.setting, 1);
		}
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x000A18C8 File Offset: 0x0009FAC8
	public void OnOption2()
	{
		this.SetTarget(new Vector3(112.644f, 12.3f, 0f), new Vector3(74f, 22f, 1f));
		if (this.fetchComplete)
		{
			if (this.customEvents)
			{
				if (this.settingSet)
				{
					DataDirector.instance.SettingValueSet(this.setting, 0);
				}
				this.onOption2.Invoke();
				return;
			}
			DataDirector.instance.SettingValueSet(this.setting, 0);
		}
	}

	// Token: 0x04001E01 RID: 7681
	public string option1Text = "ON";

	// Token: 0x04001E02 RID: 7682
	public string option2Text = "OFF";

	// Token: 0x04001E03 RID: 7683
	public RectTransform optionsBox;

	// Token: 0x04001E04 RID: 7684
	public RectTransform optionsBoxBehind;

	// Token: 0x04001E05 RID: 7685
	public Vector3 targetPosition;

	// Token: 0x04001E06 RID: 7686
	public Vector3 targetScale;

	// Token: 0x04001E07 RID: 7687
	public DataDirector.Setting setting;

	// Token: 0x04001E08 RID: 7688
	public bool customEvents = true;

	// Token: 0x04001E09 RID: 7689
	public bool settingSet;

	// Token: 0x04001E0A RID: 7690
	public bool customFetch = true;

	// Token: 0x04001E0B RID: 7691
	public UnityEvent onOption1;

	// Token: 0x04001E0C RID: 7692
	public UnityEvent onOption2;

	// Token: 0x04001E0D RID: 7693
	public UnityEvent fetchSetting;

	// Token: 0x04001E0E RID: 7694
	public TextMeshProUGUI option1TextMesh;

	// Token: 0x04001E0F RID: 7695
	public TextMeshProUGUI option2TextMesh;

	// Token: 0x04001E10 RID: 7696
	public bool startSettingFetch = true;

	// Token: 0x04001E11 RID: 7697
	private bool fetchComplete;

	// Token: 0x04001E12 RID: 7698
	public string settingName;
}
