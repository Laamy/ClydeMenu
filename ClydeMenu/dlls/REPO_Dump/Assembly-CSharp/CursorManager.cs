using System;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class CursorManager : MonoBehaviour
{
	// Token: 0x06000936 RID: 2358 RVA: 0x00058231 File Offset: 0x00056431
	private void Awake()
	{
		if (!CursorManager.instance)
		{
			CursorManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x0005825C File Offset: 0x0005645C
	private void Update()
	{
		if (this.unlockTimer > 0f)
		{
			if (MenuCursor.instance)
			{
				MenuCursor.instance.Show();
			}
			this.unlockTimer -= Time.deltaTime;
			return;
		}
		if (this.unlockTimer != -1234f)
		{
			Cursor.lockState = CursorLockMode.Locked;
			this.unlockTimer = -1234f;
		}
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x000582BD File Offset: 0x000564BD
	public void Unlock(float _time)
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = false;
		this.unlockTimer = _time;
	}

	// Token: 0x040010CD RID: 4301
	public static CursorManager instance;

	// Token: 0x040010CE RID: 4302
	private float unlockTimer;
}
