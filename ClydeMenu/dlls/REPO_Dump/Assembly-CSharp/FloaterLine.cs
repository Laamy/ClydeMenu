using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class FloaterLine : MonoBehaviour
{
	// Token: 0x0600027C RID: 636 RVA: 0x00019A65 File Offset: 0x00017C65
	private void Start()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.widthMultiplier = 0f;
		this.physGrabObject = this.lineTarget.GetComponent<PhysGrabObject>();
	}

	// Token: 0x0600027D RID: 637 RVA: 0x00019A94 File Offset: 0x00017C94
	private void Update()
	{
		if (this.lineTarget)
		{
			Vector3 position = base.transform.position;
			Vector3 b = this.lineTarget.position;
			if (this.physGrabObject)
			{
				b = this.physGrabObject.midPoint;
			}
			Vector3[] array = new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 20f;
				array[i] = Vector3.Lerp(position, b, num) + Vector3.up * Mathf.Sin(num * 3.1415927f) * 1f;
				float num2 = 1f - Mathf.Abs(num - 0.5f) * 2f;
				float num3 = 1f;
				if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop)
				{
					num2 *= 3f;
					num3 = 2f;
				}
				array[i] += Vector3.right * Mathf.Sin(Time.time * (30f * num3) + (float)i) * 0.02f * num2;
				array[i] += Vector3.forward * Mathf.Cos(Time.time * (30f * num3) + (float)i) * 0.02f * num2;
			}
			this.lineRenderer.material.mainTextureOffset = new Vector2(Time.time * 2f, 0f);
			this.lineRenderer.positionCount = 20;
			this.lineRenderer.SetPositions(array);
		}
		else
		{
			this.outro = true;
		}
		if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop && !this.redMaterialSet)
		{
			this.lineRenderer.material = this.redMaterial;
			this.redMaterialSet = true;
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
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x0400048B RID: 1163
	public Transform lineTarget;

	// Token: 0x0400048C RID: 1164
	private LineRenderer lineRenderer;

	// Token: 0x0400048D RID: 1165
	private PhysGrabObject physGrabObject;

	// Token: 0x0400048E RID: 1166
	internal FloaterAttackLogic floaterAttack;

	// Token: 0x0400048F RID: 1167
	internal bool outro;

	// Token: 0x04000490 RID: 1168
	public Material redMaterial;

	// Token: 0x04000491 RID: 1169
	internal bool redMaterialSet;
}
