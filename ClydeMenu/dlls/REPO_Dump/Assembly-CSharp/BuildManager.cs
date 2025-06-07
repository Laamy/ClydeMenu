using System;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class BuildManager : MonoBehaviour
{
	// Token: 0x06000929 RID: 2345 RVA: 0x00057A2C File Offset: 0x00055C2C
	private void Awake()
	{
		if (!BuildManager.instance)
		{
			BuildManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			Debug.Log("VERSION: " + this.version.title);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x040010B5 RID: 4277
	public static BuildManager instance;

	// Token: 0x040010B6 RID: 4278
	public Version version;
}
