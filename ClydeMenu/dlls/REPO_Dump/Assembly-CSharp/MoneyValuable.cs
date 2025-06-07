using System;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class MoneyValuable : MonoBehaviour
{
	// Token: 0x06001559 RID: 5465 RVA: 0x000BC511 File Offset: 0x000BA711
	public void MoneyBurst()
	{
		this.moneyBurst.Play();
	}

	// Token: 0x040024EC RID: 9452
	public ParticleSystem moneyBurst;
}
