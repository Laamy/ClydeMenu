using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000226 RID: 550
public class MenuSliderMicrophone : MonoBehaviour
{
	// Token: 0x06001241 RID: 4673 RVA: 0x000A4DA8 File Offset: 0x000A2FA8
	private void Awake()
	{
		this.menuSlider = base.GetComponent<MenuSlider>();
		this.micData = new float[128];
		this.SetOptions();
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x000A4DCC File Offset: 0x000A2FCC
	private void Update()
	{
		if (this.currentDeviceCount != SessionManager.instance.micDeviceList.Count)
		{
			this.SetOptions();
			return;
		}
		if (this.menuSlider.bigSettingText.textMeshPro.text != "device name" && SessionManager.instance.micDeviceCurrent != this.menuSlider.bigSettingText.textMeshPro.text)
		{
			SessionManager.instance.micDeviceCurrent = this.menuSlider.bigSettingText.textMeshPro.text;
			DataDirector.instance.micDevice = SessionManager.instance.micDeviceCurrent;
			DataDirector.instance.SaveSettings();
		}
		if (!PlayerVoiceChat.instance)
		{
			if (SessionManager.instance.micDeviceCurrent != this.currentDeviceName)
			{
				Microphone.End(this.currentDeviceName);
				this.currentDeviceName = SessionManager.instance.micDeviceCurrent;
				this.microphoneClipEnabled = false;
			}
			if (this.currentDeviceName != "NONE")
			{
				if (!this.microphoneClipEnabled)
				{
					bool flag = false;
					string[] devices = Microphone.devices;
					for (int i = 0; i < devices.Length; i++)
					{
						if (devices[i] == this.currentDeviceName)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						this.microphoneClipEnabled = true;
						this.microphoneClip = Microphone.Start(this.currentDeviceName, true, 1, 44100);
					}
				}
				if (this.microphoneClipEnabled)
				{
					int num = Microphone.GetPosition(this.currentDeviceName) - 128 + 1;
					if (num < 0)
					{
						return;
					}
					this.microphoneClip.GetData(this.micData, num);
					float num2 = 0f;
					for (int j = 0; j < this.micData.Length; j++)
					{
						num2 += this.micData[j] * this.micData[j];
					}
					this.micLevel = Mathf.Sqrt(num2 / (float)this.micData.Length) * this.micGain;
					this.micLevel = Mathf.Clamp01(this.micLevel);
				}
			}
			if (!this.microphoneClipEnabled)
			{
				this.micLevel = 0f;
			}
		}
		else
		{
			this.micLevel = PlayerVoiceChat.instance.clipLoudnessNoTTS * 5f;
		}
		this.micLevelBar.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Lerp(this.micLevelBar.GetComponent<RectTransform>().localScale.x, this.micLevel, Time.deltaTime * 10f), 0.2f, 1f);
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x000A5040 File Offset: 0x000A3240
	private void SetOptions()
	{
		this.menuSlider.customOptions.Clear();
		foreach (string optionText in SessionManager.instance.micDeviceList)
		{
			this.menuSlider.CustomOptionAdd(optionText, this.micEvent);
		}
		this.menuSlider.CustomOptionAdd("NONE", this.micEvent);
		foreach (MenuSlider.CustomOption customOption in this.menuSlider.customOptions)
		{
			customOption.customValueInt = this.menuSlider.customOptions.IndexOf(customOption);
		}
		this.currentDeviceCount = SessionManager.instance.micDeviceList.Count;
	}

	// Token: 0x04001EB3 RID: 7859
	private MenuSlider menuSlider;

	// Token: 0x04001EB4 RID: 7860
	public UnityEvent micEvent;

	// Token: 0x04001EB5 RID: 7861
	public Image micLevelBar;

	// Token: 0x04001EB6 RID: 7862
	private AudioClip microphoneClip;

	// Token: 0x04001EB7 RID: 7863
	private bool microphoneClipEnabled;

	// Token: 0x04001EB8 RID: 7864
	private string currentDeviceName;

	// Token: 0x04001EB9 RID: 7865
	private int currentDeviceCount;

	// Token: 0x04001EBA RID: 7866
	private const int sampleDataLength = 128;

	// Token: 0x04001EBB RID: 7867
	private float[] micData;

	// Token: 0x04001EBC RID: 7868
	private float micLevel;

	// Token: 0x04001EBD RID: 7869
	private float micGain = 10f;
}
