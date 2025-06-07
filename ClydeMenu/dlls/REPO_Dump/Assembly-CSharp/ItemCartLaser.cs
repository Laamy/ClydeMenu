using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class ItemCartLaser : MonoBehaviour
{
	// Token: 0x06000B78 RID: 2936 RVA: 0x00066164 File Offset: 0x00064364
	private void Start()
	{
		this.cartCannonMain = base.GetComponent<ItemCartCannonMain>();
		this.photonView = base.GetComponent<PhotonView>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.physGrabObjectImpactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.shootParticles = new List<ParticleSystem>(this.shootParticlesTransform.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x000661CF File Offset: 0x000643CF
	private void StateInactive()
	{
		if (this.stateStart)
		{
			this.ResetHatches();
			this.stateStart = false;
		}
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x000661E6 File Offset: 0x000643E6
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.ResetHatches();
			this.stateStart = false;
		}
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00066200 File Offset: 0x00064400
	private void StateBuildup()
	{
		if (this.stateStart)
		{
			this.ResetHatches();
			this.soundBuildupStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.animationEvent2 = false;
			this.animationEvent1 = false;
			this.stateStart = false;
		}
		if (this.cartCannonMain.stateTimer > 0.08f && !this.animationEvent1)
		{
			this.AnimationEvent1();
		}
		if (this.cartCannonMain.stateTimer > 0.5f && !this.animationEvent2)
		{
			this.AnimationEvent2();
		}
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x0006629C File Offset: 0x0006449C
	private void StateShooting()
	{
		if (this.stateStart)
		{
			this.ResetHatches();
			this.ShootParticles(true);
			this.stateStart = false;
		}
		Vector3 endPosition = this.muzzleTransform.position + this.muzzleTransform.forward * 15f;
		bool isHitting = false;
		RaycastHit raycastHit;
		if (Physics.Raycast(this.muzzleTransform.position, this.muzzleTransform.forward, out raycastHit, 15f, SemiFunc.LayerMaskGetVisionObstruct()))
		{
			endPosition = raycastHit.point;
			isHitting = true;
		}
		this.semiLaser.LaserActive(this.muzzleTransform.position, endPosition, isHitting);
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00066340 File Offset: 0x00064540
	private void StateGoingBack()
	{
		if (this.stateStart)
		{
			this.ResetHatches();
			this.itemBattery.RemoveFullBar(1);
			this.soundGoingBack.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.stateStart = false;
			this.HatchWarningLightsTurnOn();
		}
		float t = this.animationCurveHatchType1.Evaluate(this.cartCannonMain.stateTimer / this.cartCannonMain.stateTimerMax);
		float t2 = this.animationCurveHatchType2.Evaluate(this.cartCannonMain.stateTimer / this.cartCannonMain.stateTimerMax);
		float t3 = this.animationCurveWarningLight.Evaluate(this.cartCannonMain.stateTimer / this.cartCannonMain.stateTimerMax);
		this.hatch1Left.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 25f, t));
		this.hatch1Right.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -25f, t));
		this.hatch2Left.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -120f, t2));
		this.hatch2Right.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 120f, t2));
		this.warningLight1Light.intensity = Mathf.Lerp(0f, 4f, t3);
		this.warningLight2Light.intensity = Mathf.Lerp(0f, 4f, t3);
		this.warningLight1MeshRenderer.material.SetColor("_EmissionColor", Color.Lerp(new Color(0f, 0f, 0f), new Color(1f, 0f, 0f), t3));
		this.warningLight2MeshRenderer.material.SetColor("_EmissionColor", Color.Lerp(new Color(0f, 0f, 0f), new Color(1f, 0f, 0f), t3));
		if (this.cartCannonMain.stateTimer > 0.2f && !this.hatchLeft.isPlaying)
		{
			this.hatchLeft.Play();
			this.hatchRight.Play();
		}
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x000665A4 File Offset: 0x000647A4
	private void ParticlePlay()
	{
		foreach (ParticleSystem particleSystem in this.particles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x000665F4 File Offset: 0x000647F4
	private void StateMachine()
	{
		switch (this.stateCurrent)
		{
		case 0:
			this.StateInactive();
			return;
		case 1:
			this.StateActive();
			return;
		case 2:
			this.StateBuildup();
			return;
		case 3:
			this.StateShooting();
			return;
		case 4:
			this.StateGoingBack();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00066648 File Offset: 0x00064848
	private void Update()
	{
		this.statePrev = this.stateCurrent;
		this.stateCurrent = (int)this.cartCannonMain.stateCurrent;
		if (this.stateCurrent != this.statePrev)
		{
			this.stateStart = true;
		}
		bool playing = this.stateCurrent == 2;
		float pitchMultiplier = Mathf.Lerp(0.8f, 1.2f, this.cartCannonMain.stateTimer / this.cartCannonMain.stateTimerMax);
		this.soundBuildupLoop.PlayLoop(playing, 1f, 1f, pitchMultiplier);
		this.StateMachine();
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x000666D8 File Offset: 0x000648D8
	private void ResetHatches()
	{
		this.hatch1Left.localRotation = Quaternion.Euler(0f, 0f, 0f);
		this.hatch1Right.localRotation = Quaternion.Euler(0f, 0f, 0f);
		this.hatch2Left.localRotation = Quaternion.Euler(0f, 0f, 0f);
		this.hatch2Right.localRotation = Quaternion.Euler(0f, 0f, 0f);
		this.HatchWarningLightsTurnOff();
		this.ShootParticles(false);
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00066770 File Offset: 0x00064970
	private void HatchWarningLightsTurnOn()
	{
		this.warningLight1Light.enabled = true;
		this.warningLight2Light.enabled = true;
		this.warningLight1MeshRenderer.material.SetColor("_EmissionColor", new Color(1f, 0f, 0f));
		this.warningLight2MeshRenderer.material.SetColor("_EmissionColor", new Color(1f, 0f, 0f));
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x000667E8 File Offset: 0x000649E8
	private void HatchWarningLightsTurnOff()
	{
		this.warningLight1Light.enabled = false;
		this.warningLight2Light.enabled = false;
		this.warningLight1MeshRenderer.material.SetColor("_EmissionColor", new Color(0f, 0f, 0f));
		this.warningLight2MeshRenderer.material.SetColor("_EmissionColor", new Color(0f, 0f, 0f));
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0006685F File Offset: 0x00064A5F
	private void AnimationEvent1()
	{
		this.ParticlePlay();
		this.animationEvent1 = true;
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0006686E File Offset: 0x00064A6E
	private void AnimationEvent2()
	{
		this.ParticlePlay();
		this.animationEvent2 = true;
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00066880 File Offset: 0x00064A80
	private void ShootParticles(bool _play)
	{
		foreach (ParticleSystem particleSystem in this.shootParticles)
		{
			if (_play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x04001290 RID: 4752
	public SemiLaser semiLaser;

	// Token: 0x04001291 RID: 4753
	public Transform muzzleTransform;

	// Token: 0x04001292 RID: 4754
	private ItemCartCannonMain cartCannonMain;

	// Token: 0x04001293 RID: 4755
	private PhotonView photonView;

	// Token: 0x04001294 RID: 4756
	private PhysGrabObject physGrabObject;

	// Token: 0x04001295 RID: 4757
	private ItemBattery itemBattery;

	// Token: 0x04001296 RID: 4758
	public Sound soundHit;

	// Token: 0x04001297 RID: 4759
	public AnimationCurve animationCurveBuildup;

	// Token: 0x04001298 RID: 4760
	public AnimationCurve animationCurveHatchType1;

	// Token: 0x04001299 RID: 4761
	public AnimationCurve animationCurveHatchType2;

	// Token: 0x0400129A RID: 4762
	public AnimationCurve animationCurveWarningLight;

	// Token: 0x0400129B RID: 4763
	private Vector3 movingPieceStartPosition;

	// Token: 0x0400129C RID: 4764
	public Sound soundBuildupStart;

	// Token: 0x0400129D RID: 4765
	public Sound soundBuildupLoop;

	// Token: 0x0400129E RID: 4766
	public Sound soundGoingBack;

	// Token: 0x0400129F RID: 4767
	private PhysGrabObjectImpactDetector physGrabObjectImpactDetector;

	// Token: 0x040012A0 RID: 4768
	private bool animationEvent1;

	// Token: 0x040012A1 RID: 4769
	private bool animationEvent2;

	// Token: 0x040012A2 RID: 4770
	public Transform animationEventTransform;

	// Token: 0x040012A3 RID: 4771
	public Transform shootParticlesTransform;

	// Token: 0x040012A4 RID: 4772
	private List<ParticleSystem> particles = new List<ParticleSystem>();

	// Token: 0x040012A5 RID: 4773
	private List<ParticleSystem> shootParticles = new List<ParticleSystem>();

	// Token: 0x040012A6 RID: 4774
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040012A7 RID: 4775
	private int statePrev;

	// Token: 0x040012A8 RID: 4776
	internal int stateCurrent;

	// Token: 0x040012A9 RID: 4777
	private bool stateStart;

	// Token: 0x040012AA RID: 4778
	public HurtCollider laserHurtCollider;

	// Token: 0x040012AB RID: 4779
	public Transform hatch1Right;

	// Token: 0x040012AC RID: 4780
	public Transform hatch1Left;

	// Token: 0x040012AD RID: 4781
	public Transform hatch2Right;

	// Token: 0x040012AE RID: 4782
	public Transform hatch2Left;

	// Token: 0x040012AF RID: 4783
	public MeshRenderer warningLight1MeshRenderer;

	// Token: 0x040012B0 RID: 4784
	public MeshRenderer warningLight2MeshRenderer;

	// Token: 0x040012B1 RID: 4785
	public Light warningLight1Light;

	// Token: 0x040012B2 RID: 4786
	public Light warningLight2Light;

	// Token: 0x040012B3 RID: 4787
	public ParticleSystem hatchLeft;

	// Token: 0x040012B4 RID: 4788
	public ParticleSystem hatchRight;
}
