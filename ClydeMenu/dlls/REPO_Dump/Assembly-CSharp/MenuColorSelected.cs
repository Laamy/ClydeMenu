using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000218 RID: 536
public class MenuColorSelected : MonoBehaviour
{
	// Token: 0x060011E2 RID: 4578 RVA: 0x000A2958 File Offset: 0x000A0B58
	private void Start()
	{
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.positionSpring = new SpringVector3();
		this.positionSpring.speed = 50f;
		this.positionSpring.damping = 0.55f;
		this.positionSpring.lastPosition = base.transform.position;
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x000A29B2 File Offset: 0x000A0BB2
	public void SetColor(Color color, Vector3 position)
	{
		base.StartCoroutine(this.SetColorRoutine(color, position));
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x000A29C3 File Offset: 0x000A0BC3
	private IEnumerator SetColorRoutine(Color color, Vector3 position)
	{
		yield return new WaitForSeconds(0.01f);
		this.rawImage.color = color;
		this.selectedPosition = position;
		this.goTime = true;
		yield break;
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x000A29E0 File Offset: 0x000A0BE0
	private void Update()
	{
		if (!this.goTime)
		{
			return;
		}
		if (this.parentPage.currentPageState == MenuPage.PageState.Closing)
		{
			return;
		}
		base.transform.position = SemiFunc.SpringVector3Get(this.positionSpring, this.selectedPosition + Vector3.up * 0.038f + Vector3.right * 0.046f, -1f);
		base.transform.position = new Vector3(base.transform.position.x + 18f, base.transform.position.y + 16f, 1f);
	}

	// Token: 0x04001E48 RID: 7752
	public SpringVector3 positionSpring;

	// Token: 0x04001E49 RID: 7753
	internal Vector3 selectedPosition;

	// Token: 0x04001E4A RID: 7754
	public RawImage rawImage;

	// Token: 0x04001E4B RID: 7755
	private MenuPage parentPage;

	// Token: 0x04001E4C RID: 7756
	private bool goTime;
}
