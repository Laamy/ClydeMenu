using System;
using UnityEngine;

// Token: 0x02000036 RID: 54
public class CameraCrouchRotation : MonoBehaviour
{
	// Token: 0x060000CA RID: 202 RVA: 0x00007AB4 File Offset: 0x00005CB4
	private void Start()
	{
		this.RotationLerp = 1f;
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00007AC4 File Offset: 0x00005CC4
	private void Update()
	{
		this.RotationLerp += Time.deltaTime * this.RotationSpeed;
		this.RotationLerp = Mathf.Clamp01(this.RotationLerp);
		float num;
		if (this.CameraCrouchPosition.Active)
		{
			num = this.RotationCurveIntro.Evaluate(this.RotationLerp) * this.Rotation;
		}
		else
		{
			num = this.RotationCurveOutro.Evaluate(this.RotationLerp) * this.Rotation;
		}
		num *= GameplayManager.instance.cameraAnimation;
		base.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
	}

	// Token: 0x0400020E RID: 526
	public CameraCrouchPosition CameraCrouchPosition;

	// Token: 0x0400020F RID: 527
	[Space]
	public float Rotation;

	// Token: 0x04000210 RID: 528
	public float RotationSpeed;

	// Token: 0x04000211 RID: 529
	public AnimationCurve RotationCurveIntro;

	// Token: 0x04000212 RID: 530
	public AnimationCurve RotationCurveOutro;

	// Token: 0x04000213 RID: 531
	[HideInInspector]
	public float RotationLerp;
}
