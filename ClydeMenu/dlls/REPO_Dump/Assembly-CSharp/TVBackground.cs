using System;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class TVBackground : MonoBehaviour
{
	// Token: 0x06001365 RID: 4965 RVA: 0x000ACF77 File Offset: 0x000AB177
	private void Start()
	{
		this.material = base.GetComponent<Renderer>().material;
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x000ACF8C File Offset: 0x000AB18C
	private void Update()
	{
		this.offset.x = Time.time * this.scrollSpeed.x;
		this.offset.y = Time.time * this.scrollSpeed.y;
		this.material.SetTextureOffset("_MainTex", this.offset);
	}

	// Token: 0x04002137 RID: 8503
	public Vector2 scrollSpeed = new Vector2(0.5f, 0.5f);

	// Token: 0x04002138 RID: 8504
	private Vector2 offset;

	// Token: 0x04002139 RID: 8505
	private Material material;
}
