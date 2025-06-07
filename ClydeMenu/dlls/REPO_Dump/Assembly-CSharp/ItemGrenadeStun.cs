using System;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class ItemGrenadeStun : MonoBehaviour
{
	// Token: 0x06000B95 RID: 2965 RVA: 0x00066C50 File Offset: 0x00064E50
	private void Start()
	{
		this.stunExplosion = base.GetComponentInChildren<StunExplosion>().transform;
		this.stunExplosion.gameObject.SetActive(false);
		this.itemGrenade = base.GetComponent<ItemGrenade>();
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x00066C80 File Offset: 0x00064E80
	public void Explosion()
	{
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundTinnitus.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameObject gameObject = Object.Instantiate<GameObject>(this.stunExplosion.gameObject, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = null;
		gameObject.SetActive(true);
		gameObject.GetComponent<StunExplosion>().itemGrenade = this.itemGrenade;
	}

	// Token: 0x040012C3 RID: 4803
	public Sound soundExplosion;

	// Token: 0x040012C4 RID: 4804
	public Sound soundTinnitus;

	// Token: 0x040012C5 RID: 4805
	private Transform stunExplosion;

	// Token: 0x040012C6 RID: 4806
	private ItemGrenade itemGrenade;
}
