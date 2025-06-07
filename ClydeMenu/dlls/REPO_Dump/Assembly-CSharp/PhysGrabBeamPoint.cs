using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001A5 RID: 421
public class PhysGrabBeamPoint : MonoBehaviour
{
	// Token: 0x06000E50 RID: 3664 RVA: 0x0007FF8B File Offset: 0x0007E18B
	private void Start()
	{
		this.originalScale = base.transform.localScale;
		this.originalMaterial = base.GetComponent<Renderer>().material;
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x0007FFB0 File Offset: 0x0007E1B0
	private void OnEnable()
	{
		if (VideoGreenScreen.instance)
		{
			base.GetComponent<Renderer>().material = this.greenScreenMaterial;
			using (IEnumerator enumerator = base.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					((Transform)obj).GetComponent<Renderer>().material = this.greenScreenMaterial;
				}
				return;
			}
		}
		base.GetComponent<Renderer>().material = this.originalMaterial;
		foreach (object obj2 in base.transform)
		{
			((Transform)obj2).GetComponent<Renderer>().material = this.originalMaterial;
		}
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x00080090 File Offset: 0x0007E290
	private void Update()
	{
		float num = Time.time * this.tileSpeedX;
		float num2 = Time.time * this.tileSpeedY;
		base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(num, num2);
		float num3 = Mathf.Sin(Time.time * this.textureJitterSpeed) * 0.1f;
		base.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f + num3, 1f + num3);
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-num, -num2);
			transform.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f - num3, 1f - num3);
		}
		float num4 = Mathf.Sin(Time.time * this.sphereJitterSpeed) * (this.originalScale.x * 0.3f);
		base.transform.localScale = (this.originalScale + new Vector3(num4, num4, num4)) * 0.5f;
	}

	// Token: 0x0400178D RID: 6029
	public float tileSpeedX = 0.5f;

	// Token: 0x0400178E RID: 6030
	public float tileSpeedY = 0.5f;

	// Token: 0x0400178F RID: 6031
	public float textureJitterSpeed = 10f;

	// Token: 0x04001790 RID: 6032
	public float sphereJitterSpeed = 10f;

	// Token: 0x04001791 RID: 6033
	private Vector3 originalScale;

	// Token: 0x04001792 RID: 6034
	public Material originalMaterial;

	// Token: 0x04001793 RID: 6035
	public Material greenScreenMaterial;
}
