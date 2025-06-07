using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000248 RID: 584
public class PostProcessing : MonoBehaviour
{
	// Token: 0x060012FA RID: 4858 RVA: 0x000A9CC3 File Offset: 0x000A7EC3
	private void Awake()
	{
		PostProcessing.Instance = this;
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x000A9CCC File Offset: 0x000A7ECC
	private void Start()
	{
		this.volume.profile.TryGetSettings<Grain>(out this.grain);
		this.grainSizeDefault = this.grain.size.value;
		this.grainIntensityDefault = this.grain.intensity.value;
		this.grain.intensity.value = 1f;
		this.volume.profile.TryGetSettings<MotionBlur>(out this.motionBlur);
		this.motionBlurDefault = this.motionBlur.shutterAngle.value;
		this.volume.profile.TryGetSettings<LensDistortion>(out this.lensDistortion);
		this.volume.profile.TryGetSettings<Bloom>(out this.bloom);
		this.volume.profile.TryGetSettings<ColorGrading>(out this.colorGrading);
		this.volume.profile.TryGetSettings<Vignette>(out this.vignette);
		this.volume.profile.TryGetSettings<ChromaticAberration>(out this.chromaticAberration);
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x000A9DD4 File Offset: 0x000A7FD4
	private void Update()
	{
		if (!this.setupDone)
		{
			return;
		}
		Color color = this.vignetteColor;
		float num = this.vignetteIntensity;
		float num2 = this.vignetteSmoothness;
		if (this.vignetteOverrideActive)
		{
			if (this.vignetteOverrideTimer > 0f)
			{
				color = Color.Lerp(color, this.vignetteOverrideColor, this.vignetteOverrideLerp);
				num = Mathf.Lerp(num, this.vignetteOverrideIntensity, this.vignetteOverrideLerp);
				num2 = Mathf.Lerp(num2, this.vignetteOverrideSmoothness, this.vignetteOverrideLerp);
				this.vignetteOverrideLerp += this.vignetteOverrideSpeedIn * Time.deltaTime;
				this.vignetteOverrideLerp = Mathf.Clamp01(this.vignetteOverrideLerp);
				this.vignetteOverrideTimer -= Time.deltaTime;
			}
			else
			{
				color = Color.Lerp(color, this.vignetteOverrideColor, this.vignetteOverrideLerp);
				num = Mathf.Lerp(num, this.vignetteOverrideIntensity, this.vignetteOverrideLerp);
				num2 = Mathf.Lerp(num2, this.vignetteOverrideSmoothness, this.vignetteOverrideLerp);
				this.vignetteOverrideLerp -= this.vignetteOverrideSpeedOut * Time.deltaTime;
				if (this.vignetteOverrideLerp <= 0f)
				{
					this.vignetteOverrideActive = false;
					this.vignetteOverrideLerp = 0f;
				}
			}
		}
		this.vignette.color.value = color;
		this.vignette.intensity.value = num;
		this.vignette.smoothness.value = num2;
		if (this.saturationOverrideActive)
		{
			if (this.saturationOverrideTimer > 0f)
			{
				this.colorGrading.saturation.value = Mathf.Lerp(this.colorGradingSaturation, this.saturationOverrideAmount, this.saturationOverrideLerp);
				this.saturationOverrideLerp += this.saturationOverrideSpeedIn * Time.deltaTime;
				this.saturationOverrideLerp = Mathf.Clamp01(this.saturationOverrideLerp);
				this.saturationOverrideTimer -= Time.deltaTime;
			}
			else
			{
				this.colorGrading.saturation.value = Mathf.Lerp(this.colorGradingSaturation, this.saturationOverrideAmount, this.saturationOverrideLerp);
				this.saturationOverrideLerp -= this.saturationOverrideSpeedOut * Time.deltaTime;
				if (this.saturationOverrideLerp <= 0f)
				{
					this.colorGrading.saturation.value = this.colorGradingSaturation;
					this.saturationOverrideActive = false;
					this.saturationOverrideLerp = 0f;
				}
			}
		}
		if (this.contrastOverrideActive)
		{
			if (this.contrastOverrideTimer > 0f)
			{
				this.colorGrading.contrast.value = Mathf.Lerp(this.colorGradingContrast, this.contrastOverrideAmount, this.contrastOverrideLerp);
				this.contrastOverrideLerp += this.contrastOverrideSpeedIn * Time.deltaTime;
				this.contrastOverrideLerp = Mathf.Clamp01(this.contrastOverrideLerp);
				this.contrastOverrideTimer -= Time.deltaTime;
			}
			else
			{
				this.colorGrading.contrast.value = Mathf.Lerp(this.colorGradingContrast, this.contrastOverrideAmount, this.contrastOverrideLerp);
				this.contrastOverrideLerp -= this.contrastOverrideSpeedOut * Time.deltaTime;
				if (this.contrastOverrideLerp <= 0f)
				{
					this.colorGrading.contrast.value = this.colorGradingContrast;
					this.contrastOverrideActive = false;
					this.contrastOverrideLerp = 0f;
				}
			}
		}
		if (this.bloomDisableTimer > 0f)
		{
			this.bloomDisableTimer -= Time.deltaTime;
			if (this.bloomDisableTimer <= 0f && DataDirector.instance.SettingValueFetch(DataDirector.Setting.Bloom) == 1)
			{
				this.bloom.active = true;
			}
		}
		if (this.grainDisableTimer > 0f)
		{
			this.grainDisableTimer -= Time.deltaTime;
			if (this.grainDisableTimer <= 0f && DataDirector.instance.SettingValueFetch(DataDirector.Setting.Grain) == 1)
			{
				this.grain.active = true;
			}
		}
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x000AA1A4 File Offset: 0x000A83A4
	public void SpectateSet()
	{
		this.motionBlur.shutterAngle.value = 1f;
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x000AA1BB File Offset: 0x000A83BB
	public void SpectateReset()
	{
		this.motionBlur.shutterAngle.value = this.motionBlurDefault;
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x000AA1D4 File Offset: 0x000A83D4
	public void Setup()
	{
		this.colorGrading.temperature.value = LevelGenerator.Instance.Level.ColorTemperature;
		this.colorGrading.colorFilter.value = LevelGenerator.Instance.Level.ColorFilter;
		this.colorGradingSaturation = this.colorGrading.saturation.value;
		this.colorGradingContrast = this.colorGrading.contrast.value;
		this.bloom.intensity.value = LevelGenerator.Instance.Level.BloomIntensity;
		this.bloom.threshold.value = LevelGenerator.Instance.Level.BloomThreshold;
		this.vignette.color.value = LevelGenerator.Instance.Level.VignetteColor;
		this.vignetteColor = this.vignette.color.value;
		this.vignette.intensity.value = LevelGenerator.Instance.Level.VignetteIntensity;
		this.vignetteIntensity = this.vignette.intensity.value;
		this.vignette.smoothness.value = LevelGenerator.Instance.Level.VignetteSmoothness;
		this.vignetteSmoothness = this.vignette.smoothness.value;
		base.StartCoroutine(this.Intro());
		this.setupDone = true;
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x000AA33C File Offset: 0x000A853C
	private IEnumerator Intro()
	{
		while (GameDirector.instance.currentState < GameDirector.gameState.Main)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (this.introLerp < 1f)
		{
			this.grain.intensity.value = Mathf.Lerp(0.8f, this.grainIntensityDefault, this.introCurve.Evaluate(this.introLerp));
			this.grain.size.value = Mathf.Lerp(1.5f, this.grainSizeDefault, this.introCurve.Evaluate(this.introLerp));
			this.introLerp += this.introSpeed * Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x000AA34C File Offset: 0x000A854C
	public void VignetteOverride(Color _color, float _intensity, float _smoothness, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		if (this.vignetteOverrideActive && _obj != this.vignetteOverrideObject)
		{
			return;
		}
		_smoothness = Mathf.Clamp01(_smoothness);
		this.vignetteOverrideActive = true;
		this.vignetteOverrideObject = _obj;
		this.vignetteOverrideTimer = _time;
		this.vignetteOverrideSpeedIn = _speedIn;
		this.vignetteOverrideSpeedOut = _speedOut;
		this.vignetteOverrideColor = _color;
		this.vignetteOverrideIntensity = _intensity;
		this.vignetteOverrideSmoothness = _smoothness;
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x000AA3B8 File Offset: 0x000A85B8
	public void SaturationOverride(float _amount, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		if (this.saturationOverrideActive && _obj != this.saturationOverrideObject)
		{
			return;
		}
		this.saturationOverrideActive = true;
		this.saturationOverrideObject = _obj;
		this.saturationOverrideTimer = _time;
		this.saturationOverrideSpeedIn = _speedIn;
		this.saturationOverrideSpeedOut = _speedOut;
		this.saturationOverrideAmount = _amount;
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x000AA40C File Offset: 0x000A860C
	public void ContrastOverride(float _amount, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		if (this.contrastOverrideActive && _obj != this.contrastOverrideObject)
		{
			return;
		}
		this.contrastOverrideActive = true;
		this.contrastOverrideObject = _obj;
		this.contrastOverrideTimer = _time;
		this.contrastOverrideSpeedIn = _speedIn;
		this.contrastOverrideSpeedOut = _speedOut;
		this.contrastOverrideAmount = _amount;
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x000AA45D File Offset: 0x000A865D
	public void BloomDisable(float _time)
	{
		this.bloomDisableTimer = _time;
		this.bloom.active = false;
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x000AA472 File Offset: 0x000A8672
	public void GrainDisable(float _time)
	{
		this.grainDisableTimer = _time;
		this.grain.active = false;
	}

	// Token: 0x04002044 RID: 8260
	public static PostProcessing Instance;

	// Token: 0x04002045 RID: 8261
	private bool setupDone;

	// Token: 0x04002046 RID: 8262
	public PostProcessVolume volume;

	// Token: 0x04002047 RID: 8263
	internal Grain grain;

	// Token: 0x04002048 RID: 8264
	private float grainDisableTimer;

	// Token: 0x04002049 RID: 8265
	internal Bloom bloom;

	// Token: 0x0400204A RID: 8266
	private float bloomDisableTimer;

	// Token: 0x0400204B RID: 8267
	internal ColorGrading colorGrading;

	// Token: 0x0400204C RID: 8268
	private float colorGradingSaturation;

	// Token: 0x0400204D RID: 8269
	private float colorGradingContrast;

	// Token: 0x0400204E RID: 8270
	internal Vignette vignette;

	// Token: 0x0400204F RID: 8271
	private Color vignetteColor;

	// Token: 0x04002050 RID: 8272
	private float vignetteIntensity;

	// Token: 0x04002051 RID: 8273
	private float vignetteSmoothness;

	// Token: 0x04002052 RID: 8274
	internal MotionBlur motionBlur;

	// Token: 0x04002053 RID: 8275
	internal LensDistortion lensDistortion;

	// Token: 0x04002054 RID: 8276
	internal ChromaticAberration chromaticAberration;

	// Token: 0x04002055 RID: 8277
	public AnimationCurve introCurve;

	// Token: 0x04002056 RID: 8278
	public float introSpeed;

	// Token: 0x04002057 RID: 8279
	private float introLerp;

	// Token: 0x04002058 RID: 8280
	private float motionBlurDefault;

	// Token: 0x04002059 RID: 8281
	private float bloomDefault;

	// Token: 0x0400205A RID: 8282
	private float grainIntensityDefault;

	// Token: 0x0400205B RID: 8283
	private float grainSizeDefault;

	// Token: 0x0400205C RID: 8284
	[Space]
	private bool vignetteOverrideActive;

	// Token: 0x0400205D RID: 8285
	private float vignetteOverrideLerp;

	// Token: 0x0400205E RID: 8286
	private float vignetteOverrideTimer;

	// Token: 0x0400205F RID: 8287
	private float vignetteOverrideSpeedIn;

	// Token: 0x04002060 RID: 8288
	private float vignetteOverrideSpeedOut;

	// Token: 0x04002061 RID: 8289
	private Color vignetteOverrideColor;

	// Token: 0x04002062 RID: 8290
	private float vignetteOverrideIntensity;

	// Token: 0x04002063 RID: 8291
	private float vignetteOverrideSmoothness;

	// Token: 0x04002064 RID: 8292
	private GameObject vignetteOverrideObject;

	// Token: 0x04002065 RID: 8293
	private bool saturationOverrideActive;

	// Token: 0x04002066 RID: 8294
	private float saturationOverrideLerp;

	// Token: 0x04002067 RID: 8295
	private float saturationOverrideTimer;

	// Token: 0x04002068 RID: 8296
	private float saturationOverrideSpeedIn;

	// Token: 0x04002069 RID: 8297
	private float saturationOverrideSpeedOut;

	// Token: 0x0400206A RID: 8298
	private float saturationOverrideAmount;

	// Token: 0x0400206B RID: 8299
	private GameObject saturationOverrideObject;

	// Token: 0x0400206C RID: 8300
	private bool contrastOverrideActive;

	// Token: 0x0400206D RID: 8301
	private float contrastOverrideLerp;

	// Token: 0x0400206E RID: 8302
	private float contrastOverrideTimer;

	// Token: 0x0400206F RID: 8303
	private float contrastOverrideSpeedIn;

	// Token: 0x04002070 RID: 8304
	private float contrastOverrideSpeedOut;

	// Token: 0x04002071 RID: 8305
	private float contrastOverrideAmount;

	// Token: 0x04002072 RID: 8306
	private GameObject contrastOverrideObject;
}
