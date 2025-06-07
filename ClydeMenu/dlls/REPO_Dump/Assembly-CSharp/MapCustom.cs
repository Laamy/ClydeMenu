using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class MapCustom : MonoBehaviour
{
	// Token: 0x06000DAD RID: 3501 RVA: 0x000779F8 File Offset: 0x00075BF8
	private void Start()
	{
		base.StartCoroutine(this.AddToMap());
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00077A07 File Offset: 0x00075C07
	private IEnumerator AddToMap()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Map.Instance.AddCustom(this, this.sprite, this.color);
		yield break;
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00077A16 File Offset: 0x00075C16
	public void Hide()
	{
		if (this.mapCustomEntity)
		{
			this.mapCustomEntity.Hide();
		}
	}

	// Token: 0x04001601 RID: 5633
	public Sprite sprite;

	// Token: 0x04001602 RID: 5634
	public Color color = new Color(0f, 1f, 0.92f);

	// Token: 0x04001603 RID: 5635
	public MapCustomEntity mapCustomEntity;
}
