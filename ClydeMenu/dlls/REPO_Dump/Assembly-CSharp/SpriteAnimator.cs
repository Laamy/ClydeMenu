using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class SpriteAnimator : MonoBehaviour
{
	// Token: 0x0600135B RID: 4955 RVA: 0x000ACBA1 File Offset: 0x000AADA1
	private void Start()
	{
		if (this.spriteRenderer == null)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		}
		this.secondsPerFrame = 1f / (float)this.framesPerSecond;
		base.StartCoroutine(this.AnimateSprite());
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x000ACBDD File Offset: 0x000AADDD
	private IEnumerator AnimateSprite()
	{
		while (this.isAnimating)
		{
			this.spriteRenderer.sprite = this.animationSprites[this.currentSpriteIndex];
			this.currentSpriteIndex = (this.currentSpriteIndex + 1) % this.animationSprites.Count;
			yield return new WaitForSeconds(this.secondsPerFrame);
		}
		yield break;
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x000ACBEC File Offset: 0x000AADEC
	private void OnDisable()
	{
		this.isAnimating = false;
	}

	// Token: 0x04002104 RID: 8452
	public SpriteRenderer spriteRenderer;

	// Token: 0x04002105 RID: 8453
	public List<Sprite> animationSprites;

	// Token: 0x04002106 RID: 8454
	public int framesPerSecond = 12;

	// Token: 0x04002107 RID: 8455
	private int currentSpriteIndex;

	// Token: 0x04002108 RID: 8456
	private bool isAnimating = true;

	// Token: 0x04002109 RID: 8457
	private float secondsPerFrame;
}
