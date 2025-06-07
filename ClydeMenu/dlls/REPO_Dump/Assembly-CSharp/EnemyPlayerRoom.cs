using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class EnemyPlayerRoom : MonoBehaviour
{
	// Token: 0x06000687 RID: 1671 RVA: 0x0003E9E9 File Offset: 0x0003CBE9
	private void Start()
	{
		this.LogicActive = true;
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x0003E9FF File Offset: 0x0003CBFF
	private void OnEnable()
	{
		if (!this.LogicActive)
		{
			this.LogicActive = true;
			base.StartCoroutine(this.Logic());
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0003EA1D File Offset: 0x0003CC1D
	private void OnDisable()
	{
		this.LogicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0003EA2C File Offset: 0x0003CC2C
	private IEnumerator Logic()
	{
		while (GameDirector.instance.PlayerList.Count == 0)
		{
			yield return new WaitForSeconds(1f);
		}
		for (;;)
		{
			this.SameAny = false;
			this.SameLocal = false;
			foreach (RoomVolume x in this.RoomVolumeCheck.CurrentRooms)
			{
				foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
				{
					foreach (RoomVolume y in playerAvatar.RoomVolumeCheck.CurrentRooms)
					{
						if (x == y)
						{
							if (!playerAvatar.isDisabled)
							{
								this.SameAny = true;
							}
							if (playerAvatar.isLocal)
							{
								this.SameLocal = true;
								break;
							}
						}
					}
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x04000AB6 RID: 2742
	public RoomVolumeCheck RoomVolumeCheck;

	// Token: 0x04000AB7 RID: 2743
	private bool LogicActive;

	// Token: 0x04000AB8 RID: 2744
	internal bool SameAny;

	// Token: 0x04000AB9 RID: 2745
	internal bool SameLocal;
}
