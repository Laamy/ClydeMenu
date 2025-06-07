using System;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class ModuleBlockObject : MonoBehaviour
{
	// Token: 0x06000819 RID: 2073 RVA: 0x0004F788 File Offset: 0x0004D988
	private void Start()
	{
		if (!base.transform.parent)
		{
			base.transform.parent = LevelGenerator.Instance.LevelParent.transform;
		}
	}
}
