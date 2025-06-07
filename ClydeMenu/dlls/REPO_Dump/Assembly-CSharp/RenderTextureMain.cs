using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000290 RID: 656
public class RenderTextureMain : MonoBehaviour
{
	// Token: 0x060014A2 RID: 5282 RVA: 0x000B657E File Offset: 0x000B477E
	private void Awake()
	{
		RenderTextureMain.instance = this;
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x000B6588 File Offset: 0x000B4788
	private void Start()
	{
		this.textureWidthOriginal = this.textureWidthSmall;
		this.textureHeightOriginal = this.textureHeightSmall;
		this.originalSize = base.transform.localScale;
		foreach (Camera camera in Camera.main.GetComponentsInChildren<Camera>())
		{
			this.cameras.Add(camera);
		}
		this.ResetResolution();
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x000B65F0 File Offset: 0x000B47F0
	private void Update()
	{
		if (this.shakeActive)
		{
			if (this.shakeXLerp >= 1f)
			{
				this.shakeXLerp = 0f;
				this.shakeXOld = this.shakeXNew;
				this.shakeXNew = Random.Range(-5f, 5f);
			}
			else
			{
				this.shakeXLerp += Time.deltaTime * 100f;
				this.shakeX = Mathf.Lerp(this.shakeXOld, this.shakeXNew, this.shakeCurve.Evaluate(this.shakeXLerp));
			}
			if (this.shakeYLerp >= 1f)
			{
				this.shakeYLerp = 0f;
				this.shakeYOld = this.shakeYNew;
				this.shakeYNew = Random.Range(-5f, 5f);
			}
			else
			{
				this.shakeYLerp += Time.deltaTime * 100f;
				this.shakeY = Mathf.Lerp(this.shakeYOld, this.shakeYNew, this.shakeCurve.Evaluate(this.shakeYLerp));
			}
			base.transform.localPosition = new Vector3(this.shakeX, this.shakeY, 0f);
			this.shakeTimer -= Time.deltaTime;
			if (this.shakeTimer <= 0f)
			{
				base.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.shakeActive = false;
			}
		}
		if (this.sizeResetTimer > 0f)
		{
			this.sizeResetTimer -= Time.deltaTime;
		}
		else if (base.transform.localScale != this.originalSize)
		{
			base.transform.localScale = this.originalSize;
		}
		if (this.textureResetTimer > 0f)
		{
			this.textureResetTimer -= Time.deltaTime;
		}
		else if (this.renderTexture.width != (int)this.textureWidthOriginal || this.renderTexture.height != (int)this.textureHeightOriginal)
		{
			this.ResetResolution();
		}
		if (this.overlayDisableTimer > 0f)
		{
			this.overlayDisableTimer -= Time.deltaTime;
			if (this.overlayDisableTimer <= 0f)
			{
				this.overlayRawImage.enabled = true;
			}
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x000B6835 File Offset: 0x000B4A35
	public void Shake(float _time)
	{
		this.shakeActive = true;
		this.shakeTimer = _time;
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x000B6845 File Offset: 0x000B4A45
	public void ChangeSize(float _width, float _height, float _time)
	{
		base.transform.localScale = new Vector3(_width, _height, 1f);
		this.sizeResetTimer = _time;
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x000B6865 File Offset: 0x000B4A65
	public void ChangeResolution(float _width, float _height, float _time)
	{
		this.textureWidth = _width;
		this.textureHeight = _height;
		this.SetRenderTexture();
		this.textureResetTimer = _time;
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x000B6882 File Offset: 0x000B4A82
	public void ResetResolution()
	{
		this.textureWidth = this.textureWidthOriginal;
		this.textureHeight = this.textureHeightOriginal;
		this.SetRenderTexture();
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x000B68A4 File Offset: 0x000B4AA4
	private void SetRenderTexture()
	{
		this.renderTexture.Release();
		this.renderTexture.width = (int)this.textureWidth;
		this.renderTexture.height = (int)this.textureHeight;
		this.renderTexture.Create();
		this.cameras[0].targetTexture = this.renderTexture;
		foreach (Camera camera in this.cameras)
		{
			camera.enabled = false;
			camera.enabled = true;
		}
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x000B6950 File Offset: 0x000B4B50
	private void OnApplicationQuit()
	{
		this.textureWidth = this.textureWidthSmall;
		this.textureHeight = this.textureHeightSmall;
		this.SetRenderTexture();
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x000B6970 File Offset: 0x000B4B70
	public void OverlayDisable()
	{
		this.overlayRawImage.enabled = false;
		this.overlayDisableTimer = 0.5f;
	}

	// Token: 0x0400235F RID: 9055
	public static RenderTextureMain instance;

	// Token: 0x04002360 RID: 9056
	private List<Camera> cameras = new List<Camera>();

	// Token: 0x04002361 RID: 9057
	public RenderTexture renderTexture;

	// Token: 0x04002362 RID: 9058
	[Space]
	public float textureWidthSmall;

	// Token: 0x04002363 RID: 9059
	public float textureHeightSmall;

	// Token: 0x04002364 RID: 9060
	[Space]
	public float textureWidthMedium;

	// Token: 0x04002365 RID: 9061
	public float textureHeightMedium;

	// Token: 0x04002366 RID: 9062
	[Space]
	public float textureWidthLarge;

	// Token: 0x04002367 RID: 9063
	public float textureHeightLarge;

	// Token: 0x04002368 RID: 9064
	internal float textureWidthOriginal;

	// Token: 0x04002369 RID: 9065
	internal float textureHeightOriginal;

	// Token: 0x0400236A RID: 9066
	internal float textureWidth;

	// Token: 0x0400236B RID: 9067
	internal float textureHeight;

	// Token: 0x0400236C RID: 9068
	internal float textureResetTimer;

	// Token: 0x0400236D RID: 9069
	internal float sizeResetTimer;

	// Token: 0x0400236E RID: 9070
	[Space]
	public AnimationCurve shakeCurve;

	// Token: 0x0400236F RID: 9071
	private float shakeTimer;

	// Token: 0x04002370 RID: 9072
	private bool shakeActive;

	// Token: 0x04002371 RID: 9073
	private float shakeX;

	// Token: 0x04002372 RID: 9074
	private float shakeXOld;

	// Token: 0x04002373 RID: 9075
	private float shakeXNew;

	// Token: 0x04002374 RID: 9076
	private float shakeXLerp = 1f;

	// Token: 0x04002375 RID: 9077
	private float shakeY;

	// Token: 0x04002376 RID: 9078
	private float shakeYOld;

	// Token: 0x04002377 RID: 9079
	private float shakeYNew;

	// Token: 0x04002378 RID: 9080
	private float shakeYLerp = 1f;

	// Token: 0x04002379 RID: 9081
	private Vector3 originalSize;

	// Token: 0x0400237A RID: 9082
	public RawImage overlayRawImage;

	// Token: 0x0400237B RID: 9083
	private float overlayDisableTimer;
}
