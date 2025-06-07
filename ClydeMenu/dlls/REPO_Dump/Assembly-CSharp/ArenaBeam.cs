using System;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class ArenaBeam : MonoBehaviour
{
	// Token: 0x06000789 RID: 1929 RVA: 0x00047E98 File Offset: 0x00046098
	private void Start()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.widthMultiplier = 0f;
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00047EB8 File Offset: 0x000460B8
	private void Update()
	{
		if (this.lineTarget)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = this.lineTarget.position;
			Vector3[] array = new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 19f;
				array[i] = Vector3.Lerp(position, position2, num);
				float d = 1f - Mathf.Abs(num - 0.3f) * 2f;
				float num2 = 1f;
				array[i] += Vector3.right * Mathf.Sin(Time.time * (30f * num2) + (float)i) * 0.05f * d;
				array[i] += Vector3.forward * Mathf.Cos(Time.time * (30f * num2) + (float)i) * 0.05f * d;
			}
			this.lineRenderer.material.mainTextureOffset = new Vector2(Time.time * 2f, 0f);
			this.lineRenderer.positionCount = 20;
			this.lineRenderer.SetPositions(array);
		}
		else
		{
			this.outro = true;
		}
		if (!this.outro)
		{
			if (this.lineRenderer.widthMultiplier < 0.195f)
			{
				this.lineRenderer.widthMultiplier = Mathf.Lerp(this.lineRenderer.widthMultiplier, 0.2f, Time.deltaTime * 2f);
				return;
			}
			this.lineRenderer.widthMultiplier = 0.2f;
			return;
		}
		else
		{
			if (this.lineRenderer.widthMultiplier > 0.005f)
			{
				this.lineRenderer.widthMultiplier = Mathf.Lerp(this.lineRenderer.widthMultiplier, 0f, Time.deltaTime * 2f);
				return;
			}
			this.lineRenderer.widthMultiplier = 0f;
			base.transform.parent.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x04000D58 RID: 3416
	public Transform lineTarget;

	// Token: 0x04000D59 RID: 3417
	private LineRenderer lineRenderer;

	// Token: 0x04000D5A RID: 3418
	private PhysGrabObject physGrabObject;

	// Token: 0x04000D5B RID: 3419
	internal bool outro;
}
