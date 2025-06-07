using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class DirtFinderMapDoorTarget : MonoBehaviour
{
	// Token: 0x06000D88 RID: 3464 RVA: 0x000767E9 File Offset: 0x000749E9
	private void Start()
	{
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x000767F8 File Offset: 0x000749F8
	public IEnumerator Logic()
	{
		while (this.Target && this.Target.gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.DoorUpdate(this.HingeTransform, this.Target.transform, this.Layer);
			}
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040015BB RID: 5563
	public Transform Target;

	// Token: 0x040015BC RID: 5564
	public Transform HingeTransform;

	// Token: 0x040015BD RID: 5565
	public MapLayer Layer;
}
