using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class MapToolController : MonoBehaviour
{
	// Token: 0x06000DC0 RID: 3520 RVA: 0x00077C14 File Offset: 0x00075E14
	private void Start()
	{
		this.VisualTransform.gameObject.SetActive(false);
		this.photonView = base.GetComponent<PhotonView>();
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			this.DisplayMesh.material = this.DisplayMaterial;
			base.transform.parent.parent = this.FollowTransform;
			base.transform.parent.localPosition = Vector3.zero;
			base.transform.parent.localRotation = Quaternion.identity;
			return;
		}
		this.DisplayMesh.material = this.DisplayMaterialClient;
		Transform[] componentsInChildren = this.VisualTransform.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer("Triggers");
		}
		this.SoundStart.SpatialBlend = 1f;
		this.SoundStart.Volume *= 0.3f;
		this.SoundStart.VolumeRandom *= 0.3f;
		this.SoundStart.OffscreenFalloff = 0.6f;
		this.SoundStart.OffscreenVolume = 0.6f;
		this.SoundStop.SpatialBlend = 1f;
		this.SoundStop.Volume *= 0.3f;
		this.SoundStop.VolumeRandom *= 0.3f;
		this.SoundStop.OffscreenFalloff = 0.6f;
		this.SoundStop.OffscreenVolume = 0.6f;
		this.SoundLoop.SpatialBlend = 1f;
		this.SoundLoop.Volume *= 0.3f;
		this.SoundLoop.VolumeRandom *= 0.3f;
		this.SoundLoop.OffscreenFalloff = 0.6f;
		this.SoundLoop.OffscreenVolume = 0.6f;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00077E00 File Offset: 0x00076000
	private void Update()
	{
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			if (!this.PlayerAvatar.isDisabled && !this.PlayerAvatar.isTumbling && !CameraAim.Instance.AimTargetActive && !SemiFunc.MenuLevel())
			{
				if (InputManager.instance.InputToggleGet(InputKey.Map))
				{
					if (SemiFunc.InputDown(InputKey.Map))
					{
						this.mapToggled = !this.mapToggled;
					}
				}
				else
				{
					this.mapToggled = false;
				}
				if ((SemiFunc.InputHold(InputKey.Map) || this.mapToggled || Map.Instance.debugActive) && !PlayerController.instance.sprinting)
				{
					if (this.HideLerp >= 1f)
					{
						this.Active = true;
					}
				}
				else
				{
					this.mapToggled = false;
					if (this.HideLerp <= 0f)
					{
						this.Active = false;
					}
				}
			}
			else
			{
				this.Active = false;
			}
			if (this.Active)
			{
				StatsUI.instance.Show();
				ItemInfoUI.instance.Hide();
				ItemInfoExtraUI.instance.Hide();
				if (MissionUI.instance.Text.text != "")
				{
					MissionUI.instance.Show();
				}
			}
		}
		if (this.Active != this.ActivePrev)
		{
			this.ActivePrev = this.Active;
			if (GameManager.Multiplayer() && this.photonView.IsMine)
			{
				this.photonView.RPC("SetActiveRPC", RpcTarget.Others, new object[]
				{
					this.Active
				});
			}
			if (this.Active)
			{
				if (!GameManager.Multiplayer() || this.photonView.IsMine)
				{
					GameDirector.instance.CameraShake.Shake(2f, 0.1f);
					Map.Instance.ActiveSet(true);
				}
				this.VisualTransform.gameObject.SetActive(true);
				this.SoundStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				if (!GameManager.Multiplayer() || this.photonView.IsMine)
				{
					GameDirector.instance.CameraShake.Shake(2f, 0.1f);
					Map.Instance.ActiveSet(false);
				}
				this.SoundStop.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		float x = 90f;
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			x = 0f;
		}
		float num = 1f;
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			num = 2f;
		}
		if (this.Active)
		{
			if (this.HideLerp > 0f)
			{
				this.HideLerp -= Time.deltaTime * this.IntroSpeed * num;
			}
			this.HideScale = Mathf.LerpUnclamped(1f, 0f, this.IntroCurve.Evaluate(this.HideLerp));
			this.HideTransform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(x, 0f, 0f), this.IntroCurve.Evaluate(this.HideLerp));
		}
		else
		{
			if (this.HideLerp < 1f)
			{
				this.HideLerp += Time.deltaTime * this.OutroSpeed * num;
				if (this.HideLerp > 1f)
				{
					this.VisualTransform.gameObject.SetActive(false);
				}
			}
			this.HideScale = Mathf.LerpUnclamped(1f, 0f, this.OutroCurve.Evaluate(this.HideLerp));
		}
		if ((!GameManager.Multiplayer() || this.photonView.IsMine) && this.Active)
		{
			SemiFunc.UIHideWorldSpace();
			PlayerController.instance.MoveMult(this.MoveMultiplier, 0.1f);
			CameraTopFade.Instance.Set(this.FadeAmount, 0.1f);
			CameraBob.Instance.SetMultiplier(this.BobMultiplier, 0.1f);
			CameraZoom.Instance.OverrideZoomSet(50f, 0.05f, 2f, 2f, base.gameObject, 100);
			CameraNoise.Instance.Override(0.025f, 0.25f);
			Aim.instance.SetState(Aim.State.Hidden);
		}
		Vector3 a = Vector3.one;
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			base.transform.parent.position = this.FollowTransformClient.position;
			base.transform.parent.rotation = this.FollowTransformClient.rotation;
			a = this.FollowTransformClient.localScale;
			this.mainSpringTransform.rotation = SemiFunc.SpringQuaternionGet(this.mainSpring, this.mainSpringTransformTarget.rotation, -1f);
		}
		base.transform.parent.localScale = Vector3.Lerp(base.transform.parent.localScale, a * this.HideScale, Time.deltaTime * 20f);
		this.displaySpringTransform.rotation = SemiFunc.SpringQuaternionGet(this.displaySpring, this.displaySpringTransformTarget.rotation, -1f);
		if (this.Active)
		{
			this.DisplayJointAngleDiff = (this.DisplayJointAnglePreviousX - this.displaySpringTransform.localRotation.x) * 50f;
			this.DisplayJointAngleDiff = Mathf.Clamp(this.DisplayJointAngleDiff, -0.1f, 0.1f);
			this.DisplayJointAnglePreviousX = this.displaySpringTransform.localRotation.x;
			this.SoundLoop.LoopPitch = Mathf.Lerp(this.SoundLoop.LoopPitch, 1f - this.DisplayJointAngleDiff, Time.deltaTime * 10f);
		}
		this.SoundLoop.PlayLoop(this.Active, 5f, 5f, 1f);
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x000783EB File Offset: 0x000765EB
	[PunRPC]
	public void SetActiveRPC(bool active)
	{
		this.Active = active;
	}

	// Token: 0x04001615 RID: 5653
	public static MapToolController instance;

	// Token: 0x04001616 RID: 5654
	internal bool Active;

	// Token: 0x04001617 RID: 5655
	private bool ActivePrev;

	// Token: 0x04001618 RID: 5656
	internal PhotonView photonView;

	// Token: 0x04001619 RID: 5657
	public PlayerAvatar PlayerAvatar;

	// Token: 0x0400161A RID: 5658
	[Space]
	public Transform FollowTransform;

	// Token: 0x0400161B RID: 5659
	public Transform FollowTransformClient;

	// Token: 0x0400161C RID: 5660
	[Space]
	public Transform ControllerTransform;

	// Token: 0x0400161D RID: 5661
	public Transform VisualTransform;

	// Token: 0x0400161E RID: 5662
	public Transform PlayerLookTarget;

	// Token: 0x0400161F RID: 5663
	public Transform HideTransform;

	// Token: 0x04001620 RID: 5664
	[Space]
	private float DisplayJointAngleDiff;

	// Token: 0x04001621 RID: 5665
	private float DisplayJointAnglePreviousX;

	// Token: 0x04001622 RID: 5666
	[Space]
	public float MoveMultiplier = 0.5f;

	// Token: 0x04001623 RID: 5667
	public float FadeAmount = 0.5f;

	// Token: 0x04001624 RID: 5668
	public float BobMultiplier = 0.1f;

	// Token: 0x04001625 RID: 5669
	[Space]
	public Sound SoundStart;

	// Token: 0x04001626 RID: 5670
	public Sound SoundStop;

	// Token: 0x04001627 RID: 5671
	public Sound SoundLoop;

	// Token: 0x04001628 RID: 5672
	[Space]
	public MeshRenderer DisplayMesh;

	// Token: 0x04001629 RID: 5673
	public Material DisplayMaterial;

	// Token: 0x0400162A RID: 5674
	public Material DisplayMaterialClient;

	// Token: 0x0400162B RID: 5675
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x0400162C RID: 5676
	public float IntroSpeed;

	// Token: 0x0400162D RID: 5677
	public AnimationCurve OutroCurve;

	// Token: 0x0400162E RID: 5678
	public float OutroSpeed;

	// Token: 0x0400162F RID: 5679
	internal float HideLerp;

	// Token: 0x04001630 RID: 5680
	private float HideScale;

	// Token: 0x04001631 RID: 5681
	[Space]
	public Transform displaySpringTransform;

	// Token: 0x04001632 RID: 5682
	public Transform displaySpringTransformTarget;

	// Token: 0x04001633 RID: 5683
	public SpringQuaternion displaySpring;

	// Token: 0x04001634 RID: 5684
	[Space]
	public Transform mainSpringTransform;

	// Token: 0x04001635 RID: 5685
	public Transform mainSpringTransformTarget;

	// Token: 0x04001636 RID: 5686
	public SpringQuaternion mainSpring;

	// Token: 0x04001637 RID: 5687
	private bool mapToggled;
}
