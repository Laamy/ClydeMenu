using System;
using UnityEngine;

// Token: 0x020002A9 RID: 681
public class FleshBlobValuable : MonoBehaviour
{
	// Token: 0x06001550 RID: 5456 RVA: 0x000BBF58 File Offset: 0x000BA158
	public void DieHatFly()
	{
		this.capParticleTransform.parent = null;
		this.capParticle.Play();
	}

	// Token: 0x040024D6 RID: 9430
	public ParticleSystem capParticle;

	// Token: 0x040024D7 RID: 9431
	public Transform capParticleTransform;
}
