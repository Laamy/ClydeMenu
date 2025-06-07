using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class PlayerListDisplay : MonoBehaviourPunCallbacks
{
	// Token: 0x06000922 RID: 2338 RVA: 0x000579DF File Offset: 0x00055BDF
	private void Start()
	{
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x000579E1 File Offset: 0x00055BE1
	private void Update()
	{
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x000579E3 File Offset: 0x00055BE3
	[PunRPC]
	private void StartLoadingRPC()
	{
	}

	// Token: 0x040010A0 RID: 4256
	public TextMeshProUGUI playerListText;

	// Token: 0x040010A1 RID: 4257
	public TextMeshProUGUI instructionText;

	// Token: 0x040010A2 RID: 4258
	public TextMeshProUGUI roomNameText;

	// Token: 0x040010A3 RID: 4259
	public GameObject loadingUI;

	// Token: 0x040010A4 RID: 4260
	private bool loading;

	// Token: 0x040010A5 RID: 4261
	public GameObject punVoiceClient;

	// Token: 0x040010A6 RID: 4262
	internal int playersCount;

	// Token: 0x040010A7 RID: 4263
	private bool voiceInitialized;

	// Token: 0x040010A8 RID: 4264
	public MenuPageMain menuPageMain;
}
