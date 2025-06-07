using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000ED RID: 237
public class TruckMenuAnimated : MonoBehaviour
{
	// Token: 0x0600084A RID: 2122 RVA: 0x00050C34 File Offset: 0x0004EE34
	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.keepAnimatorStateOnDisable = true;
		this.photonView = base.GetComponent<PhotonView>();
		this.breakerCooldown = Random.Range(this.breakerCooldownMin, this.breakerCooldownMax);
		this.breakerTriggers.Add("SpeedUp");
		this.breakerTriggers.Add("SlowDown");
		this.breakerTriggers.Add("Swerve");
		this.breakerTriggers.Add("SkeletonHit");
		this.breakerTriggers.Add("TruckPass");
		this.breakerTriggers.Shuffle<string>();
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x00050CD8 File Offset: 0x0004EED8
	private void Update()
	{
		this.antennaMiddleTransform.rotation = SemiFunc.SpringQuaternionGet(this.antennaMiddleSpring, this.antennaMiddleTarget.rotation, -1f);
		this.frontPanelTransform.rotation = SemiFunc.SpringQuaternionGet(this.frontPanelSpring, this.frontPanelTarget.rotation, -1f);
		this.frontPanelTransform.localEulerAngles = new Vector3(0f, this.frontPanelTransform.localEulerAngles.y, 0f);
		this.windowRightTransform.rotation = SemiFunc.SpringQuaternionGet(this.windowRightSpring, this.windowRightTarget.rotation, -1f);
		this.windowRightTransform.localEulerAngles = new Vector3(this.windowRightTransform.localEulerAngles.x, 0f, 0f);
		this.windowLeftTransform.rotation = SemiFunc.SpringQuaternionGet(this.windowLeftSpring, this.windowLeftTarget.rotation, -1f);
		this.windowLeftTransform.localEulerAngles = new Vector3(this.windowLeftTransform.localEulerAngles.x, 0f, 0f);
		this.dishTransform.rotation = SemiFunc.SpringQuaternionGet(this.dishSpring, this.dishTarget.rotation, -1f);
		this.antennaBackTransform.rotation = SemiFunc.SpringQuaternionGet(this.antennaBackSpring, this.antennaBackTarget.rotation, -1f);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			this.breakerCooldown -= Time.deltaTime;
			if (this.breakerCooldown <= 0f)
			{
				this.BreakerTrigger();
			}
		}
		this.soundLoop.PlayLoop(true, 2f, 2f, 1f);
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x00050EAC File Offset: 0x0004F0AC
	private void BreakerTrigger()
	{
		this.breakerCooldown = Random.Range(this.breakerCooldownMin, this.breakerCooldownMax);
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("BreakerTriggerRPC", RpcTarget.All, new object[]
			{
				this.breakerTriggers[this.breakerTriggerIndex]
			});
		}
		else
		{
			this.BreakerTriggerRPC(this.breakerTriggers[this.breakerTriggerIndex], default(PhotonMessageInfo));
		}
		this.breakerTriggerIndex++;
		if (this.breakerTriggerIndex >= this.breakerTriggers.Count)
		{
			string text = this.breakerTriggers[this.breakerTriggers.Count - 1];
			this.breakerTriggers.Shuffle<string>();
			while (this.breakerTriggers[0] == text)
			{
				this.breakerTriggers.Shuffle<string>();
			}
			this.breakerTriggerIndex = 0;
		}
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x00050F91 File Offset: 0x0004F191
	[PunRPC]
	private void BreakerTriggerRPC(string _triggerName, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!this.animator)
		{
			return;
		}
		this.animator.SetTrigger(_triggerName);
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x00050FB8 File Offset: 0x0004F1B8
	public void SkeletonHitFirstImpulse()
	{
		this.soundSkeletonHit.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
		this.particleSkeletonBitsFirst.Play();
		this.particleSkeletonSmokeFirst.Play();
		GameDirector.instance.CameraShake.Shake(1f, 0.3f);
		GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x00051044 File Offset: 0x0004F244
	public void SkeletonHitLastImpulse()
	{
		this.soundSkeletonHitSkull.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
		this.particleSkeletonBitsLast.Play();
		this.particleSkeletonSmokeLast.Play();
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0005109C File Offset: 0x0004F29C
	public void PlaySwerve()
	{
		this.soundSwerve.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x000510D3 File Offset: 0x0004F2D3
	public void PlaySpeedUp()
	{
		this.soundSpeedUp.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0005110A File Offset: 0x0004F30A
	public void PlaySlowDown()
	{
		this.soundSlowDown.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x00051141 File Offset: 0x0004F341
	public void PlayBodyRustleLong01()
	{
		this.soundBodyRustleLong01.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x00051178 File Offset: 0x0004F378
	public void PlayBodyRustleLong02()
	{
		this.soundBodyRustleLong02.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x000511AF File Offset: 0x0004F3AF
	public void PlayBodyRustleLong03()
	{
		this.soundBodyRustleLong03.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x000511E6 File Offset: 0x0004F3E6
	public void PlayBodyRustleShort01()
	{
		this.soundBodyRustleShort01.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0005121D File Offset: 0x0004F41D
	public void PlayBodyRustleShort02()
	{
		this.soundBodyRustleShort02.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00051254 File Offset: 0x0004F454
	public void PlayBodyRustleShort03()
	{
		this.soundBodyRustleShort03.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0005128B File Offset: 0x0004F48B
	public void PlaySwerveFast01()
	{
		this.soundSwerveFast01.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x000512C2 File Offset: 0x0004F4C2
	public void PlaySwerveFast02()
	{
		this.soundSwerveFast02.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x000512F9 File Offset: 0x0004F4F9
	public void PlayFirePass()
	{
		this.soundFirePass.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x00051330 File Offset: 0x0004F530
	public void PlayFirePassSwerve01()
	{
		this.soundFirePassSwerve01.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x00051367 File Offset: 0x0004F567
	public void PlayFirePassSwerve02()
	{
		this.soundFirePassSwerve02.Play(this.soundLoop.Source.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x000513A0 File Offset: 0x0004F5A0
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
		Gizmos.matrix = this.antennaMiddleTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 6f);
		Gizmos.matrix = this.frontPanelTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 1.5f);
		Gizmos.matrix = this.windowRightTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * -2.5f);
		Gizmos.matrix = this.windowLeftTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * -2.5f);
		Gizmos.matrix = this.dishTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 4f);
		Gizmos.matrix = this.antennaBackTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 8f);
	}

	// Token: 0x04000F50 RID: 3920
	public Transform antennaMiddleTransform;

	// Token: 0x04000F51 RID: 3921
	public Transform antennaMiddleTarget;

	// Token: 0x04000F52 RID: 3922
	public SpringQuaternion antennaMiddleSpring;

	// Token: 0x04000F53 RID: 3923
	[Space(20f)]
	public Transform frontPanelTransform;

	// Token: 0x04000F54 RID: 3924
	public Transform frontPanelTarget;

	// Token: 0x04000F55 RID: 3925
	public SpringQuaternion frontPanelSpring;

	// Token: 0x04000F56 RID: 3926
	[Space(20f)]
	public Transform windowRightTransform;

	// Token: 0x04000F57 RID: 3927
	public Transform windowRightTarget;

	// Token: 0x04000F58 RID: 3928
	public SpringQuaternion windowRightSpring;

	// Token: 0x04000F59 RID: 3929
	[Space(20f)]
	public Transform windowLeftTransform;

	// Token: 0x04000F5A RID: 3930
	public Transform windowLeftTarget;

	// Token: 0x04000F5B RID: 3931
	public SpringQuaternion windowLeftSpring;

	// Token: 0x04000F5C RID: 3932
	[Space(20f)]
	public Transform dishTransform;

	// Token: 0x04000F5D RID: 3933
	public Transform dishTarget;

	// Token: 0x04000F5E RID: 3934
	public SpringQuaternion dishSpring;

	// Token: 0x04000F5F RID: 3935
	[Space(20f)]
	public Transform antennaBackTransform;

	// Token: 0x04000F60 RID: 3936
	public Transform antennaBackTarget;

	// Token: 0x04000F61 RID: 3937
	public SpringQuaternion antennaBackSpring;

	// Token: 0x04000F62 RID: 3938
	private Animator animator;

	// Token: 0x04000F63 RID: 3939
	private PhotonView photonView;

	// Token: 0x04000F64 RID: 3940
	private float breakerCooldown;

	// Token: 0x04000F65 RID: 3941
	private float breakerCooldownMin = 8f;

	// Token: 0x04000F66 RID: 3942
	private float breakerCooldownMax = 16f;

	// Token: 0x04000F67 RID: 3943
	private List<string> breakerTriggers = new List<string>();

	// Token: 0x04000F68 RID: 3944
	private int breakerTriggerIndex;

	// Token: 0x04000F69 RID: 3945
	public ParticleSystem particleSkeletonBitsFirst;

	// Token: 0x04000F6A RID: 3946
	public ParticleSystem particleSkeletonSmokeFirst;

	// Token: 0x04000F6B RID: 3947
	public ParticleSystem particleSkeletonBitsLast;

	// Token: 0x04000F6C RID: 3948
	public ParticleSystem particleSkeletonSmokeLast;

	// Token: 0x04000F6D RID: 3949
	public Sound soundLoop;

	// Token: 0x04000F6E RID: 3950
	[Space]
	public Sound soundSwerve;

	// Token: 0x04000F6F RID: 3951
	public Sound soundSpeedUp;

	// Token: 0x04000F70 RID: 3952
	public Sound soundSlowDown;

	// Token: 0x04000F71 RID: 3953
	[Space]
	public Sound soundBodyRustleLong01;

	// Token: 0x04000F72 RID: 3954
	public Sound soundBodyRustleLong02;

	// Token: 0x04000F73 RID: 3955
	public Sound soundBodyRustleLong03;

	// Token: 0x04000F74 RID: 3956
	[Space]
	public Sound soundBodyRustleShort01;

	// Token: 0x04000F75 RID: 3957
	public Sound soundBodyRustleShort02;

	// Token: 0x04000F76 RID: 3958
	public Sound soundBodyRustleShort03;

	// Token: 0x04000F77 RID: 3959
	[Space]
	public Sound soundSkeletonHit;

	// Token: 0x04000F78 RID: 3960
	public Sound soundSkeletonHitSkull;

	// Token: 0x04000F79 RID: 3961
	[Space]
	public Sound soundSwerveFast01;

	// Token: 0x04000F7A RID: 3962
	public Sound soundSwerveFast02;

	// Token: 0x04000F7B RID: 3963
	[Space]
	public Sound soundFirePass;

	// Token: 0x04000F7C RID: 3964
	public Sound soundFirePassSwerve01;

	// Token: 0x04000F7D RID: 3965
	public Sound soundFirePassSwerve02;
}
