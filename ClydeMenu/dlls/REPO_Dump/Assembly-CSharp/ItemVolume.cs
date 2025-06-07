using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class ItemVolume : MonoBehaviour
{
	// Token: 0x06000C01 RID: 3073 RVA: 0x00069A57 File Offset: 0x00067C57
	private void Start()
	{
		this.itemAttributes = base.GetComponentInParent<ItemAttributes>();
		if (this.itemAttributes)
		{
			base.gameObject.tag = "Untagged";
		}
		if (SemiFunc.IsNotMasterClient())
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00069A90 File Offset: 0x00067C90
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		ItemAttributes componentInParent = base.GetComponentInParent<ItemAttributes>();
		if (componentInParent)
		{
			if (this.itemVolume != componentInParent.item.itemVolume)
			{
				this.itemVolume = componentInParent.item.itemVolume;
			}
			string text = "Item Volume " + this.itemVolume.ToString();
			if (base.gameObject.name != text)
			{
				base.gameObject.name = text;
			}
		}
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00069B14 File Offset: 0x00067D14
	private void OnDrawGizmos()
	{
		ItemAttributes componentInParent = base.GetComponentInParent<ItemAttributes>();
		int num = 0;
		foreach (GameObject gameObject in this.volumes)
		{
			if (this.itemVolume == (SemiFunc.itemVolume)num)
			{
				Color yellow = Color.yellow;
				Gizmos.color = yellow;
				Gizmos.matrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);
				Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), Vector3.one);
				yellow.a = 0.5f;
				Gizmos.color = yellow;
				if (!componentInParent)
				{
					Gizmos.DrawCube(Vector3.zero, Vector3.one);
				}
				Gizmos.matrix = Matrix4x4.identity;
			}
			num++;
		}
	}

	// Token: 0x04001367 RID: 4967
	public SemiFunc.itemVolume itemVolume;

	// Token: 0x04001368 RID: 4968
	public SemiFunc.itemSecretShopType itemSecretShopType;

	// Token: 0x04001369 RID: 4969
	public List<GameObject> volumes = new List<GameObject>();

	// Token: 0x0400136A RID: 4970
	private ItemAttributes itemAttributes;
}
