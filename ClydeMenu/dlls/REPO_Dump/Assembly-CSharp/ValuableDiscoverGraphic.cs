using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000296 RID: 662
public class ValuableDiscoverGraphic : MonoBehaviour
{
	// Token: 0x060014C5 RID: 5317 RVA: 0x000B72F8 File Offset: 0x000B54F8
	private void Start()
	{
		this.canvasRect = ValuableDiscover.instance.canvasRect;
		this.mainCamera = Camera.main;
		this.waitTimer = this.waitTime;
		if (this.state == ValuableDiscoverGraphic.State.Reminder)
		{
			this.waitTimer = this.waitTime * 0.5f;
		}
		if (this.state == ValuableDiscoverGraphic.State.Bad)
		{
			this.waitTimer = this.waitTime * 3f;
		}
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x000B7370 File Offset: 0x000B5570
	private void Update()
	{
		if (this.target)
		{
			bool flag = true;
			Bounds bigBounds = new Bounds(this.target.centerPoint, Vector3.zero);
			foreach (MeshRenderer meshRenderer in this.target.GetComponentsInChildren<MeshRenderer>())
			{
				bigBounds.Encapsulate(meshRenderer.bounds);
			}
			if (SemiFunc.OnScreen(bigBounds.center, 0.5f, 0.5f))
			{
				Rect rect = this.RendererBoundsInScreenSpace(bigBounds);
				if (rect.width > 2f || rect.height > 2f)
				{
					this.topLeftTargetNew = rect.center;
					this.topRightTargetNew = rect.center;
					this.botLeftTargetNew = rect.center;
					this.botRightTargetNew = rect.center;
					this.middleTargetNew = rect.center;
					this.middleTargetSizeNew = new Vector2(0f, 0f);
				}
				else
				{
					this.topLeftTargetNew = this.GetScreenPosition(new Vector3(rect.xMin, rect.yMax, 0f));
					this.topRightTargetNew = this.GetScreenPosition(new Vector3(rect.xMax, rect.yMax, 0f));
					this.botLeftTargetNew = this.GetScreenPosition(new Vector3(rect.xMin, rect.yMin, 0f));
					this.botRightTargetNew = this.GetScreenPosition(new Vector3(rect.xMax, rect.yMin, 0f));
					this.middleTargetNew = this.GetScreenPosition(rect.center);
					this.middleTargetSizeNew = new Vector2(rect.width * 1.9f + 0.025f, rect.height + 0.025f);
				}
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				if (this.first)
				{
					if (this.state == ValuableDiscoverGraphic.State.Reminder)
					{
						this.sound.Play(this.target.centerPoint, 0.3f, 1f, 1f, 1f);
					}
					else
					{
						this.sound.Play(this.target.centerPoint, 1f, 1f, 1f, 1f);
					}
					this.middle.gameObject.SetActive(true);
					this.topLeft.gameObject.SetActive(true);
					this.topRight.gameObject.SetActive(true);
					this.botLeft.gameObject.SetActive(true);
					this.botRight.gameObject.SetActive(true);
					this.first = false;
				}
				if (this.hidden)
				{
					this.middleTarget = this.middleTargetNew;
					this.middleTargetSize = this.middleTargetSizeNew;
					this.topLeftTarget = this.topLeftTargetNew;
					this.topRightTarget = this.topRightTargetNew;
					this.botLeftTarget = this.botLeftTargetNew;
					this.botRightTarget = this.botRightTargetNew;
					this.hidden = false;
				}
				this.middleTarget = Vector2.Lerp(this.middleTarget, this.middleTargetNew, 50f * Time.deltaTime);
				this.middleTargetSize = Vector2.Lerp(this.middleTargetSize, this.middleTargetSizeNew, 50f * Time.deltaTime);
				this.topLeftTarget = Vector2.Lerp(this.topLeftTarget, this.topLeftTargetNew, 50f * Time.deltaTime);
				this.topRightTarget = Vector2.Lerp(this.topRightTarget, this.topRightTargetNew, 50f * Time.deltaTime);
				this.botLeftTarget = Vector2.Lerp(this.botLeftTarget, this.botLeftTargetNew, 50f * Time.deltaTime);
				this.botRightTarget = Vector2.Lerp(this.botRightTarget, this.botRightTargetNew, 50f * Time.deltaTime);
				this.canvasGroup.alpha = Mathf.Lerp(this.canvasGroup.alpha, 1f, 50f * Time.deltaTime);
			}
			else
			{
				this.hidden = true;
				this.topLeftTarget = this.middleTarget;
				this.topRightTarget = this.middleTarget;
				this.botLeftTarget = this.middleTarget;
				this.botRightTarget = this.middleTarget;
				this.middleTargetSize = Vector2.zero;
				this.canvasGroup.alpha = Mathf.Lerp(this.canvasGroup.alpha, 0f, 50f * Time.deltaTime);
			}
		}
		else
		{
			this.waitTimer = 0f;
		}
		this.middle.anchoredPosition = this.middleTarget;
		if (this.waitTimer > 0f)
		{
			this.animLerp = Mathf.Clamp01(this.animLerp + this.introSpeed * Time.deltaTime);
			this.middle.sizeDelta = Vector2.LerpUnclamped(Vector2.zero, this.middleTargetSize, this.introCurve.Evaluate(this.animLerp));
			this.topLeft.anchoredPosition = Vector2.LerpUnclamped(this.middleTarget, this.topLeftTarget, this.introCurve.Evaluate(this.animLerp));
			this.topRight.anchoredPosition = Vector2.LerpUnclamped(this.middleTarget, this.topRightTarget, this.introCurve.Evaluate(this.animLerp));
			this.botLeft.anchoredPosition = Vector2.LerpUnclamped(this.middleTarget, this.botLeftTarget, this.introCurve.Evaluate(this.animLerp));
			this.botRight.anchoredPosition = Vector2.LerpUnclamped(this.middleTarget, this.botRightTarget, this.introCurve.Evaluate(this.animLerp));
			if (this.animLerp >= 1f)
			{
				this.waitTimer -= Time.deltaTime;
				if (this.waitTimer <= 0f)
				{
					this.animLerp = 0f;
					return;
				}
			}
		}
		else
		{
			this.animLerp = Mathf.Clamp01(this.animLerp + this.outroSpeed * Time.deltaTime);
			this.middle.sizeDelta = Vector2.LerpUnclamped(this.middleTargetSize, Vector2.zero, this.outroCurve.Evaluate(this.animLerp));
			this.topLeft.anchoredPosition = Vector2.LerpUnclamped(this.topLeftTarget, this.middleTarget, this.outroCurve.Evaluate(this.animLerp));
			this.topRight.anchoredPosition = Vector2.LerpUnclamped(this.topRightTarget, this.middleTarget, this.outroCurve.Evaluate(this.animLerp));
			this.botLeft.anchoredPosition = Vector2.LerpUnclamped(this.botLeftTarget, this.middleTarget, this.outroCurve.Evaluate(this.animLerp));
			this.botRight.anchoredPosition = Vector2.LerpUnclamped(this.botRightTarget, this.middleTarget, this.outroCurve.Evaluate(this.animLerp));
			if (this.animLerp >= 1f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x000B7A6C File Offset: 0x000B5C6C
	public void ReminderSetup()
	{
		this.state = ValuableDiscoverGraphic.State.Reminder;
		this.middle.GetComponent<Image>().color = this.ColorReminderMiddle;
		this.topLeft.GetComponent<Image>().color = this.ColorReminderCorner;
		this.topRight.GetComponent<Image>().color = this.ColorReminderCorner;
		this.botLeft.GetComponent<Image>().color = this.ColorReminderCorner;
		this.botRight.GetComponent<Image>().color = this.ColorReminderCorner;
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x000B7AF0 File Offset: 0x000B5CF0
	public void BadSetup()
	{
		this.state = ValuableDiscoverGraphic.State.Bad;
		this.middle.GetComponent<Image>().color = this.ColorBadMiddle;
		this.topLeft.GetComponent<Image>().color = this.ColorBadCorner;
		this.topRight.GetComponent<Image>().color = this.ColorBadCorner;
		this.botLeft.GetComponent<Image>().color = this.ColorBadCorner;
		this.botRight.GetComponent<Image>().color = this.ColorBadCorner;
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x000B7B74 File Offset: 0x000B5D74
	private Vector3 GetScreenPosition(Vector3 _position)
	{
		return new Vector3(_position.x * this.canvasRect.sizeDelta.x - this.canvasRect.sizeDelta.x * 0.5f, _position.y * this.canvasRect.sizeDelta.y - this.canvasRect.sizeDelta.y * 0.5f, _position.z) / SemiFunc.UIMulti();
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x000B7BF4 File Offset: 0x000B5DF4
	private Rect RendererBoundsInScreenSpace(Bounds bigBounds)
	{
		if (this.screenSpaceCorners == null)
		{
			this.screenSpaceCorners = new Vector3[8];
		}
		this.screenSpaceCorners[0] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		this.screenSpaceCorners[1] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		this.screenSpaceCorners[2] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		this.screenSpaceCorners[3] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		this.screenSpaceCorners[4] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		this.screenSpaceCorners[5] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		this.screenSpaceCorners[6] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		this.screenSpaceCorners[7] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		float x = this.screenSpaceCorners[0].x;
		float y = this.screenSpaceCorners[0].y;
		float x2 = this.screenSpaceCorners[0].x;
		float y2 = this.screenSpaceCorners[0].y;
		for (int i = 1; i < 8; i++)
		{
			if (this.screenSpaceCorners[i].x < x)
			{
				x = this.screenSpaceCorners[i].x;
			}
			if (this.screenSpaceCorners[i].y < y)
			{
				y = this.screenSpaceCorners[i].y;
			}
			if (this.screenSpaceCorners[i].x > x2)
			{
				x2 = this.screenSpaceCorners[i].x;
			}
			if (this.screenSpaceCorners[i].y > y2)
			{
				y2 = this.screenSpaceCorners[i].y;
			}
		}
		return Rect.MinMaxRect(x, y, x2, y2);
	}

	// Token: 0x040023B1 RID: 9137
	public PhysGrabObject target;

	// Token: 0x040023B2 RID: 9138
	private ValuableDiscoverGraphic.State state;

	// Token: 0x040023B3 RID: 9139
	private Camera mainCamera;

	// Token: 0x040023B4 RID: 9140
	private RectTransform canvasRect;

	// Token: 0x040023B5 RID: 9141
	private Vector3[] screenSpaceCorners;

	// Token: 0x040023B6 RID: 9142
	private bool hidden = true;

	// Token: 0x040023B7 RID: 9143
	private bool first = true;

	// Token: 0x040023B8 RID: 9144
	[Space]
	public Color colorDiscoverCorner;

	// Token: 0x040023B9 RID: 9145
	public Color colorDiscoverMiddle;

	// Token: 0x040023BA RID: 9146
	[Space]
	public Color ColorReminderCorner;

	// Token: 0x040023BB RID: 9147
	public Color ColorReminderMiddle;

	// Token: 0x040023BC RID: 9148
	[Space]
	public Color ColorBadCorner;

	// Token: 0x040023BD RID: 9149
	public Color ColorBadMiddle;

	// Token: 0x040023BE RID: 9150
	[Space]
	public Sound sound;

	// Token: 0x040023BF RID: 9151
	[Space]
	public AnimationCurve introCurve;

	// Token: 0x040023C0 RID: 9152
	public float introSpeed;

	// Token: 0x040023C1 RID: 9153
	public AnimationCurve outroCurve;

	// Token: 0x040023C2 RID: 9154
	public float outroSpeed;

	// Token: 0x040023C3 RID: 9155
	public float waitTime;

	// Token: 0x040023C4 RID: 9156
	private float waitTimer;

	// Token: 0x040023C5 RID: 9157
	private float animLerp;

	// Token: 0x040023C6 RID: 9158
	[Space]
	public RectTransform middle;

	// Token: 0x040023C7 RID: 9159
	private Vector2 middleTarget;

	// Token: 0x040023C8 RID: 9160
	private Vector2 middleTargetNew;

	// Token: 0x040023C9 RID: 9161
	private Vector2 middleTargetSize;

	// Token: 0x040023CA RID: 9162
	private Vector2 middleTargetSizeNew;

	// Token: 0x040023CB RID: 9163
	public RectTransform topLeft;

	// Token: 0x040023CC RID: 9164
	private Vector2 topLeftTarget;

	// Token: 0x040023CD RID: 9165
	private Vector2 topLeftTargetNew;

	// Token: 0x040023CE RID: 9166
	public RectTransform topRight;

	// Token: 0x040023CF RID: 9167
	private Vector2 topRightTarget;

	// Token: 0x040023D0 RID: 9168
	private Vector2 topRightTargetNew;

	// Token: 0x040023D1 RID: 9169
	public RectTransform botLeft;

	// Token: 0x040023D2 RID: 9170
	private Vector2 botLeftTarget;

	// Token: 0x040023D3 RID: 9171
	private Vector2 botLeftTargetNew;

	// Token: 0x040023D4 RID: 9172
	public RectTransform botRight;

	// Token: 0x040023D5 RID: 9173
	private Vector2 botRightTarget;

	// Token: 0x040023D6 RID: 9174
	private Vector2 botRightTargetNew;

	// Token: 0x040023D7 RID: 9175
	private CanvasGroup canvasGroup;

	// Token: 0x0200040F RID: 1039
	public enum State
	{
		// Token: 0x04002DA3 RID: 11683
		Discover,
		// Token: 0x04002DA4 RID: 11684
		Reminder,
		// Token: 0x04002DA5 RID: 11685
		Bad
	}
}
