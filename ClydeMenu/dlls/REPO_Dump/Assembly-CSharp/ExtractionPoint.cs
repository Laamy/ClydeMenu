using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class ExtractionPoint : MonoBehaviour
{
	// Token: 0x060007A3 RID: 1955 RVA: 0x00048804 File Offset: 0x00046A04
	private void Start()
	{
		this.shopButtonOriginalPosition = this.shopButton.localPosition;
		this.buttonOriginalMaterial = this.button.material;
		this.spotlight1.enabled = false;
		this.spotlight2.enabled = false;
		this.emojiLight.enabled = false;
		this.emojiScreen.enabled = false;
		this.haulGoalScreen.enabled = false;
		this.spotLightColor = this.spotlight1.color;
		this.tubeStartPosition = this.extractionTube.localPosition;
		this.rampStartPosition = this.ramp.localPosition;
		this.photonView = base.GetComponent<PhotonView>();
		this.originalHaulColor = this.haulGoalScreen.color;
		this.StateSet(ExtractionPoint.State.Idle);
		this.extractionTube.localPosition = new Vector3(this.tubeStartPosition.x, 0f, this.tubeStartPosition.z);
		this.spotlight1StartRotation = this.spotlightHead1.rotation;
		this.spotlight2StartRotation = this.spotlightHead2.rotation;
		this.spotlightIntensity = this.spotlight1.intensity;
		this.spotLightRange = this.spotlight1.range;
		this.emojiLightIntensity = this.emojiLight.intensity;
		this.originalEmojiLightColor = this.emojiLight.color;
		this.prevEmoji = "Jannek farts on the moon!";
		this.buttonOriginalPosition = this.button.transform.localPosition;
		RoundDirector.instance.extractionPoints++;
		RoundDirector.instance.extractionPointList.Add(base.gameObject);
		this.surplusLightIntensity = this.surplusLight.intensity;
		this.surplusLightRange = this.surplusLight.range;
		this.surplusLight.intensity = 0f;
		this.surplusLight.range = 0f;
		if (base.GetComponentInParent<StartRoom>())
		{
			this.inStartRoom = true;
		}
		this.platform.gameObject.SetActive(false);
		base.StartCoroutine(this.MapHideOnStart());
		this.isShop = SemiFunc.RunIsShop();
		if (!this.isShop)
		{
			Object.Destroy(this.shopStation.gameObject);
			return;
		}
		ShopManager.instance.isThief = false;
		ShopManager.instance.extractionPoint = base.transform;
		RoundDirector.instance.extractionPointSurplus = 0;
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00048A5D File Offset: 0x00046C5D
	private IEnumerator MapHideOnStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(1f);
		DirtFinderMapFloor[] array = this.mapActive;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MapObject.Hide();
		}
		array = this.mapUsed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MapObject.Hide();
		}
		yield break;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x00048A6C File Offset: 0x00046C6C
	public void ActivateTheFirstExtractionPointAutomaticallyWhenAPlayerLeaveTruck()
	{
		this.OnClick();
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x00048A74 File Offset: 0x00046C74
	public void OnClick()
	{
		if (this.isLocked)
		{
			return;
		}
		if (!this.StateIs(ExtractionPoint.State.Idle))
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsNotMasterClient())
			{
				RoundDirector.instance.RequestExtractionPointActivation(this.photonView.ViewID);
			}
			if (SemiFunc.IsMasterClient())
			{
				RoundDirector.instance.ExtractionPointActivate(this.photonView.ViewID);
				return;
			}
		}
		else
		{
			this.ButtonPress();
			RoundDirector.instance.extractionPointActive = true;
			RoundDirector.instance.extractionPointCurrent = this;
			RoundDirector.instance.ExtractionPointsLock(base.gameObject);
		}
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x00048B00 File Offset: 0x00046D00
	public void ButtonPress()
	{
		if (this.StateIs(ExtractionPoint.State.Idle))
		{
			this.StateSet(ExtractionPoint.State.Active);
		}
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00048B14 File Offset: 0x00046D14
	private void SetLightsEmissionColor(Color color)
	{
		Material[] materials = this.spotlightHead1.GetComponentInChildren<MeshRenderer>().materials;
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].SetColor("_EmissionColor", color);
		}
		materials = this.spotlightHead2.GetComponentInChildren<MeshRenderer>().materials;
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].SetColor("_EmissionColor", color);
		}
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x00048B7B File Offset: 0x00046D7B
	private void TextBlink(Color textColorOriginal, Color textColor, float time)
	{
		this.textBlinkTime = time;
		this.textBlinkColor = textColor;
		this.textBlinkColorOriginal = textColorOriginal;
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x00048B94 File Offset: 0x00046D94
	private void TextBlinkLogic()
	{
		if (this.textBlinkTime > 0f)
		{
			this.textBlinkTime -= Time.deltaTime;
			if (this.textBlinkTime <= 0f)
			{
				this.haulGoalScreen.color = this.textBlinkColorOriginal;
				return;
			}
			this.haulGoalScreen.color = this.textBlinkColor;
		}
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x00048BF0 File Offset: 0x00046DF0
	public void OnShopClick()
	{
		if (!this.StateIs(ExtractionPoint.State.Active) || this.tubeSlamDownEval < 1f)
		{
			return;
		}
		if (this.haulGoal - this.haulCurrent >= 0 && this.haulGoal - this.haulCurrent != this.haulGoal)
		{
			this.StateSet(ExtractionPoint.State.Success);
			return;
		}
		this.StateSet(ExtractionPoint.State.Cancel);
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x00048C48 File Offset: 0x00046E48
	private void ShopButtonAnimation()
	{
		if (!this.shopButtonAnimation)
		{
			return;
		}
		this.shopButtonAnimationEval += Time.deltaTime * 2f;
		this.shopButtonAnimationEval = Mathf.Clamp01(this.shopButtonAnimationEval);
		float num = this.buttonPressAnimationCurve.Evaluate(this.shopButtonAnimationEval);
		Color a = new Color(1f, 0.5f, 0f, 1f);
		this.shopButton.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(a, Color.white, num));
		num = Mathf.Clamp(num, 0.5f, 1f);
		this.shopButton.localScale = new Vector3(1f, num, 1f);
		if (this.shopButtonAnimationEval >= 1f)
		{
			this.shopButtonAnimation = false;
			this.shopButtonAnimationEval = 0f;
		}
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x00048D28 File Offset: 0x00046F28
	public void HitCeiling()
	{
		this.ceilingParticles.Play();
		this.soundTubeHitCeiling.Play(this.extractionTube.position, 1f, 1f, 1f, 1f);
		this.soundTubeHitCeilingGlobal.Play(this.extractionTube.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, this.extractionTube.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.extractionTube.position, 0.1f);
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00048DF4 File Offset: 0x00046FF4
	private void EmojiScreenGlitch(Color color)
	{
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			this.soundEmojiGlitch.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
		}
		this.emojiScreenGlitchTimer = 0.2f;
		this.emojiScreenGlitch.SetActive(true);
		this.emojiScreen.enabled = false;
		this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x00048E7C File Offset: 0x0004707C
	private void HaulGoalSet(int value)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("HaulGoalSetRPC", RpcTarget.All, new object[]
				{
					value
				});
				return;
			}
		}
		else
		{
			this.HaulGoalSetRPC(value, default(PhotonMessageInfo));
		}
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x00048EC8 File Offset: 0x000470C8
	[PunRPC]
	public void HaulGoalSetRPC(int value, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.haulGoal = value;
		RoundDirector.instance.extractionHaulGoal = value;
		this.haulGoalFetched = true;
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x00048EEC File Offset: 0x000470EC
	private void ResetLights()
	{
		this.spotlight1.range = this.spotLightRange;
		this.spotlight2.range = this.spotLightRange;
		this.spotlight1.intensity = this.spotlightIntensity;
		this.spotlight2.intensity = this.spotlightIntensity;
		this.emojiLight.intensity = this.emojiLightIntensity;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00048F50 File Offset: 0x00047150
	private void EmojiScreenGlitchLogic()
	{
		if (this.emojiDelay > 0f)
		{
			return;
		}
		this.currentEmoji = this.emojiScreen.text;
		if (this.prevEmoji != this.currentEmoji)
		{
			this.prevEmoji = this.currentEmoji;
			this.EmojiScreenGlitch(Color.yellow);
		}
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			return;
		}
		Vector2 textureOffset = this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.GetTextureOffset("_MainTex");
		textureOffset.y += Time.deltaTime * 15f;
		this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", textureOffset);
		this.emojiScreenGlitchTimer -= Time.deltaTime;
		if (this.thirtyFPSUpdate)
		{
			float num = Random.Range(0.1f, 1f);
			this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(num, num));
		}
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			this.emojiScreenGlitch.SetActive(false);
			this.emojiScreen.enabled = true;
		}
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00049070 File Offset: 0x00047270
	private void HaulInternalStatsUpdate()
	{
		this.haulPrevious = this.haulCurrent;
		this.haulCurrent = RoundDirector.instance.currentHaul + RoundDirector.instance.extractionPointSurplus;
		if (this.isShop)
		{
			this.haulCurrent = SemiFunc.ShopGetTotalCost() * 1000;
			this.haulGoal = SemiFunc.StatGetRunCurrency() * 1000;
		}
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x000490D0 File Offset: 0x000472D0
	private void HaulChecker()
	{
		this.HaulInternalStatsUpdate();
		if (this.haulPrevious != this.haulCurrent)
		{
			this.haulUpdateEffectTimer = 0.3f;
			if (this.haulCurrent > this.haulPrevious)
			{
				this.deductedFromHaul = false;
				this.soundHaulIncrease.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.deductedFromHaul = true;
				this.soundHaulDecrease.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
			}
			this.haulPrevious = this.haulCurrent;
		}
		if (this.haulUpdateEffectTimer > 0f)
		{
			this.haulUpdateEffectTimer -= Time.deltaTime;
			this.haulUpdateEffectTimer = Mathf.Max(0f, this.haulUpdateEffectTimer);
			Color color = Color.white;
			if (this.deductedFromHaul)
			{
				color = Color.red;
			}
			if (this.isShop)
			{
				color = Color.red;
				if (this.deductedFromHaul)
				{
					color = Color.white;
				}
			}
			this.haulGoalScreen.color = color;
			if (this.thirtyFPSUpdate)
			{
				this.haulGoalScreen.text = this.GlitchyText();
			}
			this.resetHaulText = false;
		}
		else if (!this.resetHaulText)
		{
			this.haulGoalScreen.color = this.originalHaulColor;
			this.SetHaulText();
			this.resetHaulText = true;
		}
		if (!this.isShop)
		{
			if (this.haulGoal - this.haulCurrent <= 0 && this.haulGoalFetched)
			{
				this.successDelay -= Time.deltaTime;
				if (this.successDelay <= 0f)
				{
					this.StateSet(ExtractionPoint.State.Success);
					return;
				}
			}
			else
			{
				this.successDelay = 1.5f;
			}
		}
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00049298 File Offset: 0x00047498
	private void UpdateEmojiText()
	{
		if (this.haulCurrent == 0)
		{
			this.SetEmojiScreen("<sprite name=:'(>", false);
			if (this.isShop)
			{
				this.SetEmojiScreen("<sprite name=shoppingcart>", false);
				if (this.isThief)
				{
					this.SetEmojiScreen("<sprite name=thief>", false);
					return;
				}
			}
		}
		else
		{
			float num = (float)this.haulCurrent / (float)this.haulGoal;
			string[] array = new string[]
			{
				"<sprite name=:'(>",
				"<sprite name=:(>",
				"<sprite name=mellow>",
				"<sprite name=:)>",
				"<sprite name=:D>",
				"<sprite name=cryinglaughing>"
			};
			if (this.isShop)
			{
				num = 0f;
				array = new string[]
				{
					"<sprite name=shoppingcart>"
				};
				if (this.isThief)
				{
					array = new string[]
					{
						"<sprite name=thief>"
					};
				}
			}
			int num2 = Mathf.FloorToInt(num * (float)(array.Length - 1));
			num2 = Mathf.Clamp(num2, 0, array.Length - 1);
			this.SetEmojiScreen(array[num2], false);
		}
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x00049388 File Offset: 0x00047588
	private void ThirtyFPS()
	{
		if (this.thirtyFPSUpdateTimer > 0f)
		{
			this.thirtyFPSUpdateTimer -= Time.deltaTime;
			this.thirtyFPSUpdateTimer = Mathf.Max(0f, this.thirtyFPSUpdateTimer);
			return;
		}
		this.thirtyFPSUpdate = true;
		this.thirtyFPSUpdateTimer = 0.033333335f;
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x000493E0 File Offset: 0x000475E0
	private void Update()
	{
		this.ShopButtonAnimation();
		bool playing = this.tubeHit && this.tubeSlamDownEval > 0.8f && this.StateIs(ExtractionPoint.State.Extracting);
		this.soundSuckLoop.PlayLoop(playing, 2f, 2f, 1f);
		bool playing2 = this.StateIs(ExtractionPoint.State.Warning);
		this.soundWarningLightsLoop.PlayLoop(playing2, 2f, 2f, 1f);
		bool playing3 = this.StateIs(ExtractionPoint.State.Surplus) && !this.haulSurplusAnimatedDone && !this.surplusIntroText;
		this.surplusStateIncreaseLoop.PlayLoop(playing3, 2f, 2f, 1f);
		this.ThirtyFPS();
		this.HaulBarAnimateScale();
		this.StateCancel();
		this.StateIdle();
		this.StateActive();
		this.StateSuccess();
		this.StateSurplus();
		this.StateWarning();
		this.StateExtracting();
		this.StateComplete();
		this.StateTaxReturn();
		this.EmojiScreenGlitchLogic();
		this.TextBlinkLogic();
		this.SurplusLightLogic();
		this.TubeScreenTextChangeLogic();
		this.stateEnd = false;
		this.thirtyFPSUpdate = false;
		if (this.stateTimer > 0f)
		{
			if (this.initialStateTime == 0f)
			{
				this.initialStateTime = this.stateTimer;
			}
			this.stateTimer -= Time.deltaTime;
			this.stateTimer = Mathf.Max(0f, this.stateTimer);
		}
		else if (!this.stateEnd && this.stateTimer != -123f)
		{
			this.stateEnd = true;
			this.stateTimer = -123f;
			this.initialStateTime = 0f;
		}
		if (this.stateSetTo != ExtractionPoint.State.None)
		{
			this.currentState = this.stateSetTo;
			this.stateStart = true;
			this.settingState = false;
			this.stateEnd = false;
			this.stateSetTo = ExtractionPoint.State.None;
		}
		if (this.isLocked && this.buttonGrabObject.enabled && this.buttonGrabObject.playerGrabbing.Count > 0 && this.buttonDenyCooldown <= 0f)
		{
			foreach (PhysGrabber physGrabber in Enumerable.ToList<PhysGrabber>(this.buttonGrabObject.playerGrabbing))
			{
				if (!SemiFunc.IsMultiplayer())
				{
					physGrabber.ReleaseObject(0.1f);
				}
				else
				{
					physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
					{
						false,
						1.5f
					});
				}
			}
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("ButtonDenyRPC", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this.ButtonDenyRPC();
			}
		}
		if (this.buttonDenyCooldown > 0f)
		{
			this.buttonDenyCooldown -= Time.deltaTime;
			if (this.buttonDenyCooldown <= 0f)
			{
				this.buttonLight.enabled = false;
				this.button.material = this.buttonOff;
				this.buttonGrabObject.enabled = true;
			}
		}
		if (this.buttonDenyActive)
		{
			this.buttonDenyLerp += 0.75f * Time.deltaTime;
			this.buttonDenyLerp = Mathf.Clamp01(this.buttonDenyLerp);
			this.buttonDenyTransform.localPosition = new Vector3(0f, 0f, -0.06f * this.buttonDenyCurve.Evaluate(this.buttonDenyLerp));
			if (this.buttonDenyLerp >= 1f)
			{
				this.buttonDenyActive = false;
				this.buttonDenyLerp = 0f;
			}
		}
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x00049770 File Offset: 0x00047970
	private void StateIdle()
	{
		if (!this.StateIs(ExtractionPoint.State.Idle))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		if (this.isLocked)
		{
			if (this.tubeScreenTextString != "LOCKED")
			{
				this.TubeScreenTextChange("LOCKED", Color.red);
			}
			this.ButtonToggle(false);
			return;
		}
		Color color = new Color(1f, 0.5f, 0f);
		if (this.tubeScreenTextString != "READY")
		{
			this.TubeScreenTextChange("READY", color);
		}
		if (this.soundPingTimer <= 0f && !SemiFunc.RunIsTutorial() && !SemiFunc.RunIsRecording())
		{
			this.soundPing.Play(this.soundPingTransform.position, 1f, 1f, 1f, 1f);
			this.soundPingTimer = 4f;
		}
		else
		{
			this.soundPingTimer -= Time.deltaTime;
		}
		this.ButtonToggle(true);
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00049868 File Offset: 0x00047A68
	private void StateActive()
	{
		if (!this.StateIs(ExtractionPoint.State.Active))
		{
			return;
		}
		if (this.stateStart)
		{
			this.extractionArea.SetActive(false);
			this.taxReturn = false;
			if (this.tubeScreenTextString != "ACTIVE")
			{
				this.TubeScreenTextChange("ACTIVE", Color.green);
			}
			DirtFinderMapFloor[] array = this.mapInactive;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MapObject.Hide();
			}
			array = this.mapActive;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MapObject.Show();
			}
			this.platform.gameObject.SetActive(true);
			this.emojiLight.enabled = true;
			this.ButtonToggle(false);
			this.emojiScreen.enabled = true;
			this.haulGoalScreen.enabled = true;
			this.spotlight1.enabled = false;
			this.spotlight2.enabled = false;
			this.emojiLight.enabled = false;
			this.emojiScreen.enabled = false;
			this.roomVolume.SetActive(true);
			this.tubeSlamDownEval = 0f;
			this.emojiLight.color = this.originalEmojiLightColor;
			this.emojiDelay = 2f;
			this.ResetLightIntensity();
			this.spotlight1Delay = 1f;
			this.spotlight2Delay = 1.5f;
			this.successDelay = 0f;
			this.tubeHitCeiling = false;
			this.ResetLights();
			if (this.cancelExtraction)
			{
				this.buttonPressed = false;
				this.tubeSlamDownEval = 1f;
				this.cancelExtraction = false;
				this.spotlight1Delay = 0f;
				this.spotlight2Delay = 0f;
				this.emojiDelay = 0f;
			}
			else
			{
				if (!this.isShop)
				{
					if (!this.inStartRoom)
					{
						SemiFunc.EnemyInvestigate(base.transform.position, 20f, false);
					}
					this.jingleLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.jingleGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.buttonDelay = 0.5f;
				this.buttonPressed = true;
				GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, this.button.transform.position, 0.1f);
				GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.button.transform.position, 0.1f);
				this.soundButton.Play(this.button.transform.position, 1f, 1f, 1f, 1f);
				int value = RoundDirector.instance.haulGoal / RoundDirector.instance.extractionPoints;
				if (this.isShop)
				{
					value = SemiFunc.StatGetRunCurrency() * 1000;
				}
				this.HaulGoalSet(value);
			}
			this.haulGoalScreen.color = this.originalHaulColor;
			this.currentEmoji = this.emojiScreen.text;
			this.stateStart = false;
			this.HaulInternalStatsUpdate();
			this.SetHaulText();
			this.UpdateEmojiText();
		}
		if (!this.tubeHitCeiling && this.buttonPressed && this.tubeSlamDownEval > 0.8f)
		{
			this.tubeHitCeiling = true;
			this.HitCeiling();
		}
		if (this.buttonPressed)
		{
			if (this.buttonPressEval < 1f)
			{
				this.buttonPressEval += 8f * Time.deltaTime;
				this.buttonPressEval = Mathf.Min(1f, this.buttonPressEval);
				float num = this.buttonPressAnimationCurve.Evaluate(this.buttonPressEval);
				this.button.transform.localPosition = new Vector3(this.buttonOriginalPosition.x, this.buttonOriginalPosition.y, this.buttonOriginalPosition.z - 0.1f * num);
			}
			if (this.emojiDelay > 0f && !this.isShop)
			{
				SemiFunc.UIBigMessage("EXTRACTION POINT ACTIVATED", "{!}", 25f, Color.white, Color.white);
				SemiFunc.UIFocusText("Fill the extraction point with valuables", Color.white, AssetManager.instance.colorYellow, 3f);
			}
		}
		if (this.buttonDelay >= 0f)
		{
			this.buttonDelay -= Time.deltaTime;
		}
		if (this.spotlight1Delay <= 0f)
		{
			if (!this.spotlight1.enabled)
			{
				if (this.buttonPressed)
				{
					this.soundActivate1.Play(this.spotlight1.transform.position, 1f, 1f, 1f, 1f);
					GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, this.spotlight1.transform.position, 0.1f);
					GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.spotlight1.transform.position, 0.1f);
				}
				this.spotlight1.enabled = true;
				this.spotlight1.color = this.spotLightColor;
			}
		}
		else if (this.buttonDelay <= 0f)
		{
			this.spotlight1Delay -= Time.deltaTime;
		}
		if (this.spotlight2Delay <= 0f)
		{
			if (!this.spotlight2.enabled)
			{
				if (this.buttonPressed)
				{
					this.soundActivate2.Play(this.spotlight2.transform.position, 1f, 1f, 1f, 1f);
					GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, this.spotlight2.transform.position, 0.1f);
					GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.spotlight2.transform.position, 0.1f);
				}
				this.spotlight2.enabled = true;
				this.spotlight2.color = this.spotLightColor;
			}
		}
		else if (this.buttonDelay <= 0f)
		{
			this.spotlight2Delay -= Time.deltaTime;
		}
		if (this.emojiDelay <= 0f)
		{
			if (!this.emojiLight.enabled)
			{
				if (this.buttonPressed)
				{
					this.soundActivate3.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
					GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, this.emojiScreen.transform.position, 0.1f);
					GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.emojiScreen.transform.position, 0.1f);
				}
				this.emojiLight.enabled = true;
				this.emojiScreen.enabled = true;
				this.SetHaulText();
			}
		}
		else
		{
			if (this.buttonDelay <= 0f)
			{
				this.emojiDelay -= Time.deltaTime;
			}
			if (this.thirtyFPSUpdate)
			{
				this.haulGoalScreen.text = this.GlitchyText();
			}
		}
		if (this.tubeSlamDownEval < 1f && this.buttonDelay <= 0f)
		{
			if (this.tubeSlamDownEval == 0f)
			{
				this.tubeHitParticles.SetActive(true);
				this.upParticles.Play();
				this.soundTubeRaise.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.soundTubeRaiseGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
				GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, this.extractionTube.position, 0.1f);
				GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.extractionTube.position, 0.1f);
			}
			this.tubeSlamDownEval += 2f * Time.deltaTime;
			this.tubeSlamDownEval = Mathf.Min(1f, this.tubeSlamDownEval);
			float num2 = this.tubeSlamDown.Evaluate(this.tubeSlamDownEval);
			this.extractionTube.localPosition = new Vector3(this.tubeStartPosition.x, this.tubeStartPosition.y * num2, this.tubeStartPosition.z);
		}
		if (!this.isCompletedRightAway)
		{
			this.HaulChecker();
		}
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0004A170 File Offset: 0x00048370
	private void HaulBarAnimateScale()
	{
		float num = Mathf.Lerp(this.haulBar.localScale.x, this.haulBarTargetScale, Time.deltaTime * 10f);
		num = Mathf.Clamp01(num);
		if (float.IsNaN(num))
		{
			num = 0f;
		}
		this.haulBar.localScale = new Vector3(num, 1f, 1f);
		this.haulBar.localScale = new Vector3(Mathf.Min(1f, this.haulBar.localScale.x), 1f, 1f);
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0004A208 File Offset: 0x00048408
	private void SetHaulText()
	{
		if (!this.isShop)
		{
			string text = "<color=#bd4300>$</color>";
			this.haulGoalScreen.text = text + SemiFunc.DollarGetString(Mathf.Max(0, this.haulCurrent));
			this.haulBarTargetScale = (float)this.haulCurrent / (float)this.haulGoal;
		}
		else
		{
			string text2 = "<color=#bd4300>$</color>";
			this.haulGoalScreen.text = text2 + SemiFunc.DollarGetString(this.haulGoal - this.haulCurrent);
			if (this.haulGoal - this.haulCurrent < 0)
			{
				this.haulGoalScreen.text = SemiFunc.DollarGetString(this.haulGoal - this.haulCurrent);
				this.haulGoalScreen.color = Color.red;
			}
		}
		this.UpdateEmojiText();
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0004A2C8 File Offset: 0x000484C8
	private void SetEmojiScreen(string emoji, bool creepyFace = false)
	{
		if (creepyFace)
		{
			this.grossUp.SetActive(true);
		}
		else
		{
			this.grossUp.SetActive(false);
		}
		this.emojiScreen.text = "<size=100>|</size>" + emoji + "<size=100>|</size>";
		this.emojiScreen.color = new Color(1f, 1f, 1f, 0f);
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0004A334 File Offset: 0x00048534
	private void ShopButtonPushVisualsStart()
	{
		if (!this.isShop)
		{
			return;
		}
		if (!this.shopButtonAnimation)
		{
			this.shopButton.localScale = new Vector3(1f, 0.1f, 1f);
			this.soundButton.Play(this.shopButton.position, 1f, 1f, 1f, 1f);
			this.shopButtonAnimationEval = 0f;
			this.shopButtonAnimation = true;
		}
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0004A3B0 File Offset: 0x000485B0
	private void StateSuccess()
	{
		if (!this.StateIs(ExtractionPoint.State.Success))
		{
			return;
		}
		if (this.stateStart)
		{
			this.ShopButtonPushVisualsStart();
			this.ResetLights();
			this.ResetLightIntensity();
			this.extractionArea.SetActive(false);
			this.SetSpotlightColor(Color.green);
			this.tubeSlamDownEval = 0f;
			this.tubeHitParticles.SetActive(false);
			this.haulGoalScreen.text = "$" + SemiFunc.DollarGetString(Mathf.Max(0, RoundDirector.instance.haulGoal - RoundDirector.instance.currentHaul));
			this.stateStart = false;
			this.SetEmojiScreen("<sprite name=check>", false);
			this.emojiLight.color = Color.green;
			this.EmojiScreenGlitch(Color.green);
			this.emojiDelay = 0f;
			this.soundSuccess.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundGreenLights.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.haulGoalScreen.text = "!!!!!!!!!";
			this.haulGoalScreen.color = Color.green;
			this.stateTimer = 2f;
			if (this.isShop)
			{
				this.stateTimer = 1f;
			}
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, this.haulGoalScreen.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.haulGoalScreen.transform.position, 0.1f);
		}
		if (!this.isCompletedRightAway)
		{
			this.CancelExtraction();
		}
		if (this.stateEnd)
		{
			if (this.isShop)
			{
				this.StateSet(ExtractionPoint.State.Warning);
				return;
			}
			this.haulSurplus = Mathf.Abs(RoundDirector.instance.currentHaul - this.haulGoal);
			if (this.haulSurplus > 0)
			{
				this.StateSet(ExtractionPoint.State.Surplus);
				return;
			}
			this.StateSet(ExtractionPoint.State.Warning);
		}
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x0004A5D8 File Offset: 0x000487D8
	private void StateSurplus()
	{
		if (!this.StateIs(ExtractionPoint.State.Surplus))
		{
			return;
		}
		if (this.stateStart)
		{
			if (RoundDirector.instance.extractionPointsCompleted != RoundDirector.instance.extractionPoints - 1)
			{
				this.taxReturn = true;
			}
			this.ResetLights();
			this.ResetLightIntensity();
			this.extractionArea.SetActive(false);
			this.SetSpotlightColor(Color.green);
			this.SetEmojiScreen("<sprite name=surplus>", false);
			this.emojiLight.color = Color.green;
			this.EmojiScreenGlitch(Color.green);
			this.emojiDelay = 0f;
			this.tubeSlamDownEval = 0f;
			this.tubeHitParticles.SetActive(false);
			this.stateStart = false;
			this.surplusStateStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundGreenLights.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.haulGoalScreen.text = "TAX RETURN";
			this.haulGoalScreen.color = Color.green;
			this.haulSurplusAnimated = 0;
			this.haulSurplusAnimatedDone = false;
			this.surplusIntroText = true;
			this.cancelExtraction = true;
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, this.haulGoalScreen.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.haulGoalScreen.transform.position, 0.1f);
			if (this.haulSurplus < 1000)
			{
				this.stateTimer = 4f;
				this.surplusLevel = 1;
			}
			else if (this.haulSurplus >= 1000 && this.haulSurplus < 5000)
			{
				this.stateTimer = 5f;
				this.surplusLevel = 2;
			}
			else if (this.haulSurplus >= 5000 && this.haulSurplus < 10000)
			{
				this.stateTimer = 6f;
				this.surplusLevel = 3;
			}
			else if (this.haulSurplus >= 10000 && this.haulSurplus < 20000)
			{
				this.stateTimer = 7f;
				this.surplusLevel = 4;
			}
			else if (this.haulSurplus >= 20000 && this.haulSurplus < 50000)
			{
				this.stateTimer = 8f;
				this.surplusLevel = 4;
			}
			else if (this.haulSurplus >= 50000)
			{
				this.stateTimer = 9f;
				this.surplusLevel = 4;
			}
		}
		if (!this.isCompletedRightAway)
		{
			this.CancelExtraction();
		}
		if (!this.haulSurplusAnimatedDone)
		{
			this.haulSurplus = Mathf.Abs(RoundDirector.instance.currentHaul - this.haulGoal);
		}
		float num = 1.5f;
		if (this.stateTimer < this.initialStateTime - num)
		{
			this.surplusIntroText = false;
			if (this.haulSurplusAnimated < this.haulSurplus && this.initialStateTime != 0f)
			{
				float num2 = 2f;
				float num3 = (this.initialStateTime - num - this.stateTimer) / (this.initialStateTime - num - num2);
				num3 = Mathf.Clamp01(num3);
				this.surplusStateIncreaseLoop.LoopPitch = 1f + num3;
				this.haulSurplusAnimated = (int)((float)this.haulSurplus * num3);
				this.haulGoalScreen.text = "+$" + SemiFunc.DollarGetString(this.haulSurplusAnimated);
			}
			else if (!this.haulSurplusAnimatedDone && this.initialStateTime != 0f)
			{
				this.haulSurplus = Mathf.Abs(this.haulCurrent - this.haulGoal);
				this.SetEmojiScreen("<sprite name=moneyeyes>", false);
				GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, this.haulGoalScreen.transform.position, 0.1f);
				GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.haulGoalScreen.transform.position, 0.1f);
				this.haulSurplusAnimatedDone = true;
				this.TextBlink(Color.green, Color.white, 0.5f);
				if (this.surplusLevel == 1)
				{
					this.surplusStateDoneLevel1.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				if (this.surplusLevel == 2)
				{
					this.surplusStateDoneLevel1.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				if (this.surplusLevel == 3)
				{
					this.surplusStateDoneLevel1.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				if (this.surplusLevel == 4)
				{
					this.surplusStateDoneLevel1.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
			}
		}
		if (this.stateEnd && !this.isCompletedRightAway)
		{
			this.StateSet(ExtractionPoint.State.Warning);
		}
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x0004AB08 File Offset: 0x00048D08
	private void SpawnTaxReturn()
	{
		if (RoundDirector.instance.extractionPointSurplus > 0)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				GameObject gameObject = AssetManager.instance.surplusValuableSmall;
				if (RoundDirector.instance.extractionPointSurplus > 10000)
				{
					gameObject = AssetManager.instance.surplusValuableBig;
				}
				else if (RoundDirector.instance.extractionPointSurplus > 5000)
				{
					gameObject = AssetManager.instance.surplusValuableMedium;
				}
				GameObject gameObject2;
				if (!SemiFunc.IsMultiplayer())
				{
					gameObject2 = Object.Instantiate<GameObject>(gameObject, this.surplusSpawnTransform.position, Quaternion.identity);
				}
				else
				{
					gameObject2 = PhotonNetwork.InstantiateRoomObject("Valuables/" + gameObject.name, this.surplusSpawnTransform.position, Quaternion.identity, 0, null);
				}
				gameObject2.GetComponent<ValuableObject>().dollarValueOverride = RoundDirector.instance.extractionPointSurplus;
				gameObject2.GetComponent<PhysGrabObject>().spawnTorque = Random.insideUnitSphere * 0.05f;
			}
			this.surplusLightActive = true;
			this.surplusLight.intensity = this.surplusLightIntensity;
			this.surplusLight.range = this.surplusLightRange;
		}
		RoundDirector.instance.extractionPointSurplus = 0;
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x0004AC24 File Offset: 0x00048E24
	private bool CancelExtraction()
	{
		this.haulCurrent = RoundDirector.instance.currentHaul + RoundDirector.instance.extractionPointSurplus;
		if (this.isShop)
		{
			this.haulCurrent = SemiFunc.ShopGetTotalCost() * 1000;
		}
		if (!this.isShop)
		{
			if (this.haulGoal - this.haulCurrent > 0)
			{
				this.StateSet(ExtractionPoint.State.Cancel);
				return true;
			}
		}
		else if (this.haulGoal - this.haulCurrent < 0 || this.haulGoal - this.haulCurrent == this.haulGoal)
		{
			this.StateSet(ExtractionPoint.State.Cancel);
			return true;
		}
		return false;
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0004ACB5 File Offset: 0x00048EB5
	private void ResetLightIntensity()
	{
		this.emojiLight.intensity = this.emojiLightIntensity;
		this.spotlight1.intensity = this.spotlightIntensity;
		this.spotlight2.intensity = this.spotlightIntensity;
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0004ACEC File Offset: 0x00048EEC
	private void StateWarning()
	{
		if (!this.StateIs(ExtractionPoint.State.Warning))
		{
			return;
		}
		if (this.stateStart)
		{
			this.ResetLights();
			this.SetSpotlightColor(Color.red);
			this.ResetLightIntensity();
			this.spotlight1.intensity = this.spotlightIntensity * 2f;
			this.spotlight2.intensity = this.spotlightIntensity * 2f;
			this.spotlight1.range *= 2f;
			this.spotlight2.range *= 2f;
			this.SetEmojiScreen("<sprite name=!>", false);
			this.emojiLight.color = Color.red;
			this.EmojiScreenGlitch(Color.red);
			this.stateTimer = 3f;
			this.stateStart = false;
			this.haulGoalScreen.text = "3";
			this.haulGoalScreen.color = Color.red;
			this.soundAlarm.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
			this.soundAlarmGlobal.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
			SemiFunc.EnemyInvestigate(base.transform.position, 20f, false);
			this.extractionArea.SetActive(true);
			this.HaulInternalStatsUpdate();
			this.SetHaulText();
		}
		if (this.stateTimer > 0f)
		{
			string text = Mathf.CeilToInt(this.stateTimer).ToString();
			if (this.haulGoalScreen.text != text)
			{
				this.soundAlarm.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
				this.soundAlarmGlobal.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
				this.haulGoalScreen.text = text;
			}
		}
		else if (this.thirtyFPSUpdate)
		{
			this.haulGoalScreen.text = this.GlitchyText();
		}
		this.spotlightHead1.Rotate(0f, 500f * Time.deltaTime, 0f);
		this.spotlightHead2.Rotate(0f, -500f * Time.deltaTime, 0f);
		if (!this.isCompletedRightAway)
		{
			this.CancelExtraction();
		}
		if (this.stateEnd)
		{
			this.StateSet(ExtractionPoint.State.Extracting);
		}
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0004AF88 File Offset: 0x00049188
	private void SetSpotlightColor(Color color)
	{
		this.spotlight1.color = color;
		this.spotlight2.color = color;
		this.SetLightsEmissionColor(color);
		this.spotlight1.intensity = this.spotlightIntensity;
		this.spotlight2.intensity = this.spotlightIntensity;
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0004AFD8 File Offset: 0x000491D8
	private void StateCancel()
	{
		if (!this.StateIs(ExtractionPoint.State.Cancel))
		{
			return;
		}
		if (this.stateStart)
		{
			this.ShopButtonPushVisualsStart();
			this.taxReturn = false;
			this.extractionArea.SetActive(false);
			this.SetEmojiScreen("<sprite name=X>", false);
			this.emojiLight.color = Color.red;
			this.emojiLight.intensity = this.emojiLightIntensity;
			this.haulGoalScreen.color = Color.red;
			this.ResetLights();
			this.spotlight1CancelRotation = this.spotlightHead1.rotation;
			this.spotlight2CancelRotation = this.spotlightHead2.rotation;
			if (this.spotlight1CancelRotation != this.spotlight1StartRotation || this.spotlight2CancelRotation != this.spotlight2StartRotation)
			{
				this.cancelSpotlights = true;
			}
			this.cancelSpotlightEval = 0f;
			this.ResetLightIntensity();
			this.SetSpotlightColor(Color.red);
			this.stateTimer = 1f;
			this.stateStart = false;
			this.suckParticles.Stop();
			this.soundCancel.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.cancelExtraction = true;
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, this.emojiScreen.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.emojiScreen.transform.position, 0.1f);
			if (this.cancelTube)
			{
				this.tubeCancelPosition = this.extractionTube.localPosition;
				this.tubeSlamDownEval = 0f;
				this.stateTimer = 2f;
				this.soundTubeRetract.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.tubeHitCeiling = false;
			}
		}
		if (this.cancelTube && !this.tubeHitCeiling && this.tubeSlamDownEval > 0.8f)
		{
			this.tubeHitCeiling = true;
			this.HitCeiling();
		}
		if (this.thirtyFPSUpdate)
		{
			this.haulGoalScreen.text = this.GlitchyText();
		}
		if (this.cancelTube)
		{
			if (this.tubeSlamDownEval < 1f)
			{
				this.tubeSlamDownEval += 4f * Time.deltaTime;
				this.tubeSlamDownEval = Mathf.Min(1f, this.tubeSlamDownEval);
				float num = this.tubeSlamDown.Evaluate(this.tubeSlamDownEval);
				this.extractionTube.localPosition = new Vector3(this.tubeStartPosition.x, this.tubeStartPosition.y * num, this.tubeStartPosition.z);
			}
			else
			{
				this.cancelTube = false;
			}
		}
		if (this.cancelSpotlights)
		{
			if (this.cancelSpotlightEval < 1f)
			{
				this.cancelSpotlightEval += 4f * Time.deltaTime;
				this.cancelSpotlightEval = Mathf.Min(1f, this.cancelSpotlightEval);
				float t = this.tubeSlamDown.Evaluate(this.cancelSpotlightEval);
				this.spotlightHead1.rotation = Quaternion.Lerp(this.spotlight1CancelRotation, this.spotlight1StartRotation, t);
				this.spotlightHead2.rotation = Quaternion.Lerp(this.spotlight2CancelRotation, this.spotlight2StartRotation, t);
			}
			else
			{
				this.cancelSpotlights = false;
			}
		}
		if (this.stateEnd)
		{
			this.StateSet(ExtractionPoint.State.Active);
		}
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0004B358 File Offset: 0x00049558
	private void StateExtracting()
	{
		if (!this.StateIs(ExtractionPoint.State.Extracting))
		{
			return;
		}
		if (this.stateStart)
		{
			Color color = new Color(1f, 0.5f, 0f);
			this.TubeScreenTextChange("EXTRACTING", color);
			this.cancelTube = true;
			this.extractionArea.SetActive(false);
			this.ResetLights();
			this.stateTimer = 5f;
			this.tubeHit = false;
			this.stateStart = false;
			this.cancelExtraction = false;
			this.soundTubeBuildup.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundAlarmFinal.Play(this.emojiScreen.transform.position, 1f, 1f, 1f, 1f);
			this.tubeSlamDownEval = 0f;
			if (this.isShop)
			{
				this.hurtColliderMainTimer = 0.25f;
			}
			else
			{
				this.hurtColliderMainTimer = 1f;
			}
			CurrencyUI.instance.FetchCurrency();
			if (this.isShop)
			{
				this.stateTimer = 2f;
			}
			this.HaulInternalStatsUpdate();
			this.SetHaulText();
			this.SetEmojiScreen("<sprite name=creepycrying>", true);
		}
		if (this.tubeSlamDownEval > 0f && this.tubeSlamDownEval < 0.8f)
		{
			this.hurtColliderMain.SetActive(false);
			this.hurtColliders.SetActive(true);
		}
		else
		{
			this.hurtColliders.SetActive(false);
		}
		if (this.tubeSlamDownEval > 0.8f && !this.tubeHit)
		{
			this.tubeHitParticles.SetActive(true);
			GameDirector.instance.CameraImpact.ShakeDistance(10f, 3f, 8f, this.extractionTube.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(10f, 3f, 8f, this.extractionTube.position, 0.1f);
			this.suckParticles.Play();
			this.tubeHit = true;
			this.amountOfValuables = RoundDirector.instance.dollarHaulList.Count;
			this.suckUpTimeLeft = this.stateTimer;
			this.suckUpVariableTimer = this.suckUpTimeLeft / (float)this.amountOfValuables;
			this.soundTubeSlam.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundTubeSlamGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
			if (!this.isShop)
			{
				if (!this.isCompletedRightAway)
				{
					RoundDirector.instance.HaulCheck();
				}
			}
			else
			{
				ShopManager.instance.ShopCheck();
			}
			this.extractionHaul = this.haulGoal;
			if (!this.isCompletedRightAway && this.CancelExtraction())
			{
				return;
			}
			if (!this.cancelExtraction)
			{
				this.ExtractionPointSurplus();
				RoundDirector.instance.ExtractionCompletedAllCheck();
			}
		}
		if (this.cancelExtraction)
		{
			return;
		}
		if (this.tubeHit)
		{
			if (!this.isShop)
			{
				if (!this.isCompletedRightAway)
				{
					SemiFunc.UIBigMessage("EXTRACTION POINT COMPLETED", "{check}", 25f, Color.white, Color.white);
				}
				else
				{
					SemiFunc.UIBigMessage("EXTRACTION POINT SKIPPED", "{:O}", 25f, Color.white, Color.white);
				}
				this.haulSurplus = Mathf.Abs(this.haulCurrent - this.haulGoal);
				if (RoundDirector.instance.extractionPointsCompleted == RoundDirector.instance.extractionPoints - 1)
				{
					this.extractionHaul = this.haulGoal + this.haulSurplus;
				}
				HaulUI.instance.Hide();
				ShopCostUI.instance.Show();
				CurrencyUI.instance.Show();
				float num = this.stateTimer / this.suckUpTimeLeft;
				ShopCostUI.instance.animatedValue = Mathf.CeilToInt(Mathf.Lerp(0f, Mathf.Ceil((float)(this.extractionHaul / 1000)), 1f - num));
			}
			if (this.suckUpVariableTimer <= 0f)
			{
				if (!this.isShop)
				{
					this.DestroyTheFirstPhysObjectsInHaulList();
				}
				else
				{
					this.DestroyTheFirstPhysObjectsInShopList();
				}
				this.suckUpVariableTimer = this.suckUpTimeLeft / (float)this.amountOfValuables;
			}
			else
			{
				this.suckUpVariableTimer -= Time.deltaTime;
			}
			if (this.hurtColliderMainTimer > 0f)
			{
				this.hurtColliderMainTimer -= Time.deltaTime;
				if (this.hurtColliderMainTimer <= 0f)
				{
					this.hurtColliderMain.SetActive(true);
				}
			}
			GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, this.extractionTube.position, 0.5f);
			GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, this.extractionTube.position, 0.5f);
		}
		if (this.tubeSlamDownEval < 1f)
		{
			this.spotlightHead1.Rotate(0f, 500f * Time.deltaTime, 0f);
			this.spotlightHead2.Rotate(0f, -500f * Time.deltaTime, 0f);
			if (this.thirtyFPSUpdate)
			{
				this.haulGoalScreen.text = this.GlitchyText();
			}
			this.tubeSlamDownEval += 2f * Time.deltaTime;
			this.tubeSlamDownEval = Mathf.Min(1f, this.tubeSlamDownEval);
			float num2 = this.tubeSlamDown.Evaluate(this.tubeSlamDownEval);
			this.extractionTube.localPosition = new Vector3(this.tubeStartPosition.x, this.tubeStartPosition.y * (1f - num2), this.tubeStartPosition.z);
			this.spotlight1.intensity = 6f * (1f - num2);
			this.spotlight2.intensity = 6f * (1f - num2);
			this.emojiLight.intensity = 5f * (1f - num2);
		}
		if (this.tubeHit && !this.isShop)
		{
			this.extractionTube.localPosition = new Vector3(0f, 0f + 0.025f * Mathf.Sin(Time.time * 60f), 0f);
			this.suckInRampEval += 2f * Time.deltaTime;
			this.suckInRampEval = Mathf.Min(1f, this.suckInRampEval);
			float num3 = this.tubeSlamDown.Evaluate(this.suckInRampEval);
			this.ramp.localPosition = new Vector3(this.rampStartPosition.x, this.rampStartPosition.y - 0.05f * num3, this.rampStartPosition.z * (1f - num3));
		}
		if ((double)this.stateTimer <= 0.3)
		{
			this.upParticles.Play();
		}
		if (this.stateEnd)
		{
			this.hurtColliderMain.SetActive(false);
			if (!this.taxReturn)
			{
				this.StateSet(ExtractionPoint.State.Complete);
			}
			else
			{
				this.StateSet(ExtractionPoint.State.TaxReturn);
			}
			this.tubeHitParticles.SetActive(false);
			if (!this.isShop)
			{
				this.DestroyAllPhysObjectsInHaulList();
				this.roomVolume.SetActive(false);
				return;
			}
			this.DestroyAllPhysObjectsInShoppingList();
		}
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0004BAA0 File Offset: 0x00049CA0
	private string GlitchyText()
	{
		string text = "";
		for (int i = 0; i < 9; i++)
		{
			bool flag = false;
			if (Random.Range(0, 4) == 0 && i <= 5)
			{
				text += "TAX";
				i += 2;
				flag = true;
			}
			if (Random.Range(0, 3) == 0 && !flag)
			{
				text += "$";
				flag = true;
			}
			if (!flag)
			{
				text += Random.Range(0, 10).ToString();
			}
		}
		return text;
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x0004BB18 File Offset: 0x00049D18
	private void ThiefPunishment()
	{
		if (SemiFunc.ShopGetTotalCost() <= 0)
		{
			return;
		}
		this.isThief = true;
		ShopManager.instance.isThief = true;
		this.SetEmojiScreen("<sprite name=thief>", false);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (TruckScreenText.instance.playerChatBoxState == TruckScreenText.PlayerChatBoxState.Idle)
			{
				TruckScreenText.instance.GotoPage(3);
			}
			if (SemiFunc.IsMultiplayer())
			{
				for (int i = 0; i < 5; i++)
				{
					PhotonNetwork.InstantiateRoomObject("Items/Item Grenade Explosive", base.transform.position + base.transform.up * 0.2f + base.transform.up * (0.2f * (float)i), Quaternion.identity, 0, null);
				}
			}
			else
			{
				for (int j = 0; j < 5; j++)
				{
					Object.Instantiate(Resources.Load("Items/Item Grenade Explosive"), base.transform.position + base.transform.up * 0.2f + base.transform.up * (0.2f * (float)j), Quaternion.identity);
				}
			}
		}
		SemiFunc.UIFocusText("Avoid the grenades!", Color.red, Color.red, 3f);
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0004BC54 File Offset: 0x00049E54
	private void StateTaxReturn()
	{
		if (!this.StateIs(ExtractionPoint.State.TaxReturn))
		{
			return;
		}
		if (this.stateStart)
		{
			this.cancelTube = false;
			this.tubeHitCeiling = false;
			this.tubeSlamDownEval = 0f;
			this.extractionArea.SetActive(false);
			this.tubeHitParticles.SetActive(true);
			this.soundSuckEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.suckParticles.Stop();
			this.spotlightHead1.rotation = this.spotlight1StartRotation;
			this.spotlightHead2.rotation = this.spotlight2StartRotation;
			this.stateStart = false;
			this.runCurrencyBefore = SemiFunc.StatGetRunCurrency();
			this.extractionHaul = Mathf.CeilToInt((float)(this.extractionHaul / 1000));
			SemiFunc.StatSetRunCurrency(this.runCurrencyBefore + this.extractionHaul);
			CurrencyUI.instance.FetchCurrency();
			RoundDirector.instance.ExtractionCompleted();
			this.platform.gameObject.SetActive(false);
			this.completeJingleGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.completeJingleLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
			this.stateTimer = 3f;
		}
		CurrencyUI.instance.Show();
		if (!this.tubeHitCeiling && this.tubeSlamDownEval > 0.8f)
		{
			this.tubeHitCeiling = true;
			this.HitCeiling();
			this.TubeScreenTextChange("TAX RETURN", Color.green);
			this.SpawnTaxReturn();
		}
		if (this.tubeSlamDownEval < 1f)
		{
			this.tubeSlamDownEval += 2f * Time.deltaTime;
			this.tubeSlamDownEval = Mathf.Min(1f, this.tubeSlamDownEval);
			float num = this.tubeSlamDown.Evaluate(1f - this.tubeSlamDownEval);
			this.extractionTube.localPosition = new Vector3(this.tubeStartPosition.x, 2.2f * (1f - num), this.tubeStartPosition.z);
		}
		if (this.stateEnd)
		{
			this.StateSet(ExtractionPoint.State.Complete);
		}
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0004BEF8 File Offset: 0x0004A0F8
	private void StateComplete()
	{
		if (!this.StateIs(ExtractionPoint.State.Complete))
		{
			return;
		}
		if (this.stateStart)
		{
			this.TubeScreenTextChange("COMPLETED", Color.green);
			this.extractionArea.SetActive(false);
			this.cancelTube = false;
			if (!this.shopStation)
			{
				DirtFinderMapFloor[] array = this.mapActive;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].MapObject.Hide();
				}
				array = this.mapUsed;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].MapObject.Show();
				}
			}
			this.suckParticles.Stop();
			this.tubeSlamDownEval = 0f;
			this.stateStart = false;
			if (!this.taxReturn)
			{
				this.tubeHitParticles.SetActive(true);
				this.soundSuckEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			this.soundTubeRaise.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundTubeRaiseGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.tubeHitCeiling = false;
			this.spotlightHead1.rotation = this.spotlight1StartRotation;
			this.spotlightHead2.rotation = this.spotlight2StartRotation;
			if (!this.isShop)
			{
				RoundDirector.instance.extractionPointActive = false;
				RoundDirector.instance.ExtractionPointsUnlock();
				if (!this.taxReturn)
				{
					this.HaulInternalStatsUpdate();
					this.extractionHaul = this.haulGoal + this.haulSurplus;
					int num = SemiFunc.StatGetRunTotalHaul();
					this.runCurrencyBefore = SemiFunc.StatGetRunCurrency();
					this.extractionHaul = Mathf.CeilToInt((float)(this.extractionHaul / 1000));
					SemiFunc.StatSetRunCurrency(this.runCurrencyBefore + this.extractionHaul);
					SemiFunc.StatSetRunTotalHaul(num + this.extractionHaul);
					CurrencyUI.instance.FetchCurrency();
					RoundDirector.instance.ExtractionCompleted();
					this.platform.gameObject.SetActive(false);
					this.completeJingleGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.completeJingleLocal.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.stateTimer = 3f;
				if (RoundDirector.instance.extractionPointsCompleted < RoundDirector.instance.extractionPoints)
				{
					SemiFunc.UIFocusText("Find the next extraction point", Color.white, AssetManager.instance.colorYellow, 3f);
				}
				else
				{
					SemiFunc.UIFocusText("Get back to the truck!", Color.white, AssetManager.instance.colorYellow, 3f);
				}
			}
			else
			{
				this.cancelExtraction = true;
				this.isThief = false;
				ShopManager.instance.isThief = false;
				this.stateTimer = 1f;
				this.ThiefPunishment();
				this.HaulInternalStatsUpdate();
				this.SetHaulText();
			}
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
		}
		if (!this.isShop && this.stateTimer > 0f)
		{
			CurrencyUI.instance.Show();
		}
		if (!this.tubeHitCeiling && this.tubeSlamDownEval > 0.8f)
		{
			this.tubeHitCeiling = true;
			this.HitCeiling();
		}
		if (this.tubeSlamDownEval < 1f)
		{
			this.tubeSlamDownEval += 2f * Time.deltaTime;
			this.tubeSlamDownEval = Mathf.Min(1f, this.tubeSlamDownEval);
			float num2 = this.tubeSlamDown.Evaluate(1f - this.tubeSlamDownEval);
			if (this.taxReturn)
			{
				this.extractionTube.localPosition = new Vector3(this.tubeStartPosition.x, Mathf.LerpUnclamped(this.tubeStartPosition.y, 2.2f, num2), this.tubeStartPosition.z);
			}
			else
			{
				this.extractionTube.localPosition = new Vector3(this.tubeStartPosition.x, this.tubeStartPosition.y * (1f - num2), this.tubeStartPosition.z);
			}
		}
		if (this.stateEnd && this.isShop)
		{
			this.cancelExtraction = true;
			this.StateSet(ExtractionPoint.State.Active);
		}
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0004C398 File Offset: 0x0004A598
	private void ExtractionPointSurplus()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				int num = this.CalculateSurplus();
				this.photonView.RPC("ExtractionPointSurplusRPC", RpcTarget.All, new object[]
				{
					num
				});
				return;
			}
		}
		else
		{
			int surplus = this.CalculateSurplus();
			this.ExtractionPointSurplusRPC(surplus, default(PhotonMessageInfo));
		}
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0004C3F4 File Offset: 0x0004A5F4
	private int CalculateSurplus()
	{
		int b = this.haulCurrent - this.haulGoal;
		return Mathf.Max(0, b);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0004C418 File Offset: 0x0004A618
	private void SurplusLightLogic()
	{
		if (this.surplusLightActive)
		{
			if (this.surplusLightTimer > 0f)
			{
				this.surplusLightTimer -= Time.deltaTime;
				if (this.surplusLightTimer <= 0f)
				{
					this.surplusLightOutroSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
					return;
				}
			}
			else
			{
				this.surplusLightLerp += 2f * Time.deltaTime;
				this.surplusLight.intensity = Mathf.Lerp(0f, this.surplusLightIntensity, this.surplusLightOutro.Evaluate(this.surplusLightLerp));
				if (this.surplusLightLerp >= 1f)
				{
					this.surplusLight.intensity = 0f;
					this.surplusLight.range = 0f;
					this.surplusLightActive = false;
				}
			}
		}
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0004C503 File Offset: 0x0004A703
	[PunRPC]
	public void ExtractionPointSurplusRPC(int surplus, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		RoundDirector.instance.extractionPointSurplus = surplus;
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0004C51C File Offset: 0x0004A71C
	private void StateSet(ExtractionPoint.State newState)
	{
		if (this.settingState)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient() && this.stateSetTo == ExtractionPoint.State.None)
			{
				this.settingState = true;
				this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
				{
					newState
				});
				return;
			}
		}
		else if (this.stateSetTo == ExtractionPoint.State.None)
		{
			this.settingState = true;
			this.StateSetRPC(newState, default(PhotonMessageInfo));
		}
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0004C58F File Offset: 0x0004A78F
	[PunRPC]
	public void StateSetRPC(ExtractionPoint.State state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.stateSetTo = state;
		this.stateTimer = 0f;
		this.stateEnd = true;
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0004C5B3 File Offset: 0x0004A7B3
	private bool StateIs(ExtractionPoint.State state)
	{
		return this.currentState == state;
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0004C5C0 File Offset: 0x0004A7C0
	private void DestroyTheFirstPhysObjectsInHaulList()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (RoundDirector.instance.dollarHaulList.Count == 0)
			{
				return;
			}
			if (RoundDirector.instance.dollarHaulList[0] && RoundDirector.instance.dollarHaulList[0].GetComponent<PhysGrabObject>())
			{
				RoundDirector.instance.totalHaul += (int)RoundDirector.instance.dollarHaulList[0].GetComponent<ValuableObject>().dollarValueCurrent;
				RoundDirector.instance.dollarHaulList[0].GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
				RoundDirector.instance.dollarHaulList.RemoveAt(0);
			}
		}
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0004C674 File Offset: 0x0004A874
	private void DestroyTheFirstPhysObjectsInShopList()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (ShopManager.instance.shoppingList.Count == 0)
			{
				return;
			}
			ItemAttributes itemAttributes = ShopManager.instance.shoppingList[0];
			if (itemAttributes && itemAttributes.GetComponent<PhysGrabObject>() && SemiFunc.StatGetRunCurrency() - itemAttributes.value >= 0)
			{
				SemiFunc.StatSetRunCurrency(SemiFunc.StatGetRunCurrency() - itemAttributes.value);
				StatsManager.instance.ItemPurchase(itemAttributes.item.itemAssetName);
				if (itemAttributes.item.itemType == SemiFunc.itemType.item_upgrade)
				{
					StatsManager.instance.AddItemsUpgradesPurchased(itemAttributes.item.itemAssetName);
				}
				if (itemAttributes.item.itemType == SemiFunc.itemType.power_crystal)
				{
					Dictionary<string, int> runStats = StatsManager.instance.runStats;
					runStats["chargingStationChargeTotal"] = runStats["chargingStationChargeTotal"] + 17;
					if (StatsManager.instance.runStats["chargingStationChargeTotal"] > 100)
					{
						StatsManager.instance.runStats["chargingStationChargeTotal"] = 100;
					}
					Debug.Log("Charging station charge total: " + StatsManager.instance.runStats["chargingStationChargeTotal"].ToString());
				}
				itemAttributes.GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
				ShopManager.instance.shoppingList.RemoveAt(0);
			}
			SemiFunc.ShopUpdateCost();
		}
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0004C7D0 File Offset: 0x0004A9D0
	private void DestroyAllPhysObjectsInShoppingList()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				playerAvatar.playerDeathHead.Revive();
			}
			List<ItemAttributes> list = new List<ItemAttributes>();
			foreach (ItemAttributes itemAttributes in ShopManager.instance.shoppingList)
			{
				if (itemAttributes && itemAttributes.GetComponent<PhysGrabObject>() && SemiFunc.StatGetRunCurrency() - itemAttributes.value >= 0)
				{
					SemiFunc.StatSetRunCurrency(SemiFunc.StatGetRunCurrency() - itemAttributes.GetComponent<ItemAttributes>().value);
					StatsManager.instance.ItemPurchase(itemAttributes.item.itemAssetName);
					if (itemAttributes.item.itemType == SemiFunc.itemType.item_upgrade)
					{
						StatsManager.instance.AddItemsUpgradesPurchased(itemAttributes.item.itemAssetName);
					}
					if (itemAttributes.item.itemType == SemiFunc.itemType.power_crystal)
					{
						Dictionary<string, int> runStats = StatsManager.instance.runStats;
						runStats["chargingStationChargeTotal"] = runStats["chargingStationChargeTotal"] + 17;
						if (StatsManager.instance.runStats["chargingStationChargeTotal"] > 100)
						{
							StatsManager.instance.runStats["chargingStationChargeTotal"] = 100;
						}
					}
					itemAttributes.GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
					list.Add(itemAttributes);
				}
			}
			foreach (ItemAttributes itemAttributes2 in list)
			{
				ShopManager.instance.shoppingList.Remove(itemAttributes2);
			}
			SemiFunc.ShopUpdateCost();
		}
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0004C9E4 File Offset: 0x0004ABE4
	private void DestroyAllPhysObjectsInHaulList()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				playerAvatar.playerDeathHead.Revive();
			}
			foreach (GameObject gameObject in RoundDirector.instance.dollarHaulList)
			{
				if (gameObject && gameObject.GetComponent<PhysGrabObject>())
				{
					RoundDirector.instance.totalHaul += (int)gameObject.GetComponent<ValuableObject>().dollarValueCurrent;
					gameObject.GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
				}
			}
		}
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0004CAC4 File Offset: 0x0004ACC4
	private void TubeScreenTextChange(string text, Color color)
	{
		if (this.tubeScreenTextString != text)
		{
			this.soundActivate2.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.tubeScreenTextString = text;
		this.tubeScreenChangeTimer = 0.2f;
		this.tubeScreenText.color = Color.white;
		this.tubeScreenLight.color = Color.white;
		this.tubeScreenTextColor = color;
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0004CB44 File Offset: 0x0004AD44
	private void TubeScreenTextChangeLogic()
	{
		if (this.tubeScreenChangeTimer > 0f)
		{
			this.tubeScreenChangeTimer -= Time.deltaTime;
			if (this.thirtyFPSUpdate)
			{
				this.tubeScreenText.text = this.GlitchyText();
			}
			if (this.tubeScreenChangeTimer <= 0f)
			{
				this.tubeScreenText.text = this.tubeScreenTextString;
				this.tubeScreenText.color = this.tubeScreenTextColor;
				this.tubeScreenLight.color = this.tubeScreenTextColor;
			}
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0004CBCC File Offset: 0x0004ADCC
	private void ButtonToggle(bool _active)
	{
		if (this.buttonActive == _active)
		{
			return;
		}
		if (this.buttonDenyCooldown > 0f)
		{
			this.buttonDenyCooldown = 0f;
		}
		if (_active)
		{
			this.button.material = this.buttonOriginalMaterial;
			this.buttonGrabObject.enabled = true;
			this.buttonLight.enabled = true;
		}
		else
		{
			this.button.material = this.buttonOff;
			this.buttonGrabObject.enabled = true;
			this.buttonLight.enabled = false;
		}
		if (this.currentState != ExtractionPoint.State.Idle)
		{
			this.buttonGrabObject.enabled = false;
		}
		this.buttonActive = _active;
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0004CC70 File Offset: 0x0004AE70
	[PunRPC]
	private void ButtonDenyRPC()
	{
		this.buttonDenyActive = true;
		this.buttonDenyLerp = 0f;
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, this.emojiScreen.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.emojiScreen.transform.position, 0.1f);
		this.soundCancel.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.buttonLight.enabled = true;
		this.button.material = this.buttonDenyMaterial;
		this.buttonGrabObject.enabled = false;
		this.buttonDenyCooldown = 1f;
		PlayerAvatar playerAvatarScript = PlayerController.instance.playerAvatarScript;
		if (!playerAvatarScript.isDisabled && Vector3.Distance(playerAvatarScript.transform.position, base.transform.position) < 10f && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialOnlyOneExtraction, 1))
		{
			TutorialDirector.instance.ActivateTip("Only One Extraction", 2f, false);
		}
	}

	// Token: 0x04000D72 RID: 3442
	public GameObject extractionArea;

	// Token: 0x04000D73 RID: 3443
	public Sound soundButton;

	// Token: 0x04000D74 RID: 3444
	public Sound soundActivate1;

	// Token: 0x04000D75 RID: 3445
	public Sound soundActivate2;

	// Token: 0x04000D76 RID: 3446
	public Sound soundActivate3;

	// Token: 0x04000D77 RID: 3447
	public Sound soundAlarm;

	// Token: 0x04000D78 RID: 3448
	public Sound soundAlarmGlobal;

	// Token: 0x04000D79 RID: 3449
	public Sound soundAlarmFinal;

	// Token: 0x04000D7A RID: 3450
	public Sound soundCancel;

	// Token: 0x04000D7B RID: 3451
	public Sound soundEmojiGlitch;

	// Token: 0x04000D7C RID: 3452
	public Sound soundGreenLights;

	// Token: 0x04000D7D RID: 3453
	public Sound soundLightsOn;

	// Token: 0x04000D7E RID: 3454
	public Sound soundHaulIncrease;

	// Token: 0x04000D7F RID: 3455
	public Sound soundHaulDecrease;

	// Token: 0x04000D80 RID: 3456
	public Sound soundWarningLightsLoop;

	// Token: 0x04000D81 RID: 3457
	public Sound soundSuccess;

	// Token: 0x04000D82 RID: 3458
	public Sound soundSuckEnd;

	// Token: 0x04000D83 RID: 3459
	public Sound soundSuckLoop;

	// Token: 0x04000D84 RID: 3460
	public Sound soundTubeBuildup;

	// Token: 0x04000D85 RID: 3461
	public Sound soundTubeSlam;

	// Token: 0x04000D86 RID: 3462
	public Sound soundTubeSlamGlobal;

	// Token: 0x04000D87 RID: 3463
	public Sound soundTubeRaise;

	// Token: 0x04000D88 RID: 3464
	public Sound soundTubeRaiseGlobal;

	// Token: 0x04000D89 RID: 3465
	public Sound soundTubeRetract;

	// Token: 0x04000D8A RID: 3466
	public Sound soundTubeHitCeiling;

	// Token: 0x04000D8B RID: 3467
	public Sound soundTubeHitCeilingGlobal;

	// Token: 0x04000D8C RID: 3468
	public Sound jingleLocal;

	// Token: 0x04000D8D RID: 3469
	public Sound jingleGlobal;

	// Token: 0x04000D8E RID: 3470
	public Sound surplusStateStart;

	// Token: 0x04000D8F RID: 3471
	public Sound surplusStateIncreaseLoop;

	// Token: 0x04000D90 RID: 3472
	public Sound surplusStateDoneLevel1;

	// Token: 0x04000D91 RID: 3473
	public Sound surplusStateDoneLevel2;

	// Token: 0x04000D92 RID: 3474
	public Sound surplusStateDoneLevel3;

	// Token: 0x04000D93 RID: 3475
	public Sound surplusStateDoneLevel4;

	// Token: 0x04000D94 RID: 3476
	public Sound surplusDeductionStart;

	// Token: 0x04000D95 RID: 3477
	public Sound surplusDeductionLoop;

	// Token: 0x04000D96 RID: 3478
	public Sound surplusDeductionEnd;

	// Token: 0x04000D97 RID: 3479
	public Sound completeJingleLocal;

	// Token: 0x04000D98 RID: 3480
	public Sound completeJingleGlobal;

	// Token: 0x04000D99 RID: 3481
	public Sound soundPing;

	// Token: 0x04000D9A RID: 3482
	public Transform soundPingTransform;

	// Token: 0x04000D9B RID: 3483
	public Sound surplusLightOutroSound;

	// Token: 0x04000D9C RID: 3484
	public Transform safetySpawn;

	// Token: 0x04000D9D RID: 3485
	public Transform haulBar;

	// Token: 0x04000D9E RID: 3486
	private float haulBarTargetScale;

	// Token: 0x04000D9F RID: 3487
	private bool stateStart = true;

	// Token: 0x04000DA0 RID: 3488
	public TextMeshPro emojiScreen;

	// Token: 0x04000DA1 RID: 3489
	public TextMeshPro haulGoalScreen;

	// Token: 0x04000DA2 RID: 3490
	public TextMeshPro tubeScreenText;

	// Token: 0x04000DA3 RID: 3491
	public Light tubeScreenLight;

	// Token: 0x04000DA4 RID: 3492
	public Light spotlight1;

	// Token: 0x04000DA5 RID: 3493
	public Light spotlight2;

	// Token: 0x04000DA6 RID: 3494
	public Light emojiLight;

	// Token: 0x04000DA7 RID: 3495
	private float tubeScreenChangeTimer;

	// Token: 0x04000DA8 RID: 3496
	private string tubeScreenTextString = "";

	// Token: 0x04000DA9 RID: 3497
	private Color tubeScreenTextColor = Color.white;

	// Token: 0x04000DAA RID: 3498
	public GameObject grossUp;

	// Token: 0x04000DAB RID: 3499
	[Space]
	public Light buttonLight;

	// Token: 0x04000DAC RID: 3500
	public MeshRenderer button;

	// Token: 0x04000DAD RID: 3501
	public Material buttonOff;

	// Token: 0x04000DAE RID: 3502
	public StaticGrabObject buttonGrabObject;

	// Token: 0x04000DAF RID: 3503
	public Material buttonDenyMaterial;

	// Token: 0x04000DB0 RID: 3504
	private bool buttonActive;

	// Token: 0x04000DB1 RID: 3505
	private float buttonDenyCooldown;

	// Token: 0x04000DB2 RID: 3506
	public Transform buttonDenyTransform;

	// Token: 0x04000DB3 RID: 3507
	public AnimationCurve buttonDenyCurve;

	// Token: 0x04000DB4 RID: 3508
	private bool buttonDenyActive;

	// Token: 0x04000DB5 RID: 3509
	private float buttonDenyLerp;

	// Token: 0x04000DB6 RID: 3510
	[Space]
	public Material spotlightOff;

	// Token: 0x04000DB7 RID: 3511
	public Material spotlightOn;

	// Token: 0x04000DB8 RID: 3512
	public Transform extractionTube;

	// Token: 0x04000DB9 RID: 3513
	public Transform spotlightHead1;

	// Token: 0x04000DBA RID: 3514
	public Transform spotlightHead2;

	// Token: 0x04000DBB RID: 3515
	private Color spotLightColor;

	// Token: 0x04000DBC RID: 3516
	private float stateTimer;

	// Token: 0x04000DBD RID: 3517
	private bool stateEnd;

	// Token: 0x04000DBE RID: 3518
	public AnimationCurve tubeSlamDown;

	// Token: 0x04000DBF RID: 3519
	public AnimationCurve buttonPressAnimationCurve;

	// Token: 0x04000DC0 RID: 3520
	private float tubeSlamDownEval;

	// Token: 0x04000DC1 RID: 3521
	private Vector3 tubeStartPosition;

	// Token: 0x04000DC2 RID: 3522
	private bool thirtyFPSUpdate;

	// Token: 0x04000DC3 RID: 3523
	private float thirtyFPSUpdateTimer;

	// Token: 0x04000DC4 RID: 3524
	public Transform platform;

	// Token: 0x04000DC5 RID: 3525
	public Transform ramp;

	// Token: 0x04000DC6 RID: 3526
	private Vector3 rampStartPosition;

	// Token: 0x04000DC7 RID: 3527
	[Space]
	public GameObject hurtColliders;

	// Token: 0x04000DC8 RID: 3528
	public GameObject hurtColliderMain;

	// Token: 0x04000DC9 RID: 3529
	private float hurtColliderMainTimer;

	// Token: 0x04000DCA RID: 3530
	[Space]
	public GameObject tubeHitParticles;

	// Token: 0x04000DCB RID: 3531
	private bool tubeHit;

	// Token: 0x04000DCC RID: 3532
	public ParticleSystem suckParticles;

	// Token: 0x04000DCD RID: 3533
	public ParticleSystem upParticles;

	// Token: 0x04000DCE RID: 3534
	public ParticleSystem ceilingParticles;

	// Token: 0x04000DCF RID: 3535
	private PhotonView photonView;

	// Token: 0x04000DD0 RID: 3536
	public GameObject roomVolume;

	// Token: 0x04000DD1 RID: 3537
	public GameObject emojiScreenGlitch;

	// Token: 0x04000DD2 RID: 3538
	private int amountOfValuables;

	// Token: 0x04000DD3 RID: 3539
	private float suckUpVariableTimer;

	// Token: 0x04000DD4 RID: 3540
	private float suckUpTimeLeft;

	// Token: 0x04000DD5 RID: 3541
	private float haulUpdateEffectTimer;

	// Token: 0x04000DD6 RID: 3542
	private int haulPrevious;

	// Token: 0x04000DD7 RID: 3543
	private int haulCurrent;

	// Token: 0x04000DD8 RID: 3544
	private bool deductedFromHaul;

	// Token: 0x04000DD9 RID: 3545
	private Color originalHaulColor;

	// Token: 0x04000DDA RID: 3546
	private bool resetHaulText;

	// Token: 0x04000DDB RID: 3547
	private bool settingState;

	// Token: 0x04000DDC RID: 3548
	private float spotlight1Delay;

	// Token: 0x04000DDD RID: 3549
	private float spotlight2Delay;

	// Token: 0x04000DDE RID: 3550
	private float emojiDelay;

	// Token: 0x04000DDF RID: 3551
	private float successDelay;

	// Token: 0x04000DE0 RID: 3552
	[Space]
	public Transform surplusSpawnTransform;

	// Token: 0x04000DE1 RID: 3553
	public Light surplusLight;

	// Token: 0x04000DE2 RID: 3554
	public AnimationCurve surplusLightOutro;

	// Token: 0x04000DE3 RID: 3555
	private bool surplusLightActive;

	// Token: 0x04000DE4 RID: 3556
	private float surplusLightIntensity;

	// Token: 0x04000DE5 RID: 3557
	private float surplusLightRange;

	// Token: 0x04000DE6 RID: 3558
	private float surplusLightTimer = 5f;

	// Token: 0x04000DE7 RID: 3559
	private float surplusLightLerp;

	// Token: 0x04000DE8 RID: 3560
	private int haulSurplus;

	// Token: 0x04000DE9 RID: 3561
	private int haulSurplusAnimated;

	// Token: 0x04000DEA RID: 3562
	private bool haulSurplusAnimatedDone;

	// Token: 0x04000DEB RID: 3563
	private int surplusLevel;

	// Token: 0x04000DEC RID: 3564
	private bool surplusIntroText;

	// Token: 0x04000DED RID: 3565
	[HideInInspector]
	public int haulGoal;

	// Token: 0x04000DEE RID: 3566
	private bool cancelExtraction;

	// Token: 0x04000DEF RID: 3567
	private Vector3 tubeCancelPosition;

	// Token: 0x04000DF0 RID: 3568
	private bool cancelTube;

	// Token: 0x04000DF1 RID: 3569
	private Quaternion spotlight1StartRotation;

	// Token: 0x04000DF2 RID: 3570
	private Quaternion spotlight2StartRotation;

	// Token: 0x04000DF3 RID: 3571
	private Quaternion spotlight1CancelRotation;

	// Token: 0x04000DF4 RID: 3572
	private Quaternion spotlight2CancelRotation;

	// Token: 0x04000DF5 RID: 3573
	private bool cancelSpotlights;

	// Token: 0x04000DF6 RID: 3574
	private float cancelSpotlightEval;

	// Token: 0x04000DF7 RID: 3575
	private float spotlightIntensity;

	// Token: 0x04000DF8 RID: 3576
	private float spotLightRange;

	// Token: 0x04000DF9 RID: 3577
	private float emojiLightIntensity;

	// Token: 0x04000DFA RID: 3578
	private Color originalEmojiLightColor;

	// Token: 0x04000DFB RID: 3579
	private float emojiScreenGlitchTimer;

	// Token: 0x04000DFC RID: 3580
	private string prevEmoji;

	// Token: 0x04000DFD RID: 3581
	private string currentEmoji;

	// Token: 0x04000DFE RID: 3582
	private float buttonDelay;

	// Token: 0x04000DFF RID: 3583
	private float buttonPressEval;

	// Token: 0x04000E00 RID: 3584
	private bool buttonPressed;

	// Token: 0x04000E01 RID: 3585
	private Vector3 buttonOriginalPosition;

	// Token: 0x04000E02 RID: 3586
	private bool tubeHitCeiling;

	// Token: 0x04000E03 RID: 3587
	private bool haulGoalFetched;

	// Token: 0x04000E04 RID: 3588
	[HideInInspector]
	public bool isLocked;

	// Token: 0x04000E05 RID: 3589
	private float suckInRampEval;

	// Token: 0x04000E06 RID: 3590
	private Material buttonOriginalMaterial;

	// Token: 0x04000E07 RID: 3591
	private bool isShop;

	// Token: 0x04000E08 RID: 3592
	private bool taxReturn;

	// Token: 0x04000E09 RID: 3593
	private bool inStartRoom;

	// Token: 0x04000E0A RID: 3594
	[Space]
	public Transform shopStation;

	// Token: 0x04000E0B RID: 3595
	public Transform shopButton;

	// Token: 0x04000E0C RID: 3596
	private float shopButtonAnimationEval;

	// Token: 0x04000E0D RID: 3597
	private bool shopButtonAnimation;

	// Token: 0x04000E0E RID: 3598
	private Vector3 shopButtonOriginalPosition;

	// Token: 0x04000E0F RID: 3599
	private float initialStateTime;

	// Token: 0x04000E10 RID: 3600
	private float textBlinkTime;

	// Token: 0x04000E11 RID: 3601
	private Color textBlinkColor = Color.white;

	// Token: 0x04000E12 RID: 3602
	private Color textBlinkColorOriginal = Color.white;

	// Token: 0x04000E13 RID: 3603
	private int extractionHaul;

	// Token: 0x04000E14 RID: 3604
	private int runCurrencyBefore;

	// Token: 0x04000E15 RID: 3605
	private ExtractionPoint.State stateSetTo;

	// Token: 0x04000E16 RID: 3606
	private float soundPingTimer;

	// Token: 0x04000E17 RID: 3607
	[Space]
	public DirtFinderMapFloor[] mapActive;

	// Token: 0x04000E18 RID: 3608
	public DirtFinderMapFloor[] mapUsed;

	// Token: 0x04000E19 RID: 3609
	public DirtFinderMapFloor[] mapInactive;

	// Token: 0x04000E1A RID: 3610
	internal ExtractionPoint.State currentState = ExtractionPoint.State.Idle;

	// Token: 0x04000E1B RID: 3611
	private bool isThief;

	// Token: 0x04000E1C RID: 3612
	private bool isCompletedRightAway;

	// Token: 0x0200033D RID: 829
	public enum State
	{
		// Token: 0x040029D3 RID: 10707
		None,
		// Token: 0x040029D4 RID: 10708
		Idle,
		// Token: 0x040029D5 RID: 10709
		Active,
		// Token: 0x040029D6 RID: 10710
		Success,
		// Token: 0x040029D7 RID: 10711
		Warning,
		// Token: 0x040029D8 RID: 10712
		Cancel,
		// Token: 0x040029D9 RID: 10713
		Extracting,
		// Token: 0x040029DA RID: 10714
		Complete,
		// Token: 0x040029DB RID: 10715
		Surplus,
		// Token: 0x040029DC RID: 10716
		TaxReturn
	}
}
