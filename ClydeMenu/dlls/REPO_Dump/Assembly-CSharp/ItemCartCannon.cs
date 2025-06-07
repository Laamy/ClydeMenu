using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class ItemCartCannon : MonoBehaviour
{
	// Token: 0x06000B5B RID: 2907 RVA: 0x00065628 File Offset: 0x00063828
	private void Start()
	{
		this.movingPieceStartPosition = this.shootAnimationTransform.localPosition;
		this.movingPieceStartPositionRecoil = this.shootNozzleRecoilTransform.localPosition;
		this.cartCannonMain = base.GetComponent<ItemCartCannonMain>();
		this.photonView = base.GetComponent<PhotonView>();
		this.bulletPrefab.SetActive(false);
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.particles = new List<ParticleSystem>(this.animationEventTransform.GetComponentsInChildren<ParticleSystem>());
		this.shootParticles = new List<ParticleSystem>(this.shootParticlesTransform.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x000656CB File Offset: 0x000638CB
	private void StateInactive()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x000656DC File Offset: 0x000638DC
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x000656F0 File Offset: 0x000638F0
	private void StateBuildup()
	{
		if (this.stateStart)
		{
			this.buildup1.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.doRecoil = true;
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
		float num = this.animationCurveShoot.Evaluate(this.cartCannonMain.stateTimer / this.cartCannonMain.stateTimerMax);
		this.shootAnimationTransform.localPosition = new Vector3(this.movingPieceStartPosition.x, this.movingPieceStartPosition.y, this.movingPieceStartPosition.z - this.movingPieceStartPosition.z * num);
		if (this.doRecoil)
		{
			float num2 = this.animationCurveShootRecoil.Evaluate(this.recoilTimer / this.recoilTimerMax);
			this.shootNozzleRecoilTransform.localPosition = new Vector3(this.movingPieceStartPositionRecoil.x, this.movingPieceStartPositionRecoil.y, this.movingPieceStartPositionRecoil.z - 4f + 4f * num2);
			if (this.recoilTimer >= this.recoilTimerMax)
			{
				this.doRecoil = false;
				this.recoilTimer = 0f;
				this.shootNozzleRecoilTransform.localPosition = this.movingPieceStartPositionRecoil;
				return;
			}
			this.recoilTimer += Time.deltaTime;
		}
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x00065895 File Offset: 0x00063A95
	private void StateShooting()
	{
		if (this.stateStart)
		{
			this.ShootLogic();
			this.stateStart = false;
		}
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x000658AC File Offset: 0x00063AAC
	private void StateGoingBack()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		float num = 1f - this.animationCurveShoot.Evaluate(this.cartCannonMain.stateTimer / this.cartCannonMain.stateTimerMax);
		this.shootAnimationTransform.localPosition = new Vector3(this.movingPieceStartPosition.x, this.movingPieceStartPosition.y, this.movingPieceStartPosition.z - this.movingPieceStartPosition.z * num);
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x00065930 File Offset: 0x00063B30
	private void ParticlePlay()
	{
		foreach (ParticleSystem particleSystem in this.particles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x00065980 File Offset: 0x00063B80
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

	// Token: 0x06000B63 RID: 2915 RVA: 0x000659D1 File Offset: 0x00063BD1
	private void Update()
	{
		this.statePrev = this.stateCurrent;
		this.stateCurrent = (int)this.cartCannonMain.stateCurrent;
		if (this.stateCurrent != this.statePrev)
		{
			this.stateStart = true;
		}
		this.StateMachine();
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00065A0C File Offset: 0x00063C0C
	private void ShootLogic()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		Transform muzzle = this.cartCannonMain.muzzle;
		this.itemBattery.RemoveFullBar(1);
		this.physGrabObject.rb.AddForceAtPosition(-muzzle.forward * 20f, muzzle.position, ForceMode.Impulse);
		Vector3 endPosition = muzzle.position + muzzle.forward * 25f;
		bool hit = false;
		bool flag = false;
		Vector3 vector = muzzle.forward;
		float num = 0.1f;
		float num2 = 25f;
		if (num > 0f)
		{
			float angle = Random.Range(0f, num / 2f);
			float angle2 = Random.Range(0f, 360f);
			Vector3 normalized = Vector3.Cross(vector, Random.onUnitSphere).normalized;
			Quaternion rhs = Quaternion.AngleAxis(angle, normalized);
			vector = (Quaternion.AngleAxis(angle2, vector) * rhs * vector).normalized;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(muzzle.position, vector, out raycastHit, num2, SemiFunc.LayerMaskGetVisionObstruct() + LayerMask.GetMask(new string[]
		{
			"Enemy"
		})))
		{
			endPosition = raycastHit.point;
			hit = true;
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			endPosition = muzzle.position + vector * num2;
			hit = true;
		}
		this.ShootBullet(endPosition, hit);
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x00065B6C File Offset: 0x00063D6C
	private void ShootBullet(Vector3 _endPosition, bool _hit)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ShootBulletRPC", RpcTarget.All, new object[]
			{
				_endPosition,
				_hit
			});
			return;
		}
		this.ShootBulletRPC(_endPosition, _hit, default(PhotonMessageInfo));
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x00065BC4 File Offset: 0x00063DC4
	[PunRPC]
	public void ShootBulletRPC(Vector3 _endPosition, bool _hit, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.ShootParticlesPlay();
		Transform muzzle = this.cartCannonMain.muzzle;
		ItemGunBullet component = Object.Instantiate<GameObject>(this.bulletPrefab, muzzle.position, muzzle.rotation).GetComponent<ItemGunBullet>();
		component.hitPosition = _endPosition;
		component.bulletHit = _hit;
		component.shootLineWidthCurve = this.animationCurve;
		this.soundHit.Play(_endPosition, 1f, 1f, 1f, 1f);
		this.shootSound.Play(muzzle.position, 1f, 1f, 1f, 1f);
		component.ActivateAll();
		this.particleScriptExplosion.PlayExplosionSoundMedium(_endPosition);
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00065C7A File Offset: 0x00063E7A
	private void AnimationEvent1()
	{
		this.ParticlePlay();
		this.animationEvent1 = true;
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x00065C89 File Offset: 0x00063E89
	private void AnimationEvent2()
	{
		this.ParticlePlay();
		this.buildup2.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.animationEvent2 = true;
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x00065CC4 File Offset: 0x00063EC4
	private void ShootParticlesPlay()
	{
		foreach (ParticleSystem particleSystem in this.shootParticles)
		{
			if (particleSystem.isPlaying)
			{
				particleSystem.Stop();
			}
			particleSystem.Play();
		}
	}

	// Token: 0x04001268 RID: 4712
	private ItemCartCannonMain cartCannonMain;

	// Token: 0x04001269 RID: 4713
	private PhotonView photonView;

	// Token: 0x0400126A RID: 4714
	public AnimationCurve animationCurve;

	// Token: 0x0400126B RID: 4715
	public GameObject bulletPrefab;

	// Token: 0x0400126C RID: 4716
	private PhysGrabObject physGrabObject;

	// Token: 0x0400126D RID: 4717
	private ItemBattery itemBattery;

	// Token: 0x0400126E RID: 4718
	public Sound soundHit;

	// Token: 0x0400126F RID: 4719
	public AnimationCurve animationCurveShoot;

	// Token: 0x04001270 RID: 4720
	public AnimationCurve animationCurveShootRecoil;

	// Token: 0x04001271 RID: 4721
	public Transform shootAnimationTransform;

	// Token: 0x04001272 RID: 4722
	public Transform shootParticlesTransform;

	// Token: 0x04001273 RID: 4723
	private Vector3 movingPieceStartPosition;

	// Token: 0x04001274 RID: 4724
	private Vector3 movingPieceStartPositionRecoil;

	// Token: 0x04001275 RID: 4725
	public Sound buildup1;

	// Token: 0x04001276 RID: 4726
	public Sound buildup2;

	// Token: 0x04001277 RID: 4727
	public Sound shootSound;

	// Token: 0x04001278 RID: 4728
	public Sound shootSoundGlobal;

	// Token: 0x04001279 RID: 4729
	private bool animationEvent1;

	// Token: 0x0400127A RID: 4730
	private bool animationEvent2;

	// Token: 0x0400127B RID: 4731
	public Transform animationEventTransform;

	// Token: 0x0400127C RID: 4732
	public Transform shootNozzleRecoilTransform;

	// Token: 0x0400127D RID: 4733
	private List<ParticleSystem> particles = new List<ParticleSystem>();

	// Token: 0x0400127E RID: 4734
	private List<ParticleSystem> shootParticles = new List<ParticleSystem>();

	// Token: 0x0400127F RID: 4735
	private bool doRecoil;

	// Token: 0x04001280 RID: 4736
	private float recoilTimer;

	// Token: 0x04001281 RID: 4737
	private float recoilTimerMax = 0.4f;

	// Token: 0x04001282 RID: 4738
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04001283 RID: 4739
	private int statePrev;

	// Token: 0x04001284 RID: 4740
	private int stateCurrent;

	// Token: 0x04001285 RID: 4741
	private bool stateStart;
}
