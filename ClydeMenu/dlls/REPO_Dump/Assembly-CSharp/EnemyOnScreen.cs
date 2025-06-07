using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000A2 RID: 162
public class EnemyOnScreen : MonoBehaviour
{
	// Token: 0x06000678 RID: 1656 RVA: 0x0003E744 File Offset: 0x0003C944
	private void Awake()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.MainCamera = Camera.main;
		if (this.points.Length == 0)
		{
			this.points = new Transform[1];
			this.points[0] = this.Enemy.CenterTransform;
		}
		this.LogicActive = true;
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x0003E7A4 File Offset: 0x0003C9A4
	private void OnEnable()
	{
		if (!this.LogicActive)
		{
			this.LogicActive = true;
			base.StartCoroutine(this.Logic());
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0003E7C2 File Offset: 0x0003C9C2
	private void OnDisable()
	{
		this.LogicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0003E7D1 File Offset: 0x0003C9D1
	private IEnumerator Logic()
	{
		while (this.OnScreenPlayer.Count == 0)
		{
			yield return new WaitForSeconds(this.OnScreenTimer);
		}
		for (;;)
		{
			this.CulledLocal = true;
			this.CulledAny = true;
			this.OnScreenLocal = false;
			this.OnScreenAny = false;
			foreach (Transform transform in this.points)
			{
				if (Vector3.Distance(transform.position, CameraUtils.Instance.MainCamera.transform.position) <= this.maxDistance && SemiFunc.OnScreen(transform.position, this.paddingWidth, this.paddingHeight))
				{
					this.CulledLocal = false;
					this.CulledAny = false;
					Vector3 direction = this.MainCamera.transform.position - transform.position;
					float num = Mathf.Min(Vector3.Distance(this.MainCamera.transform.position, transform.position), 12f);
					RaycastHit raycastHit;
					if (!Physics.Raycast(transform.position, direction, out raycastHit, num, this.Enemy.VisionMask) || raycastHit.transform.CompareTag("Player") || raycastHit.transform.GetComponent<PlayerTumble>())
					{
						this.OnScreenLocal = true;
						this.OnScreenAny = true;
					}
				}
				if (this.OnScreenAny && !this.CulledAny)
				{
					break;
				}
			}
			if (GameManager.Multiplayer())
			{
				foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
				{
					if (!playerAvatar.isDisabled && playerAvatar.photonView.IsMine)
					{
						if (this.CulledLocal != this.CulledLocalPrevious || this.OnScreenLocal != this.OnScreenLocalPrevious)
						{
							this.CulledLocalPrevious = this.CulledLocal;
							this.OnScreenLocalPrevious = this.OnScreenLocal;
							this.OnScreenPlayerUpdate(playerAvatar.photonView.ViewID, this.OnScreenLocal, this.CulledLocal);
							break;
						}
						break;
					}
				}
				foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
				{
					if (!playerAvatar2.isDisabled)
					{
						if (this.OnScreenPlayer[playerAvatar2.photonView.ViewID])
						{
							this.OnScreenAny = true;
						}
						if (!this.CulledPlayer[playerAvatar2.photonView.ViewID])
						{
							this.CulledAny = false;
						}
					}
				}
			}
			yield return new WaitForSeconds(this.OnScreenTimer);
		}
		yield break;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x0003E7E0 File Offset: 0x0003C9E0
	public bool GetOnScreen(PlayerAvatar _playerAvatar)
	{
		if (!GameManager.Multiplayer())
		{
			return this.OnScreenLocal;
		}
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar == _playerAvatar && this.OnScreenPlayer[playerAvatar.photonView.ViewID])
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x0003E868 File Offset: 0x0003CA68
	private void OnScreenPlayerUpdate(int playerID, bool onScreen, bool culled)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.OnScreenPlayerUpdateRPC(playerID, onScreen, culled);
			return;
		}
		this.Enemy.PhotonView.RPC("OnScreenPlayerUpdateRPC", RpcTarget.All, new object[]
		{
			playerID,
			onScreen,
			culled
		});
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x0003E8C2 File Offset: 0x0003CAC2
	[PunRPC]
	private void OnScreenPlayerUpdateRPC(int playerID, bool onScreen, bool culled)
	{
		this.CulledPlayer[playerID] = culled;
		this.OnScreenPlayer[playerID] = onScreen;
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x0003E8DE File Offset: 0x0003CADE
	public void PlayerAdded(int photonID)
	{
		this.OnScreenPlayer.TryAdd(photonID, false);
		this.CulledPlayer.TryAdd(photonID, false);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x0003E8FC File Offset: 0x0003CAFC
	public void PlayerRemoved(int photonID)
	{
		this.OnScreenPlayer.Remove(photonID);
		this.CulledPlayer.Remove(photonID);
	}

	// Token: 0x04000AA1 RID: 2721
	private Enemy Enemy;

	// Token: 0x04000AA2 RID: 2722
	private Camera MainCamera;

	// Token: 0x04000AA3 RID: 2723
	public Transform[] points;

	// Token: 0x04000AA4 RID: 2724
	[Space]
	public float maxDistance = 20f;

	// Token: 0x04000AA5 RID: 2725
	[Space]
	public float paddingWidth = 0.1f;

	// Token: 0x04000AA6 RID: 2726
	public float paddingHeight = 0.1f;

	// Token: 0x04000AA7 RID: 2727
	private bool LogicActive;

	// Token: 0x04000AA8 RID: 2728
	private float OnScreenTimer = 0.25f;

	// Token: 0x04000AA9 RID: 2729
	internal bool OnScreenLocal;

	// Token: 0x04000AAA RID: 2730
	private bool OnScreenLocalPrevious;

	// Token: 0x04000AAB RID: 2731
	internal bool CulledLocal;

	// Token: 0x04000AAC RID: 2732
	private bool CulledLocalPrevious;

	// Token: 0x04000AAD RID: 2733
	internal bool OnScreenAny;

	// Token: 0x04000AAE RID: 2734
	internal bool CulledAny;

	// Token: 0x04000AAF RID: 2735
	internal Dictionary<int, bool> OnScreenPlayer = new Dictionary<int, bool>();

	// Token: 0x04000AB0 RID: 2736
	internal Dictionary<int, bool> CulledPlayer = new Dictionary<int, bool>();
}
