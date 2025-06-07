using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class WindowManager : MonoBehaviour
{
	// Token: 0x06000AB0 RID: 2736
	[DllImport("user32.dll")]
	public static extern bool SetWindowText(IntPtr hwnd, string lpString);

	// Token: 0x06000AB1 RID: 2737
	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0005EAE0 File Offset: 0x0005CCE0
	private void Awake()
	{
		if (!WindowManager.instance)
		{
			WindowManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			WindowManager.SetWindowText(WindowManager.FindWindow(null, "Repo"), "R.E.P.O.");
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0400114D RID: 4429
	public static WindowManager instance;
}
