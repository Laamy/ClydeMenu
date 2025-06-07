using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class CameraZoom : MonoBehaviour
{
	// Token: 0x060000BC RID: 188 RVA: 0x00007398 File Offset: 0x00005598
	private void Awake()
	{
		CameraZoom.Instance = this;
		this.zoomPrev = this.playerZoomDefault;
		this.zoomNew = this.playerZoomDefault;
	}

	// Token: 0x060000BD RID: 189 RVA: 0x000073B8 File Offset: 0x000055B8
	public void OverrideZoomSet(float zoom, float time, float speedIn, float speedOut, GameObject obj, int priority)
	{
		if (priority > this.OverrideZoomPriority)
		{
			return;
		}
		if (obj != this.OverrideZoomObject)
		{
			this.zoomLerp = 0f;
			this.zoomPrev = this.zoomCurrent;
		}
		this.zoomNew = zoom;
		this.OverrideZoomObject = obj;
		this.OverrideZoomTimer = time;
		this.OverrideZoomSpeedIn = speedIn;
		this.OverrideZoomSpeedOut = speedOut;
		this.OverrideZoomPriority = priority;
		this.OverrideActive = true;
	}

	// Token: 0x060000BE RID: 190 RVA: 0x0000742C File Offset: 0x0000562C
	private void Update()
	{
		if (SpectateCamera.instance)
		{
			return;
		}
		if (!LevelGenerator.Instance.Generated || this.PlayerController.playerAvatarScript.isDisabled)
		{
			return;
		}
		if (this.OverrideZoomTimer > 0f)
		{
			this.OverrideZoomTimer -= Time.deltaTime;
			this.zoomLerp += Time.deltaTime * this.OverrideZoomSpeedIn;
		}
		else if (this.OverrideZoomTimer <= 0f)
		{
			if (this.OverrideActive)
			{
				this.OverrideActive = false;
				this.OverrideZoomObject = null;
				this.OverrideZoomPriority = 999;
				this.zoomLerp = 0f;
				this.zoomPrev = this.zoomCurrent;
				this.zoomNew = this.playerZoomDefault;
			}
			this.zoomLerp += Time.deltaTime * this.OverrideZoomSpeedOut;
		}
		this.zoomLerp = Mathf.Clamp01(this.zoomLerp);
		if (this.PlayerController.CanSlide)
		{
			float num = this.SprintZoom + (float)StatsManager.instance.playerUpgradeSpeed[PlayerController.instance.playerAvatarScript.steamID] * 2f;
			num *= GameplayManager.instance.cameraAnimation;
			float b = Mathf.Lerp(0f, num, this.PlayerController.SprintSpeedLerp);
			this.SprintZoomCurrent = Mathf.Lerp(this.SprintZoomCurrent, b, 2f * Time.deltaTime);
		}
		else
		{
			this.SprintZoomCurrent = Mathf.Lerp(this.SprintZoomCurrent, 0f, 2f * Time.deltaTime);
		}
		if (this.PlayerController.playerAvatarScript.isTumbling)
		{
			float num2 = this.PlayerController.playerAvatarScript.tumble.physGrabObject.rbVelocity.magnitude * 5f;
			num2 = Mathf.Clamp(num2, 0f, 30f);
			num2 *= GameplayManager.instance.cameraAnimation;
			this.TumbleVelocityZoom = Mathf.Lerp(this.TumbleVelocityZoom, num2, 2f * Time.deltaTime);
		}
		else
		{
			this.TumbleVelocityZoom = Mathf.Lerp(this.TumbleVelocityZoom, 0f, 2f * Time.deltaTime);
		}
		this.zoomCurrent = Mathf.LerpUnclamped(this.zoomPrev, this.zoomNew, this.OverrideZoomCurve.Evaluate(this.zoomLerp));
		if (SemiFunc.MenuLevel() && CameraNoPlayerTarget.instance)
		{
			this.zoomCurrent = CameraNoPlayerTarget.instance.cam.fieldOfView;
		}
		foreach (Camera camera in this.cams)
		{
			camera.fieldOfView = this.zoomCurrent + this.SprintZoomCurrent + this.TumbleVelocityZoom;
		}
	}

	// Token: 0x040001E3 RID: 483
	public static CameraZoom Instance;

	// Token: 0x040001E4 RID: 484
	public PlayerController PlayerController;

	// Token: 0x040001E5 RID: 485
	public List<Camera> cams;

	// Token: 0x040001E6 RID: 486
	public CameraTarget camController;

	// Token: 0x040001E7 RID: 487
	public AnimNoise camNoise;

	// Token: 0x040001E8 RID: 488
	public float playerZoomDefault;

	// Token: 0x040001E9 RID: 489
	public float SprintZoom;

	// Token: 0x040001EA RID: 490
	private float SprintZoomCurrent;

	// Token: 0x040001EB RID: 491
	private float TumbleVelocityZoom;

	// Token: 0x040001EC RID: 492
	private float zoomLerp;

	// Token: 0x040001ED RID: 493
	private float zoomPrev;

	// Token: 0x040001EE RID: 494
	private float zoomCurrent;

	// Token: 0x040001EF RID: 495
	private float zoomNew;

	// Token: 0x040001F0 RID: 496
	private GameObject OverrideZoomObject;

	// Token: 0x040001F1 RID: 497
	private float OverrideZoomTimer;

	// Token: 0x040001F2 RID: 498
	private float OverrideZoomSpeedIn;

	// Token: 0x040001F3 RID: 499
	private float OverrideZoomSpeedOut;

	// Token: 0x040001F4 RID: 500
	public AnimationCurve OverrideZoomCurve;

	// Token: 0x040001F5 RID: 501
	private int OverrideZoomPriority = 999;

	// Token: 0x040001F6 RID: 502
	private bool OverrideActive;
}
