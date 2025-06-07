using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200023A RID: 570
public class CleanDirector : MonoBehaviour
{
	// Token: 0x0600129F RID: 4767 RVA: 0x000A746D File Offset: 0x000A566D
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		CleanDirector.instance = this;
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x000A7484 File Offset: 0x000A5684
	private void RandomlyRemoveExcessSpots()
	{
		if (PhotonNetwork.IsMasterClient || GameManager.instance.gameMode == 0)
		{
			foreach (CleanDirector.CleaningSpots cleaningSpots in this.cleaningSpots)
			{
				Interaction.InteractionType type = cleaningSpots.InteractionType;
				int cleaningSpotsMax = cleaningSpots.CleaningSpotsMax;
				Func<GameObject, bool> <>9__1;
				for (int i = Enumerable.Count<GameObject>(this.CleanList, (GameObject spot) => spot.GetComponent<CleanSpotIdentifier>().InteractionType == type); i > cleaningSpotsMax; i--)
				{
					IEnumerable<GameObject> cleanList = this.CleanList;
					Func<GameObject, bool> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = ((GameObject spot) => spot.GetComponent<CleanSpotIdentifier>().InteractionType == type));
					}
					List<GameObject> list = Enumerable.ToList<GameObject>(Enumerable.Where<GameObject>(cleanList, func));
					int num = Random.Range(0, list.Count);
					GameObject gameObject = list[num];
					if (GameManager.instance.gameMode == 1)
					{
						if (gameObject.GetComponent<PhotonView>() == null)
						{
							Debug.LogWarning("Photon View not found for: " + gameObject.name);
						}
						this.CleanList.Remove(gameObject);
						PhotonNetwork.Destroy(gameObject);
					}
					else
					{
						this.CleanList.Remove(gameObject);
						Object.Destroy(gameObject);
					}
				}
			}
		}
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x000A75E8 File Offset: 0x000A57E8
	private void Update()
	{
		if (!this.RemoveExcessSpots && GameDirector.instance.currentState >= GameDirector.gameState.Start)
		{
			this.RandomlyRemoveExcessSpots();
			this.RemoveExcessSpots = true;
		}
	}

	// Token: 0x04001FBC RID: 8124
	public static CleanDirector instance;

	// Token: 0x04001FBD RID: 8125
	public List<GameObject> CleanList = new List<GameObject>();

	// Token: 0x04001FBE RID: 8126
	public List<CleanDirector.CleaningSpots> cleaningSpots;

	// Token: 0x04001FBF RID: 8127
	internal bool RemoveExcessSpots;

	// Token: 0x04001FC0 RID: 8128
	private PhotonView photonView;

	// Token: 0x020003F3 RID: 1011
	[Serializable]
	public class CleaningSpots
	{
		// Token: 0x04002CFE RID: 11518
		public Interaction.InteractionType InteractionType;

		// Token: 0x04002CFF RID: 11519
		public int CleaningSpotsMax;
	}
}
