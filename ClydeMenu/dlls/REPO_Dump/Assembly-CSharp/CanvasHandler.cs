using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class CanvasHandler : MonoBehaviour
{
	// Token: 0x060006DF RID: 1759 RVA: 0x00041C20 File Offset: 0x0003FE20
	private void Start()
	{
		this.dirt1Renderer = this.Dirt1.GetComponent<MeshRenderer>();
		this.dirt2Renderer = this.Dirt2.GetComponent<MeshRenderer>();
		this.dirtHangRenderer = this.DirtHang.GetComponent<MeshRenderer>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00041C6C File Offset: 0x0003FE6C
	[PunRPC]
	private void StartCleaningRPC()
	{
		this.multiplayerCleaning = true;
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00041C75 File Offset: 0x0003FE75
	[PunRPC]
	private void StopCleaningRPC()
	{
		this.multiplayerCleaning = false;
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00041C80 File Offset: 0x0003FE80
	[PunRPC]
	private void CleaningDoneRPC()
	{
		this.currentState = CanvasHandler.State.Clean;
		this.cleanEffect.Clean();
		this.InteractableLight.StartFading();
		this.InteractionArea.SetActive(false);
		this.SetMaterialAlpha(this.dirt1Renderer, 0f);
		this.SetMaterialAlpha(this.dirt2Renderer, 0f);
		this.SetMaterialAlpha(this.dirtHangRenderer, 0f);
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00041CEC File Offset: 0x0003FEEC
	private void Update()
	{
		if (this.isCleaningTimer > 0f)
		{
			this.isCleaning = true;
			this.isCleaningTimer -= 1f * Time.deltaTime;
		}
		else
		{
			this.isCleaning = false;
		}
		if (this.currentState == CanvasHandler.State.Clean)
		{
			this.isCleaning = false;
		}
		if ((double)this.fadeMultiplier < 0.5 && this.fadeMultiplier != 0f)
		{
			this.isCleaning = true;
		}
		if (this.isCleaning)
		{
			this.cleanStateTimer = 0f;
			if (this.currentState == CanvasHandler.State.Dirty || this.currentState == CanvasHandler.State.Cleaning)
			{
				if (this.currentState != CanvasHandler.State.Cleaning)
				{
					this.StartWiggle();
					this.currentState = CanvasHandler.State.Cleaning;
				}
			}
			else
			{
				this.isCleaning = false;
			}
		}
		if (this.currentState == CanvasHandler.State.Cleaning)
		{
			if (!this.DebugNoClean)
			{
				if ((double)this.fadeMultiplier > 0.5)
				{
					this.fadeMultiplier -= this.cleaningSpeed * Time.deltaTime;
				}
				else
				{
					this.fadeMultiplier -= this.cleaningSpeed * 2f * Time.deltaTime;
				}
			}
			this.SetMaterialAlpha(this.dirt1Renderer, this.fadeMultiplier);
			this.SetMaterialAlpha(this.dirt2Renderer, this.fadeMultiplier);
			this.SetMaterialAlpha(this.dirtHangRenderer, this.fadeMultiplier);
			this.cleanStateTimer += 1f * Time.deltaTime;
			if ((double)this.cleanStateTimer > 0.2)
			{
				this.PaintingSwingEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.currentState = CanvasHandler.State.Dirty;
			}
			if (this.fadeMultiplier < 0f)
			{
				this.PaintingSwingEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.currentState = CanvasHandler.State.Clean;
				this.cleanEffect.Clean();
				this.InteractableLight.StartFading();
				this.fadeMultiplier = 0f;
				this.InteractionArea.SetActive(false);
			}
		}
		if (this.currentState != CanvasHandler.State.Cleaning)
		{
			this.StopWiggle();
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
			if (this.currentState == CanvasHandler.State.Clean && this.currentState != this.previousState)
			{
				this.photonView.RPC("CleaningDoneRPC", RpcTarget.AllBuffered, Array.Empty<object>());
				this.previousState = this.currentState;
			}
			this.cleanInputPrevious = this.cleanInput;
			this.cleanInput = this.multiplayerCleaning;
		}
		this.PaintingSwingLoop.PlayLoop(this.isCleaning, 1f, 2f, 1f);
		this.isCleaning = false;
		if (this.cleanInput)
		{
			this.isCleaningTimer = 0.1f;
			this.cleanInput = false;
		}
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00042010 File Offset: 0x00040210
	private void SetMaterialAlpha(MeshRenderer renderer, float alpha)
	{
		Color color = renderer.material.color;
		color.a = Mathf.Clamp(alpha, 0f, 1f);
		renderer.material.color = color;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x0004204C File Offset: 0x0004024C
	private void StartWiggle()
	{
		this.isWiggling = true;
		this.DustParticles.Play();
		this.currentWiggleAmount = this.wiggleAmount;
		base.StopAllCoroutines();
		base.StartCoroutine(this.WiggleCoroutine());
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x0004207F File Offset: 0x0004027F
	private void StopWiggle()
	{
		this.DustParticles.Stop();
		this.isWiggling = false;
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00042093 File Offset: 0x00040293
	private IEnumerator WiggleCoroutine()
	{
		float time = 0f;
		float currentZRotation = this.Painting.transform.localRotation.eulerAngles.z;
		if (currentZRotation > 180f)
		{
			currentZRotation -= 360f;
		}
		float phaseOffset = Mathf.Asin(currentZRotation / this.wiggleAmount) - this.wiggleSpeed * time;
		float lerpFactor = 0.15f;
		while (this.isWiggling || Mathf.Abs(this.currentWiggleAmount) > 0.1f)
		{
			float b = Mathf.Sin(time * this.wiggleSpeed + phaseOffset) * this.currentWiggleAmount;
			float num = Mathf.Lerp(currentZRotation, b, lerpFactor);
			this.Painting.transform.localRotation = Quaternion.Euler(0f, 0f, num);
			currentZRotation = num;
			if (!this.isWiggling)
			{
				this.currentWiggleAmount *= this.dampening;
			}
			time += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000B9E RID: 2974
	public bool DebugNoClean;

	// Token: 0x04000B9F RID: 2975
	private PhotonView photonView;

	// Token: 0x04000BA0 RID: 2976
	[Header("Connected Objects")]
	public GameObject Dirt1;

	// Token: 0x04000BA1 RID: 2977
	public GameObject Dirt2;

	// Token: 0x04000BA2 RID: 2978
	public GameObject DirtHang;

	// Token: 0x04000BA3 RID: 2979
	public GameObject Painting;

	// Token: 0x04000BA4 RID: 2980
	public ParticleSystem DustParticles;

	// Token: 0x04000BA5 RID: 2981
	public CleanEffect cleanEffect;

	// Token: 0x04000BA6 RID: 2982
	public GameObject InteractionArea;

	// Token: 0x04000BA7 RID: 2983
	public LightInteractableFadeRemove InteractableLight;

	// Token: 0x04000BA8 RID: 2984
	[Space]
	[Header("Sounds")]
	public Sound PaintingSwingLoop;

	// Token: 0x04000BA9 RID: 2985
	public Sound PaintingSwingEnd;

	// Token: 0x04000BAA RID: 2986
	[Space]
	[Header("Painting Swing Settings")]
	public float wiggleSpeed = 20f;

	// Token: 0x04000BAB RID: 2987
	public float wiggleAmount = 5f;

	// Token: 0x04000BAC RID: 2988
	public float dampening = 0.95f;

	// Token: 0x04000BAD RID: 2989
	private bool isWiggling;

	// Token: 0x04000BAE RID: 2990
	private float currentWiggleAmount;

	// Token: 0x04000BAF RID: 2991
	[Space]
	[Header("Cleaning Settings")]
	private float cleanStateTimer;

	// Token: 0x04000BB0 RID: 2992
	public bool isCleaning;

	// Token: 0x04000BB1 RID: 2993
	public bool isCleaningPrevious;

	// Token: 0x04000BB2 RID: 2994
	[HideInInspector]
	public float isCleaningTimer;

	// Token: 0x04000BB3 RID: 2995
	private bool cleanInputPrevious;

	// Token: 0x04000BB4 RID: 2996
	public bool cleanInput;

	// Token: 0x04000BB5 RID: 2997
	[HideInInspector]
	public bool CleanDone;

	// Token: 0x04000BB6 RID: 2998
	private bool multiplayerCleaning;

	// Token: 0x04000BB7 RID: 2999
	public CanvasHandler.State currentState;

	// Token: 0x04000BB8 RID: 3000
	public CanvasHandler.State previousState;

	// Token: 0x04000BB9 RID: 3001
	[HideInInspector]
	public float fadeMultiplier = 1f;

	// Token: 0x04000BBA RID: 3002
	public float cleaningSpeed = 0.1f;

	// Token: 0x04000BBB RID: 3003
	private MeshRenderer dirt1Renderer;

	// Token: 0x04000BBC RID: 3004
	private MeshRenderer dirt2Renderer;

	// Token: 0x04000BBD RID: 3005
	private MeshRenderer dirtHangRenderer;

	// Token: 0x02000331 RID: 817
	public enum State
	{
		// Token: 0x04002993 RID: 10643
		Dirty,
		// Token: 0x04002994 RID: 10644
		Cleaning,
		// Token: 0x04002995 RID: 10645
		Clean
	}
}
