using System;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class AnimNoise : MonoBehaviour
{
	// Token: 0x0600128B RID: 4747 RVA: 0x000A680C File Offset: 0x000A4A0C
	public void NoiseOverride(float time, float speed, float strength, float introSpeed, float outroSpeed)
	{
		this.noiseOverrideTimer = time;
		this.noiseOverrideSpeed = Mathf.Max(speed, speed * this.noiseOverrideMultSpeed);
		this.noiseOverrideStrength = Mathf.Max(strength, strength * this.noiseOverrideMultStrength);
		this.noiseOverrideIntroSpeed = introSpeed;
		this.noiseOverrideOutroSpeed = outroSpeed;
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x000A6858 File Offset: 0x000A4A58
	private void Update()
	{
		float num = Mathf.LerpUnclamped(this.noiseRotXOld, this.noiseRotXNew, this.noiseCurve.Evaluate(this.noiseRotXLerp));
		this.noiseRotXLerp += this.noiseRotXSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noiseRotXLerp >= 1f)
		{
			this.noiseRotXOld = this.noiseRotXNew;
			this.noiseRotXNew = Random.Range(this.noiseRotXAmountMin, this.noiseRotXAmountMax) * this.noiseStrength;
			this.noiseRotXSpeed = Random.Range(this.noiseRotXSpeedMin, this.noiseRotXSpeedMax);
			this.noiseRotXLerp = 0f;
		}
		float num2 = Mathf.LerpUnclamped(this.noiseRotYOld, this.noiseRotYNew, this.noiseCurve.Evaluate(this.noiseRotYLerp));
		this.noiseRotYLerp += this.noiseRotYSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noiseRotYLerp >= 1f)
		{
			this.noiseRotYOld = this.noiseRotYNew;
			this.noiseRotYNew = Random.Range(this.noiseRotYAmountMin, this.noiseRotYAmountMax) * this.noiseStrength;
			this.noiseRotYSpeed = Random.Range(this.noiseRotYSpeedMin, this.noiseRotYSpeedMax);
			this.noiseRotYLerp = 0f;
		}
		float num3 = Mathf.LerpUnclamped(this.noiseRotZOld, this.noiseRotZNew, this.noiseCurve.Evaluate(this.noiseRotZLerp));
		this.noiseRotZLerp += this.noiseRotZSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noiseRotZLerp >= 1f)
		{
			this.noiseRotZOld = this.noiseRotZNew;
			this.noiseRotZNew = Random.Range(this.noiseRotZAmountMin, this.noiseRotZAmountMax) * this.noiseStrength;
			this.noiseRotZSpeed = Random.Range(this.noiseRotZSpeedMin, this.noiseRotZSpeedMax);
			this.noiseRotZLerp = 0f;
		}
		float num4 = Mathf.LerpUnclamped(this.noisePosXOld, this.noisePosXNew, this.noiseCurve.Evaluate(this.noisePosXLerp));
		this.noisePosXLerp += this.noisePosXSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noisePosXLerp >= 1f)
		{
			this.noisePosXOld = this.noisePosXNew;
			this.noisePosXNew = Random.Range(this.noisePosXAmountMin, this.noisePosXAmountMax) * this.noiseStrength;
			this.noisePosXSpeed = Random.Range(this.noisePosXSpeedMin, this.noisePosXSpeedMax);
			this.noisePosXLerp = 0f;
		}
		float num5 = Mathf.LerpUnclamped(this.noisePosYOld, this.noisePosYNew, this.noiseCurve.Evaluate(this.noisePosYLerp));
		this.noisePosYLerp += this.noisePosYSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noisePosYLerp >= 1f)
		{
			this.noisePosYOld = this.noisePosYNew;
			this.noisePosYNew = Random.Range(this.noisePosYAmountMin, this.noisePosYAmountMax) * this.noiseStrength;
			this.noisePosYSpeed = Random.Range(this.noisePosYSpeedMin, this.noisePosYSpeedMax);
			this.noisePosYLerp = 0f;
		}
		float num6 = Mathf.LerpUnclamped(this.noisePosZOld, this.noisePosZNew, this.noiseCurve.Evaluate(this.noisePosZLerp));
		this.noisePosZLerp += this.noisePosZSpeed * this.noiseSpeed * Time.deltaTime;
		if (this.noisePosZLerp >= 1f)
		{
			this.noisePosZOld = this.noisePosZNew;
			this.noisePosZNew = Random.Range(this.noisePosZAmountMin, this.noisePosZAmountMax) * this.noiseStrength;
			this.noisePosZSpeed = Random.Range(this.noisePosZSpeedMin, this.noisePosZSpeedMax);
			this.noisePosZLerp = 0f;
		}
		if (this.noiseOverrideTimer > 0f)
		{
			this.noiseOverrideLerp = Mathf.Clamp01(this.noiseOverrideLerp + this.noiseOverrideIntroSpeed * Time.deltaTime);
			this.noiseOverrideTimer -= Time.deltaTime;
		}
		else
		{
			this.noiseOverrideLerp = Mathf.Clamp01(this.noiseOverrideLerp - this.noiseOverrideOutroSpeed * Time.deltaTime);
		}
		this.noiseStrength = Mathf.Lerp(this.noiseStrengthDefault, this.noiseOverrideStrength, this.noiseOverrideCurve.Evaluate(this.noiseOverrideLerp));
		this.noiseSpeed = Mathf.Lerp(this.noiseSpeedDefault, this.noiseOverrideSpeed, this.noiseOverrideCurve.Evaluate(this.noiseOverrideLerp));
		base.transform.localPosition = new Vector3(num4 * this.MasterAmount, num5 * this.MasterAmount, num6 * this.MasterAmount);
		base.transform.localRotation = Quaternion.Euler(num * this.MasterAmount, num2 * this.MasterAmount, num3 * this.MasterAmount);
	}

	// Token: 0x04001F18 RID: 7960
	public AnimationCurve noiseCurve;

	// Token: 0x04001F19 RID: 7961
	public AnimationCurve noiseOverrideCurve;

	// Token: 0x04001F1A RID: 7962
	public float noiseStrengthDefault = 1f;

	// Token: 0x04001F1B RID: 7963
	public float noiseSpeedDefault = 1f;

	// Token: 0x04001F1C RID: 7964
	private float noiseStrength = 1f;

	// Token: 0x04001F1D RID: 7965
	private float noiseSpeed = 1f;

	// Token: 0x04001F1E RID: 7966
	[HideInInspector]
	public float MasterAmount = 1f;

	// Token: 0x04001F1F RID: 7967
	[Header("Override Multipliers")]
	public float noiseOverrideMultStrength = 1f;

	// Token: 0x04001F20 RID: 7968
	public float noiseOverrideMultSpeed = 1f;

	// Token: 0x04001F21 RID: 7969
	private float noiseOverrideLerp;

	// Token: 0x04001F22 RID: 7970
	private float noiseOverrideTimer;

	// Token: 0x04001F23 RID: 7971
	private float noiseOverrideStrength;

	// Token: 0x04001F24 RID: 7972
	private float noiseOverrideSpeed;

	// Token: 0x04001F25 RID: 7973
	private float noiseOverrideIntroSpeed;

	// Token: 0x04001F26 RID: 7974
	private float noiseOverrideOutroSpeed;

	// Token: 0x04001F27 RID: 7975
	[Header("Rotation X")]
	public float noiseRotXAmountMin;

	// Token: 0x04001F28 RID: 7976
	public float noiseRotXAmountMax;

	// Token: 0x04001F29 RID: 7977
	public float noiseRotXSpeedMin;

	// Token: 0x04001F2A RID: 7978
	public float noiseRotXSpeedMax;

	// Token: 0x04001F2B RID: 7979
	private float noiseRotXLerp = 1f;

	// Token: 0x04001F2C RID: 7980
	private float noiseRotXNew;

	// Token: 0x04001F2D RID: 7981
	private float noiseRotXOld;

	// Token: 0x04001F2E RID: 7982
	private float noiseRotXSpeed;

	// Token: 0x04001F2F RID: 7983
	[Header("Rotation Y")]
	public float noiseRotYAmountMin;

	// Token: 0x04001F30 RID: 7984
	public float noiseRotYAmountMax;

	// Token: 0x04001F31 RID: 7985
	public float noiseRotYSpeedMin;

	// Token: 0x04001F32 RID: 7986
	public float noiseRotYSpeedMax;

	// Token: 0x04001F33 RID: 7987
	private float noiseRotYLerp = 1f;

	// Token: 0x04001F34 RID: 7988
	private float noiseRotYNew;

	// Token: 0x04001F35 RID: 7989
	private float noiseRotYOld;

	// Token: 0x04001F36 RID: 7990
	private float noiseRotYSpeed;

	// Token: 0x04001F37 RID: 7991
	[Header("Rotation Z")]
	public float noiseRotZAmountMin;

	// Token: 0x04001F38 RID: 7992
	public float noiseRotZAmountMax;

	// Token: 0x04001F39 RID: 7993
	public float noiseRotZSpeedMin;

	// Token: 0x04001F3A RID: 7994
	public float noiseRotZSpeedMax;

	// Token: 0x04001F3B RID: 7995
	private float noiseRotZLerp = 1f;

	// Token: 0x04001F3C RID: 7996
	private float noiseRotZNew;

	// Token: 0x04001F3D RID: 7997
	private float noiseRotZOld;

	// Token: 0x04001F3E RID: 7998
	private float noiseRotZSpeed;

	// Token: 0x04001F3F RID: 7999
	[Header("Position X")]
	public float noisePosXAmountMin;

	// Token: 0x04001F40 RID: 8000
	public float noisePosXAmountMax;

	// Token: 0x04001F41 RID: 8001
	public float noisePosXSpeedMin;

	// Token: 0x04001F42 RID: 8002
	public float noisePosXSpeedMax;

	// Token: 0x04001F43 RID: 8003
	private float noisePosXLerp = 1f;

	// Token: 0x04001F44 RID: 8004
	private float noisePosXNew;

	// Token: 0x04001F45 RID: 8005
	private float noisePosXOld;

	// Token: 0x04001F46 RID: 8006
	private float noisePosXSpeed;

	// Token: 0x04001F47 RID: 8007
	[Header("Position Y")]
	public float noisePosYAmountMin;

	// Token: 0x04001F48 RID: 8008
	public float noisePosYAmountMax;

	// Token: 0x04001F49 RID: 8009
	public float noisePosYSpeedMin;

	// Token: 0x04001F4A RID: 8010
	public float noisePosYSpeedMax;

	// Token: 0x04001F4B RID: 8011
	private float noisePosYLerp = 1f;

	// Token: 0x04001F4C RID: 8012
	private float noisePosYNew;

	// Token: 0x04001F4D RID: 8013
	private float noisePosYOld;

	// Token: 0x04001F4E RID: 8014
	private float noisePosYSpeed;

	// Token: 0x04001F4F RID: 8015
	[Header("Position Z")]
	public float noisePosZAmountMin;

	// Token: 0x04001F50 RID: 8016
	public float noisePosZAmountMax;

	// Token: 0x04001F51 RID: 8017
	public float noisePosZSpeedMin;

	// Token: 0x04001F52 RID: 8018
	public float noisePosZSpeedMax;

	// Token: 0x04001F53 RID: 8019
	private float noisePosZLerp = 1f;

	// Token: 0x04001F54 RID: 8020
	private float noisePosZNew;

	// Token: 0x04001F55 RID: 8021
	private float noisePosZOld;

	// Token: 0x04001F56 RID: 8022
	private float noisePosZSpeed;
}
