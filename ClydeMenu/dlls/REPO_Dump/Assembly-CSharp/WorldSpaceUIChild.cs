using System;
using UnityEngine;

// Token: 0x02000298 RID: 664
public class WorldSpaceUIChild : MonoBehaviour
{
	// Token: 0x060014D0 RID: 5328 RVA: 0x000B8320 File Offset: 0x000B6520
	protected virtual void Start()
	{
		this.myRect = base.GetComponent<RectTransform>();
		this.SetPosition();
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x000B8334 File Offset: 0x000B6534
	protected virtual void Update()
	{
		this.SetPosition();
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x000B833C File Offset: 0x000B653C
	private void SetPosition()
	{
		Vector3 a = SemiFunc.UIWorldToCanvasPosition(this.worldPosition);
		this.myRect.anchoredPosition = a + this.positionOffset;
	}

	// Token: 0x040023E7 RID: 9191
	internal Vector3 worldPosition;

	// Token: 0x040023E8 RID: 9192
	private RectTransform myRect;

	// Token: 0x040023E9 RID: 9193
	internal Vector3 positionOffset;
}
