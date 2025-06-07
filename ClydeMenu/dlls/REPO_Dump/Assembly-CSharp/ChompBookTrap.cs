using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002C2 RID: 706
public class ChompBookTrap : Trap
{
	// Token: 0x06001631 RID: 5681 RVA: 0x000C2872 File Offset: 0x000C0A72
	protected override void Start()
	{
		base.Start();
		this.initialBookRotation = this.closedBookTop.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x000C28A8 File Offset: 0x000C0AA8
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		if (this.trapActive)
		{
			this.physGrabObject.OverrideIndestructible(0.1f);
			this.enemyInvestigate = true;
		}
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x000C28E0 File Offset: 0x000C0AE0
	private void FixedUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.trapActive && this.targetTransform)
		{
			this.playerDirection = (this.targetTransform.position - this.physGrabObject.midPoint).normalized;
			Quaternion b = Quaternion.LookRotation(this.targetTransform.position - this.physGrabObject.midPoint);
			this.lookRotation = Quaternion.Slerp(this.lookRotation, b, Time.deltaTime * 5f);
			Vector3 vector = SemiFunc.PhysFollowRotation(base.transform, this.lookRotation, this.rb, 0.3f);
			if (this.physGrabObject.playerGrabbing.Count > 0)
			{
				vector *= 0.25f;
			}
			this.rb.AddTorque(vector, ForceMode.Impulse);
			if (this.attackedTimer <= 0f)
			{
				Vector3 a = SemiFunc.PhysFollowPosition(base.transform.position, this.targetTransform.position, this.rb.velocity, 1.5f);
				this.rb.AddForce(a * 10f * Time.fixedDeltaTime, ForceMode.Impulse);
			}
			else
			{
				this.attackedTimer -= Time.fixedDeltaTime;
			}
			this.physGrabObject.OverrideZeroGravity(0.1f);
		}
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x000C2A40 File Offset: 0x000C0C40
	public void Attack()
	{
		if (!this.isLocal)
		{
			return;
		}
		this.targetTransform = SemiFunc.PlayerGetNearestTransformWithinRange(10f, this.physGrabObject.centerPoint, true, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		if (this.targetTransform)
		{
			this.attackedTimer = 0.5f;
			this.rb.AddForce(this.playerDirection * 2f, ForceMode.Impulse);
		}
		else
		{
			this.rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddForce(normalized * 3f, ForceMode.Impulse);
			this.rb.AddTorque(normalized * 1f, ForceMode.Impulse);
		}
		this.biteCount++;
		if (this.biteCount >= this.biteAmount)
		{
			this.TrapStop();
		}
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x000C2B39 File Offset: 0x000C0D39
	public void ChompSound()
	{
		this.chomp.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x000C2B66 File Offset: 0x000C0D66
	public void StopAnimation()
	{
		if (this.trapStopped)
		{
			this.animator.enabled = false;
		}
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x000C2B7C File Offset: 0x000C0D7C
	private void TrapStopLogic()
	{
		this.trapActive = false;
		this.trapStopped = true;
		this.DeparentAndDestroy(this.lockParticle);
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x000C2B98 File Offset: 0x000C0D98
	public void TrapStop()
	{
		if (!GameManager.Multiplayer())
		{
			this.TrapStopRPC();
			return;
		}
		this.photonView.RPC("TrapStopRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x000C2BBE File Offset: 0x000C0DBE
	[PunRPC]
	private void TrapStopRPC()
	{
		this.TrapStopLogic();
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x000C2BC8 File Offset: 0x000C0DC8
	private void DeparentAndDestroy(ParticleSystem particleSystem)
	{
		if (particleSystem && particleSystem.isPlaying)
		{
			particleSystem.gameObject.transform.parent = null;
			particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
			particleSystem.Stop(false);
		}
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x000C2C0C File Offset: 0x000C0E0C
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.GrabRelease();
			this.trapActive = true;
			this.trapTriggered = true;
			this.biteBookTop.SetActive(true);
			this.biteBookBot.SetActive(true);
			this.closedBookTop.SetActive(false);
			this.closedBookBot.SetActive(false);
			this.chainLock.SetActive(false);
			this.lockBreak.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.lockParticle.Play(false);
			this.animator.enabled = true;
		}
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x000C2CB8 File Offset: 0x000C0EB8
	public void GrabRelease()
	{
		bool flag = false;
		foreach (PhysGrabber physGrabber in Enumerable.ToList<PhysGrabber>(this.physGrabObject.playerGrabbing))
		{
			if (!SemiFunc.IsMultiplayer())
			{
				physGrabber.ReleaseObject(0.1f);
			}
			else
			{
				physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
				{
					false,
					0.1f
				});
			}
			flag = true;
		}
		if (flag)
		{
			if (GameManager.instance.gameMode == 0)
			{
				this.GrabReleaseRPC(default(PhotonMessageInfo));
				return;
			}
			this.photonView.RPC("GrabReleaseRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x000C2D88 File Offset: 0x000C0F88
	[PunRPC]
	private void GrabReleaseRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.physGrabObject.grabDisableTimer = 1f;
	}

	// Token: 0x04002667 RID: 9831
	private Animator animator;

	// Token: 0x04002668 RID: 9832
	[Space]
	[Header("Book Components")]
	public GameObject closedBookTop;

	// Token: 0x04002669 RID: 9833
	public GameObject closedBookBot;

	// Token: 0x0400266A RID: 9834
	public GameObject chainLock;

	// Token: 0x0400266B RID: 9835
	public GameObject biteBookTop;

	// Token: 0x0400266C RID: 9836
	public GameObject biteBookBot;

	// Token: 0x0400266D RID: 9837
	[Space]
	[Header("Sounds")]
	public Sound chomp;

	// Token: 0x0400266E RID: 9838
	public Sound lockBreak;

	// Token: 0x0400266F RID: 9839
	[Space]
	private Quaternion initialBookRotation;

	// Token: 0x04002670 RID: 9840
	private Rigidbody rb;

	// Token: 0x04002671 RID: 9841
	public ParticleSystem lockParticle;

	// Token: 0x04002672 RID: 9842
	private Transform targetTransform;

	// Token: 0x04002673 RID: 9843
	private Vector3 playerDirection;

	// Token: 0x04002674 RID: 9844
	public int biteAmount;

	// Token: 0x04002675 RID: 9845
	private int biteCount;

	// Token: 0x04002676 RID: 9846
	private Quaternion lookRotation;

	// Token: 0x04002677 RID: 9847
	private float attackedTimer;

	// Token: 0x04002678 RID: 9848
	private bool trapStopped;
}
