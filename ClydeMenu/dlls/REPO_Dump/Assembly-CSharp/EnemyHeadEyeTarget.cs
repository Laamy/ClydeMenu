using System;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class EnemyHeadEyeTarget : MonoBehaviour
{
	// Token: 0x06000306 RID: 774 RVA: 0x0001E410 File Offset: 0x0001C610
	private void Start()
	{
		this.Camera = Camera.main;
	}

	// Token: 0x06000307 RID: 775 RVA: 0x0001E420 File Offset: 0x0001C620
	private void Update()
	{
		if (!this.Enemy.CheckChase())
		{
			this.Idle = true;
		}
		else
		{
			this.Idle = false;
		}
		if (this.Idle || !this.Enemy.TargetPlayerAvatar)
		{
			base.transform.position = this.Follow.position + this.Follow.forward * this.IdleOffset;
		}
		else
		{
			base.transform.position = this.Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position;
		}
		base.transform.rotation = this.Follow.rotation;
		float num = Vector3.Distance(this.Enemy.transform.position, this.Camera.transform.position) * this.PupilSizeMultiplier;
		num = Mathf.Clamp(num, this.PupilMinSize, this.PupilMaxSize);
		this.PupilCurrentSize = Mathf.Lerp(this.PupilCurrentSize, num, this.PupilSizeSpeed * Time.deltaTime);
	}

	// Token: 0x06000308 RID: 776 RVA: 0x0001E530 File Offset: 0x0001C730
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		MeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = this.DebugShow;
		}
	}

	// Token: 0x04000538 RID: 1336
	public Enemy Enemy;

	// Token: 0x04000539 RID: 1337
	public Transform Follow;

	// Token: 0x0400053A RID: 1338
	[Space]
	public Vector3 Limit;

	// Token: 0x0400053B RID: 1339
	public float Speed;

	// Token: 0x0400053C RID: 1340
	public bool DebugShow;

	// Token: 0x0400053D RID: 1341
	[Space]
	public bool Idle = true;

	// Token: 0x0400053E RID: 1342
	public float IdleOffset;

	// Token: 0x0400053F RID: 1343
	[Space]
	public float PupilSizeMultiplier;

	// Token: 0x04000540 RID: 1344
	public float PupilSizeSpeed;

	// Token: 0x04000541 RID: 1345
	public float PupilMinSize;

	// Token: 0x04000542 RID: 1346
	public float PupilMaxSize;

	// Token: 0x04000543 RID: 1347
	[HideInInspector]
	public float PupilCurrentSize;

	// Token: 0x04000544 RID: 1348
	private Camera Camera;
}
