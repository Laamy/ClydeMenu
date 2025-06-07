using System;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class AudioAttack : MonoBehaviour
{
	// Token: 0x06000017 RID: 23 RVA: 0x000026AC File Offset: 0x000008AC
	private void Start()
	{
		AudioSource component = base.GetComponent<AudioSource>();
		if (Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) < component.maxDistance)
		{
			LevelMusic.instance.Interrupt(10f);
		}
		EnemyDirector.instance.spawnIdlePauseTimer = 0f;
	}
}
