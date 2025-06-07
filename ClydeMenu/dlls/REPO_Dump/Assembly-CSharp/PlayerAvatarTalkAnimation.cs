using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class PlayerAvatarTalkAnimation : MonoBehaviour
{
	// Token: 0x06000F5E RID: 3934 RVA: 0x0008A43D File Offset: 0x0008863D
	private void Start()
	{
		this.playerAvatarVisuals = base.GetComponent<PlayerAvatarVisuals>();
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x0008A44C File Offset: 0x0008864C
	private void Update()
	{
		if (this.playerAvatarVisuals.isMenuAvatar && !this.playerAvatar)
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (!GameManager.Multiplayer())
		{
			return;
		}
		if (!this.playerAvatar.voiceChatFetched)
		{
			return;
		}
		if (!this.audioSourceFetched)
		{
			this.audioSource = this.playerAvatar.voiceChat.audioSource;
			this.audioSourceFetched = true;
		}
		if (!this.audioSource)
		{
			return;
		}
		float x = 0f;
		if (this.playerAvatar.voiceChat.clipLoudness > 0.005f)
		{
			x = Mathf.Lerp(0f, -this.rotationMaxAngle, this.playerAvatar.voiceChat.clipLoudness * 4f);
		}
		this.objectToRotate.transform.localRotation = Quaternion.Slerp(this.objectToRotate.transform.localRotation, Quaternion.Euler(x, 0f, 0f), 100f * Time.deltaTime);
	}

	// Token: 0x04001963 RID: 6499
	public AudioSource audioSource;

	// Token: 0x04001964 RID: 6500
	public PlayerAvatar playerAvatar;

	// Token: 0x04001965 RID: 6501
	public GameObject objectToRotate;

	// Token: 0x04001966 RID: 6502
	private PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x04001967 RID: 6503
	[Space]
	public float threshold = 0.01f;

	// Token: 0x04001968 RID: 6504
	public float rotationMaxAngle = 45f;

	// Token: 0x04001969 RID: 6505
	public float amountMultiplier = 1f;

	// Token: 0x0400196A RID: 6506
	private bool audioSourceFetched;
}
