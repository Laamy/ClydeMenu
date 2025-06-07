using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class ItemGrenadeExplosive : MonoBehaviour
{
	// Token: 0x06000B91 RID: 2961 RVA: 0x00066BA0 File Offset: 0x00064DA0
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		if (SemiFunc.RunIsShop() && SemiFunc.IsMasterClientOrSingleplayer())
		{
			ItemToggle component = base.GetComponent<ItemToggle>();
			if (ShopManager.instance.isThief)
			{
				base.StartCoroutine(this.ThiefLaunch());
				component.ToggleItem(true, -1);
				base.GetComponent<ItemGrenade>().isSpawnedGrenade = true;
			}
		}
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x00066BFB File Offset: 0x00064DFB
	private IEnumerator ThiefLaunch()
	{
		yield return new WaitForSeconds(0.2f);
		Rigidbody component = base.GetComponent<Rigidbody>();
		Vector3 a = ShopManager.instance.extractionPoint.forward;
		a += Vector3.up * Random.Range(0.1f, 0.5f);
		a += ShopManager.instance.extractionPoint.right * Random.Range(-0.5f, 0.5f);
		component.AddForce(a * (float)Random.Range(3, 7), ForceMode.Impulse);
		yield break;
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x00066C0C File Offset: 0x00064E0C
	public void Explosion()
	{
		this.particleScriptExplosion.Spawn(base.transform.position, 1.2f, 75, 160, 4f, false, false, 1f);
	}

	// Token: 0x040012C2 RID: 4802
	private ParticleScriptExplosion particleScriptExplosion;
}
