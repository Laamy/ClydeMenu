using System;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class EnemyThinManTentacle : MonoBehaviour
{
	// Token: 0x0600053D RID: 1341 RVA: 0x00033968 File Offset: 0x00031B68
	private void Update()
	{
		if (this.rotationLerp >= 1f)
		{
			this.rotationStartPos = base.transform.localRotation;
			this.rotationEndPos = Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
			this.rotationLerpSpeed = Random.Range(1f, 2f);
			this.rotationLerp = 0f;
		}
		else
		{
			this.rotationLerp += Time.deltaTime * this.rotationLerpSpeed;
			base.transform.localRotation = Quaternion.Lerp(this.rotationStartPos, this.rotationEndPos, this.wiggleCurve.Evaluate(this.rotationLerp));
		}
		if (this.scaleLerp >= 1f)
		{
			this.scaleStartPos = base.transform.localScale;
			this.scaleEndPos = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
			this.scaleLerpSpeed = Random.Range(2f, 4f);
			this.scaleLerp = 0f;
			return;
		}
		this.scaleLerp += Time.deltaTime * this.scaleLerpSpeed;
		base.transform.localScale = Vector3.Lerp(this.scaleStartPos, this.scaleEndPos, this.wiggleCurve.Evaluate(this.scaleLerp));
	}

	// Token: 0x04000871 RID: 2161
	public AnimationCurve wiggleCurve;

	// Token: 0x04000872 RID: 2162
	private float rotationLerp = 1f;

	// Token: 0x04000873 RID: 2163
	private float rotationLerpSpeed;

	// Token: 0x04000874 RID: 2164
	private Quaternion rotationStartPos;

	// Token: 0x04000875 RID: 2165
	private Quaternion rotationEndPos;

	// Token: 0x04000876 RID: 2166
	private float scaleLerp = 1f;

	// Token: 0x04000877 RID: 2167
	private float scaleLerpSpeed;

	// Token: 0x04000878 RID: 2168
	private Vector3 scaleStartPos;

	// Token: 0x04000879 RID: 2169
	private Vector3 scaleEndPos;
}
