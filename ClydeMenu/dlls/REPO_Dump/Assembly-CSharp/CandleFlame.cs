using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class CandleFlame : MonoBehaviour
{
	// Token: 0x06000930 RID: 2352 RVA: 0x00057F78 File Offset: 0x00056178
	private void Awake()
	{
		if (!this.propLight || !this.propLight.lightComponent)
		{
			Debug.LogError("Candle Flame missing Prop Light!", base.gameObject);
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.logicActive)
		{
			base.StartCoroutine(this.LogicCoroutine());
		}
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00057FD6 File Offset: 0x000561D6
	private void OnEnable()
	{
		if (!this.logicActive)
		{
			base.StartCoroutine(this.LogicCoroutine());
		}
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00057FED File Offset: 0x000561ED
	private void OnDisable()
	{
		this.logicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00057FFC File Offset: 0x000561FC
	private IEnumerator LogicCoroutine()
	{
		this.logicActive = true;
		for (;;)
		{
			yield return new WaitForSeconds(this.Logic());
		}
		yield break;
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x0005800C File Offset: 0x0005620C
	private float Logic()
	{
		if (this.propLight.turnedOff)
		{
			base.gameObject.SetActive(false);
			return 999f;
		}
		if (this.propLight.lightComponent.intensity > 0f)
		{
			this.flickerXLerp += Time.deltaTime * this.flickerXSpeed;
			if (this.flickerXLerp >= 1f)
			{
				this.flickerXLerp = 0f;
				this.flickerXOld = this.flickerXNew;
				this.flickerXNew = Random.Range(0.8f, 1.2f);
				this.flickerXSpeed = Random.Range(25f, 75f);
			}
			this.flickerYLerp += Time.deltaTime * this.flickerYSpeed;
			if (this.flickerYLerp >= 1f)
			{
				this.flickerYLerp = 0f;
				this.flickerYOld = this.flickerYNew;
				this.flickerYNew = Random.Range(0.8f, 1.2f);
				this.flickerYSpeed = Random.Range(25f, 75f);
			}
			this.flickerZLerp += Time.deltaTime * this.flickerZSpeed;
			if (this.flickerZLerp >= 1f)
			{
				this.flickerZLerp = 0f;
				this.flickerZOld = this.flickerZNew;
				this.flickerZNew = Random.Range(0.8f, 1.2f);
				this.flickerZSpeed = Random.Range(25f, 75f);
			}
			base.transform.localScale = new Vector3(Mathf.Lerp(this.flickerXOld, this.flickerXNew, this.flickerCurve.Evaluate(this.flickerXLerp)), Mathf.Lerp(this.flickerYOld, this.flickerYNew, this.flickerCurve.Evaluate(this.flickerYLerp)), Mathf.Lerp(this.flickerZOld, this.flickerZNew, this.flickerCurve.Evaluate(this.flickerZLerp)));
			return 0.025f;
		}
		return 2f;
	}

	// Token: 0x040010BD RID: 4285
	private bool logicActive;

	// Token: 0x040010BE RID: 4286
	public PropLight propLight;

	// Token: 0x040010BF RID: 4287
	public AnimationCurve flickerCurve;

	// Token: 0x040010C0 RID: 4288
	public AnimationCurve swayCurve;

	// Token: 0x040010C1 RID: 4289
	private float flickerXNew;

	// Token: 0x040010C2 RID: 4290
	private float flickerXOld;

	// Token: 0x040010C3 RID: 4291
	private float flickerXLerp = 1f;

	// Token: 0x040010C4 RID: 4292
	private float flickerXSpeed;

	// Token: 0x040010C5 RID: 4293
	private float flickerYNew;

	// Token: 0x040010C6 RID: 4294
	private float flickerYOld;

	// Token: 0x040010C7 RID: 4295
	private float flickerYLerp = 1f;

	// Token: 0x040010C8 RID: 4296
	private float flickerYSpeed;

	// Token: 0x040010C9 RID: 4297
	private float flickerZNew;

	// Token: 0x040010CA RID: 4298
	private float flickerZOld;

	// Token: 0x040010CB RID: 4299
	private float flickerZLerp = 1f;

	// Token: 0x040010CC RID: 4300
	private float flickerZSpeed;
}
