using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class ValuableEgg : Trap
{
	// Token: 0x0600158F RID: 5519 RVA: 0x000BEA67 File Offset: 0x000BCC67
	protected override void Start()
	{
		base.Start();
		this.originalSize = this.objectTransform.localScale;
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x000BEA98 File Offset: 0x000BCC98
	protected override void Update()
	{
		this.screamSound.PlayLoop(this.currentState == ValuableEgg.EggState.State5, 1f, 1f, 1f);
		if (this.currentState == ValuableEgg.EggState.Explode)
		{
			this.Explode();
		}
		if (this.currentState == ValuableEgg.EggState.State5)
		{
			this.PlayEffects();
			this.AnimateArmsAndLegs();
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.currentState == ValuableEgg.EggState.State5)
		{
			this.explosionTimer += Time.deltaTime;
			if (this.explosionTimer >= this.explosionTime)
			{
				this.SetNextState();
			}
			this.enemyInvestigate = true;
		}
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x000BEB2C File Offset: 0x000BCD2C
	protected void FixedUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		List<PhysGrabber> playerGrabbing = this.physGrabObject.playerGrabbing;
		bool flag = false;
		using (List<PhysGrabber>.Enumerator enumerator = playerGrabbing.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isRotating)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			Quaternion turnX = Quaternion.Euler(0f, -180f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			this.physGrabObject.OverrideTorqueStrength(2f + this.physGrabObject.massOriginal, 0.1f);
		}
		if (this.currentState == ValuableEgg.EggState.State5)
		{
			this.ScreamShake();
		}
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x000BEC04 File Offset: 0x000BCE04
	[PunRPC]
	public void SetStateRPC(ValuableEgg.EggState _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x000BEC18 File Offset: 0x000BCE18
	private void SetState(ValuableEgg.EggState _state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(_state, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			_state
		});
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x000BEC68 File Offset: 0x000BCE68
	private void SetNextState()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		switch (this.currentState)
		{
		case ValuableEgg.EggState.State1:
			this.SetState(ValuableEgg.EggState.State2);
			return;
		case ValuableEgg.EggState.State2:
			this.SetState(ValuableEgg.EggState.State3);
			return;
		case ValuableEgg.EggState.State3:
			this.SetState(ValuableEgg.EggState.State4);
			return;
		case ValuableEgg.EggState.State4:
			this.SetState(ValuableEgg.EggState.State5);
			return;
		case ValuableEgg.EggState.State5:
			this.SetState(ValuableEgg.EggState.Explode);
			return;
		default:
			return;
		}
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x000BECC8 File Offset: 0x000BCEC8
	public void Explode()
	{
		this.particleScriptExplosion.Spawn(this.meshRenderer.transform.position, 1.5f, 100, 300, 1f, false, false, 1f);
		this.physGrabObject.dead = true;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x000BED20 File Offset: 0x000BCF20
	public void Crack()
	{
		if (this.currentState == ValuableEgg.EggState.State5)
		{
			return;
		}
		this.crackSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
		{
			physGrabber.OverrideGrabRelease();
		}
		ParticleSystem.MainModule main = this.breakParticle.main;
		switch (this.currentState)
		{
		case ValuableEgg.EggState.State1:
			this.objectTransform.localScale = this.originalSize * 0.9f;
			this.meshRenderer.material = this.state2Material;
			break;
		case ValuableEgg.EggState.State2:
			this.objectTransform.localScale = this.originalSize * 0.7f;
			this.meshRenderer.material = this.state3Material;
			main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.6f, 0.6f, 0f));
			break;
		case ValuableEgg.EggState.State3:
			this.objectTransform.localScale = this.originalSize * 0.5f;
			this.meshRenderer.material = this.state4Material;
			main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.7f, 0f));
			break;
		case ValuableEgg.EggState.State4:
			this.objectTransform.localScale = this.originalSize * 0.25f;
			this.meshRenderer.material = this.state5Material;
			main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.3f, 0f));
			this.EnableLimbs();
			break;
		}
		this.breakParticle.Play();
		this.CameraShake();
		this.SetNextState();
		this.SetMass();
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x000BEF18 File Offset: 0x000BD118
	private void SetMass()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		switch (this.currentState)
		{
		case ValuableEgg.EggState.State1:
			this.physGrabObject.OverrideMass(2f, 0.1f);
			this.physGrabObject.massOriginal = 2f;
			return;
		case ValuableEgg.EggState.State2:
			this.physGrabObject.OverrideMass(1.75f, 0.1f);
			this.physGrabObject.massOriginal = 1.75f;
			return;
		case ValuableEgg.EggState.State3:
			this.physGrabObject.OverrideMass(1.25f, 0.1f);
			this.physGrabObject.massOriginal = 1.25f;
			return;
		case ValuableEgg.EggState.State4:
			this.physGrabObject.OverrideMass(1f, 0.1f);
			this.physGrabObject.massOriginal = 1f;
			return;
		case ValuableEgg.EggState.State5:
			this.physGrabObject.OverrideMass(0.5f, 0.1f);
			this.physGrabObject.massOriginal = 0.5f;
			return;
		default:
			return;
		}
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x000BF00C File Offset: 0x000BD20C
	private void EnableLimbs()
	{
		this.armL.gameObject.SetActive(true);
		this.armR.gameObject.SetActive(true);
		this.legL.gameObject.SetActive(true);
		this.legR.gameObject.SetActive(true);
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x000BF060 File Offset: 0x000BD260
	private void ScreamShake()
	{
		this.joltTimer -= Time.deltaTime;
		if (this.joltTimer <= 0f)
		{
			Vector3 torque = Random.insideUnitSphere.normalized * this.torqueMultiplier;
			this.rb.AddTorque(torque, ForceMode.Impulse);
			this.joltTimer = Random.Range(this.joltIntervalRange.x, this.joltIntervalRange.y);
		}
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x000BF0D4 File Offset: 0x000BD2D4
	private void CameraShake()
	{
		GameDirector.instance.CameraShake.ShakeDistance(this.cameraShakeStrength, 3f, 8f, base.transform.position, this.cameraShakeTime);
		GameDirector.instance.CameraImpact.ShakeDistance(this.cameraShakeStrength, this.cameraShakeBounds.x, this.cameraShakeBounds.y, base.transform.position, this.cameraShakeTime);
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x000BF14D File Offset: 0x000BD34D
	private void PlayEffects()
	{
		if (this.effectsOn)
		{
			return;
		}
		this.pointLight.enabled = true;
		this.effectsOn = true;
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x000BF16C File Offset: 0x000BD36C
	private void AnimateArmsAndLegs()
	{
		float x = Mathf.Lerp(this.armRange.x, this.armRange.y, Mathf.PingPong(Time.time * 8f, 1f));
		float x2 = Mathf.Lerp(this.armRange.x, this.armRange.y, Mathf.PingPong(Time.time * 8f + 0.5f, 1f));
		this.armL.localRotation = Quaternion.Euler(x, 0f, 0f);
		this.armR.localRotation = Quaternion.Euler(x2, 0f, 0f);
		float x3 = Mathf.Lerp(this.legRange.x, this.legRange.y, Mathf.PingPong(Time.time * 8f, 1f));
		float x4 = Mathf.Lerp(this.legRange.x, this.legRange.y, Mathf.PingPong(Time.time * 8f + 0.5f, 1f));
		this.legL.localRotation = Quaternion.Euler(x3, 0f, 0f);
		this.legR.localRotation = Quaternion.Euler(x4, 0f, 0f);
	}

	// Token: 0x0400257D RID: 9597
	[Header("Times")]
	public float explosionTime = 3f;

	// Token: 0x0400257E RID: 9598
	public float crackRestTime = 0.5f;

	// Token: 0x0400257F RID: 9599
	public Vector2 joltIntervalRange = new Vector2(0.2f, 1f);

	// Token: 0x04002580 RID: 9600
	[Header("Physics")]
	public float torqueMultiplier = 0.5f;

	// Token: 0x04002581 RID: 9601
	[Header("Camera Shake")]
	public float cameraShakeTime = 0.2f;

	// Token: 0x04002582 RID: 9602
	public float cameraShakeStrength = 3f;

	// Token: 0x04002583 RID: 9603
	public Vector2 cameraShakeBounds = new Vector2(1.5f, 5f);

	// Token: 0x04002584 RID: 9604
	[Header("Arm & Leg Animation")]
	public Vector2 armRange = new Vector2(-45f, 45f);

	// Token: 0x04002585 RID: 9605
	public Vector2 legRange = new Vector2(-25f, 20f);

	// Token: 0x04002586 RID: 9606
	[Header("Transforms")]
	public Transform objectTransform;

	// Token: 0x04002587 RID: 9607
	public Transform armL;

	// Token: 0x04002588 RID: 9608
	public Transform armR;

	// Token: 0x04002589 RID: 9609
	public Transform legL;

	// Token: 0x0400258A RID: 9610
	public Transform legR;

	// Token: 0x0400258B RID: 9611
	[Header("Materials")]
	public Material state2Material;

	// Token: 0x0400258C RID: 9612
	public Material state3Material;

	// Token: 0x0400258D RID: 9613
	public Material state4Material;

	// Token: 0x0400258E RID: 9614
	public Material state5Material;

	// Token: 0x0400258F RID: 9615
	[Header("Mesh renderer")]
	public MeshRenderer meshRenderer;

	// Token: 0x04002590 RID: 9616
	[Header("Particles")]
	public ParticleSystem breakParticle;

	// Token: 0x04002591 RID: 9617
	[Space]
	public Light pointLight;

	// Token: 0x04002592 RID: 9618
	[Header("Sounds")]
	public Sound crackSound;

	// Token: 0x04002593 RID: 9619
	public Sound screamSound;

	// Token: 0x04002594 RID: 9620
	public Sound explosionSound;

	// Token: 0x04002595 RID: 9621
	private ValuableEgg.EggState currentState;

	// Token: 0x04002596 RID: 9622
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002597 RID: 9623
	private Rigidbody rb;

	// Token: 0x04002598 RID: 9624
	private Vector3 originalSize;

	// Token: 0x04002599 RID: 9625
	private float explosionTimer;

	// Token: 0x0400259A RID: 9626
	private float joltTimer;

	// Token: 0x0400259B RID: 9627
	private bool effectsOn;

	// Token: 0x02000416 RID: 1046
	public enum EggState
	{
		// Token: 0x04002DBF RID: 11711
		State1,
		// Token: 0x04002DC0 RID: 11712
		State2,
		// Token: 0x04002DC1 RID: 11713
		State3,
		// Token: 0x04002DC2 RID: 11714
		State4,
		// Token: 0x04002DC3 RID: 11715
		State5,
		// Token: 0x04002DC4 RID: 11716
		Explode
	}
}
