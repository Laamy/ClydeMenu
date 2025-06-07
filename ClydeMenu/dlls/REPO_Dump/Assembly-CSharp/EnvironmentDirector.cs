using System;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class EnvironmentDirector : MonoBehaviour
{
	// Token: 0x0600079E RID: 1950 RVA: 0x00048629 File Offset: 0x00046829
	private void Awake()
	{
		EnvironmentDirector.Instance = this;
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x00048631 File Offset: 0x00046831
	private void Start()
	{
		this.MainCamera = Camera.main;
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x00048640 File Offset: 0x00046840
	public void Setup()
	{
		RenderSettings.fogColor = LevelGenerator.Instance.Level.FogColor;
		RenderSettings.fogStartDistance = LevelGenerator.Instance.Level.FogStartDistance;
		RenderSettings.fogEndDistance = LevelGenerator.Instance.Level.FogEndDistance;
		this.MainCamera.backgroundColor = RenderSettings.fogColor;
		this.MainCamera.farClipPlane = RenderSettings.fogEndDistance + 1f;
		this.DarkAdaptationLerp = 0.1f;
		if (LevelGenerator.Instance.Level.AmbiencePresets.Count > 0)
		{
			AmbienceLoop.instance.Setup();
			AmbienceBreakers.instance.Setup();
		}
		else
		{
			Debug.LogError("Level is missing ambience preset!");
		}
		this.SetupDone = true;
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x000486F8 File Offset: 0x000468F8
	private void Update()
	{
		if (!this.SetupDone)
		{
			return;
		}
		Color ambientColor = LevelGenerator.Instance.Level.AmbientColor;
		Color ambientColorAdaptation = LevelGenerator.Instance.Level.AmbientColorAdaptation;
		if (!FlashlightController.Instance.LightActive)
		{
			if (this.DarkAdaptationLerp < 1f)
			{
				this.DarkAdaptationLerp += Time.deltaTime * this.DarkAdaptationSpeedIn;
				this.DarkAdaptationLerp = Mathf.Clamp01(this.DarkAdaptationLerp);
				RenderSettings.ambientLight = Color.Lerp(ambientColor, ambientColorAdaptation, this.DarkAdaptationCurve.Evaluate(this.DarkAdaptationLerp));
				return;
			}
		}
		else if (this.DarkAdaptationLerp > 0f)
		{
			this.DarkAdaptationLerp -= Time.deltaTime * this.DarkAdaptationSpeedOut;
			this.DarkAdaptationLerp = Mathf.Clamp01(this.DarkAdaptationLerp);
			RenderSettings.ambientLight = Color.Lerp(ambientColor, ambientColorAdaptation, this.DarkAdaptationCurve.Evaluate(this.DarkAdaptationLerp));
		}
	}

	// Token: 0x04000D6B RID: 3435
	public static EnvironmentDirector Instance;

	// Token: 0x04000D6C RID: 3436
	private bool SetupDone;

	// Token: 0x04000D6D RID: 3437
	private Camera MainCamera;

	// Token: 0x04000D6E RID: 3438
	[Space]
	public float DarkAdaptationSpeedIn = 5f;

	// Token: 0x04000D6F RID: 3439
	public float DarkAdaptationSpeedOut = 5f;

	// Token: 0x04000D70 RID: 3440
	public AnimationCurve DarkAdaptationCurve;

	// Token: 0x04000D71 RID: 3441
	private float DarkAdaptationLerp;
}
