using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class DirtFinderMapFloor : MonoBehaviour
{
	// Token: 0x06000D8D RID: 3469 RVA: 0x00076826 File Offset: 0x00074A26
	private void Start()
	{
		base.StartCoroutine(this.Add());
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x00076835 File Offset: 0x00074A35
	private IEnumerator Add()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Map.Instance.AddFloor(this);
		yield break;
	}

	// Token: 0x040015BF RID: 5567
	public DirtFinderMapFloor.FloorType Type;

	// Token: 0x040015C0 RID: 5568
	internal MapObject MapObject;

	// Token: 0x0200039B RID: 923
	public enum FloorType
	{
		// Token: 0x04002BA8 RID: 11176
		Floor_1x1,
		// Token: 0x04002BA9 RID: 11177
		Floor_1x1_Diagonal,
		// Token: 0x04002BAA RID: 11178
		Floor_1x05,
		// Token: 0x04002BAB RID: 11179
		Floor_1x025,
		// Token: 0x04002BAC RID: 11180
		Floor_1x05_Diagonal,
		// Token: 0x04002BAD RID: 11181
		Floor_1x025_Diagonal,
		// Token: 0x04002BAE RID: 11182
		Truck_Floor,
		// Token: 0x04002BAF RID: 11183
		Truck_Wall,
		// Token: 0x04002BB0 RID: 11184
		Used_Floor,
		// Token: 0x04002BB1 RID: 11185
		Used_Wall,
		// Token: 0x04002BB2 RID: 11186
		Inactive_Floor,
		// Token: 0x04002BB3 RID: 11187
		Inactive_Wall,
		// Token: 0x04002BB4 RID: 11188
		Floor_1x1_Curve,
		// Token: 0x04002BB5 RID: 11189
		Floor_1x1_Curve_Inverted,
		// Token: 0x04002BB6 RID: 11190
		Floor_1x05_Curve,
		// Token: 0x04002BB7 RID: 11191
		Floor_1x05_Curve_Inverted
	}
}
