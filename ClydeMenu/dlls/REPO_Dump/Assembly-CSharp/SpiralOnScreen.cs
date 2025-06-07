using System;
using UnityEngine;

// Token: 0x020002B5 RID: 693
public class SpiralOnScreen : MonoBehaviour
{
	// Token: 0x060015C9 RID: 5577 RVA: 0x000C043D File Offset: 0x000BE63D
	private void Start()
	{
		this.MakeTransparent();
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x000C0448 File Offset: 0x000BE648
	private void Update()
	{
		if (this.isActive)
		{
			this.FadeIn();
			this.spiral.Rotate(0f, 0f, this.rotationSpeed * Time.deltaTime);
			return;
		}
		this.FadeOut();
		if (this.spiralSpriteRenderer.material.color.a <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x000C04B2 File Offset: 0x000BE6B2
	public void SetActive()
	{
		this.isActive = true;
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x000C04BB File Offset: 0x000BE6BB
	public void SetInactive()
	{
		this.isActive = false;
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x000C04C4 File Offset: 0x000BE6C4
	private void FadeIn()
	{
		Color color = this.spiralSpriteRenderer.material.color;
		color.a = Mathf.Lerp(color.a, 1f, Time.deltaTime * this.fadeInSpeed);
		this.spiralSpriteRenderer.material.color = color;
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x000C0518 File Offset: 0x000BE718
	private void FadeOut()
	{
		Color color = this.spiralSpriteRenderer.material.color;
		color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * this.fadeOutSpeed);
		this.spiralSpriteRenderer.material.color = color;
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x000C056C File Offset: 0x000BE76C
	private void MakeTransparent()
	{
		Color color = this.spiralSpriteRenderer.material.color;
		color.a = 0f;
		this.spiralSpriteRenderer.material.color = color;
	}

	// Token: 0x040025CB RID: 9675
	public Transform spiral;

	// Token: 0x040025CC RID: 9676
	public SpriteRenderer spiralSpriteRenderer;

	// Token: 0x040025CD RID: 9677
	public float rotationSpeed = 1f;

	// Token: 0x040025CE RID: 9678
	public float fadeInSpeed = 1f;

	// Token: 0x040025CF RID: 9679
	public float fadeOutSpeed = 1f;

	// Token: 0x040025D0 RID: 9680
	private bool isActive;
}
