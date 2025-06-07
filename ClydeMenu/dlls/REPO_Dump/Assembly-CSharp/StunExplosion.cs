using System;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class StunExplosion : MonoBehaviour
{
	// Token: 0x06000BAF RID: 2991 RVA: 0x00067CFE File Offset: 0x00065EFE
	private void Start()
	{
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>();
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x00067D0C File Offset: 0x00065F0C
	public void StunExplosionReset()
	{
		this.removeTimer = 0f;
		this.lightEval = 0f;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00067D30 File Offset: 0x00065F30
	private void Update()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (this.light)
		{
			if (this.lightEval < 1f)
			{
				this.light.intensity = 10f * this.lightCurve.Evaluate(this.lightEval);
				this.lightEval += 0.2f * Time.deltaTime;
			}
			else
			{
				this.light.intensity = 0f;
			}
		}
		if (this.removeTimer > 0.5f)
		{
			this.hurtCollider.gameObject.SetActive(false);
		}
		else
		{
			this.hurtCollider.gameObject.SetActive(true);
		}
		this.removeTimer += Time.deltaTime;
		if (this.removeTimer >= 20f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04001300 RID: 4864
	public Light light;

	// Token: 0x04001301 RID: 4865
	public AnimationCurve lightCurve;

	// Token: 0x04001302 RID: 4866
	private float lightEval;

	// Token: 0x04001303 RID: 4867
	private float removeTimer;

	// Token: 0x04001304 RID: 4868
	private HurtCollider hurtCollider;

	// Token: 0x04001305 RID: 4869
	public ItemGrenade itemGrenade;
}
