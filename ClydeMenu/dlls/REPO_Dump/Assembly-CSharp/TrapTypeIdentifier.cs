using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000257 RID: 599
public class TrapTypeIdentifier : MonoBehaviour
{
	// Token: 0x0600134C RID: 4940 RVA: 0x000AC778 File Offset: 0x000AA978
	private void Start()
	{
		Module componentInParent = base.GetComponentInParent<Module>();
		Debug.LogError(string.Concat(new string[]
		{
			"Remove + '",
			this.trapType,
			"' in '",
			componentInParent.gameObject.name,
			"'"
		}));
		TrapDirector.instance.TrapList.Add(base.gameObject);
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x000AC7E0 File Offset: 0x000AA9E0
	[PunRPC]
	private void DestroyTrigger()
	{
		Object.Destroy(this.Trigger);
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x000AC7ED File Offset: 0x000AA9ED
	[PunRPC]
	private void DestroyTrap()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x040020EE RID: 8430
	public string trapType;

	// Token: 0x040020EF RID: 8431
	[Header("Must add the trigger!")]
	public GameObject Trigger;

	// Token: 0x040020F0 RID: 8432
	public bool OnlyRemoveTrigger;

	// Token: 0x040020F1 RID: 8433
	[HideInInspector]
	public bool TriggerRemoved;
}
