using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class PhysGrabHinge : MonoBehaviour
{
	// Token: 0x06000DD5 RID: 3541 RVA: 0x00078C78 File Offset: 0x00076E78
	private void Awake()
	{
		this.photon = base.GetComponent<PhotonView>();
		Sound.CopySound(this.hingeAudio.moveLoop, this.moveLoop);
		this.moveLoop.Source = this.audioSource;
		this.joint = base.GetComponent<HingeJoint>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.impactDetector.particleDisable = true;
		this.joint.anchor = this.hingePoint.localPosition;
		this.hingePointRb = this.hingePoint.GetComponent<Rigidbody>();
		if (this.hingePointRb)
		{
			this.hingePointHasRb = true;
			this.hingePointPosition = this.hingePoint.position;
		}
		if (SemiFunc.IsMultiplayer() && SemiFunc.IsNotMasterClient())
		{
			Object.Destroy(this.joint);
			this.joint = null;
			this.hingePointHasRb = false;
		}
		this.restPosition = base.transform.position;
		this.restRotation = base.transform.rotation;
		base.StartCoroutine(this.RigidBodyGet());
		base.gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectHinge");
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectHinge");
		}
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x00078DF8 File Offset: 0x00076FF8
	private IEnumerator RigidBodyGet()
	{
		while (!this.physGrabObject.spawned)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.hingePoint.transform.parent = base.transform.parent;
		this.WallTagSet();
		yield break;
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x00078E08 File Offset: 0x00077008
	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.closeDisableTimer = 0.1f;
			return;
		}
		if (this.closing && other.gameObject.CompareTag("Phys Grab Object"))
		{
			this.closing = false;
		}
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00078E54 File Offset: 0x00077054
	private void OnJointBreak(float breakForce)
	{
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.physGrabObject.rb.AddForce(-this.physGrabObject.rb.velocity * 2f, ForceMode.Impulse);
			this.physGrabObject.rb.AddTorque(-this.physGrabObject.rb.angularVelocity * 10f, ForceMode.Impulse);
			this.HingeBreakImpulse();
			this.broken = true;
		}
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00078EE4 File Offset: 0x000770E4
	private void FixedUpdate()
	{
		if (this.broken)
		{
			this.brokenTimer += Time.fixedDeltaTime;
		}
		if (this.dead || this.broken || !this.physGrabObject.spawned)
		{
			return;
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			if (GameManager.Multiplayer())
			{
				this.physGrabObject.photonTransformView.KinematicClientForce(0.1f);
			}
			if (this.hingePointHasRb)
			{
				if (this.joint.angle >= this.hingeOffsetPositiveThreshold)
				{
					Vector3 b = this.hingePointPosition + this.hingePoint.TransformDirection(this.hingeOffsetPositive);
					Vector3 vector = Vector3.Lerp(this.hingePointRb.transform.position, b, this.hingeOffsetSpeed * Time.fixedDeltaTime);
					if (this.hingePointRb.position != vector)
					{
						this.hingePointRb.MovePosition(vector);
					}
				}
				else if (this.joint.angle <= this.hingeOffsetNegativeThreshold)
				{
					Vector3 b2 = this.hingePointPosition + this.hingePoint.TransformDirection(this.hingeOffsetNegative);
					Vector3 vector2 = Vector3.Lerp(this.hingePointRb.transform.position, b2, this.hingeOffsetSpeed * Time.fixedDeltaTime);
					if (this.hingePointRb.position != vector2)
					{
						this.hingePointRb.MovePosition(vector2);
					}
				}
				else
				{
					Vector3 vector3 = Vector3.Lerp(this.hingePointRb.transform.position, this.hingePointPosition, this.hingeOffsetSpeed * Time.fixedDeltaTime);
					if (this.closed)
					{
						vector3 = this.hingePointPosition;
					}
					if (this.hingePointRb.position != vector3)
					{
						this.hingePointRb.MovePosition(vector3);
					}
				}
			}
			if (!this.closed && this.closeDisableTimer <= 0f && this.joint)
			{
				if (!this.closing)
				{
					float num = Vector3.Dot(this.physGrabObject.rb.angularVelocity.normalized, (-this.joint.axis * this.joint.angle).normalized);
					if (this.physGrabObject.rb.angularVelocity.magnitude < this.closeMaxSpeed && Mathf.Abs(this.joint.angle) < this.closeThreshold && (num > 0f || this.physGrabObject.rb.angularVelocity.magnitude < 0.1f))
					{
						this.closeHeavy = false;
						this.closeSpeed = Mathf.Max(this.physGrabObject.rb.angularVelocity.magnitude, 0.2f);
						if (this.closeSpeed > this.closeHeavySpeed)
						{
							this.closeHeavy = true;
						}
						this.closing = true;
					}
				}
				else if (this.physGrabObject.playerGrabbing.Count > 0)
				{
					this.closing = false;
				}
				else
				{
					Vector3 vector4 = this.restRotation.eulerAngles - this.physGrabObject.rb.rotation.eulerAngles;
					vector4 = Vector3.ClampMagnitude(vector4, this.closeSpeed);
					this.physGrabObject.rb.AddRelativeTorque(vector4, ForceMode.Acceleration);
					if (Mathf.Abs(this.joint.angle) < 2f)
					{
						this.closedForceTimer = 0.25f;
						this.closing = false;
						this.CloseImpulse(this.closeHeavy);
					}
				}
			}
			if (this.physGrabObject.playerGrabbing.Count > 0)
			{
				this.closeDisableTimer = 0.1f;
			}
			else if (this.closeDisableTimer > 0f)
			{
				this.closeDisableTimer -= 1f * Time.fixedDeltaTime;
			}
			if (this.closed)
			{
				if (this.closedForceTimer > 0f)
				{
					this.closedForceTimer -= 1f * Time.fixedDeltaTime;
				}
				else if (this.physGrabObject.rb.angularVelocity.magnitude > this.openForceNeeded)
				{
					this.OpenImpulse();
					this.closeDisableTimer = 2f;
					this.closing = false;
				}
				if (this.closed && !this.physGrabObject.rb.isKinematic && (this.physGrabObject.rb.position != this.restPosition || this.physGrabObject.rb.rotation != this.restRotation))
				{
					this.physGrabObject.rb.MovePosition(this.restPosition);
					this.physGrabObject.rb.MoveRotation(this.restRotation);
					this.physGrabObject.rb.angularVelocity = Vector3.zero;
					this.physGrabObject.rb.velocity = Vector3.zero;
				}
			}
			if (this.physGrabObject.playerGrabbing.Count <= 0 && !this.closing && !this.closed)
			{
				Vector3 angularVelocity = this.physGrabObject.rb.angularVelocity;
				if (angularVelocity.magnitude <= 0.1f && this.bounceVelocity.magnitude > 0.5f && this.bounceCooldown <= 0f)
				{
					this.bounceCooldown = 1f;
					this.physGrabObject.rb.AddTorque(this.bounceAmount * -this.bounceVelocity.normalized, ForceMode.Impulse);
					if (this.bounceEffect == PhysGrabHinge.BounceEffect.Heavy)
					{
						this.physGrabObject.heavyImpactImpulse = true;
					}
					else if (this.bounceEffect == PhysGrabHinge.BounceEffect.Medium)
					{
						this.physGrabObject.mediumImpactImpulse = true;
					}
					else
					{
						this.physGrabObject.lightImpactImpulse = true;
					}
					this.moveLoopEndDisableTimer = 1f;
				}
				this.bounceVelocity = angularVelocity;
			}
			else
			{
				this.bounceVelocity = Vector3.zero;
			}
			if (this.bounceCooldown > 0f)
			{
				this.bounceCooldown -= 1f * Time.fixedDeltaTime;
			}
			if (!this.closing)
			{
				this.physGrabObject.OverrideDrag(this.drag, 0.1f);
				this.physGrabObject.OverrideAngularDrag(this.drag, 0.1f);
			}
		}
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x00079548 File Offset: 0x00077748
	private void Update()
	{
		if (this.dead)
		{
			this.deadTimer -= 1f * Time.deltaTime;
			if (this.deadTimer <= 0f)
			{
				this.impactDetector.DestroyObject(true);
			}
			return;
		}
		if (this.broken)
		{
			this.moveLoop.PlayLoop(false, 1f, 1f, 1f);
			return;
		}
		if (this.hingeAudio.moveLoopEnabled)
		{
			if (this.physGrabObject.rbVelocity.magnitude > this.hingeAudio.moveLoopThreshold)
			{
				if (!this.moveLoopActive)
				{
					this.fadeOutFast = false;
					this.moveLoopActive = true;
				}
				this.moveLoop.PlayLoop(true, this.hingeAudio.moveLoopFadeInSpeed, this.hingeAudio.moveLoopFadeOutSpeed, 1f);
				this.moveLoop.LoopPitch = Mathf.Max(this.moveLoop.Pitch + this.physGrabObject.rbVelocity.magnitude * this.hingeAudio.moveLoopVelocityMult, 0.1f);
			}
			else
			{
				if (this.moveLoopActive)
				{
					if (this.moveLoopEndDisableTimer <= 0f)
					{
						this.hingeAudio.moveLoopEnd.Play(this.moveLoop.Source.transform.position, 1f, 1f, 1f, 1f);
						this.moveLoopEndDisableTimer = 3f;
					}
					this.moveLoopActive = false;
				}
				if (this.fadeOutFast)
				{
					this.moveLoop.PlayLoop(false, this.hingeAudio.moveLoopFadeInSpeed, 20f, 1f);
				}
				else
				{
					this.moveLoop.PlayLoop(false, this.hingeAudio.moveLoopFadeInSpeed, this.hingeAudio.moveLoopFadeOutSpeed, 1f);
				}
				this.moveLoopEndDisableTimer = 0.5f;
			}
			if (this.moveLoopEndDisableTimer > 0f)
			{
				this.moveLoopEndDisableTimer -= 1f * Time.deltaTime;
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.investigateDelay > 0f)
		{
			this.investigateDelay -= 1f * Time.deltaTime;
			if (this.investigateDelay <= 0f && this.physGrabObject.enemyInteractTimer <= 0f)
			{
				EnemyDirector.instance.SetInvestigate(this.physGrabObject.midPoint, this.investigateRadius, false);
			}
		}
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x000797AC File Offset: 0x000779AC
	private void WallTagSet()
	{
		string text = "Untagged";
		if (this.closed && !this.broken && !this.dead)
		{
			text = "Wall";
		}
		if (text == "Wall" && this.wallTagHinges.Length != 0)
		{
			foreach (PhysGrabHinge physGrabHinge in this.wallTagHinges)
			{
				if (!physGrabHinge || !physGrabHinge.closed)
				{
					return;
				}
			}
		}
		if (this.wallTagObjects.Length != 0)
		{
			foreach (GameObject gameObject in this.wallTagObjects)
			{
				if (gameObject)
				{
					gameObject.tag = text;
				}
			}
		}
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00079854 File Offset: 0x00077A54
	private void EnemyInvestigate(float radius)
	{
		this.investigateDelay = 0.1f;
		this.investigateRadius = radius;
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x00079868 File Offset: 0x00077A68
	private void CloseImpulse(bool heavy)
	{
		this.EnemyInvestigate(1f);
		if (GameManager.instance.gameMode == 0)
		{
			this.CloseImpulseRPC(heavy);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photon.RPC("CloseImpulseRPC", RpcTarget.All, new object[]
			{
				heavy
			});
		}
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x000798BC File Offset: 0x00077ABC
	[PunRPC]
	private void CloseImpulseRPC(bool heavy)
	{
		this.fadeOutFast = true;
		GameDirector.instance.CameraImpact.ShakeDistance(this.closeShake * 0.5f, 3f, 10f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(this.closeShake, 3f, 10f, base.transform.position, 0.1f);
		if (heavy)
		{
			this.hingeAudio.CloseHeavy.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		}
		else
		{
			this.hingeAudio.Close.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		}
		this.moveLoopEndDisableTimer = 1f;
		this.closed = true;
		this.WallTagSet();
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x000799BB File Offset: 0x00077BBB
	private void OpenImpulse()
	{
		this.EnemyInvestigate(0.5f);
		if (GameManager.instance.gameMode == 0)
		{
			this.OpenImpulseRPC();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photon.RPC("OpenImpulseRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x000799F8 File Offset: 0x00077BF8
	[PunRPC]
	private void OpenImpulseRPC()
	{
		GameDirector.instance.CameraImpact.ShakeDistance(this.openShake * 0.5f, 3f, 10f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(this.openShake, 3f, 10f, base.transform.position, 0.1f);
		if (this.physGrabObject.rbAngularVelocity.magnitude > this.openHeavyThreshold)
		{
			this.hingeAudio.OpenHeavy.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		}
		else
		{
			this.hingeAudio.Open.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		}
		this.closed = false;
		this.WallTagSet();
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00079AFA File Offset: 0x00077CFA
	private void HingeBreakImpulse()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.HingeBreakRPC();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photon.RPC("HingeBreakRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00079B2C File Offset: 0x00077D2C
	[PunRPC]
	private void HingeBreakRPC()
	{
		GameDirector.instance.CameraImpact.ShakeDistance(this.hingeBreakShake * 0.5f, 3f, 10f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(this.hingeBreakShake, 3f, 10f, base.transform.position, 0.1f);
		this.hingeAudio.HingeBreak.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		this.physGrabObject.heavyBreakImpulse = true;
		this.impactDetector.isHinge = false;
		this.impactDetector.isBrokenHinge = true;
		this.impactDetector.particleDisable = false;
		this.broken = true;
		this.WallTagSet();
		int layer = LayerMask.NameToLayer("PhysGrabObject");
		base.gameObject.layer = layer;
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.layer = layer;
		}
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00079C78 File Offset: 0x00077E78
	public void DestroyHinge()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.DestroyHingeRPC();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photon.RPC("DestroyHingeRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x00079CAA File Offset: 0x00077EAA
	[PunRPC]
	private void DestroyHingeRPC()
	{
		this.dead = true;
		this.WallTagSet();
	}

	// Token: 0x0400165B RID: 5723
	private PhotonView photon;

	// Token: 0x0400165C RID: 5724
	public HingeAudio hingeAudio;

	// Token: 0x0400165D RID: 5725
	public AudioSource audioSource;

	// Token: 0x0400165E RID: 5726
	[Space]
	public Transform hingePoint;

	// Token: 0x0400165F RID: 5727
	private Rigidbody hingePointRb;

	// Token: 0x04001660 RID: 5728
	private bool hingePointHasRb;

	// Token: 0x04001661 RID: 5729
	public float hingeOffsetPositiveThreshold = 15f;

	// Token: 0x04001662 RID: 5730
	public float hingeOffsetNegativeThreshold = -15f;

	// Token: 0x04001663 RID: 5731
	public float hingeOffsetSpeed = 5f;

	// Token: 0x04001664 RID: 5732
	public Vector3 hingeOffsetPositive;

	// Token: 0x04001665 RID: 5733
	public Vector3 hingeOffsetNegative;

	// Token: 0x04001666 RID: 5734
	private Vector3 hingePointPosition;

	// Token: 0x04001667 RID: 5735
	[Space]
	public float hingeBreakShake = 3f;

	// Token: 0x04001668 RID: 5736
	[Space]
	public float closeThreshold = 10f;

	// Token: 0x04001669 RID: 5737
	public float closeMaxSpeed = 1f;

	// Token: 0x0400166A RID: 5738
	public float closeHeavySpeed = 5f;

	// Token: 0x0400166B RID: 5739
	public float closeShake = 3f;

	// Token: 0x0400166C RID: 5740
	private bool closeHeavy;

	// Token: 0x0400166D RID: 5741
	private float closeSpeed;

	// Token: 0x0400166E RID: 5742
	internal bool closed = true;

	// Token: 0x0400166F RID: 5743
	private float closedForceTimer;

	// Token: 0x04001670 RID: 5744
	private bool closing;

	// Token: 0x04001671 RID: 5745
	private float closeDisableTimer;

	// Token: 0x04001672 RID: 5746
	[Space]
	private float openForceNeeded = 0.04f;

	// Token: 0x04001673 RID: 5747
	public float openHeavyThreshold = 3f;

	// Token: 0x04001674 RID: 5748
	public float openShake = 3f;

	// Token: 0x04001675 RID: 5749
	internal HingeJoint joint;

	// Token: 0x04001676 RID: 5750
	private PhysGrabObject physGrabObject;

	// Token: 0x04001677 RID: 5751
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04001678 RID: 5752
	private bool moveLoopActive;

	// Token: 0x04001679 RID: 5753
	private float moveLoopEndDisableTimer;

	// Token: 0x0400167A RID: 5754
	[HideInInspector]
	public Sound moveLoop;

	// Token: 0x0400167B RID: 5755
	private Vector3 restPosition;

	// Token: 0x0400167C RID: 5756
	private Quaternion restRotation;

	// Token: 0x0400167D RID: 5757
	internal bool dead;

	// Token: 0x0400167E RID: 5758
	private float deadTimer = 0.1f;

	// Token: 0x0400167F RID: 5759
	internal bool broken;

	// Token: 0x04001680 RID: 5760
	internal float brokenTimer;

	// Token: 0x04001681 RID: 5761
	[Space]
	public float drag;

	// Token: 0x04001682 RID: 5762
	[Space]
	public float bounceAmount = 0.2f;

	// Token: 0x04001683 RID: 5763
	public PhysGrabHinge.BounceEffect bounceEffect = PhysGrabHinge.BounceEffect.Medium;

	// Token: 0x04001684 RID: 5764
	private Vector3 bounceVelocity;

	// Token: 0x04001685 RID: 5765
	private float bounceCooldown;

	// Token: 0x04001686 RID: 5766
	[Space]
	public PhysGrabHinge[] wallTagHinges;

	// Token: 0x04001687 RID: 5767
	public GameObject[] wallTagObjects;

	// Token: 0x04001688 RID: 5768
	private float investigateDelay;

	// Token: 0x04001689 RID: 5769
	private float investigateRadius;

	// Token: 0x0400168A RID: 5770
	private bool fadeOutFast;

	// Token: 0x020003A9 RID: 937
	public enum BounceEffect
	{
		// Token: 0x04002BEF RID: 11247
		Light,
		// Token: 0x04002BF0 RID: 11248
		Medium,
		// Token: 0x04002BF1 RID: 11249
		Heavy
	}
}
