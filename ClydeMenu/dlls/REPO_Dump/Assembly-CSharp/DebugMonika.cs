using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public class DebugMonika : MonoBehaviour
{
	// Token: 0x06001073 RID: 4211 RVA: 0x0009808B File Offset: 0x0009628B
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>(true);
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x000980AC File Offset: 0x000962AC
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

	// Token: 0x04001C3B RID: 7227
	private HurtCollider hurtCollider;

	// Token: 0x04001C3C RID: 7228
	private float hurtColliderTimer;

	// Token: 0x04001C3D RID: 7229
	private Transform playerTransform;
}
