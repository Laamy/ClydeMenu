using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000012 RID: 18
public class MixerEffects : MonoBehaviour
{
	// Token: 0x06000043 RID: 67 RVA: 0x0000353F File Offset: 0x0000173F
	private void Start()
	{
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003541 File Offset: 0x00001741
	private void Update()
	{
		this.mixer.SetFloat("Pitch", 1f - Mathf.Clamp(Mathf.Abs(this.camTilt.tiltZresult * this.PitchTiltMultiplier), 0f, this.PitchTiltMax));
	}

	// Token: 0x0400006C RID: 108
	public AudioMixer mixer;

	// Token: 0x0400006D RID: 109
	public CameraTilt camTilt;

	// Token: 0x0400006E RID: 110
	[Space]
	public float DistortionTiltMultiplier = 0.001f;

	// Token: 0x0400006F RID: 111
	public float DistortionTiltMax = 0.2f;

	// Token: 0x04000070 RID: 112
	public float DistortionTiltSpeed = 1f;

	// Token: 0x04000071 RID: 113
	private float DistortionTilt;

	// Token: 0x04000072 RID: 114
	private float DistortionDefault;

	// Token: 0x04000073 RID: 115
	[Space]
	public float LowpassTiltMultiplier = 10f;

	// Token: 0x04000074 RID: 116
	public float LowpassTiltMax = 1000f;

	// Token: 0x04000075 RID: 117
	public float LowpassTiltSpeed = 1f;

	// Token: 0x04000076 RID: 118
	private float LowpassTilt;

	// Token: 0x04000077 RID: 119
	private float LowpassDefault;

	// Token: 0x04000078 RID: 120
	[Space]
	public float PitchTiltMultiplier = 0.001f;

	// Token: 0x04000079 RID: 121
	public float PitchTiltMax = 0.1f;
}
