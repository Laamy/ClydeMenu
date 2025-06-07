using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class MapBacktrackPoint : MonoBehaviour
{
	// Token: 0x06000D7E RID: 3454 RVA: 0x00076516 File Offset: 0x00074716
	private void Awake()
	{
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00076528 File Offset: 0x00074728
	public void Show(bool _sameLayer)
	{
		Color color = this.spriteRenderer.color;
		if (_sameLayer)
		{
			color.a = 1f;
		}
		else
		{
			color.a = 0.2f;
		}
		this.spriteRenderer.color = color;
		base.StopCoroutine(this.Animate());
		base.StartCoroutine(this.Animate());
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x00076583 File Offset: 0x00074783
	private IEnumerator Animate()
	{
		this.animating = true;
		this.lerp = 0f;
		for (;;)
		{
			this.lerp += Time.deltaTime * this.speed;
			base.transform.localScale = Vector3.one * this.curve.Evaluate(this.lerp);
			if (this.lerp >= 1f)
			{
				break;
			}
			yield return new WaitForSeconds(0.05f);
		}
		this.animating = false;
		yield break;
	}

	// Token: 0x040015A8 RID: 5544
	public SpriteRenderer spriteRenderer;

	// Token: 0x040015A9 RID: 5545
	public AnimationCurve curve;

	// Token: 0x040015AA RID: 5546
	public float speed;

	// Token: 0x040015AB RID: 5547
	private float lerp;

	// Token: 0x040015AC RID: 5548
	public bool animating;
}
