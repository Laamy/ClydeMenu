using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class DirtFinderMapEnemy : MonoBehaviour
{
	// Token: 0x06000D8B RID: 3467 RVA: 0x0007680F File Offset: 0x00074A0F
	public IEnumerator Logic()
	{
		while (this.Parent != null && this.Parent.gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.EnemyPositionSet(base.transform, this.Parent.transform);
			}
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x040015BE RID: 5566
	public Transform Parent;
}
