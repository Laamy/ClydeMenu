using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class AnimateTextPeriods : MonoBehaviour
{
	// Token: 0x0600108E RID: 4238 RVA: 0x00098FF5 File Offset: 0x000971F5
	private void Awake()
	{
		this.textMesh = base.GetComponent<TextMeshProUGUI>();
		this.textString = this.textMesh.text;
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x00099014 File Offset: 0x00097214
	private void OnEnable()
	{
		this.animateCoroutine = base.StartCoroutine(this.AnimateDots());
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x00099028 File Offset: 0x00097228
	private IEnumerator AnimateDots()
	{
		for (;;)
		{
			this.textMesh.text = this.textString + "...".Substring(0, Mathf.FloorToInt(Time.unscaledTime * 8f % 4f));
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x00099037 File Offset: 0x00097237
	private void OnDisable()
	{
		if (this.animateCoroutine != null)
		{
			base.StopCoroutine(this.animateCoroutine);
		}
	}

	// Token: 0x04001C6F RID: 7279
	private TextMeshProUGUI textMesh;

	// Token: 0x04001C70 RID: 7280
	private string textString;

	// Token: 0x04001C71 RID: 7281
	private Coroutine animateCoroutine;
}
