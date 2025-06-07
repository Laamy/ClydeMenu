using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class RoachOrbit : MonoBehaviour
{
	// Token: 0x06000703 RID: 1795 RVA: 0x0004286C File Offset: 0x00040A6C
	private void Start()
	{
		this.startPosition = base.transform.position;
		this.noiseOffsetX = base.transform.position.x;
		this.noiseOffsetZ = base.transform.position.z;
		this.noiseOffsetX2 = base.transform.position.x * 1.5f;
		this.noiseOffsetZ2 = base.transform.position.z * 1.5f;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x000428FC File Offset: 0x00040AFC
	[PunRPC]
	private void SquashRPC()
	{
		this.squashSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Object.Instantiate<GameObject>(this.roachSmashPrefab, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0004295C File Offset: 0x00040B5C
	public void Squash()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.squashSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Object.Instantiate<GameObject>(this.roachSmashPrefab, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
			return;
		}
		this.photonView.RPC("SquashRPC", RpcTarget.AllBuffered, Array.Empty<object>());
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x000429E0 File Offset: 0x00040BE0
	private void Update()
	{
		this.roachLoopSound.PlayLoop(true, 1f, 2f, 1f);
		float num;
		if (GameManager.instance.gameMode == 0)
		{
			num = Time.time;
		}
		else
		{
			num = NetworkManager.instance.gameTime;
		}
		float num2 = num * this.noiseSpeed;
		float num3 = Mathf.PerlinNoise(this.noiseOffsetX + num2 * this.noiseScale, 0f) * 2f - 1f;
		float num4 = Mathf.PerlinNoise(0f, this.noiseOffsetZ + num2 * this.noiseScale) * 2f - 1f;
		num2 = num * this.noiseSpeed2;
		float num5 = Mathf.PerlinNoise(this.noiseOffsetX2 + num2 * this.noiseScale2, 0f) * 2f - 1f;
		float num6 = Mathf.PerlinNoise(0f, this.noiseOffsetZ2 + num2 * this.noiseScale2) * 2f - 1f;
		float x = (num3 + num5) / 2f;
		float z = (num4 + num6) / 2f;
		Vector3 vector = this.startPosition + new Vector3(x, 0f, z) * this.radius;
		Vector3 vector2 = vector - base.transform.position;
		base.transform.position = vector;
		if (vector2 != Vector3.zero)
		{
			Quaternion quaternion = Quaternion.LookRotation(vector2);
			Quaternion rhs = Quaternion.Euler(0f, -90f, 0f);
			quaternion *= rhs;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
		}
	}

	// Token: 0x04000BE4 RID: 3044
	[Header("Roach Smash")]
	public GameObject roachSmashPrefab;

	// Token: 0x04000BE5 RID: 3045
	public float radius = 5f;

	// Token: 0x04000BE6 RID: 3046
	public float rotationSpeed = 1f;

	// Token: 0x04000BE7 RID: 3047
	public float noiseScale = 1f;

	// Token: 0x04000BE8 RID: 3048
	public float noiseSpeed = 0.5f;

	// Token: 0x04000BE9 RID: 3049
	public float noiseScale2 = 0.5f;

	// Token: 0x04000BEA RID: 3050
	public float noiseSpeed2 = 1f;

	// Token: 0x04000BEB RID: 3051
	private Vector3 startPosition;

	// Token: 0x04000BEC RID: 3052
	private float noiseOffsetX;

	// Token: 0x04000BED RID: 3053
	private float noiseOffsetZ;

	// Token: 0x04000BEE RID: 3054
	private float noiseOffsetX2;

	// Token: 0x04000BEF RID: 3055
	private float noiseOffsetZ2;

	// Token: 0x04000BF0 RID: 3056
	private PhotonView photonView;

	// Token: 0x04000BF1 RID: 3057
	public Sound squashSound;

	// Token: 0x04000BF2 RID: 3058
	public Sound roachLoopSound;
}
