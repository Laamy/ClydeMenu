using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class ToolBackAway : MonoBehaviour
{
	// Token: 0x06000734 RID: 1844 RVA: 0x00044CC9 File Offset: 0x00042EC9
	private void Start()
	{
		this.StartPosition = base.transform.localPosition;
		this.Mask = SemiFunc.LayerMaskGetVisionObstruct();
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x00044CE8 File Offset: 0x00042EE8
	private void FixedUpdate()
	{
		if (this.Active)
		{
			if (this.RaycastTimer <= 0f)
			{
				this.RaycastTimer = this.RaycastTime;
				this.LengthHit = this.Length;
				foreach (RaycastHit raycastHit in Physics.RaycastAll(this.ParentTransform.position, this.ParentTransform.forward, this.Length, this.Mask))
				{
					if ((!raycastHit.transform.CompareTag("Player") || !raycastHit.transform.GetComponent<PlayerController>()) && raycastHit.distance < this.LengthHit)
					{
						this.LengthHit = raycastHit.distance;
					}
				}
				return;
			}
			this.RaycastTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x00044DC0 File Offset: 0x00042FC0
	private void Update()
	{
		if (this.Active)
		{
			this.BackAwayAmount = Mathf.Max(-this.BackAwayAmountMax, this.LengthHit - this.Length);
		}
		else
		{
			this.BackAwayAmount = 0f;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref this.springParams, Time.deltaTime, this.springFreq, this.springDamping);
		SpringUtils.UpdateDampedSpringMotion(ref this.current, ref this.velocity, this.BackAwayAmount, this.springParams);
		base.transform.localPosition = new Vector3(this.StartPosition.x, this.StartPosition.y, this.StartPosition.z + this.current);
	}

	// Token: 0x04000C8F RID: 3215
	public bool Active;

	// Token: 0x04000C90 RID: 3216
	public Transform ParentTransform;

	// Token: 0x04000C91 RID: 3217
	private LayerMask Mask;

	// Token: 0x04000C92 RID: 3218
	private float RaycastTime = 0.1f;

	// Token: 0x04000C93 RID: 3219
	private float RaycastTimer;

	// Token: 0x04000C94 RID: 3220
	public float Length = 1f;

	// Token: 0x04000C95 RID: 3221
	private float LengthHit;

	// Token: 0x04000C96 RID: 3222
	private float BackAwayAmount;

	// Token: 0x04000C97 RID: 3223
	public float BackAwayAmountMax;

	// Token: 0x04000C98 RID: 3224
	private Vector3 StartPosition;

	// Token: 0x04000C99 RID: 3225
	public float springFreq = 15f;

	// Token: 0x04000C9A RID: 3226
	public float springDamping = 0.5f;

	// Token: 0x04000C9B RID: 3227
	private float target;

	// Token: 0x04000C9C RID: 3228
	private float current;

	// Token: 0x04000C9D RID: 3229
	private float velocity;

	// Token: 0x04000C9E RID: 3230
	private SpringUtils.tDampedSpringMotionParams springParams = new SpringUtils.tDampedSpringMotionParams();
}
