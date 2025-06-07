using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class DebugJannek : MonoBehaviour
{
	// Token: 0x06001070 RID: 4208 RVA: 0x00097F40 File Offset: 0x00096140
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>(true);
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x00097F60 File Offset: 0x00096160
	private void Update()
	{
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
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
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
				PlayerController.instance.playerAvatarScript.playerHealth.Heal(30, true);
			}
		}
	}

	// Token: 0x04001C38 RID: 7224
	private HurtCollider hurtCollider;

	// Token: 0x04001C39 RID: 7225
	private float hurtColliderTimer;

	// Token: 0x04001C3A RID: 7226
	private Transform playerTransform;
}
