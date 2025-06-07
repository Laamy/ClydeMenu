using System;
using UnityEngine;

// Token: 0x0200022F RID: 559
public class PlayerCrownSet : MonoBehaviour
{
	// Token: 0x0600127B RID: 4731 RVA: 0x000A6493 File Offset: 0x000A4693
	private void Start()
	{
		if (!PlayerCrownSet.instance)
		{
			PlayerCrownSet.instance = this;
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001F0A RID: 7946
	public static PlayerCrownSet instance;

	// Token: 0x04001F0B RID: 7947
	internal bool crownOwnerFetched;

	// Token: 0x04001F0C RID: 7948
	internal string crownOwnerSteamID;
}
