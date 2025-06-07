using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class CameraMainMenu : CameraNoPlayerTarget
{
	// Token: 0x0600009A RID: 154 RVA: 0x0000633D File Offset: 0x0000453D
	protected override void Awake()
	{
		base.Awake();
		CameraNoise.Instance.AnimNoise.noiseStrengthDefault = 0.3f;
		CameraNoise.Instance.AnimNoise.noiseSpeedDefault = 4f;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00006370 File Offset: 0x00004570
	protected override void Update()
	{
		base.Update();
		if (GameDirector.instance.currentState == GameDirector.gameState.Main && this.introLerp < 1f)
		{
			this.introLerp += 0.25f * Time.deltaTime;
			base.transform.localEulerAngles = new Vector3(Mathf.LerpUnclamped(0f, -45f, this.introCurve.Evaluate(this.introLerp)), 0f, 0f);
		}
	}

	// Token: 0x0400018B RID: 395
	public AnimationCurve introCurve;

	// Token: 0x0400018C RID: 396
	private float introLerp;
}
