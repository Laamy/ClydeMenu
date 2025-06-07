using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class ItemHealthPack : MonoBehaviour
{
	// Token: 0x06000BB7 RID: 2999 RVA: 0x00067EE4 File Offset: 0x000660E4
	private void Start()
	{
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.material = this.mesh.material;
		this.materialEmissionOriginal = this.material.GetColor(this.materialPropertyEmission);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x00067F58 File Offset: 0x00066158
	private void Update()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		this.LightLogic();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemToggle.toggleState && !this.used)
		{
			PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID);
			if (playerAvatar)
			{
				if (playerAvatar.playerHealth.health >= playerAvatar.playerHealth.maxHealth)
				{
					if (SemiFunc.IsMultiplayer())
					{
						this.photonView.RPC("RejectRPC", RpcTarget.All, Array.Empty<object>());
					}
					else
					{
						this.RejectRPC(default(PhotonMessageInfo));
					}
					this.itemToggle.ToggleItem(false, -1);
					this.physGrabObject.rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
					this.physGrabObject.rb.AddTorque(-this.physGrabObject.transform.right * 0.05f, ForceMode.Impulse);
					return;
				}
				playerAvatar.playerHealth.HealOther(this.healAmount, true);
				StatsManager.instance.ItemRemove(this.itemAttributes.instanceName);
				this.physGrabObject.impactDetector.indestructibleBreakEffects = true;
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("UsedRPC", RpcTarget.All, Array.Empty<object>());
					return;
				}
				this.UsedRPC(default(PhotonMessageInfo));
			}
		}
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x000680C0 File Offset: 0x000662C0
	private void LightLogic()
	{
		if (this.used && this.lightIntensityLerp < 1f)
		{
			this.lightIntensityLerp += 1f * Time.deltaTime;
			this.propLight.lightComponent.intensity = this.lightIntensityCurve.Evaluate(this.lightIntensityLerp);
			this.propLight.originalIntensity = this.propLight.lightComponent.intensity;
			this.material.SetColor(this.materialPropertyEmission, Color.Lerp(Color.black, this.materialEmissionOriginal, this.lightIntensityCurve.Evaluate(this.lightIntensityLerp)));
		}
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x00068170 File Offset: 0x00066370
	[PunRPC]
	private void UsedRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, base.transform.position, 0.2f);
		this.itemToggle.ToggleDisable(true);
		this.itemAttributes.DisableUI(true);
		Object.Destroy(this.itemEquippable);
		ParticleSystem[] array = this.particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		this.soundUse.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.used = true;
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x00068228 File Offset: 0x00066428
	[PunRPC]
	private void RejectRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID);
		if (playerAvatar.isLocal)
		{
			playerAvatar.physGrabber.ReleaseObjectRPC(false, 1f);
		}
		ParticleSystem[] array = this.rejectParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, base.transform.position, 0.2f);
		this.soundReject.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x000682E0 File Offset: 0x000664E0
	public void OnDestroy()
	{
		foreach (ParticleSystem particleSystem in this.particles)
		{
			if (particleSystem && particleSystem.isPlaying)
			{
				particleSystem.transform.SetParent(null);
				particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
			}
		}
		foreach (ParticleSystem particleSystem2 in this.rejectParticles)
		{
			if (particleSystem2 && particleSystem2.isPlaying)
			{
				particleSystem2.transform.SetParent(null);
				particleSystem2.main.stopAction = ParticleSystemStopAction.Destroy;
			}
		}
	}

	// Token: 0x04001307 RID: 4871
	public int healAmount;

	// Token: 0x04001308 RID: 4872
	private ItemToggle itemToggle;

	// Token: 0x04001309 RID: 4873
	private ItemEquippable itemEquippable;

	// Token: 0x0400130A RID: 4874
	private ItemAttributes itemAttributes;

	// Token: 0x0400130B RID: 4875
	private PhotonView photonView;

	// Token: 0x0400130C RID: 4876
	private PhysGrabObject physGrabObject;

	// Token: 0x0400130D RID: 4877
	[Space]
	public ParticleSystem[] particles;

	// Token: 0x0400130E RID: 4878
	public ParticleSystem[] rejectParticles;

	// Token: 0x0400130F RID: 4879
	[Space]
	public PropLight propLight;

	// Token: 0x04001310 RID: 4880
	public AnimationCurve lightIntensityCurve;

	// Token: 0x04001311 RID: 4881
	private float lightIntensityLerp;

	// Token: 0x04001312 RID: 4882
	public MeshRenderer mesh;

	// Token: 0x04001313 RID: 4883
	private Material material;

	// Token: 0x04001314 RID: 4884
	private Color materialEmissionOriginal;

	// Token: 0x04001315 RID: 4885
	private int materialPropertyEmission = Shader.PropertyToID("_EmissionColor");

	// Token: 0x04001316 RID: 4886
	[Space]
	public Sound soundUse;

	// Token: 0x04001317 RID: 4887
	public Sound soundReject;

	// Token: 0x04001318 RID: 4888
	private bool used;
}
