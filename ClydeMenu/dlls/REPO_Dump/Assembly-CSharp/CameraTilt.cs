using System;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class CameraTilt : MonoBehaviour
{
	// Token: 0x060000B0 RID: 176 RVA: 0x00006E9F File Offset: 0x0000509F
	private void Awake()
	{
		CameraTilt.Instance = this;
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00006EA8 File Offset: 0x000050A8
	private void Update()
	{
		if (SemiFunc.MenuLevel())
		{
			base.transform.localRotation = Quaternion.identity;
			return;
		}
		if (PlayerController.instance.Crouching)
		{
			this.AmountCurrent = Mathf.Lerp(this.AmountCurrent, this.Amount * this.CrouchMultiplier, Time.deltaTime * 5f);
		}
		else
		{
			this.AmountCurrent = Mathf.Lerp(this.AmountCurrent, this.Amount, Time.deltaTime * 5f);
		}
		float num = SemiFunc.InputMovementX();
		if (GameDirector.instance.DisableInput || SpectateCamera.instance || PlayerController.instance.InputDisableTimer > 0f)
		{
			num = 0f;
		}
		if (base.transform.rotation.x != this.previousX && base.transform.rotation.y != this.previousY)
		{
			if (Mathf.Abs(base.transform.rotation.eulerAngles.y - this.previousY) < 180f && Mathf.Abs(base.transform.rotation.eulerAngles.x - this.previousX) < 180f)
			{
				this.tiltXresult = (this.previousX - base.transform.rotation.eulerAngles.x) / Time.deltaTime * this.tiltX;
				this.tiltXresult = Mathf.Clamp(this.tiltXresult, -this.tiltXMax, this.tiltXMax);
				this.tiltZresult = (base.transform.rotation.eulerAngles.y - this.previousY) / Time.deltaTime * this.tiltZ + num * this.strafeAmount;
				this.tiltZresult = Mathf.Clamp(this.tiltZresult, -this.tiltZMax, this.tiltZMax);
				float num2 = 1f;
				if (SpectateCamera.instance)
				{
					num2 = 0.1f;
				}
				num2 *= GameplayManager.instance.cameraAnimation;
				this.targetAngle = Quaternion.Euler(this.tiltXresult * this.AmountCurrent * num2, 0f, this.tiltZresult * this.AmountCurrent * num2);
			}
			this.previousX = base.transform.rotation.eulerAngles.x;
			this.previousY = base.transform.rotation.eulerAngles.y;
		}
		float num3 = 3f;
		if (this.targetAngle == Quaternion.identity)
		{
			num3 = 10f;
		}
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, this.targetAngle, num3 * Time.deltaTime);
	}

	// Token: 0x040001C3 RID: 451
	public static CameraTilt Instance;

	// Token: 0x040001C4 RID: 452
	public float tiltZ = 250f;

	// Token: 0x040001C5 RID: 453
	public float tiltZMax = 10f;

	// Token: 0x040001C6 RID: 454
	[Space]
	public float tiltX = 250f;

	// Token: 0x040001C7 RID: 455
	public float tiltXMax = 10f;

	// Token: 0x040001C8 RID: 456
	[Space]
	public float strafeAmount = 1f;

	// Token: 0x040001C9 RID: 457
	public float CrouchMultiplier = 1f;

	// Token: 0x040001CA RID: 458
	private float Amount = 1f;

	// Token: 0x040001CB RID: 459
	private float AmountCurrent = 1f;

	// Token: 0x040001CC RID: 460
	private float previousX;

	// Token: 0x040001CD RID: 461
	private float previousY;

	// Token: 0x040001CE RID: 462
	private Quaternion targetAngle;

	// Token: 0x040001CF RID: 463
	[HideInInspector]
	public float tiltXresult;

	// Token: 0x040001D0 RID: 464
	[HideInInspector]
	public float tiltZresult;
}
