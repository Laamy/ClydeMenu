using System;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class PlayerEyes : MonoBehaviour
{
	// Token: 0x06000FD2 RID: 4050 RVA: 0x00091100 File Offset: 0x0008F300
	private void Start()
	{
		this.playerAvatarVisuals = base.GetComponent<PlayerAvatarVisuals>();
		this.playerAvatar = this.playerAvatarVisuals.playerAvatar;
		this.playerAvatarRightArm = base.GetComponent<PlayerAvatarRightArm>();
		if (!this.playerAvatarVisuals.isMenuAvatar && (!GameManager.Multiplayer() || (this.playerAvatar && this.playerAvatar.photonView.IsMine)))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00091170 File Offset: 0x0008F370
	private void MenuLookAt()
	{
		if (!this.playerAvatarVisuals.isMenuAvatar)
		{
			return;
		}
		this.Override(this.menuAvatarPointer.position, 0.1f, this.menuAvatarPointer.gameObject);
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x000911A4 File Offset: 0x0008F3A4
	private void LookAtTransform(Transform _otherPhysGrabPoint, bool _softOverride)
	{
		this.lookAtActive = false;
		if (this.overrideActive)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = this.overridePosition;
		}
		else if (this.playerAvatarRightArm.mapToolController.Active && this.playerAvatarRightArm.mapToolController.HideLerp <= 0f)
		{
			this.lookAt.transform.position = this.playerAvatarRightArm.mapToolController.PlayerLookTarget.position;
		}
		else if (this.playerAvatarRightArm.physGrabBeam.lineRenderer.enabled && this.playerAvatar.physGrabber.grabbedObjectTransform && !this.playerAvatar.physGrabber.grabbedObjectTransform.GetComponent<PhysGrabCart>())
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = this.playerAvatarRightArm.physGrabBeam.PhysGrabPoint.position;
		}
		else if (this.playerAvatar.healthGrab.staticGrabObject.playerGrabbing.Count > 0)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = this.playerAvatarVisuals.transform.position + this.playerAvatarVisuals.transform.forward * 2f;
		}
		else if (this.playerAvatar.isTumbling && this.playerAvatar.tumble.physGrabObject.playerGrabbing.Count > 0)
		{
			this.lookAtActive = true;
			Vector3 position = this.playerAvatar.tumble.physGrabObject.playerGrabbing[0].playerAvatar.playerAvatarVisuals.headLookAtTransform.position;
			if (this.playerAvatar.isLocal)
			{
				position = this.playerAvatar.localCameraPosition;
			}
			this.lookAt.transform.position = position;
		}
		else if (_otherPhysGrabPoint)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = _otherPhysGrabPoint.position;
		}
		else if (_softOverride)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = this.overrideSoftPosition;
		}
		else if (this.currentTalker)
		{
			Vector3 position2 = this.currentTalker.playerAvatarVisuals.headLookAtTransform.position;
			if (this.currentTalker.isLocal)
			{
				position2 = this.currentTalker.localCameraPosition;
			}
			this.lookAtActive = true;
			this.lookAt.transform.position = position2;
		}
		else if (this.playerAvatar.isTumbling)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = base.transform.position + this.playerAvatar.localCameraTransform.forward * 2f;
		}
		else
		{
			this.lookAt.transform.position = this.targetIdle.position;
		}
		this.lookAt.transform.rotation = this.targetIdle.rotation;
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x000914D8 File Offset: 0x0008F6D8
	private void ClosestPhysGrabPoint()
	{
		this.otherPhysGrabPoint = null;
		float num = 6f;
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar != this.playerAvatar && playerAvatar.physGrabber.physGrabBeamComponent.lineRenderer.enabled && playerAvatar.physGrabber.grabbedObjectTransform && !playerAvatar.physGrabber.grabbedObjectTransform.GetComponent<PhysGrabCart>())
			{
				float num2 = Vector3.Distance(playerAvatar.physGrabber.physGrabPoint.position, this.eyeLeft.position);
				if (num2 < num)
				{
					num = num2;
					this.otherPhysGrabPoint = playerAvatar.physGrabber.physGrabPoint;
				}
			}
		}
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000915C4 File Offset: 0x0008F7C4
	private void EyesLead()
	{
		if (this.playerAvatarVisuals.turnDifference > 1f && this.playerAvatarVisuals.turnDirection != 0f)
		{
			this.eyeSideAmount = -this.playerAvatarVisuals.turnDirection * 50f;
			this.eyeLeadTimer = 0.5f;
		}
		else
		{
			this.eyeSideAmount = 0f;
		}
		if (this.playerAvatarVisuals.upDifference > 2f && this.playerAvatarVisuals.upDirection != 0f)
		{
			this.eyeUpAmount = this.playerAvatarVisuals.upDifference * -this.playerAvatarVisuals.upDirection * 20f;
			this.eyeLeadTimer = 0.5f;
		}
		else
		{
			this.eyeUpAmount = 0f;
		}
		if (this.eyeLeadTimer > 0f)
		{
			this.eyeLeadTimer -= this.deltaTime;
			Vector3 localEulerAngles = new Vector3(this.eyeUpAmount, this.eyeSideAmount, 0f);
			Quaternion localRotation = this.targetLead.localRotation;
			this.targetLead.localEulerAngles = localEulerAngles;
			Quaternion localRotation2 = this.targetLead.localRotation;
			this.targetLead.localRotation = localRotation;
			this.targetLead.localRotation = Quaternion.Lerp(localRotation, localRotation2, this.deltaTime * 5f);
			return;
		}
		this.eyeSideAmount = 0f;
		this.eyeUpAmount = 0f;
		this.targetLead.localRotation = Quaternion.Lerp(this.targetLead.localRotation, Quaternion.identity, this.deltaTime * 20f);
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x00091750 File Offset: 0x0008F950
	private void Update()
	{
		if (this.playerAvatarVisuals.isMenuAvatar && !this.playerAvatar)
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (!this.playerAvatarVisuals.isMenuAvatar && (!LevelGenerator.Instance.Generated || this.playerAvatar.playerHealth.hurtFreeze))
		{
			return;
		}
		this.deltaTime = this.playerAvatarVisuals.deltaTime;
		this.MenuLookAt();
		float num = this.playerExpressions.pupilLeftScaleAmount * this.pupilSizeMultiplier * this.pupilLeftSizeMultiplier;
		float num2 = this.playerExpressions.pupilRightScaleAmount * this.pupilSizeMultiplier * this.pupilRightSizeMultiplier;
		num = Mathf.Clamp(num, 0.25f, 2.5f);
		num2 = Mathf.Clamp(num2, 0.25f, 2.5f);
		this.pupilLeft.localScale = Vector3.one * num;
		this.pupilRight.localScale = Vector3.one * num2;
		this.EyesLead();
		this.ClosestPhysGrabPoint();
		if (this.currentTalker && !this.currentTalker.voiceChat.isTalking)
		{
			this.currentTalkerTime = 0f;
		}
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (!playerAvatar.isDisabled && playerAvatar.voiceChat && playerAvatar.voiceChat.isTalking && Vector3.Distance(base.transform.position, playerAvatar.transform.position) < 6f)
			{
				this.currentTalkerTimer = Random.Range(2f, 4f);
				if (playerAvatar != this.playerAvatar && playerAvatar.voiceChat.isTalkingStartTime > this.currentTalkerTime)
				{
					this.currentTalker = playerAvatar;
					this.currentTalkerTime = playerAvatar.voiceChat.isTalkingStartTime;
				}
			}
		}
		if (this.currentTalkerTimer > 0f)
		{
			this.currentTalkerTimer -= this.deltaTime;
			if (this.currentTalkerTimer <= 0f)
			{
				this.currentTalker = null;
			}
		}
		bool softOverride = false;
		if (this.overrideSoftActive)
		{
			softOverride = true;
			if (this.overrideSoftObject)
			{
				PlayerAvatar component = this.overrideSoftObject.GetComponent<PlayerAvatar>();
				if (component && component == this.playerAvatar)
				{
					softOverride = false;
				}
				else
				{
					PlayerTumble component2 = this.overrideSoftObject.GetComponent<PlayerTumble>();
					if (component2 && component2.playerAvatar == this.playerAvatar)
					{
						softOverride = false;
					}
				}
			}
		}
		this.LookAtTransform(this.otherPhysGrabPoint, softOverride);
		if (this.overrideSoftTimer > 0f)
		{
			this.overrideSoftTimer -= this.deltaTime;
			if (this.overrideSoftTimer <= 0f)
			{
				this.overrideSoftActive = false;
			}
		}
		if (this.overrideTimer > 0f)
		{
			this.overrideTimer -= this.deltaTime;
			if (this.overrideTimer <= 0f)
			{
				this.overrideActive = false;
			}
		}
		this.EyeLookAt(ref this.eyeRight, ref this.springQuaternionRight, true, 50f, 30f);
		this.EyeLookAt(ref this.eyeLeft, ref this.springQuaternionLeft, true, 50f, 30f);
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x00091AB8 File Offset: 0x0008FCB8
	public void Override(Vector3 _position, float _time, GameObject _obj)
	{
		if (this.overrideActive && _obj != this.overrideObject)
		{
			return;
		}
		this.overrideActive = true;
		this.overrideObject = _obj;
		this.overridePosition = _position;
		this.overrideTimer = _time;
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x00091AED File Offset: 0x0008FCED
	public void OverrideSoft(Vector3 _position, float _time, GameObject _obj)
	{
		if (this.overrideSoftActive && _obj != this.overrideSoftObject)
		{
			return;
		}
		this.overrideSoftActive = true;
		this.overrideSoftObject = _obj;
		this.overrideSoftPosition = _position;
		this.overrideSoftTimer = _time;
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x00091B24 File Offset: 0x0008FD24
	public void EyeLookAt(ref Transform _eyeTransform, ref SpringQuaternion _springQuaternion, bool _useSpring, float _clampX, float _clamY)
	{
		Quaternion localRotation = _eyeTransform.localRotation;
		Vector3 forward = SemiFunc.ClampDirection(this.lookAt.position - _eyeTransform.transform.position, this.lookAt.forward, _clampX);
		_eyeTransform.rotation = Quaternion.LookRotation(forward);
		float num = _eyeTransform.localEulerAngles.y;
		if (num > _clamY && num < 180f)
		{
			num = _clamY;
		}
		else if (num < 360f - _clamY && num > 180f)
		{
			num = 360f - _clamY;
		}
		else if (num < -_clamY)
		{
			num = -_clamY;
		}
		_eyeTransform.localEulerAngles = new Vector3(_eyeTransform.localEulerAngles.x, num, _eyeTransform.localEulerAngles.z);
		Quaternion localRotation2 = _eyeTransform.localRotation;
		_eyeTransform.localRotation = localRotation;
		if (_useSpring)
		{
			_eyeTransform.localRotation = SemiFunc.SpringQuaternionGet(_springQuaternion, localRotation2, this.deltaTime);
			return;
		}
		_eyeTransform.localRotation = localRotation2;
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x00091C14 File Offset: 0x0008FE14
	private void OnDrawGizmos()
	{
		if (!this.debugDraw)
		{
			return;
		}
		float d = 0.075f;
		Gizmos.color = new Color(1f, 0.93f, 0.99f, 0.6f);
		Gizmos.matrix = this.lookAt.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one * d);
		Gizmos.color = new Color(0f, 1f, 0.98f, 0.3f);
		Gizmos.DrawCube(Vector3.zero, Vector3.one * d);
	}

	// Token: 0x04001AD6 RID: 6870
	public bool debugDraw;

	// Token: 0x04001AD7 RID: 6871
	private PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x04001AD8 RID: 6872
	private PlayerAvatar playerAvatar;

	// Token: 0x04001AD9 RID: 6873
	private PlayerAvatarRightArm playerAvatarRightArm;

	// Token: 0x04001ADA RID: 6874
	public PlayerExpression playerExpressions;

	// Token: 0x04001ADB RID: 6875
	public Transform menuAvatarPointer;

	// Token: 0x04001ADC RID: 6876
	public Transform eyeLeft;

	// Token: 0x04001ADD RID: 6877
	public Transform eyeRight;

	// Token: 0x04001ADE RID: 6878
	public Transform pupilLeft;

	// Token: 0x04001ADF RID: 6879
	public Transform pupilRight;

	// Token: 0x04001AE0 RID: 6880
	[Space]
	public Transform targetIdle;

	// Token: 0x04001AE1 RID: 6881
	public Transform targetLead;

	// Token: 0x04001AE2 RID: 6882
	[Space]
	public Transform lookAt;

	// Token: 0x04001AE3 RID: 6883
	public SpringQuaternion springQuaternionLeft;

	// Token: 0x04001AE4 RID: 6884
	public SpringQuaternion springQuaternionRight;

	// Token: 0x04001AE5 RID: 6885
	private Transform otherPhysGrabPoint;

	// Token: 0x04001AE6 RID: 6886
	private float eyeLeadTimer;

	// Token: 0x04001AE7 RID: 6887
	private float eyeSideAmount;

	// Token: 0x04001AE8 RID: 6888
	private float eyeUpAmount;

	// Token: 0x04001AE9 RID: 6889
	internal bool lookAtActive;

	// Token: 0x04001AEA RID: 6890
	private PlayerAvatar currentTalker;

	// Token: 0x04001AEB RID: 6891
	private float currentTalkerTime;

	// Token: 0x04001AEC RID: 6892
	private float currentTalkerTimer;

	// Token: 0x04001AED RID: 6893
	private bool overrideActive;

	// Token: 0x04001AEE RID: 6894
	private float overrideTimer;

	// Token: 0x04001AEF RID: 6895
	private Vector3 overridePosition;

	// Token: 0x04001AF0 RID: 6896
	private GameObject overrideObject;

	// Token: 0x04001AF1 RID: 6897
	private bool overrideSoftActive;

	// Token: 0x04001AF2 RID: 6898
	private float overrideSoftTimer;

	// Token: 0x04001AF3 RID: 6899
	private Vector3 overrideSoftPosition;

	// Token: 0x04001AF4 RID: 6900
	private GameObject overrideSoftObject;

	// Token: 0x04001AF5 RID: 6901
	private float deltaTime;

	// Token: 0x04001AF6 RID: 6902
	internal float pupilLeftSizeMultiplier = 1f;

	// Token: 0x04001AF7 RID: 6903
	internal float pupilRightSizeMultiplier = 1f;

	// Token: 0x04001AF8 RID: 6904
	internal float pupilSizeMultiplier = 1f;
}
