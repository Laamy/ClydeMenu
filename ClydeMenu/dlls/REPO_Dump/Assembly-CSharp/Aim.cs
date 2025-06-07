using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200025E RID: 606
public class Aim : MonoBehaviour
{
	// Token: 0x0600136C RID: 4972 RVA: 0x000AD33D File Offset: 0x000AB53D
	private void Awake()
	{
		Aim.instance = this;
		this.image = base.GetComponent<Image>();
		this.defaultState = this.aimStates[0];
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x000AD364 File Offset: 0x000AB564
	private void Update()
	{
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= 1f * Time.deltaTime;
		}
		else if (this.currentState != Aim.State.Default)
		{
			this.animLerp = 0f;
			this.currentState = Aim.State.Default;
			this.currentSprite = this.defaultState.Sprite;
			this.currentColor = this.defaultState.Color;
		}
		if (this.currentState == this.previousState)
		{
			if (this.animLerp < 1f)
			{
				this.animLerp += 10f * Time.deltaTime;
				base.transform.localScale = Vector3.one * this.curveOutro.Evaluate(this.animLerp);
			}
		}
		else
		{
			this.animLerp += 15f * Time.deltaTime;
			base.transform.localScale = Vector3.one * this.curveIntro.Evaluate(this.animLerp);
			if (this.animLerp >= 1f)
			{
				this.image.sprite = this.currentSprite;
				this.image.color = this.currentColor;
				this.previousState = this.currentState;
				this.animLerp = 0f;
			}
		}
		if (this.previousState == this.currentState)
		{
			if (this.currentState == Aim.State.Rotate)
			{
				base.transform.localRotation = Quaternion.Euler(0f, 0f, base.transform.localRotation.eulerAngles.z - 100f * Time.deltaTime);
				return;
			}
			base.transform.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x000AD520 File Offset: 0x000AB720
	public void SetState(Aim.State _state)
	{
		if (_state == this.currentState)
		{
			this.stateTimer = 0.25f;
			return;
		}
		foreach (Aim.AimState aimState in this.aimStates)
		{
			if (aimState.State == _state)
			{
				this.currentState = aimState.State;
				this.currentSprite = aimState.Sprite;
				this.currentColor = aimState.Color;
				this.animLerp = 0f;
				this.stateTimer = 0.2f;
				break;
			}
		}
	}

	// Token: 0x0400216D RID: 8557
	public static Aim instance;

	// Token: 0x0400216E RID: 8558
	[Space]
	public AnimationCurve curveIntro;

	// Token: 0x0400216F RID: 8559
	public AnimationCurve curveOutro;

	// Token: 0x04002170 RID: 8560
	private float animLerp;

	// Token: 0x04002171 RID: 8561
	[Space]
	public List<Aim.AimState> aimStates;

	// Token: 0x04002172 RID: 8562
	private Aim.AimState defaultState;

	// Token: 0x04002173 RID: 8563
	private Image image;

	// Token: 0x04002174 RID: 8564
	private float stateTimer;

	// Token: 0x04002175 RID: 8565
	private Aim.State currentState;

	// Token: 0x04002176 RID: 8566
	private Aim.State previousState;

	// Token: 0x04002177 RID: 8567
	private Sprite currentSprite;

	// Token: 0x04002178 RID: 8568
	private Color currentColor;

	// Token: 0x02000403 RID: 1027
	public enum State
	{
		// Token: 0x04002D70 RID: 11632
		Default,
		// Token: 0x04002D71 RID: 11633
		Grabbable,
		// Token: 0x04002D72 RID: 11634
		Grab,
		// Token: 0x04002D73 RID: 11635
		Rotate,
		// Token: 0x04002D74 RID: 11636
		Hidden
	}

	// Token: 0x02000404 RID: 1028
	[Serializable]
	public class AimState
	{
		// Token: 0x04002D75 RID: 11637
		public Aim.State State;

		// Token: 0x04002D76 RID: 11638
		public Sprite Sprite;

		// Token: 0x04002D77 RID: 11639
		public Color Color;
	}
}
