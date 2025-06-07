using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class ItemGrenadeDuctTaped : MonoBehaviour
{
	// Token: 0x06000B88 RID: 2952 RVA: 0x000668FE File Offset: 0x00064AFE
	private void Start()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x00066918 File Offset: 0x00064B18
	public void Explosion()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			for (int i = 0; i < 3; i++)
			{
				Vector3 b = new Vector3(0f, 0.2f * (float)i, 0f);
				ItemGrenadeHuman component = Object.Instantiate<GameObject>(this.grenadePrefab, base.transform.position + b, Quaternion.identity).GetComponent<ItemGrenadeHuman>();
				component.Initialize();
				component.Spawn();
			}
		}
		else if (SemiFunc.IsMasterClient())
		{
			for (int j = 0; j < 3; j++)
			{
				Vector3 b2 = new Vector3(0f, 0.2f * (float)j, 0f);
				GameObject gameObject = PhotonNetwork.Instantiate("Items/Item Grenade Human", base.transform.position + b2, Quaternion.identity, 0, null);
				gameObject.GetComponent<ItemGrenadeHuman>().Initialize();
				gameObject.GetComponent<ItemGrenadeHuman>().Spawn();
			}
		}
		this.particleScriptExplosion.Spawn(base.transform.position, 0.8f, 50, 100, 4f, false, true, 1f);
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundExplosionGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040012B5 RID: 4789
	public GameObject grenadePrefab;

	// Token: 0x040012B6 RID: 4790
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040012B7 RID: 4791
	private PhotonView photonView;

	// Token: 0x040012B8 RID: 4792
	public Sound soundExplosion;

	// Token: 0x040012B9 RID: 4793
	public Sound soundExplosionGlobal;
}
