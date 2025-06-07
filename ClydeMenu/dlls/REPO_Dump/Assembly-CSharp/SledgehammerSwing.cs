using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public class SledgehammerSwing : MonoBehaviour
{
	// Token: 0x06000729 RID: 1833 RVA: 0x00044329 File Offset: 0x00042529
	public void Swing()
	{
		if (!this.Swinging)
		{
			this.Swinging = true;
			this.SwingSound = true;
		}
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00044341 File Offset: 0x00042541
	public void HitOutro()
	{
		this.MeshTransform.gameObject.SetActive(false);
		this.DisableTimer = 0.1f;
		this.SwingingOutro = true;
		this.LerpAmount = 0.5f;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00044374 File Offset: 0x00042574
	private void Update()
	{
		this.CanHit = false;
		if (this.Swinging && this.DisableTimer <= 0f)
		{
			if (!this.SwingingOutro)
			{
				if (this.LerpAmount == 0f)
				{
					PlayerController.instance.MoveForce(PlayerController.instance.transform.forward, -5f, 0.25f);
					GameDirector.instance.CameraShake.Shake(5f, 0f);
					this.Controller.SoundMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				if (this.SwingSound && (double)this.LerpAmount >= 0.2)
				{
					this.Controller.SoundSwing.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.SwingSound = false;
				}
				if ((double)this.LerpAmount >= 0.25 && (double)this.LerpAmount <= 0.3)
				{
					PlayerController.instance.MoveForce(PlayerController.instance.transform.forward, 20f, 0.01f);
				}
				if ((double)this.LerpAmount >= 0.2 && (double)this.LerpAmount <= 0.5)
				{
					this.CanHit = true;
				}
				this.LerpAmount += this.SwingSpeed * Time.deltaTime;
				if (this.LerpAmount >= 1f)
				{
					this.SwingingOutro = true;
					this.LerpAmount = 0f;
				}
			}
			else
			{
				if (this.LerpAmount == 0f)
				{
					GameDirector.instance.CameraShake.Shake(2f, 0.5f);
					this.Controller.SoundMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				if ((double)this.LerpAmount >= 0.25 && (double)this.LerpAmount <= 0.3)
				{
					PlayerController.instance.MoveForce(PlayerController.instance.transform.forward, -5f, 0.25f);
				}
				this.LerpAmount += this.OutroSpeed * Time.deltaTime;
				if (this.LerpAmount >= 1f)
				{
					if (!this.DebugSwing)
					{
						this.Swinging = false;
					}
					this.SwingingOutro = false;
					this.LerpAmount = 0f;
				}
			}
		}
		if (!this.SwingingOutro)
		{
			this.LerpResult = this.SwingCurve.Evaluate(this.LerpAmount);
		}
		else
		{
			this.LerpResult = this.OutroCurve.Evaluate(this.LerpAmount);
		}
		base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.identity, Quaternion.Euler(this.SwingRotation.x, this.SwingRotation.y, this.SwingRotation.z), this.LerpResult);
		base.transform.localPosition = Vector3.LerpUnclamped(Vector3.zero, this.SwingPosition, this.LerpResult);
		if (this.DisableTimer > 0f)
		{
			this.DisableTimer -= Time.deltaTime;
			if (this.DisableTimer <= 0f)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x000446E0 File Offset: 0x000428E0
	private void OnDrawGizmosSelected()
	{
		if (this.DebugMeshActive)
		{
			Gizmos.color = new Color(0.75f, 0f, 0f, 0.75f);
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawMesh(this.DebugMesh, 0, Vector3.zero + this.SwingPosition, Quaternion.Euler(this.SwingRotation.x, this.SwingRotation.y, this.SwingRotation.z), Vector3.one);
		}
	}

	// Token: 0x04000C62 RID: 3170
	[Header("Debug")]
	public bool DebugSwing;

	// Token: 0x04000C63 RID: 3171
	public bool DebugMeshActive;

	// Token: 0x04000C64 RID: 3172
	public Mesh DebugMesh;

	// Token: 0x04000C65 RID: 3173
	[Space]
	[Header("Other")]
	public SledgehammerController Controller;

	// Token: 0x04000C66 RID: 3174
	[Space]
	[Header("Swinging")]
	public Transform MeshTransform;

	// Token: 0x04000C67 RID: 3175
	[Space]
	[Header("State")]
	public bool Swinging;

	// Token: 0x04000C68 RID: 3176
	public bool CanHit;

	// Token: 0x04000C69 RID: 3177
	private bool SwingingOutro;

	// Token: 0x04000C6A RID: 3178
	[Space]
	[Header("Swinging")]
	public AnimationCurve SwingCurve;

	// Token: 0x04000C6B RID: 3179
	public float SwingSpeed = 1f;

	// Token: 0x04000C6C RID: 3180
	public Vector3 SwingRotation;

	// Token: 0x04000C6D RID: 3181
	public Vector3 SwingPosition;

	// Token: 0x04000C6E RID: 3182
	private bool SwingSound = true;

	// Token: 0x04000C6F RID: 3183
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x04000C70 RID: 3184
	public float OutroSpeed = 1f;

	// Token: 0x04000C71 RID: 3185
	private float LerpAmount;

	// Token: 0x04000C72 RID: 3186
	private float LerpResult;

	// Token: 0x04000C73 RID: 3187
	private float DisableTimer;
}
