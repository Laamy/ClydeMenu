using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000003 RID: 3
public class AmbienceBreakers : MonoBehaviour
{
	// Token: 0x06000007 RID: 7 RVA: 0x0000216A File Offset: 0x0000036A
	private void Awake()
	{
		AmbienceBreakers.instance = this;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000008 RID: 8 RVA: 0x0000217E File Offset: 0x0000037E
	private void Start()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.isLocal = true;
		}
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002195 File Offset: 0x00000395
	public void Setup()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000021AA File Offset: 0x000003AA
	private IEnumerator Logic()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.cooldownTimer = Random.Range(this.cooldownMin, this.cooldownMax);
		if (!this.isLocal)
		{
			yield break;
		}
		for (;;)
		{
			if (this.cooldownTimer > 0f)
			{
				this.cooldownTimer -= this.updateRate;
			}
			else
			{
				this.cooldownTimer = Random.Range(this.cooldownMin, this.cooldownMax);
				Vector2 normalized = Random.insideUnitCircle.normalized;
				float d = Random.Range(this.minDistance, this.maxDistance);
				Vector3 vector = GameDirector.instance.PlayerList[Random.Range(0, GameDirector.instance.PlayerList.Count)].transform.position + new Vector3(normalized.x, 0f, normalized.y) * d;
				int num = Random.Range(0, LevelGenerator.Instance.Level.AmbiencePresets.Count);
				if (this.presetOverride != -1)
				{
					num = this.presetOverride;
				}
				this.presetOverride = -1;
				if (LevelGenerator.Instance.Level.AmbiencePresets[num].breakers.Count > 0)
				{
					int num2 = Random.Range(0, LevelGenerator.Instance.Level.AmbiencePresets[num].breakers.Count);
					if (this.breakerOverride != -1)
					{
						num2 = this.breakerOverride;
					}
					this.breakerOverride = -1;
					if (!GameManager.Multiplayer())
					{
						this.PlaySoundRPC(vector, num, num2);
					}
					else
					{
						this.photonView.RPC("PlaySoundRPC", RpcTarget.All, new object[]
						{
							vector,
							num,
							num2
						});
					}
				}
			}
			yield return new WaitForSeconds(this.updateRate);
		}
		yield break;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x000021BC File Offset: 0x000003BC
	public void LiveTest(LevelAmbience _presetOverride, LevelAmbienceBreaker _breakerOverride)
	{
		foreach (LevelAmbience levelAmbience in LevelGenerator.Instance.Level.AmbiencePresets)
		{
			if (levelAmbience == _presetOverride)
			{
				this.presetOverride = LevelGenerator.Instance.Level.AmbiencePresets.IndexOf(levelAmbience);
				foreach (LevelAmbienceBreaker levelAmbienceBreaker in levelAmbience.breakers)
				{
					if (levelAmbienceBreaker == _breakerOverride)
					{
						this.breakerOverride = levelAmbience.breakers.IndexOf(levelAmbienceBreaker);
					}
				}
			}
		}
		this.cooldownTimer = 0f;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002294 File Offset: 0x00000494
	[PunRPC]
	public void PlaySoundRPC(Vector3 _position, int _preset, int _breaker)
	{
		LevelAmbienceBreaker levelAmbienceBreaker = LevelGenerator.Instance.Level.AmbiencePresets[_preset].breakers[_breaker];
		this.sound.Volume = levelAmbienceBreaker.volume;
		this.sound.Sounds[0] = levelAmbienceBreaker.sound;
		this.sound.Play(_position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04000003 RID: 3
	public static AmbienceBreakers instance;

	// Token: 0x04000004 RID: 4
	private PhotonView photonView;

	// Token: 0x04000005 RID: 5
	private bool isLocal;

	// Token: 0x04000006 RID: 6
	[Space]
	public float minDistance = 8f;

	// Token: 0x04000007 RID: 7
	public float maxDistance = 15f;

	// Token: 0x04000008 RID: 8
	[Space]
	public float cooldownMin = 20f;

	// Token: 0x04000009 RID: 9
	public float cooldownMax = 120f;

	// Token: 0x0400000A RID: 10
	private float cooldownTimer;

	// Token: 0x0400000B RID: 11
	[Space]
	public Sound sound;

	// Token: 0x0400000C RID: 12
	private int presetOverride = -1;

	// Token: 0x0400000D RID: 13
	private int breakerOverride = -1;

	// Token: 0x0400000E RID: 14
	private float updateRate = 0.5f;
}
