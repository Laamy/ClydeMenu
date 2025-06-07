using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class DebugAxel : MonoBehaviour
{
	// Token: 0x0600105F RID: 4191 RVA: 0x00097645 File Offset: 0x00095845
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>(true);
		this.hurtCollider.gameObject.SetActive(false);
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00097668 File Offset: 0x00095868
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F7))
		{
			this.SpawnObject(this.spawnObject, base.transform.position + base.transform.forward * 2f, this.spawnObjectPath);
		}
		if (Input.GetKeyDown(KeyCode.F6))
		{
			Application.targetFrameRate = 20;
			EnemyDirector.instance.SetInvestigate(base.transform.position, 999f, false);
			foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
			{
				playerAvatar.playerDeathHead.inExtractionPoint = true;
				playerAvatar.playerDeathHead.Revive();
			}
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
			PlayerController.instance.playerAvatarScript.playerHealth.Hurt(10, true, -1);
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			PlayerController.instance.playerAvatarScript.playerHealth.Heal(10, true);
		}
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x00097864 File Offset: 0x00095A64
	private void SpawnObject(GameObject _object, Vector3 _position, string _path)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			Object.Instantiate<GameObject>(_object, _position, Quaternion.identity);
			return;
		}
		PhotonNetwork.Instantiate(_path + _object.name, _position, Quaternion.identity, 0, null);
	}

	// Token: 0x04001BF8 RID: 7160
	private HurtCollider hurtCollider;

	// Token: 0x04001BF9 RID: 7161
	private float hurtColliderTimer;

	// Token: 0x04001BFA RID: 7162
	private Transform playerTransform;

	// Token: 0x04001BFB RID: 7163
	public GameObject spawnObject;

	// Token: 0x04001BFC RID: 7164
	public string spawnObjectPath = "Valuables/";

	// Token: 0x04001BFD RID: 7165
	public Sound sound;
}
