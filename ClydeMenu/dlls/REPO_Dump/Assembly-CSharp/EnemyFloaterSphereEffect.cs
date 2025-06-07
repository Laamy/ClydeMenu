using System;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class EnemyFloaterSphereEffect : MonoBehaviour
{
	// Token: 0x0600025F RID: 607 RVA: 0x00018498 File Offset: 0x00016698
	private void Start()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.lightSphere = base.GetComponentInChildren<Light>();
		this.floaterAttack = base.GetComponentInParent<FloaterAttackLogic>();
		this.originalScale = base.transform.localScale.x;
		this.myChildNumber = base.transform.GetSiblingIndex();
		this.originalMaterialColor = this.meshRenderer.material.color;
		if (this.lightSphere)
		{
			this.originalLightColor = this.lightSphere.color;
			this.originalLightIntensity = this.lightSphere.intensity;
			this.originalLightRange = this.lightSphere.range;
		}
	}

	// Token: 0x06000260 RID: 608 RVA: 0x00018548 File Offset: 0x00016748
	private void StateMachine()
	{
		switch (this.state)
		{
		case EnemyFloaterSphereEffect.FloaterSphereEffectState.levitate:
			this.StateLevitate();
			return;
		case EnemyFloaterSphereEffect.FloaterSphereEffectState.stop:
			this.StateStop();
			return;
		case EnemyFloaterSphereEffect.FloaterSphereEffectState.smash:
			this.StateSmash();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000261 RID: 609 RVA: 0x00018584 File Offset: 0x00016784
	private void StateLevitate()
	{
		if (this.stateStart)
		{
			base.transform.localScale = new Vector3(this.originalScale, this.originalScale, this.originalScale);
			this.meshRenderer.material.color = this.originalMaterialColor;
			if (this.lightSphere)
			{
				this.lightSphere.color = this.originalLightColor;
				this.lightSphere.intensity = this.originalLightIntensity;
				this.lightSphere.range = this.originalLightRange;
			}
			this.stateStart = false;
		}
		this.PulseEffect();
	}

	// Token: 0x06000262 RID: 610 RVA: 0x0001861E File Offset: 0x0001681E
	private void StateStop()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.StopEffect();
	}

	// Token: 0x06000263 RID: 611 RVA: 0x00018635 File Offset: 0x00016835
	private void StateSmash()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000264 RID: 612 RVA: 0x00018648 File Offset: 0x00016848
	private void Update()
	{
		this.StateMachine();
		if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.levitate || this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.start)
		{
			this.StateSet(EnemyFloaterSphereEffect.FloaterSphereEffectState.levitate);
		}
		if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop)
		{
			this.StateSet(EnemyFloaterSphereEffect.FloaterSphereEffectState.stop);
		}
		if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.smash)
		{
			this.StateSet(EnemyFloaterSphereEffect.FloaterSphereEffectState.smash);
		}
	}

	// Token: 0x06000265 RID: 613 RVA: 0x000186A7 File Offset: 0x000168A7
	private void StateSet(EnemyFloaterSphereEffect.FloaterSphereEffectState _state)
	{
		if (this.state != _state)
		{
			this.state = _state;
			this.stateStart = true;
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x000186C0 File Offset: 0x000168C0
	private void PulseEffect()
	{
		if (base.transform.parent.transform.localScale == Vector3.zero)
		{
			return;
		}
		base.transform.localScale += new Vector3(1f, 1f, 1f) * Time.deltaTime * 2f;
		Color color = this.meshRenderer.material.color;
		if (base.transform.localScale.magnitude > 10f)
		{
			color.a -= 1f * Time.deltaTime;
			if (this.lightSphere)
			{
				this.lightSphere.intensity = 4f * color.a;
			}
		}
		this.meshRenderer.material.color = color;
		if (this.lightSphere)
		{
			this.lightSphere.range = base.transform.localScale.x * 2.8f;
		}
		this.meshRenderer.material.mainTextureOffset += new Vector2(0.1f, 0.1f) * Time.deltaTime;
		if (color.a <= 0f)
		{
			if (this.lightSphere)
			{
				this.lightSphere.intensity = 4f;
			}
			if (this.lightSphere)
			{
				this.lightSphere.range = 0f;
			}
			base.transform.localScale = Vector3.zero;
			color.a = 1f;
			this.meshRenderer.material.color = color;
		}
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0001887C File Offset: 0x00016A7C
	private void StopEffect()
	{
		if (this.lightSphere)
		{
			Color red = Color.red;
			float b = 8f;
			float b2 = 15f;
			this.lightSphere.color = Color.Lerp(this.lightSphere.color, red, Time.deltaTime * 10f);
			this.lightSphere.intensity = Mathf.Lerp(this.lightSphere.intensity, b, Time.deltaTime * 10f);
			this.lightSphere.range = Mathf.Lerp(this.lightSphere.range, b2, Time.deltaTime * 10f);
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 10f);
		base.transform.localScale += new Vector3(0.4f, 0.4f, 0.4f) * Mathf.Sin((Time.time + (float)(this.myChildNumber * 10)) * (float)this.myChildNumber * 20f) * (0.1f + (float)this.myChildNumber / 10f);
		this.meshRenderer.material.color = Color.Lerp(this.meshRenderer.material.color, Color.red, Time.deltaTime * 10f);
	}

	// Token: 0x0400046F RID: 1135
	private MeshRenderer meshRenderer;

	// Token: 0x04000470 RID: 1136
	private Light lightSphere;

	// Token: 0x04000471 RID: 1137
	private FloaterAttackLogic floaterAttack;

	// Token: 0x04000472 RID: 1138
	private bool stateStart = true;

	// Token: 0x04000473 RID: 1139
	private float originalScale;

	// Token: 0x04000474 RID: 1140
	private Color originalLightColor;

	// Token: 0x04000475 RID: 1141
	private float originalLightIntensity;

	// Token: 0x04000476 RID: 1142
	private float originalLightRange;

	// Token: 0x04000477 RID: 1143
	private int myChildNumber;

	// Token: 0x04000478 RID: 1144
	private Color originalMaterialColor;

	// Token: 0x04000479 RID: 1145
	internal EnemyFloaterSphereEffect.FloaterSphereEffectState state;

	// Token: 0x0200030E RID: 782
	public enum FloaterSphereEffectState
	{
		// Token: 0x0400287B RID: 10363
		levitate,
		// Token: 0x0400287C RID: 10364
		stop,
		// Token: 0x0400287D RID: 10365
		smash
	}
}
