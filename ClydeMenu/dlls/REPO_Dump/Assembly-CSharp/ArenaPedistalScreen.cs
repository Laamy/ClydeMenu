using System;
using TMPro;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class ArenaPedistalScreen : MonoBehaviour
{
	// Token: 0x06000791 RID: 1937 RVA: 0x0004823C File Offset: 0x0004643C
	private void Update()
	{
		if (this.glitchTimer > 0f)
		{
			this.glitchTimer -= Time.deltaTime;
			float x = Mathf.Sin(Time.time * 100f) * 0.1f;
			float y = Mathf.Sin(Time.time * 100f) * 0.1f;
			this.glitchMeshRenderer.material.mainTextureOffset = new Vector2(x, y);
			return;
		}
		this.glitchObject.SetActive(false);
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x000482BC File Offset: 0x000464BC
	public void SwitchNumber(int number, bool finalPlayer = false)
	{
		if (!this.glitchMeshRenderer)
		{
			return;
		}
		this.screenText.text = number.ToString();
		float x = Random.Range(0f, 100f);
		float y = Random.Range(0f, 100f);
		this.glitchMeshRenderer.material.mainTextureOffset = new Vector2(x, y);
		this.glitchObject.SetActive(true);
		if (finalPlayer)
		{
			this.screenText.color = Color.green;
			this.numberLight.color = Color.green;
			Color color = new Color(0f, 1f, 0f, 0.65f);
			this.screenScanLines.color = color;
		}
		this.glitchTimer = 0.2f;
	}

	// Token: 0x04000D5F RID: 3423
	public TextMeshPro screenText;

	// Token: 0x04000D60 RID: 3424
	public GameObject glitchObject;

	// Token: 0x04000D61 RID: 3425
	public MeshRenderer glitchMeshRenderer;

	// Token: 0x04000D62 RID: 3426
	private float glitchTimer;

	// Token: 0x04000D63 RID: 3427
	public Light numberLight;

	// Token: 0x04000D64 RID: 3428
	public SpriteRenderer screenScanLines;
}
