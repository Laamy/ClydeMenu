using System;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class ItemMineExplosive : MonoBehaviour
{
	// Token: 0x06000C1E RID: 3102 RVA: 0x0006B02F File Offset: 0x0006922F
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x0006B040 File Offset: 0x00069240
	public void OnTriggered()
	{
		this.particleScriptExplosion.Spawn(base.transform.position, 1.2f, 75, 200, 4f, false, false, 1f);
	}

	// Token: 0x040013A9 RID: 5033
	private ParticleScriptExplosion particleScriptExplosion;
}
