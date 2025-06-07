using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class TruckHealerLine : MonoBehaviour
{
	// Token: 0x060008B8 RID: 2232 RVA: 0x0005432D File Offset: 0x0005252D
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.widthMultiplier = 0f;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0005434C File Offset: 0x0005254C
	private void Update()
	{
		if (!this.lineTarget)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (this.curveEval >= 1f)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.curveEval += Time.deltaTime * 2.5f;
		this.lineRenderer.widthMultiplier = this.widthCurve.Evaluate(this.curveEval);
		if (this.lineTarget)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = this.lineTarget.position;
			Vector3[] array = new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 19f;
				array[i] = Vector3.Lerp(position, position2, num) - Vector3.up * Mathf.Sin(num * 3.1415927f) * 0.5f;
				float d = 1f - Mathf.Abs(num - 0.5f) * 2f;
				float num2 = 1f;
				float d2 = this.wobbleCurve.Evaluate(num) * 2f;
				array[i] += Vector3.right * Mathf.Sin(Time.time * (30f * num2) + (float)i) * 0.02f * d * d2;
				array[i] += Vector3.forward * Mathf.Cos(Time.time * (30f * num2) + (float)i) * 0.02f * d * d2;
			}
			this.lineRenderer.material.mainTextureOffset = new Vector2(-Time.time * 2f, 0f);
			this.lineRenderer.positionCount = 20;
			this.lineRenderer.SetPositions(array);
			return;
		}
		this.outro = true;
	}

	// Token: 0x04000FE6 RID: 4070
	public Transform lineTarget;

	// Token: 0x04000FE7 RID: 4071
	private LineRenderer lineRenderer;

	// Token: 0x04000FE8 RID: 4072
	public AnimationCurve wobbleCurve;

	// Token: 0x04000FE9 RID: 4073
	public AnimationCurve widthCurve;

	// Token: 0x04000FEA RID: 4074
	private float curveEval;

	// Token: 0x04000FEB RID: 4075
	internal bool outro;
}
