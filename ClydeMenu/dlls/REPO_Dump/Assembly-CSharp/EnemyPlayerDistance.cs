using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class EnemyPlayerDistance : MonoBehaviour
{
	// Token: 0x06000682 RID: 1666 RVA: 0x0003E96D File Offset: 0x0003CB6D
	private void Start()
	{
		this.Enemy = base.GetComponent<Enemy>();
		this.LogicActive = true;
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0003E98F File Offset: 0x0003CB8F
	private void OnDisable()
	{
		this.LogicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0003E99E File Offset: 0x0003CB9E
	private void OnEnable()
	{
		if (!this.LogicActive)
		{
			this.LogicActive = true;
			base.StartCoroutine(this.Logic());
		}
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x0003E9BC File Offset: 0x0003CBBC
	private IEnumerator Logic()
	{
		for (;;)
		{
			this.PlayerDistanceLocal = 999f;
			this.PlayerDistanceClosest = 999f;
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				float num = Vector3.Distance(this.CheckTransform.position, playerAvatar.PlayerVisionTarget.VisionTransform.position);
				if (playerAvatar.isLocal)
				{
					this.PlayerDistanceLocal = num;
				}
				if (!playerAvatar.isDisabled && num < this.PlayerDistanceClosest)
				{
					this.PlayerDistanceClosest = num;
				}
			}
			yield return new WaitForSeconds(0.25f);
		}
		yield break;
	}

	// Token: 0x04000AB1 RID: 2737
	private Enemy Enemy;

	// Token: 0x04000AB2 RID: 2738
	public Transform CheckTransform;

	// Token: 0x04000AB3 RID: 2739
	private bool LogicActive;

	// Token: 0x04000AB4 RID: 2740
	internal float PlayerDistanceLocal = 1000f;

	// Token: 0x04000AB5 RID: 2741
	internal float PlayerDistanceClosest = 1000f;
}
