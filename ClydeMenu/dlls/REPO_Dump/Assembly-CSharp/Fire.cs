using System;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class Fire : MonoBehaviour
{
	// Token: 0x06001086 RID: 4230 RVA: 0x00098DC5 File Offset: 0x00096FC5
	private void Update()
	{
		if (this.propLight.turnedOff)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x00098DDF File Offset: 0x00096FDF
	public void OnHit()
	{
		this.soundHit.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04001C58 RID: 7256
	public PropLight propLight;

	// Token: 0x04001C59 RID: 7257
	[Space]
	public Sound soundHit;
}
