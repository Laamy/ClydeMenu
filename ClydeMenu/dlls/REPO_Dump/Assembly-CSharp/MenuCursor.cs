using System;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class MenuCursor : MonoBehaviour
{
	// Token: 0x060011BB RID: 4539 RVA: 0x000A1B14 File Offset: 0x0009FD14
	private void Start()
	{
		this.mesh = base.transform.GetChild(0).gameObject;
		base.transform.localScale = Vector3.zero;
		this.mesh.SetActive(false);
		MenuCursor.instance = this;
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x000A1B4F File Offset: 0x0009FD4F
	public void OverridePosition(Vector3 _position)
	{
		base.transform.localPosition = new Vector3(_position.x, _position.y, 0f);
		this.overridePosTimer = 0.1f;
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x000A1B80 File Offset: 0x0009FD80
	private void Update()
	{
		if (this.overridePosTimer <= 0f)
		{
			if (Cursor.lockState == CursorLockMode.None)
			{
				Vector2 vector = SemiFunc.UIMousePosToUIPos();
				base.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
			}
		}
		else
		{
			this.overridePosTimer -= Time.deltaTime;
		}
		if (this.showTimer > 0f)
		{
			if (!this.mesh.activeSelf)
			{
				this.mesh.SetActive(true);
			}
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 30f);
			this.showTimer -= Time.deltaTime;
			return;
		}
		if (this.mesh.activeSelf)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.zero, Time.deltaTime * 30f);
			if (base.transform.localScale.magnitude < 0.1f && this.mesh.activeSelf)
			{
				this.mesh.SetActive(false);
			}
		}
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x000A1CAB File Offset: 0x0009FEAB
	public void Show()
	{
		this.showTimer = 0.01f;
	}

	// Token: 0x04001E19 RID: 7705
	private float showTimer;

	// Token: 0x04001E1A RID: 7706
	private GameObject mesh;

	// Token: 0x04001E1B RID: 7707
	public static MenuCursor instance;

	// Token: 0x04001E1C RID: 7708
	private float overridePosTimer;
}
