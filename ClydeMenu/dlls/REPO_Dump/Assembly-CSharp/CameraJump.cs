using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class CameraJump : MonoBehaviour
{
	// Token: 0x06000093 RID: 147 RVA: 0x0000604A File Offset: 0x0000424A
	private void Awake()
	{
		CameraJump.instance = this;
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00006054 File Offset: 0x00004254
	public void Jump()
	{
		GameDirector.instance.CameraImpact.Shake(1f, 0.05f);
		GameDirector.instance.CameraShake.Shake(2f, 0.1f);
		this.jumpActive = true;
		this.jumpLerp = 0f;
	}

	// Token: 0x06000095 RID: 149 RVA: 0x000060A8 File Offset: 0x000042A8
	public void Land()
	{
		if (this.landActive)
		{
			return;
		}
		GameDirector.instance.CameraImpact.Shake(1f, 0.05f);
		GameDirector.instance.CameraShake.Shake(2f, 0.1f);
		this.landActive = true;
		this.landLerp = 0f;
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00006104 File Offset: 0x00004304
	private void Update()
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if (this.jumpActive)
		{
			if (this.jumpLerp >= 1f)
			{
				this.jumpActive = false;
				this.jumpLerp = 0f;
			}
			else
			{
				vector += Vector3.LerpUnclamped(Vector3.zero, this.jumpPosition, this.jumpCurve.Evaluate(this.jumpLerp));
				vector2 += Vector3.LerpUnclamped(Vector3.zero, this.jumpRotation, this.jumpCurve.Evaluate(this.jumpLerp));
				this.jumpLerp += this.jumpSpeed * Time.deltaTime;
			}
		}
		if (this.landActive)
		{
			if (this.landLerp >= 1f)
			{
				this.landActive = false;
				this.landLerp = 0f;
			}
			else
			{
				vector += Vector3.LerpUnclamped(Vector3.zero, this.landPosition, this.landCurve.Evaluate(this.landLerp));
				vector2 += Vector3.LerpUnclamped(Vector3.zero, this.landRotation, this.landCurve.Evaluate(this.landLerp));
				this.landLerp += this.landSpeed * Time.deltaTime;
			}
		}
		vector *= GameplayManager.instance.cameraAnimation;
		vector2 *= GameplayManager.instance.cameraAnimation;
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, vector, 30f * Time.deltaTime);
		Quaternion localRotation = base.transform.localRotation;
		base.transform.localEulerAngles = vector2;
		Quaternion localRotation2 = base.transform.localRotation;
		base.transform.localRotation = localRotation;
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, localRotation2, 30f * Time.deltaTime);
	}

	// Token: 0x0400017E RID: 382
	public static CameraJump instance;

	// Token: 0x0400017F RID: 383
	internal bool jumpActive;

	// Token: 0x04000180 RID: 384
	public AnimationCurve jumpCurve;

	// Token: 0x04000181 RID: 385
	public float jumpSpeed = 1f;

	// Token: 0x04000182 RID: 386
	private float jumpLerp;

	// Token: 0x04000183 RID: 387
	public Vector3 jumpPosition;

	// Token: 0x04000184 RID: 388
	public Vector3 jumpRotation;

	// Token: 0x04000185 RID: 389
	[Space]
	private bool landActive;

	// Token: 0x04000186 RID: 390
	public AnimationCurve landCurve;

	// Token: 0x04000187 RID: 391
	public float landSpeed = 1f;

	// Token: 0x04000188 RID: 392
	private float landLerp;

	// Token: 0x04000189 RID: 393
	public Vector3 landPosition;

	// Token: 0x0400018A RID: 394
	public Vector3 landRotation;
}
