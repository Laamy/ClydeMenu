using System;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class CleanSpotIdentifier : MonoBehaviour
{
	// Token: 0x060006D8 RID: 1752 RVA: 0x00041B71 File Offset: 0x0003FD71
	private void Start()
	{
		CleanDirector.instance.CleanList.Add(base.gameObject);
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00041B88 File Offset: 0x0003FD88
	private void Update()
	{
	}

	// Token: 0x04000B9C RID: 2972
	public Interaction.InteractionType InteractionType;
}
