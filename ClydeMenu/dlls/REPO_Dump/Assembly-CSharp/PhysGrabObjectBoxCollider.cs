using System;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public class PhysGrabObjectBoxCollider : MonoBehaviour
{
	// Token: 0x06000E9F RID: 3743 RVA: 0x000848BC File Offset: 0x00082ABC
	private void Start()
	{
		if (this.unEquipCollider)
		{
			BoxCollider component = base.GetComponent<BoxCollider>();
			if (component)
			{
				component.enabled = false;
			}
		}
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x000848E8 File Offset: 0x00082AE8
	private void OnDrawGizmos()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		BoxCollider component = base.GetComponent<BoxCollider>();
		if (component == null)
		{
			return;
		}
		Color color = new Color(0f, 1f, 0f, 1f * this.gizmoTransparency);
		Color color2 = new Color(0f, 1f, 0f, 0.2f * this.gizmoTransparency);
		if (this.unEquipCollider)
		{
			color = (color2 = new Color(0f, 0.5f, 0f, 1f * this.gizmoTransparency));
			color2.a = 0.2f * this.gizmoTransparency;
		}
		Gizmos.color = color;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(component.center, component.size);
		Gizmos.color = color2;
		Gizmos.DrawCube(component.center, component.size);
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x04001837 RID: 6199
	public bool drawGizmos = true;

	// Token: 0x04001838 RID: 6200
	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;

	// Token: 0x04001839 RID: 6201
	public bool unEquipCollider;
}
