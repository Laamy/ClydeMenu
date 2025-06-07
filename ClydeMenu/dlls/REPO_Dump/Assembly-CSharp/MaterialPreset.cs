using System;
using UnityEngine;

// Token: 0x020000FF RID: 255
[CreateAssetMenu(fileName = "Material - _____", menuName = "Other/Material Preset", order = 1)]
public class MaterialPreset : ScriptableObject
{
	// Token: 0x060008E8 RID: 2280 RVA: 0x00055EFF File Offset: 0x000540FF
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00055F08 File Offset: 0x00054108
	public void Setup()
	{
		this.RareImpactLightCurrent = (float)Random.Range(this.RareImpactLightMin, this.RareImpactLightMax);
		this.RareImpactMediumCurrent = (float)Random.Range(this.RareImpactMediumMin, this.RareImpactMediumMax);
		this.RareImpactHeavyCurrent = (float)Random.Range(this.RareImpactHeavyMin, this.RareImpactHeavyMax);
		this.RareFootstepLightCurrent = (float)Random.Range(this.RareFootstepLightMin, this.RareFootstepLightMax);
		this.RareFootstepMediumCurrent = (float)Random.Range(this.RareFootstepMediumMin, this.RareFootstepMediumMax);
		this.RareFootstepHeavyCurrent = (float)Random.Range(this.RareFootstepHeavyMin, this.RareFootstepHeavyMax);
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00055FA8 File Offset: 0x000541A8
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.ImpactLight.Type = AudioManager.AudioType.MaterialImpact;
		this.ImpactMedium.Type = AudioManager.AudioType.MaterialImpact;
		this.ImpactHeavy.Type = AudioManager.AudioType.MaterialImpact;
		this.RareImpactLight.Type = AudioManager.AudioType.MaterialImpact;
		this.RareImpactMedium.Type = AudioManager.AudioType.MaterialImpact;
		this.RareImpactHeavy.Type = AudioManager.AudioType.MaterialImpact;
		this.FootstepLight.Type = AudioManager.AudioType.Footstep;
		this.FootstepMedium.Type = AudioManager.AudioType.Footstep;
		this.FootstepHeavy.Type = AudioManager.AudioType.Footstep;
		this.RareFootstepLight.Type = AudioManager.AudioType.Footstep;
		this.RareFootstepMedium.Type = AudioManager.AudioType.Footstep;
		this.RareFootstepHeavy.Type = AudioManager.AudioType.Footstep;
		this.SlideOneShot.Type = AudioManager.AudioType.MaterialImpact;
		this.SlideLoop.Type = AudioManager.AudioType.MaterialImpact;
	}

	// Token: 0x04001035 RID: 4149
	public string Name;

	// Token: 0x04001036 RID: 4150
	public Materials.Type Type;

	// Token: 0x04001037 RID: 4151
	[Space]
	[Header("Impact Sounds")]
	public Sound ImpactLight;

	// Token: 0x04001038 RID: 4152
	public Sound ImpactMedium;

	// Token: 0x04001039 RID: 4153
	public Sound ImpactHeavy;

	// Token: 0x0400103A RID: 4154
	[Space]
	[Header("Rare Impact Sounds")]
	public Sound RareImpactLight;

	// Token: 0x0400103B RID: 4155
	public int RareImpactLightMin;

	// Token: 0x0400103C RID: 4156
	public int RareImpactLightMax;

	// Token: 0x0400103D RID: 4157
	[HideInInspector]
	public float RareImpactLightCurrent;

	// Token: 0x0400103E RID: 4158
	[Space]
	public Sound RareImpactMedium;

	// Token: 0x0400103F RID: 4159
	public int RareImpactMediumMin;

	// Token: 0x04001040 RID: 4160
	public int RareImpactMediumMax;

	// Token: 0x04001041 RID: 4161
	[HideInInspector]
	public float RareImpactMediumCurrent;

	// Token: 0x04001042 RID: 4162
	[Space]
	public Sound RareImpactHeavy;

	// Token: 0x04001043 RID: 4163
	public int RareImpactHeavyMin;

	// Token: 0x04001044 RID: 4164
	public int RareImpactHeavyMax;

	// Token: 0x04001045 RID: 4165
	[HideInInspector]
	public float RareImpactHeavyCurrent;

	// Token: 0x04001046 RID: 4166
	[Space]
	[Header("Footstep Sounds")]
	public Sound FootstepLight;

	// Token: 0x04001047 RID: 4167
	public Sound FootstepMedium;

	// Token: 0x04001048 RID: 4168
	public Sound FootstepHeavy;

	// Token: 0x04001049 RID: 4169
	[Space]
	[Header("Rare Footstep Sounds")]
	public Sound RareFootstepLight;

	// Token: 0x0400104A RID: 4170
	public int RareFootstepLightMin;

	// Token: 0x0400104B RID: 4171
	public int RareFootstepLightMax;

	// Token: 0x0400104C RID: 4172
	[HideInInspector]
	public float RareFootstepLightCurrent;

	// Token: 0x0400104D RID: 4173
	[Space]
	public Sound RareFootstepMedium;

	// Token: 0x0400104E RID: 4174
	public int RareFootstepMediumMin;

	// Token: 0x0400104F RID: 4175
	public int RareFootstepMediumMax;

	// Token: 0x04001050 RID: 4176
	[HideInInspector]
	public float RareFootstepMediumCurrent;

	// Token: 0x04001051 RID: 4177
	[Space]
	public Sound RareFootstepHeavy;

	// Token: 0x04001052 RID: 4178
	public int RareFootstepHeavyMin;

	// Token: 0x04001053 RID: 4179
	public int RareFootstepHeavyMax;

	// Token: 0x04001054 RID: 4180
	[HideInInspector]
	public float RareFootstepHeavyCurrent;

	// Token: 0x04001055 RID: 4181
	[Space]
	[Header("Slide Sounds")]
	public Sound SlideOneShot;

	// Token: 0x04001056 RID: 4182
	public Sound SlideLoop;
}
