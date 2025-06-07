using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class DebugEnemyInvestigate : MonoBehaviour
{
	// Token: 0x060000DA RID: 218 RVA: 0x000081A4 File Offset: 0x000063A4
	private void Update()
	{
		this.lerp += Time.deltaTime;
		this.radiusCurrent = Mathf.Lerp(0f, this.radius, this.animationCurve.Evaluate(this.lerp));
		if (this.lerp >= 1f)
		{
			this.alpha -= Time.deltaTime;
			if (this.alpha <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00008224 File Offset: 0x00006424
	private void OnDrawGizmos()
	{
		base.transform.eulerAngles = Vector3.zero;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 1f, this.alpha);
		Gizmos.DrawWireSphere(Vector3.zero, 0.1f);
		Gizmos.color = new Color(1f, 0.62f, 0f, 0.23f * this.alpha);
		Gizmos.DrawSphere(Vector3.zero, this.radiusCurrent);
		Gizmos.color = new Color(1f, 0.62f, 0f, this.alpha);
		Gizmos.DrawWireSphere(Vector3.zero, this.radiusCurrent);
		base.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, this.radiusCurrent);
		base.transform.localEulerAngles = new Vector3(0f, 45f, 0f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, this.radiusCurrent);
		base.transform.localEulerAngles = new Vector3(0f, 0f, 45f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, this.radiusCurrent);
	}

	// Token: 0x04000227 RID: 551
	public AnimationCurve animationCurve;

	// Token: 0x04000228 RID: 552
	private float lerp;

	// Token: 0x04000229 RID: 553
	private float alpha = 1f;

	// Token: 0x0400022A RID: 554
	internal float radius = 1f;

	// Token: 0x0400022B RID: 555
	private float radiusCurrent = 2f;
}
