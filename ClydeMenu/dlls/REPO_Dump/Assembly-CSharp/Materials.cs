using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class Materials : MonoBehaviour
{
	// Token: 0x060008E1 RID: 2273 RVA: 0x000558E2 File Offset: 0x00053AE2
	private void Awake()
	{
		Materials.Instance = this;
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x000558EC File Offset: 0x00053AEC
	public void Impulse(Vector3 origin, Vector3 direction, Materials.SoundType soundType, bool footstep, Materials.MaterialTrigger materialTrigger, Materials.HostType hostType)
	{
		Vector3 material = this.GetMaterial(origin, materialTrigger);
		if (this.LastMaterialList)
		{
			float volumeMultiplier = 1f;
			float falloffMultiplier = 1f;
			float offscreenVolumeMultiplier = 1f;
			float offscreenFalloffMultiplier = 1f;
			if (hostType == Materials.HostType.OtherPlayer)
			{
				volumeMultiplier = 0.5f;
			}
			else if (hostType == Materials.HostType.Enemy)
			{
				volumeMultiplier = 0.5f;
				falloffMultiplier = 0.5f;
				offscreenVolumeMultiplier = 0.25f;
				offscreenFalloffMultiplier = 0.25f;
			}
			switch (soundType)
			{
			case Materials.SoundType.Light:
				if (footstep)
				{
					if (this.LastMaterialList.RareFootstepLightMax > 0)
					{
						this.LastMaterialList.RareFootstepLightCurrent -= 1f;
						if (this.LastMaterialList.RareFootstepLightCurrent <= 0f)
						{
							this.LastMaterialList.RareFootstepLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
							this.LastMaterialList.RareFootstepLightCurrent = (float)Random.Range(this.LastMaterialList.RareFootstepLightMin, this.LastMaterialList.RareFootstepLightMax);
						}
					}
					this.LastMaterialList.FootstepLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					return;
				}
				if (this.LastMaterialList.RareImpactLightMax > 0)
				{
					this.LastMaterialList.RareImpactLightCurrent -= 1f;
					if (this.LastMaterialList.RareImpactLightCurrent <= 0f)
					{
						this.LastMaterialList.RareImpactLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						this.LastMaterialList.RareImpactLightCurrent = (float)Random.Range(this.LastMaterialList.RareImpactLightMin, this.LastMaterialList.RareImpactLightMax);
					}
				}
				this.LastMaterialList.ImpactLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				return;
			case Materials.SoundType.Medium:
				if (footstep)
				{
					if (this.LastMaterialList.RareFootstepMediumMax > 0)
					{
						this.LastMaterialList.RareFootstepMediumCurrent -= 1f;
						if (this.LastMaterialList.RareFootstepMediumCurrent <= 0f)
						{
							this.LastMaterialList.RareFootstepMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
							this.LastMaterialList.RareFootstepMediumCurrent = (float)Random.Range(this.LastMaterialList.RareFootstepMediumMin, this.LastMaterialList.RareFootstepMediumMax);
						}
					}
					this.LastMaterialList.FootstepMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					return;
				}
				if (this.LastMaterialList.RareImpactMediumMax > 0)
				{
					this.LastMaterialList.RareImpactMediumCurrent -= 1f;
					if (this.LastMaterialList.RareImpactMediumCurrent <= 0f)
					{
						this.LastMaterialList.RareImpactMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						this.LastMaterialList.RareImpactMediumCurrent = (float)Random.Range(this.LastMaterialList.RareImpactMediumMin, this.LastMaterialList.RareImpactMediumMax);
					}
				}
				this.LastMaterialList.ImpactMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				return;
			case Materials.SoundType.Heavy:
				if (footstep)
				{
					if (this.LastMaterialList.RareFootstepHeavyMax > 0)
					{
						this.LastMaterialList.RareFootstepHeavyCurrent -= 1f;
						if (this.LastMaterialList.RareFootstepHeavyCurrent <= 0f)
						{
							this.LastMaterialList.RareFootstepHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
							this.LastMaterialList.RareFootstepHeavyCurrent = (float)Random.Range(this.LastMaterialList.RareFootstepHeavyMin, this.LastMaterialList.RareFootstepHeavyMax);
						}
					}
					this.LastMaterialList.FootstepHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					return;
				}
				if (this.LastMaterialList.RareImpactHeavyMax > 0)
				{
					this.LastMaterialList.RareImpactHeavyCurrent -= 1f;
					if (this.LastMaterialList.RareImpactHeavyCurrent <= 0f)
					{
						this.LastMaterialList.RareImpactHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						this.LastMaterialList.RareImpactHeavyCurrent = (float)Random.Range(this.LastMaterialList.RareImpactHeavyMin, this.LastMaterialList.RareImpactHeavyMax);
					}
				}
				this.LastMaterialList.ImpactHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00055CCC File Offset: 0x00053ECC
	public void Slide(Vector3 origin, Materials.MaterialTrigger materialTrigger, float spatialBlend, bool isPlayer)
	{
		float volumeMultiplier = 1f;
		if (!isPlayer)
		{
			volumeMultiplier = 0.5f;
		}
		Vector3 material = this.GetMaterial(origin, materialTrigger);
		if (this.LastMaterialList)
		{
			this.LastMaterialList.SlideOneShot.SpatialBlend = spatialBlend;
			this.LastMaterialList.SlideOneShot.Play(material, volumeMultiplier, 1f, 1f, 1f);
		}
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00055D34 File Offset: 0x00053F34
	public void SlideLoop(Vector3 origin, Materials.MaterialTrigger materialTrigger, float spatialBlend, float pitchMultiplier)
	{
		Vector3 position = origin;
		bool flag = materialTrigger.SlidingLoopObject != null;
		if (!flag || materialTrigger.SlidingLoopObject.getMaterialTimer <= 0f)
		{
			position = this.GetMaterial(origin, materialTrigger);
			if (flag)
			{
				materialTrigger.SlidingLoopObject.getMaterialTimer = 0.25f;
			}
		}
		if (materialTrigger.LastMaterialList != null)
		{
			bool flag2 = false;
			if (!flag)
			{
				flag2 = true;
			}
			else if (materialTrigger.SlidingLoopObject.material != materialTrigger.LastMaterialList)
			{
				materialTrigger.SlidingLoopObject = null;
				flag2 = true;
			}
			if (flag2)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(AudioManager.instance.AudioMaterialSlidingLoop, position, Quaternion.identity, AudioManager.instance.SoundsParent);
				materialTrigger.SlidingLoopObject = gameObject.GetComponent<MaterialSlidingLoop>();
				materialTrigger.SlidingLoopObject.material = materialTrigger.LastMaterialList;
			}
			materialTrigger.SlidingLoopObject.activeTimer = 0.1f;
			materialTrigger.SlidingLoopObject.transform.position = position;
			materialTrigger.SlidingLoopObject.pitchMultiplier = pitchMultiplier;
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00055E2C File Offset: 0x0005402C
	private Vector3 GetMaterial(Vector3 origin, Materials.MaterialTrigger materialTrigger)
	{
		origin = new Vector3(origin.x, origin.y + 0.1f, origin.z);
		Materials.Type _type = materialTrigger.LastMaterialType;
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, Vector3.down, out raycastHit, 1f, this.LayerMask, QueryTriggerInteraction.Collide))
		{
			MaterialSurface component = raycastHit.collider.gameObject.GetComponent<MaterialSurface>();
			if (component)
			{
				_type = component.Type;
				origin = raycastHit.point;
			}
		}
		this.LastMaterialList = this.MaterialList.Find((MaterialPreset x) => x.Type == _type);
		materialTrigger.LastMaterialType = _type;
		materialTrigger.LastMaterialList = this.LastMaterialList;
		return origin;
	}

	// Token: 0x04001030 RID: 4144
	public static Materials Instance;

	// Token: 0x04001031 RID: 4145
	public LayerMask LayerMask;

	// Token: 0x04001032 RID: 4146
	[Space]
	public List<MaterialPreset> MaterialList;

	// Token: 0x04001033 RID: 4147
	private MaterialPreset LastMaterialList;

	// Token: 0x02000369 RID: 873
	public enum Type
	{
		// Token: 0x04002AA9 RID: 10921
		None,
		// Token: 0x04002AAA RID: 10922
		Wood,
		// Token: 0x04002AAB RID: 10923
		Rug,
		// Token: 0x04002AAC RID: 10924
		Tile,
		// Token: 0x04002AAD RID: 10925
		Stone,
		// Token: 0x04002AAE RID: 10926
		Catwalk,
		// Token: 0x04002AAF RID: 10927
		Snow,
		// Token: 0x04002AB0 RID: 10928
		Metal,
		// Token: 0x04002AB1 RID: 10929
		Wetmetal,
		// Token: 0x04002AB2 RID: 10930
		Gravel,
		// Token: 0x04002AB3 RID: 10931
		Grass,
		// Token: 0x04002AB4 RID: 10932
		Water,
		// Token: 0x04002AB5 RID: 10933
		Vent,
		// Token: 0x04002AB6 RID: 10934
		Beam
	}

	// Token: 0x0200036A RID: 874
	public enum SoundType
	{
		// Token: 0x04002AB8 RID: 10936
		Light,
		// Token: 0x04002AB9 RID: 10937
		Medium,
		// Token: 0x04002ABA RID: 10938
		Heavy
	}

	// Token: 0x0200036B RID: 875
	public enum HostType
	{
		// Token: 0x04002ABC RID: 10940
		LocalPlayer,
		// Token: 0x04002ABD RID: 10941
		OtherPlayer,
		// Token: 0x04002ABE RID: 10942
		Enemy
	}

	// Token: 0x0200036C RID: 876
	[Serializable]
	public class MaterialTrigger
	{
		// Token: 0x04002ABF RID: 10943
		internal MaterialPreset LastMaterialList;

		// Token: 0x04002AC0 RID: 10944
		internal Materials.Type LastMaterialType;

		// Token: 0x04002AC1 RID: 10945
		internal MaterialSlidingLoop SlidingLoopObject;
	}
}
