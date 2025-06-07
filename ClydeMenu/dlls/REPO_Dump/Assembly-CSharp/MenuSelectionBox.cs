using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000206 RID: 518
public class MenuSelectionBox : MonoBehaviour
{
	// Token: 0x0600117F RID: 4479 RVA: 0x0009F8D8 File Offset: 0x0009DAD8
	private void Start()
	{
		this.targetPosition = base.transform.localPosition;
		this.targetScale = base.transform.localScale * 100f;
		this.originalPos = base.transform.localPosition;
		this.originalScale = base.transform.localScale;
		this.rawImage = base.GetComponentInChildren<RawImage>();
		this.menuPage = base.GetComponentInParent<MenuPage>();
		this.menuPage.selectionBox = this;
		this.rectTransform = base.GetComponent<RectTransform>();
		this.isInScrollBox = false;
		this.menuScrollBox = base.GetComponentInParent<MenuScrollBox>();
		if (this.menuScrollBox)
		{
			this.isInScrollBox = true;
		}
		else
		{
			MenuSelectionBox.instance = this;
		}
		MenuManager.instance.SelectionBoxAdd(this);
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0009F9A0 File Offset: 0x0009DBA0
	private void Update()
	{
		if (this.menuPage.currentPageState != MenuPage.PageState.Active && !this.menuPage.addedPageOnTop)
		{
			this.rawImage.color = new Color(0.4f, 0.08f, 0.015f, 0f);
			RectTransform component = this.menuPage.GetComponent<RectTransform>();
			base.transform.localPosition = new Vector3(component.rect.width / 2f, component.rect.height / 2f, base.transform.localPosition.z);
			this.currentScale = new Vector3(0f, 0f, 1f);
			this.targetScale = this.currentScale * 100f;
			this.targetPosition = this.rectTransform.localPosition;
			this.activeTargetTimer = 0f;
			return;
		}
		if (this.prevPosTimer <= 0f)
		{
			this.prevPos = this.currentPos;
			this.currentPos = base.transform.localPosition;
			this.prevPosTimer = 0.008333334f;
		}
		else
		{
			this.prevPosTimer -= Time.deltaTime;
		}
		if (this.rawImage.color.a < 0.001f)
		{
			this.currentScale = Vector3.zero;
			this.rectTransform.localPosition = this.targetPosition;
		}
		else
		{
			this.rectTransform.localPosition = Vector3.Lerp(this.rectTransform.localPosition, this.targetPosition, 20f * Time.deltaTime);
		}
		this.currentScale = Vector3.Lerp(this.currentScale, this.targetScale / 100f, 20f * Time.deltaTime);
		this.pulsateLerp += 3f * Time.deltaTime;
		this.pulsateLerp = Mathf.Clamp01(this.pulsateLerp);
		Vector3 a;
		if (this.pulsateState)
		{
			a = Vector3.LerpUnclamped(-Vector3.one, Vector3.one, this.pulsateInCurve.Evaluate(this.pulsateLerp));
		}
		else
		{
			a = Vector3.LerpUnclamped(Vector3.one, -Vector3.one, this.pulsateOutCurve.Evaluate(this.pulsateLerp));
		}
		if (this.pulsateLerp >= 1f)
		{
			this.pulsateLerp = 0f;
			this.pulsateState = !this.pulsateState;
		}
		base.transform.localScale = this.currentScale + a * 0.01f;
		if (this.activeTargetTimer > 0f)
		{
			this.activeTargetTimer -= Time.deltaTime;
			Color a2 = new Color(0.08f, 0.2f, 0.4f, 0.75f);
			Color b = new Color(0.2f, 0.5f, 1f, 1f);
			if (Vector3.Distance(base.transform.localPosition, this.targetPosition) <= 5f)
			{
				this.prevPos = this.currentPos;
			}
			this.rawImage.color = Color.Lerp(a2, b, Vector3.Distance(this.prevPos, this.currentPos) * 0.5f);
		}
		else
		{
			Color b2 = new Color(0.4f, 0.08f, 0.015f, 0f);
			this.rawImage.color = Color.Lerp(this.rawImage.color, b2, 10f * Time.deltaTime);
		}
		this.ClickColorAnimate();
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x0009FD28 File Offset: 0x0009DF28
	public void MenuSelectionBoxSetTarget(Vector3 pos, Vector3 scale, MenuPage parentPage, bool _isInScrollBox, MenuScrollBox _menuScrollBox, Vector2 customScale = default(Vector2))
	{
		if (_isInScrollBox != this.isInScrollBox || (_isInScrollBox && _menuScrollBox != this.menuScrollBox))
		{
			MenuSelectionBox menuSelectionBox = MenuManager.instance.SelectionBoxGetCorrect(parentPage, _menuScrollBox);
			if (menuSelectionBox)
			{
				MenuManager.instance.SetActiveSelectionBox(menuSelectionBox);
				parentPage.selectionBox = menuSelectionBox;
				menuSelectionBox.Reinstate();
				menuSelectionBox.MenuSelectionBoxSetTarget(pos, scale, parentPage, _isInScrollBox, _menuScrollBox, customScale);
			}
			return;
		}
		MenuManager.instance.SetActiveSelectionBox(this);
		if (MenuSelectionBox.instance != this)
		{
			this.Reinstate();
		}
		if (this.firstSelection)
		{
			this.firstSelection = false;
			base.transform.localPosition = pos;
			this.currentScale = Vector3.zero;
			base.transform.localScale = this.currentScale;
			this.targetPosition = pos;
			this.targetScale = scale;
			return;
		}
		pos = new Vector3(pos.x, pos.y, 0f);
		this.targetPosition = pos;
		this.targetScale = scale + new Vector3(customScale.x, customScale.y, 0f);
		float num = this.targetScale.y * 0.2f;
		this.targetScale += new Vector3(num, num, 0f);
		this.targetPosition += new Vector3(0f, 0f, 0f);
		this.activeTargetTimer = 0.2f;
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x0009FE96 File Offset: 0x0009E096
	public void SetClick(Color color)
	{
		this.flashColor = color;
		this.clickTimer = 1f;
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x0009FEAC File Offset: 0x0009E0AC
	private void ClickColorAnimate()
	{
		if (this.clickTimer <= 0f)
		{
			return;
		}
		Color a = this.flashColor;
		Color b = new Color(0.08f, 0.2f, 0.4f, 0.75f);
		this.rawImage.color = Color.Lerp(a, b, 1f - this.clickTimer);
		this.clickTimer -= Time.deltaTime * 10f;
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x0009FF20 File Offset: 0x0009E120
	private void OnEnable()
	{
		base.transform.localPosition = this.originalPos;
		this.currentScale = this.originalScale;
		base.transform.localScale = this.originalScale;
		this.targetScale = this.originalScale * 100f;
		this.targetPosition = this.originalPos;
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0009FF7D File Offset: 0x0009E17D
	private void OnDestroy()
	{
		MenuManager.instance.SelectionBoxRemove(this);
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0009FF8A File Offset: 0x0009E18A
	public void Reinstate()
	{
		MenuSelectionBox.instance = this;
	}

	// Token: 0x04001DAC RID: 7596
	public static MenuSelectionBox instance;

	// Token: 0x04001DAD RID: 7597
	internal Vector3 targetPosition;

	// Token: 0x04001DAE RID: 7598
	internal Vector3 targetScale;

	// Token: 0x04001DAF RID: 7599
	private Vector3 currentScale;

	// Token: 0x04001DB0 RID: 7600
	internal RawImage rawImage;

	// Token: 0x04001DB1 RID: 7601
	internal RectTransform rectTransform;

	// Token: 0x04001DB2 RID: 7602
	internal Vector3 originalPos;

	// Token: 0x04001DB3 RID: 7603
	internal Vector3 originalScale;

	// Token: 0x04001DB4 RID: 7604
	private float activeTargetTimer;

	// Token: 0x04001DB5 RID: 7605
	private float prevPosTimer;

	// Token: 0x04001DB6 RID: 7606
	private Vector3 prevPos;

	// Token: 0x04001DB7 RID: 7607
	private Vector3 currentPos;

	// Token: 0x04001DB8 RID: 7608
	private float clickTimer;

	// Token: 0x04001DB9 RID: 7609
	private Color flashColor;

	// Token: 0x04001DBA RID: 7610
	internal MenuPage menuPage;

	// Token: 0x04001DBB RID: 7611
	internal bool firstSelection = true;

	// Token: 0x04001DBC RID: 7612
	internal bool isInScrollBox;

	// Token: 0x04001DBD RID: 7613
	internal MenuScrollBox menuScrollBox;

	// Token: 0x04001DBE RID: 7614
	public AnimationCurve pulsateInCurve;

	// Token: 0x04001DBF RID: 7615
	public AnimationCurve pulsateOutCurve;

	// Token: 0x04001DC0 RID: 7616
	private float pulsateLerp;

	// Token: 0x04001DC1 RID: 7617
	private bool pulsateState;
}
