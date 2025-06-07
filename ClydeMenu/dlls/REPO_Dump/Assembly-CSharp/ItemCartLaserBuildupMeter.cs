using System;
using UnityEngine;

// Token: 0x0200014D RID: 333
public class ItemCartLaserBuildupMeter : MonoBehaviour
{
	// Token: 0x06000B6B RID: 2923 RVA: 0x00065D50 File Offset: 0x00063F50
	private void Start()
	{
		this.itemCartLaser = base.GetComponentInParent<ItemCartLaser>();
		this.itemCartCannonMain = base.GetComponentInParent<ItemCartCannonMain>();
		this.lightMeter = base.GetComponentInChildren<Light>();
		this.meshRenderer = base.GetComponentInChildren<MeshRenderer>();
		this.animationCurveBuildup = this.itemCartLaser.animationCurveBuildup;
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x00065DA0 File Offset: 0x00063FA0
	private void Update()
	{
		this.cartLaserStatePrev = this.itemCartLaser.stateCurrent;
		if (this.cartLaserStateCurrent != this.itemCartLaser.stateCurrent)
		{
			this.cartLaserStateCurrent = this.itemCartLaser.stateCurrent;
			this.UpdateMeterState();
		}
		this.StateMachine();
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00065DF0 File Offset: 0x00063FF0
	private void StateMachine()
	{
		switch (this.stateCurrent)
		{
		case ItemCartLaserBuildupMeter.State.Inactive:
			this.StateInactive();
			return;
		case ItemCartLaserBuildupMeter.State.Buildup:
			this.StateBuildup();
			return;
		case ItemCartLaserBuildupMeter.State.Shooting:
			this.StateShooting();
			return;
		case ItemCartLaserBuildupMeter.State.GoingBack:
			this.StateGoingBack();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00065E38 File Offset: 0x00064038
	private void LoopTexture()
	{
		this.meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(0f, -Time.time * 20f));
		float y = 2f + 2f * base.transform.localScale.z;
		this.meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1f, y));
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x00065EB0 File Offset: 0x000640B0
	private void LightIntensity()
	{
		float intensity = base.transform.localScale.z * 4f;
		this.lightMeter.intensity = intensity;
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00065EE0 File Offset: 0x000640E0
	private void StateInactive()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.ToggleMeterEffect(false);
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00065EF8 File Offset: 0x000640F8
	private void StateBuildup()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.ToggleMeterEffect(true);
		}
		float z = this.animationCurveBuildup.Evaluate(this.itemCartCannonMain.stateTimer / this.itemCartCannonMain.stateTimerMax);
		base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, z);
		this.LoopTexture();
		this.LightIntensity();
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00065F7C File Offset: 0x0006417C
	private void StateShooting()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.ToggleMeterEffect(true);
			base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, 1f);
		}
		this.lightMeter.intensity = 2f + Mathf.Sin(Time.time * 10f) * 2f;
		this.LoopTexture();
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00066004 File Offset: 0x00064204
	private void StateGoingBack()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.ToggleMeterEffect(true);
		}
		float num = this.animationCurveBuildup.Evaluate(this.itemCartCannonMain.stateTimer / this.itemCartCannonMain.stateTimerMax);
		base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, 1f - num);
		this.LightIntensity();
		this.LoopTexture();
		if (num > 0.95f && this.lightMeter.enabled)
		{
			this.ToggleMeterEffect(false);
		}
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x000660A9 File Offset: 0x000642A9
	private void ToggleMeterEffect(bool _toggle)
	{
		this.meshRenderer.enabled = _toggle;
		this.lightMeter.enabled = _toggle;
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x000660C4 File Offset: 0x000642C4
	private void UpdateMeterState()
	{
		if (this.cartLaserStateCurrent == 0 || this.cartLaserStateCurrent == 1)
		{
			this.StateSet(ItemCartLaserBuildupMeter.State.Inactive);
		}
		if (this.cartLaserStateCurrent == 2)
		{
			this.StateSet(ItemCartLaserBuildupMeter.State.Buildup);
		}
		if (this.cartLaserStateCurrent == 3)
		{
			this.StateSet(ItemCartLaserBuildupMeter.State.Shooting);
		}
		if (this.cartLaserStateCurrent == 4)
		{
			this.StateSet(ItemCartLaserBuildupMeter.State.GoingBack);
		}
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00066119 File Offset: 0x00064319
	private void StateSet(ItemCartLaserBuildupMeter.State _state)
	{
		if (this.stateCurrent == this.statePrevState)
		{
			return;
		}
		if (this.stateCurrent == _state)
		{
			return;
		}
		this.statePrevState = this.stateCurrent;
		this.stateCurrent = _state;
		this.stateStart = true;
	}

	// Token: 0x04001286 RID: 4742
	private ItemCartLaser itemCartLaser;

	// Token: 0x04001287 RID: 4743
	private Light lightMeter;

	// Token: 0x04001288 RID: 4744
	private MeshRenderer meshRenderer;

	// Token: 0x04001289 RID: 4745
	private ItemCartCannonMain itemCartCannonMain;

	// Token: 0x0400128A RID: 4746
	private AnimationCurve animationCurveBuildup;

	// Token: 0x0400128B RID: 4747
	private ItemCartLaserBuildupMeter.State statePrevState = ItemCartLaserBuildupMeter.State.GoingBack;

	// Token: 0x0400128C RID: 4748
	private ItemCartLaserBuildupMeter.State stateCurrent;

	// Token: 0x0400128D RID: 4749
	private bool stateStart = true;

	// Token: 0x0400128E RID: 4750
	private int cartLaserStatePrev;

	// Token: 0x0400128F RID: 4751
	private int cartLaserStateCurrent;

	// Token: 0x0200037E RID: 894
	private enum State
	{
		// Token: 0x04002B38 RID: 11064
		Inactive,
		// Token: 0x04002B39 RID: 11065
		Buildup,
		// Token: 0x04002B3A RID: 11066
		Shooting,
		// Token: 0x04002B3B RID: 11067
		GoingBack
	}
}
