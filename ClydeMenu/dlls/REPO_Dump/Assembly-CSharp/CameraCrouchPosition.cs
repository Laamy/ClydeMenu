using System;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class CameraCrouchPosition : MonoBehaviour
{
	// Token: 0x060000C6 RID: 198 RVA: 0x0000794C File Offset: 0x00005B4C
	private void Awake()
	{
		CameraCrouchPosition.instance = this;
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00007954 File Offset: 0x00005B54
	private void Start()
	{
		this.Player = PlayerController.instance;
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00007964 File Offset: 0x00005B64
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.Player.Crouching)
		{
			this.Active = true;
		}
		else
		{
			this.Active = false;
		}
		if (this.Active != this.ActivePrev)
		{
			if (this.Active)
			{
				PlayerController.instance.playerAvatarScript.StandToCrouch();
			}
			else
			{
				PlayerController.instance.playerAvatarScript.CrouchToStand();
			}
			GameDirector.instance.CameraShake.Shake(2f, 0.1f);
			this.CameraCrouchRotation.RotationLerp = 0f;
			this.ActivePrev = this.Active;
		}
		float num = this.PositionSpeed * PlayerController.instance.playerAvatarScript.playerAvatarVisuals.animationSpeedMultiplier;
		if (this.Player.Sliding)
		{
			num *= 2f;
		}
		if (this.Active)
		{
			this.Lerp += Time.deltaTime * num;
		}
		else
		{
			this.Lerp -= Time.deltaTime * num;
		}
		this.Lerp = Mathf.Clamp01(this.Lerp);
		base.transform.localPosition = new Vector3(0f, this.AnimationCurve.Evaluate(this.Lerp) * this.Position, 0f);
	}

	// Token: 0x04000205 RID: 517
	public static CameraCrouchPosition instance;

	// Token: 0x04000206 RID: 518
	public CameraCrouchRotation CameraCrouchRotation;

	// Token: 0x04000207 RID: 519
	[Space]
	public float Position;

	// Token: 0x04000208 RID: 520
	public float PositionSpeed;

	// Token: 0x04000209 RID: 521
	public AnimationCurve AnimationCurve;

	// Token: 0x0400020A RID: 522
	internal float Lerp;

	// Token: 0x0400020B RID: 523
	[HideInInspector]
	public bool Active;

	// Token: 0x0400020C RID: 524
	internal bool ActivePrev;

	// Token: 0x0400020D RID: 525
	private PlayerController Player;
}
