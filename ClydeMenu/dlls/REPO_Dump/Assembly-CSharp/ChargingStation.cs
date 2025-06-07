using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class ChargingStation : MonoBehaviour, IPunObservable
{
	// Token: 0x06000836 RID: 2102 RVA: 0x0004FDFA File Offset: 0x0004DFFA
	private void Awake()
	{
		ChargingStation.instance = this;
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0004FE04 File Offset: 0x0004E004
	private void Start()
	{
		this.chargeRate = 0.05f;
		foreach (object obj in this.crystalCylinder)
		{
			Transform transform = (Transform)obj;
			this.crystals.Add(transform);
		}
		this.chargingStationEmissionMaterial = this.meshObject.GetComponent<Renderer>().material;
		this.chargeBar = base.transform.Find("Charge");
		this.photonView = base.GetComponent<PhotonView>();
		this.chargeArea = base.transform.Find("Charge Area");
		this.lockedTransform = base.transform.Find("Locked");
		this.light1 = base.transform.Find("Light1").GetComponent<Light>();
		this.light2 = base.transform.Find("Light2").GetComponent<Light>();
		if (!SemiFunc.RunIsShop())
		{
			if (this.lockedTransform)
			{
				Object.Destroy(this.lockedTransform.gameObject);
			}
		}
		else
		{
			if (this.subtleLight)
			{
				Object.Destroy(this.subtleLight);
			}
			if (this.chargeArea)
			{
				Object.Destroy(this.chargeArea.gameObject);
			}
			if (this.chargeBar)
			{
				Object.Destroy(this.chargeBar.gameObject);
			}
			Object.Destroy(this.light1.gameObject);
			Object.Destroy(this.light2.gameObject);
		}
		this.chargeInt = SemiFunc.StatGetItemsPurchased("Item Power Crystal");
		int num = Mathf.RoundToInt((float)this.chargeInt * 10f / 6f * 10f);
		this.chargeTotal = StatsManager.instance.runStats["chargingStationChargeTotal"];
		if (this.chargeTotal > num)
		{
			this.chargeTotal = num;
		}
		StatsManager.instance.runStats["chargingStationChargeTotal"] = this.chargeTotal;
		this.chargeFloat = (float)this.chargeTotal / 100f;
		this.chargeSegmentCurrent = Mathf.RoundToInt(this.chargeFloat * (float)this.chargeSegments);
		this.chargeSegmentPrevious = this.chargeSegmentCurrent;
		if (this.chargeInt <= 0)
		{
			this.OutOfCrystalsShutdown();
		}
		this.chargeScale = this.chargeFloat;
		this.chargeScaleTarget = (float)this.chargeSegmentCurrent / (float)this.chargeSegments;
		if (this.chargeBar)
		{
			this.chargeBar.localScale = new Vector3(this.chargeScale, 1f, 1f);
		}
		if (this.chargeTotal <= 2)
		{
			this.chargeInt = 0;
			this.chargeTotal = 0;
			this.chargeFloat = 0f;
		}
		while (this.crystals.Count > this.chargeInt)
		{
			Object.Destroy(this.crystals[0].gameObject);
			this.crystals.RemoveAt(0);
			if (this.crystals.Count == 0)
			{
				this.OutOfCrystalsShutdown();
				break;
			}
		}
		if (RunManager.instance.levelsCompleted < 1)
		{
			Object.Destroy(base.gameObject);
		}
		base.StartCoroutine(this.MissionText());
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x00050140 File Offset: 0x0004E340
	private void OutOfCrystalsShutdown()
	{
		this.chargingStationEmissionMaterial.SetColor("_EmissionColor", Color.black);
		if (this.light1)
		{
			this.light1.enabled = false;
		}
		if (this.light2)
		{
			this.light2.enabled = false;
		}
		if (this.subtleLight)
		{
			Color color = new Color(0.1f, 0.1f, 0.2f);
			this.subtleLight.GetComponent<Light>().color = color;
		}
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x000501C8 File Offset: 0x0004E3C8
	public IEnumerator MissionText()
	{
		while (LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(2f);
		if (SemiFunc.RunIsLobby())
		{
			SemiFunc.UIFocusText("Enjoy the ride, recharge stuff and GEAR UP!", Color.white, AssetManager.instance.colorYellow, 3f);
		}
		yield break;
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x000501D0 File Offset: 0x0004E3D0
	private void StopCharge()
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("StopChargeRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.StopChargeRPC();
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x000501F6 File Offset: 0x0004E3F6
	[PunRPC]
	public void StopChargeRPC()
	{
		this.soundStop.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.isCharging = false;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0005022A File Offset: 0x0004E42A
	private void StartCharge()
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("StartChargeRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.StartChargeRPC();
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x00050250 File Offset: 0x0004E450
	[PunRPC]
	public void StartChargeRPC()
	{
		this.soundStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.isCharging = true;
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x00050284 File Offset: 0x0004E484
	private void ChargeAreaCheck()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (this.chargeFloat <= 0f)
		{
			if (this.isCharging)
			{
				this.isChargingPrev = this.isCharging;
				this.StopCharge();
			}
			return;
		}
		this.chargeAreaCheckTimer += Time.deltaTime;
		if (this.chargeAreaCheckTimer > 0.5f)
		{
			Collider[] array = Physics.OverlapBox(this.chargeArea.position, this.chargeArea.localScale / 2f, this.chargeArea.localRotation, SemiFunc.LayerMaskGetPhysGrabObject());
			this.itemsCharging.Clear();
			Collider[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ItemBattery componentInParent = array2[i].GetComponentInParent<ItemBattery>();
				if (componentInParent && componentInParent.batteryLifeInt < componentInParent.batteryBars && !this.itemsCharging.Contains(componentInParent))
				{
					this.itemsCharging.Add(componentInParent);
				}
			}
			this.chargeAreaCheckTimer = 0f;
		}
		bool flag = false;
		foreach (ItemBattery itemBattery in this.itemsCharging)
		{
			if (itemBattery.batteryLifeInt < itemBattery.batteryBars)
			{
				itemBattery.ChargeBattery(base.gameObject, 7.5f);
				this.chargeFloat = Mathf.Max(0f, this.chargeFloat - this.chargeRate * 0.5f * Time.deltaTime);
				this.chargeTotal = Mathf.Clamp((int)(this.chargeFloat * 100f), 0, 100);
				flag = true;
				if (!this.isCharging)
				{
					this.StartCharge();
					this.isChargingPrev = this.isCharging;
				}
			}
		}
		if (flag)
		{
			StatsManager.instance.runStats["chargingStationCharge"] = this.chargeInt;
			StatsManager.instance.runStats["chargingStationChargeTotal"] = this.chargeTotal;
		}
		if (!flag && this.isCharging)
		{
			this.isChargingPrev = this.isCharging;
			this.StopCharge();
		}
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0005049C File Offset: 0x0004E69C
	private void ChargingEffects()
	{
		if (this.isCharging)
		{
			TutorialDirector.instance.playerUsedChargingStation = true;
			this.crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, Mathf.PingPong(Time.time * 150f, 5f) - 2.5f);
			int num = 0;
			foreach (Transform transform in this.crystals)
			{
				if (transform)
				{
					num++;
					float value = 0.1f + Mathf.PingPong((Time.time + (float)num) * 5f, 1f);
					Color value2 = Color.yellow * Mathf.LinearToGammaSpace(value);
					transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", value2);
				}
			}
			this.crystalCooldown = 0f;
			return;
		}
		this.crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, 0f);
		foreach (Transform transform2 in this.crystals)
		{
			if (transform2)
			{
				this.crystalCooldown += Time.deltaTime * 0.5f;
				float num2 = this.chargeCurve.Evaluate(this.crystalCooldown);
				float value3 = Mathf.Lerp(1f, 0.1f, num2);
				Color value4 = Color.yellow * Mathf.LinearToGammaSpace(value3);
				transform2.GetComponent<Renderer>().material.SetColor("_EmissionColor", value4);
				this.crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, (Mathf.PingPong(Time.time * 250f, 10f) - 5f) * (1f - num2));
			}
		}
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x000506AC File Offset: 0x0004E8AC
	private void Update()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		this.chargeScaleTarget = (float)this.chargeSegmentCurrent / (float)this.chargeSegments;
		this.barLight.intensity = Mathf.Min(2.5f, this.chargeScaleTarget * 2.5f);
		this.soundLoop.PlayLoop(this.isCharging, 2f, 2f, 1f);
		this.AnimateChargeBar();
		this.ChargingEffects();
		if (this.isCharging && this.crystals.Count > 0)
		{
			float num = 0.5f + Mathf.PingPong(Time.time * 5f, 0.5f);
			Color value = Color.yellow * Mathf.LinearToGammaSpace(num);
			this.chargingStationEmissionMaterial.SetColor("_EmissionColor", value);
			if (this.light1 && this.light2)
			{
				this.light1.enabled = true;
				this.light2.enabled = true;
				this.light1.intensity = num;
				this.light2.intensity = num;
			}
		}
		else if (this.light1 && this.light2)
		{
			this.chargingStationEmissionMaterial.SetColor("_EmissionColor", Color.black);
			this.light1.enabled = false;
			this.light2.enabled = false;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (RunManager.instance.restarting)
		{
			return;
		}
		this.ChargeAreaCheck();
		this.chargeSegmentCurrent = Mathf.RoundToInt(this.chargeFloat * (float)this.chargeSegments);
		if (this.chargeSegmentCurrent < this.chargeSegmentPrevious)
		{
			this.UpdateChargeBar(this.chargeInt);
		}
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00050864 File Offset: 0x0004EA64
	private void AnimateChargeBar()
	{
		if (!this.chargeBar)
		{
			return;
		}
		if (Mathf.Approximately(this.chargeBar.localScale.x, this.chargeScaleTarget))
		{
			return;
		}
		this.chargeCurveTime += Time.deltaTime;
		this.chargeScale = Mathf.Lerp(this.chargeScale, this.chargeScaleTarget, this.chargeCurve.Evaluate(this.chargeCurveTime));
		this.chargeBar.localScale = new Vector3(this.chargeScale, 1f, 1f);
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x000508F8 File Offset: 0x0004EAF8
	private void UpdateChargeBar(int segmentPassed)
	{
		this.chargeInt = Mathf.CeilToInt((float)this.chargeTotal * 6f / 100f);
		if (this.chargeTotal <= 2)
		{
			this.chargeInt = 0;
			this.chargeTotal = 0;
			this.chargeFloat = 0f;
		}
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("UpdateChargeBarRPC", RpcTarget.All, new object[]
			{
				segmentPassed
			});
			return;
		}
		this.UpdateChargeBarRPC(segmentPassed);
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00050974 File Offset: 0x0004EB74
	private void DestroyCrystal()
	{
		if (this.crystals.Count < 1)
		{
			return;
		}
		Transform transform = this.crystals[0];
		Vector3 position = transform.position + transform.up * 0.1f;
		this.lightParticle.transform.position = position;
		this.fireflyParticles.transform.position = position;
		this.bitsParticles.transform.position = position;
		this.lightParticle.Play();
		this.fireflyParticles.Play();
		this.bitsParticles.Play();
		this.soundPowerCrystalBreak.Play(position, 1f, 1f, 1f, 1f);
		StatsManager.instance.runStats["chargingStationCharge"] = this.chargeInt;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StatsManager.instance.SetItemPurchase(this.item, StatsManager.instance.GetItemPurchased(this.item) - 1);
		}
		Object.Destroy(transform.gameObject);
		this.crystals.RemoveAt(0);
		if (this.crystals.Count == 0)
		{
			this.OutOfCrystalsShutdown();
		}
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x00050A9B File Offset: 0x0004EC9B
	[PunRPC]
	public void UpdateChargeBarRPC(int segmentPassed)
	{
		this.chargeSegmentPrevious = this.chargeSegmentCurrent;
		this.chargeSegmentCurrent = segmentPassed;
		this.chargeCurveTime = 0f;
		if (this.chargeInt < this.crystals.Count)
		{
			this.DestroyCrystal();
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00050AD4 File Offset: 0x0004ECD4
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			stream.SendNext(this.isCharging);
			return;
		}
		this.isCharging = (bool)stream.ReceiveNext();
	}

	// Token: 0x04000F27 RID: 3879
	public static ChargingStation instance;

	// Token: 0x04000F28 RID: 3880
	private PhotonView photonView;

	// Token: 0x04000F29 RID: 3881
	private Transform chargeBar;

	// Token: 0x04000F2A RID: 3882
	internal int chargeTotal = 100;

	// Token: 0x04000F2B RID: 3883
	private float chargeFloat = 1f;

	// Token: 0x04000F2C RID: 3884
	private float chargeScale = 1f;

	// Token: 0x04000F2D RID: 3885
	private float chargeScaleTarget = 1f;

	// Token: 0x04000F2E RID: 3886
	internal int chargeInt;

	// Token: 0x04000F2F RID: 3887
	private int chargeSegments = 20;

	// Token: 0x04000F30 RID: 3888
	private int chargeSegmentCurrent = 20;

	// Token: 0x04000F31 RID: 3889
	private int chargeSegmentPrevious = 20;

	// Token: 0x04000F32 RID: 3890
	private float chargeRate = 0.05f;

	// Token: 0x04000F33 RID: 3891
	public AnimationCurve chargeCurve;

	// Token: 0x04000F34 RID: 3892
	private float chargeCurveTime;

	// Token: 0x04000F35 RID: 3893
	private Transform chargeArea;

	// Token: 0x04000F36 RID: 3894
	private float chargeAreaCheckTimer;

	// Token: 0x04000F37 RID: 3895
	private List<ItemBattery> itemsCharging = new List<ItemBattery>();

	// Token: 0x04000F38 RID: 3896
	private Transform lockedTransform;

	// Token: 0x04000F39 RID: 3897
	public Light barLight;

	// Token: 0x04000F3A RID: 3898
	public GameObject meshObject;

	// Token: 0x04000F3B RID: 3899
	private Material chargingStationEmissionMaterial;

	// Token: 0x04000F3C RID: 3900
	private bool isCharging;

	// Token: 0x04000F3D RID: 3901
	private bool isChargingPrev;

	// Token: 0x04000F3E RID: 3902
	private Light light1;

	// Token: 0x04000F3F RID: 3903
	private Light light2;

	// Token: 0x04000F40 RID: 3904
	public Sound soundStart;

	// Token: 0x04000F41 RID: 3905
	public Sound soundStop;

	// Token: 0x04000F42 RID: 3906
	public Sound soundLoop;

	// Token: 0x04000F43 RID: 3907
	public Transform crystalCylinder;

	// Token: 0x04000F44 RID: 3908
	public List<Transform> crystals = new List<Transform>();

	// Token: 0x04000F45 RID: 3909
	public ParticleSystem lightParticle;

	// Token: 0x04000F46 RID: 3910
	public ParticleSystem fireflyParticles;

	// Token: 0x04000F47 RID: 3911
	public ParticleSystem bitsParticles;

	// Token: 0x04000F48 RID: 3912
	public Sound soundPowerCrystalBreak;

	// Token: 0x04000F49 RID: 3913
	private float crystalCooldown;

	// Token: 0x04000F4A RID: 3914
	public Item item;

	// Token: 0x04000F4B RID: 3915
	public GameObject subtleLight;
}
