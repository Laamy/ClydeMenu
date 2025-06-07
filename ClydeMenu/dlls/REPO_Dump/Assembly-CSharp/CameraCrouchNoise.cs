using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class CameraCrouchNoise : MonoBehaviour
{
	// Token: 0x060000C3 RID: 195 RVA: 0x00007843 File Offset: 0x00005A43
	private void Start()
	{
		this.Player = PlayerController.instance;
		this.AnimNoise.MasterAmount = 0f;
		this.AnimNoise.enabled = false;
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x0000786C File Offset: 0x00005A6C
	private void Update()
	{
		if (this.Player.Crouching && !RecordingDirector.instance)
		{
			this.AnimNoise.enabled = true;
			this.AnimNoise.MasterAmount = Mathf.Lerp(this.AnimNoise.MasterAmount, this.Strength * GameplayManager.instance.cameraNoise, Time.deltaTime * this.LerpSpeed);
			return;
		}
		if (this.AnimNoise.enabled)
		{
			this.AnimNoise.MasterAmount = Mathf.Lerp(this.AnimNoise.MasterAmount, 0f, Time.deltaTime * this.LerpSpeed);
			if (this.AnimNoise.MasterAmount < 0.001f)
			{
				this.AnimNoise.enabled = false;
			}
		}
	}

	// Token: 0x04000201 RID: 513
	private PlayerController Player;

	// Token: 0x04000202 RID: 514
	public AnimNoise AnimNoise;

	// Token: 0x04000203 RID: 515
	public float Strength = 1f;

	// Token: 0x04000204 RID: 516
	public float LerpSpeed = 2f;
}
