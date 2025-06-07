using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class DebugRuben : MonoBehaviour
{
	// Token: 0x06001079 RID: 4217 RVA: 0x000982B2 File Offset: 0x000964B2
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>(true);
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x000982D4 File Offset: 0x000964D4
	private void Update()
	{
		if (SemiFunc.KeyDownRuben(KeyCode.F6))
		{
			this.SpawnObject(AssetManager.instance.surplusValuableSmall, base.transform.position + base.transform.forward * 2f, "Valuables/");
		}
		if (SemiFunc.KeyDownRuben(KeyCode.F7))
		{
			this.SpawnObject(AssetManager.instance.surplusValuableBig, base.transform.position + base.transform.forward * 2f, "Valuables/");
		}
		if (SemiFunc.KeyDownRuben(KeyCode.F5))
		{
			EnemyDirector.instance.SetInvestigate(base.transform.position, 999f, false);
		}
		if (!LevelGenerator.Instance.Generated && SpectateCamera.instance && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			return;
		}
		if (!PlayerController.instance.playerAvatarScript || PlayerController.instance.playerAvatarScript.deadSet)
		{
			return;
		}
		base.transform.position = Camera.main.transform.position;
		base.transform.rotation = Camera.main.transform.rotation;
		if (Input.GetKeyDown(KeyCode.F2))
		{
			this.hurtCollider.gameObject.SetActive(true);
			this.hurtColliderTimer = 0.2f;
		}
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				this.hurtCollider.gameObject.SetActive(false);
			}
		}
		if (Input.GetKeyDown(KeyCode.F4))
		{
			PlayerController.instance.playerAvatarScript.playerHealth.Heal(75, true);
		}
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x00098499 File Offset: 0x00096699
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

	// Token: 0x04001C3F RID: 7231
	private PhotonView photonView;

	// Token: 0x04001C40 RID: 7232
	private HurtCollider hurtCollider;

	// Token: 0x04001C41 RID: 7233
	private float hurtColliderTimer;

	// Token: 0x04001C42 RID: 7234
	private Transform playerTransform;

	// Token: 0x04001C43 RID: 7235
	public List<GameObject> spawnObjects;
}
