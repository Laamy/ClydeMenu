using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class ItemLadder : MonoBehaviour
{
	// Token: 0x06000B08 RID: 2824 RVA: 0x000624A0 File Offset: 0x000606A0
	private void Start()
	{
		this.itemToggle = base.gameObject.GetComponent<ItemToggle>();
		this.itemEquippable = base.gameObject.GetComponent<ItemEquippable>();
		this.photonView = base.gameObject.GetComponent<PhotonView>();
		this.itemBattery = base.gameObject.GetComponent<ItemBattery>();
		this.physGrabObject = base.gameObject.GetComponent<PhysGrabObject>();
		this.rb = base.gameObject.GetComponent<Rigidbody>();
		this.previousToggleState = this.itemToggle.toggleState;
		this.previousEquippedState = this.itemEquippable.isEquipped;
		this.colliderPivotOriginalScale = this.colliderPivot.localScale;
		this.collisionCheckPivotOriginalPosition = this.collisionCheckPivot.localPosition;
		this.topTransformPivotOriginalPosition = this.topTransformPivot.localPosition;
		this.plasmaBridgePivotOriginalScale = this.plasmaBridgePivot.localScale;
		this.bridgeBaseColor = this.bridge.material.GetColor("_Color");
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x00062594 File Offset: 0x00060794
	private void Update()
	{
		this.bridgeLoop.PlayLoop(this.extensionIndex != 0, 1f, 1f, 1f);
		if (this.animate && !this.itemEquippable.isEquipped && !this.IsMinSize())
		{
			this.AnimateExtension();
		}
		else if (this.animate)
		{
			this.AnimateRetraction();
		}
		if (this.flickering)
		{
			this.FlickerBridge();
		}
		if (this.extensionIndex > 0 && !SemiFunc.RunIsShop())
		{
			this.itemBattery.OverrideBatteryShow(0.1f);
		}
		if (this.IndexChanged())
		{
			this.ScaleCollider();
			this.IncrementCollisionCheck();
			if (this.extensionIndex == 0)
			{
				this.retractSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				this.Retract();
				if (!this.itemEquippable.isEquipped)
				{
					this.StartAnimation();
				}
				this.flickering = false;
			}
			else
			{
				this.StartAnimation();
				this.extendSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				this.animationCurveEval = 0f;
				this.itemBattery.batteryLife -= this.extensionBatteryDrain;
			}
		}
		if (this.StateChanged())
		{
			if (this.currentState == ItemLadder.States.Denied)
			{
				this.bridge.material.SetColor("_Color", new Color(1f, 0.8f, 0.2f) * 3f);
				this.grabIcon.material.SetColor("_EmissionColor", Color.green);
				this.grabIcon.material.mainTextureOffset = new Vector2(0.5f, 0f);
				this.deniedSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			}
			else if (this.currentState == ItemLadder.States.Neutral)
			{
				if (this.previousPreviousState != ItemLadder.States.Denied)
				{
					this.grabIcon.material.SetColor("_EmissionColor", Color.red);
					this.grabIcon.material.mainTextureOffset = new Vector2(0f, 0f);
				}
				this.bridge.material.SetColor("_Color", this.bridgeBaseColor);
			}
			else if (this.currentState == ItemLadder.States.Grabbed)
			{
				this.grabIcon.material.SetColor("_EmissionColor", Color.green);
				this.grabIcon.material.mainTextureOffset = new Vector2(0.5f, 0f);
				this.bridge.material.SetColor("_Color", this.bridgeBaseColor);
				this.justGrabbed = true;
			}
			else if (this.currentState == ItemLadder.States.OutOfBattery)
			{
				this.flickering = false;
				this.bridge.material.SetColor("_Color", Color.red * 0f);
				this.grabIcon.material.SetColor("_EmissionColor", Color.red);
				this.grabIcon.material.mainTextureOffset = new Vector2(0f, 0f);
				this.itemBattery.batteryLife = 0f;
			}
			else if (this.currentState == ItemLadder.States.Flickering)
			{
				this.flickering = true;
				this.flickeringSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			}
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.EPressed())
		{
			if (!this.IsColliding() && !this.animate && !this.itemEquippable.isEquipped)
			{
				this.Extend();
			}
			else if (this.extensionIndex != 0 && !this.animate)
			{
				this.deniedTime = Time.time;
				if (!this.flickering)
				{
					this.StateChange(ItemLadder.States.Denied);
				}
			}
		}
		if (this.currentState == ItemLadder.States.Denied && Time.time - this.deniedTime >= 0.25f)
		{
			if (this.physGrabObject.grabbed)
			{
				this.StateChange(ItemLadder.States.Grabbed);
			}
			else
			{
				this.StateChange(ItemLadder.States.Neutral);
			}
		}
		if (this.EquipPressed() && this.extensionIndex != 0)
		{
			this.SetExtensionIndex(0);
		}
		if (this.itemBattery.batteryLife <= 0f && this.extensionIndex > 0 && !this.flickering)
		{
			this.StateChange(ItemLadder.States.Flickering);
			this.flickering = true;
		}
		if (this.flickering)
		{
			this.flickeringTimer += Time.deltaTime;
			if (this.flickeringTimer >= this.flickeringTime)
			{
				this.flickeringTimer = 0f;
				this.GrabRelease();
				this.StateChange(ItemLadder.States.OutOfBattery);
				this.SetExtensionIndex(0);
				this.flickering = false;
			}
		}
		if (SemiFunc.RunIsShop())
		{
			if (this.physGrabObject.impactDetector.inCart && this.extensionIndex > 0)
			{
				this.SetExtensionIndex(0);
				this.GrabRelease();
				this.shopTimerOn = false;
				this.shopTimer = 10f;
			}
			if (!this.shopTimerOn)
			{
				return;
			}
			this.shopTimer -= Time.deltaTime;
			if (this.shopTimer <= 0f)
			{
				this.SetExtensionIndex(0);
				this.GrabRelease();
				this.shopTimerOn = false;
				this.shopTimer = 10f;
			}
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x00062ADC File Offset: 0x00060CDC
	private void FixedUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		float value = 2f + 0.5f * (float)this.extensionIndex;
		this.physGrabObject.OverrideMass(value, 0.1f);
		if (!this.physGrabObject.grabbed)
		{
			return;
		}
		this.physGrabObject.OverrideExtraTorqueStrengthDisable(0.1f);
		this.physGrabObject.OverrideTorqueStrength(Mathf.Min(4f + (float)this.extensionIndex * 0.15f, 2f), 0.1f);
		this.physGrabObject.OverrideGrabStrength(Mathf.Min(1f + 0.5f * (float)this.extensionIndex, 3f), 0.1f);
		if (this.justGrabbed && !this.IsCollidingWithGround())
		{
			this.justGrabbed = false;
			if (this.physGrabObject.playerGrabbing.Count == 1)
			{
				Quaternion turnX = Quaternion.Euler(0f, 0f, 0f);
				Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
				Quaternion identity = Quaternion.identity;
				this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
		}
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x00062BF8 File Offset: 0x00060DF8
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

	// Token: 0x06000B0C RID: 2828 RVA: 0x00062CC8 File Offset: 0x00060EC8
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

	// Token: 0x06000B0D RID: 2829 RVA: 0x00062D4C File Offset: 0x00060F4C
	private void StartAnimation()
	{
		this.startTopPosition = this.topTransformPivot.localPosition;
		this.startScale = this.plasmaBridgePivot.localScale;
		if (this.extensionIndex > 0)
		{
			this.animationCurveEval = 0f;
		}
		else
		{
			this.animationCurveEval = 1f;
		}
		this.animate = true;
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x00062DA4 File Offset: 0x00060FA4
	private void AnimateExtension()
	{
		this.animationCurveEval += Time.deltaTime * this.extendingSpeed;
		float t = this.animationCurve.Evaluate(this.animationCurveEval);
		Vector3 b = this.startTopPosition;
		b.z = (float)(this.extensionIndex * this.extensionAmount) + 1f;
		this.topTransformPivot.localPosition = Vector3.Lerp(this.startTopPosition, b, t);
		Vector3 b2 = this.startScale;
		b2.z = (float)(this.extensionIndex * this.extensionAmount);
		this.plasmaBridgePivot.localScale = Vector3.Lerp(this.startScale, b2, t);
		if (this.animationCurveEval >= 1f)
		{
			this.animate = false;
		}
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x00062E60 File Offset: 0x00061060
	private void AnimateRetraction()
	{
		this.animationCurveEval -= Time.deltaTime * 3f;
		float t = this.animationCurve.Evaluate(this.animationCurveEval);
		Vector3 a = this.topTransformPivotOriginalPosition;
		this.topTransformPivot.localPosition = Vector3.Lerp(a, this.startTopPosition, t);
		Vector3 a2 = this.plasmaBridgePivotOriginalScale;
		this.plasmaBridgePivot.localScale = Vector3.Lerp(a2, this.startScale, t);
		if (this.animationCurveEval <= 0f)
		{
			this.animate = false;
			this.plasmaBridgePivot.localScale = this.plasmaBridgePivotOriginalScale;
		}
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x00062EFA File Offset: 0x000610FA
	[PunRPC]
	public void SetExtensionIndexRPC(int _index, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.extensionIndex = _index;
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00062F0C File Offset: 0x0006110C
	private void SetExtensionIndex(int _index)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.extensionIndex == _index)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetExtensionIndexRPC(_index, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("SetExtensionIndexRPC", RpcTarget.All, new object[]
		{
			_index
		});
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x00062F64 File Offset: 0x00061164
	public void StateChange(ItemLadder.States _state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.currentState == _state)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("StateChangeRPC", RpcTarget.All, new object[]
			{
				_state
			});
			return;
		}
		this.StateChangeRPC(_state, default(PhotonMessageInfo));
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x00062FBB File Offset: 0x000611BB
	[PunRPC]
	public void StateChangeRPC(ItemLadder.States _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00062FD0 File Offset: 0x000611D0
	private void Retract()
	{
		this.colliderPivot.localScale = this.colliderPivotOriginalScale;
		this.ResetCollisionCheckPosition();
		if (!this.itemEquippable.isEquipped)
		{
			this.StartAnimation();
			return;
		}
		this.topTransformPivot.localPosition = this.topTransformPivotOriginalPosition;
		this.plasmaBridgePivot.localScale = this.plasmaBridgePivotOriginalScale;
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0006302A File Offset: 0x0006122A
	private void Extend()
	{
		if (this.flickering || this.itemBattery.batteryLife >= this.extensionBatteryDrain)
		{
			this.SetExtensionIndex(this.extensionIndex + 1);
		}
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x00063055 File Offset: 0x00061255
	private bool IsMinSize()
	{
		return this.extensionIndex == 0;
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x00063060 File Offset: 0x00061260
	private Collider[] GetCollidingColliders()
	{
		return Physics.OverlapBox(this.collisionCheck.position, this.collisionCheck.lossyScale / 2f, this.collisionCheck.rotation);
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x00063092 File Offset: 0x00061292
	private Collider[] GetCollidingCollidersGround()
	{
		return Physics.OverlapBox(this.groundCollisionCheck.position, this.groundCollisionCheck.lossyScale / 2f, this.groundCollisionCheck.rotation);
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x000630C4 File Offset: 0x000612C4
	private bool IsColliding()
	{
		foreach (Collider collider in this.GetCollidingColliders())
		{
			if (collider.gameObject.layer != LayerMask.NameToLayer("RoomVolume") && !collider.transform.IsChildOf(base.transform))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00063118 File Offset: 0x00061318
	private bool IsCollidingWithGround()
	{
		foreach (Collider collider in this.GetCollidingCollidersGround())
		{
			if (collider.gameObject.layer != LayerMask.NameToLayer("RoomVolume") && !collider.transform.IsChildOf(base.transform))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x0006316B File Offset: 0x0006136B
	private bool EPressed()
	{
		if (this.itemToggle.toggleState != this.previousToggleState)
		{
			this.previousToggleState = this.itemToggle.toggleState;
			return true;
		}
		return false;
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x00063194 File Offset: 0x00061394
	private bool EquipPressed()
	{
		if (this.itemEquippable.isEquipped != this.previousEquippedState)
		{
			this.previousEquippedState = this.itemEquippable.isEquipped;
			return true;
		}
		return false;
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x000631BD File Offset: 0x000613BD
	private bool StateChanged()
	{
		if (this.previousState != this.currentState)
		{
			this.previousPreviousState = this.previousState;
			this.previousState = this.currentState;
			return true;
		}
		return false;
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x000631E8 File Offset: 0x000613E8
	private void ResetCollisionCheckPosition()
	{
		this.collisionCheckPivot.localPosition = this.collisionCheckPivotOriginalPosition;
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x000631FC File Offset: 0x000613FC
	private void ScaleCollider()
	{
		Vector3 localScale = this.colliderPivot.localScale;
		localScale.z += (float)this.extensionAmount;
		this.colliderPivot.localScale = localScale;
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x00063234 File Offset: 0x00061434
	private void IncrementCollisionCheck()
	{
		Vector3 localPosition = this.collisionCheckPivot.localPosition;
		localPosition.z += (float)this.extensionAmount;
		this.collisionCheckPivot.localPosition = localPosition;
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0006326B File Offset: 0x0006146B
	private bool IndexChanged()
	{
		if (this.previousExtensionIndex != this.extensionIndex)
		{
			this.previousExtensionIndex = this.extensionIndex;
			return true;
		}
		return false;
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0006328A File Offset: 0x0006148A
	public void OnGrab()
	{
		if (this.currentState != ItemLadder.States.Grabbed && !this.flickering)
		{
			this.StateChange(ItemLadder.States.Grabbed);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.RunIsShop())
		{
			this.shopTimerOn = false;
			this.shopTimer = 10f;
		}
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x000632C4 File Offset: 0x000614C4
	public void OnRelease()
	{
		if (this.physGrabObject.playerGrabbing.Count == 0 && !this.flickering && this.currentState != ItemLadder.States.OutOfBattery)
		{
			this.StateChange(ItemLadder.States.Neutral);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.RunIsShop() && this.extensionIndex > 0)
		{
			this.shopTimerOn = true;
			this.shopTimer = 10f;
		}
		this.justGrabbed = false;
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x0006332C File Offset: 0x0006152C
	public void FlickerBridge()
	{
		float b = 0.7f + Mathf.PingPong(Time.time, 0.1f);
		this.bridge.material.SetColor("_Color", Color.red * b * 3f);
	}

	// Token: 0x040011CE RID: 4558
	[Header("Assignables")]
	public Transform topTransformPivot;

	// Token: 0x040011CF RID: 4559
	public Transform bottomTransform;

	// Token: 0x040011D0 RID: 4560
	public Transform plasmaBridgePivot;

	// Token: 0x040011D1 RID: 4561
	[Space]
	public MeshRenderer grabIcon;

	// Token: 0x040011D2 RID: 4562
	public MeshRenderer bridge;

	// Token: 0x040011D3 RID: 4563
	[Space]
	public Transform collisionCheckPivot;

	// Token: 0x040011D4 RID: 4564
	public Transform collisionCheck;

	// Token: 0x040011D5 RID: 4565
	public Transform groundCollisionCheck;

	// Token: 0x040011D6 RID: 4566
	[Space]
	public Transform colliderPivot;

	// Token: 0x040011D7 RID: 4567
	[Space]
	public AnimationCurve animationCurve;

	// Token: 0x040011D8 RID: 4568
	[Header("Extension variables")]
	public float extendingSpeed = 1f;

	// Token: 0x040011D9 RID: 4569
	public float extensionBatteryDrain = 5f;

	// Token: 0x040011DA RID: 4570
	public int extensionAmount = 2;

	// Token: 0x040011DB RID: 4571
	[Header("Sounds")]
	public Sound grabSound;

	// Token: 0x040011DC RID: 4572
	public Sound releaseSound;

	// Token: 0x040011DD RID: 4573
	public Sound extendSound;

	// Token: 0x040011DE RID: 4574
	public Sound retractSound;

	// Token: 0x040011DF RID: 4575
	public Sound bridgeLoop;

	// Token: 0x040011E0 RID: 4576
	public Sound deniedSound;

	// Token: 0x040011E1 RID: 4577
	public Sound flickeringSound;

	// Token: 0x040011E2 RID: 4578
	[HideInInspector]
	public bool denied;

	// Token: 0x040011E3 RID: 4579
	private ItemLadder.States currentState = ItemLadder.States.Neutral;

	// Token: 0x040011E4 RID: 4580
	private ItemLadder.States previousState = ItemLadder.States.Neutral;

	// Token: 0x040011E5 RID: 4581
	private ItemLadder.States previousPreviousState = ItemLadder.States.Neutral;

	// Token: 0x040011E6 RID: 4582
	private Vector3 colliderPivotOriginalScale;

	// Token: 0x040011E7 RID: 4583
	private Vector3 collisionCheckPivotOriginalPosition;

	// Token: 0x040011E8 RID: 4584
	private Vector3 topTransformPivotOriginalPosition;

	// Token: 0x040011E9 RID: 4585
	private Vector3 plasmaBridgePivotOriginalScale;

	// Token: 0x040011EA RID: 4586
	private PhotonView photonView;

	// Token: 0x040011EB RID: 4587
	private ItemToggle itemToggle;

	// Token: 0x040011EC RID: 4588
	private ItemEquippable itemEquippable;

	// Token: 0x040011ED RID: 4589
	private ItemBattery itemBattery;

	// Token: 0x040011EE RID: 4590
	private PhysGrabObject physGrabObject;

	// Token: 0x040011EF RID: 4591
	private Rigidbody rb;

	// Token: 0x040011F0 RID: 4592
	private bool animate;

	// Token: 0x040011F1 RID: 4593
	private bool previousToggleState;

	// Token: 0x040011F2 RID: 4594
	private bool previousEquippedState;

	// Token: 0x040011F3 RID: 4595
	private int previousExtensionIndex;

	// Token: 0x040011F4 RID: 4596
	private int extensionIndex;

	// Token: 0x040011F5 RID: 4597
	private float deniedTime;

	// Token: 0x040011F6 RID: 4598
	private float animationCurveEval;

	// Token: 0x040011F7 RID: 4599
	private Material grabMaterial;

	// Token: 0x040011F8 RID: 4600
	private Color bridgeBaseColor;

	// Token: 0x040011F9 RID: 4601
	private Vector3 startTopPosition;

	// Token: 0x040011FA RID: 4602
	private Vector3 startScale;

	// Token: 0x040011FB RID: 4603
	private bool flickering;

	// Token: 0x040011FC RID: 4604
	private float shopTimer = 10f;

	// Token: 0x040011FD RID: 4605
	private bool shopTimerOn;

	// Token: 0x040011FE RID: 4606
	private bool justGrabbed;

	// Token: 0x040011FF RID: 4607
	private float flickeringTime = 3f;

	// Token: 0x04001200 RID: 4608
	private float flickeringTimer;

	// Token: 0x0200037B RID: 891
	public enum States
	{
		// Token: 0x04002B29 RID: 11049
		Denied,
		// Token: 0x04002B2A RID: 11050
		Neutral,
		// Token: 0x04002B2B RID: 11051
		Grabbed,
		// Token: 0x04002B2C RID: 11052
		OutOfBattery,
		// Token: 0x04002B2D RID: 11053
		Flickering
	}
}
