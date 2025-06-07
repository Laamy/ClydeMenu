using System;
using UnityEngine;

// Token: 0x020002CC RID: 716
public class ValuablePropSwitch : MonoBehaviour
{
	// Token: 0x06001666 RID: 5734 RVA: 0x000C5CCD File Offset: 0x000C3ECD
	private void Start()
	{
		this.ValuableParent.SetActive(true);
		this.PropParent.SetActive(false);
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x000C5CE8 File Offset: 0x000C3EE8
	public void Setup()
	{
		ValuablePropSwitch[] componentsInParent = base.gameObject.GetComponentsInParent<ValuablePropSwitch>(true);
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			if (componentsInParent[i] != this)
			{
				Debug.LogError("ValuablePropSwitch: Switches inside switches is not supported...", base.gameObject);
			}
		}
		if (!base.gameObject.GetComponentInChildren<ValuableVolume>(true))
		{
			Debug.LogError(base.gameObject.GetComponentInParent<Module>().gameObject.name + "  |  ValuablePropSwitch: No ValuableVolume found in children...", base.gameObject);
			return;
		}
		bool flag = false;
		ValuableObject[] componentsInChildren = base.GetComponentsInChildren<ValuableObject>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.activeSelf)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.PropParent.SetActive(false);
			this.ValuableParent.SetActive(true);
		}
		else
		{
			this.ValuableParent.SetActive(false);
			this.PropParent.SetActive(true);
		}
		this.SetupComplete = true;
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x000C5DD0 File Offset: 0x000C3FD0
	public void DebugToggle()
	{
		if (this.DebugSwitch)
		{
			this.DebugSwitch = false;
			this.DebugState = "Valuable Active";
			if (this.ValuableParent != null)
			{
				this.ValuableParent.SetActive(true);
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
			if (this.ValuableParent != null)
			{
				this.ValuableParent.SetActive(false);
			}
		}
	}

	// Token: 0x040026A8 RID: 9896
	public GameObject ValuableParent;

	// Token: 0x040026A9 RID: 9897
	public GameObject PropParent;

	// Token: 0x040026AA RID: 9898
	internal bool SetupComplete;

	// Token: 0x040026AB RID: 9899
	[HideInInspector]
	public string DebugState = "...";

	// Token: 0x040026AC RID: 9900
	[HideInInspector]
	public bool DebugSwitch;

	// Token: 0x040026AD RID: 9901
	[HideInInspector]
	public string ChildValuableString = "...";
}
