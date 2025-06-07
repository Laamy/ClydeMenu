using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200009F RID: 159
[RequireComponent(typeof(EnemyGrounded))]
public class EnemyJump : MonoBehaviour
{
	// Token: 0x0600064C RID: 1612 RVA: 0x0003D83F File Offset: 0x0003BA3F
	private void Awake()
	{
		this.enemy.Jump = this;
		this.enemy.HasJump = true;
		if (this.gapJump && !this.gapCheckerActive)
		{
			base.StartCoroutine(this.GapChecker());
			this.gapCheckerActive = true;
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0003D87D File Offset: 0x0003BA7D
	private void Start()
	{
		if (!this.enemy.HasRigidbody)
		{
			Debug.LogError("EnemyJump: No Rigidbody found on " + this.enemy.name);
			this.stuckJump = false;
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0003D8AD File Offset: 0x0003BAAD
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.gapCheckerActive = false;
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x0003D8BC File Offset: 0x0003BABC
	private void OnEnable()
	{
		if (this.gapJump && !this.gapCheckerActive)
		{
			base.StartCoroutine(this.GapChecker());
			this.gapCheckerActive = true;
		}
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x0003D8E2 File Offset: 0x0003BAE2
	public void StuckReset()
	{
		this.stuckJumpImpulse = false;
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0003D8EB File Offset: 0x0003BAEB
	public void SurfaceJumpTrigger(Vector3 _direction)
	{
		if (!this.jumping)
		{
			this.surfaceJumpImpulse = true;
			this.surfaceJumpDirection = _direction;
		}
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x0003D903 File Offset: 0x0003BB03
	public void SurfaceJumpDisable(float _time)
	{
		this.surfaceJumpImpulse = false;
		this.surfaceJumpDisableTimer = _time;
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x0003D913 File Offset: 0x0003BB13
	public void StuckTrigger(Vector3 _direction)
	{
		if (!this.jumping)
		{
			this.stuckJumpImpulse = true;
			this.stuckJumpImpulseDirection = _direction;
		}
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x0003D92B File Offset: 0x0003BB2B
	public void StuckDisable(float _time)
	{
		this.stuckJumpDisableTimer = _time;
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x0003D934 File Offset: 0x0003BB34
	private IEnumerator GapChecker()
	{
		this.gapCheckerActive = true;
		for (;;)
		{
			if (this.enemy.Grounded.grounded && this.enemy.NavMeshAgent.HasPath())
			{
				int num = 8;
				float d = 0.5f;
				float maxDistance = 2f;
				Vector3 forward = this.enemy.Rigidbody.transform.forward;
				forward.y = 0f;
				Vector3 vector = this.enemy.Rigidbody.physGrabObject.centerPoint + forward * d;
				bool flag = false;
				for (int i = 0; i < num; i++)
				{
					if (Physics.Raycast(vector, Vector3.down * 0.25f, maxDistance, SemiFunc.LayerMaskGetVisionObstruct()))
					{
						if (flag)
						{
							this.gapJumpImpulse = true;
						}
					}
					else if (i < 2)
					{
						flag = true;
					}
					vector += forward * d;
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
		yield break;
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x0003D944 File Offset: 0x0003BB44
	private void FixedUpdate()
	{
		if (!this.jumping)
		{
			this.timeSinceJumped += Time.fixedDeltaTime;
		}
		else
		{
			this.timeSinceJumped = 0f;
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		bool flag = false;
		if (this.enemy.Rigidbody.grabbed || this.enemy.IsStunned() || this.enemy.Rigidbody.teleportedTimer > 0f)
		{
			this.stuckJumpImpulse = false;
			this.gapJumpImpulse = false;
			return;
		}
		float d = this.gapJumpForceUp;
		float d2 = this.gapJumpForceForward;
		if (this.gapJumpOverrideTimer > 0f)
		{
			d = this.gapJumpOverrideUp;
			d2 = this.gapJumpOverrideForward;
			this.gapJumpOverrideTimer -= Time.fixedDeltaTime;
		}
		if (this.gapJumpImpulse && !this.jumping && this.jumpCooldown <= 0f)
		{
			if (this.gapJumpDelayTimer > 0f)
			{
				this.JumpingDelaySet(true);
				this.enemy.NavMeshAgent.Stop(0.1f);
				this.enemy.Rigidbody.OverrideFollowPosition(0.1f, 0f, -1f);
				this.enemy.Rigidbody.OverrideColliderMaterialStunned(0.1f);
				this.gapJumpDelayTimer -= Time.fixedDeltaTime;
			}
			else
			{
				this.enemy.Rigidbody.DisableFollowPosition(0.5f, 10f);
				Vector3 vector = this.enemy.Rigidbody.transform.forward * d2;
				vector.y = 0f;
				vector += Vector3.up * d;
				this.enemy.Rigidbody.JumpImpulse();
				this.enemy.Rigidbody.rb.AddForce(vector, ForceMode.Impulse);
				this.enemy.NavMeshAgent.OverrideAgent(10f, 999f, 0.5f);
				this.gapJumpImpulse = false;
				this.stuckJumpImpulse = false;
				flag = true;
			}
		}
		else
		{
			this.gapJumpDelayTimer = this.gapJumpDelay;
		}
		float d3 = this.stuckJumpForceUp;
		float d4 = this.stuckJumpForceSide;
		if (this.stuckJumpOverrideTimer > 0f)
		{
			d3 = this.stuckJumpOverrideUp;
			d4 = this.stuckJumpOverrideSide;
			this.stuckJumpOverrideTimer -= Time.fixedDeltaTime;
		}
		if (this.enemy.TeleportedTimer > 0f)
		{
			this.StuckDisable(0.5f);
		}
		if (this.stuckJumpDisableTimer > 0f)
		{
			this.stuckJumpDisableTimer -= Time.fixedDeltaTime;
			this.stuckJumpImpulse = false;
		}
		else if (this.stuckJump)
		{
			if (this.cartJumpTimer > 0f && this.enemy.Rigidbody.touchingCartTimer > 0f)
			{
				if (this.cartJumpCooldown > 0f)
				{
					this.cartJumpCooldown -= Time.fixedDeltaTime;
				}
				else
				{
					this.stuckJumpImpulse = true;
					this.cartJumpCooldown = 2f;
				}
			}
			if (this.enemy.StuckCount >= this.stuckJumpCount)
			{
				this.stuckJumpImpulse = true;
				this.enemy.StuckCount = 0;
			}
			if (!flag && this.stuckJumpImpulse && this.enemy.Grounded.grounded && !this.jumping && this.jumpCooldown <= 0f)
			{
				if (this.stuckJumpImpulseDirection == Vector3.zero)
				{
					this.stuckJumpImpulseDirection = this.enemy.transform.position - this.enemy.Rigidbody.transform.position;
				}
				Vector3 vector2 = this.stuckJumpImpulseDirection.normalized * d4;
				vector2.y = 0f;
				vector2 += Vector3.up * d3;
				this.stuckJumpImpulseDirection = Vector3.zero;
				this.enemy.Rigidbody.JumpImpulse();
				this.enemy.Rigidbody.rb.AddForce(vector2, ForceMode.Impulse);
				this.stuckJumpImpulse = false;
				flag = true;
			}
		}
		if (this.cartJumpTimer > 0f)
		{
			this.cartJumpTimer -= Time.fixedDeltaTime;
		}
		if (this.surfaceJump)
		{
			float d5 = this.surfaceJumpForceUp;
			float d6 = this.surfaceJumpForceSide;
			if (this.surfaceJumpOverrideTimer > 0f)
			{
				d5 = this.surfaceJumpOverrideUp;
				d6 = this.surfaceJumpOverrideSide;
				this.surfaceJumpOverrideTimer -= Time.fixedDeltaTime;
			}
			if (this.surfaceJumpDisableTimer > 0f)
			{
				this.surfaceJumpDisableTimer -= Time.fixedDeltaTime;
			}
			else if (!flag && this.surfaceJumpImpulse && this.enemy.Grounded.grounded && !this.jumping && this.jumpCooldown <= 0f)
			{
				this.enemy.Rigidbody.DisableFollowPosition(0.2f, 20f);
				this.enemy.NavMeshAgent.Stop(0.3f);
				Vector3 a = this.surfaceJumpDirection * d6;
				a.y = 0f;
				this.enemy.Rigidbody.JumpImpulse();
				this.enemy.Rigidbody.rb.AddForce(a + Vector3.up * d5, ForceMode.Impulse);
				this.surfaceJumpImpulse = false;
				flag = true;
			}
		}
		if (!this.jumping)
		{
			if (flag)
			{
				this.JumpingDelaySet(false);
				this.JumpingSet(true);
				this.LandDelaySet(false);
				this.enemy.Grounded.GroundedDisable(0.1f);
			}
		}
		else if (this.enemy.Grounded.grounded)
		{
			if (this.warpAgentOnLand && !this.enemy.NavMeshAgent.IsDisabled())
			{
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			}
			this.JumpingDelaySet(false);
			this.JumpingSet(false);
			if (this.gapLandDelay > 0f)
			{
				this.LandDelaySet(true);
				this.gapLandDelayTimer = this.gapLandDelay;
			}
			this.jumpCooldown = 0.25f;
		}
		if (this.jumpCooldown > 0f)
		{
			this.jumpCooldown -= Time.fixedDeltaTime;
			this.jumpCooldown = Mathf.Max(this.jumpCooldown, 0f);
			this.enemy.StuckCount = 0;
			this.surfaceJumpImpulse = false;
			this.stuckJumpImpulse = false;
			this.gapJumpImpulse = false;
		}
		if (this.gapLandDelayTimer > 0f)
		{
			this.enemy.NavMeshAgent.Stop(0.1f);
			this.enemy.Rigidbody.OverrideFollowPosition(0.1f, 0f, -1f);
			this.enemy.Rigidbody.OverrideColliderMaterialStunned(0.1f);
			this.gapLandDelayTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x0003E040 File Offset: 0x0003C240
	public void JumpingSet(bool _jumping)
	{
		if (_jumping == this.jumping)
		{
			return;
		}
		if (_jumping)
		{
			this.enemy.Grounded.grounded = false;
		}
		this.jumping = _jumping;
		if (GameManager.Multiplayer() && PhotonNetwork.IsMasterClient)
		{
			this.enemy.Rigidbody.photonView.RPC("JumpingSetRPC", RpcTarget.Others, new object[]
			{
				this.jumping
			});
		}
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0003E0B0 File Offset: 0x0003C2B0
	public void JumpingDelaySet(bool _jumpingDelay)
	{
		if (this.jumpingDelay == _jumpingDelay)
		{
			return;
		}
		this.jumpingDelay = _jumpingDelay;
		if (SemiFunc.IsMasterClient())
		{
			this.enemy.Rigidbody.photonView.RPC("JumpingDelaySetRPC", RpcTarget.Others, new object[]
			{
				this.jumpingDelay
			});
		}
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0003E104 File Offset: 0x0003C304
	public void LandDelaySet(bool _landDelay)
	{
		if (this.landDelay == _landDelay)
		{
			return;
		}
		this.landDelay = _landDelay;
		if (SemiFunc.IsMasterClient())
		{
			this.enemy.Rigidbody.photonView.RPC("LandDelaySetRPC", RpcTarget.Others, new object[]
			{
				this.landDelay
			});
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x0003E158 File Offset: 0x0003C358
	public void CartJump(float _time)
	{
		this.cartJumpTimer = _time;
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x0003E161 File Offset: 0x0003C361
	public void GapJumpOverride(float _time, float _up, float _forward)
	{
		this.gapJumpOverrideTimer = _time;
		this.gapJumpOverrideUp = _up;
		this.gapJumpOverrideForward = _forward;
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0003E178 File Offset: 0x0003C378
	public void StuckJumpOverride(float _time, float _up, float _side)
	{
		this.stuckJumpOverrideTimer = _time;
		this.stuckJumpOverrideUp = _up;
		this.stuckJumpOverrideSide = _side;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0003E18F File Offset: 0x0003C38F
	public void SurfaceJumpOverride(float _time, float _up, float _side)
	{
		this.surfaceJumpOverrideTimer = _time;
		this.surfaceJumpOverrideUp = _up;
		this.surfaceJumpOverrideSide = _side;
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0003E1A6 File Offset: 0x0003C3A6
	[PunRPC]
	private void JumpingSetRPC(bool _jumping)
	{
		this.jumping = _jumping;
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0003E1AF File Offset: 0x0003C3AF
	[PunRPC]
	private void JumpingDelaySetRPC(bool _jumpingDelay)
	{
		this.jumpingDelay = _jumpingDelay;
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x0003E1B8 File Offset: 0x0003C3B8
	[PunRPC]
	private void LandDelaySetRPC(bool _landDelay)
	{
		this.landDelay = _landDelay;
	}

	// Token: 0x04000A6F RID: 2671
	public Enemy enemy;

	// Token: 0x04000A70 RID: 2672
	internal bool jumping;

	// Token: 0x04000A71 RID: 2673
	internal bool jumpingDelay;

	// Token: 0x04000A72 RID: 2674
	internal bool landDelay;

	// Token: 0x04000A73 RID: 2675
	internal float jumpCooldown;

	// Token: 0x04000A74 RID: 2676
	internal float timeSinceJumped;

	// Token: 0x04000A75 RID: 2677
	[Space]
	public bool warpAgentOnLand;

	// Token: 0x04000A76 RID: 2678
	[Space]
	public bool surfaceJump = true;

	// Token: 0x04000A77 RID: 2679
	public float surfaceJumpForceUp = 5f;

	// Token: 0x04000A78 RID: 2680
	public float surfaceJumpForceSide = 2f;

	// Token: 0x04000A79 RID: 2681
	private bool surfaceJumpImpulse;

	// Token: 0x04000A7A RID: 2682
	private Vector3 surfaceJumpDirection;

	// Token: 0x04000A7B RID: 2683
	private float surfaceJumpDisableTimer;

	// Token: 0x04000A7C RID: 2684
	private float surfaceJumpOverrideTimer;

	// Token: 0x04000A7D RID: 2685
	private float surfaceJumpOverrideUp;

	// Token: 0x04000A7E RID: 2686
	private float surfaceJumpOverrideSide;

	// Token: 0x04000A7F RID: 2687
	[Space]
	public bool stuckJump;

	// Token: 0x04000A80 RID: 2688
	private float stuckJumpDisableTimer;

	// Token: 0x04000A81 RID: 2689
	private float cartJumpTimer;

	// Token: 0x04000A82 RID: 2690
	private float cartJumpCooldown;

	// Token: 0x04000A83 RID: 2691
	public int stuckJumpCount = 5;

	// Token: 0x04000A84 RID: 2692
	public float stuckJumpForceUp = 5f;

	// Token: 0x04000A85 RID: 2693
	public float stuckJumpForceSide = 2f;

	// Token: 0x04000A86 RID: 2694
	private bool stuckJumpImpulse;

	// Token: 0x04000A87 RID: 2695
	private Vector3 stuckJumpImpulseDirection;

	// Token: 0x04000A88 RID: 2696
	private float stuckJumpOverrideTimer;

	// Token: 0x04000A89 RID: 2697
	private float stuckJumpOverrideUp;

	// Token: 0x04000A8A RID: 2698
	private float stuckJumpOverrideSide;

	// Token: 0x04000A8B RID: 2699
	[Space]
	public bool gapJump;

	// Token: 0x04000A8C RID: 2700
	public float gapJumpForceUp = 5f;

	// Token: 0x04000A8D RID: 2701
	public float gapJumpForceForward = 5f;

	// Token: 0x04000A8E RID: 2702
	internal bool gapJumpImpulse;

	// Token: 0x04000A8F RID: 2703
	private float gapJumpOverrideTimer;

	// Token: 0x04000A90 RID: 2704
	private float gapJumpOverrideUp;

	// Token: 0x04000A91 RID: 2705
	private float gapJumpOverrideForward;

	// Token: 0x04000A92 RID: 2706
	public float gapJumpDelay;

	// Token: 0x04000A93 RID: 2707
	private float gapJumpDelayTimer;

	// Token: 0x04000A94 RID: 2708
	public float gapLandDelay;

	// Token: 0x04000A95 RID: 2709
	private float gapLandDelayTimer;

	// Token: 0x04000A96 RID: 2710
	private bool gapCheckerActive;
}
