using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class ModuleConnectObject : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x0600081B RID: 2075 RVA: 0x0004F7BE File Offset: 0x0004D9BE
	private void Start()
	{
		base.StartCoroutine(this.ConnectingCheck());
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0004F7CD File Offset: 0x0004D9CD
	private IEnumerator ConnectingCheck()
	{
		while (!this.MasterSetup)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!base.transform.parent)
		{
			base.transform.parent = LevelGenerator.Instance.LevelParent.transform;
		}
		yield break;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0004F7DC File Offset: 0x0004D9DC
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.ModuleConnecting);
			stream.SendNext(this.MasterSetup);
			return;
		}
		this.ModuleConnecting = (bool)stream.ReceiveNext();
		this.MasterSetup = (bool)stream.ReceiveNext();
	}

	// Token: 0x04000EFF RID: 3839
	public bool ModuleConnecting;

	// Token: 0x04000F00 RID: 3840
	public bool MasterSetup;
}
