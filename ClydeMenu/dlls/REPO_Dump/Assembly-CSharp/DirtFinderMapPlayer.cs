using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class DirtFinderMapPlayer : MonoBehaviour
{
	// Token: 0x06000D90 RID: 3472 RVA: 0x0007684C File Offset: 0x00074A4C
	private void Awake()
	{
		DirtFinderMapPlayer.Instance = this;
		this.StartOffset = base.transform.position;
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x00076865 File Offset: 0x00074A65
	private void OnEnable()
	{
		this.PlayerTransform = null;
		base.StartCoroutine(this.FindPlayer());
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x0007687B File Offset: 0x00074A7B
	private IEnumerator FindPlayer()
	{
		yield return new WaitForSeconds(0.1f);
		while (!this.PlayerTransform)
		{
			if (PlayerController.instance)
			{
				this.PlayerTransform = PlayerController.instance.transform;
				base.StartCoroutine(this.Logic());
			}
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x0007688A File Offset: 0x00074A8A
	private IEnumerator Logic()
	{
		for (;;)
		{
			base.transform.position = this.PlayerTransform.transform.position * Map.Instance.Scale + Map.Instance.OverLayerParent.position + this.StartOffset;
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, 0f, base.transform.localPosition.z);
			base.transform.rotation = this.PlayerTransform.rotation;
			MapLayer layerParent = Map.Instance.GetLayerParent(this.PlayerTransform.position.y + 0.01f);
			Map.Instance.PlayerLayer = layerParent.layer;
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x040015C1 RID: 5569
	public static DirtFinderMapPlayer Instance;

	// Token: 0x040015C2 RID: 5570
	private Transform PlayerTransform;

	// Token: 0x040015C3 RID: 5571
	private Vector3 StartOffset;
}
