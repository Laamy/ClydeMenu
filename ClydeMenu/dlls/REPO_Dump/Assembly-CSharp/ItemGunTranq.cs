using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class ItemGunTranq : MonoBehaviour
{
	// Token: 0x06000BB3 RID: 2995 RVA: 0x00067E14 File Offset: 0x00066014
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00067E24 File Offset: 0x00066024
	public void MakeHitPlayersHigh()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		PlayerAvatar instance = PlayerAvatar.instance;
		instance.OverridePupilSize(3f, 4, 1f, 1f, 5f, 0.5f, 1.8f);
		this.photonView.RPC("SlowDownVoiceRPC", RpcTarget.Others, new object[]
		{
			instance.photonView.ViewID
		});
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x00067E90 File Offset: 0x00066090
	[PunRPC]
	public void SlowDownVoiceRPC(int _photonID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(_photonID);
		if (!SemiFunc.OwnerOnlyRPC(_info, playerAvatar.photonView))
		{
			return;
		}
		playerAvatar.voiceChat.OverridePitch(0.65f, 1f, 1f, 3f, 0.1f, 20f);
	}

	// Token: 0x04001306 RID: 4870
	private PhotonView photonView;
}
