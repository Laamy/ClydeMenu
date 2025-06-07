using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B7 RID: 695
public class TrafficLightValuable : Trap
{
	// Token: 0x060015D4 RID: 5588 RVA: 0x000C071E File Offset: 0x000BE91E
	protected override void Start()
	{
		base.Start();
		this.TurnAllLightsOff();
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x000C0738 File Offset: 0x000BE938
	protected override void Update()
	{
		base.Update();
		this.CheckForStateChange();
		this.ClientBasedEffects();
		this.MasterClientStateManager();
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x000C0752 File Offset: 0x000BE952
	private void CheckForStateChange()
	{
		if (this.currentState != this.previousState)
		{
			this.stateChanged = true;
			this.previousState = this.currentState;
		}
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x000C0778 File Offset: 0x000BE978
	private void MasterClientStateManager()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.CheckForGrabActivate();
		if (this.IsActive() && this.currentState != TrafficLightValuable.States.LingeringOnRed && this.currentState != TrafficLightValuable.States.LingeringOnRedFlickering)
		{
			this.ManageLightState();
		}
		else if (this.currentState == TrafficLightValuable.States.LingeringOnRed || this.currentState == TrafficLightValuable.States.LingeringOnRedFlickering)
		{
			this.LingerOnRedState();
		}
		if (this.IsRed())
		{
			this.RedState();
		}
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x000C07DB File Offset: 0x000BE9DB
	private void CheckForGrabActivate()
	{
		if (SemiFunc.FPSImpulse15())
		{
			return;
		}
		if (this.physGrabObject.grabbed && !this.IsActive())
		{
			this.SetState(TrafficLightValuable.States.Green);
		}
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x000C0804 File Offset: 0x000BEA04
	private void ManageLightState()
	{
		this.lightTimer += Time.deltaTime;
		switch (this.currentState)
		{
		case TrafficLightValuable.States.Red:
			if (this.redLightDuration - this.lightTimer <= 0.5f)
			{
				this.SetState(TrafficLightValuable.States.RedFlickering);
				return;
			}
			break;
		case TrafficLightValuable.States.Green:
			if (this.greenLightDuration - this.lightTimer <= 0.5f)
			{
				this.SetState(TrafficLightValuable.States.GreenFlickering);
				return;
			}
			break;
		case TrafficLightValuable.States.Yellow:
			if (this.yellowLightDuration - this.lightTimer <= 0.5f)
			{
				this.SetState(TrafficLightValuable.States.YellowFlickering);
				return;
			}
			break;
		case TrafficLightValuable.States.RedFlickering:
			if (this.lightTimer >= this.redLightDuration)
			{
				this.SetState(TrafficLightValuable.States.Off);
				return;
			}
			break;
		case TrafficLightValuable.States.GreenFlickering:
			if (this.lightTimer >= this.greenLightDuration)
			{
				this.SetState(TrafficLightValuable.States.Yellow);
				return;
			}
			break;
		case TrafficLightValuable.States.YellowFlickering:
			if (this.lightTimer >= this.yellowLightDuration)
			{
				this.SetState(TrafficLightValuable.States.Red);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x000C08E4 File Offset: 0x000BEAE4
	private void RedState()
	{
		if (this.physGrabObject.grabbed)
		{
			foreach (PhysGrabber physGrabber in new List<PhysGrabber>(this.physGrabObject.playerGrabbing))
			{
				this.ZapPlayer(physGrabber.playerAvatar);
			}
			if (this.currentState != TrafficLightValuable.States.LingeringOnRed)
			{
				this.SetState(TrafficLightValuable.States.LingeringOnRed);
			}
		}
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x000C0964 File Offset: 0x000BEB64
	private void LingerOnRedState()
	{
		this.lingerTimer += Time.deltaTime;
		if (this.lingerOnRedTime - this.lingerTimer <= 0.5f && this.currentState != TrafficLightValuable.States.LingeringOnRedFlickering)
		{
			this.SetState(TrafficLightValuable.States.LingeringOnRedFlickering);
		}
		if (this.lingerTimer >= this.lingerOnRedTime)
		{
			this.lingerTimer = 0f;
			this.SetState(TrafficLightValuable.States.Off);
		}
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x000C09C7 File Offset: 0x000BEBC7
	private void ClientBasedEffects()
	{
		this.StartOfStateManager();
		this.MaterialEffects();
		this.SoundEffects();
		if (this.isFlashing)
		{
			this.FlashEffect();
		}
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x000C09EC File Offset: 0x000BEBEC
	private void StartOfStateManager()
	{
		if (!this.stateChanged)
		{
			return;
		}
		switch (this.currentState)
		{
		case TrafficLightValuable.States.Red:
			this.color = Color.red;
			this.SwitchLightsBasedOnState();
			this.turnOnSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.flickeringSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			break;
		case TrafficLightValuable.States.Green:
			this.color = Color.green;
			this.SwitchLightsBasedOnState();
			this.turnOnSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.flickeringSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			break;
		case TrafficLightValuable.States.Yellow:
			this.color = Color.yellow;
			this.SwitchLightsBasedOnState();
			this.turnOnSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.flickeringSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			break;
		case TrafficLightValuable.States.LingeringOnRed:
			this.color = Color.red;
			this.SwitchLightsBasedOnState();
			this.zapSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.smokeSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.ZapParticles();
			this.ZapCameraShake();
			this.FlashStart();
			break;
		case TrafficLightValuable.States.Off:
			this.TurnAllLightsOff();
			this.flickeringSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			break;
		}
		this.stateChanged = false;
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x000C0C30 File Offset: 0x000BEE30
	private void MaterialEffects()
	{
		if (this.IsFlickering())
		{
			switch (this.currentState)
			{
			case TrafficLightValuable.States.RedFlickering:
				this.FlickerLight(this.redLight.material);
				return;
			case TrafficLightValuable.States.GreenFlickering:
				this.FlickerLight(this.greenLight.material);
				return;
			case TrafficLightValuable.States.YellowFlickering:
				this.FlickerLight(this.yellowLight.material);
				return;
			case TrafficLightValuable.States.LingeringOnRed:
				break;
			case TrafficLightValuable.States.LingeringOnRedFlickering:
				this.FlickerLight(this.redLight.material);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x000C0CB0 File Offset: 0x000BEEB0
	private void SoundEffects()
	{
		this.slowTickSound.PlayLoop(this.IsActive() && this.IsRed(), 1f, 1f, 1f);
		this.fastTickSound.PlayLoop(this.IsActive() && (this.IsGreen() || this.IsYellow()), 1f, 1f, 1f);
		if (this.IsFlickering())
		{
			this.flickeringSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x000C0D51 File Offset: 0x000BEF51
	private void ZapPlayer(PlayerAvatar _player)
	{
		EnemyDirector.instance.SetInvestigate(base.transform.position, 15f, false);
		this.TumblePlayer(_player);
		this.TumbleTrafficLight();
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x000C0D7C File Offset: 0x000BEF7C
	private void TumblePlayer(PlayerAvatar _player)
	{
		Vector3 a = _player.transform.localRotation * Vector3.back;
		_player.tumble.TumbleRequest(true, false);
		_player.tumble.TumbleForce(a * this.tumbleForceMultiplier);
		_player.tumble.TumbleTorque(-_player.transform.right * 45f);
		_player.tumble.TumbleOverrideTime(3f);
		_player.tumble.ImpactHurtSet(3f, 10);
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x000C0E0C File Offset: 0x000BF00C
	private void TumbleTrafficLight()
	{
		Vector3 torque = Random.insideUnitSphere.normalized * this.SelfTorqueMultiplier;
		Vector3 a = Random.insideUnitSphere.normalized * this.SelfTorqueMultiplier;
		this.rb.AddTorque(torque, ForceMode.Impulse);
		this.rb.AddForce(a * this.SelfTorqueMultiplier, ForceMode.Impulse);
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x000C0E70 File Offset: 0x000BF070
	[PunRPC]
	public void SetStateRPC(TrafficLightValuable.States _state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.currentState = _state;
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x000C0E84 File Offset: 0x000BF084
	private void SetState(TrafficLightValuable.States _state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(_state, default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			_state
		});
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x000C0ED4 File Offset: 0x000BF0D4
	private void TurnAllLightsOff()
	{
		this.pointLightGreen.enabled = false;
		this.pointLightRed.enabled = false;
		this.pointLightYellow.enabled = false;
		this.yellowLight.material.DisableKeyword("_EMISSION");
		this.greenLight.material.DisableKeyword("_EMISSION");
		this.redLight.material.DisableKeyword("_EMISSION");
		this.boxLight.material.DisableKeyword("_EMISSION");
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x000C0F5C File Offset: 0x000BF15C
	private void SwitchLightsBasedOnState()
	{
		this.lightTimer = 0f;
		this.TurnAllLightsOff();
		switch (this.currentState)
		{
		case TrafficLightValuable.States.Red:
			this.ToggleLight(true, this.redLight.material, this.pointLightRed);
			return;
		case TrafficLightValuable.States.Green:
			this.ToggleLight(true, this.greenLight.material, this.pointLightGreen);
			return;
		case TrafficLightValuable.States.Yellow:
			this.ToggleLight(true, this.yellowLight.material, this.pointLightYellow);
			return;
		case TrafficLightValuable.States.RedFlickering:
		case TrafficLightValuable.States.GreenFlickering:
		case TrafficLightValuable.States.YellowFlickering:
			break;
		case TrafficLightValuable.States.LingeringOnRed:
			this.ToggleLight(true, this.redLight.material, this.pointLightRed);
			break;
		default:
			return;
		}
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x000C1008 File Offset: 0x000BF208
	private void ToggleLight(bool _status, Material _material, Light _light)
	{
		if (_status)
		{
			_material.EnableKeyword("_EMISSION");
			_material.SetColor("_EmissionColor", this.color * 2f);
			_light.intensity = 3f;
			_light.enabled = true;
			return;
		}
		_material.DisableKeyword("_EMISSION");
		_light.enabled = false;
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x000C1063 File Offset: 0x000BF263
	private void ZapParticles()
	{
		this.sparkParticles.Play();
		this.smokeParticles.Play();
		this.zapParticles.Play();
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x000C1088 File Offset: 0x000BF288
	private void ZapCameraShake()
	{
		GameDirector.instance.CameraShake.ShakeDistance(this.cameraShakeStrength, 3f, 8f, base.transform.position, this.cameraShakeTime);
		GameDirector.instance.CameraImpact.ShakeDistance(this.cameraShakeStrength, this.cameraShakeBounds.x, this.cameraShakeBounds.y, base.transform.position, this.cameraShakeTime);
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x000C1104 File Offset: 0x000BF304
	private void FlickerLight(Material _light)
	{
		float num = Mathf.PingPong(Time.time, Random.Range(0.3f, 1.5f));
		_light.SetColor("_EmissionColor", this.color * num);
		if (this.pointLightGreen.enabled)
		{
			this.pointLightGreen.intensity = num * 2f;
		}
		if (this.pointLightRed.enabled)
		{
			this.pointLightRed.intensity = num * 2f;
		}
		if (this.pointLightYellow.enabled)
		{
			this.pointLightYellow.intensity = num * 2f;
		}
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x000C119F File Offset: 0x000BF39F
	private void FlashStart()
	{
		this.isFlashing = true;
		this.flashTimer = 0f;
		this.zapLight.enabled = true;
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x000C11BF File Offset: 0x000BF3BF
	private void FlashEffect()
	{
		this.flashTimer += Time.deltaTime;
		if (this.flashTimer >= this.flashDuration)
		{
			this.zapLight.enabled = false;
			this.isFlashing = false;
		}
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x000C11F4 File Offset: 0x000BF3F4
	private bool IsActive()
	{
		return this.currentState != TrafficLightValuable.States.Off;
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x000C1202 File Offset: 0x000BF402
	private bool IsYellow()
	{
		return this.currentState == TrafficLightValuable.States.Yellow || this.currentState == TrafficLightValuable.States.YellowFlickering;
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x000C1218 File Offset: 0x000BF418
	private bool IsRed()
	{
		return this.currentState == TrafficLightValuable.States.Red || this.currentState == TrafficLightValuable.States.RedFlickering || this.currentState == TrafficLightValuable.States.LingeringOnRed || this.currentState == TrafficLightValuable.States.LingeringOnRedFlickering;
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x000C123F File Offset: 0x000BF43F
	private bool IsGreen()
	{
		return this.currentState == TrafficLightValuable.States.Green || this.currentState == TrafficLightValuable.States.GreenFlickering;
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x000C1255 File Offset: 0x000BF455
	private bool IsFlickering()
	{
		return this.currentState == TrafficLightValuable.States.RedFlickering || this.currentState == TrafficLightValuable.States.GreenFlickering || this.currentState == TrafficLightValuable.States.YellowFlickering || this.currentState == TrafficLightValuable.States.LingeringOnRedFlickering;
	}

	// Token: 0x040025E9 RID: 9705
	[Header("MeshRenderers")]
	public MeshRenderer greenLight;

	// Token: 0x040025EA RID: 9706
	public MeshRenderer redLight;

	// Token: 0x040025EB RID: 9707
	public MeshRenderer yellowLight;

	// Token: 0x040025EC RID: 9708
	public MeshRenderer boxLight;

	// Token: 0x040025ED RID: 9709
	[Header("Point Lights")]
	public Light pointLightGreen;

	// Token: 0x040025EE RID: 9710
	public Light pointLightRed;

	// Token: 0x040025EF RID: 9711
	public Light pointLightYellow;

	// Token: 0x040025F0 RID: 9712
	[Header("Zap effect light")]
	public Light zapLight;

	// Token: 0x040025F1 RID: 9713
	[Header("Light Durations")]
	public float redLightDuration = 5f;

	// Token: 0x040025F2 RID: 9714
	public float greenLightDuration = 5f;

	// Token: 0x040025F3 RID: 9715
	public float yellowLightDuration = 2f;

	// Token: 0x040025F4 RID: 9716
	public float lingerOnRedTime = 2f;

	// Token: 0x040025F5 RID: 9717
	[Header("Particles")]
	public ParticleSystem sparkParticles;

	// Token: 0x040025F6 RID: 9718
	public ParticleSystem smokeParticles;

	// Token: 0x040025F7 RID: 9719
	public ParticleSystem zapParticles;

	// Token: 0x040025F8 RID: 9720
	[Header("Camera Shake")]
	public float cameraShakeTime = 0.2f;

	// Token: 0x040025F9 RID: 9721
	public float cameraShakeStrength = 3f;

	// Token: 0x040025FA RID: 9722
	public Vector2 cameraShakeBounds = new Vector2(1.5f, 5f);

	// Token: 0x040025FB RID: 9723
	[Header("Physics")]
	public float SelfTorqueMultiplier = 2f;

	// Token: 0x040025FC RID: 9724
	public float tumbleForceMultiplier = 15f;

	// Token: 0x040025FD RID: 9725
	[Header("Sounds")]
	public Sound zapSound;

	// Token: 0x040025FE RID: 9726
	public Sound flickeringSound;

	// Token: 0x040025FF RID: 9727
	public Sound turnOnSound;

	// Token: 0x04002600 RID: 9728
	public Sound smokeSound;

	// Token: 0x04002601 RID: 9729
	public Sound fastTickSound;

	// Token: 0x04002602 RID: 9730
	public Sound slowTickSound;

	// Token: 0x04002603 RID: 9731
	private Color color;

	// Token: 0x04002604 RID: 9732
	internal TrafficLightValuable.States currentState = TrafficLightValuable.States.Off;

	// Token: 0x04002605 RID: 9733
	internal TrafficLightValuable.States previousState = TrafficLightValuable.States.Off;

	// Token: 0x04002606 RID: 9734
	private Rigidbody rb;

	// Token: 0x04002607 RID: 9735
	private float lightTimer;

	// Token: 0x04002608 RID: 9736
	private float lingerTimer;

	// Token: 0x04002609 RID: 9737
	private bool isFlashing;

	// Token: 0x0400260A RID: 9738
	private float flashDuration = 0.1f;

	// Token: 0x0400260B RID: 9739
	private float flashTimer;

	// Token: 0x0400260C RID: 9740
	private bool stateChanged;

	// Token: 0x02000417 RID: 1047
	public enum States
	{
		// Token: 0x04002DC6 RID: 11718
		Red,
		// Token: 0x04002DC7 RID: 11719
		Green,
		// Token: 0x04002DC8 RID: 11720
		Yellow,
		// Token: 0x04002DC9 RID: 11721
		RedFlickering,
		// Token: 0x04002DCA RID: 11722
		GreenFlickering,
		// Token: 0x04002DCB RID: 11723
		YellowFlickering,
		// Token: 0x04002DCC RID: 11724
		LingeringOnRed,
		// Token: 0x04002DCD RID: 11725
		LingeringOnRedFlickering,
		// Token: 0x04002DCE RID: 11726
		Off
	}
}
