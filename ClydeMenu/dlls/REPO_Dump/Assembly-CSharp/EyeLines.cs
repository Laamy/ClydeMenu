using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002B3 RID: 691
public class EyeLines : MonoBehaviour
{
	// Token: 0x0600159E RID: 5534 RVA: 0x000BF356 File Offset: 0x000BD556
	public bool IsActive()
	{
		return this.isActive;
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x000BF35E File Offset: 0x000BD55E
	public void SetIsActive(bool _isActive)
	{
		if (_isActive && !this.isActive)
		{
			this.lineRendererLeft.widthMultiplier = 0f;
			this.lineRendererRight.widthMultiplier = 0f;
		}
		this.isActive = _isActive;
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x000BF392 File Offset: 0x000BD592
	private void Awake()
	{
		this.lineRendererLeft.widthMultiplier = 0f;
		this.lineRendererRight.widthMultiplier = 0f;
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x000BF3B4 File Offset: 0x000BD5B4
	public void InitializeLine(PlayerAvatar _targetPlayer)
	{
		this.targetPlayer = _targetPlayer;
		this.isActive = true;
		base.transform.localPosition = Vector3.zero;
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x000BF3D4 File Offset: 0x000BD5D4
	public void DrawLine(LineRenderer _lineRenderer, Vector3 _startPoint, Vector3 _endPoint)
	{
		if (this.isActive)
		{
			this.FadeLineIn(_lineRenderer);
			Vector3[] array = new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 19f;
				array[i] = Vector3.Lerp(_startPoint, _endPoint, num) - Vector3.up * Mathf.Sin(num * 3.1415927f) * 0.5f;
				float d = 1f - Mathf.Abs(num - 0.5f) * 2f;
				float num2 = 1f;
				array[i] += Vector3.right * Mathf.Sin(Time.time * (30f * num2) + (float)i) * 0.02f * d;
				array[i] += Vector3.forward * Mathf.Cos(Time.time * (30f * num2) + (float)i) * 0.02f * d;
			}
			_lineRenderer.material.mainTextureOffset = new Vector2(Time.time * this.textureScrollSpeed, 0f);
			_lineRenderer.positionCount = 20;
			_lineRenderer.SetPositions(array);
			return;
		}
		this.FadeLineOut(_lineRenderer);
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x000BF534 File Offset: 0x000BD734
	public void DrawLines()
	{
		this.DrawLine(this.lineRendererLeft, base.transform.position, this.targetPlayer.playerAvatarVisuals.playerEyes.pupilLeft.position);
		this.DrawLine(this.lineRendererRight, base.transform.position, this.targetPlayer.playerAvatarVisuals.playerEyes.pupilRight.position);
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x000BF5A3 File Offset: 0x000BD7A3
	private void Update()
	{
		base.transform.localPosition = Vector3.zero;
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x000BF5B5 File Offset: 0x000BD7B5
	private void FadeLineIn(LineRenderer _lineRenderer)
	{
		if (_lineRenderer.widthMultiplier < 0.195f)
		{
			_lineRenderer.widthMultiplier = Mathf.Lerp(_lineRenderer.widthMultiplier, 0.2f, Time.deltaTime * 2f);
			return;
		}
		_lineRenderer.widthMultiplier = 0.2f;
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x000BF5F1 File Offset: 0x000BD7F1
	private void FadeLineOut(LineRenderer _lineRenderer)
	{
		if (_lineRenderer.widthMultiplier > 0.005f)
		{
			_lineRenderer.widthMultiplier = Mathf.Lerp(_lineRenderer.widthMultiplier, 0f, Time.deltaTime * 15f);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400259C RID: 9628
	private List<Transform> targetsList = new List<Transform>();

	// Token: 0x0400259D RID: 9629
	private PlayerAvatar targetPlayer;

	// Token: 0x0400259E RID: 9630
	private bool isActive;

	// Token: 0x0400259F RID: 9631
	public LineRenderer lineRendererLeft;

	// Token: 0x040025A0 RID: 9632
	public LineRenderer lineRendererRight;

	// Token: 0x040025A1 RID: 9633
	public float textureScrollSpeed = 0.5f;
}
