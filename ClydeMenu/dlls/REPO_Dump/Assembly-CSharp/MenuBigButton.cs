using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F2 RID: 498
public class MenuBigButton : MonoBehaviour
{
	// Token: 0x060010EE RID: 4334 RVA: 0x0009C01E File Offset: 0x0009A21E
	private void Start()
	{
		this.behindButtonMainColor = this.behindButtonBG.color;
		this.mainButtonMainColor = this.mainButtonBG.color;
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x0009C044 File Offset: 0x0009A244
	private void Update()
	{
		MenuBigButton.State state = this.state;
		if (state == MenuBigButton.State.Main)
		{
			this.StateMain();
			return;
		}
		if (state != MenuBigButton.State.Edit)
		{
			return;
		}
		this.StateEdit();
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x0009C070 File Offset: 0x0009A270
	private void StateMain()
	{
		if (this.menuButton.hovering)
		{
			Color color = new Color(0.7f, 0.2f, 0f, 1f);
			this.mainButtonBG.color = color;
			this.behindButtonBG.color = AssetManager.instance.colorYellow;
		}
		else
		{
			this.mainButtonBG.color = this.mainButtonMainColor;
			this.behindButtonBG.color = this.behindButtonMainColor;
		}
		if (this.menuButton.clicked)
		{
			Color color2 = new Color(1f, 0.5f, 0f, 1f);
			this.mainButtonBG.color = color2;
			this.behindButtonBG.color = Color.white;
		}
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x0009C130 File Offset: 0x0009A330
	private void StateEdit()
	{
		this.menuButton.buttonText.text = "[press new button]";
		if (this.menuButton.hovering)
		{
			Color color = new Color(0.5f, 0.1f, 0f, 1f);
			this.mainButtonBG.color = color;
			color = new Color(1f, 0.5f, 0f, 1f);
			this.behindButtonBG.color = color;
		}
		else
		{
			Color color2 = new Color(0.5f, 0.1f, 0f, 1f);
			this.mainButtonBG.color = color2;
			color2 = new Color(0.7f, 0.2f, 0f, 1f);
			this.behindButtonBG.color = color2;
		}
		if (this.menuButton.clicked)
		{
			Color color3 = new Color(1f, 0.5f, 0f, 1f);
			this.mainButtonBG.color = color3;
			this.behindButtonBG.color = Color.white;
		}
	}

	// Token: 0x04001CCA RID: 7370
	public string buttonTitle = "";

	// Token: 0x04001CCB RID: 7371
	public string buttonName = "NewButton";

	// Token: 0x04001CCC RID: 7372
	public RawImage mainButtonBG;

	// Token: 0x04001CCD RID: 7373
	public RawImage behindButtonBG;

	// Token: 0x04001CCE RID: 7374
	public MenuButton menuButton;

	// Token: 0x04001CCF RID: 7375
	public TextMeshProUGUI buttonTitleTextMesh;

	// Token: 0x04001CD0 RID: 7376
	private Color mainButtonMainColor;

	// Token: 0x04001CD1 RID: 7377
	private Color behindButtonMainColor;

	// Token: 0x04001CD2 RID: 7378
	public MenuBigButton.State state;

	// Token: 0x020003D4 RID: 980
	public enum State
	{
		// Token: 0x04002C89 RID: 11401
		Main,
		// Token: 0x04002C8A RID: 11402
		Edit
	}
}
