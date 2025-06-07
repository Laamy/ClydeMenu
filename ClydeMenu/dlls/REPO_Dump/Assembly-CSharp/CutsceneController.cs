using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class CutsceneController : MonoBehaviour
{
	// Token: 0x0600093A RID: 2362 RVA: 0x000582DC File Offset: 0x000564DC
	private void Start()
	{
		this.Animator = base.GetComponent<Animator>();
		this.StartTimer = Random.Range(this.StartTimeMin, this.StartTimeMax);
		this.MainCamera = GameDirector.instance.MainCamera;
		this.PreviousParent = GameDirector.instance.MainCameraParent;
		this.PreviousFOV = this.MainCamera.fieldOfView;
		this.MainCamera.transform.parent = this.Parent;
		this.MainCamera.fieldOfView = this.Camera.fieldOfView;
		this.MainCamera.transform.localPosition = Vector3.zero;
		this.MainCamera.transform.localRotation = Quaternion.identity;
		GameDirector.instance.volumeCutsceneOnly.TransitionTo(0.1f);
		this.Camera.enabled = false;
		this.Active = true;
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x000583BC File Offset: 0x000565BC
	private void Update()
	{
		if (!this.Started)
		{
			HUD.instance.Hide();
			VideoOverlay.Instance.Override(0.1f, 1f, 5f);
			this.StartTimer -= Time.deltaTime;
			if (this.StartTimer <= 0f || this.DebugLoop)
			{
				if (this.DebugLoop || !GameDirectorStatic.CatchCutscenePlayed || Random.Range(0, 3) == 0)
				{
					this.Animator.SetBool("Play", true);
					this.Started = true;
				}
				else
				{
					this.End();
				}
			}
		}
		if (this.Active)
		{
			VideoOverlay.Instance.Override(0.1f, 1f, 5f);
			GameDirector.instance.SetDisableInput(0.5f);
			this.MainCamera.fieldOfView = this.Camera.fieldOfView;
			if (this.EndActive)
			{
				this.EndTimer -= Time.deltaTime;
				if (this.DebugLoop || this.EndTimer <= 0f)
				{
					this.End();
					return;
				}
			}
			else
			{
				VideoOverlay.Instance.Override(0.1f, 0.2f, 5f);
			}
		}
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x000584F0 File Offset: 0x000566F0
	private void ResetCamera()
	{
		this.MainCamera.transform.parent = GameDirector.instance.MainCameraParent;
		this.MainCamera.fieldOfView = this.PreviousFOV;
		this.MainCamera.transform.localPosition = Vector3.zero;
		this.MainCamera.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x00058552 File Offset: 0x00056752
	public void EndSet()
	{
		if (!this.DebugLoop)
		{
			this.Animator.SetBool("Play", false);
		}
		this.EndActive = true;
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00058574 File Offset: 0x00056774
	private void End()
	{
		if (!this.DebugLoop)
		{
			GameDirectorStatic.CatchCutscenePlayed = true;
			this.Active = false;
			this.ResetCamera();
			if (!this.CatchCutscene)
			{
				GameDirector.instance.volumeOn.TransitionTo(0.1f);
			}
			this.ParentEnable.SetActive(false);
		}
	}

	// Token: 0x040010CF RID: 4303
	public Camera Camera;

	// Token: 0x040010D0 RID: 4304
	private Camera MainCamera;

	// Token: 0x040010D1 RID: 4305
	public Transform Parent;

	// Token: 0x040010D2 RID: 4306
	private Transform PreviousParent;

	// Token: 0x040010D3 RID: 4307
	private float PreviousFOV;

	// Token: 0x040010D4 RID: 4308
	private bool Active;

	// Token: 0x040010D5 RID: 4309
	public GameObject ParentEnable;

	// Token: 0x040010D6 RID: 4310
	[Space]
	public bool CatchCutscene;

	// Token: 0x040010D7 RID: 4311
	[Space]
	public float StartTimeMin;

	// Token: 0x040010D8 RID: 4312
	public float StartTimeMax;

	// Token: 0x040010D9 RID: 4313
	private float StartTimer;

	// Token: 0x040010DA RID: 4314
	private bool Started;

	// Token: 0x040010DB RID: 4315
	[Space]
	private bool EndActive;

	// Token: 0x040010DC RID: 4316
	public float EndTimer;

	// Token: 0x040010DD RID: 4317
	[Space]
	public bool DebugLoop;

	// Token: 0x040010DE RID: 4318
	private Animator Animator;
}
