using System;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class LobbyObjectScroller : MonoBehaviour
{
	// Token: 0x0600098A RID: 2442 RVA: 0x00058FD6 File Offset: 0x000571D6
	private void Start()
	{
		this.truck = base.GetComponentInParent<TruckLandscapeScroller>();
		if (this.truck != null)
		{
			this.scrollSpeed *= this.truck.truckSpeed;
		}
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0005900C File Offset: 0x0005720C
	private void Update()
	{
		base.transform.position += Vector3.right * this.scrollSpeed * Time.deltaTime;
		if (base.transform.position.x > this.maxDistanceX + this.offsetX)
		{
			base.transform.position = new Vector3(-this.maxDistanceX + this.offsetX, base.transform.position.y, base.transform.position.z);
		}
	}

	// Token: 0x040010F7 RID: 4343
	public float scrollSpeed = 12f;

	// Token: 0x040010F8 RID: 4344
	public float maxDistanceX = 80f;

	// Token: 0x040010F9 RID: 4345
	private float offsetX = -22f;

	// Token: 0x040010FA RID: 4346
	private TruckLandscapeScroller truck;
}
