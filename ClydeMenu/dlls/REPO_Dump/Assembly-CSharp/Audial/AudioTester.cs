using System;
using UnityEngine;

namespace Audial
{
	// Token: 0x020002ED RID: 749
	public class AudioTester : MonoBehaviour
	{
		// Token: 0x0600178D RID: 6029 RVA: 0x000C95F8 File Offset: 0x000C77F8
		public void ClearBuffer()
		{
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x000C95FA File Offset: 0x000C77FA
		public void SetRunEffectInEditMode()
		{
		}

		// Token: 0x1700004C RID: 76
		// (set) Token: 0x0600178F RID: 6031 RVA: 0x000C95FC File Offset: 0x000C77FC
		public bool playAudio
		{
			set
			{
				base.gameObject.SendMessage("ClearBuffer");
				base.gameObject.SendMessage("ResetUtils", SendMessageOptions.DontRequireReceiver);
				if (this.hasAudioSource && this.audioSource.clip != null)
				{
					this.audioSource.Play();
				}
			}
		}

		// Token: 0x1700004D RID: 77
		// (set) Token: 0x06001790 RID: 6032 RVA: 0x000C9650 File Offset: 0x000C7850
		public bool stopAudio
		{
			set
			{
				base.gameObject.SendMessage("ClearBuffer");
				if (this.hasAudioSource)
				{
					this.audioSource.Stop();
				}
			}
		}

		// Token: 0x040027BA RID: 10170
		[HideInInspector]
		public bool hasAudioSource = true;

		// Token: 0x040027BB RID: 10171
		[HideInInspector]
		public AudioSource audioSource;
	}
}
