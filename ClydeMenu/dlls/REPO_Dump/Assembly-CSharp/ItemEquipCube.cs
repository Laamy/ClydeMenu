using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class ItemEquipCube : MonoBehaviour
{
	// Token: 0x06000BCD RID: 3021 RVA: 0x00068B35 File Offset: 0x00066D35
	private void OnTriggerStay(Collider other)
	{
		this.isObstructed = true;
		this.obstructedTimer = 0.1f;
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00068B49 File Offset: 0x00066D49
	private void Update()
	{
		if (this.obstructedTimer > 0f)
		{
			this.obstructedTimer -= Time.deltaTime;
			return;
		}
		this.isObstructed = false;
	}

	// Token: 0x04001334 RID: 4916
	[HideInInspector]
	public bool isObstructed;

	// Token: 0x04001335 RID: 4917
	private float obstructedTimer;
}
