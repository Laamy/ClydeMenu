using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001E3 RID: 483
public class DebugRobin : MonoBehaviour
{
	// Token: 0x06001076 RID: 4214 RVA: 0x000981D8 File Offset: 0x000963D8
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F6))
		{
			this.SpawnObject(AssetManager.instance.surplusValuableMedium, base.transform.position + base.transform.forward * 2f, "Valuables/");
		}
		if (Input.GetKeyDown(KeyCode.F4))
		{
			PlayerController.instance.playerAvatarScript.playerHealth.Hurt(10, true, -1);
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			PlayerController.instance.playerAvatarScript.playerHealth.Heal(10, true);
		}
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x00098272 File Offset: 0x00096472
	private void SpawnObject(GameObject _object, Vector3 _position, string _path)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			Object.Instantiate<GameObject>(_object, _position, Quaternion.identity);
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			PhotonNetwork.InstantiateRoomObject(_path + _object.name, _position, Quaternion.identity, 0, null);
		}
	}

	// Token: 0x04001C3E RID: 7230
	private Transform playerTransform;
}
