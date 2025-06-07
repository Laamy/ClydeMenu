using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class GraphicsManager : MonoBehaviour
{
	// Token: 0x06000978 RID: 2424 RVA: 0x000589CA File Offset: 0x00056BCA
	private void Awake()
	{
		GraphicsManager.instance = this;
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x000589D4 File Offset: 0x00056BD4
	private void Update()
	{
		if (this.firstSetupTimer > 0f)
		{
			this.firstSetupTimer -= Time.deltaTime;
			if (this.firstSetupTimer <= 0f)
			{
				this.UpdateAll();
			}
			return;
		}
		if (this.fullscreenCheckTimer <= 0f)
		{
			this.fullscreenCheckTimer = 1f;
			if (Screen.fullScreenMode != this.windowMode || Screen.fullScreen != this.windowFullscreen)
			{
				if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
				{
					DataDirector.instance.SettingValueSet(DataDirector.Setting.WindowMode, 0);
				}
				else if (Screen.fullScreenMode == FullScreenMode.Windowed)
				{
					DataDirector.instance.SettingValueSet(DataDirector.Setting.WindowMode, 1);
				}
				this.UpdateWindowMode(false);
				if (GraphicsButtonWindowMode.instance)
				{
					GraphicsButtonWindowMode.instance.UpdateSlider();
					return;
				}
			}
		}
		else
		{
			this.fullscreenCheckTimer -= Time.deltaTime;
		}
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00058AA4 File Offset: 0x00056CA4
	public void UpdateAll()
	{
		this.UpdateVsync();
		this.UpdateMaxFPS();
		this.UpdateLightDistance();
		this.UpdateShadowQuality();
		this.UpdateShadowDistance();
		this.UpdateMotionBlur();
		this.UpdateLensDistortion();
		this.UpdateBloom();
		this.UpdateChromaticAberration();
		this.UpdateGrain();
		this.UpdateWindowMode(false);
		this.UpdateRenderSize();
		this.UpdateGlitchLoop();
		this.UpdateGamma();
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00058B06 File Offset: 0x00056D06
	public void UpdateVsync()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Vsync) == 1)
		{
			QualitySettings.vSyncCount = 1;
			return;
		}
		QualitySettings.vSyncCount = 0;
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00058B24 File Offset: 0x00056D24
	public void UpdateMaxFPS()
	{
		Application.targetFrameRate = DataDirector.instance.SettingValueFetch(DataDirector.Setting.MaxFPS);
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00058B38 File Offset: 0x00056D38
	public void UpdateLightDistance()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.LightDistance))
		{
		case 0:
			this.lightDistance = 10f;
			break;
		case 1:
			this.lightDistance = 15f;
			break;
		case 2:
			this.lightDistance = 20f;
			break;
		case 3:
			this.lightDistance = 25f;
			break;
		case 4:
			this.lightDistance = 30f;
			break;
		}
		LightManager.instance.UpdateInstant();
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00058BB8 File Offset: 0x00056DB8
	public void UpdateShadowQuality()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ShadowQuality))
		{
		case 0:
			QualitySettings.shadowResolution = ShadowResolution.Low;
			return;
		case 1:
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			return;
		case 2:
			QualitySettings.shadowResolution = ShadowResolution.High;
			return;
		case 3:
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00058C04 File Offset: 0x00056E04
	public void UpdateShadowDistance()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ShadowDistance))
		{
		case 0:
			this.shadowDistance = 5f;
			break;
		case 1:
			this.shadowDistance = 10f;
			break;
		case 2:
			this.shadowDistance = 15f;
			break;
		case 3:
			this.shadowDistance = 20f;
			break;
		case 4:
			this.shadowDistance = 25f;
			break;
		}
		QualitySettings.shadowDistance = this.shadowDistance;
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00058C84 File Offset: 0x00056E84
	public void UpdateMotionBlur()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.MotionBlur) == 1)
		{
			PostProcessing.Instance.motionBlur.active = true;
			return;
		}
		PostProcessing.Instance.motionBlur.active = false;
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00058CB6 File Offset: 0x00056EB6
	public void UpdateLensDistortion()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.LensEffect) == 1)
		{
			PostProcessing.Instance.lensDistortion.active = true;
			return;
		}
		PostProcessing.Instance.lensDistortion.active = false;
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00058CE8 File Offset: 0x00056EE8
	public void UpdateBloom()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Bloom) == 1)
		{
			PostProcessing.Instance.bloom.active = true;
			return;
		}
		PostProcessing.Instance.bloom.active = false;
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x00058D1A File Offset: 0x00056F1A
	public void UpdateChromaticAberration()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ChromaticAberration) == 1)
		{
			PostProcessing.Instance.chromaticAberration.active = true;
			return;
		}
		PostProcessing.Instance.chromaticAberration.active = false;
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00058D4C File Offset: 0x00056F4C
	public void UpdateGrain()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Grain) == 1)
		{
			PostProcessing.Instance.grain.active = true;
			return;
		}
		PostProcessing.Instance.grain.active = false;
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00058D80 File Offset: 0x00056F80
	public void UpdateWindowMode(bool _setResolution)
	{
		int num = DataDirector.instance.SettingValueFetch(DataDirector.Setting.WindowMode);
		if (num != 0)
		{
			if (num == 1)
			{
				this.windowMode = FullScreenMode.Windowed;
				this.windowFullscreen = false;
				if (_setResolution)
				{
					List<Resolution> list = new List<Resolution>();
					foreach (Resolution resolution in Screen.resolutions)
					{
						if ((float)resolution.width / (float)resolution.height == 1.7777778f)
						{
							list.Add(resolution);
						}
					}
					Resolution resolution2 = Screen.resolutions[Screen.resolutions.Length - 1];
					if (list.Count > 0)
					{
						resolution2 = list[list.Count / 2];
					}
					Screen.SetResolution(resolution2.width, resolution2.height, this.windowFullscreen);
				}
			}
		}
		else
		{
			this.windowMode = FullScreenMode.FullScreenWindow;
			this.windowFullscreen = true;
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, this.windowFullscreen);
		}
		this.fullscreenCheckTimer = 1f;
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00058E90 File Offset: 0x00057090
	public void UpdateRenderSize()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.RenderSize))
		{
		case 0:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthLarge;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightLarge;
			break;
		case 1:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthMedium;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightMedium;
			break;
		case 2:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthSmall;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightSmall;
			break;
		}
		RenderTextureMain.instance.ResetResolution();
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00058F44 File Offset: 0x00057144
	public void UpdateGlitchLoop()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.GlitchLoop) == 1)
		{
			this.glitchLoop = true;
			return;
		}
		this.glitchLoop = false;
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00058F64 File Offset: 0x00057164
	public void UpdateGamma()
	{
		this.gamma = (float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.Gamma);
		PostProcessing.Instance.colorGrading.gamma.value = new Vector4(0f, 0f, 0f, this.gammaCurve.Evaluate(this.gamma / 100f));
	}

	// Token: 0x040010ED RID: 4333
	public static GraphicsManager instance;

	// Token: 0x040010EE RID: 4334
	internal float lightDistance;

	// Token: 0x040010EF RID: 4335
	internal float shadowDistance;

	// Token: 0x040010F0 RID: 4336
	internal float gamma;

	// Token: 0x040010F1 RID: 4337
	public AnimationCurve gammaCurve;

	// Token: 0x040010F2 RID: 4338
	internal bool glitchLoop;

	// Token: 0x040010F3 RID: 4339
	private float fullscreenCheckTimer;

	// Token: 0x040010F4 RID: 4340
	private FullScreenMode windowMode;

	// Token: 0x040010F5 RID: 4341
	private bool windowFullscreen;

	// Token: 0x040010F6 RID: 4342
	private float firstSetupTimer = 1f;
}
