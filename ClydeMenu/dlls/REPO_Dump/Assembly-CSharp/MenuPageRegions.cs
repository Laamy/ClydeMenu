using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class MenuPageRegions : MonoBehaviourPunCallbacks
{
	// Token: 0x0600125A RID: 4698 RVA: 0x000A5C7E File Offset: 0x000A3E7E
	private void Start()
	{
		base.StartCoroutine(this.GetRegions());
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x000A5C8D File Offset: 0x000A3E8D
	private IEnumerator GetRegions()
	{
		PhotonNetwork.Disconnect();
		while (PhotonNetwork.NetworkingClient.State != ClientState.Disconnected && PhotonNetwork.NetworkingClient.State != ClientState.PeerCreated)
		{
			yield return null;
		}
		DataDirector.instance.PhotonSetAppId();
		SteamManager.instance.SendSteamAuthTicket();
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
		ServerSettings.ResetBestRegionCodeInPreferences();
		PhotonNetwork.ConnectUsingSettings();
		while (!this.connectedToMaster)
		{
			yield return null;
		}
		this.loadingGraphics.SetDone();
		yield return new WaitForSecondsRealtime(0.3f);
		Vector3 _position = this.transformStartPosition.position;
		MenuElementRegion component = Object.Instantiate<GameObject>(this.regionPrefab, _position, Quaternion.identity, this.transformStartPosition.parent).GetComponent<MenuElementRegion>();
		component.textName.text = "Pick Best Region";
		component.textName.color = Color.white;
		component.textPing.text = "";
		component.parentPage = this;
		component.regionCode = "";
		float _pitch = 0f;
		foreach (Region region in PhotonNetwork.NetworkingClient.RegionHandler.EnabledRegions)
		{
			_position = new Vector3(_position.x, _position.y - 32f, _position.z);
			string text = region.Code;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1234284266U)
			{
				if (num <= 1094220446U)
				{
					if (num <= 733221362U)
					{
						if (num != 162435063U)
						{
							if (num == 733221362U)
							{
								if (text == "uae")
								{
									text = "United Arab Emirates";
								}
							}
						}
						else if (text == "asia")
						{
							text = "Asia";
						}
					}
					else if (num != 944899161U)
					{
						if (num == 1094220446U)
						{
							if (text == "in")
							{
								text = "India";
							}
						}
					}
					else if (text == "sa")
					{
						text = "South America";
					}
				}
				else if (num <= 1195724803U)
				{
					if (num != 1178800089U)
					{
						if (num == 1195724803U)
						{
							if (text == "tr")
							{
								text = "Turkey";
							}
						}
					}
					else if (text == "us")
					{
						text = "USA East";
					}
				}
				else if (num != 1209692303U)
				{
					if (num == 1234284266U)
					{
						if (text == "usw")
						{
							text = "USA West";
						}
					}
				}
				else if (text == "eu")
				{
					text = "Europe";
				}
			}
			else if (num <= 1564435063U)
			{
				if (num <= 1430067016U)
				{
					if (num != 1281878564U)
					{
						if (num == 1430067016U)
						{
							if (text == "kr")
							{
								text = "South Korea";
							}
						}
					}
					else if (text == "za")
					{
						text = "South Africa";
					}
				}
				else if (num != 1478825755U)
				{
					if (num == 1564435063U)
					{
						if (text == "jp")
						{
							text = "Japan";
						}
					}
				}
				else if (text == "au")
				{
					text = "Australia";
				}
			}
			else if (num <= 1715139444U)
			{
				if (num != 1630118516U)
				{
					if (num == 1715139444U)
					{
						if (text == "hk")
						{
							text = "Hong Kong";
						}
					}
				}
				else if (text == "cn")
				{
					text = "Chinese Mainland";
				}
			}
			else if (num != 2169308743U)
			{
				if (num == 4118036804U)
				{
					if (text == "cae")
					{
						text = "Canada East";
					}
				}
			}
			else if (text == "ussc")
			{
				text = "USA South Central";
			}
			MenuElementRegion component2 = Object.Instantiate<GameObject>(this.regionPrefab, _position, Quaternion.identity, this.transformStartPosition.parent).GetComponent<MenuElementRegion>();
			component2.textName.text = text;
			string text2;
			if (region.Ping > 200 || region.Ping == RegionPinger.PingWhenFailed)
			{
				text2 = ">200";
				component2.textPing.color = new Color(0.8f, 0.2f, 0.2f);
			}
			else
			{
				if (region.Ping < 50)
				{
					component2.textPing.color = new Color(0.2f, 0.8f, 0.2f);
				}
				else if (region.Ping < 100)
				{
					component2.textPing.color = new Color(0.8f, 0.8f, 0.2f);
				}
				else
				{
					component2.textPing.color = new Color(0.8f, 0.4f, 0.2f);
				}
				text2 = region.Ping.ToString();
			}
			component2.textPing.text = text2 + " ms";
			component2.parentPage = this;
			component2.regionCode = region.Code;
			float pitch = MenuManager.instance.soundPageIntro.Pitch;
			MenuManager.instance.soundPageIntro.Pitch = 1f + _pitch;
			MenuManager.instance.soundPageIntro.Play(Vector3.zero, 0.75f, 1f, 1f, 1f);
			MenuManager.instance.soundPageIntro.Pitch = pitch;
			_pitch += 0.1f;
			yield return new WaitForSecondsRealtime(0.15f);
		}
		List<Region>.Enumerator enumerator = default(List<Region>.Enumerator);
		PhotonNetwork.Disconnect();
		this.menuScrollBox.enabled = true;
		while (this.scrollCanvasGroup.alpha < 0.99f)
		{
			this.scrollCanvasGroup.alpha += Time.deltaTime * 5f;
			yield return null;
		}
		this.scrollCanvasGroup.alpha = 1f;
		yield break;
		yield break;
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x000A5C9C File Offset: 0x000A3E9C
	public override void OnConnectedToMaster()
	{
		this.connectedToMaster = true;
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x000A5CA5 File Offset: 0x000A3EA5
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Back) && MenuManager.instance.currentMenuPageIndex == MenuPageIndex.Regions)
		{
			this.ExitPage();
		}
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x000A5CC4 File Offset: 0x000A3EC4
	public void ExitPage()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Main, false);
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x000A5CE0 File Offset: 0x000A3EE0
	public void PickRegion(string _region)
	{
		DataDirector.instance.networkRegion = _region;
		if (this.type == MenuPageRegions.Type.HostGame)
		{
			SemiFunc.MainMenuSetMultiplayer();
			MenuManager.instance.PageCloseAll();
			MenuManager.instance.PageOpen(MenuPageIndex.Saves, false);
			return;
		}
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.PublicGameChoice, false);
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x000A5D36 File Offset: 0x000A3F36
	private void OnDestroy()
	{
		PhotonNetwork.Disconnect();
	}

	// Token: 0x04001EE7 RID: 7911
	internal MenuPageRegions.Type type;

	// Token: 0x04001EE8 RID: 7912
	private bool connectedToMaster;

	// Token: 0x04001EE9 RID: 7913
	public GameObject regionPrefab;

	// Token: 0x04001EEA RID: 7914
	public Transform transformStartPosition;

	// Token: 0x04001EEB RID: 7915
	public MenuScrollBox menuScrollBox;

	// Token: 0x04001EEC RID: 7916
	public CanvasGroup scrollCanvasGroup;

	// Token: 0x04001EED RID: 7917
	public MenuLoadingGraphics loadingGraphics;

	// Token: 0x020003EC RID: 1004
	internal enum Type
	{
		// Token: 0x04002CD0 RID: 11472
		HostGame,
		// Token: 0x04002CD1 RID: 11473
		PlayRandom
	}
}
