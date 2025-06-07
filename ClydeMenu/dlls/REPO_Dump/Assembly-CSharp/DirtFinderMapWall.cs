using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class DirtFinderMapWall : MonoBehaviour
{
	// Token: 0x06000D98 RID: 3480 RVA: 0x000768C7 File Offset: 0x00074AC7
	private void Start()
	{
		base.StartCoroutine(this.Add());
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x000768D6 File Offset: 0x00074AD6
	private IEnumerator Add()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Map.Instance.AddWall(this);
		yield break;
	}

	// Token: 0x040015C6 RID: 5574
	public DirtFinderMapWall.WallType Type;

	// Token: 0x020003A0 RID: 928
	public enum WallType
	{
		// Token: 0x04002BC5 RID: 11205
		Wall_1x1,
		// Token: 0x04002BC6 RID: 11206
		Door_1x1,
		// Token: 0x04002BC7 RID: 11207
		Door_1x2,
		// Token: 0x04002BC8 RID: 11208
		Door_Blocked,
		// Token: 0x04002BC9 RID: 11209
		Door_1x1_Diagonal,
		// Token: 0x04002BCA RID: 11210
		Wall_1x05,
		// Token: 0x04002BCB RID: 11211
		Wall_1x025,
		// Token: 0x04002BCC RID: 11212
		Wall_1x1_Diagonal,
		// Token: 0x04002BCD RID: 11213
		Wall_1x05_Diagonal,
		// Token: 0x04002BCE RID: 11214
		Wall_1x025_Diagonal,
		// Token: 0x04002BCF RID: 11215
		Door_1x05_Diagonal,
		// Token: 0x04002BD0 RID: 11216
		Door_1x1_Wizard,
		// Token: 0x04002BD1 RID: 11217
		Door_Blocked_Wizard,
		// Token: 0x04002BD2 RID: 11218
		Stairs,
		// Token: 0x04002BD3 RID: 11219
		Door_1x05,
		// Token: 0x04002BD4 RID: 11220
		Door_1x1_Arctic,
		// Token: 0x04002BD5 RID: 11221
		Door_Blocked_Arctic,
		// Token: 0x04002BD6 RID: 11222
		Wall_1x1_Curve,
		// Token: 0x04002BD7 RID: 11223
		Wall_1x05_Curve
	}
}
