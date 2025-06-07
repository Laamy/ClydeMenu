using System;
using TMPro;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class MenuTextInput : MonoBehaviour
{
	// Token: 0x060011C5 RID: 4549 RVA: 0x000A1E8A File Offset: 0x000A008A
	private void Start()
	{
		this.textUI = base.GetComponentInChildren<SemiUI>();
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x000A1E98 File Offset: 0x000A0098
	private void Update()
	{
		this.tooLongDenyCooldown -= Time.deltaTime;
		MenuManager.instance.TextInputActive();
		if (this.textCurrent == "\b")
		{
			this.textCurrent = "";
		}
		this.textCurrent += Input.inputString;
		this.textCurrent = this.textCurrent.Replace("\n", "");
		if (Input.inputString == "\b")
		{
			this.textCurrent = this.textCurrent.Remove(Mathf.Max(this.textCurrent.Length - 2, 0));
		}
		this.textCurrent = this.textCurrent.Replace("\r", "");
		if (this.textCurrent.Length > this.maxLength)
		{
			this.textCurrent = this.textCurrent.Remove(this.maxLength);
			if (this.tooLongDenyCooldown <= 0f)
			{
				this.tooLongDenyCooldown = 0.25f;
				this.textUI.SemiUITextFlashColor(Color.red, 0.2f);
				this.textUI.SemiUISpringShakeX(10f, 10f, 0.3f);
				this.textUI.SemiUISpringScale(0.05f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, true);
			}
		}
		this.InputTextSet();
		if (this.textPrevious != this.textCurrent)
		{
			if (this.textCurrent.Length > this.textPrevious.Length)
			{
				this.textUI.SemiUISpringShakeY(1f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 2f, 0.2f, true);
			}
			else
			{
				this.textUI.SemiUITextFlashColor(Color.red, 0.2f);
				this.textUI.SemiUISpringShakeX(5f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Dud, null, 2f, 1f, true);
			}
		}
		this.textPrevious = this.textCurrent;
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x000A20C0 File Offset: 0x000A02C0
	private void InputTextSet()
	{
		if (this.upperOnly)
		{
			this.textCurrent = this.textCurrent.ToUpper();
		}
		this.textMain.text = this.textCurrent;
		float num = this.textMain.renderedWidth;
		if (num < 0f)
		{
			num = 0f;
		}
		Vector3 position = this.textMain.transform.position + new Vector3(num + 1f, 0f, 0f);
		this.textCursor.transform.position = position;
		if (Mathf.Sin(Time.time * 8f) > 0f)
		{
			this.textCursor.text = "|";
			return;
		}
		this.textCursor.text = "";
	}

	// Token: 0x04001E24 RID: 7716
	public TextMeshProUGUI textMain;

	// Token: 0x04001E25 RID: 7717
	public TextMeshProUGUI textCursor;

	// Token: 0x04001E26 RID: 7718
	private SemiUI textUI;

	// Token: 0x04001E27 RID: 7719
	[Space]
	public bool upperOnly;

	// Token: 0x04001E28 RID: 7720
	public int maxLength = 60;

	// Token: 0x04001E29 RID: 7721
	internal string textCurrent = "";

	// Token: 0x04001E2A RID: 7722
	private string textPrevious = "";

	// Token: 0x04001E2B RID: 7723
	private float tooLongDenyCooldown;
}
