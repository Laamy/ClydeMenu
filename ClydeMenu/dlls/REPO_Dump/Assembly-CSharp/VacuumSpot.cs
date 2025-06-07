using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class VacuumSpot : MonoBehaviour
{
	// Token: 0x06000759 RID: 1881 RVA: 0x000463DC File Offset: 0x000445DC
	private void Start()
	{
		this.LightIntensity = this.Light.intensity;
		this.photonView = base.GetComponent<PhotonView>();
		this.PileRendererAlpha = this.PileRenderer.material.color.a;
		this.DecalRendererAlpha = this.DecalRenderer.material.color.a;
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0004643C File Offset: 0x0004463C
	[PunRPC]
	private void StartCleaningRPC()
	{
		this.multiplayerCleaning = true;
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00046445 File Offset: 0x00044645
	[PunRPC]
	private void StopCleaningRPC()
	{
		this.multiplayerCleaning = false;
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00046450 File Offset: 0x00044650
	private void Update()
	{
		if (this.DecreaseTimer > 0f)
		{
			this.Amount -= this.DecreaseSpeed * Time.deltaTime;
			if (this.Amount > 0.2f)
			{
				this.DecreaseTimer -= 1f * Time.deltaTime;
			}
			float num = Mathf.Lerp(0f, 1f, this.AlphaCurve.Evaluate(this.Amount));
			Color color = this.PileRenderer.material.color;
			color.a = this.PileRendererAlpha * num;
			this.PileRenderer.material.color = color;
			Color color2 = this.DecalRenderer.material.color;
			color2.a = this.DecalRendererAlpha * num;
			this.DecalRenderer.material.color = color2;
			float num2 = Mathf.Lerp(0f, 1f, this.ScaleCurve.Evaluate(this.Amount));
			this.PileMesh.localScale = new Vector3(1f - (1f - num2) * 0.4f, 0.5f + num2 * 0.5f, 1f - (1f - num2) * 0.4f);
			this.DecalMesh.localScale = new Vector3(1f - (1f - num2) * 0.2f, 1f, 1f - (1f - num2) * 0.2f);
			this.Light.intensity = this.LightIntensity * num2;
			if (this.Amount <= 0f)
			{
				this.CleanEffect.SetActive(true);
				this.CleanEffect.GetComponent<CleanEffect>().Clean();
				this.CleanEffect.transform.parent = null;
				if (GameManager.instance.gameMode == 1)
				{
					if (PhotonNetwork.IsMasterClient && !this.syncDestroy)
					{
						PhotonNetwork.Destroy(base.gameObject);
						this.syncDestroy = true;
					}
				}
				else
				{
					Object.Destroy(base.gameObject);
				}
			}
		}
		if (GameManager.instance.gameMode == 1)
		{
			if (this.cleanInput && this.cleanInput != this.cleanInputPrevious)
			{
				this.photonView.RPC("StartCleaningRPC", RpcTarget.All, Array.Empty<object>());
			}
			if (!this.cleanInput && this.cleanInput != this.cleanInputPrevious)
			{
				this.photonView.RPC("StopCleaningRPC", RpcTarget.All, Array.Empty<object>());
			}
			this.cleanInputPrevious = this.cleanInput;
			this.cleanInput = this.multiplayerCleaning;
		}
		if (this.cleanInput)
		{
			this.DecreaseTimer = 0.1f;
			this.cleanInput = false;
		}
	}

	// Token: 0x04000CF3 RID: 3315
	public GameObject VacuumSpotVisual;

	// Token: 0x04000CF4 RID: 3316
	[HideInInspector]
	public float Amount = 1f;

	// Token: 0x04000CF5 RID: 3317
	public float DecreaseSpeed;

	// Token: 0x04000CF6 RID: 3318
	[HideInInspector]
	public bool CleanDone;

	// Token: 0x04000CF7 RID: 3319
	[Space]
	public Transform PileMesh;

	// Token: 0x04000CF8 RID: 3320
	public MeshRenderer PileRenderer;

	// Token: 0x04000CF9 RID: 3321
	private float PileRendererAlpha;

	// Token: 0x04000CFA RID: 3322
	[Space]
	public Transform DecalMesh;

	// Token: 0x04000CFB RID: 3323
	public MeshRenderer DecalRenderer;

	// Token: 0x04000CFC RID: 3324
	private float DecalRendererAlpha;

	// Token: 0x04000CFD RID: 3325
	[Space]
	public AnimationCurve ScaleCurve;

	// Token: 0x04000CFE RID: 3326
	public AnimationCurve AlphaCurve;

	// Token: 0x04000CFF RID: 3327
	public Light Light;

	// Token: 0x04000D00 RID: 3328
	private float LightIntensity;

	// Token: 0x04000D01 RID: 3329
	[HideInInspector]
	public bool Decreasing;

	// Token: 0x04000D02 RID: 3330
	[HideInInspector]
	public float DecreaseTimer;

	// Token: 0x04000D03 RID: 3331
	public GameObject CleanEffect;

	// Token: 0x04000D04 RID: 3332
	private bool multiplayerCleaning;

	// Token: 0x04000D05 RID: 3333
	private PhotonView photonView;

	// Token: 0x04000D06 RID: 3334
	private bool cleanInputPrevious;

	// Token: 0x04000D07 RID: 3335
	public bool cleanInput;

	// Token: 0x04000D08 RID: 3336
	private bool syncDestroy;
}
