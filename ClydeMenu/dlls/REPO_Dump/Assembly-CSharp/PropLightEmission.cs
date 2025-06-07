using System;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class PropLightEmission : MonoBehaviour
{
	// Token: 0x060008DF RID: 2271 RVA: 0x0005589E File Offset: 0x00053A9E
	private void Awake()
	{
		this.meshRenderer = base.GetComponent<Renderer>();
		this.material = this.meshRenderer.material;
		this.originalEmission = this.material.GetColor("_EmissionColor");
	}

	// Token: 0x0400102B RID: 4139
	public bool levelLight = true;

	// Token: 0x0400102C RID: 4140
	internal bool turnedOff;

	// Token: 0x0400102D RID: 4141
	internal Renderer meshRenderer;

	// Token: 0x0400102E RID: 4142
	internal Color originalEmission;

	// Token: 0x0400102F RID: 4143
	internal Material material;
}
