using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000291 RID: 657
public class RewindEffect : MonoBehaviour
{
	// Token: 0x060014AD RID: 5293 RVA: 0x000B69B2 File Offset: 0x000B4BB2
	private void Awake()
	{
		RewindEffect.Instance = this;
	}

	// Token: 0x060014AE RID: 5294 RVA: 0x000B69BA File Offset: 0x000B4BBA
	private void Start()
	{
		this.RewindEffectUI.SetActive(false);
		this.RewindLines.SetActive(false);
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x000B69D4 File Offset: 0x000B4BD4
	private void Update()
	{
		if (this.PlayRewind)
		{
			GameDirector.instance.SetDisableInput(0.5f);
			VideoOverlay.Instance.Override(0.1f, 1f, 5f);
		}
		this.RewindLoopSound.PlayLoop(this.PlayRewind, 0.9f, 0.9f, 1f);
		if (!this.PlayRewind && !this.RewindEnd && Vector3.Distance(this.PlayerTransfrom.position, this.lastScreenshotPosition) > this.movementThreshold)
		{
			if (!this.FirstStep)
			{
				this.CaptureScreenshot();
				this.lastScreenshotPosition = this.PlayerTransfrom.position;
				return;
			}
			this.ClearScreenshots();
			this.FirstStep = false;
		}
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x000B6A8C File Offset: 0x000B4C8C
	private void CaptureScreenshot()
	{
		if (this.screenshots.Count >= this.maxScreenshots)
		{
			Object.Destroy(this.screenshots[0]);
			this.screenshots.RemoveAt(0);
			this.TimeSnapshots.RemoveAt(0);
		}
		Texture texture = this.RenderTextureMain.texture;
		RenderTexture renderTexture = new RenderTexture(128, 72, 24);
		renderTexture.Create();
		Graphics.Blit(texture, renderTexture);
		RenderTexture.active = renderTexture;
		Texture2D texture2D = new Texture2D(128, 72, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(0f, 0f, 128f, 72f), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		this.screenshots.Add(texture2D);
		this.TimeSnapshots.Add(this.Timecode.GetSnapshot());
		Object.Destroy(renderTexture);
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x000B6B68 File Offset: 0x000B4D68
	public void PlayRewindEffect()
	{
		if (this.screenshots.Count >= 10)
		{
			this.RewindStartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.PlayRewind = true;
			this.RewindLines.SetActive(true);
			this.RewindEffectUI.SetActive(true);
			base.StartCoroutine(this.RewindCoroutine());
			return;
		}
		this.RewindEnding();
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x000B6BE2 File Offset: 0x000B4DE2
	private IEnumerator RewindCoroutine()
	{
		Image rewindImage = this.RewindEffectUI.GetComponent<Image>();
		rewindImage.enabled = true;
		float displayTimePerScreenshot = this.rewindDuration / (float)this.screenshots.Count;
		int num;
		for (int i = this.screenshots.Count - 1; i >= 0; i = num - 1)
		{
			this.Timecode.SetTime(this.TimeSnapshots[i]);
			Texture2D screenshot = this.screenshots[i];
			Sprite sprite = Sprite.Create(screenshot, new Rect(0f, 0f, (float)screenshot.width, (float)screenshot.height), new Vector2(0.5f, 0.5f));
			rewindImage.sprite = sprite;
			yield return new WaitForSeconds(displayTimePerScreenshot);
			Object.Destroy(sprite);
			Object.Destroy(screenshot);
			screenshot = null;
			sprite = null;
			num = i;
		}
		this.RewindEnding();
		yield break;
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x000B6BF4 File Offset: 0x000B4DF4
	public void ClearScreenshots()
	{
		foreach (Texture2D obj in this.screenshots)
		{
			Object.Destroy(obj);
		}
		this.screenshots.Clear();
		this.lastScreenshotPosition = this.PlayerTransfrom.position;
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x000B6C60 File Offset: 0x000B4E60
	public void RewindEnding()
	{
		this.Timecode.SetToStartSnapshot();
		this.RewindEndSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.PlayRewind = false;
		this.ClearScreenshots();
		this.RewindLines.SetActive(false);
		this.lastScreenshotPosition = this.PlayerTransfrom.position;
		this.FirstStep = true;
	}

	// Token: 0x0400237C RID: 9084
	public static RewindEffect Instance;

	// Token: 0x0400237D RID: 9085
	public Timecode Timecode;

	// Token: 0x0400237E RID: 9086
	[HideInInspector]
	public List<Timecode.TimeSnapshot> TimeSnapshots;

	// Token: 0x0400237F RID: 9087
	[Space]
	public int maxScreenshots = 50;

	// Token: 0x04002380 RID: 9088
	public float movementThreshold = 3f;

	// Token: 0x04002381 RID: 9089
	public Transform PlayerTransfrom;

	// Token: 0x04002382 RID: 9090
	private List<Texture2D> screenshots = new List<Texture2D>();

	// Token: 0x04002383 RID: 9091
	public GameObject RewindEffectUI;

	// Token: 0x04002384 RID: 9092
	private Vector3 lastScreenshotPosition;

	// Token: 0x04002385 RID: 9093
	[HideInInspector]
	public bool PlayRewind;

	// Token: 0x04002386 RID: 9094
	public float rewindDuration = 1.5f;

	// Token: 0x04002387 RID: 9095
	private bool RewindEnd;

	// Token: 0x04002388 RID: 9096
	private bool FirstStep = true;

	// Token: 0x04002389 RID: 9097
	public GameObject RewindLines;

	// Token: 0x0400238A RID: 9098
	public RawImage RenderTextureMain;

	// Token: 0x0400238B RID: 9099
	public Sound RewindStartSound;

	// Token: 0x0400238C RID: 9100
	public Sound RewindEndSound;

	// Token: 0x0400238D RID: 9101
	public Sound RewindLoopSound;
}
