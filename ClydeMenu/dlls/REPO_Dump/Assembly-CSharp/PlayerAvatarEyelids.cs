using System;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class PlayerAvatarEyelids : MonoBehaviour
{
	// Token: 0x06000F4A RID: 3914 RVA: 0x000895D8 File Offset: 0x000877D8
	private void Start()
	{
		this.springUpper.damping = 0.01f;
		this.springUpper.speed = 40f;
		this.playerVisuals = base.GetComponentInParent<PlayerAvatarVisuals>();
		this.parentTransform = this.playerVisuals.transform;
		this.springLower.damping = 0.01f;
		this.springLower.speed = 40f;
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x00089644 File Offset: 0x00087844
	private void Update()
	{
		Quaternion localRotation = base.transform.localRotation;
		base.transform.localRotation = Quaternion.LookRotation(base.transform.forward, this.parentTransform.up);
		base.transform.localRotation = Quaternion.Euler(localRotation.x, localRotation.y, base.transform.localRotation.eulerAngles.z);
	}

	// Token: 0x04001910 RID: 6416
	public PlayerAvatarEyelids.Eye eye;

	// Token: 0x04001911 RID: 6417
	public Transform eyelidUpperScale;

	// Token: 0x04001912 RID: 6418
	public Transform eyelidUpperRotation;

	// Token: 0x04001913 RID: 6419
	public Transform eyelidUpper;

	// Token: 0x04001914 RID: 6420
	public Transform eyelidLowerScale;

	// Token: 0x04001915 RID: 6421
	public Transform eyelidLowerRotation;

	// Token: 0x04001916 RID: 6422
	public Transform eyelidLower;

	// Token: 0x04001917 RID: 6423
	internal float eyelidUpperClosedPercentage;

	// Token: 0x04001918 RID: 6424
	internal float eyelidLowerClosedPercentage;

	// Token: 0x04001919 RID: 6425
	private SpringFloat springUpper = new SpringFloat();

	// Token: 0x0400191A RID: 6426
	private SpringFloat springUpperScale = new SpringFloat();

	// Token: 0x0400191B RID: 6427
	private SpringFloat springUpperRotation = new SpringFloat();

	// Token: 0x0400191C RID: 6428
	private SpringFloat springLower = new SpringFloat();

	// Token: 0x0400191D RID: 6429
	private SpringFloat springLowerScale = new SpringFloat();

	// Token: 0x0400191E RID: 6430
	private SpringFloat springLowerRotation = new SpringFloat();

	// Token: 0x0400191F RID: 6431
	private PlayerAvatarVisuals playerVisuals;

	// Token: 0x04001920 RID: 6432
	private Transform parentTransform;

	// Token: 0x020003B7 RID: 951
	public enum Eye
	{
		// Token: 0x04002C23 RID: 11299
		Left,
		// Token: 0x04002C24 RID: 11300
		Right
	}
}
