using System;
using TMPro;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class MenuPlayerListed : MonoBehaviour
{
	// Token: 0x06001171 RID: 4465 RVA: 0x0009EEB4 File Offset: 0x0009D0B4
	private void Start()
	{
		this.parentTransform = base.transform.parent.GetComponent<RectTransform>();
		this.playerHead.focusPoint.SetParent(this.parentTransform);
		this.playerHead.myFocusPoint.SetParent(this.parentTransform);
		this.midScreenFocus = new Vector3((float)(MenuManager.instance.screenUIWidth / 2), (float)(MenuManager.instance.screenUIHeight / 2), 0f) - this.parentTransform.localPosition - this.parentTransform.parent.GetComponent<RectTransform>().localPosition;
		if (this.forceCrown)
		{
			this.leftCrown.SetActive(true);
			this.rightCrown.SetActive(true);
			this.ForcePlayer(Arena.instance.winnerPlayer);
			TextMeshProUGUI componentInChildren = base.GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren && this.playerAvatar)
			{
				componentInChildren.text = this.playerAvatar.playerName;
			}
		}
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x0009EFB4 File Offset: 0x0009D1B4
	public void ForcePlayer(PlayerAvatar _playerAvatar)
	{
		this.playerHead.SetPlayer(_playerAvatar);
		this.playerAvatar = _playerAvatar;
		this.localFetch = false;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x0009EFD0 File Offset: 0x0009D1D0
	private void Update()
	{
		if (SemiFunc.FPSImpulse5() && !this.crownSetterWasHere && PlayerCrownSet.instance && PlayerCrownSet.instance.crownOwnerFetched)
		{
			if (this.playerAvatar && PlayerCrownSet.instance.crownOwnerSteamID == this.playerAvatar.steamID)
			{
				this.leftCrown.SetActive(true);
				this.rightCrown.SetActive(true);
			}
			this.crownSetterWasHere = true;
		}
		if (!this.localFetch && this.playerAvatar)
		{
			MenuButtonKick componentInChildren = base.GetComponentInChildren<MenuButtonKick>();
			if (componentInChildren)
			{
				componentInChildren.Setup(this.playerAvatar);
			}
			this.isLocal = this.playerAvatar.isLocal;
			this.localFetch = true;
		}
		if (!this.forceCrown && this.playerHead.myFocusPoint.localPosition != this.midScreenFocus)
		{
			this.playerHead.myFocusPoint.localPosition = this.midScreenFocus;
		}
		if (this.playerAvatar)
		{
			if (!this.fetchCrown)
			{
				if (SessionManager.instance.CrownedPlayerGet() == this.playerAvatar)
				{
					this.leftCrown.SetActive(true);
					this.rightCrown.SetActive(true);
				}
				this.fetchCrown = true;
			}
			if (this.isSpectate && this.playerName.text != this.playerAvatar.playerName)
			{
				this.playerName.text = this.playerAvatar.playerName;
			}
			if (this.playerAvatar.voiceChatFetched && this.playerAvatar.voiceChat.isTalking)
			{
				Color white = new Color(0.6f, 0.6f, 0.4f);
				if (this.brightName)
				{
					white = Color.white;
				}
				this.playerName.color = Color.Lerp(this.playerName.color, white, Time.deltaTime * 10f);
			}
			else
			{
				Color white2 = new Color(0.2f, 0.2f, 0.2f);
				if (this.brightName)
				{
					white2 = Color.white;
				}
				this.playerName.color = Color.Lerp(this.playerName.color, white2, Time.deltaTime * 10f);
			}
		}
		if (!this.forceCrown)
		{
			if (RunManager.instance.levelCurrent != RunManager.instance.levelLobbyMenu)
			{
				base.transform.localPosition = new Vector3(-23f, (float)(-(float)this.listSpot * 22), 0f);
				return;
			}
			base.transform.localPosition = new Vector3(0f, (float)(-(float)this.listSpot * 32), 0f);
		}
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x0009F27D File Offset: 0x0009D47D
	public void MenuPlayerListedOutro()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001D83 RID: 7555
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001D84 RID: 7556
	internal int listSpot;

	// Token: 0x04001D85 RID: 7557
	public TextMeshProUGUI playerName;

	// Token: 0x04001D86 RID: 7558
	public MenuPlayerHead playerHead;

	// Token: 0x04001D87 RID: 7559
	private RectTransform parentTransform;

	// Token: 0x04001D88 RID: 7560
	private Vector3 midScreenFocus;

	// Token: 0x04001D89 RID: 7561
	public TextMeshProUGUI pingText;

	// Token: 0x04001D8A RID: 7562
	private bool localFetch;

	// Token: 0x04001D8B RID: 7563
	internal bool isLocal;

	// Token: 0x04001D8C RID: 7564
	public bool isSpectate = true;

	// Token: 0x04001D8D RID: 7565
	public GameObject leftCrown;

	// Token: 0x04001D8E RID: 7566
	public GameObject rightCrown;

	// Token: 0x04001D8F RID: 7567
	private bool fetchCrown;

	// Token: 0x04001D90 RID: 7568
	public bool forceCrown;

	// Token: 0x04001D91 RID: 7569
	private bool crownSetterWasHere;

	// Token: 0x04001D92 RID: 7570
	public bool brightName;
}
