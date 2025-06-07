using System;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class MenuButtonKick : MonoBehaviour
{
	// Token: 0x060010FC RID: 4348 RVA: 0x0009C2CC File Offset: 0x0009A4CC
	private void Awake()
	{
		this.button = base.GetComponentInChildren<MenuButton>();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
		this.canvasGroup.alpha = 0f;
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x0009C2F8 File Offset: 0x0009A4F8
	private void Update()
	{
		if (!this.setup)
		{
			return;
		}
		float num;
		if (this.button.hovering)
		{
			num = SemiFunc.SpringFloatGet(this.hoverSpring, 1f, -1f);
		}
		else
		{
			num = SemiFunc.SpringFloatGet(this.hoverSpring, 0f, -1f);
		}
		this.backgroundRect.localScale = new Vector3(1f + num * 0.25f, 1f + num * 0.25f, 1f);
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x0009C378 File Offset: 0x0009A578
	public void Setup(PlayerAvatar _playerAvatar)
	{
		this.playerAvatar = _playerAvatar;
		if (!SemiFunc.IsMasterClient())
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.playerAvatar.isLocal)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.GetComponentInChildren<MenuButtonPopUp>().bodyText = "Do you really want to kick\n" + this.playerAvatar.playerName;
		this.canvasGroup.alpha = 1f;
		this.setup = true;
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x0009C3F1 File Offset: 0x0009A5F1
	public void Kick()
	{
		NetworkManager.instance.BanPlayer(this.playerAvatar);
	}

	// Token: 0x04001CD8 RID: 7384
	private MenuButton button;

	// Token: 0x04001CD9 RID: 7385
	private CanvasGroup canvasGroup;

	// Token: 0x04001CDA RID: 7386
	private PlayerAvatar playerAvatar;

	// Token: 0x04001CDB RID: 7387
	private bool setup;

	// Token: 0x04001CDC RID: 7388
	public RectTransform backgroundRect;

	// Token: 0x04001CDD RID: 7389
	public SpringFloat hoverSpring;
}
