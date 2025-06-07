using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B4 RID: 692
public class GumballValuable : Trap
{
	// Token: 0x060015A8 RID: 5544 RVA: 0x000BF64C File Offset: 0x000BD84C
	protected void Awake()
	{
		base.Start();
		this.CloseEyes();
		this.GetValues();
		this.InstantiateHypnosisLines();
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x000BF668 File Offset: 0x000BD868
	private void InstantiateHypnosisLines()
	{
		for (int i = 0; i < this.playerCount; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.hypnosisLine, base.transform.position, Quaternion.identity);
			gameObject.transform.SetParent(this.allPlayers[i].localCameraTransform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.GetComponent<EyeLines>().InitializeLine(this.allPlayers[i]);
			gameObject.transform.SetParent(this.HypnosisLinesParenTransform);
			this.hypnosisLines.Add(gameObject);
			gameObject.SetActive(false);
		}
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x000BF720 File Offset: 0x000BD920
	private void GetValues()
	{
		this.allPlayers = SemiFunc.PlayerGetList();
		this.playerCount = this.allPlayers.Count;
		for (int i = 0; i < this.playerCount; i++)
		{
			this.playerInSight.Add(false);
			this.previouslySeenList.Add(false);
		}
		this.lightIntensityOriginal = this.pointLight.intensity;
		this.lightRangeOriginal = this.pointLight.range;
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x000BF794 File Offset: 0x000BD994
	protected override void Update()
	{
		base.Update();
		if (this.physGrabObject.grabbed && !this.isActive)
		{
			this.OpenEyes();
		}
		else if (!this.physGrabObject.grabbed && this.isActive)
		{
			this.CloseEyes();
		}
		this.CheckForLeftPlayers();
		this.ClientEffectsLoop();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.isActive)
		{
			this.CheckForListChange();
		}
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x000BF804 File Offset: 0x000BDA04
	public void ClientEffectsLoop()
	{
		this.hypnosisSoundLoop.PlayLoop(this.isActive, 1f, 1f, 1f);
		int num = 0;
		while (num < this.playerCount && this.playerCount != 0)
		{
			EyeLines component = this.hypnosisLines[num].GetComponent<EyeLines>();
			if (this.playerInSight[num])
			{
				this.hypnosisLines[num].SetActive(true);
				component.SetIsActive(true);
				if (this.allPlayers[num].isLocal)
				{
					this.VignetteOverride();
					this.ScreenSpiralOn();
					this.CameraAimAndZoom(this.allPlayers[num]);
					this.PlayerAvatarOverride(this.allPlayers[num]);
					PostProcessing.Instance.SaturationOverride(-50f, 0.8f, 5f, 0.1f, base.gameObject);
				}
			}
			else
			{
				component.SetIsActive(false);
				if (this.allPlayers[num].isLocal)
				{
					this.ScreenSpiralOff();
				}
			}
			if (!this.allPlayers[num].isLocal)
			{
				this.DrawHypnosisLines(this.allPlayers[num], this.hypnosisLines[num]);
			}
			num++;
		}
		if (this.pointLight.enabled && !this.isActive)
		{
			this.LerpLightOff(this.pointLight);
			return;
		}
		if (this.isActive)
		{
			this.LerpLightOn(this.pointLight);
		}
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x000BF97F File Offset: 0x000BDB7F
	private void CheckForListChange()
	{
		if (!SemiFunc.FPSImpulse5())
		{
			return;
		}
		this.GetAffectedPlayers();
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x000BF98F File Offset: 0x000BDB8F
	private void CheckForLeftPlayers()
	{
		if (this.playerCount != SemiFunc.PlayerGetList().Count)
		{
			this.RedoPlayersInSightList();
			this.UpdateHypnosisLinesList();
			this.playerCount = SemiFunc.PlayerGetList().Count;
			this.allPlayers = SemiFunc.PlayerGetList();
		}
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x000BF9CC File Offset: 0x000BDBCC
	private void TurnOffHypnosisLines()
	{
		for (int i = 0; i < this.playerCount; i++)
		{
			this.hypnosisLines[i].SetActive(false);
		}
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x000BF9FC File Offset: 0x000BDBFC
	private void DrawEyesIn()
	{
		for (int i = 0; i < this.playerCount; i++)
		{
			if (this.playerInSight[i] && this.allPlayers[i].isLocal)
			{
				if (!this.physGrabObject.playerGrabbing.Contains(this.allPlayers[i].physGrabber))
				{
					CameraAim.Instance.AimTargetSoftSet(this.eyeLockTransform.position, 0.1f, 2f, this.noAimStrength, base.gameObject, 100);
				}
				CameraZoom.Instance.OverrideZoomSet(this.cameraZoomAmount, 0.1f, this.cameraZoomSpeedIn * Time.deltaTime, this.cameraZoomSpeedOut * Time.deltaTime, base.gameObject, 50);
				return;
			}
		}
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x000BFAD0 File Offset: 0x000BDCD0
	private void OpenEyes()
	{
		this.isActive = true;
		this.GetAffectedPlayers();
		this.UpdateMeshes();
		this.ToggleParticles(true);
		this.ScreenSpiralOn();
		this.CameraShake();
		this.eyesOpenSound.Play(this.globe.position, 1f, 1f, 1f, 1f);
		if (!this.pointLight.enabled)
		{
			this.SetLightEnableState(true);
		}
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x000BFB44 File Offset: 0x000BDD44
	private void CloseEyes()
	{
		this.isActive = false;
		this.UpdateMeshes();
		this.ToggleParticles(false);
		this.ScreenSpiralOff();
		this.CameraShake();
		this.eyesCloseSound.Play(this.globe.position, 1f, 1f, 1f, 1f);
		this.EmptyPlayersInSightList();
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x000BFBA4 File Offset: 0x000BDDA4
	private void CameraAimAndZoom(PlayerAvatar _player)
	{
		if (!this.physGrabObject.playerGrabbing.Contains(_player.physGrabber))
		{
			CameraAim.Instance.AimTargetSoftSet(this.eyeLockTransform.position, 0.1f, 2f, this.noAimStrength, base.gameObject, 100);
		}
		CameraZoom.Instance.OverrideZoomSet(this.cameraZoomAmount, 0.1f, this.cameraZoomSpeedIn * Time.deltaTime, this.cameraZoomSpeedOut * Time.deltaTime, base.gameObject, 50);
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x000BFC2C File Offset: 0x000BDE2C
	private void PlayerAvatarOverride(PlayerAvatar _player)
	{
		_player.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Inverted, 0.25f, 0);
		_player.OverridePupilSize(3f, 4, 3f, 1f, 15f, 0.3f, 0.1f);
		SemiFunc.PlayerEyesOverride(_player, this.eyeBallMesh.position, 0.1f, base.gameObject);
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x000BFC8C File Offset: 0x000BDE8C
	private void DrawHypnosisLines(PlayerAvatar _player, GameObject _hypnosisLine)
	{
		EyeLines component = _hypnosisLine.GetComponent<EyeLines>();
		if (_hypnosisLine.activeSelf)
		{
			component.DrawLines();
		}
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x000BFCB0 File Offset: 0x000BDEB0
	private void UpdateMeshes()
	{
		this.closedLidsMesh.gameObject.SetActive(!this.isActive);
		this.openLidsMesh.gameObject.SetActive(this.isActive);
		this.eyeBallMesh.gameObject.SetActive(this.isActive);
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x000BFD04 File Offset: 0x000BDF04
	private void CameraShake()
	{
		GameDirector.instance.CameraShake.ShakeDistance(this.cameraShakeStrength, 3f, 8f, base.transform.position, this.cameraShakeTime);
		GameDirector.instance.CameraImpact.ShakeDistance(this.cameraShakeStrength, this.cameraShakeBounds.x, this.cameraShakeBounds.y, base.transform.position, this.cameraShakeTime);
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x000BFD80 File Offset: 0x000BDF80
	private void ToggleParticles(bool _state)
	{
		if (_state)
		{
			using (List<ParticleSystem>.Enumerator enumerator = this.particles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ParticleSystem particleSystem = enumerator.Current;
					particleSystem.Play();
				}
				return;
			}
		}
		foreach (ParticleSystem particleSystem2 in this.particles)
		{
			particleSystem2.Stop();
		}
	}

	// Token: 0x060015B9 RID: 5561 RVA: 0x000BFE14 File Offset: 0x000BE014
	private void VignetteOverride()
	{
		PostProcessing.Instance.VignetteOverride(new Color(0f, 0f, 0f), this.vignetteIntensity, 0.5f, 0.1f, 2f, 0.1f, base.gameObject);
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x000BFE54 File Offset: 0x000BE054
	private void ScreenSpiralOn()
	{
		if (this.spiralOnScreen)
		{
			return;
		}
		for (int i = 0; i < this.playerCount; i++)
		{
			if (this.allPlayers[i].isLocal && this.playerInSight[i])
			{
				this.spiralOnScreen = true;
				Transform localCameraTransform = this.allPlayers[i].localCameraTransform;
				GameObject gameObject = Object.Instantiate<GameObject>(this.screenSpiralEffect, base.transform.position, Quaternion.identity, localCameraTransform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				this.spiralScreenEffect = gameObject.GetComponent<SpiralOnScreen>();
				this.spiralScreenEffect.SetActive();
			}
		}
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x000BFF0F File Offset: 0x000BE10F
	private void ScreenSpiralOff()
	{
		if (this.spiralOnScreen)
		{
			this.spiralScreenEffect.SetInactive();
			this.spiralOnScreen = false;
		}
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x000BFF2B File Offset: 0x000BE12B
	private void SetLightEnableState(bool _state)
	{
		this.pointLight.intensity = 0f;
		this.pointLight.range = 0f;
		this.animationCurveEval = 0f;
		this.pointLight.enabled = _state;
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x000BFF64 File Offset: 0x000BE164
	private void LerpLightOn(Light _light)
	{
		if (_light.intensity < this.lightIntensityOriginal - 0.01f)
		{
			this.animationCurveEval += Time.deltaTime * 0.2f;
			float t = this.lightIntensityCurve.Evaluate(this.animationCurveEval);
			_light.intensity = Mathf.Lerp(_light.intensity, this.lightIntensityOriginal, t);
			_light.range = Mathf.Lerp(_light.range, this.lightRangeOriginal, t);
		}
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x000BFFE0 File Offset: 0x000BE1E0
	private void LerpLightOff(Light _light)
	{
		this.animationCurveEval += Time.deltaTime * 1f;
		float t = this.lightIntensityCurve.Evaluate(this.animationCurveEval);
		_light.intensity = Mathf.Lerp(_light.intensity, 0f, t);
		_light.range = Mathf.Lerp(_light.range, 0f, t);
		if (this.pointLight.intensity < 0.01f)
		{
			this.SetLightEnableState(false);
		}
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x000C0060 File Offset: 0x000BE260
	private void EmptyPlayersInSightList()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		for (int i = 0; i < this.playerCount; i++)
		{
			this.PlayerStateChanged(false, this.allPlayers[i].photonView.ViewID);
		}
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x000C00A4 File Offset: 0x000BE2A4
	private void GetAffectedPlayers()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		for (int i = 0; i < this.playerCount; i++)
		{
			if (!this.allPlayers[i].isDisabled)
			{
				if (this.InLineOfSight(this.allPlayers[i], i) && !this.playerInSight[i])
				{
					this.PlayerStateChanged(true, this.allPlayers[i].photonView.ViewID);
				}
				else if (!this.InLineOfSight(this.allPlayers[i], i) && this.playerInSight[i])
				{
					this.PlayerStateChanged(false, this.allPlayers[i].photonView.ViewID);
				}
			}
		}
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x000C0168 File Offset: 0x000BE368
	public void PlayerStateChanged(bool _state, int _playerID)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("PlayerStateChangedRPC", RpcTarget.All, new object[]
			{
				_state,
				_playerID
			});
			return;
		}
		this.PlayerStateChangedRPC(_state, _playerID, default(PhotonMessageInfo));
	}

	// Token: 0x060015C2 RID: 5570 RVA: 0x000C01C0 File Offset: 0x000BE3C0
	[PunRPC]
	public void PlayerStateChangedRPC(bool _state, int _playerID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		for (int i = 0; i < this.playerCount; i++)
		{
			if (this.allPlayers[i].photonView.ViewID == _playerID)
			{
				this.playerInSight[i] = _state;
				return;
			}
		}
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x000C0210 File Offset: 0x000BE410
	private bool InLineOfSight(PlayerAvatar _player, int i)
	{
		if (SemiFunc.PlayerVisionCheck(this.top.position, 10f, _player, this.previouslySeenList[i]) || SemiFunc.PlayerVisionCheck(this.bottom.position, 10f, _player, this.previouslySeenList[i]) || SemiFunc.PlayerVisionCheck(this.front.position, 10f, _player, this.previouslySeenList[i]) || SemiFunc.PlayerVisionCheck(this.back.position, 10f, _player, this.previouslySeenList[i]))
		{
			this.previouslySeenList[i] = true;
		}
		else
		{
			this.previouslySeenList[i] = false;
		}
		return this.previouslySeenList[i];
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x000C02D8 File Offset: 0x000BE4D8
	private bool IsActive()
	{
		for (int i = 0; i < this.playerCount; i++)
		{
			if (this.playerInSight[i])
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x000C0308 File Offset: 0x000BE508
	private void RedoPlayersInSightList()
	{
		for (int i = 0; i < this.playerCount; i++)
		{
			this.playerInSight[i] = false;
		}
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x000C0334 File Offset: 0x000BE534
	private void UpdateHypnosisLinesList()
	{
		this.TurnOffHypnosisLines();
		List<PlayerAvatar> list = SemiFunc.PlayerGetList();
		for (int i = 0; i < list.Count; i++)
		{
			this.hypnosisLines[i].GetComponent<EyeLines>().InitializeLine(list[i]);
			this.hypnosisLines[i].SetActive(false);
		}
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x000C038D File Offset: 0x000BE58D
	private void OnDestroy()
	{
		this.ScreenSpiralOff();
		this.TurnOffHypnosisLines();
	}

	// Token: 0x040025A2 RID: 9634
	[Header("Transforms")]
	public Transform closedLidsMesh;

	// Token: 0x040025A3 RID: 9635
	public Transform openLidsMesh;

	// Token: 0x040025A4 RID: 9636
	public Transform eyeBallMesh;

	// Token: 0x040025A5 RID: 9637
	public Transform bodyTransform;

	// Token: 0x040025A6 RID: 9638
	public Transform eyeLockTransform;

	// Token: 0x040025A7 RID: 9639
	public Transform globe;

	// Token: 0x040025A8 RID: 9640
	public Transform HypnosisLinesParenTransform;

	// Token: 0x040025A9 RID: 9641
	public Transform eyeLineParent;

	// Token: 0x040025AA RID: 9642
	[Header("Line of sight transforms")]
	public Transform top;

	// Token: 0x040025AB RID: 9643
	public Transform bottom;

	// Token: 0x040025AC RID: 9644
	public Transform front;

	// Token: 0x040025AD RID: 9645
	public Transform back;

	// Token: 0x040025AE RID: 9646
	[Header("Lights")]
	public Light pointLight;

	// Token: 0x040025AF RID: 9647
	public AnimationCurve lightIntensityCurve;

	// Token: 0x040025B0 RID: 9648
	[Header("Hypnosis Line GameObject")]
	public GameObject hypnosisLine;

	// Token: 0x040025B1 RID: 9649
	[Header("Particles")]
	public List<ParticleSystem> particles = new List<ParticleSystem>();

	// Token: 0x040025B2 RID: 9650
	[Header("Camera Shake")]
	public float cameraShakeTime = 0.2f;

	// Token: 0x040025B3 RID: 9651
	public float cameraShakeStrength = 3f;

	// Token: 0x040025B4 RID: 9652
	public Vector2 cameraShakeBounds = new Vector2(1.5f, 5f);

	// Token: 0x040025B5 RID: 9653
	[Header("Vignette")]
	public float vignetteIntensity = 0.5f;

	// Token: 0x040025B6 RID: 9654
	[Header("Camera Zoom")]
	public float cameraZoomSpeedIn = 0.5f;

	// Token: 0x040025B7 RID: 9655
	public float cameraZoomSpeedOut = 0.5f;

	// Token: 0x040025B8 RID: 9656
	public float cameraZoomAmount = 20f;

	// Token: 0x040025B9 RID: 9657
	[Header("EyeLock")]
	public float noAimStrength = 1.5f;

	// Token: 0x040025BA RID: 9658
	[Header("Spiral on screen")]
	public GameObject screenSpiralEffect;

	// Token: 0x040025BB RID: 9659
	[Header("Sounds")]
	public Sound eyesOpenSound;

	// Token: 0x040025BC RID: 9660
	public Sound eyesCloseSound;

	// Token: 0x040025BD RID: 9661
	public Sound hypnosisSoundLoop;

	// Token: 0x040025BE RID: 9662
	private bool spiralOnScreen;

	// Token: 0x040025BF RID: 9663
	private float lightIntensityOriginal;

	// Token: 0x040025C0 RID: 9664
	private float lightRangeOriginal;

	// Token: 0x040025C1 RID: 9665
	private float animationCurveEval;

	// Token: 0x040025C2 RID: 9666
	private int playerCount;

	// Token: 0x040025C3 RID: 9667
	private List<PlayerAvatar> allPlayers;

	// Token: 0x040025C4 RID: 9668
	private List<bool> playerInSight = new List<bool>();

	// Token: 0x040025C5 RID: 9669
	private List<GameObject> hypnosisLines = new List<GameObject>();

	// Token: 0x040025C6 RID: 9670
	private SpiralOnScreen spiralScreenEffect;

	// Token: 0x040025C7 RID: 9671
	private EyeLines eyeLines;

	// Token: 0x040025C8 RID: 9672
	private bool isActive;

	// Token: 0x040025C9 RID: 9673
	private int oldPlayerCount;

	// Token: 0x040025CA RID: 9674
	private List<bool> previouslySeenList = new List<bool>();
}
