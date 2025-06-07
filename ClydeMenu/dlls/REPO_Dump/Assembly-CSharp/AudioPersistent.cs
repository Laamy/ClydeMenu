using System;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class AudioPersistent : MonoBehaviour
{
	// Token: 0x06000023 RID: 35 RVA: 0x000029E9 File Offset: 0x00000BE9
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		base.transform.parent = null;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002A0E File Offset: 0x00000C0E
	private void Update()
	{
		if (!this.audioSource.isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04000042 RID: 66
	private AudioSource audioSource;
}
