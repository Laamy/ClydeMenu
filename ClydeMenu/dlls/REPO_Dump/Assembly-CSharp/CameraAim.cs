using System;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class CameraAim : MonoBehaviour
{
	// Token: 0x06000078 RID: 120 RVA: 0x0000506C File Offset: 0x0000326C
	private void Awake()
	{
		CameraAim.Instance = this;
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00005074 File Offset: 0x00003274
	public void AimTargetSet(Vector3 position, float time, float speed, GameObject obj, int priority)
	{
		if (priority > this.AimTargetPriority)
		{
			return;
		}
		if (obj != this.AimTargetObject && this.AimTargetLerp != 0f)
		{
			return;
		}
		this.AimTargetActive = true;
		this.AimTargetObject = obj;
		this.AimTargetPosition = position;
		this.AimTargetTimer = time;
		this.AimTargetSpeed = speed;
		this.AimTargetPriority = priority;
	}

	// Token: 0x0600007A RID: 122 RVA: 0x000050D8 File Offset: 0x000032D8
	public void AimTargetSoftSet(Vector3 position, float time, float strength, float strengthNoAim, GameObject obj, int priority)
	{
		if (priority > this.AimTargetSoftPriority)
		{
			return;
		}
		if (this.AimTargetSoftObject && obj != this.AimTargetSoftObject)
		{
			return;
		}
		if (obj != this.AimTargetSoftObject)
		{
			this.PlayerAimingTimer = 0f;
		}
		this.AimTargetSoftPosition = position;
		this.AimTargetSoftTimer = time;
		this.AimTargetSoftStrength = strength;
		this.AimTargetSoftStrengthNoAim = strengthNoAim;
		this.AimTargetSoftObject = obj;
		this.AimTargetSoftPriority = priority;
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00005154 File Offset: 0x00003354
	public void CameraAimSpawn(float _rotation)
	{
		this.aimHorizontal = _rotation;
		this.playerAim = Quaternion.Euler(this.aimVertical, this.aimHorizontal, 0f);
		base.transform.localRotation = this.playerAim;
	}

	// Token: 0x0600007C RID: 124 RVA: 0x0000518A File Offset: 0x0000338A
	public void OverrideAimStop()
	{
		this.overrideAimStopTimer = 0.2f;
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00005197 File Offset: 0x00003397
	public void AdditiveAimY(float _amount)
	{
		this.additiveAimY += _amount;
	}

	// Token: 0x0600007E RID: 126 RVA: 0x000051A7 File Offset: 0x000033A7
	private void OverrideAimStopTick()
	{
		if (this.overrideAimStopTimer > 0f)
		{
			this.overrideAimStop = true;
			this.overrideAimStopTimer -= Time.deltaTime;
			return;
		}
		this.overrideAimStop = false;
	}

	// Token: 0x0600007F RID: 127 RVA: 0x000051D8 File Offset: 0x000033D8
	private void Update()
	{
		if (this.AimTargetTimer > 0f || this.AimTargetSoftTimer > 0f || this.overrideAimSmoothTimer > 0f)
		{
			this.defaultSettingAmount = Mathf.Lerp(this.defaultSettingAmount, 1f, 10f * Time.deltaTime);
		}
		else
		{
			this.defaultSettingAmount = Mathf.Lerp(this.defaultSettingAmount, 0f, 10f * Time.deltaTime);
		}
		float num = Mathf.Lerp(GameplayManager.instance.cameraSmoothing / 100f, (float)DataDirector.instance.cameraSmoothingDefault / 100f, this.defaultSettingAmount);
		float t = Mathf.Lerp(GameplayManager.instance.aimSensitivity / 100f, (float)DataDirector.instance.aimSensitivityDefault / 100f, this.defaultSettingAmount);
		this.AimSpeedMouse = Mathf.Lerp(0.2f, 4f, t);
		if (GameDirector.instance.currentState >= GameDirector.gameState.Main)
		{
			if (!GameDirector.instance.DisableInput && this.AimTargetTimer <= 0f && !this.overrideAimStop)
			{
				InputManager.instance.mouseSensitivity = 0.05f;
				Vector2 a = new Vector2(SemiFunc.InputMouseX(), SemiFunc.InputMouseY());
				Vector2 a2 = new Vector2(Input.GetAxis("Gamepad Aim X"), Input.GetAxis("Gamepad Aim Y"));
				a2 = Vector2.zero;
				if (GameplayManager.instance.aimInvertVertical)
				{
					a.y *= -1f;
				}
				if (this.AimTargetSoftTimer > 0f)
				{
					if (a.magnitude > 1f)
					{
						a = a.normalized;
					}
					else
					{
						a = Vector2.zero;
					}
					if (a2.magnitude > 0.1f)
					{
						a2 = a2.normalized;
					}
					else
					{
						a2 = Vector2.zero;
					}
				}
				else
				{
					a *= this.AimSpeedMouse;
					a2 *= this.AimSpeedGamepad * Time.deltaTime;
				}
				this.aimHorizontal += a[0];
				this.aimHorizontal += a2[0];
				if (this.aimHorizontal > 360f)
				{
					this.aimHorizontal -= 360f;
				}
				if (this.aimHorizontal < -360f)
				{
					this.aimHorizontal += 360f;
				}
				this.aimVertical += -a[1];
				this.aimVertical += -a2[1];
				this.aimVertical = Mathf.Clamp(this.aimVertical, -70f, 80f);
				this.playerAim = Quaternion.Euler(this.aimVertical, this.aimHorizontal, 0f);
				if (num != 0f)
				{
					this.playerAim = Quaternion.RotateTowards(base.transform.localRotation, this.playerAim, 10000f * Time.deltaTime);
				}
				if (a2.magnitude > 0f || a.magnitude > 0f)
				{
					this.PlayerAimingTimer = 0.1f;
				}
			}
			if (this.PlayerAimingTimer > 0f)
			{
				this.PlayerAimingTimer -= Time.deltaTime;
			}
			if (this.AimTargetTimer > 0f)
			{
				this.AimTargetTimer -= Time.deltaTime;
				this.AimTargetLerp += Time.deltaTime * this.AimTargetSpeed;
				this.AimTargetLerp = Mathf.Clamp01(this.AimTargetLerp);
			}
			else if (this.AimTargetLerp > 0f)
			{
				this.ResetPlayerAim(base.transform.localRotation);
				this.AimTargetLerp = 0f;
				this.AimTargetPriority = 999;
				this.AimTargetActive = false;
			}
			Quaternion quaternion = Quaternion.LerpUnclamped(this.playerAim, Quaternion.LookRotation(this.AimTargetPosition - base.transform.position), this.AimTargetCurve.Evaluate(this.AimTargetLerp));
			if (this.AimTargetSoftTimer > 0f && this.AimTargetTimer <= 0f)
			{
				float num2 = this.AimTargetSoftStrength;
				if (this.PlayerAimingTimer <= 0f)
				{
					num2 = this.AimTargetSoftStrengthNoAim;
				}
				this.AimTargetSoftStrengthCurrent = Mathf.Lerp(this.AimTargetSoftStrengthCurrent, num2, 10f * Time.deltaTime);
				Quaternion b = Quaternion.LookRotation(this.AimTargetSoftPosition - base.transform.position);
				quaternion = Quaternion.Lerp(quaternion, b, num2 * Time.deltaTime);
				this.AimTargetSoftTimer -= Time.deltaTime;
				if (this.AimTargetSoftTimer <= 0f)
				{
					this.AimTargetSoftObject = null;
					this.AimTargetSoftPriority = 999;
				}
			}
			float num3 = Mathf.Lerp(50f, 8f, num);
			this.aimSmoothOriginal = num3;
			if (this.overrideAimSmoothTimer > 0f)
			{
				num3 = this.overrideAimSmooth;
			}
			base.transform.localRotation = Quaternion.Euler(base.transform.localRotation.eulerAngles.x + this.additiveAimY, base.transform.localRotation.eulerAngles.y, base.transform.localRotation.eulerAngles.z);
			quaternion = Quaternion.Euler(quaternion.eulerAngles.x + this.additiveAimY, quaternion.eulerAngles.y, quaternion.eulerAngles.z);
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, quaternion, num3 * Time.deltaTime);
			this.ResetPlayerAim(quaternion);
		}
		if (SemiFunc.MenuLevel() && CameraNoPlayerTarget.instance)
		{
			base.transform.localRotation = CameraNoPlayerTarget.instance.transform.rotation;
		}
		if (this.overrideAimSmoothTimer > 0f)
		{
			this.overrideAimSmoothTimer -= Time.deltaTime;
		}
		this.additiveAimY = 0f;
		this.OverrideAimStopTick();
	}

	// Token: 0x06000080 RID: 128 RVA: 0x000057CC File Offset: 0x000039CC
	private void ResetPlayerAim(Quaternion _rotation)
	{
		if (_rotation.eulerAngles.x > 180f)
		{
			this.aimVertical = _rotation.eulerAngles.x - 360f;
		}
		else
		{
			this.aimVertical = _rotation.eulerAngles.x;
		}
		this.aimHorizontal = _rotation.eulerAngles.y;
		this.playerAim = _rotation;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00005831 File Offset: 0x00003A31
	public void OverrideAimSmooth(float _smooth, float _time)
	{
		this.overrideAimSmooth = _smooth;
		this.overrideAimSmoothTimer = _time;
	}

	// Token: 0x0400012E RID: 302
	public static CameraAim Instance;

	// Token: 0x0400012F RID: 303
	public CameraTarget camController;

	// Token: 0x04000130 RID: 304
	public Transform playerTransform;

	// Token: 0x04000131 RID: 305
	public float AimSpeedMouse = 1f;

	// Token: 0x04000132 RID: 306
	public float AimSpeedGamepad = 1f;

	// Token: 0x04000133 RID: 307
	private float aimVertical;

	// Token: 0x04000134 RID: 308
	private float aimHorizontal;

	// Token: 0x04000135 RID: 309
	internal float aimSmoothOriginal = 2f;

	// Token: 0x04000136 RID: 310
	private Quaternion playerAim = Quaternion.identity;

	// Token: 0x04000137 RID: 311
	private Vector3 AimTargetPosition = Vector3.zero;

	// Token: 0x04000138 RID: 312
	public AnimationCurve AimTargetCurve;

	// Token: 0x04000139 RID: 313
	[Space]
	public bool AimTargetActive;

	// Token: 0x0400013A RID: 314
	private float AimTargetTimer;

	// Token: 0x0400013B RID: 315
	private float AimTargetSpeed;

	// Token: 0x0400013C RID: 316
	private float AimTargetLerp;

	// Token: 0x0400013D RID: 317
	private GameObject AimTargetObject;

	// Token: 0x0400013E RID: 318
	private int AimTargetPriority = 999;

	// Token: 0x0400013F RID: 319
	private bool AimTargetSoftActive;

	// Token: 0x04000140 RID: 320
	private float AimTargetSoftTimer;

	// Token: 0x04000141 RID: 321
	private float AimTargetSoftStrengthCurrent;

	// Token: 0x04000142 RID: 322
	private float AimTargetSoftStrength;

	// Token: 0x04000143 RID: 323
	private float AimTargetSoftStrengthNoAim;

	// Token: 0x04000144 RID: 324
	private Vector3 AimTargetSoftPosition;

	// Token: 0x04000145 RID: 325
	private GameObject AimTargetSoftObject;

	// Token: 0x04000146 RID: 326
	private int AimTargetSoftPriority = 999;

	// Token: 0x04000147 RID: 327
	private float overrideAimStopTimer;

	// Token: 0x04000148 RID: 328
	internal bool overrideAimStop;

	// Token: 0x04000149 RID: 329
	private float PlayerAimingTimer;

	// Token: 0x0400014A RID: 330
	private float overrideAimSmooth;

	// Token: 0x0400014B RID: 331
	private float overrideAimSmoothTimer;

	// Token: 0x0400014C RID: 332
	private float defaultSettingAmount;

	// Token: 0x0400014D RID: 333
	private float additiveAimY;
}
