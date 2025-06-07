using System;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class GameplayManager : MonoBehaviour
{
	// Token: 0x0600094C RID: 2380 RVA: 0x00058644 File Offset: 0x00056844
	private void Awake()
	{
		GameplayManager.instance = this;
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0005864C File Offset: 0x0005684C
	private void Start()
	{
		this.UpdateAll();
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x00058654 File Offset: 0x00056854
	private void Update()
	{
		if (this.cameraAnimationOverrideTimer > 0f)
		{
			this.cameraAnimationOverrideTimer -= Time.deltaTime;
			if (this.cameraAnimationOverrideTimer <= 0f)
			{
				this.UpdateCameraAnimation();
			}
		}
		if (this.cameraNoiseOverrideTimer > 0f)
		{
			this.cameraNoiseOverrideTimer -= Time.deltaTime;
		}
		else
		{
			this.cameraNoise = DataDirector.instance.SettingValueFetchFloat(DataDirector.Setting.CameraNoise);
		}
		if (this.cameraShakeOverrideTimer > 0f)
		{
			this.cameraShakeOverrideTimer -= Time.deltaTime;
			return;
		}
		this.cameraShake = DataDirector.instance.SettingValueFetchFloat(DataDirector.Setting.CameraShake);
		if (SpectateCamera.instance)
		{
			if (SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
			{
				this.cameraShake *= 0.1f;
				return;
			}
			this.cameraShake *= 0.5f;
		}
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00058736 File Offset: 0x00056936
	public void UpdateAll()
	{
		this.UpdateTips();
		this.UpdateCameraSmoothing();
		this.UpdateAimSensitivity();
		this.UpdateCameraAnimation();
		this.UpdatePlayerNames();
		this.UpdateAimInvertVertical();
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x0005875C File Offset: 0x0005695C
	public void UpdateTips()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Tips) == 1)
		{
			this.tips = true;
			return;
		}
		this.tips = false;
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x0005877C File Offset: 0x0005697C
	public void UpdateCameraSmoothing()
	{
		this.cameraSmoothing = (float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.CameraSmoothing);
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00058791 File Offset: 0x00056991
	public void UpdateAimSensitivity()
	{
		this.aimSensitivity = (float)DataDirector.instance.SettingValueFetch(DataDirector.Setting.AimSensitivity);
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x000587A8 File Offset: 0x000569A8
	public void UpdateCameraAnimation()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.CameraAnimation))
		{
		case 0:
			this.cameraAnimation = 0f;
			return;
		case 1:
			this.cameraAnimation = 0.25f;
			return;
		case 2:
			this.cameraAnimation = 0.5f;
			return;
		case 3:
			this.cameraAnimation = 0.75f;
			return;
		case 4:
			this.cameraAnimation = 1f;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00058818 File Offset: 0x00056A18
	public void UpdatePlayerNames()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.PlayerNames) == 1)
		{
			this.playerNames = true;
			return;
		}
		this.playerNames = false;
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x00058838 File Offset: 0x00056A38
	public void UpdateAimInvertVertical()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.AimInvertVertical) == 1)
		{
			this.aimInvertVertical = true;
			return;
		}
		this.aimInvertVertical = false;
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00058858 File Offset: 0x00056A58
	public void OverrideCameraAnimation(float _value, float _time)
	{
		this.cameraAnimation = _value;
		this.cameraAnimationOverrideTimer = _time;
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00058868 File Offset: 0x00056A68
	public void OverrideCameraNoise(float _value, float _time)
	{
		this.cameraNoise = _value;
		this.cameraNoiseOverrideTimer = _time;
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00058878 File Offset: 0x00056A78
	public void OverrideCameraShake(float _value, float _time)
	{
		this.cameraShake = _value;
		this.cameraShakeOverrideTimer = _time;
	}

	// Token: 0x040010DF RID: 4319
	public static GameplayManager instance;

	// Token: 0x040010E0 RID: 4320
	internal bool tips;

	// Token: 0x040010E1 RID: 4321
	internal bool playerNames;

	// Token: 0x040010E2 RID: 4322
	internal bool aimInvertVertical;

	// Token: 0x040010E3 RID: 4323
	internal float cameraSmoothing;

	// Token: 0x040010E4 RID: 4324
	internal float aimSensitivity;

	// Token: 0x040010E5 RID: 4325
	internal float cameraAnimation;

	// Token: 0x040010E6 RID: 4326
	internal float cameraNoise;

	// Token: 0x040010E7 RID: 4327
	internal float cameraShake;

	// Token: 0x040010E8 RID: 4328
	private float cameraAnimationOverrideTimer;

	// Token: 0x040010E9 RID: 4329
	private float cameraNoiseOverrideTimer;

	// Token: 0x040010EA RID: 4330
	private float cameraShakeOverrideTimer;
}
