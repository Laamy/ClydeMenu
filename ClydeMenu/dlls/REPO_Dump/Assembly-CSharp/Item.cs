using System;
using UnityEngine;

// Token: 0x0200015E RID: 350
[CreateAssetMenu(fileName = "NewItem", menuName = "Other/Item")]
public class Item : ScriptableObject
{
	// Token: 0x06000BFF RID: 3071 RVA: 0x000699F7 File Offset: 0x00067BF7
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.itemAssetName = base.name;
		this.prefab = Resources.Load<GameObject>("Items/" + this.itemAssetName);
	}

	// Token: 0x04001356 RID: 4950
	public bool disabled;

	// Token: 0x04001357 RID: 4951
	[Space]
	public string itemAssetName;

	// Token: 0x04001358 RID: 4952
	public string itemName = "N/A";

	// Token: 0x04001359 RID: 4953
	public string description;

	// Token: 0x0400135A RID: 4954
	[Space]
	public SemiFunc.itemType itemType;

	// Token: 0x0400135B RID: 4955
	public SemiFunc.emojiIcon emojiIcon;

	// Token: 0x0400135C RID: 4956
	public SemiFunc.itemVolume itemVolume;

	// Token: 0x0400135D RID: 4957
	public SemiFunc.itemSecretShopType itemSecretShopType;

	// Token: 0x0400135E RID: 4958
	[Space]
	public ColorPresets colorPreset;

	// Token: 0x0400135F RID: 4959
	public GameObject prefab;

	// Token: 0x04001360 RID: 4960
	public Value value;

	// Token: 0x04001361 RID: 4961
	[Space]
	public int maxAmount = 1;

	// Token: 0x04001362 RID: 4962
	public int maxAmountInShop = 1;

	// Token: 0x04001363 RID: 4963
	[Space]
	public bool maxPurchase;

	// Token: 0x04001364 RID: 4964
	public int maxPurchaseAmount = 1;

	// Token: 0x04001365 RID: 4965
	[Space]
	public Quaternion spawnRotationOffset;

	// Token: 0x04001366 RID: 4966
	public bool physicalItem = true;
}
