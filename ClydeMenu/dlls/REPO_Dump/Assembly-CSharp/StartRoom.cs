using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class StartRoom : MonoBehaviour
{
	// Token: 0x06000822 RID: 2082 RVA: 0x0004F9CD File Offset: 0x0004DBCD
	private void Start()
	{
		base.transform.parent = LevelGenerator.Instance.LevelParent.transform;
	}
}
