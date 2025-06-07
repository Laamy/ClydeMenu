using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000259 RID: 601
public class TrapPropSwitch : MonoBehaviour
{
	// Token: 0x06001357 RID: 4951 RVA: 0x000ACAB5 File Offset: 0x000AACB5
	private void Start()
	{
		this.TrapParent.SetActive(true);
		this.PropParent.SetActive(true);
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x000ACADC File Offset: 0x000AACDC
	public IEnumerator Setup()
	{
		while (!TrapDirector.instance.TrapListUpdated)
		{
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		Trap componentInChildren = base.GetComponentInChildren<Trap>();
		if (componentInChildren != null && componentInChildren.gameObject.activeSelf)
		{
			this.PropParent.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x000ACAEC File Offset: 0x000AACEC
	public void DebugToggle()
	{
		if (this.DebugSwitch)
		{
			this.DebugSwitch = false;
			this.DebugState = "Trap Active";
			if (this.TrapParent != null)
			{
				this.TrapParent.SetActive(true);
			}
			if (this.PropParent != null)
			{
				this.PropParent.SetActive(false);
				return;
			}
		}
		else
		{
			this.DebugSwitch = true;
			this.DebugState = "Prop Active";
			if (this.PropParent != null)
			{
				this.PropParent.SetActive(true);
			}
			if (this.TrapParent != null)
			{
				this.TrapParent.SetActive(false);
			}
		}
	}

	// Token: 0x04002100 RID: 8448
	public GameObject TrapParent;

	// Token: 0x04002101 RID: 8449
	public GameObject PropParent;

	// Token: 0x04002102 RID: 8450
	[HideInInspector]
	public string DebugState = "...";

	// Token: 0x04002103 RID: 8451
	[HideInInspector]
	public bool DebugSwitch;
}
