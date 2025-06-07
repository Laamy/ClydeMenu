using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020002C6 RID: 710
public class ValuableWizardTimeGlass : MonoBehaviour
{
	// Token: 0x06001653 RID: 5715 RVA: 0x000C5270 File Offset: 0x000C3470
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.particleSystemGlitter.Play();
			this.particleSystemSwirl.Play();
			this.stateStart = false;
			this.timeGlassLight.gameObject.SetActive(true);
		}
		if (!this.timeGlassLight.gameObject.activeSelf)
		{
			this.timeGlassLight.gameObject.SetActive(true);
		}
		if (this.particleSystemTransform.gameObject.activeSelf)
		{
			List<PhysGrabber> playerGrabbing = this.physGrabObject.playerGrabbing;
			if (playerGrabbing.Count > this.particleFocus)
			{
				PhysGrabber physGrabber = playerGrabbing[this.particleFocus];
				if (physGrabber)
				{
					Transform headLookAtTransform = physGrabber.playerAvatar.playerAvatarVisuals.headLookAtTransform;
					if (headLookAtTransform)
					{
						this.particleSystemTransform.LookAt(headLookAtTransform);
					}
					this.particleFocus++;
				}
				else
				{
					this.particleFocus = 0;
				}
			}
			else
			{
				this.particleFocus = 0;
			}
		}
		this.soundPitchLerp = Mathf.Lerp(this.soundPitchLerp, 1f, Time.deltaTime * 2f);
		this.timeGlassLight.intensity = Mathf.Lerp(this.timeGlassLight.intensity, 4f, Time.deltaTime * 2f);
		Color a = new Color(0.5f, 0f, 1f);
		this.timeGlassMaterial.material.SetColor("_EmissionColor", a * this.timeGlassLight.intensity);
		foreach (PhysGrabber physGrabber2 in this.physGrabObject.playerGrabbing)
		{
			if (physGrabber2 && !physGrabber2.isLocal)
			{
				physGrabber2.playerAvatar.voiceChat.OverridePitch(0.65f, 1f, 2f, 0.1f, 0f, 0f);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.OverrideDrag(20f, 0.1f);
			this.physGrabObject.OverrideAngularDrag(40f, 0.1f);
			if (!this.physGrabObject.grabbed)
			{
				this.SetState(ValuableWizardTimeGlass.States.Idle);
			}
		}
		if (this.physGrabObject.grabbedLocal)
		{
			PlayerAvatar instance = PlayerAvatar.instance;
			if (instance.voiceChat)
			{
				instance.voiceChat.OverridePitch(0.65f, 1f, 2f, 0.1f, 0f, 0f);
			}
			instance.OverridePupilSize(3f, 4, 1f, 1f, 5f, 0.5f, 0.1f);
			PlayerController.instance.OverrideSpeed(0.5f, 0.1f);
			PlayerController.instance.OverrideLookSpeed(0.5f, 2f, 1f, 0.1f);
			PlayerController.instance.OverrideAnimationSpeed(0.2f, 1f, 2f, 0.1f);
			PlayerController.instance.OverrideTimeScale(0.1f, 0.1f);
			this.physGrabObject.OverrideTorqueStrength(0.6f, 0.1f);
			CameraZoom.Instance.OverrideZoomSet(50f, 0.1f, 0.5f, 1f, base.gameObject, 0);
			PostProcessing.Instance.SaturationOverride(50f, 0.1f, 0.5f, 0.1f, base.gameObject);
		}
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x000C55E8 File Offset: 0x000C37E8
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.particleSystemGlitter.Stop();
			this.particleSystemSwirl.Stop();
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.physGrabObject.grabbed)
		{
			this.SetState(ValuableWizardTimeGlass.States.Active);
		}
		this.timeGlassLight.intensity = Mathf.Lerp(this.timeGlassLight.intensity, 0f, Time.deltaTime * 10f);
		this.soundPitchLerp = Mathf.Lerp(this.soundPitchLerp, 0f, Time.deltaTime * 10f);
		Color a = new Color(0.5f, 0f, 1f);
		this.timeGlassMaterial.material.SetColor("_EmissionColor", a * this.timeGlassLight.intensity);
		if (this.timeGlassLight.intensity < 0.01f)
		{
			this.timeGlassLight.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x000C56E0 File Offset: 0x000C38E0
	[PunRPC]
	public void SetStateRPC(ValuableWizardTimeGlass.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x000C56F0 File Offset: 0x000C38F0
	private void SetState(ValuableWizardTimeGlass.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(state);
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x000C5729 File Offset: 0x000C3929
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x000C5744 File Offset: 0x000C3944
	private void Update()
	{
		float pitchMultiplier = Mathf.Lerp(2f, 0.5f, this.soundPitchLerp);
		this.soundTimeGlassLoop.PlayLoop(this.currentState == ValuableWizardTimeGlass.States.Active, 0.8f, 0.8f, pitchMultiplier);
		ValuableWizardTimeGlass.States states = this.currentState;
		if (states != ValuableWizardTimeGlass.States.Idle)
		{
			if (states == ValuableWizardTimeGlass.States.Active)
			{
				this.StateActive();
				return;
			}
		}
		else
		{
			this.StateIdle();
		}
	}

	// Token: 0x04002693 RID: 9875
	private PhysGrabObject physGrabObject;

	// Token: 0x04002694 RID: 9876
	private PhotonView photonView;

	// Token: 0x04002695 RID: 9877
	internal ValuableWizardTimeGlass.States currentState;

	// Token: 0x04002696 RID: 9878
	private bool stateStart;

	// Token: 0x04002697 RID: 9879
	public Transform particleSystemTransform;

	// Token: 0x04002698 RID: 9880
	public ParticleSystem particleSystemSwirl;

	// Token: 0x04002699 RID: 9881
	public ParticleSystem particleSystemGlitter;

	// Token: 0x0400269A RID: 9882
	public MeshRenderer timeGlassMaterial;

	// Token: 0x0400269B RID: 9883
	[FormerlySerializedAs("light")]
	public Light timeGlassLight;

	// Token: 0x0400269C RID: 9884
	public Sound soundTimeGlassLoop;

	// Token: 0x0400269D RID: 9885
	private float soundPitchLerp;

	// Token: 0x0400269E RID: 9886
	private int particleFocus;

	// Token: 0x0200041D RID: 1053
	public enum States
	{
		// Token: 0x04002DE5 RID: 11749
		Idle,
		// Token: 0x04002DE6 RID: 11750
		Active
	}
}
