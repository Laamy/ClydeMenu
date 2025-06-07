using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000247 RID: 583
public static class ListExtension
{
	// Token: 0x060012F8 RID: 4856 RVA: 0x000A9C54 File Offset: 0x000A7E54
	public static void Shuffle<T>(this IList<T> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list.Swap(i, Random.Range(0, list.Count));
		}
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x000A9C88 File Offset: 0x000A7E88
	public static void Swap<T>(this IList<T> list, int i, int j)
	{
		T t = list[j];
		T t2 = list[i];
		list[i] = t;
		list[j] = t2;
	}
}
