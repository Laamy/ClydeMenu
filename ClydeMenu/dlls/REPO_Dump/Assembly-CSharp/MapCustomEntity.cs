using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class MapCustomEntity : MonoBehaviour
{
	// Token: 0x06000DB1 RID: 3505 RVA: 0x00077A52 File Offset: 0x00075C52
	public IEnumerator Logic()
	{
		while (this.Parent && this.Parent.gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.CustomPositionSet(base.transform, this.Parent.transform);
			}
			MapLayer layerParent = Map.Instance.GetLayerParent(this.Parent.transform.position.y + 1f);
			Color color = this.spriteRenderer.color;
			if (this.mapCustomHideTimer > 0f)
			{
				this.mapCustomHideTimer -= 0.1f;
				color.a = 0f;
			}
			else if (layerParent.layer == Map.Instance.PlayerLayer)
			{
				color.a = 1f;
			}
			else
			{
				color.a = 0.3f;
			}
			this.spriteRenderer.color = color;
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00077A61 File Offset: 0x00075C61
	public void Hide()
	{
		this.mapCustomHideTimer = 0.5f;
	}

	// Token: 0x04001604 RID: 5636
	public Transform Parent;

	// Token: 0x04001605 RID: 5637
	public SpriteRenderer spriteRenderer;

	// Token: 0x04001606 RID: 5638
	public MapCustom mapCustom;

	// Token: 0x04001607 RID: 5639
	private float mapCustomHideTimer;
}
