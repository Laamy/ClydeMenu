using System;
using UnityEngine;

// Token: 0x020001B4 RID: 436
public class PlayerAvatarMenuHover : MonoBehaviour
{
	// Token: 0x06000EE4 RID: 3812 RVA: 0x00086286 File Offset: 0x00084486
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.menuElementHover = base.GetComponent<MenuElementHover>();
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x000862AC File Offset: 0x000844AC
	private void Update()
	{
		Vector2 vector = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(this.rectTransform);
		this.pointer.localPosition = new Vector3(vector.x * 0.98f, vector.y * 1.035f, 0f) / SemiFunc.UIMulti() * 2.23f;
		this.pointer.localPosition += new Vector3(-0.065f, -0.06f, 0f);
		this.pointer.GetComponent<MeshRenderer>().enabled = false;
		if (SemiFunc.InputHold(InputKey.Grab) && this.menuElementHover.isHovering)
		{
			if (!this.startClick)
			{
				this.startClick = true;
				this.mouseClickPos = vector;
			}
			Vector2 vector2 = (vector - this.mouseClickPos) * 25f;
			this.playerAvatarMenu.Rotate(new Vector3(0f, -vector2.x, 0f));
			return;
		}
		this.startClick = false;
	}

	// Token: 0x0400187C RID: 6268
	private RectTransform rectTransform;

	// Token: 0x0400187D RID: 6269
	private MenuPage parentPage;

	// Token: 0x0400187E RID: 6270
	public Transform pointer;

	// Token: 0x0400187F RID: 6271
	private bool startClick;

	// Token: 0x04001880 RID: 6272
	private Vector2 mouseClickPos;

	// Token: 0x04001881 RID: 6273
	public PlayerAvatarMenu playerAvatarMenu;

	// Token: 0x04001882 RID: 6274
	private MenuElementHover menuElementHover;
}
