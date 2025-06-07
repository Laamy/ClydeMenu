using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class MapValuable : MonoBehaviour
{
	// Token: 0x06000DBD RID: 3517 RVA: 0x00077BEE File Offset: 0x00075DEE
	private void Start()
	{
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x00077BFD File Offset: 0x00075DFD
	private IEnumerator Logic()
	{
		for (;;)
		{
			if (!Map.Instance.Active)
			{
				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				if (!this.target)
				{
					break;
				}
				Map.Instance.CustomPositionSet(base.transform, this.target.transform);
				MapLayer layerParent = Map.Instance.GetLayerParent(this.target.transform.position.y + 1f);
				Color color = this.spriteRenderer.color;
				if (layerParent.layer == Map.Instance.PlayerLayer)
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
		}
		Object.Destroy(base.gameObject);
		yield break;
		yield break;
	}

	// Token: 0x04001611 RID: 5649
	public ValuableObject target;

	// Token: 0x04001612 RID: 5650
	[Space]
	public SpriteRenderer spriteRenderer;

	// Token: 0x04001613 RID: 5651
	[Space]
	public Sprite spriteSmall;

	// Token: 0x04001614 RID: 5652
	public Sprite spriteBig;
}
