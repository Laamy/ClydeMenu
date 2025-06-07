using System;
using UnityEngine;

// Token: 0x0200026A RID: 618
public class HUD : MonoBehaviour
{
	// Token: 0x060013AC RID: 5036 RVA: 0x000AEE4A File Offset: 0x000AD04A
	private void Awake()
	{
		HUD.instance = this;
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x000AEE52 File Offset: 0x000AD052
	public void Hide()
	{
		this.hideParent.SetActive(false);
		this.hidden = true;
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x000AEE67 File Offset: 0x000AD067
	public void Show()
	{
		this.hideParent.SetActive(true);
		this.hidden = false;
	}

	// Token: 0x040021C8 RID: 8648
	public static HUD instance;

	// Token: 0x040021C9 RID: 8649
	public bool hidden;

	// Token: 0x040021CA RID: 8650
	public GameObject hideParent;
}
