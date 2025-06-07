using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class MenuSpectateList : SemiUI
{
	// Token: 0x060011A9 RID: 4521 RVA: 0x000A12E0 File Offset: 0x0009F4E0
	protected override void Update()
	{
		base.Update();
		if (!SemiFunc.IsMultiplayer())
		{
			base.Hide();
			return;
		}
		if (!SpectateCamera.instance)
		{
			base.Hide();
			return;
		}
		base.SemiUIScoot(new Vector2(0f, (float)this.listObjects.Count * 22f));
		this.listCheckTimer -= Time.deltaTime;
		if (this.listCheckTimer <= 0f)
		{
			this.listCheckTimer = 1f;
			List<PlayerAvatar> list = SemiFunc.PlayerGetList();
			bool flag = false;
			foreach (PlayerAvatar playerAvatar in list)
			{
				if (playerAvatar.isDisabled)
				{
					if (!this.spectatingPlayers.Contains(playerAvatar))
					{
						this.PlayerAdd(playerAvatar);
						flag = true;
					}
				}
				else if (this.spectatingPlayers.Contains(playerAvatar))
				{
					this.PlayerRemove(playerAvatar);
					flag = true;
				}
			}
			foreach (PlayerAvatar playerAvatar2 in Enumerable.ToList<PlayerAvatar>(this.spectatingPlayers))
			{
				if (!list.Contains(playerAvatar2))
				{
					this.PlayerRemove(playerAvatar2);
					flag = true;
				}
			}
			if (flag)
			{
				this.listObjects.Sort((GameObject a, GameObject b) => a.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID.CompareTo(b.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID));
				for (int i = 0; i < this.listObjects.Count; i++)
				{
					this.listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
					this.listObjects[i].transform.SetSiblingIndex(i);
				}
			}
		}
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x000A14AC File Offset: 0x0009F6AC
	private void PlayerAdd(PlayerAvatar player)
	{
		this.spectatingPlayers.Add(player);
		GameObject gameObject = Object.Instantiate<GameObject>(this.menuPlayerListedPrefab, base.transform);
		MenuPlayerListed component = gameObject.GetComponent<MenuPlayerListed>();
		component.playerAvatar = player;
		component.playerHead.SetPlayer(player);
		this.listObjects.Add(gameObject);
		component.listSpot = Mathf.Max(this.listObjects.Count - 1, 0);
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x000A1514 File Offset: 0x0009F714
	private void PlayerRemove(PlayerAvatar player)
	{
		this.spectatingPlayers.Remove(player);
		foreach (GameObject gameObject in this.listObjects)
		{
			if (gameObject.GetComponent<MenuPlayerListed>().playerAvatar == player)
			{
				gameObject.GetComponent<MenuPlayerListed>().MenuPlayerListedOutro();
				this.listObjects.Remove(gameObject);
				break;
			}
		}
		for (int i = 0; i < this.listObjects.Count; i++)
		{
			this.listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
		}
	}

	// Token: 0x04001DFD RID: 7677
	public GameObject menuPlayerListedPrefab;

	// Token: 0x04001DFE RID: 7678
	internal List<PlayerAvatar> spectatingPlayers = new List<PlayerAvatar>();

	// Token: 0x04001DFF RID: 7679
	internal List<GameObject> listObjects = new List<GameObject>();

	// Token: 0x04001E00 RID: 7680
	private float listCheckTimer;
}
