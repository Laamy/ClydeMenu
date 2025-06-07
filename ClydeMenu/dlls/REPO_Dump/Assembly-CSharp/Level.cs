using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E0 RID: 224
[CreateAssetMenu(fileName = "Level - _____", menuName = "Level/Level Preset", order = 0)]
public class Level : ScriptableObject
{
	// Token: 0x06000810 RID: 2064 RVA: 0x0004F2ED File Offset: 0x0004D4ED
	public void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		if (Application.isPlaying && LevelGenerator.Instance != null && LevelGenerator.Instance.Generated)
		{
			EnvironmentDirector.Instance.Setup();
			PostProcessing.Instance.Setup();
		}
	}

	// Token: 0x04000EB5 RID: 3765
	public string ResourcePath = "";

	// Token: 0x04000EB6 RID: 3766
	[Space]
	public string NarrativeName = "";

	// Token: 0x04000EB7 RID: 3767
	[Space]
	public int ModuleAmount = 6;

	// Token: 0x04000EB8 RID: 3768
	public int PassageMaxAmount = 2;

	// Token: 0x04000EB9 RID: 3769
	[Space]
	public bool HasEnemies = true;

	// Token: 0x04000EBA RID: 3770
	[Space]
	public GameObject ConnectObject;

	// Token: 0x04000EBB RID: 3771
	public GameObject BlockObject;

	// Token: 0x04000EBC RID: 3772
	public List<GameObject> StartRooms;

	// Token: 0x04000EBD RID: 3773
	[Space]
	public Sprite LoadingGraphic01;

	// Token: 0x04000EBE RID: 3774
	public Sprite LoadingGraphic02;

	// Token: 0x04000EBF RID: 3775
	public Sprite LoadingGraphic03;

	// Token: 0x04000EC0 RID: 3776
	[Header("Difficulty 1")]
	public List<GameObject> ModulesNormal1;

	// Token: 0x04000EC1 RID: 3777
	public List<GameObject> ModulesPassage1;

	// Token: 0x04000EC2 RID: 3778
	public List<GameObject> ModulesDeadEnd1;

	// Token: 0x04000EC3 RID: 3779
	public List<GameObject> ModulesExtraction1;

	// Token: 0x04000EC4 RID: 3780
	[Space]
	[Header("Difficulty 2")]
	public List<GameObject> ModulesNormal2;

	// Token: 0x04000EC5 RID: 3781
	public List<GameObject> ModulesPassage2;

	// Token: 0x04000EC6 RID: 3782
	public List<GameObject> ModulesDeadEnd2;

	// Token: 0x04000EC7 RID: 3783
	public List<GameObject> ModulesExtraction2;

	// Token: 0x04000EC8 RID: 3784
	[Space]
	[Header("Difficulty 3")]
	public List<GameObject> ModulesNormal3;

	// Token: 0x04000EC9 RID: 3785
	public List<GameObject> ModulesPassage3;

	// Token: 0x04000ECA RID: 3786
	public List<GameObject> ModulesDeadEnd3;

	// Token: 0x04000ECB RID: 3787
	public List<GameObject> ModulesExtraction3;

	// Token: 0x04000ECC RID: 3788
	public List<LevelValuables> ValuablePresets;

	// Token: 0x04000ECD RID: 3789
	public LevelMusicAsset MusicPreset;

	// Token: 0x04000ECE RID: 3790
	public ConstantMusicAsset ConstantMusicPreset;

	// Token: 0x04000ECF RID: 3791
	public List<LevelAmbience> AmbiencePresets;

	// Token: 0x04000ED0 RID: 3792
	[Header("Fog")]
	public Color FogColor = Color.black;

	// Token: 0x04000ED1 RID: 3793
	public float FogStartDistance;

	// Token: 0x04000ED2 RID: 3794
	public float FogEndDistance = 15f;

	// Token: 0x04000ED3 RID: 3795
	[Space(20f)]
	[Header("Environment")]
	public Color AmbientColor = new Color(0f, 0f, 0.2f);

	// Token: 0x04000ED4 RID: 3796
	public Color AmbientColorAdaptation = new Color(0.06f, 0.06f, 0.39f);

	// Token: 0x04000ED5 RID: 3797
	[Space(20f)]
	[Header("Color")]
	public float ColorTemperature;

	// Token: 0x04000ED6 RID: 3798
	public Color ColorFilter = Color.white;

	// Token: 0x04000ED7 RID: 3799
	[Space(20f)]
	[Header("Bloom")]
	public float BloomIntensity = 10f;

	// Token: 0x04000ED8 RID: 3800
	public float BloomThreshold = 0.9f;

	// Token: 0x04000ED9 RID: 3801
	[Space(20f)]
	[Header("Vignette")]
	public Color VignetteColor = new Color(0.02f, 0f, 0.22f, 0f);

	// Token: 0x04000EDA RID: 3802
	[Range(0f, 1f)]
	public float VignetteIntensity = 0.4f;

	// Token: 0x04000EDB RID: 3803
	[Range(0f, 1f)]
	public float VignetteSmoothness = 0.7f;
}
