using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class PlayerDeathSpot : MonoBehaviour
{
	// Token: 0x06000ABB RID: 2747 RVA: 0x0005EC17 File Offset: 0x0005CE17
	private void Awake()
	{
		GameDirector.instance.PlayerDeathSpots.Add(this);
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x0005EC29 File Offset: 0x0005CE29
	private void Update()
	{
		this.timer -= Time.deltaTime;
		if (this.timer <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0005EC55 File Offset: 0x0005CE55
	private void OnDestroy()
	{
		GameDirector.instance.PlayerDeathSpots.Remove(this);
	}

	// Token: 0x04001154 RID: 4436
	private float timer = 5f;
}
