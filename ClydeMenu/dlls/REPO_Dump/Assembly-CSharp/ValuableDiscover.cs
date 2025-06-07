using System;
using UnityEngine;

// Token: 0x02000295 RID: 661
public class ValuableDiscover : MonoBehaviour
{
	// Token: 0x060014C0 RID: 5312 RVA: 0x000B721A File Offset: 0x000B541A
	private void Awake()
	{
		ValuableDiscover.instance = this;
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x000B7230 File Offset: 0x000B5430
	private void Update()
	{
		float b = 1f;
		if (this.hideTimer > 0f)
		{
			b = 0f;
			this.hideTimer -= Time.deltaTime;
		}
		this.hideAlpha = Mathf.Lerp(this.hideAlpha, b, Time.deltaTime * 20f);
		this.canvasGroup.alpha = this.hideAlpha;
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x000B7298 File Offset: 0x000B5498
	public void New(PhysGrabObject _target, ValuableDiscoverGraphic.State _state)
	{
		ValuableDiscoverGraphic component = Object.Instantiate<GameObject>(this.graphicPrefab, base.transform).GetComponent<ValuableDiscoverGraphic>();
		component.target = _target;
		if (_state == ValuableDiscoverGraphic.State.Reminder)
		{
			component.ReminderSetup();
		}
		if (_state == ValuableDiscoverGraphic.State.Bad)
		{
			component.BadSetup();
		}
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x000B72D7 File Offset: 0x000B54D7
	public void Hide()
	{
		this.hideTimer = 0.1f;
	}

	// Token: 0x040023AB RID: 9131
	public static ValuableDiscover instance;

	// Token: 0x040023AC RID: 9132
	public GameObject graphicPrefab;

	// Token: 0x040023AD RID: 9133
	public RectTransform canvasRect;

	// Token: 0x040023AE RID: 9134
	private CanvasGroup canvasGroup;

	// Token: 0x040023AF RID: 9135
	private float hideTimer;

	// Token: 0x040023B0 RID: 9136
	internal float hideAlpha = 1f;
}
