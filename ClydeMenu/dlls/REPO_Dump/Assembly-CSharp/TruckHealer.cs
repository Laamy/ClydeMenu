using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class TruckHealer : MonoBehaviour
{
	// Token: 0x060008BB RID: 2235 RVA: 0x00054568 File Offset: 0x00052768
	private void Start()
	{
		this.healSphereRenderer = this.healSphere.GetComponent<MeshRenderer>();
		this.zRotationHatch1Open = this.hatch1Transform.localEulerAngles.z;
		this.zRotationHatch2Open = this.hatch2Transform.localEulerAngles.z;
		this.hatch1Transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		this.hatch2Transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		this.lightIntensityOriginal = this.healerLight.intensity;
		this.healerLight.intensity = 0f;
		this.healerLight.enabled = false;
		this.healSphereSizeOriginal = this.healSphere.localScale.x;
		this.healSphere.localScale = new Vector3(0f, 0f, 0f);
		this.healSphere.gameObject.SetActive(false);
		this.healParticlesList.AddRange(this.healParticles.GetComponentsInChildren<ParticleSystem>());
		TruckHealer.instance = this;
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0005467F File Offset: 0x0005287F
	private void StateClosed()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		if (!this.allHealingDone && RoundDirector.instance.allExtractionPointsCompleted)
		{
			this.StateUpdate(TruckHealer.State.Opening);
		}
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x000546AC File Offset: 0x000528AC
	private void PlayHealParticles()
	{
		foreach (ParticleSystem particleSystem in this.healParticlesList)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x000546FC File Offset: 0x000528FC
	private void StateOpening()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.hatchAnimEval = 0f;
			this.healerLight.enabled = true;
			this.swirlParticles.Play();
			this.hatchOpenedEffect = false;
			this.healSphere.gameObject.SetActive(true);
			this.soundOpen.Play(this.healerBeamOrigin.position, 1f, 1f, 1f, 1f);
		}
		if (this.hatchAnimEval < 1f)
		{
			this.hatchAnimEval += Time.deltaTime * 2f;
			if (this.hatchAnimEval > 0.8f && !this.hatchOpenedEffect)
			{
				this.hatchOpenedEffect = true;
				SemiFunc.CameraShakeImpactDistance(base.transform.position, 4f, 0.1f, 6f, 15f);
				this.partSmokeCeiling.Play();
				this.PartSmokeCeilingPoof.Play();
				this.soundSlam.Play(this.healerBeamOrigin.position, 1f, 1f, 1f, 1f);
			}
			if (this.healerLight.intensity < this.lightIntensityOriginal - 0.01f)
			{
				this.healerLight.intensity = Mathf.Lerp(this.healerLight.intensity, this.lightIntensityOriginal, this.hatchCurve.Evaluate(this.hatchAnimEval));
			}
			if (this.healSphere.localScale.x < this.healSphereSizeOriginal - 0.01f)
			{
				this.healSphere.localScale = new Vector3(Mathf.Lerp(0f, this.healSphereSizeOriginal, this.hatchCurve.Evaluate(this.hatchAnimEval)), Mathf.Lerp(0f, this.healSphereSizeOriginal, this.hatchCurve.Evaluate(this.hatchAnimEval)), Mathf.Lerp(0f, this.healSphereSizeOriginal, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			}
			else
			{
				this.healSphere.localScale = new Vector3(this.healSphereSizeOriginal, this.healSphereSizeOriginal, this.healSphereSizeOriginal);
			}
			this.hatch1Transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, this.zRotationHatch1Open, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			this.hatch2Transform.localEulerAngles = new Vector3(0f, 180f, Mathf.Lerp(0f, this.zRotationHatch2Open, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			return;
		}
		this.StateUpdate(TruckHealer.State.Open);
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x000549A4 File Offset: 0x00052BA4
	private void StateOpen()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		if (SemiFunc.FPSImpulse5())
		{
			List<PlayerAvatar> list = SemiFunc.PlayerGetAll();
			int count = list.Count;
			int num = 0;
			using (List<PlayerAvatar>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.finalHeal)
					{
						num++;
					}
				}
			}
			if (num >= count)
			{
				this.allHealingDone = true;
				this.StateUpdate(TruckHealer.State.Closing);
			}
		}
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00054A2C File Offset: 0x00052C2C
	private void StateClosing()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.swirlParticles.Stop();
			this.hatchClosedEffect = false;
			this.hatchAnimEval = 0f;
			this.soundClose.Play(this.healerBeamOrigin.position, 1f, 1f, 1f, 1f);
		}
		if (this.hatchAnimEval < 1f)
		{
			this.hatchAnimEval += Time.deltaTime * 2f;
			if (this.healerLight.intensity > 0.01f)
			{
				this.healerLight.intensity = Mathf.Lerp(this.healerLight.intensity, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval));
			}
			else
			{
				this.healerLight.enabled = false;
			}
			if (this.hatchAnimEval > 0.8f && !this.hatchClosedEffect)
			{
				this.hatchClosedEffect = true;
				SemiFunc.CameraShakeImpactDistance(base.transform.position, 4f, 0.1f, 6f, 15f);
				this.partSmokeCeiling.Play();
				this.PartSmokeCeilingPoof.Play();
				this.soundSlam.Play(this.healerBeamOrigin.position, 1f, 1f, 1f, 1f);
			}
			if (this.healSphere.localScale.x > 0.01f)
			{
				this.healSphere.localScale = new Vector3(Mathf.Lerp(this.healSphereSizeOriginal, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)), Mathf.Lerp(this.healSphereSizeOriginal, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)), Mathf.Lerp(this.healSphereSizeOriginal, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			}
			else
			{
				this.healSphere.localScale = new Vector3(0f, 0f, 0f);
				this.healSphere.gameObject.SetActive(false);
			}
			this.hatch1Transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(this.zRotationHatch1Open, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			this.hatch2Transform.localEulerAngles = new Vector3(0f, 180f, Mathf.Lerp(this.zRotationHatch2Open, 0f, this.hatchCurve.Evaluate(this.hatchAnimEval)));
			return;
		}
		this.StateUpdate(TruckHealer.State.Closed);
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00054CC4 File Offset: 0x00052EC4
	private void StateMachine()
	{
		switch (this.currentState)
		{
		case TruckHealer.State.Closed:
			this.StateClosed();
			return;
		case TruckHealer.State.Opening:
			this.StateOpening();
			return;
		case TruckHealer.State.Open:
			this.StateOpen();
			return;
		case TruckHealer.State.Closing:
			this.StateClosing();
			return;
		default:
			return;
		}
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x00054D0C File Offset: 0x00052F0C
	private void Update()
	{
		bool playing = this.currentState == TruckHealer.State.Open;
		this.soundLoop.PlayLoop(playing, 2f, 2f, 1f);
		this.StateMachine();
		this.ScrollSphereTexture();
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x00054D4A File Offset: 0x00052F4A
	private void StateUpdate(TruckHealer.State _newState)
	{
		if (this.currentState != _newState)
		{
			this.currentState = _newState;
			this.stateStart = true;
		}
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x00054D64 File Offset: 0x00052F64
	private void ScrollSphereTexture()
	{
		if (!this.healSphereRenderer.gameObject.activeSelf)
		{
			return;
		}
		this.healSphereRenderer.material.mainTextureOffset = new Vector2(this.healSphereRenderer.material.mainTextureOffset.x, this.healSphereRenderer.material.mainTextureOffset.y + Time.deltaTime * 0.1f);
		this.healSpherePulseParent.localScale = new Vector3(1f + Mathf.Sin(Time.time * 5f) * 0.1f, 1f + Mathf.Sin(Time.time * 5f) * 0.1f, 1f + Mathf.Sin(Time.time * 5f) * 0.1f);
		this.healSpherePulseParent.localEulerAngles = new Vector3(0f, Time.time * 200f, 0f);
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00054E58 File Offset: 0x00053058
	public void Heal(PlayerAvatar _playerAvatar)
	{
		if (this.currentState != TruckHealer.State.Open)
		{
			return;
		}
		TruckHealerLine component = Object.Instantiate<GameObject>(this.healerBeamPrefab, this.healerBeamOrigin.position, Quaternion.identity).GetComponent<TruckHealerLine>();
		if (!_playerAvatar.isLocal)
		{
			component.lineTarget = _playerAvatar.playerAvatarVisuals.attachPointTopHeadMiddle;
		}
		else
		{
			component.lineTarget = _playerAvatar.localCameraTransform;
		}
		this.PlayHealParticles();
	}

	// Token: 0x04000FEC RID: 4076
	public Transform hatch1Transform;

	// Token: 0x04000FED RID: 4077
	public Transform hatch2Transform;

	// Token: 0x04000FEE RID: 4078
	public Light healerLight;

	// Token: 0x04000FEF RID: 4079
	public AnimationCurve hatchCurve;

	// Token: 0x04000FF0 RID: 4080
	public Transform healSphere;

	// Token: 0x04000FF1 RID: 4081
	public Transform healSpherePulseParent;

	// Token: 0x04000FF2 RID: 4082
	public ParticleSystem swirlParticles;

	// Token: 0x04000FF3 RID: 4083
	private MeshRenderer healSphereRenderer;

	// Token: 0x04000FF4 RID: 4084
	private float hatchAnimEval;

	// Token: 0x04000FF5 RID: 4085
	private float zRotationHatch1Open;

	// Token: 0x04000FF6 RID: 4086
	private float zRotationHatch2Open;

	// Token: 0x04000FF7 RID: 4087
	private float lightIntensityOriginal;

	// Token: 0x04000FF8 RID: 4088
	private float healSphereSizeOriginal;

	// Token: 0x04000FF9 RID: 4089
	private bool hatchClosedEffect;

	// Token: 0x04000FFA RID: 4090
	private bool hatchOpenedEffect;

	// Token: 0x04000FFB RID: 4091
	public ParticleSystem partSmokeCeiling;

	// Token: 0x04000FFC RID: 4092
	public ParticleSystem PartSmokeCeilingPoof;

	// Token: 0x04000FFD RID: 4093
	public Transform healParticles;

	// Token: 0x04000FFE RID: 4094
	private List<ParticleSystem> healParticlesList = new List<ParticleSystem>();

	// Token: 0x04000FFF RID: 4095
	public GameObject healerBeamPrefab;

	// Token: 0x04001000 RID: 4096
	public Transform healerBeamOrigin;

	// Token: 0x04001001 RID: 4097
	public Sound soundOpen;

	// Token: 0x04001002 RID: 4098
	public Sound soundClose;

	// Token: 0x04001003 RID: 4099
	public Sound soundSlam;

	// Token: 0x04001004 RID: 4100
	public Sound soundLoop;

	// Token: 0x04001005 RID: 4101
	private bool allHealingDone;

	// Token: 0x04001006 RID: 4102
	public static TruckHealer instance;

	// Token: 0x04001007 RID: 4103
	internal TruckHealer.State currentState;

	// Token: 0x04001008 RID: 4104
	private bool stateStart = true;

	// Token: 0x02000363 RID: 867
	public enum State
	{
		// Token: 0x04002A86 RID: 10886
		Closed,
		// Token: 0x04002A87 RID: 10887
		Opening,
		// Token: 0x04002A88 RID: 10888
		Open,
		// Token: 0x04002A89 RID: 10889
		Closing
	}
}
