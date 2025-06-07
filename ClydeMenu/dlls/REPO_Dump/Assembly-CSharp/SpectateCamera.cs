using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class SpectateCamera : MonoBehaviour
{
	// Token: 0x06001051 RID: 4177 RVA: 0x000963F0 File Offset: 0x000945F0
	private void Awake()
	{
		SpectateCamera.instance = this;
		this.normalTransformPivot.parent = null;
		this.MainCamera = GameDirector.instance.MainCamera;
		foreach (Camera camera in this.MainCamera.GetComponentsInChildren<Camera>())
		{
			if (camera != this.MainCamera)
			{
				this.TopCamera = camera;
			}
		}
		this.ParentObject = CameraNoise.Instance.transform;
		this.PreviousParent = this.ParentObject.parent;
		this.ParentObject.parent = base.transform;
		this.ParentObject.localPosition = Vector3.zero;
		this.ParentObject.localRotation = Quaternion.identity;
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x000964A4 File Offset: 0x000946A4
	private void OnDestroy()
	{
		QualitySettings.shadowDistance = 15f;
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x000964B0 File Offset: 0x000946B0
	private void LateUpdate()
	{
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideOvercharge();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideAim();
		SemiFunc.UIShowSpectate();
		MissionUI.instance.Hide();
		SpectateCamera.State state = this.currentState;
		if (state != SpectateCamera.State.Death)
		{
			if (state == SpectateCamera.State.Normal)
			{
				this.StateNormal();
			}
		}
		else
		{
			this.StateDeath();
		}
		this.RoomVolumeLogic();
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x0009650C File Offset: 0x0009470C
	private void StateDeath()
	{
		if (this.stateImpulse)
		{
			this.MainCamera.transform.localPosition = new Vector3(0f, 0f, -50f);
			this.MainCamera.transform.localRotation = Quaternion.identity;
			this.MainCamera.nearClipPlane = 0.01f;
			this.previousFarClipPlane = this.MainCamera.farClipPlane;
			this.MainCamera.farClipPlane = 70f;
			this.MainCamera.farClipPlane = 90f;
			this.deathCameraNearClipPlane = 70f;
			this.MainCamera.nearClipPlane = 70f;
			QualitySettings.shadowDistance = 90f;
			RenderSettings.fog = false;
			PostProcessing.Instance.SpectateSet();
			this.DeathNearClipLogic(true);
			LightManager.instance.UpdateInstant();
			CameraGlitch.Instance.PlayShort();
			this.previousFieldOfView = this.MainCamera.fieldOfView;
			this.cameraFieldOfView = 8f;
			this.MainCamera.fieldOfView = 16f;
			this.TopCamera.fieldOfView = this.MainCamera.fieldOfView;
			this.stateImpulse = false;
			if (SemiFunc.RunIsArena())
			{
				this.stateTimer = 1.5f;
			}
			else
			{
				this.stateTimer = 4f;
			}
			AudioManager.instance.AudioListener.TargetPositionTransform = this.deathPlayerSpectatePoint;
			GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
			GameDirector.instance.CameraShake.Shake(2f, 1f);
		}
		this.stateTimer -= Time.deltaTime;
		CameraNoise.Instance.Override(0.03f, 0.25f);
		this.deathCurrentY += SemiFunc.InputMouseX() * CameraAim.Instance.AimSpeedMouse;
		Vector3 position = base.transform.position;
		if (this.deathPlayerSpectatePoint)
		{
			position = this.deathPlayerSpectatePoint.position;
		}
		if (this.CheckState(SpectateCamera.State.Death))
		{
			position = this.deathPosition;
		}
		Vector3 vector = position;
		Quaternion rotation = Quaternion.Euler(88f, this.deathCurrentY, 0f);
		this.deathTargetOrbit = rotation;
		float num = Mathf.Lerp(50f, 2.5f, GameplayManager.instance.cameraSmoothing / 100f);
		this.deathSmoothOrbit = Quaternion.Slerp(this.deathSmoothOrbit, this.deathTargetOrbit, num * Time.deltaTime);
		if (this.deathOrbitInstantSet)
		{
			this.deathSmoothOrbit = this.deathTargetOrbit;
			this.deathOrbitInstantSet = false;
		}
		rotation = this.deathSmoothOrbit;
		Vector3 b = rotation * Vector3.back * 2f;
		this.deathFollowPointTarget = vector;
		this.deathFollowPoint = Vector3.SlerpUnclamped(this.deathFollowPoint, this.deathFollowPointTarget, Time.deltaTime * this.deathSpeed);
		base.transform.position = this.deathFollowPoint + b;
		this.deathSmoothLookAtPoint = Vector3.SlerpUnclamped(this.deathSmoothLookAtPoint, position, Time.deltaTime * this.deathSpeed);
		base.transform.LookAt(this.deathSmoothLookAtPoint);
		this.MainCamera.fieldOfView = Mathf.Lerp(this.MainCamera.fieldOfView, this.cameraFieldOfView, Time.deltaTime * 10f);
		this.TopCamera.fieldOfView = this.MainCamera.fieldOfView;
		this.DeathNearClipLogic(false);
		ExtractionPoint extractionPointCurrent = RoundDirector.instance.extractionPointCurrent;
		if (extractionPointCurrent && extractionPointCurrent.currentState != ExtractionPoint.State.Idle && extractionPointCurrent.currentState != ExtractionPoint.State.Active)
		{
			bool flag = false;
			foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
			{
				if (!playerAvatar.isDisabled)
				{
					flag = false;
					break;
				}
				if (playerAvatar.playerDeathHead.inExtractionPoint)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.stateTimer = Mathf.Clamp(this.stateTimer, 0.25f, this.stateTimer);
			}
		}
		if (this.stateTimer <= 0f)
		{
			if (SemiFunc.RunIsTutorial())
			{
				foreach (PlayerAvatar playerAvatar2 in SemiFunc.PlayerGetList())
				{
					playerAvatar2.Revive(false);
				}
				return;
			}
			bool flag2 = true;
			if (SemiFunc.RunIsArena())
			{
				flag2 = false;
				using (List<PlayerAvatar>.Enumerator enumerator = SemiFunc.PlayerGetList().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.isDisabled)
						{
							flag2 = true;
							break;
						}
					}
				}
			}
			if (flag2)
			{
				this.UpdateState(SpectateCamera.State.Normal);
				return;
			}
			this.stateTimer = 1f;
		}
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x000969D8 File Offset: 0x00094BD8
	private void StateNormal()
	{
		if (this.stateImpulse)
		{
			this.PlayerSwitch(true);
			if (!this.player)
			{
				return;
			}
			RenderSettings.fog = true;
			this.MainCamera.farClipPlane = this.previousFarClipPlane;
			this.MainCamera.fieldOfView = this.previousFieldOfView;
			this.TopCamera.fieldOfView = this.MainCamera.fieldOfView;
			this.MainCamera.transform.localPosition = Vector3.zero;
			this.MainCamera.transform.localRotation = Quaternion.identity;
			AudioManager.instance.AudioListener.TargetPositionTransform = this.MainCamera.transform;
			this.stateImpulse = false;
		}
		CameraNoise.Instance.Override(0.03f, 0.25f);
		float num = SemiFunc.InputMouseX();
		float num2 = SemiFunc.InputMouseY();
		float num3 = SemiFunc.InputScrollY();
		if (CameraAim.Instance.overrideAimStop)
		{
			num = 0f;
			num2 = 0f;
			num3 = 0f;
		}
		this.normalAimHorizontal += num * CameraAim.Instance.AimSpeedMouse * 1.5f;
		if (this.normalAimHorizontal > 360f)
		{
			this.normalAimHorizontal -= 360f;
		}
		if (this.normalAimHorizontal < -360f)
		{
			this.normalAimHorizontal += 360f;
		}
		float num4 = this.normalAimVertical;
		float num5 = -(num2 * CameraAim.Instance.AimSpeedMouse) * 1.5f;
		this.normalAimVertical += num5;
		this.normalAimVertical = Mathf.Clamp(this.normalAimVertical, -70f, 70f);
		if (num3 != 0f)
		{
			this.normalMaxDistance = Mathf.Clamp(this.normalMaxDistance - num3 * 0.0025f, this.normalMinDistance, 6f);
		}
		Vector3 vector = this.normalPreviousPosition;
		if (this.player.spectatePoint)
		{
			vector = this.player.spectatePoint.position;
		}
		else if (this.player.isTumbling)
		{
			vector = this.player.tumble.physGrabObject.centerPoint;
		}
		else if (this.player.isCrouching && !this.player.isCrawling)
		{
			vector += Vector3.up * 0.3f;
		}
		else if (!this.player.isCrawling)
		{
			vector += Vector3.down * 0.15f;
		}
		this.normalPreviousPosition = vector;
		this.normalTransformPivot.position = Vector3.Lerp(this.normalTransformPivot.position, vector, 10f * Time.deltaTime);
		Quaternion b = Quaternion.Euler(this.normalAimVertical, this.normalAimHorizontal, 0f);
		float num6 = Mathf.Lerp(50f, 6.25f, GameplayManager.instance.cameraSmoothing / 100f);
		this.normalTransformPivot.rotation = Quaternion.Lerp(this.normalTransformPivot.rotation, b, num6 * Time.deltaTime);
		this.normalTransformPivot.localRotation = Quaternion.Euler(this.normalTransformPivot.localRotation.eulerAngles.x, this.normalTransformPivot.localRotation.eulerAngles.y, 0f);
		bool flag = false;
		float num7 = this.normalMaxDistance;
		RaycastHit[] array = Physics.SphereCastAll(this.normalTransformPivot.position, 0.1f, -this.normalTransformPivot.forward, this.normalMaxDistance, SemiFunc.LayerMaskGetVisionObstruct());
		if (array.Length != 0)
		{
			foreach (RaycastHit raycastHit in array)
			{
				if (!raycastHit.transform.GetComponent<PlayerHealthGrab>() && !raycastHit.transform.GetComponent<PlayerAvatar>() && !raycastHit.transform.GetComponent<PlayerTumble>() && !raycastHit.transform.GetComponent<EnemyRigidbody>())
				{
					num7 = Mathf.Min(num7, raycastHit.distance);
					if (raycastHit.transform.CompareTag("Wall"))
					{
						flag = true;
					}
					if (raycastHit.collider.bounds.size.magnitude > 2f)
					{
						flag = true;
					}
				}
			}
			this.normalDistanceTarget = Mathf.Max(this.normalMinDistance, num7);
		}
		else
		{
			this.normalDistanceTarget = this.normalMaxDistance;
		}
		Vector3 b2 = new Vector3(0f, 0f, -this.normalDistanceTarget);
		this.normalTransformDistance.localPosition = Vector3.Lerp(this.normalTransformDistance.localPosition, b2, Time.deltaTime * 5f);
		float num8 = -this.normalTransformDistance.localPosition.z;
		Vector3 direction = this.normalTransformPivot.position - this.normalTransformDistance.position;
		float num9 = direction.magnitude;
		RaycastHit raycastHit2;
		if (Physics.SphereCast(this.normalTransformDistance.position, 0.15f, direction, out raycastHit2, this.normalMaxDistance, LayerMask.GetMask(new string[]
		{
			"PlayerVisuals"
		}), QueryTriggerInteraction.Collide))
		{
			num9 = raycastHit2.distance;
		}
		num9 = num8 - num9 - 0.1f;
		if (flag)
		{
			float num10 = Mathf.Max(num7, num9);
			this.MainCamera.nearClipPlane = Mathf.Max(num8 - num10, 0.01f);
		}
		else
		{
			this.MainCamera.nearClipPlane = 0.01f;
		}
		RenderSettings.fogStartDistance = this.MainCamera.nearClipPlane;
		base.transform.position = this.normalTransformDistance.position;
		base.transform.rotation = this.normalTransformDistance.rotation;
		if (this.player && base.transform.position.y < this.player.transform.position.y + 0.25f && num5 < 0f)
		{
			this.normalAimVertical = num4;
		}
		if (SemiFunc.InputDown(InputKey.Jump))
		{
			this.PlayerSwitch(true);
		}
		if (SemiFunc.InputDown(InputKey.SpectateNext))
		{
			this.PlayerSwitch(true);
		}
		if (SemiFunc.InputDown(InputKey.SpectatePrevious))
		{
			this.PlayerSwitch(false);
		}
		if (this.player && this.player.voiceChatFetched)
		{
			this.player.voiceChat.SpatialDisable(0.1f);
		}
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x00097036 File Offset: 0x00095236
	private void UpdateState(SpectateCamera.State _state)
	{
		if (this.currentState == _state)
		{
			return;
		}
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x0009705B File Offset: 0x0009525B
	public bool CheckState(SpectateCamera.State _state)
	{
		return this.currentState == _state;
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x00097068 File Offset: 0x00095268
	private void PlayerSwitch(bool _next = true)
	{
		if (Enumerable.All<PlayerAvatar>(GameDirector.instance.PlayerList, (PlayerAvatar p) => p.isDisabled))
		{
			return;
		}
		int i = 0;
		int num = this.currentPlayerListIndex;
		int count = GameDirector.instance.PlayerList.Count;
		while (i < count)
		{
			if (_next)
			{
				num = (num + 1) % count;
			}
			else
			{
				num = (num - 1 + count) % count;
			}
			PlayerAvatar playerAvatar = GameDirector.instance.PlayerList[num];
			if (this.player != playerAvatar && !playerAvatar.isDisabled)
			{
				this.currentPlayerListIndex = num;
				this.player = playerAvatar;
				this.normalTransformPivot.position = this.player.spectatePoint.position;
				this.normalAimHorizontal = this.player.transform.eulerAngles.y;
				this.normalAimVertical = 0f;
				this.normalTransformPivot.rotation = Quaternion.Euler(this.normalAimVertical, this.normalAimHorizontal, 0f);
				this.normalTransformPivot.localRotation = Quaternion.Euler(this.normalTransformPivot.localRotation.eulerAngles.x, this.normalTransformPivot.localRotation.eulerAngles.y, 0f);
				this.normalTransformDistance.localPosition = new Vector3(0f, 0f, -2f);
				base.transform.position = this.normalTransformDistance.position;
				base.transform.rotation = this.normalTransformDistance.rotation;
				if (SemiFunc.IsMultiplayer())
				{
					SemiFunc.HUDSpectateSetName(this.player.playerName);
				}
				SemiFunc.LightManagerSetCullTargetTransform(this.player.transform);
				CameraGlitch.Instance.PlayTiny();
				GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
				AudioManager.instance.RestartAudioLoopDistances();
				this.normalMaxDistance = 3f;
				return;
			}
			i++;
		}
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x00097271 File Offset: 0x00095471
	public void UpdatePlayer(PlayerAvatar deadPlayer)
	{
		if (deadPlayer == this.player)
		{
			this.SetDeath(deadPlayer.spectatePoint);
		}
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x00097290 File Offset: 0x00095490
	public void StopSpectate()
	{
		this.ParentObject.parent = this.PreviousParent;
		this.ParentObject.localPosition = Vector3.zero;
		this.ParentObject.localRotation = Quaternion.identity;
		this.MainCamera.nearClipPlane = 0.001f;
		this.MainCamera.farClipPlane = this.previousFarClipPlane;
		this.MainCamera.transform.localPosition = Vector3.zero;
		this.MainCamera.transform.localRotation = Quaternion.identity;
		this.MainCamera.fieldOfView = this.previousFieldOfView;
		RenderSettings.fog = true;
		RenderSettings.fogStartDistance = 0f;
		PostProcessing.Instance.SpectateReset();
		PlayerAvatar.instance.spectating = false;
		SemiFunc.LightManagerSetCullTargetTransform(PlayerAvatar.instance.transform);
		AudioManager.instance.AudioListener.TargetPositionTransform = this.MainCamera.transform;
		Object.Destroy(this.normalTransformPivot.gameObject);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x00097394 File Offset: 0x00095594
	private void DeathNearClipLogic(bool _instant)
	{
		if (!this.CheckState(SpectateCamera.State.Death))
		{
			return;
		}
		Vector3 direction = this.MainCamera.transform.position - this.deathSmoothLookAtPoint;
		RaycastHit[] array = Physics.RaycastAll(this.deathSmoothLookAtPoint, direction, direction.magnitude, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		float num = float.PositiveInfinity;
		Vector3 vector = Vector3.zero;
		foreach (RaycastHit raycastHit in array)
		{
			if (raycastHit.transform.CompareTag("Ceiling") && raycastHit.distance < num)
			{
				num = raycastHit.distance;
				vector = raycastHit.point;
			}
		}
		if (vector != Vector3.zero)
		{
			this.deathCameraNearClipPlane = (this.MainCamera.transform.position - vector).magnitude + 0.5f;
		}
		else
		{
			this.deathCameraNearClipPlane = 1f;
		}
		if (_instant)
		{
			this.MainCamera.nearClipPlane = this.deathCameraNearClipPlane;
			return;
		}
		this.MainCamera.nearClipPlane = Mathf.Lerp(this.MainCamera.nearClipPlane, this.deathCameraNearClipPlane, Time.deltaTime * 10f);
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x000974CC File Offset: 0x000956CC
	public void SetDeath(Transform _spectatePoint)
	{
		this.deathPosition = _spectatePoint.position;
		this.deathPlayerSpectatePoint = _spectatePoint;
		base.transform.position = _spectatePoint.position;
		base.transform.rotation = _spectatePoint.rotation;
		this.deathFollowPoint = this.deathPosition;
		this.deathFollowPointTarget = this.deathPosition;
		this.deathSmoothLookAtPoint = this.deathPosition;
		this.deathOrbitInstantSet = true;
		SemiFunc.LightManagerSetCullTargetTransform(this.deathPlayerSpectatePoint);
		this.deathSmoothLookAtPoint = this.deathPlayerSpectatePoint.position;
		base.transform.position = this.deathFollowPointTarget;
		this.deathFollowPoint = this.deathFollowPointTarget;
		this.deathSmoothLookAtPoint = this.deathPlayerSpectatePoint.position;
		this.DeathNearClipLogic(true);
		this.UpdateState(SpectateCamera.State.Death);
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00097594 File Offset: 0x00095794
	private void RoomVolumeLogic()
	{
		RoomVolumeCheck roomVolumeCheck = PlayerController.instance.playerAvatarScript.RoomVolumeCheck;
		roomVolumeCheck.PauseCheckTimer = 1f;
		if (this.player)
		{
			RoomVolumeCheck roomVolumeCheck2 = this.player.RoomVolumeCheck;
			roomVolumeCheck.CurrentRooms.Clear();
			roomVolumeCheck.CurrentRooms.AddRange(roomVolumeCheck2.CurrentRooms);
		}
	}

	// Token: 0x04001BD4 RID: 7124
	public static SpectateCamera instance;

	// Token: 0x04001BD5 RID: 7125
	private SpectateCamera.State currentState;

	// Token: 0x04001BD6 RID: 7126
	private float stateTimer;

	// Token: 0x04001BD7 RID: 7127
	private bool stateImpulse = true;

	// Token: 0x04001BD8 RID: 7128
	internal PlayerAvatar player;

	// Token: 0x04001BD9 RID: 7129
	private float previousFarClipPlane = 0.01f;

	// Token: 0x04001BDA RID: 7130
	private float previousFieldOfView;

	// Token: 0x04001BDB RID: 7131
	private Camera MainCamera;

	// Token: 0x04001BDC RID: 7132
	private Camera TopCamera;

	// Token: 0x04001BDD RID: 7133
	private Transform ParentObject;

	// Token: 0x04001BDE RID: 7134
	private Transform PreviousParent;

	// Token: 0x04001BDF RID: 7135
	private float cameraFieldOfView = 10f;

	// Token: 0x04001BE0 RID: 7136
	private int currentPlayerListIndex;

	// Token: 0x04001BE1 RID: 7137
	private Transform deathPlayerSpectatePoint;

	// Token: 0x04001BE2 RID: 7138
	private Vector3 deathCameraOffset;

	// Token: 0x04001BE3 RID: 7139
	private float deathCameraNearClipPlane;

	// Token: 0x04001BE4 RID: 7140
	private float deathCurrentY;

	// Token: 0x04001BE5 RID: 7141
	private Vector3 deathVelocity;

	// Token: 0x04001BE6 RID: 7142
	private float deathSpeed = 6f;

	// Token: 0x04001BE7 RID: 7143
	private Vector3 deathFollowPoint;

	// Token: 0x04001BE8 RID: 7144
	private Vector3 deathFollowPointTarget;

	// Token: 0x04001BE9 RID: 7145
	private Vector3 deathFollowPointVelocity;

	// Token: 0x04001BEA RID: 7146
	private Vector3 deathSmoothLookAtPoint;

	// Token: 0x04001BEB RID: 7147
	private Quaternion deathSmoothOrbit;

	// Token: 0x04001BEC RID: 7148
	private Quaternion deathTargetOrbit;

	// Token: 0x04001BED RID: 7149
	private bool deathOrbitInstantSet;

	// Token: 0x04001BEE RID: 7150
	private Vector3 deathPosition;

	// Token: 0x04001BEF RID: 7151
	public Transform normalTransformPivot;

	// Token: 0x04001BF0 RID: 7152
	public Transform normalTransformDistance;

	// Token: 0x04001BF1 RID: 7153
	private Vector3 normalPreviousPosition;

	// Token: 0x04001BF2 RID: 7154
	private float normalAimHorizontal;

	// Token: 0x04001BF3 RID: 7155
	private float normalAimVertical;

	// Token: 0x04001BF4 RID: 7156
	private float normalMinDistance = 1f;

	// Token: 0x04001BF5 RID: 7157
	private float normalMaxDistance = 3f;

	// Token: 0x04001BF6 RID: 7158
	private float normalDistanceTarget;

	// Token: 0x04001BF7 RID: 7159
	private float normalDistanceCheckTimer;

	// Token: 0x020003C4 RID: 964
	public enum State
	{
		// Token: 0x04002C4C RID: 11340
		Death,
		// Token: 0x04002C4D RID: 11341
		Normal
	}
}
