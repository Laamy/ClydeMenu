using System;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class TextureScroller : MonoBehaviour
{
	// Token: 0x0600098D RID: 2445 RVA: 0x000590D0 File Offset: 0x000572D0
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		this.savedOffset = this.rend.material.mainTextureOffset;
		this.truck = base.GetComponentInParent<TruckLandscapeScroller>();
		if (this.truck != null)
		{
			this.scrollSpeed *= this.truck.truckSpeed;
		}
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x00059134 File Offset: 0x00057334
	private void Update()
	{
		float x = Mathf.Repeat(Time.time * this.scrollSpeed, 1f);
		Vector2 mainTextureOffset = new Vector2(x, this.savedOffset.y);
		this.rend.material.mainTextureOffset = mainTextureOffset;
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x0005917C File Offset: 0x0005737C
	private void OnDisable()
	{
		this.rend.material.mainTextureOffset = this.savedOffset;
	}

	// Token: 0x040010FB RID: 4347
	public float scrollSpeed = 0.5f;

	// Token: 0x040010FC RID: 4348
	private Renderer rend;

	// Token: 0x040010FD RID: 4349
	private Vector2 savedOffset;

	// Token: 0x040010FE RID: 4350
	private TruckLandscapeScroller truck;
}
