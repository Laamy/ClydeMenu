using System;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class EnemyChecklist : MonoBehaviour
{
	// Token: 0x060005E2 RID: 1506 RVA: 0x00039D18 File Offset: 0x00037F18
	private void ResetChecklist()
	{
		this.difficulty = false;
		this.type = false;
		this.center = false;
		this.killLookAt = false;
		this.sightingStinger = false;
		this.enemyNearMusic = false;
		this.healthMax = false;
		this.healthMeshParent = false;
		this.healthOnHurt = false;
		this.healthOnDeath = false;
		this.healthImpact = false;
		this.healthObject = false;
		this.rigidbodyPhysAttribute = false;
		this.rigidbodyAudioPreset = false;
		this.rigidbodyColliders = false;
		this.rigidbodyFollow = false;
		this.rigidbodyCustomGravity = false;
		this.rigidbodyGrab = false;
		this.rigidbodyPositionFollow = false;
		this.rigidbodyRotationFollow = false;
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00039DB4 File Offset: 0x00037FB4
	private void SetAllChecklist()
	{
		this.difficulty = true;
		this.type = true;
		this.center = true;
		this.killLookAt = true;
		this.sightingStinger = true;
		this.enemyNearMusic = true;
		this.healthMax = true;
		this.healthMeshParent = true;
		this.healthOnHurt = true;
		this.healthOnDeath = true;
		this.healthImpact = true;
		this.healthObject = true;
		this.rigidbodyPhysAttribute = true;
		this.rigidbodyAudioPreset = true;
		this.rigidbodyColliders = true;
		this.rigidbodyFollow = true;
		this.rigidbodyCustomGravity = true;
		this.rigidbodyGrab = true;
		this.rigidbodyPositionFollow = true;
		this.rigidbodyRotationFollow = true;
	}

	// Token: 0x0400095D RID: 2397
	private Color colorPositive = Color.green;

	// Token: 0x0400095E RID: 2398
	private Color colorNegative = new Color(1f, 0.74f, 0.61f);

	// Token: 0x0400095F RID: 2399
	[Space]
	public bool hasRigidbody;

	// Token: 0x04000960 RID: 2400
	public new bool name;

	// Token: 0x04000961 RID: 2401
	public bool difficulty;

	// Token: 0x04000962 RID: 2402
	public bool type;

	// Token: 0x04000963 RID: 2403
	public bool center;

	// Token: 0x04000964 RID: 2404
	public bool killLookAt;

	// Token: 0x04000965 RID: 2405
	public bool sightingStinger;

	// Token: 0x04000966 RID: 2406
	public bool enemyNearMusic;

	// Token: 0x04000967 RID: 2407
	public bool healthMax;

	// Token: 0x04000968 RID: 2408
	public bool healthMeshParent;

	// Token: 0x04000969 RID: 2409
	public bool healthOnHurt;

	// Token: 0x0400096A RID: 2410
	public bool healthOnDeath;

	// Token: 0x0400096B RID: 2411
	public bool healthImpact;

	// Token: 0x0400096C RID: 2412
	public bool healthObject;

	// Token: 0x0400096D RID: 2413
	public bool rigidbodyPhysAttribute;

	// Token: 0x0400096E RID: 2414
	public bool rigidbodyAudioPreset;

	// Token: 0x0400096F RID: 2415
	public bool rigidbodyColliders;

	// Token: 0x04000970 RID: 2416
	public bool rigidbodyFollow;

	// Token: 0x04000971 RID: 2417
	public bool rigidbodyCustomGravity;

	// Token: 0x04000972 RID: 2418
	public bool rigidbodyGrab;

	// Token: 0x04000973 RID: 2419
	public bool rigidbodyPositionFollow;

	// Token: 0x04000974 RID: 2420
	public bool rigidbodyRotationFollow;
}
