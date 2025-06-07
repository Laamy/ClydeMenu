using System;

// Token: 0x02000027 RID: 39
public class CameraLobbyMenu : CameraNoPlayerTarget
{
	// Token: 0x06000098 RID: 152 RVA: 0x00006305 File Offset: 0x00004505
	protected override void Awake()
	{
		base.Awake();
		CameraNoise.Instance.AnimNoise.noiseStrengthDefault = 0.3f;
		CameraNoise.Instance.AnimNoise.noiseSpeedDefault = 4f;
	}
}
