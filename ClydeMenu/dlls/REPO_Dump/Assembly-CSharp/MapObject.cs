using System;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class MapObject : MonoBehaviour
{
	// Token: 0x06000DBA RID: 3514 RVA: 0x00077B58 File Offset: 0x00075D58
	public void Hide()
	{
		foreach (Transform transform in base.transform.GetComponentsInChildren<Transform>(true))
		{
			if (transform != base.transform)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00077BA0 File Offset: 0x00075DA0
	public void Show()
	{
		foreach (Transform transform in base.transform.GetComponentsInChildren<Transform>(true))
		{
			if (transform != base.transform)
			{
				transform.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x04001610 RID: 5648
	public Transform parent;
}
