using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000292 RID: 658
public class RewindTexture : MonoBehaviour
{
	// Token: 0x060014B6 RID: 5302 RVA: 0x000B6D0C File Offset: 0x000B4F0C
	private void Start()
	{
		this.rawImage = base.GetComponent<RawImage>();
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x000B6D1C File Offset: 0x000B4F1C
	private void Update()
	{
		float num = Mathf.Repeat(Time.time * this.scrollSpeed, 1f);
		Rect uvRect = this.rawImage.uvRect;
		uvRect.x = num;
		uvRect.y = num;
		this.rawImage.uvRect = uvRect;
	}

	// Token: 0x0400238E RID: 9102
	public float scrollSpeed = 0.5f;

	// Token: 0x0400238F RID: 9103
	private RawImage rawImage;
}
