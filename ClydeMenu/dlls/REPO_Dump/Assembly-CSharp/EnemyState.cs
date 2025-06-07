using System;

// Token: 0x0200008A RID: 138
public enum EnemyState
{
	// Token: 0x0400090F RID: 2319
	None,
	// Token: 0x04000910 RID: 2320
	Spawn,
	// Token: 0x04000911 RID: 2321
	Roaming,
	// Token: 0x04000912 RID: 2322
	ChaseBegin,
	// Token: 0x04000913 RID: 2323
	Chase,
	// Token: 0x04000914 RID: 2324
	ChaseSlow,
	// Token: 0x04000915 RID: 2325
	ChaseEnd,
	// Token: 0x04000916 RID: 2326
	Investigate,
	// Token: 0x04000917 RID: 2327
	Sneak,
	// Token: 0x04000918 RID: 2328
	Stunned,
	// Token: 0x04000919 RID: 2329
	LookUnder,
	// Token: 0x0400091A RID: 2330
	Despawn
}
