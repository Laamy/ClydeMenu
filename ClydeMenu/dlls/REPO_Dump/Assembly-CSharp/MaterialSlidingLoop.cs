using System;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class MaterialSlidingLoop : MonoBehaviour
{
	// Token: 0x060008EC RID: 2284 RVA: 0x0005606D File Offset: 0x0005426D
	private void Start()
	{
		this.lowPassLogic = base.GetComponent<AudioLowPassLogic>();
		this.source = base.GetComponent<AudioSource>();
		this.activeTimer = 1f;
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00056094 File Offset: 0x00054294
	private void Update()
	{
		this.material.SlideLoop.Source = this.source;
		if (this.getMaterialTimer > 0f)
		{
			this.getMaterialTimer -= Time.deltaTime;
		}
		if (this.activeTimer > 0f)
		{
			this.activeTimer -= Time.deltaTime;
			this.material.SlideLoop.PlayLoop(true, 5f, 5f, this.pitchMultiplier);
			return;
		}
		this.lowPassLogic.Volume -= 5f * Time.deltaTime;
		if (this.lowPassLogic.Volume <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04001057 RID: 4183
	private AudioSource source;

	// Token: 0x04001058 RID: 4184
	public float activeTimer;

	// Token: 0x04001059 RID: 4185
	public MaterialPreset material;

	// Token: 0x0400105A RID: 4186
	public float pitchMultiplier;

	// Token: 0x0400105B RID: 4187
	public float getMaterialTimer;

	// Token: 0x0400105C RID: 4188
	private AudioLowPassLogic lowPassLogic;
}
