using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class ItemGrenadeHuman : MonoBehaviour
{
	// Token: 0x06000B8B RID: 2955 RVA: 0x00066A71 File Offset: 0x00064C71
	private void Start()
	{
		this.Initialize();
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x00066A7C File Offset: 0x00064C7C
	public void Initialize()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemGrenade = base.GetComponent<ItemGrenade>();
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00066AD1 File Offset: 0x00064CD1
	public void Spawn()
	{
		base.StartCoroutine(this.LateSpawn());
		this.itemGrenade.isSpawnedGrenade = true;
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x00066AEC File Offset: 0x00064CEC
	private IEnumerator LateSpawn()
	{
		while (!this.physGrabObject.spawned || this.rb.isKinematic)
		{
			yield return null;
		}
		this.itemToggle.ToggleItem(true, -1);
		this.itemGrenade.tickTime = Random.Range(1.5f, 3f);
		Vector3 a = Quaternion.Euler((float)Random.Range(-45, 45), (float)Random.Range(-180, 180), 0f) * Vector3.forward;
		this.rb.AddForce(a * (float)Random.Range(5, 10), ForceMode.Impulse);
		this.rb.AddTorque(Random.insideUnitSphere * Random.Range(5f, 10f), ForceMode.Impulse);
		this.itemGrenade.isSpawnedGrenade = true;
		yield break;
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x00066AFC File Offset: 0x00064CFC
	public void Explosion()
	{
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.particleScriptExplosion.Spawn(base.transform.position, 0.8f, 50, 100, 2f, false, true, 1f);
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.soundExplosionGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x040012BA RID: 4794
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040012BB RID: 4795
	private ItemToggle itemToggle;

	// Token: 0x040012BC RID: 4796
	private ItemGrenade itemGrenade;

	// Token: 0x040012BD RID: 4797
	private PhotonView photonView;

	// Token: 0x040012BE RID: 4798
	private PhysGrabObject physGrabObject;

	// Token: 0x040012BF RID: 4799
	private Rigidbody rb;

	// Token: 0x040012C0 RID: 4800
	public Sound soundExplosion;

	// Token: 0x040012C1 RID: 4801
	public Sound soundExplosionGlobal;
}
