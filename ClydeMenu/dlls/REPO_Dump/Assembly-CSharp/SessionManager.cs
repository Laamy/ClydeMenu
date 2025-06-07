using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

// Token: 0x02000230 RID: 560
public class SessionManager : MonoBehaviour
{
	// Token: 0x0600127D RID: 4733 RVA: 0x000A64BC File Offset: 0x000A46BC
	private void Awake()
	{
		if (!SessionManager.instance)
		{
			SessionManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		foreach (string text in Microphone.devices)
		{
			this.micDeviceList.Add(text);
		}
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x000A6518 File Offset: 0x000A4718
	private void Start()
	{
		bool flag = false;
		int num = 0;
		foreach (string text in this.micDeviceList)
		{
			if (text == DataDirector.instance.micDevice || DataDirector.instance.micDevice == "")
			{
				this.micDeviceCurrent = text;
				flag = true;
				break;
			}
			num++;
		}
		if (!flag && DataDirector.instance.micDevice != "NONE")
		{
			num = 0;
		}
		this.micDeviceCurrentIndex = num;
		DataDirector.instance.SettingValueSet(DataDirector.Setting.MicDevice, this.micDeviceCurrentIndex);
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x000A65D4 File Offset: 0x000A47D4
	private void Update()
	{
		if (SemiFunc.FPSImpulse1())
		{
			foreach (string text in Microphone.devices)
			{
				if (!this.micDeviceList.Contains(text))
				{
					this.micDeviceList.Add(text);
				}
			}
		}
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F12))
		{
			Application.OpenURL("file://" + Application.persistentDataPath + "/Player.log");
		}
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x000A6657 File Offset: 0x000A4857
	public void CrownPlayer()
	{
		if (this.crownedPlayerSteamID.IsNullOrEmpty())
		{
			return;
		}
		if (SemiFunc.IsMasterClient() && SemiFunc.IsMultiplayer())
		{
			PunManager.instance.CrownPlayerSync(this.crownedPlayerSteamID);
		}
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x000A6685 File Offset: 0x000A4885
	public PlayerAvatar CrownedPlayerGet()
	{
		return SemiFunc.PlayerAvatarGetFromSteamID(this.crownedPlayerSteamID);
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x000A6692 File Offset: 0x000A4892
	public void ResetCrown()
	{
		this.crownedPlayerSteamID = "";
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x000A669F File Offset: 0x000A489F
	public void Reset()
	{
		this.crownedPlayerSteamID = "";
	}

	// Token: 0x04001F0D RID: 7949
	public static SessionManager instance;

	// Token: 0x04001F0E RID: 7950
	internal string crownedPlayerSteamID;

	// Token: 0x04001F0F RID: 7951
	public GameObject crownPrefab;

	// Token: 0x04001F10 RID: 7952
	internal List<string> micDeviceList = new List<string>();

	// Token: 0x04001F11 RID: 7953
	internal string micDeviceCurrent;

	// Token: 0x04001F12 RID: 7954
	internal int micDeviceCurrentIndex;
}
