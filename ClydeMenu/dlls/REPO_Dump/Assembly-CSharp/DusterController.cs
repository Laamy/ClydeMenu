using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class DusterController : MonoBehaviour
{
	// Token: 0x0600070E RID: 1806 RVA: 0x00042CC8 File Offset: 0x00040EC8
	private void Start()
	{
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		this.MoveSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00042D1C File Offset: 0x00040F1C
	private void Update()
	{
		if (ToolController.instance.Interact && ToolController.instance.ToolHide.Active && ToolController.instance.ToolHide.ActiveLerp > 0.75f)
		{
			Interaction activeInteraction = ToolController.instance.ActiveInteraction;
			if (activeInteraction)
			{
				DirtyPainting component = activeInteraction.GetComponent<DirtyPainting>();
				if (component)
				{
					CanvasHandler canvasHandler = component.CanvasHandler;
					if (canvasHandler && canvasHandler.currentState != CanvasHandler.State.Clean)
					{
						this.DustingTimer = 0.5f;
						if (this.DusterDusting.ActiveAmount >= 0.1f)
						{
							canvasHandler.cleanInput = true;
							if (!canvasHandler.CleanDone && canvasHandler.fadeMultiplier <= 0.5f)
							{
								canvasHandler.CleanDone = true;
							}
						}
					}
				}
			}
		}
		if (this.DustingTimer > 0f)
		{
			this.ToolActiveOffset.Active = true;
			this.ToolBackAway.Active = true;
			this.Dusting = true;
			this.DustingTimer -= Time.deltaTime;
			if (this.DustingTimer <= 0f)
			{
				this.Dusting = false;
				this.ToolActiveOffset.Active = false;
				this.ToolBackAway.Active = false;
			}
		}
		if (this.Dusting && this.ToolActiveOffset.Active && this.ToolActiveOffset.ActiveLerp >= 0.3f)
		{
			this.DusterDusting.Active = true;
		}
		else
		{
			this.DusterDusting.Active = false;
		}
		this.FollowTransform.position = ToolController.instance.ToolFollow.transform.position;
		this.FollowTransform.rotation = ToolController.instance.ToolFollow.transform.rotation;
		this.FollowTransform.localScale = ToolController.instance.ToolHide.transform.localScale;
		this.ParentTransform.transform.position = ToolController.instance.ToolTargetParent.transform.position;
		this.ParentTransform.transform.rotation = ToolController.instance.ToolTargetParent.transform.rotation;
		if (this.OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			this.MoveSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
			this.OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
	}

	// Token: 0x04000BFD RID: 3069
	public Transform FollowTransform;

	// Token: 0x04000BFE RID: 3070
	public Transform ParentTransform;

	// Token: 0x04000BFF RID: 3071
	[Space]
	public ToolActiveOffset ToolActiveOffset;

	// Token: 0x04000C00 RID: 3072
	public DusterDusting DusterDusting;

	// Token: 0x04000C01 RID: 3073
	public ToolBackAway ToolBackAway;

	// Token: 0x04000C02 RID: 3074
	private bool Dusting;

	// Token: 0x04000C03 RID: 3075
	private float DustingTimer;

	// Token: 0x04000C04 RID: 3076
	[Space]
	public Sound MoveSound;

	// Token: 0x04000C05 RID: 3077
	private bool OutroAudioPlay = true;
}
