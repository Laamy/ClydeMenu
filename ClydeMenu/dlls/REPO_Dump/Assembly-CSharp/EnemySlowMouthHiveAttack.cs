using System;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class EnemySlowMouthHiveAttack : MonoBehaviour
{
	// Token: 0x06000417 RID: 1047 RVA: 0x00028D89 File Offset: 0x00026F89
	private void Start()
	{
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00028D8C File Offset: 0x00026F8C
	private void Update()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = this.hitPositionTransform.position;
		if (this.curveProgress < 1f)
		{
			this.curveProgress += Time.deltaTime * 2f;
			Vector3 position3 = Vector3.Lerp(position, position2, this.curveProgress);
			this.blobTransform.position = position3;
			float d = this.flyUpCurve.Evaluate(this.curveProgress);
			this.blobTransform.position += Vector3.up * 2f * d;
			if (Vector3.Distance(this.prevCheckPosition, this.blobTransform.position) > 0.5f)
			{
				Collider[] array = Physics.OverlapSphere(this.blobTransform.position, this.blobMeshTransform.localScale.x / 2f, SemiFunc.LayerMaskGetShouldHits());
				for (int i = 0; i < array.Length; i++)
				{
					EnemyParent componentInParent = array[i].GetComponentInParent<EnemyParent>();
					if (!componentInParent || !(componentInParent == this.enemyParent))
					{
						this.Splat();
						break;
					}
				}
				this.prevCheckPosition = this.blobTransform.position;
				return;
			}
		}
		else
		{
			this.curveProgress = 0f;
		}
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00028EDE File Offset: 0x000270DE
	private void Splat()
	{
		this.curveProgress = 0f;
	}

	// Token: 0x040006D8 RID: 1752
	public Transform hitPositionTransform;

	// Token: 0x040006D9 RID: 1753
	public Transform blobTransform;

	// Token: 0x040006DA RID: 1754
	public Transform blobMeshTransform;

	// Token: 0x040006DB RID: 1755
	public AnimationCurve flyUpCurve;

	// Token: 0x040006DC RID: 1756
	private float curveProgress;

	// Token: 0x040006DD RID: 1757
	private Vector3 prevCheckPosition;

	// Token: 0x040006DE RID: 1758
	public EnemyParent enemyParent;
}
