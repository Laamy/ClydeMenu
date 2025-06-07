using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002A7 RID: 679
public class ClownTrap : Trap
{
	// Token: 0x0600153C RID: 5436 RVA: 0x000BB4F0 File Offset: 0x000B96F0
	protected override void Start()
	{
		base.Start();
		this.initialClownRotation = this.Body.transform.localRotation;
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.noseMesh = this.Nose.GetComponent<MeshRenderer>();
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x000BB544 File Offset: 0x000B9744
	protected override void Update()
	{
		base.Update();
		if (this.trapActive)
		{
			this.enemyInvestigateRange = 15f;
			if (this.HeadSpinOneShotActive)
			{
				if (this.HeadSpinLerp == 0f)
				{
					this.HeadSpin.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
					this.enemyInvestigate = true;
				}
				this.HeadSpinLerp += this.HeadSpinSpeed * Time.deltaTime;
				if (this.HeadSpinLerp >= 1f)
				{
					this.HeadSpinLerp = 0f;
					if (this.WarningCount > 0)
					{
						this.WarningCount--;
						int warningCount = this.WarningCount;
						if (warningCount != 0)
						{
							if (warningCount == 1)
							{
								this.previousAudioSource = this.WarningVO1.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
							}
						}
						else
						{
							if (this.previousAudioSource != null)
							{
								this.previousAudioSource.Stop();
							}
							this.previousAudioSource = this.WarningVO2.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
						}
						this.HeadSpinOneShotActive = false;
						this.HeadSpinDoneLogic();
					}
				}
			}
			this.Head.transform.localEulerAngles = new Vector3(0f, this.HeadSpinCurve.Evaluate(this.HeadSpinLerp) * this.HeadSpinIntensity, 0f);
			if (this.ArmRaiseActive)
			{
				if (this.ArmRaiseLerp == 0f)
				{
					this.ArmRaise.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
				}
				this.ArmRaiseLerp += this.ArmRaiseSpeed * Time.deltaTime;
				if (this.ArmRaiseLerp >= 1f)
				{
					this.ArmRaiseLerp = 0f;
					this.HeadSpinOneShotActive = true;
					if (this.WarningCount > 0)
					{
						this.ArmRaiseActive = false;
					}
				}
			}
			this.Arms.transform.localEulerAngles = new Vector3(0f, 0f, this.ArmRaiseCurve.Evaluate(this.ArmRaiseLerp) * this.ArmRaiseIntensity);
			if (this.NoseSqueezeActive)
			{
				this.NoseSqueezeLerp += this.NoseSqueezeSpeed * Time.deltaTime;
				this.enemyInvestigate = true;
				if (this.NoseSqueezeLerp >= 1f)
				{
					this.NoseSqueezeLerp = 0f;
					if (!this.CountDownActive)
					{
						this.noseMesh.material.DisableKeyword("_EMISSION");
					}
					this.NoseSqueezeActive = false;
				}
			}
			this.Nose.transform.localScale = new Vector3(1f + this.NoseSqueezeCurve.Evaluate(this.NoseSqueezeLerp) * this.NoseSqueezeIntensity, 1f + this.NoseSqueezeCurve.Evaluate(this.NoseSqueezeLerp) * this.NoseSqueezeIntensity, 1f + this.NoseSqueezeCurve.Evaluate(this.NoseSqueezeLerp) * this.NoseSqueezeIntensity);
			if (this.CountDownActive)
			{
				float num = (float)(this.ExplosionCountDown / 50);
				float num2 = (float)(this.ExplosionCountDown / 10);
				float num3 = num * Mathf.Sin(Time.time * num2);
				float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
				this.Body.transform.localRotation = this.initialClownRotation * Quaternion.Euler(0f, num3, z);
				this.Body.transform.localPosition = new Vector3(this.Body.transform.localPosition.x, this.Body.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.Body.transform.localPosition.z);
			}
		}
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x000BB933 File Offset: 0x000B9B33
	private void FixedUpdate()
	{
		if (this.CountDownActive)
		{
			this.ExplosionCountDown++;
		}
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x000BB94C File Offset: 0x000B9B4C
	public void TrapStop()
	{
		this.trapActive = false;
		if (this.previousAudioSource != null)
		{
			this.previousAudioSource.Stop();
		}
		this.particleScriptExplosion.Spawn(this.Center.position, 1.5f, 100, 300, 1f, false, false, 1f);
		this.physgrabobject.dead = true;
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x000BB9B4 File Offset: 0x000B9BB4
	private void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.NoseSqeak.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.noseMesh.material.EnableKeyword("_EMISSION");
			this.NoseSqueezeActive = true;
			this.ArmRaiseActive = true;
			this.trapActive = true;
			this.trapTriggered = true;
			if (this.WarningCount <= 0)
			{
				if (this.previousAudioSource != null)
				{
					this.previousAudioSource.Stop();
				}
				this.previousAudioSource = this.GonnaBlowVO.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
				this.ArmRaiseActive = true;
				this.noseMesh.material.EnableKeyword("_EMISSION");
				this.clownTimer.Invoke();
				this.CountDownActive = true;
			}
		}
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x000BBAA8 File Offset: 0x000B9CA8
	private void TouchNoseLogic()
	{
		this.TrapActivate();
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x000BBAB0 File Offset: 0x000B9CB0
	public void TouchNose()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.TouchNoseLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("TouchNoseRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x000BBAE2 File Offset: 0x000B9CE2
	[PunRPC]
	private void TouchNoseRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.TouchNoseLogic();
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x000BBAF3 File Offset: 0x000B9CF3
	private void HeadSpinDoneLogic()
	{
		this.trapTriggered = false;
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x000BBAFC File Offset: 0x000B9CFC
	private void HeadSpinDone()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.HeadSpinDoneLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("HeadSpinDoneRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x000BBB2E File Offset: 0x000B9D2E
	[PunRPC]
	private void HeadSpinDoneRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.HeadSpinDoneLogic();
	}

	// Token: 0x0400249D RID: 9373
	public UnityEvent clownTimer;

	// Token: 0x0400249E RID: 9374
	public Transform Center;

	// Token: 0x0400249F RID: 9375
	private PhysGrabObject physgrabobject;

	// Token: 0x040024A0 RID: 9376
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040024A1 RID: 9377
	private MeshRenderer noseMesh;

	// Token: 0x040024A2 RID: 9378
	[Space]
	[Header("Clown Components")]
	public GameObject Body;

	// Token: 0x040024A3 RID: 9379
	public GameObject Arms;

	// Token: 0x040024A4 RID: 9380
	public GameObject Head;

	// Token: 0x040024A5 RID: 9381
	public GameObject Nose;

	// Token: 0x040024A6 RID: 9382
	[Space]
	[Header("Sounds")]
	public Sound NoseSqeak;

	// Token: 0x040024A7 RID: 9383
	public Sound HeadSpin;

	// Token: 0x040024A8 RID: 9384
	public Sound ArmRaise;

	// Token: 0x040024A9 RID: 9385
	public Sound WarningVO1;

	// Token: 0x040024AA RID: 9386
	public Sound WarningVO2;

	// Token: 0x040024AB RID: 9387
	private AudioSource previousAudioSource;

	// Token: 0x040024AC RID: 9388
	public Sound GonnaBlowVO;

	// Token: 0x040024AD RID: 9389
	[Space]
	private Quaternion initialClownRotation;

	// Token: 0x040024AE RID: 9390
	private int WarningCount = 2;

	// Token: 0x040024AF RID: 9391
	private int ExplosionCountDown;

	// Token: 0x040024B0 RID: 9392
	private bool CountDownActive;

	// Token: 0x040024B1 RID: 9393
	[Space]
	[Header("Head Spin Animation")]
	public AnimationCurve HeadSpinCurve;

	// Token: 0x040024B2 RID: 9394
	private bool HeadSpinOneShotActive;

	// Token: 0x040024B3 RID: 9395
	public float HeadSpinSpeed;

	// Token: 0x040024B4 RID: 9396
	public float HeadSpinIntensity;

	// Token: 0x040024B5 RID: 9397
	private float HeadSpinLerp;

	// Token: 0x040024B6 RID: 9398
	[Space]
	[Header("Arm raise Animation")]
	public AnimationCurve ArmRaiseCurve;

	// Token: 0x040024B7 RID: 9399
	private bool ArmRaiseActive;

	// Token: 0x040024B8 RID: 9400
	public float ArmRaiseSpeed;

	// Token: 0x040024B9 RID: 9401
	public float ArmRaiseIntensity;

	// Token: 0x040024BA RID: 9402
	private float ArmRaiseLerp;

	// Token: 0x040024BB RID: 9403
	[Space]
	[Header("Nose Squeeze Animation")]
	public AnimationCurve NoseSqueezeCurve;

	// Token: 0x040024BC RID: 9404
	private bool NoseSqueezeActive;

	// Token: 0x040024BD RID: 9405
	public float NoseSqueezeSpeed;

	// Token: 0x040024BE RID: 9406
	public float NoseSqueezeIntensity;

	// Token: 0x040024BF RID: 9407
	private float NoseSqueezeLerp;
}
