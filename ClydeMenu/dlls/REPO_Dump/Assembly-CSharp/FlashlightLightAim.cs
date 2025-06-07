using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class FlashlightLightAim : MonoBehaviour
{
	// Token: 0x060006C5 RID: 1733 RVA: 0x0004173D File Offset: 0x0003F93D
	private void Start()
	{
		this.lightComponent = base.GetComponent<Light>();
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x0004174C File Offset: 0x0003F94C
	private void Update()
	{
		this.clientAimPointCurrent = Vector3.Lerp(this.clientAimPointCurrent, this.clientAimPoint, Time.deltaTime * 20f);
		if (!this.playerAvatar.isLocal)
		{
			Vector3 vector = this.clientAimPointCurrent - base.transform.position;
			vector = SemiFunc.ClampDirection(vector, base.transform.parent.forward, 45f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * 10f);
			return;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, 100f, SemiFunc.LayerMaskGetVisionObstruct()) && !raycastHit.transform.GetComponentInParent<PlayerController>())
		{
			this.clientAimPoint = raycastHit.point;
		}
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00041836 File Offset: 0x0003FA36
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
		Gizmos.DrawSphere(this.clientAimPointCurrent, 0.1f);
	}

	// Token: 0x04000B87 RID: 2951
	public PlayerAvatar playerAvatar;

	// Token: 0x04000B88 RID: 2952
	public Vector3 clientAimPoint;

	// Token: 0x04000B89 RID: 2953
	private Vector3 clientAimPointCurrent;

	// Token: 0x04000B8A RID: 2954
	private Light lightComponent;

	// Token: 0x04000B8B RID: 2955
	private bool setBias;
}
