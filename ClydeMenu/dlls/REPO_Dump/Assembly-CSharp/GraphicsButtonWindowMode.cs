using System;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class GraphicsButtonWindowMode : MonoBehaviour
{
	// Token: 0x06000974 RID: 2420 RVA: 0x00058994 File Offset: 0x00056B94
	private void Awake()
	{
		GraphicsButtonWindowMode.instance = this;
		this.slider = base.GetComponent<MenuSlider>();
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x000589A8 File Offset: 0x00056BA8
	public void UpdateSlider()
	{
		this.slider.Start();
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x000589B5 File Offset: 0x00056BB5
	public void ButtonPressed()
	{
		GraphicsManager.instance.UpdateWindowMode(true);
	}

	// Token: 0x040010EB RID: 4331
	public static GraphicsButtonWindowMode instance;

	// Token: 0x040010EC RID: 4332
	private MenuSlider slider;
}
