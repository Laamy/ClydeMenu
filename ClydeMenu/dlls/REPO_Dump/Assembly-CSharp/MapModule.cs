using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class MapModule : MonoBehaviour
{
	// Token: 0x06000DB6 RID: 3510 RVA: 0x00077A91 File Offset: 0x00075C91
	public void Hide()
	{
		if (this.animating)
		{
			return;
		}
		this.animating = true;
		base.StartCoroutine(this.HideAnimation());
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x00077AB0 File Offset: 0x00075CB0
	private void Update()
	{
		if (Map.Instance.Active)
		{
			this.graphic.transform.rotation = DirtFinderMapPlayer.Instance.transform.rotation;
			this.graphic.transform.rotation = Quaternion.Euler(new Vector3(90f, this.graphic.transform.rotation.eulerAngles.y, this.graphic.transform.rotation.eulerAngles.z));
		}
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x00077B41 File Offset: 0x00075D41
	private IEnumerator HideAnimation()
	{
		while (this.curveLerp < 1f)
		{
			this.curveLerp += this.speed * Time.deltaTime;
			base.transform.localScale = Vector3.one * this.curve.Evaluate(this.curveLerp);
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0400160A RID: 5642
	public Module module;

	// Token: 0x0400160B RID: 5643
	public AnimationCurve curve;

	// Token: 0x0400160C RID: 5644
	public float speed;

	// Token: 0x0400160D RID: 5645
	private float curveLerp;

	// Token: 0x0400160E RID: 5646
	private bool animating;

	// Token: 0x0400160F RID: 5647
	public Transform graphic;
}
