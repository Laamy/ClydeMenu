using System;
using TMPro;
using UnityEngine;

// Token: 0x0200029C RID: 668
public class WorldSpaceUIValue : WorldSpaceUIChild
{
	// Token: 0x060014E2 RID: 5346 RVA: 0x000B8C5E File Offset: 0x000B6E5E
	private void Awake()
	{
		this.positionOffset = new Vector3(0f, -0.05f, 0f);
		WorldSpaceUIValue.instance = this;
		this.scale = base.transform.localScale;
		this.text = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x000B8CA0 File Offset: 0x000B6EA0
	protected override void Update()
	{
		base.Update();
		this.worldPosition = Vector3.Lerp(this.worldPosition, this.newWorldPosition, 50f * Time.deltaTime);
		if (this.currentPhysGrabObject)
		{
			this.newWorldPosition = this.currentPhysGrabObject.centerPoint + this.offset;
		}
		if (this.showTimer > 0f)
		{
			this.showTimer -= Time.deltaTime;
			this.curveLerp += 10f * Time.deltaTime;
			this.curveLerp = Mathf.Clamp01(this.curveLerp);
			base.transform.localScale = this.scale * this.curveIntro.Evaluate(this.curveLerp);
			return;
		}
		this.curveLerp -= 10f * Time.deltaTime;
		this.curveLerp = Mathf.Clamp01(this.curveLerp);
		base.transform.localScale = this.scale * this.curveOutro.Evaluate(this.curveLerp);
		if (this.curveLerp <= 0f)
		{
			this.currentPhysGrabObject = null;
		}
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x000B8DD4 File Offset: 0x000B6FD4
	public void Show(PhysGrabObject _grabObject, int _value, bool _cost, Vector3 _offset)
	{
		if (!this.currentPhysGrabObject || this.currentPhysGrabObject == _grabObject)
		{
			this.value = _value;
			if (_cost)
			{
				this.text.text = "-$" + SemiFunc.DollarGetString(this.value) + "K";
				this.text.fontSize = this.textSizeCost;
			}
			else
			{
				this.text.text = "$" + SemiFunc.DollarGetString(this.value);
				this.text.fontSize = this.textSizeValue;
			}
			this.showTimer = 0.1f;
			if (!this.currentPhysGrabObject)
			{
				this.offset = _offset;
				this.currentPhysGrabObject = _grabObject;
				this.newWorldPosition = this.currentPhysGrabObject.centerPoint + this.offset - Vector3.up * 0.1f;
				this.worldPosition = this.newWorldPosition;
				if (_cost)
				{
					this.text.color = this.colorCost;
					return;
				}
				this.text.color = this.colorValue;
			}
		}
	}

	// Token: 0x04002409 RID: 9225
	public static WorldSpaceUIValue instance;

	// Token: 0x0400240A RID: 9226
	private float showTimer;

	// Token: 0x0400240B RID: 9227
	private Vector3 scale;

	// Token: 0x0400240C RID: 9228
	private int value;

	// Token: 0x0400240D RID: 9229
	private TextMeshProUGUI text;

	// Token: 0x0400240E RID: 9230
	private Vector3 newWorldPosition;

	// Token: 0x0400240F RID: 9231
	private Vector3 offset;

	// Token: 0x04002410 RID: 9232
	private PhysGrabObject currentPhysGrabObject;

	// Token: 0x04002411 RID: 9233
	public AnimationCurve curveIntro;

	// Token: 0x04002412 RID: 9234
	public AnimationCurve curveOutro;

	// Token: 0x04002413 RID: 9235
	private float curveLerp;

	// Token: 0x04002414 RID: 9236
	[Space]
	public Color colorValue;

	// Token: 0x04002415 RID: 9237
	public Color colorCost;

	// Token: 0x04002416 RID: 9238
	[Space]
	public float textSizeValue;

	// Token: 0x04002417 RID: 9239
	public float textSizeCost;
}
