using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class DebugServerUI : MonoBehaviour
{
	// Token: 0x0600139E RID: 5022 RVA: 0x000AEC58 File Offset: 0x000ACE58
	private void Start()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.Text.text = "Local";
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.Text.text = "Server";
			return;
		}
		this.Text.text = "Client";
	}

	// Token: 0x040021BF RID: 8639
	public TextMeshProUGUI Text;
}
