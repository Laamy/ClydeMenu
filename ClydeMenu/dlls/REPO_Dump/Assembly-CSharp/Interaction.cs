using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class Interaction : MonoBehaviour
{
	// Token: 0x060006ED RID: 1773 RVA: 0x0004215A File Offset: 0x0004035A
	private void Start()
	{
		base.StartCoroutine(this.Add());
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00042169 File Offset: 0x00040369
	private IEnumerator Add()
	{
		while (!CleanDirector.instance.RemoveExcessSpots)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x04000BBF RID: 3007
	public Interaction.InteractionType Type;

	// Token: 0x04000BC0 RID: 3008
	public Sprite Sprite;

	// Token: 0x02000333 RID: 819
	public enum InteractionType
	{
		// Token: 0x0400299E RID: 10654
		None,
		// Token: 0x0400299F RID: 10655
		VacuumCleaner,
		// Token: 0x040029A0 RID: 10656
		Duster,
		// Token: 0x040029A1 RID: 10657
		Sledgehammer,
		// Token: 0x040029A2 RID: 10658
		DirtFinder,
		// Token: 0x040029A3 RID: 10659
		Picker
	}
}
