using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class SledgehammerController : MonoBehaviour
{
	// Token: 0x0600071E RID: 1822 RVA: 0x00043C34 File Offset: 0x00041E34
	private void Start()
	{
		this.MainCamera = Camera.main;
		this.Mask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		this.Hit.gameObject.SetActive(false);
		this.Transition.gameObject.SetActive(false);
		this.SoundMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00043CD0 File Offset: 0x00041ED0
	private void OnTriggerStay(Collider other)
	{
		if (ToolController.instance.ToolHide.Active && this.Swing.CanHit)
		{
			RoachTrigger component = other.GetComponent<RoachTrigger>();
			if (component)
			{
				float magnitude = (component.transform.position - this.MainCamera.transform.position).magnitude;
				Vector3 normalized = (component.transform.position - this.MainCamera.transform.position).normalized;
				RaycastHit raycastHit;
				if (!Physics.Raycast(this.MainCamera.transform.position, (component.transform.position - this.MainCamera.transform.position).normalized, out raycastHit, (component.transform.position - this.MainCamera.transform.position).magnitude, this.Mask))
				{
					this.Roach = component;
					this.Swing.HitOutro();
					this.Swing.CanHit = false;
					this.Transition.gameObject.SetActive(true);
					this.Transition.IntroSet();
					this.Hit.gameObject.SetActive(true);
					this.Hit.Spawn(this.Roach);
				}
			}
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00043E38 File Offset: 0x00042038
	public void HitDone()
	{
		this.Transition.gameObject.SetActive(true);
		this.Transition.OutroSet();
		GameDirector.instance.CameraImpact.Shake(3f, 0f);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		this.SoundHitOutro.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.Hit.gameObject.SetActive(false);
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00043ED0 File Offset: 0x000420D0
	public void IntroDone()
	{
		GameDirector.instance.CameraImpact.Shake(5f, 0f);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		this.Hit.gameObject.SetActive(true);
		this.Hit.Hit();
		this.Roach.RoachOrbit.Squash();
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00043F3C File Offset: 0x0004213C
	public void OutroDone()
	{
		this.Swing.gameObject.SetActive(true);
		this.Swing.MeshTransform.gameObject.SetActive(true);
		PlayerController.instance.MoveForce(PlayerController.instance.transform.forward, -5f, 0.25f);
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00043F94 File Offset: 0x00042194
	private void Update()
	{
		if (this.Hit.gameObject.activeSelf)
		{
			float magnitude = (this.Hit.transform.position - PlayerController.instance.transform.position).magnitude;
			PlayerController.instance.MoveForce(this.Hit.transform.position - PlayerController.instance.transform.position, magnitude * 30f, 0.01f);
			PlayerController.instance.InputDisable(0.1f);
		}
		if (ToolController.instance.Interact && !this.Swing.Swinging)
		{
			this.InteractImpulse = true;
		}
		if (this.InteractImpulse && ToolController.instance.ToolHide.Active && ToolController.instance.ToolHide.ActiveLerp >= 1f)
		{
			this.InteractImpulse = false;
			this.Swing.Swing();
		}
		if (this.Swing.Swinging || this.Hit.gameObject.activeSelf)
		{
			ToolController.instance.ForceActiveTimer = 0.1f;
		}
		this.ControllerTransform.transform.position = ToolController.instance.ToolTargetParent.transform.position;
		this.ControllerTransform.transform.rotation = ToolController.instance.ToolTargetParent.transform.rotation;
		this.FollowTransform.position = ToolController.instance.ToolFollow.transform.position;
		this.FollowTransform.rotation = ToolController.instance.ToolFollow.transform.rotation;
		this.FollowTransform.localScale = ToolController.instance.ToolHide.transform.localScale;
		if (this.OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			this.SoundMoveLong.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
			this.OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
	}

	// Token: 0x04000C46 RID: 3142
	public Transform ControllerTransform;

	// Token: 0x04000C47 RID: 3143
	public Transform FollowTransform;

	// Token: 0x04000C48 RID: 3144
	[Space]
	public SledgehammerSwing Swing;

	// Token: 0x04000C49 RID: 3145
	public SledgehammerHit Hit;

	// Token: 0x04000C4A RID: 3146
	public SledgehammerTransition Transition;

	// Token: 0x04000C4B RID: 3147
	private RoachTrigger Roach;

	// Token: 0x04000C4C RID: 3148
	[Space]
	[Header("Sounds")]
	public Sound SoundMoveLong;

	// Token: 0x04000C4D RID: 3149
	public Sound SoundMoveShort;

	// Token: 0x04000C4E RID: 3150
	public Sound SoundSwing;

	// Token: 0x04000C4F RID: 3151
	public Sound SoundHit;

	// Token: 0x04000C50 RID: 3152
	public Sound SoundHitOutro;

	// Token: 0x04000C51 RID: 3153
	private bool OutroAudioPlay = true;

	// Token: 0x04000C52 RID: 3154
	private LayerMask Mask;

	// Token: 0x04000C53 RID: 3155
	private Camera MainCamera;

	// Token: 0x04000C54 RID: 3156
	private bool InteractImpulse;
}
