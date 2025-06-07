using System;
using TMPro;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class TuckScreenLocked : MonoBehaviour
{
	// Token: 0x060008B1 RID: 2225 RVA: 0x00053DAC File Offset: 0x00051FAC
	private void Update()
	{
		new Color(1f, 0.2f, 0f);
		this.isLocked = true;
		if (!this.isLocked)
		{
			this.text.text = this.lockedText;
			return;
		}
		this.cycleTimer += Time.deltaTime;
		if (this.cycleTimer >= this.cycleInterval)
		{
			this.cycleTimer = 0f;
			this.textIndex++;
			if (this.textIndex >= this.textPhases.Length)
			{
				this.textIndex = 0;
			}
		}
		if (this.textIndex == -1)
		{
			this.text.text = this.lockedText + this.textPhases[0];
			return;
		}
		this.text.text = this.lockedText + this.textPhases[this.textIndex];
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00053E90 File Offset: 0x00052090
	public void LockChatToggle(bool _lock, string _lockedText = "", Color _lightColor = default(Color), Color _darkColor = default(Color))
	{
		this.isLocked = _lock;
		this.text.color = _lightColor;
		string text = ColorUtility.ToHtmlStringRGB(_darkColor);
		string text2 = ColorUtility.ToHtmlStringRGB(_lightColor);
		this.textPhases = new string[]
		{
			string.Concat(new string[]
			{
				"<color=#",
				text,
				">.</color><color=#",
				text,
				">.</color><color=#",
				text,
				">.</color>"
			}),
			string.Concat(new string[]
			{
				"<color=#",
				text2,
				">.</color><color=#",
				text,
				">.</color><color=#",
				text,
				">.</color>"
			}),
			string.Concat(new string[]
			{
				"<color=#",
				text2,
				">.</color><color=#",
				text2,
				">.</color><color=#",
				text,
				">.</color>"
			}),
			string.Concat(new string[]
			{
				"<color=#",
				text2,
				">.</color><color=#",
				text2,
				">.</color><color=#",
				text2,
				">.</color>"
			})
		};
		if (this.isLocked)
		{
			this.lockedText = _lockedText;
			this.scanLines.material.color = _lightColor;
			this.enableScreenLock.SetActive(true);
			this.cycleTimer = 0f;
			this.textIndex = -1;
			this.text.text = this.lockedText;
			return;
		}
		this.lockedText = "";
		this.enableScreenLock.SetActive(false);
		this.text.text = "";
	}

	// Token: 0x04000FCF RID: 4047
	public TextMeshProUGUI text;

	// Token: 0x04000FD0 RID: 4048
	public MeshRenderer scanLines;

	// Token: 0x04000FD1 RID: 4049
	public GameObject enableScreenLock;

	// Token: 0x04000FD2 RID: 4050
	internal bool isLocked;

	// Token: 0x04000FD3 RID: 4051
	private string lockedText = "";

	// Token: 0x04000FD4 RID: 4052
	[SerializeField]
	private float cycleInterval = 0.5f;

	// Token: 0x04000FD5 RID: 4053
	private float cycleTimer;

	// Token: 0x04000FD6 RID: 4054
	private int textIndex = -1;

	// Token: 0x04000FD7 RID: 4055
	private string[] textPhases = new string[]
	{
		"<color=#2B0050>.</color><color=#2B0050>.</color><color=#2B0050>.</color>",
		"<color=#FF0000>.</color><color=#2B0050>.</color><color=#2B0050>.</color>",
		"<color=#FF0000>.</color><color=#FF0000>.</color><color=#2B0050>.</color>",
		"<color=#FF0000>.</color><color=#FF0000>.</color><color=#FF0000>.</color>"
	};
}
