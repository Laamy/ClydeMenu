using System;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class ModulePropSwitch : MonoBehaviour
{
	// Token: 0x0600081F RID: 2079 RVA: 0x0004F848 File Offset: 0x0004DA48
	public void Setup()
	{
		int num = 0;
		while ((float)num < this.Module.transform.localRotation.eulerAngles.y)
		{
			num += 90;
			this.ConnectionSide++;
			if (this.ConnectionSide > ModulePropSwitch.Connection.Left)
			{
				this.ConnectionSide = ModulePropSwitch.Connection.Top;
			}
		}
		if (this.ConnectionSide == ModulePropSwitch.Connection.Top && this.Module.ConnectingTop)
		{
			this.Connected = true;
		}
		else if (this.ConnectionSide == ModulePropSwitch.Connection.Right && this.Module.ConnectingRight)
		{
			this.Connected = true;
		}
		else if (this.ConnectionSide == ModulePropSwitch.Connection.Bot && this.Module.ConnectingBottom)
		{
			this.Connected = true;
		}
		else if (this.ConnectionSide == ModulePropSwitch.Connection.Left && this.Module.ConnectingLeft)
		{
			this.Connected = true;
		}
		if (this.Connected)
		{
			this.NotConnectedParent.SetActive(false);
			this.ConnectedParent.SetActive(true);
			return;
		}
		this.NotConnectedParent.SetActive(true);
		this.ConnectedParent.SetActive(false);
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0004F950 File Offset: 0x0004DB50
	public void Toggle()
	{
		if (this.DebugSwitch)
		{
			this.DebugSwitch = false;
			this.DebugState = "Connected";
			this.NotConnectedParent.SetActive(false);
			this.ConnectedParent.SetActive(true);
			return;
		}
		this.DebugSwitch = true;
		this.DebugState = "Not Connected";
		this.NotConnectedParent.SetActive(true);
		this.ConnectedParent.SetActive(false);
	}

	// Token: 0x04000F01 RID: 3841
	internal Module Module;

	// Token: 0x04000F02 RID: 3842
	public GameObject ConnectedParent;

	// Token: 0x04000F03 RID: 3843
	public GameObject NotConnectedParent;

	// Token: 0x04000F04 RID: 3844
	private bool Connected;

	// Token: 0x04000F05 RID: 3845
	[Space(20f)]
	public ModulePropSwitch.Connection ConnectionSide;

	// Token: 0x04000F06 RID: 3846
	[HideInInspector]
	public string DebugState = "...";

	// Token: 0x04000F07 RID: 3847
	[HideInInspector]
	public bool DebugSwitch;

	// Token: 0x0200034F RID: 847
	public enum Connection
	{
		// Token: 0x04002A39 RID: 10809
		Top,
		// Token: 0x04002A3A RID: 10810
		Right,
		// Token: 0x04002A3B RID: 10811
		Bot,
		// Token: 0x04002A3C RID: 10812
		Left
	}
}
