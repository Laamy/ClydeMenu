using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200020B RID: 523
public class MenuSliderPointer : MonoBehaviour
{
	// Token: 0x06001195 RID: 4501 RVA: 0x000A01F9 File Offset: 0x0009E3F9
	private void Start()
	{
		this.rawImage = base.GetComponent<RawImage>();
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x000A0208 File Offset: 0x0009E408
	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(1f, 1f, 1f), 15f * Time.deltaTime);
		this.rawImage.color = Color.Lerp(this.rawImage.color, Color.red, 5f * Time.deltaTime);
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x000A027C File Offset: 0x0009E47C
	public void Tick()
	{
		if (!this.rawImage)
		{
			return;
		}
		base.transform.localScale = new Vector3(1f, 3f, 1f);
		this.rawImage.color = new Color(0.5f, 0.5f, 1f, 1f);
	}

	// Token: 0x04001DCB RID: 7627
	private RawImage rawImage;
}
